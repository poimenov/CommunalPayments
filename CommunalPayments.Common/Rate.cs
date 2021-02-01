using CommunalPayments.Common.Interfaces;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CommunalPayments.Common
{
    [Serializable]
    public class Rate : Entity
    {
        [Key]
        [ReadOnly(true)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Id", Order = 1, ResourceType = typeof(Properties.Resource))]
        public override int Id { get; set; }
        [Required]
        [Display(Name = "DateFrom", Order = 4, ResourceType = typeof(Properties.Resource))]
        public DateTime DateFrom { get; set; }
        [Required]
        [Display(Name = "VolumeFrom", Order = 5, ResourceType = typeof(Properties.Resource))]
        public decimal VolumeFrom { get; set; }
        [Required]
        [Display(Name = "Value", Order = 6, ResourceType = typeof(Properties.Resource))]
        public decimal Value { get; set; }
        [Required]
        [StringLength(25)]
        [Display(Name = "Measure", Order = 7, ResourceType = typeof(Properties.Resource))]
        public string Measure { get; set; }
        [Display(Name = "Description", Order = 8, ResourceType = typeof(Properties.Resource))]
        public string Description { get; set; }
        [Required]
        [DefaultValue(true)]
        [Display(Name = "Enabled", Order = 2, ResourceType = typeof(Properties.Resource))]
        public override bool Enabled { get; set; }
        [ForeignKey("Service")]
        [Display(Name = "Service", Order = 3, ResourceType = typeof(Properties.Resource))]
        public int ServiceId { get; set; }
        [Browsable(false)]
        public Service Service { get; set; }
        [NotMapped]
        [Display(Name = "Name", Order = 9, ResourceType = typeof(Properties.Resource))]
        public override string Name => string.Format("{0}: {1}", this.Service.Name, this.Value);
    }
}
