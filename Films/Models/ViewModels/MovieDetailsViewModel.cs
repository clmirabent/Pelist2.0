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
        public DateTime? ReleaseDate { get; set; }
        public double Rating { get; set; }
        public decimal Review { get; set; } = 0;
        public List<Genre> Genres { get; set; } = new List<Genre>();

        // Estado del usuario (opcional)
        public bool? IsFavorite { get; set; }
        public bool? IsWatched { get; set; }
        public bool? IsPending { get; set; }
        public bool? IsAbandoned { get; set; }
        public bool MostrarEstadoUsuario => IsFavorite != null; // True si hay sesión y datos
    }
}
