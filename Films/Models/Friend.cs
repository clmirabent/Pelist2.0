using System;
using System.Collections.Generic;

namespace Films.Models;

public partial class Friend
{
    public int IdFriend { get; set; }

    public int FkIdUser { get; set; } // Usuario que recibe la solicitud

    public int FkIdFriend { get; set; } // Usuario que envía la solicitud

    public bool PendingFriend { get; set; }

    public virtual User FkIdFriendNavigation { get; set; } = null!; // Relación con el usuario emisor

    public virtual User FkIdUserNavigation { get; set; } = null!; // Relación con el usuario receptor
}
