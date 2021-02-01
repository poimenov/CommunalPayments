using CommunalPayments.Common.Interfaces;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace CommunalPayments.Common
{
    [Serializable]
    public class PaymentItem : Entity
    {
        [Key]
        [ReadOnly(true)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Id", Order = 1, ResourceType = typeof(Properties.Resource))]
        public override int Id { get; set; }
        [ForeignKey("Service")]
        [Display(Name = "Service", Order = 3, ResourceType = typeof(Properties.Resource))]
        public int ServiceId { get; set; }
        [Required]
        [Display(Name = "PeriodFrom", Order = 4, ResourceType = typeof(Properties.Resource))]
        public DateTime PeriodFrom { get; set; }
        [Required]
        [Display(Name = "PeriodTo", Order = 5, ResourceType = typeof(Properties.Resource))]
        public DateTime PeriodTo { get; set; }
        [Display(Name = "LastIndication", Order = 6, ResourceType = typeof(Properties.Resource))]
        public decimal? LastIndication { get; set; }
        [Display(Name = "CurrentIndication", Order = 7, ResourceType = typeof(Properties.Resource))]
        public decimal? CurrentIndication { get; set; }
        [Display(Name = "Difference", Order = 8, ResourceType = typeof(Properties.Resource))]
        public decimal? Value { get; set; }
        [Required]
        [Display(Name = "Amount", Order = 9, ResourceType = typeof(Properties.Resource))]
        public decimal Amount { get; set; }    
        [ForeignKey("Payment")]
        [Browsable(false)]
        public int PaymentId { get; set; }
        [Browsable(false)]
        public Service Service { get; set; }
        [Browsable(false)]
        [XmlIgnore]
        public Payment Payment { get; set; }
        [Required]
        [DefaultValue(true)]
        [Display(Name = "Enabled", Order = 2, ResourceType = typeof(Properties.Resource))]
        public override bool Enabled { get; set; }
        [Browsable(false)]
        public override string Name => string.Format("{0}: {1}", Service?.Name, Amount);

    }
}
