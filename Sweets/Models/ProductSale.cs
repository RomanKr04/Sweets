using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sweets.Models
{
[Table("productsales")]
public class ProductSale
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [ForeignKey("productsales_product_id_fkey")]
    [Column("product_id")]
    [Display(Name = "Product")]
    [Required(ErrorMessage = "Please select a product")]
    public int ProductID { get; set; }
    public FinishedProduct Product { get; set; }

    [Column("quantity")]
    [Display(Name = "Quantity")]
    [Required(ErrorMessage = "Quantity is required")]
    [Range(0.01, 1000, ErrorMessage = "Quantity must be between 0.01 and 1000")]
    public double Quantity { get; set; }

    [Column("total_cost")]
    [Display(Name = "Total Cost")]
    [DataType(DataType.Currency)]
    public double TotalCost { get; set; }

    [Column("sale_date")]
    [Display(Name = "Sale Date")]
    [Required]
    public DateOnly SaleDate { get; set; }

    [ForeignKey("productsales_employee_id_fkey")]
    [Column("employee_id")]
    [Display(Name = "Employee")]
    [Required(ErrorMessage = "Please select an employee")]
    public int EmployeeID { get; set; }
    public Employee Employee { get; set; }
}
}
