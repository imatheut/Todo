/// <summary>
///  Create DTO(Data transfer Object) to prevent a direct original schema's usage from the client
///  For testing purposes this TodoDto has the same field as TodoDo Model
/// </summary>
using System.ComponentModel.DataAnnotations;

namespace Todo.Dtos
{
    public record TodoTaskDTO
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string Title { get; set; } = String.Empty;
        public DateTime CreatedDate { get; set; }
        [Required]
        public string Description { get; set; } = String.Empty;
        public short Completion { get; set; } = 0;
        public bool Done { get; set; } = false;

    }
}
