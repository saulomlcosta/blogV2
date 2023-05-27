using System.ComponentModel.DataAnnotations;

namespace BlogV2.ViewModels.Accounts
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "O E-mail é obrigatório")]
        [EmailAddress(ErrorMessage = "O E-mail é inválido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe a senha")]
        public string Password { get; set; } = string.Empty;
    }
}
