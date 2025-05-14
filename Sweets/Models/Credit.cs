using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sweets.Models
{
    [Table("credits")]
    public class Credit
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("amount")]
        public decimal Amount { get; set; }
        [Column("datereceived")]
        public DateOnly DateReceived { get; set; }
        [Column("termyears")]
        public int TermYears { get; set; }
        [Column("interestrate")]
        public int InterestRate { get; set; }
        [Column("penaltyrate")]
        public decimal PenaltyRate { get; set; }

        public virtual ICollection<Payment> Payments { get; set; }
    }
}
