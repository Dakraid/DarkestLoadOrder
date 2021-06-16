// --------------------------------------------------------------------------------------------------------------------
// Filename : LayoutDensitySelector.xaml.cs
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
    using System.Windows.Controls;

    public partial class LayoutDensitySelector : UserControl
    {
        private ResourceDictionary _compactResources;

        public LayoutDensitySelector()
        {
            InitializeComponent();
        }

        private void Standard_Checked(object sender, RoutedEventArgs e)
        {
            if (_compactResources != null)
            {
                TargetElement?.Resources.MergedDictionaries.Remove(_compactResources);
                _compactResources = null;
            }
        }

        private void Compact_Checked(object sender, RoutedEventArgs e)
        {
            if (_compactResources != null)
                return;

            _compactResources = new ResourceDictionary
            {
                Source = new Uri("/ModernWpf;component/DensityStyles/Compact.xaml", UriKind.Relative)
            };

            TargetElement?.Resources.MergedDictionaries.Add(_compactResources);
        }

    #region TargetElement

        public static readonly DependencyProperty TargetElementProperty =
            DependencyProperty.Register(nameof(TargetElement),
                typeof(FrameworkElement),
                typeof(LayoutDensitySelector),
                null);

        public FrameworkElement TargetElement
        {
            get => (FrameworkElement) GetValue(TargetElementProperty);
            set => SetValue(TargetElementProperty, value);
        }

    #endregion
    }
}
