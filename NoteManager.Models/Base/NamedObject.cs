using System.ComponentModel.DataAnnotations;

namespace NoteManager.Models.Base
{
    public abstract class NamedObject : EntityBase
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
    }
}
