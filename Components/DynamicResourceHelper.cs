// --------------------------------------------------------------------------------------------------------------------
// Filename : DynamicResourceHelper.cs
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
    using System.Windows;

    public static class DynamicResourceHelper
    {
        private static void Apply(FrameworkElement fe, DependencyProperty dp, object key)
        {
            if (fe != null && dp != null && key != null)
                fe.SetResourceReference(dp, key);
        }

    #region Property

        public static readonly DependencyProperty PropertyProperty =
            DependencyProperty.RegisterAttached("Property",
                typeof(DependencyProperty),
                typeof(DynamicResourceHelper),
                new PropertyMetadata(OnPropertyChanged));

        public static DependencyProperty GetProperty(FrameworkElement element)
        {
            return (DependencyProperty) element.GetValue(PropertyProperty);
        }

        public static void SetProperty(FrameworkElement element, DependencyProperty value)
        {
            element.SetValue(PropertyProperty, value);
        }

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element  = (FrameworkElement) d;
            var newValue = (DependencyProperty) e.NewValue;
            Apply(element, newValue, GetResourceKey(element));
        }

    #endregion

    #region ResourceKey

        public static readonly DependencyProperty ResourceKeyProperty =
            DependencyProperty.RegisterAttached("ResourceKey",
                typeof(object),
                typeof(DynamicResourceHelper),
                new PropertyMetadata(OnResourceKeyChanged));

        public static object GetResourceKey(FrameworkElement element)
        {
            return element.GetValue(ResourceKeyProperty);
        }

        public static void SetResourceKey(FrameworkElement element, object value)
        {
            element.SetValue(ResourceKeyProperty, value);
        }

        private static void OnResourceKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element  = (FrameworkElement) d;
            var newValue = e.NewValue;
            Apply(element, GetProperty(element), newValue);
        }

    #endregion
    }
}
