using System.ComponentModel.DataAnnotations;

namespace Films.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        [Display(Name = "Nombre de usuario")]
        public string Username { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }
    }
}
