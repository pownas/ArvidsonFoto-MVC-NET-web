using System.ComponentModel.DataAnnotations;

namespace ArvidsonFoto.Core.DTOs;

public class EditCategoryInputDto
{
    [Required]
    public int CategoryId { get; set; }

    [Required, MaxLength(50)]
    public string NameSv { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? NameEn { get; set; }

    [Required, MaxLength(50), RegularExpression(@"^[a-zA-Z0-9-]+$")]
    public string UrlSegmentSv { get; set; } = string.Empty;

    [MaxLength(50), RegularExpression(@"^[a-zA-Z0-9-]*$")]
    public string? UrlSegmentEn { get; set; }
}
