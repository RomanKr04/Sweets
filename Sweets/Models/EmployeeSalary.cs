using Microsoft.AspNetCore.Routing.Constraints;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sweets.Models
{
    [Table("employee_salaries")]
    public class EmployeeSalary
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("employee_id")]
        public int EmployeeId { get; set; }
        public  Employee Employee { get; set; }
        [Column("year")]
        public int Year { get; set; }
        [Column("month")]
        public int Month { get; set; }
        [Column("participation_in_purchases")]
        public int TotalPurchases { get; set; }
        [Column("participation_in_production")]
        public int TotalManufacturing {  get; set; }
        [Column("participation_in_sales")]
        public int TotalSales { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Column("total_participations")]
        public int TotalAllParticipations { get; set; }
        [Column("bonus")]
        public decimal BonusTotal { get; set; }
        [Column("total_salary")]
        public decimal TotalSalary { get; set; }
        [Column("is_paid")]
        public bool Status {  get; set; }
    }
}
