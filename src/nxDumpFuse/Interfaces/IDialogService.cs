using System.Threading.Tasks;
using Avalonia.Controls;

namespace nxDumpFuse.Interfaces
{
    public interface IDialogService
    {
        Task<string> ShowOpenFileDialogAsync(string title, FileDialogFilter filter);

        Task<string> ShowOpenFolderDialogAsync(string title);
    }
}
