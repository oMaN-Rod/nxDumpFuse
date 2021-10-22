using System.Threading.Tasks;
using Avalonia.Controls;
using nxDumpFuse.ViewModels;

namespace nxDumpFuse.Interfaces
{
    public interface IDialogService
    {
        Task<string> ShowOpenFileDialogAsync(string title, FileDialogFilter filter);

        Task<string> ShowOpenFolderDialogAsync(string title);
    }
}
