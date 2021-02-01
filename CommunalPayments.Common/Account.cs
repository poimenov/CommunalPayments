using CommunalPayments.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace CommunalPayments.Common
{
    [Serializable]
    public class Account : Entity
    {
        [Key]
        [ReadOnly(true)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Id", Order = 1, ResourceType = typeof(Properties.Resource))]
        public override int Id { get; set; }
        [Required]
        [Display(Name="Number", Order = 3, ResourceType = typeof(Properties.Resource))]
        public long Number { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "City", Order = 4, ResourceType = typeof(Properties.Resource))]
        public string City { get; set; }
        [Required]
        [StringLength(250)]
        [Display(Name = "Street", Order = 5, ResourceType = typeof(Properties.Resource))]
        public string Street { get; set; }
        [Required]
        [StringLength(25)]
        [Display(Name = "Building", Order = 6, ResourceType = typeof(Properties.Resource))]
        public string Building { get; set; }
        [StringLength(25)]
        [Display(Name = "Apartment", Order = 7, ResourceType = typeof(Properties.Resource))]
        public string Apartment { get; set; }
        [ForeignKey("Person")]
        [Display(Name = "Person", Order = 12, ResourceType = typeof(Properties.Resource))]
        public int PersonId { get; set; }
        [Browsable(false)]
        public Person Person { get; set; }
        [Required]
        [DefaultValue(true)]
        [Display(Name = "Enabled", Order = 2, ResourceType = typeof(Properties.Resource))]
        public override bool Enabled { get; set; }
        [ReadOnly(true)]
        [Display(Name = "InternalId", Order = 10, ResourceType = typeof(Properties.Resource))]
        public int InternalId { get; set; }
        [ReadOnly(true)]
        [StringLength(64)]
        [Display(Name = "Key", Order = 11, ResourceType = typeof(Properties.Resource))]
        public string Key { get; set; }
        [Browsable(false)]
        [XmlIgnore]
        public virtual List<Payment> Payments { get; set; }
        [NotMapped]
        [Display(Name = "Name", Order = 9, ResourceType = typeof(Properties.Resource))]
        public override string Name => string.Format("{0}, {1}, {2}/{3}", this.Number, this.Street, this.Building, this.Apartment);
        [NotMapped]
        [Display(Name = "Address", Order = 8, ResourceType = typeof(Properties.Resource))]
        public string Address => string.Format("{0}, {1}, {2}/{3}", this.City, this.Street, this.Building, this.Apartment);
    }
}
