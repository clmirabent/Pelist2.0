using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Films.Models.ViewModels
{
    public class SignUpViewModel
    {
        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        [StringLength(20, ErrorMessage = "El nombre de usuario no puede tener más de 20 caracteres")]
        [Remote(action: "IsUserNameAvailable", controller: "Authentication", ErrorMessage = "Este nombre de usuario ya está en uso.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress]
        [Remote(action: "IsEmailAvailable", controller: "Authentication", ErrorMessage = "Este email ya está registrado.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [DataType(DataType.Password)]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        public string Password { get; set; }

        public IFormFile? ProfileImage { get; set; }
        
        [MaxLength(120, ErrorMessage = "Máximo 120 caracteres.")]
        public string AboutMe { get; set; }

    }
}

