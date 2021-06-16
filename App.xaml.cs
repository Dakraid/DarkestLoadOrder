// --------------------------------------------------------------------------------------------------------------------
// Filename : App.xaml.cs
// Project: DarkestLoadOrder / DarkestLoadOrder
// Author : Kristian Schlikow (kristian@schlikow.de)
// Created On : 24.05.2021 19:58
// Last Modified On : 06.06.2021 14:38
// Copyrights : Copyright (c) Kristian Schlikow 2021-2021, All Rights Reserved
// License: License is provided as described within the LICENSE file shipped with the project
// If present, the license takes precedence over the individual notice within this file
// --------------------------------------------------------------------------------------------------------------------

namespace DarkestLoadOrder
{
    using System.IO;
    using System.Windows;

    using View;

    using ViewModel;

    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const string tempDir = @".\temp\";

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (Directory.Exists(tempDir))
            {
                var files = Directory.GetFiles(tempDir);

                foreach (var file in files)
                    File.Delete(file);
            }
            else
            {
                Directory.CreateDirectory(tempDir);
            }

            var model = new ModernApplicationViewModel();

            var view = new ModernApplicationView
            {
                DataContext = model
            };

            view.Show();
            //var applicationView      = new ApplicationView();
            //var applicationViewModel = new ApplicationViewModel();
            //applicationView.DataContext = applicationViewModel;
            //applicationView.Show();
        }
    }
}
