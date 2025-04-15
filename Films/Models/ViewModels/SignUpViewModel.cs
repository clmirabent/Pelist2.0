using System.ComponentModel.DataAnnotations;

namespace Films.Models.ViewModels
{
    public class SignUpViewModel
    {
        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        [StringLength(50, ErrorMessage = "El nombre de usuario no puede tener más de 50 caracteres")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [DataType(DataType.Password)]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Debe confirmar la contraseña")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmPassword { get; set; }
    }
}
