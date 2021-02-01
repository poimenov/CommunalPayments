using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace CommunalPayments.Common.Interfaces
{
    public interface INetRepository
    {
        Task<bool> Import(int userId);
        event EventHandler<ProgressChangedEventArgs> ImportProgressChanged;
        Task<Debt> GetDebt(Account account, DateTime date);
        string Login { set; }
        string Password { set; }
    }
}
