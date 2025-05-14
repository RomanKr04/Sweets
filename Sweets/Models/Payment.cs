using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sweets.Models
{
    [Table("payments")]
    public class Payment
    {
        [Column("creditid")]
        public int CreditId { get; set; }

        [Key, Column("paymentnumber")]
        public int PaymentNumber { get; set; }

        [Column("paymentdate")]
        public DateTime PaymentDate { get; set; }

        [Column("creditpart")]
        public decimal CreditPart { get; set; }

        [Column("interest")]
        public decimal Interest { get; set; }

        [Column("totalpayment")]
        public decimal TotalPayment { get; set; }

        [Column("remainingcredit")]
        public decimal RemainingCredit { get; set; }

        [Column("overduedays")]
        public int OverdueDays { get; set; }

        [Column("penaltyamount")]
        public decimal PenaltyAmount { get; set; }

        [Column("totalwithpenalty")]
        public decimal TotalWithPenalty { get; set; }

        [ForeignKey("CreditId")]
        public Credit Credit { get; set; }
    }
}
