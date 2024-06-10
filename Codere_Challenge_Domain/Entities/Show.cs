using Codere_Challenge_Core.Entities;

namespace Codere_Challenge_Domain.Entities
{
    public class Show : BaseEntity
    {
        public string Name { get; set; }
        public string Summary { get; set; }
        public string Url { get; set; }
        public string Type { get; set; }
        public string Language { get; set; }
        public List<string> Genres { get; set; }
        public string Status { get; set; }
        public int Runtime { get; set; }
        public int AverageRuntime { get; set; }
        public DateTime Premiered { get; set; }
        public DateTime Ended { get; set; }
        public string OfficialSite { get; set; }
        public Schedule Schedule { get; set; }
        public Rating Rating { get; set; }
    }
}
