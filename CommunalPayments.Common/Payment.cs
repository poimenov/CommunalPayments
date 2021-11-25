using CommunalPayments.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace CommunalPayments.Common
{
    [Serializable]
    public class Payment : Entity
    {
        public Payment()
        {
            PaymentItems = new List<PaymentItem>();
        }
        [Key]
        [ReadOnly(true)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Id", Order = 1, ResourceType = typeof(Properties.Resource))]
        public override int Id { get; set; }
        [Required]
        [DefaultValue(true)]
        [Display(Name = "Enabled", Order = 2, ResourceType = typeof(Properties.Resource))]
        public override bool Enabled { get; set; }
        [ReadOnly(true)]
        [Display(Name = "ErcId", Order = 3, ResourceType = typeof(Properties.Resource))]
        public long ErcId { get; set; }
        [ForeignKey("Bill")]
        [ReadOnly(true)]
        [Display(Name = "BillId", Order = 4, ResourceType = typeof(Properties.Resource))]
        public int? BillId { get; set; }
        [Browsable(false)]
        public Bill Bill { get; set; }
        [Required]
        [Display(Name = "PaymentDate", Order = 5, ResourceType = typeof(Properties.Resource))]
        public DateTime PaymentDate { get; set; }
        [NotMapped]
        [Display(Name = "Name", Order = 6, ResourceType = typeof(Properties.Resource))]
        public override string Name => string.Format("№{0} от {1}", this.Id, this.PaymentDate.ToString("dd MMM yyyy"));
        [NotMapped]
        [Display(Name = "Amount", Order = 7, ResourceType = typeof(Properties.Resource))]
        public decimal Sum
        {
            get
            {
                return PaymentItems.Sum(x => x.Amount);
            }
        }
        [ReadOnly(true)]
        [Display(Name = "Commission", Order = 8, ResourceType = typeof(Properties.Resource))]
        public decimal Commission { get; set; }
        [Display(Name = "Comment", Order = 9, ResourceType = typeof(Properties.Resource))]
        public string Comment { get; set; }
        [Browsable(false)]
        public virtual List<PaymentItem> PaymentItems { get; set; }
        [ForeignKey("Account")]
        [Browsable(false)]
        public int AccountId { get; set; }
        [Browsable(false)]
        public Account Account { get; set; }
        [Browsable(false)]
        public string Bbl { get; set; }
    }

    public class CheckedPayment : Payment
    {
        public CheckedPayment(Payment payment)
        {
            this.Account = payment.Account;
            this.AccountId = payment.AccountId;
            this.Bbl = payment.Bbl;
            this.Bill = payment.Bill;
            this.BillId = payment.BillId;
            this.Comment = payment.Comment;
            this.Commission = payment.Commission;
            this.Enabled = payment.Enabled;
            this.ErcId = payment.ErcId;
            this.Id = payment.Id;
            this.PaymentDate = payment.PaymentDate;
            this.PaymentItems = payment.PaymentItems;
        }
        [Display(Name = "Checked", Order = 1, ResourceType = typeof(Properties.Resource))]
        [Browsable(false)]
        public bool Checked { get; set; }

        [Display(Name = "Address", Order = 3, ResourceType = typeof(Properties.Resource))]
        [Browsable(false)]
        public string Address 
        { 
            get
            {
                return this.Account.Address;
            }
        }
    }
}
