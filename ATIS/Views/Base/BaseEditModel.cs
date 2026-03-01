using System.Collections;
using System.ComponentModel;

namespace ATIS.Views.Base
{
    public abstract class BaseEditModel : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        // -----------------------------
        // VALIDIERUNG
        // -----------------------------

        protected readonly Dictionary<string, List<string>> _errors = new();

        public bool HasErrors => _errors.Any();

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public IEnumerable GetErrors(string? propertyName)
        {
            if (propertyName != null && _errors.ContainsKey(propertyName))
                return _errors[propertyName];

            return Enumerable.Empty<string>();
        }

        protected void AddError(string propertyName, string error)
        {
            if (!_errors.ContainsKey(propertyName))
                _errors[propertyName] = new List<string>();

            if (!_errors[propertyName].Contains(error))
                _errors[propertyName].Add(error);

            RaiseErrorsChanged(propertyName);
        }

        protected void ClearErrors(string propertyName)
        {
            if (_errors.Remove(propertyName))
                RaiseErrorsChanged(propertyName);
        }

        private void RaiseErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            OnPropertyChanged(nameof(ErrorsString));
            OnPropertyChanged(nameof(HasErrors));
        }

        // -----------------------------
        // GESAMMELTE FEHLERMELDUNGEN
        // -----------------------------

        public string ErrorsString
        {
            get
            {
                if (!_errors.Any())
                    return string.Empty;

                return string.Join(Environment.NewLine,
                    _errors.SelectMany(kvp => kvp.Value));
            }
        }
    }
}
