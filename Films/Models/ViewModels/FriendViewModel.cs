namespace Films.Models.ViewModels;

public class FriendViewModel
{
    // Lista de amigos aceptados
    public List<User> Friends { get; set; }

    // Lista de usuarios que resulten de la búsqueda (usuarios potenciales a agregar)
    public List<User> SearchResults { get; set; }

    public string SearchTerm { get; set; }
}