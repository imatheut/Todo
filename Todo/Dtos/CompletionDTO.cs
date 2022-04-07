/// <summary>
///  Create DTO(Data transfer Object) to prevent a direct original schema's usage from the client
///  We only want to provide {completion} field to the client
/// </summary>
using System.ComponentModel.DataAnnotations;

namespace Todo.Dtos
{
    public record CompletionDTO
    {
        [Required]
        public short Completion { get; set; } = 0;   // Progress in percentage
    }
}
