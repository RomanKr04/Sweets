using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sweets.Models
{
    [Table("rawmaterials")]
    public class RawMaterial
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [ForeignKey("rawmaterials_unit_id_fkey")]
        [Column("unit_id")]
        public int UnitID { get; set; }
        public virtual Unit Unit { get; set; }
        [Column("quantity")]
        public double Quantity { get; set; } 
        [Column("total_cost")]
        public double TotalCost { get; set; } 

    }
}
