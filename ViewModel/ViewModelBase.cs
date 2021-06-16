// --------------------------------------------------------------------------------------------------------------------
// Filename : ViewModelBase.cs
// Project: DarkestLoadOrder / DarkestLoadOrder
// Author : Kristian Schlikow (kristian@schlikow.de)
// Created On : 31.05.2021 23:53
// Last Modified On : 10.06.2021 13:26
// Copyrights : Copyright (c) Kristian Schlikow 2021-2021, All Rights Reserved
// License: License is provided as described within the LICENSE file shipped with the project
// If present, the license takes precedence over the individual notice within this file
// --------------------------------------------------------------------------------------------------------------------

namespace DarkestLoadOrder.ViewModel
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Windows.Input;

    using JetBrains.Annotations;

    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public class RelayCommand : ICommand
        {
            private readonly Predicate<object> _canExecute;
            private readonly Action<object> _execute;

            public RelayCommand(Action<object> execute) : this(execute, null) { }

            public RelayCommand(Action<object> execute, Predicate<object> canExecute)
            {
                _execute    = execute ?? throw new ArgumentNullException(nameof(execute));
                _canExecute = canExecute;
            }

            [DebuggerStepThrough]
            public bool CanExecute(object parameter)
            {
                return _canExecute?.Invoke(parameter) ?? true;
            }

            public event EventHandler CanExecuteChanged
            {
                add => CommandManager.RequerySuggested += value;
                remove => CommandManager.RequerySuggested -= value;
            }

            public void Execute(object parameter)
            {
                _execute(parameter);
            }
        }
    }
}
