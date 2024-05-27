using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DotNetBack.Models
{
    public class Repetition
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RepetitionId { get; set; }

        [Required]
        public DateTime RepetitionDate { get; set; }

        public int WordId { get; set; }

        [ForeignKey("WordId")]
        public virtual Word Word { get; set; }
    }
}