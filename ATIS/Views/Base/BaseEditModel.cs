using System.Collections;
using System.ComponentModel;

using System.Runtime.CompilerServices;

namespace ATIS.Views.Base;

public abstract class BaseEditModel : INotifyPropertyChanged, INotifyDataErrorInfo
{
    // Fehlerliste pro Property
    protected readonly Dictionary<string, List<string>> _errors = new();

    // INotifyDataErrorInfo – gibt an, ob IRGENDEIN Fehler existiert
    public bool HasErrors => _errors.Any();

    // Prüft Fehler für ein bestimmtes Property
    public bool HasPropertyErrors(string propertyName)
    {
        return _errors.ContainsKey(propertyName) && _errors[propertyName].Count > 0;
    }

    // Fehler für ein bestimmtes Property zurückgeben
    public IEnumerable GetErrors(string? propertyName)
    {
        if (propertyName != null && _errors.ContainsKey(propertyName))
            return _errors[propertyName];

        return Enumerable.Empty<string>();
    }

    // Fehler hinzufügen
    protected void AddError(string propertyName, string error)
    {
        if (!_errors.ContainsKey(propertyName))
            _errors[propertyName] = new List<string>();

        _errors[propertyName].Add(error);
        OnErrorsChanged(propertyName);
    }

    // Fehler löschen
    protected void ClearErrors(string propertyName)
    {
        if (_errors.ContainsKey(propertyName))
        {
            _errors.Remove(propertyName);
            OnErrorsChanged(propertyName);
        }
    }

    // Event für Fehleränderungen
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    protected void OnErrorsChanged(string propertyName)
    {
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }

    // INotifyPropertyChanged
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

