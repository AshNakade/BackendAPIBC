namespace Application.Models.BC
{
    public class BCCompanies
    {
        public string? odatacontext { get; set; }
        public List<Companies>? value { get; set; } = new List<Companies>();
    }

    public class Companies
    {
        public required string id { get; set; }
        public string? systemVersion { get; set; }
        public int? timestamp { get; set; }
        public string? name { get; set; }
        public string? displayName { get; set; }
        public string? businessProfileId { get; set; }
        public DateTime? systemCreatedAt { get; set; }
        public string? systemCreatedBy { get; set; }
        public DateTime? systemModifiedAt { get; set; }
        public string? systemModifiedBy { get; set; }
    }

}
