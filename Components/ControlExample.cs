﻿// --------------------------------------------------------------------------------------------------------------------
// Filename : ControlExample.cs
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
    using System.Windows.Controls;
    using System.Windows.Markup;

    [ContentProperty(nameof(Example))]
    public class ControlExample : Control
    {
        static ControlExample()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ControlExample), new FrameworkPropertyMetadata(typeof(ControlExample)));
        }

    #region HeaderText

        public static readonly DependencyProperty HeaderTextProperty =
            DependencyProperty.Register(nameof(HeaderText),
                typeof(string),
                typeof(ControlExample),
                new PropertyMetadata(string.Empty));

        public string HeaderText
        {
            get => (string) GetValue(HeaderTextProperty);
            set => SetValue(HeaderTextProperty, value);
        }

    #endregion

    #region Example

        public static readonly DependencyProperty ExampleProperty =
            DependencyProperty.Register(nameof(Example),
                typeof(object),
                typeof(ControlExample),
                null);

        public object Example
        {
            get => GetValue(ExampleProperty);
            set => SetValue(ExampleProperty, value);
        }

    #endregion

    #region Options

        public static readonly DependencyProperty OptionsProperty =
            DependencyProperty.Register(nameof(Options),
                typeof(object),
                typeof(ControlExample),
                null);

        public object Options
        {
            get => GetValue(OptionsProperty);
            set => SetValue(OptionsProperty, value);
        }

    #endregion

    #region MaxContentWidth

        public static readonly DependencyProperty MaxContentWidthProperty =
            DependencyProperty.Register(nameof(MaxContentWidth),
                typeof(double),
                typeof(ControlExample),
                new PropertyMetadata(1028d));

        public double MaxContentWidth
        {
            get => (double) GetValue(MaxContentWidthProperty);
            set => SetValue(MaxContentWidthProperty, value);
        }

    #endregion
    }
}
