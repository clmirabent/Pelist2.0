using System;
using System.Collections.Generic;

namespace Films.Models;

public partial class Friend
{
    public int IdFriend { get; set; }

    public int FkIdUser { get; set; }

    public int FkIdFriend { get; set; }

    public bool PendingFriend { get; set; }

    public virtual User FkIdFriendNavigation { get; set; } = null!;

    public virtual User FkIdUserNavigation { get; set; } = null!;
}
