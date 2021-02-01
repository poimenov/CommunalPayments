using CommunalPayments.Common;
using CommunalPayments.Common.Interfaces;
using GalaSoft.MvvmLight.CommandWpf;
using log4net;
using MvvmDialogs;
using System.Collections.Generic;

namespace CommunalPayments.WPF.ViewModels.Dictionary
{
    public class PersonsViewModel : DictionaryWindowViewModel<Person>
    {
        private readonly IDialogService _dialogService;
        private readonly INetRepository _netRepository;
        private bool _importInProcess;
        public PersonsViewModel(IDataAccess<Person> dataAccess, IDialogService dialogService, INetRepository netRepository, ILog logger) : base(dataAccess, logger)
        {
            _dialogService = dialogService;
            _netRepository = netRepository;
            _importInProcess = false;
            _columns.Add(new KeyValuePair<string, string>("Id", "Id"));
            _columns.Add(new KeyValuePair<string, string>("Name", "Name"));
        }
        public override string ItemTypeName { get => App.ResGlobal.GetString("Person"); }
        #region ImportCmd
        public RelayCommand<object> ImportCmd { get { return new RelayCommand<object>(OnImport, obj => (obj != null && ItemList.Count > 0 && ((Entity)obj).Enabled && ((Entity)obj).Id > 0 && !_importInProcess), false); } }
        private void OnImport(object obj)
        {
            Entity item = (Entity)obj;
            if (item.Id > 0)
            {
                var dialogViewModel = new LoginViewModel();

                var success = _dialogService.ShowDialog(this, dialogViewModel);
                if (success == true)
                {
                    _importInProcess = true;
                    try
                    {
                        this.CanClose = false;
                        var dialogProgress = new ProgressViewModel(dialogViewModel.Login, dialogViewModel.Password, item.Id, _netRepository);
                        success = _dialogService.ShowDialog(this, dialogProgress);
                        if (success == true)
                        {
                            _dialogService.ShowMessageBox(this, App.ResGlobal.GetString("ImportSuccessMessage"), App.ResGlobal.GetString("Success"), System.Windows.MessageBoxButton.OK);
                        }
                        else
                        {
                            _dialogService.ShowMessageBox(this, App.ResGlobal.GetString("ImportFailureMessage"), App.ResGlobal.GetString("Failure"), System.Windows.MessageBoxButton.OK);
                        }
                    }
                    catch (System.Exception ex)
                    {
                        _dialogService.ShowMessageBox(this, ex.Message, App.ResGlobal.GetString("ErrorTitle"), System.Windows.MessageBoxButton.OK);
                    }
                    finally
                    {
                        _importInProcess = false;
                        this.CanClose = true;
                    }
                }
            }
        }
        #endregion
    }
}
