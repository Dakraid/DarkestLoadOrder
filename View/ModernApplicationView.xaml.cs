// --------------------------------------------------------------------------------------------------------------------
// Filename : ModernApplicationView.xaml.cs
// Project: DarkestLoadOrder / DarkestLoadOrder
// Author : Kristian Schlikow (kristian@schlikow.de)
// Created On : 09.06.2021 11:19
// Last Modified On : 09.06.2021 14:44
// Copyrights : Copyright (c) Kristian Schlikow 2021-2021, All Rights Reserved
// License: License is provided as described within the LICENSE file shipped with the project
// If present, the license takes precedence over the individual notice within this file
// --------------------------------------------------------------------------------------------------------------------

namespace DarkestLoadOrder.View
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Navigation;

    using ModernWpf.Controls;
    using ModernWpf.Navigation;

    using Pages;

    using ViewModel;

    /// <summary>
    ///     Interaction logic for ModernApplicationView.xaml
    /// </summary>
    public partial class ModernApplicationView : Window
    {
        public ModernApplicationView()
        {
            InitializeComponent();
            
            var dataContext = (ModernApplicationViewModel) DataContext;

            NavView.SelectedItem = NavView.MenuItems.OfType<NavigationViewItem>().First();
            Navigate(NavView.SelectedItem);

            Loaded += delegate
            {
                UpdateAppTitle();
            };

            
            Closing += dataContext.OnWindowClosing;
        }

        private void UpdateAppTitle()
        {
            //ensure the custom title bar does not overlap window caption controls
            var currMargin = AppTitleBar.Margin;
            AppTitleBar.Margin = new Thickness(currMargin.Left, currMargin.Top, TitleBar.GetSystemOverlayRightInset(this), currMargin.Bottom);
        }

        private void NavView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            ContentFrame.GoBack();
        }

        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
                Navigate(typeof(SettingsPage));
            else
                Navigate(args.InvokedItemContainer);
        }

        private void NavView_PaneOpening(NavigationView sender, object args)
        {
            UpdateAppTitleMargin(sender);
        }

        private void NavView_PaneClosing(NavigationView sender, NavigationViewPaneClosingEventArgs args)
        {
            UpdateAppTitleMargin(sender);
        }

        private void NavView_DisplayModeChanged(NavigationView sender, NavigationViewDisplayModeChangedEventArgs args)
        {
            var currMargin = AppTitleBar.Margin;

            AppTitleBar.Margin = sender.DisplayMode == NavigationViewDisplayMode.Minimal ? new Thickness(sender.CompactPaneLength * 2, currMargin.Top, currMargin.Right, currMargin.Bottom) : new Thickness(sender.CompactPaneLength, currMargin.Top, currMargin.Right, currMargin.Bottom);

            UpdateAppTitleMargin(sender);
        }

        private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            NavView.SelectedItem = e.SourcePageType() == typeof(SettingsPage) ? NavView.SettingsItem : NavView.MenuItems.OfType<NavigationViewItem>().FirstOrDefault(x => GetPageType(x) == e.SourcePageType());
        }

        private void UpdateAppTitleMargin(NavigationView sender)
        {
            const int smallLeftIndent = 4, largeLeftIndent = 24;

            var currMargin = AppTitle.Margin;

            if (sender.DisplayMode == NavigationViewDisplayMode.Expanded && sender.IsPaneOpen ||
                sender.DisplayMode == NavigationViewDisplayMode.Minimal)
                AppTitle.Margin = new Thickness(smallLeftIndent, currMargin.Top, currMargin.Right, currMargin.Bottom);
            else
                AppTitle.Margin = new Thickness(largeLeftIndent, currMargin.Top, currMargin.Right, currMargin.Bottom);
        }

        private void Navigate(object item)
        {
            if (item is not NavigationViewItem menuItem)
                return;

            var pageType = GetPageType(menuItem);

            if (ContentFrame.CurrentSourcePageType != pageType)
                ContentFrame.Navigate(pageType);

            ContentFrame.DataContext = DataContext;
        }

        private void Navigate(Type sourcePageType)
        {
            if (ContentFrame.CurrentSourcePageType != sourcePageType)
                ContentFrame.Navigate(sourcePageType);
        }

        private static Type GetPageType(FrameworkElement item)
        {
            return item.Tag as Type;
        }
    }
}
