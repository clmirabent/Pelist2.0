using Films.Models.APIModels;
using System;
using System.Collections.Generic;

namespace Films.Models;

public partial class List
{
    public int IdList { get; set; }

    public int FkIdUser { get; set; }

    public int FkIdMovie { get; set; }

    public int FkIdTypeList { get; set; }

    public virtual TypeList FkIdTypeListNavigation { get; set; } = null!;

    public virtual User FkIdUserNavigation { get; set; } = null!;

    // Relaciones
    public Movie Movie { get; set; }
}
