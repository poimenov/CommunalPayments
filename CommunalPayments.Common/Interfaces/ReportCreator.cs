using CommunalPayments.Common.Reports;

namespace CommunalPayments.Common.Interfaces
{
    internal abstract class ReportCreator
    {
        public abstract void Create(Payment payment, BankInfo bank, string appDataPath, out string reportPath);
    }
}
