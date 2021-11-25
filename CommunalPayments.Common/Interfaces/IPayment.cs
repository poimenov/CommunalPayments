using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunalPayments.Common.Interfaces
{
    public interface IPayment : IDataAccess<Payment>
    {
        Payment LastPayment(int accountId);
        IEnumerable<Payment> GetPaymentsByAccountId(int accountId);
        IEnumerable<Payment> GetUnpaidPaymentsByPersonId(int personId);
        int Create(Payment payment);
    }
}
