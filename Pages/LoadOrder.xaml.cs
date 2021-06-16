// --------------------------------------------------------------------------------------------------------------------
// Filename : LoadOrder.xaml.cs
// Project: DarkestLoadOrder / DarkestLoadOrder
// Author : Kristian Schlikow (kristian@schlikow.de)
// Created On : 09.06.2021 18:10
// Last Modified On : 09.06.2021 19:17
// Copyrights : Copyright (c) Kristian Schlikow 2021-2021, All Rights Reserved
// License: License is provided as described within the LICENSE file shipped with the project
// If present, the license takes precedence over the individual notice within this file
// --------------------------------------------------------------------------------------------------------------------

namespace DarkestLoadOrder.Pages
{
    using System;
    using System.Threading;
    using System.Windows.Data;

    using ModHelper;

    using Utility;

    using ViewModel;

    /// <summary>
    ///     Interaction logic for LoadOrder.xaml
    /// </summary>
    public partial class LoadOrder
    {
        public LoadOrder()
        {
            InitializeComponent();

            Loaded += delegate
            {
                SetFilter();
            };
        }

        private void SetFilter()
        {
            var viewAvailable = (CollectionView) CollectionViewSource.GetDefaultView(lbx_AvailableMods.ItemsSource);

            viewAvailable.Filter = FilterAvailable;
        }

        private bool FilterAvailable(object item)
        {
            var dataContext = (ModernApplicationViewModel) DataContext;

            if (dataContext == null)
                return false;

            return string.IsNullOrEmpty(dataContext.Application.SearchAvailable) || ((ObservableKeyValuePair<ulong, ModLocalItem>) item).Value.ModTitle.Contains(dataContext.Application.SearchAvailable, StringComparison.OrdinalIgnoreCase);
        }
    }
}
