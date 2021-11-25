using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CommunalPayments.Common.Interfaces
{
    public enum PayBy
    {
        Credit,
        Debt,
        Saldo
    }
    public enum PaymentMode
    {
        BankTransfer = 1,
        BankCard = 15
    }
    public enum PaymentStatus
    {
        Created = 1,
        Finished = 5
    }
    public interface INetRepository
    {
        Task<IEnumerable<Account>> GetAccounts(int userId);
        IAsyncEnumerable<(Common.Bill, int, int)> GetBills(IEnumerable<Account> accounts, IEnumerable<Service> services);
        Task<Debt> GetDebt(Account account, DateTime date);
        Task<Payment> CreatePayment(Account account, PayBy payBy, DateTime date, IEnumerable<Service> services);
        Task<bool> UpdatePayment(Payment payment);
        Task<bool> DeletePayment(Payment payment);
        Task<Bill> CreateInvoice(PaymentMode mode, IEnumerable<long> paymentErcIds);
        string Login { get; set; }
        string Password { get; set; }
    }
}
