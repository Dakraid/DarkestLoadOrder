// --------------------------------------------------------------------------------------------------------------------
// Filename : ObservableKeyValuePair.cs
// Project: DarkestLoadOrder / DarkestLoadOrder
// Author : Kristian Schlikow (kristian@schlikow.de)
// Created On : 31.05.2021 00:13
// Last Modified On : 06.06.2021 14:38
// Copyrights : Copyright (c) Kristian Schlikow 2021-2021, All Rights Reserved
// License: License is provided as described within the LICENSE file shipped with the project
// If present, the license takes precedence over the individual notice within this file
// --------------------------------------------------------------------------------------------------------------------

namespace DarkestLoadOrder.Utility
{
    using System;
    using System.ComponentModel;

    [Serializable]
    public sealed class ObservableKeyValuePair<TKey, TValue> : INotifyPropertyChanged
    {
        private TKey _key;
        private TValue _value;

        public ObservableKeyValuePair() { }

        public ObservableKeyValuePair(TKey key, TValue value)
        {
            _key   = key;
            _value = value;
        }

        public TKey Key
        {
            get => _key;

            set
            {
                _key = value;
                OnPropertyChanged("Key");
            }
        }

        public TValue Value
        {
            get => _value;

            set
            {
                _value = value;
                OnPropertyChanged("Value");
            }
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string name)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
