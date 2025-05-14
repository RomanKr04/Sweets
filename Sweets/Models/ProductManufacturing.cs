using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sweets.Models
{
    [Table("productmanufacturing")]
    public class ProductManufacturing
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [ForeignKey("FinishedProduct")]
        [Column("product_id")]
        public int FinishedProductId { get; set; }
        public FinishedProduct FinishedProduct { get; set; }

        [Column("quantity")]
        public double Quantity { get; set; }

        [Column("man_date")]
        public DateOnly ManufactureDate { get; set; }

        [ForeignKey("Employee")]
        [Column("employee_id")]
        public int EmployeeID { get; set; }
        public Employee Employee { get; set; }
    }
}
