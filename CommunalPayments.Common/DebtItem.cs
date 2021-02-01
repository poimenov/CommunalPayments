using System.ComponentModel.DataAnnotations;

namespace CommunalPayments.Common
{
    public class DebtItem
    {
		[Display(Name = "Service", Order = 1, ResourceType = typeof(Properties.Resource))]
		public string ServiceName { get; set; }
		[Display(Name = "Credited", Order = 2, ResourceType = typeof(Properties.Resource))]
		public decimal? Credited { get; set; }
		[Display(Name = "Recalculation", Order = 3, ResourceType = typeof(Properties.Resource))]
		public decimal? Recalc { get; set; }
		[Display(Name = "Pays", Order = 4, ResourceType = typeof(Properties.Resource))]
		public decimal? Pays { get; set; }
		[Display(Name = "Penalty", Order = 5, ResourceType = typeof(Properties.Resource))]
		public decimal? Penalty { get; set; }
		[Display(Name = "Subsidy", Order = 6, ResourceType = typeof(Properties.Resource))]
		public decimal? Subs { get; set; }
		[Display(Name = "Saldo", Order = 7, ResourceType = typeof(Properties.Resource))]
		public decimal? Saldo { get; set; }
		[Display(Name = "Paysnew", Order = 8, ResourceType = typeof(Properties.Resource))]
		public decimal? Paysnew { get; set; }
		[Display(Name = "FirmName", Order = 9, ResourceType = typeof(Properties.Resource))]
		public string FirmName { get; set; }
	}
}
