namespace DarkestLoadOrder.View
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Threading;
    using System.Windows;

    using ModHelper;

    using Ookii.Dialogs.Wpf;

    using Serialization.Savegame;

    using Threading;

    /// <summary>
    ///     Interaction logic for ApplicationView.xaml
    /// </summary>
    public partial class ApplicationView : Window
    {
        public ApplicationView()
        {
            InitializeComponent();
        }

        private void wnd_MainWindow_Closing(object sender, CancelEventArgs e)
        {
            var dataContext = (ViewModel.ApplicationViewModel) DataContext;
            dataContext.CloseView();

            Application.Current.Shutdown();
        }

        private void btn_SelectSaveFolder_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new VistaFolderBrowserDialog
            {
                Description = "Please select your save folder.", UseDescriptionForTitle = true
            };

            // ReSharper disable once PossibleInvalidOperationException
            if (!(bool) dialog.ShowDialog())
                return;

            var dataContext = (ViewModel.ApplicationViewModel) DataContext;
            dataContext.Application.SaveFolderPath = dialog.SelectedPath;
        }

        private void btn_SelectModFolder_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new VistaFolderBrowserDialog
            {
                Description = "Please select your workshop folder.", UseDescriptionForTitle = true
            };
            
            // ReSharper disable once PossibleInvalidOperationException
            if (!(bool) dialog.ShowDialog())
                return;

            var dataContext = (ViewModel.ApplicationViewModel) DataContext;
            dataContext.Application.ModFolderPath = dialog.SelectedPath;
        }

        private void cbx_ProfileSelect_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var dataContext = (ViewModel.ApplicationViewModel) DataContext;
            var newSelect = e.AddedItems[0]?.ToString();

            if (string.IsNullOrWhiteSpace(newSelect))
                return;

            dataContext.Application.SelectedProfile = newSelect;

            var loadProfileTask   = new Tasks.LoadProfileTask(dataContext.Application.SaveFolderPath, newSelect);
            var loadProfileThread = new Thread(loadProfileTask.Execute);
            loadProfileThread.Start();

            while (loadProfileThread.IsAlive)
                Thread.Sleep(500);

            var saveData = SaveData.FromJson(File.ReadAllText(@".\savedata_out.json"));

            foreach (var ugcs in saveData.BaseRoot.AppliedUgcs1_0)
            {
                if (!ulong.TryParse(ugcs.Value.Name, out var modId))
                    continue;

                if (!dataContext.Application.LocalMods.TryGetValue(modId, out var modLocalItem))
                    continue;

                modLocalItem.ModPriority = long.Parse(ugcs.Key);
                modLocalItem.ModSource   = ugcs.Value.Source;

                dataContext.Application.ActiveMods.Add(modId, modLocalItem);
                dataContext.Application.LocalMods.Remove(modId);
            }
        }
    }
}
