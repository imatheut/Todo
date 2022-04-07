using System.ComponentModel.DataAnnotations;

namespace Todo.Models
{

    public class TodoTask
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string Title { get; set; } = String.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        [Required]
        public string Description { get; set; } = String.Empty;
        public short Completion { get; set; } = 0;
        public bool Done { get; set; } = false;
    }
}