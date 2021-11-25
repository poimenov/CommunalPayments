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
            _columns.Add(new ColDescript("Id", "Id"));
            _columns.Add(new ColDescript("Name", "Name"));
            _columns.Add(new ColDescript("ErcId", "ErcId"));
        }
        public override string ItemTypeName { get => App.ResGlobal.GetString("Service"); }
    }
}
