using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using nxDumpFuse.Events;

namespace nxDumpFuse.Model
{
    public class Fuse
    {
        private const string XciExt = "xci";
        private const string NspExt = "nsp";
        private readonly CancellationTokenSource _cts;
        private readonly string _inputFilePath;
        private readonly string _outputDir;
        private string _outputFilePath = string.Empty;

        public Fuse(string inputFilePath, string outputDir)
        {
            _inputFilePath = inputFilePath;
            _outputDir = outputDir;
            _cts = new CancellationTokenSource();
        }

        public event EventHandlers.FuseUpdateEventHandler? FuseUpdateEvent;

        public event EventHandlers.FuseSimpleLogEventHandler? FuseSimpleLogEvent;

        protected virtual void OnFuseUpdate(FuseUpdateInfo fuseUpdateInfo)
        {
            FuseUpdateEvent?.Invoke(fuseUpdateInfo);
        }

        protected virtual void OnFuseSimpleLogEvent(FuseSimpleLog log)
        {
            FuseSimpleLogEvent?.Invoke(log);
        }

        private void Log(FuseSimpleLogType type, string message)
        {
            OnFuseSimpleLogEvent(new FuseSimpleLog(type, DateTime.Now, message));
        }

        public void FuseDump()
        {
            if (string.IsNullOrEmpty(_inputFilePath))
            {
                Log(FuseSimpleLogType.Error, "Input File cannot be empty");
                return;
            }

            if (string.IsNullOrEmpty(_outputDir))
            {
                Log(FuseSimpleLogType.Error, "Output Directory cannot be empty");
                return;
            }
            Log(FuseSimpleLogType.Information, "Fuse Started");
            GetOutputFilePath(_inputFilePath, _outputDir);
            if (string.IsNullOrEmpty(_outputFilePath))
            {
                Log(FuseSimpleLogType.Error, "Output path was null");
                return;
            }
            var inputFiles = GetInputFiles();
            if (inputFiles.Length == 0)
            {
                Log(FuseSimpleLogType.Error, "No input files found");
                return;
            }
            FuseFiles(inputFiles, _outputFilePath);
        }

        private void GetOutputFilePath(string inputFilePath, string outputDir)
        {
            
            string? fileName = Path.GetFileName(inputFilePath);
            if (Path.HasExtension(fileName))
            {
                string ext = Path.GetExtension(fileName).Replace(".", string.Empty);
                List<string> split = fileName.Split(".").ToList();

                if (int.TryParse(ext, out _) && split.Count >= 3 && split[^2] == XciExt) // .xci.00
                {
                    _outputFilePath = Path.Join(outputDir, $"{string.Join("", split.Take(split.Count - 2))}.{XciExt}");
                }
                else if (int.TryParse(ext, out _) && split.Count >= 3 && split[^2] == NspExt) // .nsp.00
                    _outputFilePath = Path.Join(outputDir, $"{string.Join("", split.Take(split.Count - 2))}.{NspExt}");
                else switch (ext.Substring(0, 2))
                {
                    // .xc0
                    case "xc" when int.TryParse(ext.Substring(ext.Length - 1, 1), out _):
                        _outputFilePath = Path.Join(outputDir, $"{Path.GetFileNameWithoutExtension(fileName)}.{XciExt}");
                        break;
                    // .ns0
                    case "ns" when int.TryParse(ext.Substring(ext.Length - 1, 1), out _):
                        _outputFilePath = Path.Join(outputDir, $"{Path.GetFileNameWithoutExtension(fileName)}.{NspExt}");
                        break;
                }
            }
            else // dir/00
            {
                var inputDir = new FileInfo(inputFilePath).Directory?.Name;
                if (string.IsNullOrEmpty(inputDir))
                {
                    inputDir = Path.GetPathRoot(_inputFilePath);
                    _outputFilePath = $"{inputDir}.{NspExt}";
                    return;
                }
                var inputDirSplit = inputDir.Split(".");
                _outputFilePath = Path.Join(outputDir, inputDirSplit.Length == 1 
                    ? $"{inputDir}.{NspExt}" 
                    : $"{string.Join("", (inputDirSplit).Take(inputDirSplit.Length - 1))}.{NspExt}");
            }
        }

        private async void FuseFiles(string[] inputFiles, string outputFilePath)
        {
            var buffer = new byte[1024 * 1024];
            var count = 0;
            long totalBytes = 0;
            var totalFileLength = GetTotalFileSize(inputFiles);
            Log(FuseSimpleLogType.Information, $"Fusing {inputFiles.Length} parts to {outputFilePath}:{totalFileLength / 1000}kB");
            await using var outputStream = File.Create(outputFilePath);
            foreach (var inputFilePath in inputFiles)
            {
                if (_cts.Token.IsCancellationRequested) return;
                ++count;
                await using var inputStream = File.OpenRead(inputFilePath);
                Log(FuseSimpleLogType.Information, $"Fusing file part {count} --> {inputFilePath}/{inputStream.Length / 1000}kB");
                var fileLength = inputStream.Length;
                long currentBytes = 0;
                int currentBlockSize;

                while ((currentBlockSize = inputStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    if (_cts.Token.IsCancellationRequested) return;
                    currentBytes += currentBlockSize;
                    totalBytes += currentBlockSize;

                    try
                    {
                        await outputStream.WriteAsync(buffer, 0, currentBlockSize, _cts.Token);
                    }
                    catch (TaskCanceledException e)
                    {
                        Log(FuseSimpleLogType.Error, e.Message);
                    }

                    OnFuseUpdate(new FuseUpdateInfo
                    {
                        Part = count,
                        Parts = inputFiles.Length,
                        ProgressPart = currentBytes * 100.0 / fileLength,
                        Progress = totalBytes * 100.0 / totalFileLength
                    });
                }
            }

            Log(FuseSimpleLogType.Information, "Fuse Complete");
        }

        private static long GetTotalFileSize(string[] inputFiles)
        {
            long totalFileSize = 0;
            inputFiles.Select(f => f).ToList().ForEach(f => totalFileSize += new FileInfo(f).Length);
            return totalFileSize;
        }

        private string[] GetInputFiles()
        {
            var inputDir = Path.GetDirectoryName(_inputFilePath);
            if (string.IsNullOrEmpty(inputDir)) inputDir = Path.GetPathRoot(_inputFilePath);
            return inputDir != null ? Directory.GetFiles(inputDir) : new string[] { };
        }

        public void StopFuse()
        {
            _cts.Cancel();
            Log(FuseSimpleLogType.Information, "Fuse Stopped");
            OnFuseUpdate(new FuseUpdateInfo
            {
                Part = 0,
                Parts = 0,
                ProgressPart = 0,
                Progress = 0
            });
            if (File.Exists(_outputFilePath))
            {
                Task.Run((() =>
                {
                    const int retries = 5;
                    for (var i = 0; i <= retries; i++)
                    {
                        try
                        {
                            File.Delete(_outputFilePath);
                            Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() => Log(FuseSimpleLogType.Information, $"Deleted {_outputFilePath}"));
                            break;
                        }
                        catch (IOException)
                        {
                            Thread.Sleep(1000);
                        }
                    }
                }));
            }
        }
    }
}