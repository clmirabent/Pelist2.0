using System;
using System.Collections.Generic;

namespace Films.Models;

public partial class Review
{
    public int IdReview { get; set; }

    public int FkIdMovie { get; set; }

    public int Rating { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int FkIdUser { get; set; }

    public virtual User FkIdUserNavigation { get; set; } = null!;
}
