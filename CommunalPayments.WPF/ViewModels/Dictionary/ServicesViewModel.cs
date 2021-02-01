using CommunalPayments.Common;
using CommunalPayments.Common.Interfaces;
using log4net;
using System.Collections.Generic;

namespace CommunalPayments.WPF.ViewModels.Dictionary
{
    public class ServicesViewModel : DictionaryWindowViewModel<Service>
    {
        public ServicesViewModel(IDataAccess<Service> dataAccess, ILog logger) : base(dataAccess, logger)
        {
            _columns.Add(new KeyValuePair<string, string>("Id", "Id"));
            _columns.Add(new KeyValuePair<string, string>("Name", "Name"));
            _columns.Add(new KeyValuePair<string, string>("ErcId", "ErcId"));
        }
        public override string ItemTypeName { get => App.ResGlobal.GetString("Service"); }
    }
}
