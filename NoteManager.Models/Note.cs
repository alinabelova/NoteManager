using System.ComponentModel.DataAnnotations;
using NoteManager.Models.Base;

namespace NoteManager.Models
{
    public class Note : NamedObject
    {
        [Required]
        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public string Description { get; set; }
        public int Position { get; set; }
    }
}
