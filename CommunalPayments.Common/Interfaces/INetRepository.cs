using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace CommunalPayments.Common.Interfaces
{
    public enum PayBy
    {
        Credit,
        Debt,
        Saldo
    }
    public interface INetRepository
    {
        Task<bool> Import(int userId);
        event EventHandler<ProgressChangedEventArgs> ImportProgressChanged;
        Task<Debt> GetDebt(Account account, DateTime date);
        Task<Payment> CreatePayment(Account account, PayBy payBy, DateTime date);
        Task<bool> UpdatePayment(Payment payment);
        Task<bool> DeletePayment(Payment payment);
        string Login { get; set; }
        string Password { get; set; }
    }
}
