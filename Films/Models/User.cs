namespace Films.Models
{
    public class User
    {
        public int IdUsuario { get; set; }
        public string Username { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string Email { get; set; }
        public string? AboutMe{ get; set; }
        public string Image { get; set; }
    }
}
