using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sweets.Models
{
    [Table("units")]
    public class Unit
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("name")]    
        public string Name { get; set; }
    }
}
