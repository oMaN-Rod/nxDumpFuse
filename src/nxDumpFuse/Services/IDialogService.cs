using System.Threading.Tasks;
using Avalonia.Controls;

namespace nxDumpFuse.Services
{
    public interface IDialogService
    {
        Task<string> ShowOpenFileDialogAsync(string title, FileDialogFilter filter);

        Task<string> ShowOpenFolderDialogAsync(string title);
    }
}
