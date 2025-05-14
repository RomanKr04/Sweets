using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sweets.Models
{
    [Table("rawmaterialpurchases")]
    public class RawMaterialPurchase
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [ForeignKey("rawmaterialpurchases_raw_material_id_fkey")]
        [Column("raw_material_id")]
        public int RawMaterialID { get; set; }
        public virtual RawMaterial RawMaterial { get; set; }
        [Column("quantity")]
        public double Quantity { get; set; }
        [Column("total_cost")]
        public double TotalCost { get; set; }

        [Column("purchase_date")]
        public DateOnly PurchaseDate { get; set; }
        [ForeignKey("rawmaterialpurchases_employee_id_fkey")]
        [Column("employee_id")]
        public int EmployeeID   { get; set; }
        public virtual Employee Employee { get; set; }
    }
}
