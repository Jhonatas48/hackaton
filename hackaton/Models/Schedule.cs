using System.ComponentModel.DataAnnotations;

namespace hackaton.Models
{
    public class Schedule
    {
        public int ScheduleId { get; set; }

        [Required(ErrorMessage = "O campoe é obrigatório.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "O campoe é obrigatório.")]
        [DataType(DataType.Date)]
        public DateTime DataInicial { get; set; }

        [Required(ErrorMessage = "O campoe é obrigatório.")]
        [DataType(DataType.Date)]
        public DateTime DataFinal { get; set; }

        //Campos de navegação
        public User User { get; set; }

        public int UserId { get; set; }
    }
}
