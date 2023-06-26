using DevOne.Security.Cryptography.BCrypt;
using hackaton.Models.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.VisualStudio.Web.CodeGeneration.CommandLine;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hackaton.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O campo Nome é obrigatório.")]
        [StringLength(40, ErrorMessage = "O campo Nome deve ter no máximo 40 caracteres.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "O campo Senha é obrigatório.")]
        [MinLength(8,ErrorMessage ="O campo Senha deve conter no mínimo 8 caracteres")]
        [MaxLength(60,ErrorMessage = "O campo Senha deve conter no máximo 30 caracteres")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "O campo CPF é obrigatório.")]
        [StringLength(14)]
        [RegularExpression(@"^\d{3}\.\d{3}\.\d{3}-\d{2}$", ErrorMessage = "CPF inválido.Digite no formato XXX.XXX.XXX.-XX")]
        [ValidCPF(ErrorMessage = "CPF digitado não é valiido")]
        public string CPF { get; set; }

        public bool IsAdmin { get; set; } = false;

        public bool Active { get; set; }=true;

        public Api Api { get; set; }
        public int ApiId { get; set; }
    }
}
