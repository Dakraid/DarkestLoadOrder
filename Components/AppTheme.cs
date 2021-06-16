// --------------------------------------------------------------------------------------------------------------------
// Filename : AppTheme.cs
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
    using System.Collections.Generic;
    using System.Globalization;
    using System.Windows.Data;

    using ModernWpf;

    public class AppThemes : List<AppTheme>
    {
        public AppThemes()
        {
            Add(AppTheme.Light);
            Add(AppTheme.Dark);
            Add(AppTheme.Default);
        }
    }

    public class AppTheme
    {
        private AppTheme(string name, ApplicationTheme? value)
        {
            Name  = name;
            Value = value;
        }

        public static AppTheme Light { get; } = new("Light", ApplicationTheme.Light);
        public static AppTheme Dark { get; } = new("Dark", ApplicationTheme.Dark);
        public static AppTheme Default { get; } = new("Use system setting", null);

        public string Name { get; }
        public ApplicationTheme? Value { get; }

        public override string ToString()
        {
            return Name;
        }
    }

    public class AppThemeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                ApplicationTheme.Light => AppTheme.Light,
                ApplicationTheme.Dark  => AppTheme.Dark,
                _                      => AppTheme.Default
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is AppTheme appTheme)
                return appTheme.Value;

            return AppTheme.Default;
        }
    }
}
