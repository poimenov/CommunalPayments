using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CommunalPayments.Common
{
    public class PayStatus
    {
        [Key]
        [ReadOnly(true)]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        [DisplayName("Payment Mode Name")]
        public string Name { get; set; }
        public virtual List<Bill> Bills { get; set; }
    }
}
