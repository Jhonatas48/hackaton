using System.ComponentModel.DataAnnotations;

namespace hackaton.Models
{
    public class Agendamento
    {
        public int AgendamentoId { get; set; }

        [Required(ErrorMessage = "O campoe é obrigatório.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "O campoe é obrigatório.")]
        [DataType(DataType.Date)]
        public DateTime DataInicial { get; set; }

        [Required(ErrorMessage = "O campoe é obrigatório.")]
        [DataType(DataType.DateTime)]
        public DateTime DataFinal { get; set; }

        //Campos de navegação
        public User User { get; set; }
    }
}
