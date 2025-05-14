using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sweets.Models
{
    [Table("employees")]
    public class Employee
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("full_name")]
        public string FullName { get; set; } = string.Empty;

        [Column("position_id")]
        public int PositionID { get; set; }
        public virtual Position Position { get; set; }
    
        [Required]
        [Column("salary")]
        public double Salary { get; set; }

        [Column("address")]
        public string? Address { get; set; }

        [Column("phone")]
        public string? Phone { get; set; }
        [Column("login")]
        public string Login {  get; set; }
        [Column ("password")]
        public string Password { get; set; }
    }
}
