using CommunalPayments.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CommunalPayments.Common
{
    [Serializable]
    public class Service : Entity
    {
        [Key]
        [ReadOnly(true)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Id", Order = 1, ResourceType = typeof(Properties.Resource))]
        public override int Id { get; set; }
        [DisplayName("Erc Id")]
        [Display(Name = "ErcId", Order = 3, ResourceType = typeof(Properties.Resource))]
        public int ErcId { get; set; }
        [Required]
        [StringLength(250)]
        [Display(Name = "Name", Order = 4, ResourceType = typeof(Properties.Resource))]
        public override string Name { get; set; }
        [Required]
        [DefaultValue(true)]
        [Display(Name = "Enabled", Order = 2, ResourceType = typeof(Properties.Resource))]
        public override bool Enabled { get; set; }
        [Browsable(false)]
        public virtual List<Rate> Rates { get; set; } 
    }
}
