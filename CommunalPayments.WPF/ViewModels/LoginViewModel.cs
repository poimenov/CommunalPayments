using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using MvvmDialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CommunalPayments.WPF.ViewModels
{
    public class LoginViewModel : ViewModelBase, IModalDialogViewModel
    {
        private readonly RelayCommand okCommand;

        private string login;
        private string password;
        private bool? dialogResult;

        public LoginViewModel()
        {
            okCommand = new RelayCommand(Ok, CanOk);
        }

        public string Login
        {
            get => login;
            set
            {
                Set(nameof(Login), ref login, value);

                okCommand.RaiseCanExecuteChanged();
            }
        }

        public string Password
        {
            get => password;
            set
            {
                Set(nameof(password), ref password, value);

                okCommand.RaiseCanExecuteChanged();
            }
        }

        public ICommand OkCommand => okCommand;

        public bool? DialogResult
        {
            get => dialogResult;
            private set => Set(nameof(DialogResult), ref dialogResult, value);
        }

        private void Ok()
        {
            if (CanOk())
            {
                DialogResult = true;
            }
        }

        private bool CanOk()
        {
            return !string.IsNullOrWhiteSpace(Login) && !string.IsNullOrWhiteSpace(Password);
        }
    }
}
