
using nxDumpFuse.Interfaces;

namespace nxDumpFuse.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, IMainWindowViewModel
    {
        public IFuseViewModel FuseViewModel { get; }
        public IAboutViewModel AboutViewModel { get; }
        public IMainWindowViewModel MainViewModel { get; }

        public MainWindowViewModel(IFuseViewModel fuseViewModel, IAboutViewModel aboutViewModel)
        {
            FuseViewModel = fuseViewModel;
            AboutViewModel = aboutViewModel;
            MainViewModel = this;
        }
    }
}
