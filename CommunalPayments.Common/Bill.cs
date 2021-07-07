using CommunalPayments.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CommunalPayments.Common
{
    public class Bill : Entity
    {
        public Bill()
        {
            Payments = new List<Payment>();
        }
        [Key]
        [ReadOnly(true)]
        [Display(Name = "Id", Order = 1, ResourceType = typeof(Properties.Resource))]
        public override int Id { get; set; }
        [ReadOnly(true)]
        [Display(Name = "ErcId", Order = 3, ResourceType = typeof(Properties.Resource))]
        public long ErcId { get; set; }
        [Required]
        [DefaultValue(true)]
        [Display(Name = "Enabled", Order = 2, ResourceType = typeof(Properties.Resource))]
        public override bool Enabled { get; set; }
        [Required]
        [Display(Name = "CreateDate", Order = 4, ResourceType = typeof(Properties.Resource))]
        public DateTime CreateDate { get; set; }
        [ForeignKey("Status")]
        [Display(Name = "Status", Order = 5, ResourceType = typeof(Properties.Resource))]
        public int StatusId { get; set; }
        [Browsable(false)]
        public PayStatus Status { get; set; }
        [ForeignKey("Mode")]
        [Display(Name = "Mode", Order = 6, ResourceType = typeof(Properties.Resource))]
        public int ModeId { get; set; }
        [Browsable(false)]
        public PayMode Mode { get; set; }
        [Browsable(false)]
        public virtual List<Payment> Payments { get; set; }
        [NotMapped]
        [Display(Name = "Name", Order = 6, ResourceType = typeof(Properties.Resource))]
        public override string Name => string.Format("Счёт №{0} от {1}", this.ErcId, this.CreateDate.ToString("dd MMM yyyy"));
    }
}
