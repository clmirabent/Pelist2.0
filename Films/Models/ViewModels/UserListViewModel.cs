using Films.Models.APIModels;

namespace Films.Models.ViewModels
{
    public class UserListViewModel
    {
        public string ListName { get; set; }
        public List<Movie> Movies { get; set; }
    }

}
