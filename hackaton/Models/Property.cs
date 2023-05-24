using System.ComponentModel.DataAnnotations;

namespace hackaton.Models
{
    public class Property
    {
        public int PropertyID { get; set; }
        [Required(ErrorMessage = "O campo Name é obrigatório")]
        public string Name { get; set; }

        public User User { get; set; }
        public int userid;
    }
}
