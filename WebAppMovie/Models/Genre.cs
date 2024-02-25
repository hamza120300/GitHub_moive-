

using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppMovie.Models
{
    public class Genre
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // byte is nit like int so, to make it increste out 
        public byte Id { get; set; }
       
        [MaxLength(100)]  // [required] by defualt 
        public string? Name { get; set; }

    }
}
