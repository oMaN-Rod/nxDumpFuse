using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive;
using Avalonia.Controls;
using nxDumpFuse.Interfaces;
using nxDumpFuse.Model;
using nxDumpFuse.Model.Enums;
using ReactiveUI;

namespace nxDumpFuse.ViewModels
{
    public class FuseViewModel : ViewModelBase, IFuseViewModel
    {
        private readonly IDialogService _dialogService;
        private Fuse? _fuse;
        private readonly Stopwatch _sw = new();
        private TimeSpan _elapsed;

        public FuseViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
            SelectInputFileCommand = ReactiveCommand.Create(SelectInputFile);
            SelectOutputFolderCommand = ReactiveCommand.Create(SelectOutputFolder);
            FuseCommand = ReactiveCommand.Create(FuseNxDump);
            StopCommand = ReactiveCommand.Create(StopDump);
            ClearLogCommand = ReactiveCommand.Create(ClearLog);
            ProgressPartText = "Part 0/0";
        }

        public ReactiveCommand<Unit, Unit> SelectInputFileCommand { get; }

        public ReactiveCommand<Unit, Unit> SelectOutputFolderCommand { get; }

        public ReactiveCommand<Unit, Unit> ClearLogCommand { get; }

        public ReactiveCommand<Unit, Unit> FuseCommand { get; }

        public ReactiveCommand<Unit, Unit> StopCommand { get; }

        private string _inputFilePath = string.Empty;
        public string InputFilePath
        {
            get => _inputFilePath;
            set => this.RaiseAndSetIfChanged(ref _inputFilePath, value);
        }

        private string _outputDir = string.Empty;
        public string OutputDir
        {
            get => _outputDir;
            set => this.RaiseAndSetIfChanged(ref _outputDir, value);
        }

        private string _progressPartText = string.Empty;
        public string ProgressPartText
        {
            get => _progressPartText;
            set => this.RaiseAndSetIfChanged(ref _progressPartText, value);
        }

        private double _progressPart;
        public double ProgressPart
        {
            get => _progressPart;
            set => this.RaiseAndSetIfChanged(ref _progressPart, value);
        }

        private double _progress;
        public double Progress
        {
            get => _progress;
            set => this.RaiseAndSetIfChanged(ref _progress, value);
        }

        private string _progressText = string.Empty;
        public string ProgressText
        {
            get => _progressText;
            set => this.RaiseAndSetIfChanged(ref _progressText, value);
        }

        private ObservableCollection<FuseSimpleLog> _logItems = new();
        public ObservableCollection<FuseSimpleLog> LogItems
        {
            get => _logItems;
            set => this.RaiseAndSetIfChanged( ref _logItems, value);
        }

        private async void SelectInputFile()
        {
            InputFilePath = await _dialogService.ShowOpenFileDialogAsync("Choose Input File", new FileDialogFilter { Name = string.Empty, Extensions = new List<string>() });
        }

        private async void SelectOutputFolder()
        {
            OutputDir = await _dialogService.ShowOpenFolderDialogAsync("Choose Output Folder");
        }

        private void FuseNxDump()
        {
            _fuse = new Fuse(InputFilePath, OutputDir);
            _fuse.FuseUpdateEvent += OnFuseUpdate;
            _fuse.FuseSimpleLogEvent += OnFuseSimpleLogEvent;
            _sw.Start();
            try
            {
                _fuse.Start();
            }
            catch (Exception e) {
                _sw.Stop();
                OnFuseSimpleLogEvent(new FuseSimpleLog(FuseSimpleLogType.Error, DateTime.Now, e.Message));
            }
        }

        private void StopDump()
        {
            _sw.Stop();
            _fuse?.Stop();
            ProgressText = string.Empty;
        }

        private void ClearLog()
        {
            LogItems.Clear();
        }

        private void OnFuseUpdate(FuseUpdateInfo fuseUpdateInfo)
        {
            if (fuseUpdateInfo.Complete)
            {
                _sw.Stop();
                ProgressText = string.Empty;
                return;
            }
            ProgressPart = fuseUpdateInfo.ProgressPart;
            ProgressPartText = $"Part {fuseUpdateInfo.Part}/{fuseUpdateInfo.Parts}";
            Progress = fuseUpdateInfo.Progress;

            if (!(_sw.Elapsed.TotalSeconds >= 0.5 &&
                  _sw.Elapsed.TotalSeconds - _elapsed.TotalSeconds >= 0.5)) return;
            _elapsed = _sw.Elapsed;
            ProgressText = $"({fuseUpdateInfo.Speed:0}MB/s) {Progress:0}% ";
        }

        private void OnFuseSimpleLogEvent(FuseSimpleLog log)
        {
            LogItems.Add(log);
        }
    }
}
