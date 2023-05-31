using System.ComponentModel.DataAnnotations;

namespace hackaton.Models
{
    public class Api
    {
        public int ApiId { get; set; }

        [Required]
        [MaxLength(25)]
        public string Name { get; set; }
        
        [Required]
        [MaxLength(36)]
        public string Token { get; set; }

        public bool Active { get; set; } = true;


    }
}
