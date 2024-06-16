namespace Codere_Challenge_Core.Filters
{
    public class ShowFilterRequest
    {
        public string? Name { get; set; }
        public string? Language { get; set; }
        public string? Genre { get; set; }
        public string? Status { get; set; }
        public DateTime? PremieredAfter { get; set; }
        public DateTime? PremieredBefore { get; set; }
        public string? Summary { get; set; }
        public string? Url { get; set; }
        public string? Type { get; set; }
        public int? Runtime { get; set; }
        public int? AverageRuntime { get; set; }
        public string? OfficialSite { get; set; }
        public string? ScheduleTime { get; set; }
        public List<string>? ScheduleDays { get; set; }
        public decimal? RatingAverage { get; set; }
        public int? Weight { get; set; }
        public string? NetworkName { get; set; }
        public DateTime? UpdatedAfter { get; set; }
    }
}
