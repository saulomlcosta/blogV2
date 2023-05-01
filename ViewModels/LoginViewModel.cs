using System.ComponentModel.DataAnnotations;

namespace BlogV2.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "O E-mail é obrigatório")]
        [EmailAddress(ErrorMessage = "O E-mail é inválido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Informe a senha")]
        public string Password { get; set; }
    }
}
