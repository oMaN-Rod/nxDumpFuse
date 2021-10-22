using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private readonly List<string> _extensions = new() { "*" };
        private Fuse? _fuse;

        public FuseViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
            SelectInputFileCommand = ReactiveCommand.Create(SelectInputFile);
            SelectOutputFolderCommand = ReactiveCommand.Create(SelectOutputFolder);
            FuseCommand = ReactiveCommand.Create(FuseNxDump);
            StopCommand = ReactiveCommand.Create(StopDump);
            ProgressPartText = "Part 0/0";
        }

        public ReactiveCommand<Unit, Unit> SelectInputFileCommand { get; }

        public ReactiveCommand<Unit, Unit> SelectOutputFolderCommand { get; }

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

        private ObservableCollection<FuseSimpleLog> _logItems = new();
        public ObservableCollection<FuseSimpleLog> LogItems
        {
            get => _logItems;
            set => this.RaiseAndSetIfChanged( ref _logItems, value);
        }

        private async void SelectInputFile()
        {
            InputFilePath = await _dialogService.ShowOpenFileDialogAsync("Choose Input File", new FileDialogFilter { Name = string.Empty, Extensions = _extensions });
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
            try
            {
                _fuse.FuseDump();
            }
            catch (Exception e) {
                OnFuseSimpleLogEvent(new FuseSimpleLog(FuseSimpleLogType.Error, DateTime.Now, e.Message));
            }
        }

        private void StopDump()
        {
            _fuse?.StopFuse();
        }

        private void OnFuseUpdate(FuseUpdateInfo fuseUpdateInfo)
        {
            ProgressPart = fuseUpdateInfo.ProgressPart;
            Progress = fuseUpdateInfo.Progress;
            ProgressPartText = $"Part {fuseUpdateInfo.Part}/{fuseUpdateInfo.Parts}";
        }

        private void OnFuseSimpleLogEvent(FuseSimpleLog log)
        {
            LogItems.Add(log);
        }
    }
}
