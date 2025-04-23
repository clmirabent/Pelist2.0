using System;
using System.Collections.Generic;

namespace Films.Models;

public partial class TypeList
{
    public int IdListType { get; set; }

    public string ListName { get; set; } = null!;

    public virtual ICollection<List> Lists { get; set; } = new List<List>();
}


