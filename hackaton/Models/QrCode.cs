using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hackaton.Models
{
    public class QrCode
    {
        public int QRCodeId { get; set; }

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "O Campo Content e obrigatório.")]
        [MaxLength(36)]
        
        public string Content { get; set; }

        public User User { get; set; }

        public int UserId { get; set; }

        public DateTime TimeExpiration { get; set; }

        public bool Expired { get; set; } = false;
    }
}
