using System;
using System.Collections.Generic;

namespace Films.Models;

public partial class MovieReview
{
    public int IdMovieReview { get; set; }

    public int FkIdMovie { get; set; }

    public decimal AverageRating { get; set; }
}
