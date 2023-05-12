using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;


namespace hackaton.Models
{
    public class QrCode
    {
        public int QRCodeId { get; set; }
        
        [System.ComponentModel.DataAnnotations.Required]
        [MaxLength(20)]
        public string Content { get; set; }
    }
}
