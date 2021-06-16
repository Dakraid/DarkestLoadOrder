// --------------------------------------------------------------------------------------------------------------------
// Filename : CommonCommands.cs
// Project: DarkestLoadOrder / DarkestLoadOrder
// Author : Kristian Schlikow (kristian@schlikow.de)
// Created On : 09.06.2021 11:53
// Last Modified On : 09.06.2021 12:00
// Copyrights : Copyright (c) Kristian Schlikow 2021-2021, All Rights Reserved
// License: License is provided as described within the LICENSE file shipped with the project
// If present, the license takes precedence over the individual notice within this file
// --------------------------------------------------------------------------------------------------------------------

namespace DarkestLoadOrder.Components
{
    using System;
    using System.Windows;
    using System.Windows.Input;

    using ModernWpf;

    public static class CommonCommands
    {
        public static ICommand ToggleTheme { get; } = new ToggleThemeCommand();

        private class ToggleThemeCommand : ICommand
        {
            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                if (parameter is FrameworkElement fe)
                {
                    ThemeManager.SetRequestedTheme(fe, ThemeManager.GetActualTheme(fe) == ElementTheme.Dark ? ElementTheme.Light : ElementTheme.Dark);
                }
                else
                {
                    var tm = ThemeManager.Current;

                    tm.ApplicationTheme = tm.ActualApplicationTheme == ApplicationTheme.Dark ? ApplicationTheme.Light : ApplicationTheme.Dark;
                }
            }
        }
    }
}
