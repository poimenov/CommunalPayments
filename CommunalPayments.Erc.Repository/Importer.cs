using CommunalPayments.Common;
using CommunalPayments.Common.Interfaces;
using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace CommunalPayments.Erc.Repository
{
    public class Importer : IImporter
    {
        private readonly ILog _logger;
        private readonly INetRepository _repository;
        private readonly IDataAccess<Common.Bill> _bills;
        private readonly IDataAccess<Account> _accounts;
        private readonly IDataAccess<Service> _services;
        public event EventHandler<ProgressChangedEventArgs> ImportProgressChanged;
        public Importer(ILog logger, INetRepository repository, IDataAccess<Common.Bill> bills, IDataAccess<Account> accounts, IDataAccess<Service> services)
        {
            _logger = logger;
            _repository = repository;
            _bills = bills;
            _services = services;
            _accounts = accounts;
        }
        protected virtual void OnProgressChanged(decimal progressPercentage, string currentUrl)
        {
            _logger.Info(string.Format("progressPercentage: {0}%, currentUrl: {1}", progressPercentage, currentUrl));
            if (progressPercentage > 100)
            {
                progressPercentage = 100;
            }
            if (null != ImportProgressChanged)
            {
                ImportProgressChanged.Invoke(this, new ProgressChangedEventArgs(Convert.ToInt32(progressPercentage), currentUrl));
            }
        }
        public async Task<bool> Import(string login, string password, int userId)
        {

            bool retVal = false;
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                return retVal;
            }
            _repository.Login = login;
            _repository.Password = password;
            _logger.Info(string.Format("Import starts at {0}", DateTime.Now));
            try
            {
                var accounts = await _repository.GetAccounts(userId);
                if (accounts.Count() > 0)
                {
                    _accounts.Create(accounts);
                    accounts = _accounts.ItemsList.Where(x => accounts.Any(y => y.Number == x.Number)).ToList();
                    OnProgressChanged(4, "Загружены данные о квартирах");
                    decimal currProc = 5;
                    await foreach ((var bill, var count, var curr) in _repository.GetBills(accounts, _services.ItemsList))
                    {
                        _bills.Create(new List<Common.Bill>() { bill });
                        OnProgressChanged(currProc, bill.Name);
                        currProc += (decimal)96 / count;
                    }
                    retVal = true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                return false;
            }
            _logger.Info(string.Format("Import finish at {0}", DateTime.Now));
            return retVal;
        }
    }
}
