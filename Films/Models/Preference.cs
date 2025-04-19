using System;
using System.Collections.Generic;

namespace Films.Models;

public partial class Preference
{
    public int IdPreference { get; set; }

    public int FkIdUser { get; set; }

    public int FkIdCategory { get; set; }

    public virtual User FkIdUserNavigation { get; set; } = null!;
}
