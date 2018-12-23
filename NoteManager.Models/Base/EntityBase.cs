using System;
using System.ComponentModel.DataAnnotations;

namespace NoteManager.Models.Base
{
    public abstract class EntityBase
    {
        [Key]
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? ModifiedAt { get; set; } = DateTime.Now;
    }
}
