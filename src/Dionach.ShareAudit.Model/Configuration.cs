using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Dionach.ShareAudit.Model
{
    public class Configuration : INotifyPropertyChanged
    {
        private Credentials _credentials = new Credentials();
        private bool _disablePortScan = false;
        private bool _disableReverseDnsLookup = false;
        private bool _isReadOnly = false;
        private string _scope;
        private bool _useAlternateAuthenticationMethod = true;
        private bool _useVerbatimScope = false;
        private bool _enableReadOnly = false;
        private bool _enableWriteOnly = false;
        private bool _enableSharesOnly = false;
        public event PropertyChangedEventHandler PropertyChanged;

        public Credentials Credentials
        {
            get => _credentials;
            set
            {
                if (ReferenceEquals(value, _credentials))
                {
                    return;
                }

                _credentials = value;
                OnPropertyChanged();
            }
        }

        public bool EnableSharesOnly
        {
            get => _enableSharesOnly;
            set
            {
                if (value == _enableSharesOnly)
                {
                    return;
                }

                _enableSharesOnly = value;
                OnPropertyChanged();
            }
        }

        public bool EnableReadOnly
        {
            get => _enableReadOnly;
            set
            {
                if (value == _enableReadOnly)
                {
                    return;
                }

                _enableReadOnly = value;
                OnPropertyChanged();
            }
        }

        public bool EnableWriteOnly
        {
            get => _enableWriteOnly;
            set
            {
                if (value == _enableWriteOnly)
                {
                    return;
                }

                _enableWriteOnly = value;
                OnPropertyChanged();
            }
        }

        public bool DisablePortScan
        {
            get => _disablePortScan;
            set
            {
                if (value == _disablePortScan)
                {
                    return;
                }

                _disablePortScan = value;
                OnPropertyChanged();
            }
        }

        public bool DisableReverseDnsLookup
        {
            get => _disableReverseDnsLookup;
            set
            {
                if (value == _disableReverseDnsLookup)
                {
                    return;
                }

                _disableReverseDnsLookup = value;
                OnPropertyChanged();
            }
        }

        public bool IsReadOnly
        {
            get => _isReadOnly;
            set
            {
                if (value == _isReadOnly)
                {
                    return;
                }

                _isReadOnly = value;
                OnPropertyChanged();
            }
        }

        public string Scope
        {
            get => _scope;
            set
            {
                if (value == _scope)
                {
                    return;
                }

                _scope = value;
                OnPropertyChanged();
            }
        }

        public bool UseAlternateAuthenticationMethod
        {
            get => _useAlternateAuthenticationMethod;
            set
            {
                if (value == _useAlternateAuthenticationMethod)
                {
                    return;
                }

                _useAlternateAuthenticationMethod = value;
                OnPropertyChanged();
            }
        }

        public bool UseVerbatimScope
        {
            get => _useVerbatimScope;
            set
            {
                if (value == _useVerbatimScope)
                {
                    return;
                }

                _useVerbatimScope = value;
                OnPropertyChanged();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
