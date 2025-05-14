using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;

namespace Sweets.Models
{
    [Table("finishedproducts")]
    public class FinishedProduct
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [ForeignKey("finishedproducts_unit_id_fkey")]
        [Column("unit_id")]
        public int UnitID { get; set; }
        public Unit Unit { get; set; }

        [Column("quantity")]
        public double Quantity { get; set; }

        [Column("totalcost")]
        public double TotalCost { get; set; }
    }
}
