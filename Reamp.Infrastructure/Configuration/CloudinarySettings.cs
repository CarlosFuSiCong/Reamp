namespace Reamp.Infrastructure.Configuration
{
    public sealed class CloudinarySettings
    {
        public string CloudName { get; set; } = default!;
        public string ApiKey { get; set; } = default!;
        public string ApiSecret { get; set; } = default!;
        public bool UseHttps { get; set; } = true;
        public bool SecureDistribution { get; set; } = true;
        public string Folder { get; set; } = "reamp";
    }
}



