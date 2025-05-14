using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sweets.Models
{
    [Table("budget")]
    public class Budget
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("total_amount")]
        public decimal TotalAmount { get; set; }

        [Column("bonus_amount")]
        public decimal BonusTotal {  get; set; }
    }
}
