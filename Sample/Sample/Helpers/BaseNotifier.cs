using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Essentials;
using Xamarin.Forms.Internals;

namespace Sample.Helpers
{
    public class BaseNotifier : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => OnPropertiesChanged(propertyName);

        protected virtual void OnPropertiesChanged(params string[] propertiesNames)
        {
            if (propertiesNames?.Length > 0)
                propertiesNames.ForEach(name => OnPropertyChanged(new PropertyChangedEventArgs(name)));
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs eventArgs)
            => MainThread.BeginInvokeOnMainThread(
                    () => PropertyChanged?.Invoke(this, eventArgs));
    }
}
