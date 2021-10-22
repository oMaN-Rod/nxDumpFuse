using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using nxDumpFuse.Interfaces;
using nxDumpFuse.ViewModels;

namespace nxDumpFuse.Services
{
    public class DialogService : IDialogService
    {
        private readonly IMainWindowProvider _mainWindowProvider;
        public DialogService(IMainWindowProvider mainWindowProvider)
        {
            _mainWindowProvider = mainWindowProvider;
        }
        public async Task<string> ShowOpenFileDialogAsync(string title, FileDialogFilter filter)
        {
            var openFileDialog = new OpenFileDialog()
            {
                Title = title,
                AllowMultiple = false
            };
            openFileDialog.Filters.Add(filter);

            var result = await openFileDialog.ShowAsync(_mainWindowProvider.GetMainWindow());
            return result.Length == 0 ? string.Empty : result[0];
        }

        public async Task<string> ShowOpenFolderDialogAsync(string title)
        {
            var openFolderDialog = new OpenFolderDialog()
            {
                Title = title
            };
            return await openFolderDialog.ShowAsync(_mainWindowProvider.GetMainWindow());
        }
    }
}
