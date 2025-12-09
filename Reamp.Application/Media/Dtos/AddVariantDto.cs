namespace Reamp.Application.Media.Dtos
{
    // DTO for adding variant to media asset
    public class AddVariantDto
    {
        public string VariantName { get; set; } = string.Empty;
        public int? Width { get; set; }
        public int? Height { get; set; }
    }
}



