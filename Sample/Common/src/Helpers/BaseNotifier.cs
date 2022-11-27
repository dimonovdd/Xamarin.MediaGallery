using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Sample.Common.Helpers;

public class BaseNotifier : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        => OnPropertiesChanged(propertyName);

    protected virtual void OnPropertiesChanged(params string[] propertiesNames)
    {
        if (propertiesNames?.Length > 0)
            foreach(var name in propertiesNames)
                OnPropertyChanged(new PropertyChangedEventArgs(name));
    }

    protected virtual void OnPropertyChanged(PropertyChangedEventArgs eventArgs)
        => MainThread.BeginInvokeOnMainThread(
                () => PropertyChanged?.Invoke(this, eventArgs));
}