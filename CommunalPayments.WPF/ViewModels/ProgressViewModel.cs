using CommunalPayments.Common.Interfaces;
using GalaSoft.MvvmLight;
using MvvmDialogs;
using System.ComponentModel;
using System.Threading.Tasks;

namespace CommunalPayments.WPF.ViewModels
{
    class ProgressViewModel : ViewModelBase, IModalDialogViewModel
    {
        private bool? dialogResult;
        private readonly IImporter _importer;
        private int currentProgress;
        private string currentUrl;
        public bool? DialogResult
        {
            get => dialogResult;
            private set => Set(nameof(DialogResult), ref dialogResult, value);
        }
        public ProgressViewModel(string login, string password, int userId, IImporter importer)
        {           
            if(!string.IsNullOrWhiteSpace(login) && !string.IsNullOrWhiteSpace(password))
            {
                _importer = importer;
                _importer.ImportProgressChanged += _netRepository_ImportProgressChanged;
                _ = this.Start(userId, login, password);
            }            
        }

        public async Task<bool> Start(int userId, string login, string password)
        {
            bool retVal = await _importer.Import(login, password, userId);
            DialogResult = true;
            return retVal;
        }

        private void _netRepository_ImportProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            CurrentProgress = e.ProgressPercentage;
            CurrentUrl = (string)e.UserState;
        }

        public int CurrentProgress
        {
            get { return currentProgress; }
            private set
            {
                if (currentProgress != value)
                {
                    currentProgress = value;
                    RaisePropertyChanged(() => CurrentProgress);
                }
            }
        }
        public string CurrentUrl
        {
            get { return currentUrl; }
            private set
            {
                if (currentUrl != value)
                {
                    currentUrl = value;
                    RaisePropertyChanged(() => CurrentUrl);
                }
            }
        }
    }
}
