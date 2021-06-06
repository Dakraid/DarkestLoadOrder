// --------------------------------------------------------------------------------------------------------------------
// Filename : ValueConverters.cs
// Project: DarkestLoadOrder / DarkestLoadOrder
// Author : Kristian Schlikow (kristian@schlikow.de)
// Created On : 02.06.2021 12:05
// Last Modified On : 06.06.2021 14:38
// Copyrights : Copyright (c) Kristian Schlikow 2021-2021, All Rights Reserved
// License: License is provided as described within the LICENSE file shipped with the project
// If present, the license takes precedence over the individual notice within this file
// --------------------------------------------------------------------------------------------------------------------

namespace DarkestLoadOrder.Utility
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Media;

    public class ListItemToPositionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not ListBoxItem item)
                return null;

            var lb = FindAncestor<ListBox>(item);

            if (lb == null)
                return null;

            var index = lb.Items.IndexOf(item.Content);

            return index;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static T FindAncestor<T>(DependencyObject from) where T : class
        {
            while (true)
                switch (from)
                {
                    case null:
                        return null;

                    case T candidate:
                        return candidate;

                    default:
                        from = VisualTreeHelper.GetParent(from);

                        break;
                }
        }
    }
}
