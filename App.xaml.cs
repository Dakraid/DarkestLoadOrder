using System.Windows;

using DarkestLoadOrder.View;
using DarkestLoadOrder.ViewModel;

namespace DarkestLoadOrder
{
    using System.IO;

    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var files = Directory.GetFiles(@".\temp\");    
            foreach (var file in files)    
            {    
                File.Delete(file);
            }

            var applicationView      = new ApplicationView();
            var applicationViewModel = new ApplicationViewModel();
            applicationView.DataContext = applicationViewModel;
            applicationView.Show();
        }
    }
}
