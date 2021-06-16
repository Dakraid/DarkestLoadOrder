// --------------------------------------------------------------------------------------------------------------------
// Filename : FontOverrides.cs
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
    using System.Windows.Media;

    public class FontOverrides : ResourceDictionary
    {
        private static readonly object[] s_keys =
        {
            SystemFonts.MessageFontFamilyKey, "ContentControlThemeFontFamily", "PivotHeaderItemFontFamily", "PivotTitleFontFamily"
        };

        private FontFamily _fontFamily;

        public FontFamily FontFamily
        {
            get => _fontFamily;

            set
            {
                if (_fontFamily == value)
                    return;
                
                _fontFamily = value;

                if (_fontFamily != null)
                    foreach (var key in s_keys)
                        this[key] = _fontFamily;
                else
                    foreach (var key in s_keys)
                        Remove(key);
            }
        }
    }
}
