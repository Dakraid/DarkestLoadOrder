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
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var files = Directory.GetFiles(@".\temp\");

            foreach (var file in files)
                File.Delete(file);

            var applicationView      = new ApplicationView();
            var applicationViewModel = new ApplicationViewModel();
            applicationView.DataContext = applicationViewModel;
            applicationView.Show();
        }
    }
}
