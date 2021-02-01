using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunalPayments.Common
{
    public class Debt
    {
        public Debt(Account account, DateTime date)
        {
            this.Account = account;
            this.YearMonths = date.AddMonths(-1).ToString("yyyyMM");
            this.DebtItems = new List<DebtItem>();
        }
        public Account Account { get; set; }
        public string YearMonths { get; set; }
        public IEnumerable<DebtItem> DebtItems { get; set; }
    }
}
