using Films.Models.APIModels;
using System;
using System.Collections.Generic;

namespace Films.Models.ViewModels
{
    public class MovieDetailsViewModel
    {
        // Info de TMDB
        public int Id { get; set; }
        public string Title { get; set; }
        public string Overview { get; set; }
        public string PosterPath { get; set; }
        public string BackdropPath { get; set; } // Agregado para la vista de detalle
        public List<Movie> RelatedMovies { get; set; } = new();
        public DateTime? ReleaseDate { get; set; }
        public decimal Review { get; set; } // En singular, la "nota" de la película
        public List<Review> Reviews { get; set; } = new(); // En plural: Lista de comentarios de los usuarios

        public List<Genre> Genres { get; set; } = new List<Genre>();
        public List<People> Persons { get; set; } = new List<People>();


        // Estado del usuario (opcional)
        public bool? IsFavorite { get; set; }
        public bool? IsWatched { get; set; }
        public bool? IsPending { get; set; }
        public bool? IsAbandoned { get; set; }
        public List<List> UserMovieLists { get; set; }
        public bool MostrarEstadoUsuario => IsFavorite != null; // True si hay sesión y datos
    }
}
