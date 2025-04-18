namespace Films.Models
{
    public class List
    {
        public int IdList { get; set; }
        public int FkIdUser { get; set; }
        public int FkIdMovie { get; set; }
        public int FkIdTypeList { get; set; }

        public User User { get; set; }
        public TypeList TypeList { get; set; }
    }
}
