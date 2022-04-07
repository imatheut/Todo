/// <summary>
///  Create DTO(Data transfer Object) to prevent a direct original schema's usage from the client
///  We only want to provide {tile} and {description} fields to the client
/// </summary>
using System.ComponentModel.DataAnnotations;

namespace Todo.Dtos
{
    public record CreateUpdateDTO
    {
        [Required]
        public string Title { get; set; } = String.Empty;
        [Required]
        public string Description { get; set; } = String.Empty;

    }
}
