using Avalonia.Controls;
using Avalonia.Controls.Templates;
using System;
using nxDumpFuse.ViewModels;

namespace nxDumpFuse
{
    public class ViewLocator : IDataTemplate
    {
        //private static ConsoleLogger _logger = new ConsoleLogger();
        public IControl Build(object data)
        {
            var name = data.GetType().FullName!.Replace("ViewModel", "");
            var type = Type.GetType(name);

            if (type != null)
            {
                return (Control)Activator.CreateInstance(type)!;
            }
            else
            {
                return new TextBlock { Text = "Not Found: " + name };
            }
        }

        public bool Match(object data)
        {
            return data is ViewModelBase;
        }
    }
}
