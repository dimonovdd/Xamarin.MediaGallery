using System.ComponentModel;
using System.Runtime.CompilerServices;
using PropertyChanged;

namespace Sample.Helpers;

public class BaseNotifier : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => RaisePropertiesChanged(propertyName);

    [SuppressPropertyChangedWarnings]
    protected virtual void RaisePropertiesChanged(params string?[] propertiesNames)
    {
        if (propertiesNames?.Length > 0)
            foreach (var name in propertiesNames)
                OnPropertyChanged(new PropertyChangedEventArgs(name));
    }

    protected virtual void OnPropertyChanged(PropertyChangedEventArgs eventArgs)
        => MainThread.BeginInvokeOnMainThread(
            () => PropertyChanged?.Invoke(this, eventArgs));
}