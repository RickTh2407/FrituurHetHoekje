using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FrituurHetHoekje.Models
{
    public class Account
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public string? LastName { get; set; }
        [Required]
        public string? Phone { get; set; }
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }
        [Required]
        public bool Staff { get; set; } = false;
        [Required]
        public bool Owner { get; set; } = false;
        [Required]
        public int Points { get; set; } = 0;
        [NotMapped]
        public List<Order>? LastOrder { get; set; }

        public virtual ICollection<Order>? Order { get; set; }
    }
}
