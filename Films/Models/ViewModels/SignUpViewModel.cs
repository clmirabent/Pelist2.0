using System.ComponentModel.DataAnnotations;

namespace Films.Models.ViewModels
{
    public class SignUpViewModel
    {
        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        [StringLength(20, ErrorMessage = "El nombre de usuario no puede tener más de 20 caracteres")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [DataType(DataType.Password)]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        public string Password { get; set; }

        public IFormFile? ProfileImage { get; set; }
        
        [MaxLength(200, ErrorMessage = "Máximo 200 caracteres.")]
        public string AboutMe { get; set; }

    }
}

