using FrituurHetHoekje.Models;
using System.ComponentModel.DataAnnotations;

namespace FrituurHetHoekje.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int OrderNr { get; set; }
        [Required]
        public List<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();
        [Required]
        public decimal TotalPrice => OrderProducts.Sum(p => p.Product.Price * p.Amount);
        [Required]
        public DateTime? Date { get; set; }
        [Required]
        public int? AccountId { get; set; }
        public virtual Account? Account { get; set; }
    }
}
