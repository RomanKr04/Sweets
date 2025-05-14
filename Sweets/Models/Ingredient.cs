using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sweets.Models
{
    [Table("ingredients")]
    public class Ingredient
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [ForeignKey("ingredients_product_id_fkey")]
        [Column("product_id")]
        public int ProductID { get; set; }  
        public FinishedProduct Product { get; set; }
        [ForeignKey("ingredients_raw_material_id_fkey")]
        [Column("raw_material_id")]
        public int RawMaterialID { get; set; }
        public RawMaterial RawMaterial { get; set; }
        [Column("quantity")]
        public decimal  Quantity     { get; set; }
    }
}
