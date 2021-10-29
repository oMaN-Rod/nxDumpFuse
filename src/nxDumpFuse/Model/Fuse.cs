using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using nxDumpFuse.Events;
using nxDumpFuse.Model.Enums;

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
        private FileCase _fileCase;

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

        private void Update(int part, int parts, double progress, double progressPart)
        {
            OnFuseUpdate(new FuseUpdateInfo
            {
                Part = part,
                Parts = parts,
                Progress = progress,
                ProgressPart = progressPart
            });
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
            
            GetOutputFilePath();
            if (string.IsNullOrEmpty(_outputFilePath))
            {
                Log(FuseSimpleLogType.Error, "Output path was null");
                return;
            }

            var inputFiles = GetInputFiles();
            if (inputFiles.Count == 0)
            {
                Log(FuseSimpleLogType.Error, "No input files found");
                return;
            }
            
            FuseFiles(inputFiles);
        }

        private void GetOutputFilePath()
        {
            
            var fileName = Path.GetFileName(_inputFilePath);
            if (Path.HasExtension(fileName))
            {
                var ext = Path.GetExtension(fileName).Replace(".", string.Empty);
                var split = fileName.Split(".").ToList();

                if (int.TryParse(ext, out _) && split.Count >= 3 && split[^2] == XciExt) // .xci.00
                {
                    _outputFilePath = Path.Join(_outputDir, $"{string.Join("", split.Take(split.Count - 2))}.{XciExt}");
                    _fileCase = FileCase.XciNumeric;
                }
                else if (int.TryParse(ext, out _) && split.Count >= 3 && split[^2] == NspExt) // .nsp.00
                {
                    _outputFilePath = Path.Join(_outputDir, $"{string.Join("", split.Take(split.Count - 2))}.{NspExt}");
                    _fileCase = FileCase.NspNumeric;
                }
                else switch (ext[..2])
                {
                    // .xc0
                    case "xc" when int.TryParse(ext.Substring(ext.Length - 1, 1), out _):
                        _outputFilePath = Path.Join(_outputDir, $"{Path.GetFileNameWithoutExtension(fileName)}.{XciExt}");
                        _fileCase = FileCase.Xci;
                        break;
                    // .ns0
                    case "ns" when int.TryParse(ext.Substring(ext.Length - 1, 1), out _):
                        _outputFilePath = Path.Join(_outputDir, $"{Path.GetFileNameWithoutExtension(fileName)}.{NspExt}");
                        _fileCase = FileCase.Nsp;
                        break;
                }
            }
            else // dir/00
            {
                _fileCase = FileCase.Numeric;
                var inputDir = new FileInfo(_inputFilePath).Directory?.Name;
                if (string.IsNullOrEmpty(inputDir))
                {
                    inputDir = Path.GetPathRoot(_inputFilePath);
                    _outputFilePath = $"{inputDir}.{NspExt}";
                    return;
                }

                var inputDirSplit = inputDir.Split(".");
                _outputFilePath = Path.Join(_outputDir, inputDirSplit.Length == 1 
                    ? $"{inputDir}.{NspExt}" 
                    : $"{string.Join("", (inputDirSplit).Take(inputDirSplit.Length - 1))}.{NspExt}");
            }
        }

        private async void FuseFiles(IReadOnlyCollection<string> inputFiles)
        {
            var buffer = new byte[1024 * 1024];
            var count = 0;
            long totalBytes = 0;
            var totalFileLength = GetTotalFileSize(inputFiles);

            Log(FuseSimpleLogType.Information, $"Fusing {inputFiles.Count} parts to {_outputFilePath}  ({ToMb(totalFileLength)}MB)");
            
            await using var outputStream = File.Create(_outputFilePath);
            foreach (var inputFilePath in inputFiles)
            {
                if (_cts.Token.IsCancellationRequested) return;
                long currentBytes = 0;
                int currentBlockSize;

                await using var inputStream = File.OpenRead(inputFilePath);
                var fileLength = inputStream.Length;

                Log(FuseSimpleLogType.Information, $"Fusing file part {++count}-> {inputFilePath} ({ToMb(fileLength)}MB)");

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

                    var progress = totalBytes * 100.0 / totalFileLength;
                    var progressPart = currentBytes * 100.0 / fileLength;
                    Update(count, inputFiles.Count, progress, progressPart);
                }
            }

            Log(FuseSimpleLogType.Information, "Fuse Complete");
        }

        private static long ToMb(long bytes)
        {
            return bytes / 1000000;
        }

        private static long GetTotalFileSize(IEnumerable<string> inputFiles)
        {
            long totalFileSize = 0;
            inputFiles.Select(f => f).ToList().ForEach(f => totalFileSize += new FileInfo(f).Length);
            return totalFileSize;
        }

        private List<string> GetInputFiles()
        {
            var inputDir = Path.GetDirectoryName(_inputFilePath);
            if (string.IsNullOrEmpty(inputDir)) return new List<string>();
            var files = new List<string>();
            switch (_fileCase)
            {
                case FileCase.XciNumeric: // .xci.00
                case FileCase.NspNumeric: // .nsp.00
                    files = Directory.GetFiles(inputDir)
                        .Where(f => int.TryParse(Path.GetExtension(f).Replace(".", ""), out _))
                        .ToList();
                    break;
                case FileCase.Xci: // .xc0
                case FileCase.Nsp: // .ns0
                    files = Directory.GetFiles(inputDir, $"{Path.GetFileNameWithoutExtension(_inputFilePath)}*")
                        .ToList();
                    break;
                case FileCase.Numeric: // dir/00
                    files = Directory.GetFiles(inputDir)
                        .Where(f => int.TryParse(Path.GetFileName(f), out _))
                        .ToList();
                    break;
            }
            files.Sort();
            return files;
        }

        public void StopFuse()
        {
            _cts.Cancel();

            Log(FuseSimpleLogType.Information, "Fuse Stopped");

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
                            Update(0, 0, 0, 0);
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