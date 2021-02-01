using CommunalPayments.Common;
using CommunalPayments.Common.Interfaces;
using log4net;
using System.Collections.Generic;

namespace CommunalPayments.WPF.ViewModels.Dictionary
{
    public class AccountsViewModel : DictionaryWindowViewModel<Account>
    {
        public AccountsViewModel(IDataAccess<Account> dataAccess, ILog logger) : base(dataAccess, logger)
        {
            _columns.Add(new KeyValuePair<string, string>("Id", "Id"));
            _columns.Add(new KeyValuePair<string, string>("Number", "Number"));
            _columns.Add(new KeyValuePair<string, string>("PersonId", "Person.Name"));
            _columns.Add(new KeyValuePair<string, string>("Address", "Address"));
        }
        public override string ItemTypeName { get => App.ResGlobal.GetString("Account"); }
        
    }
}
