/// <summary>
///  Create DTO(Data transfer Object) to prevent a direct original schema's usage from the client
///  We only want to provide {is_done} field to the client
/// </summary>
namespace Todo.Dtos
{
    public record DoneDTO
    {
        public bool IsDone { get; set; } = false;   // Mark to do as done or not
    }
}
