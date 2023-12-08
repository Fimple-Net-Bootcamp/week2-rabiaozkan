namespace Space_Weather.Models
{
    public class Planet
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Weather> Weathers { get; set; }
    }
}
