using hackaton.Models.Converters;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
namespace hackaton.Models
{
    public class Schedule
    {
        public int ScheduleId { get; set; }

        [Required(ErrorMessage = "O campo é obrigatório.")]
        public string Description { get; set; }

      
        [DataType(DataType.Date)]
        public DateTime DataInicial { get; set; }

        
        [DataType(DataType.Date)]
        public DateTime DataFinal { get; set; }

        //Campos de navegação
        public User User { get; set; }

        public int UserId { get; set; }
    }
}
