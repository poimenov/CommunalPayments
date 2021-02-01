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
    public class Person : Entity
    {
        [Key]
        [ReadOnly(true)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Id", Order = 1, ResourceType = typeof(Properties.Resource))]
        public override int Id { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "FirstName", Order = 4, ResourceType = typeof(Properties.Resource))]
        public string FirstName { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "LastName", Order = 3, ResourceType = typeof(Properties.Resource))]
        public string LastName { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "SurName", Order = 5, ResourceType = typeof(Properties.Resource))]
        public string SurName { get; set; }
        [Browsable(false)]
        [XmlIgnore]
        public virtual List<Account> Accounts { get; set; }
        [Required]
        [DefaultValue(true)]
        [Display(Name = "Enabled", Order = 2, ResourceType = typeof(Properties.Resource))]
        public override bool Enabled { get; set; }
        [NotMapped]
        [Display(Name = "FullName", Order = 6, ResourceType = typeof(Properties.Resource))]
        public override string Name
        {
            get
            {
                return string.Format("{0} {1} {2}", LastName, FirstName, SurName);
            }
        }
    }
}
