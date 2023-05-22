namespace hackaton.Models
{
    public class Property
    {
        public int PropertyID { get; set; }
        public string Name { get; set; }

        public User User { get; set; }
        public int userid;
    }
}
