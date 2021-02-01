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
        private readonly INetRepository _netRepository;
        private int currentProgress;
        private string currentUrl;
        public bool? DialogResult
        {
            get => dialogResult;
            private set => Set(nameof(DialogResult), ref dialogResult, value);
        }
        public ProgressViewModel(string login, string password, int userId, INetRepository netRepository)
        {           
            if(!string.IsNullOrWhiteSpace(login) && !string.IsNullOrWhiteSpace(password))
            {
                _netRepository = netRepository;
                _netRepository.Login = login;
                _netRepository.Password = password;
                _netRepository.ImportProgressChanged += _netRepository_ImportProgressChanged;
            }
            _ = this.Start(userId);
        }

        public async Task<bool> Start(int userId)
        {
            bool retVal = await _netRepository.Import(userId);
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
