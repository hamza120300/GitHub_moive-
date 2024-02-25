namespace WebAppMovie.Models
{
    public class Movie
    {
        
        public int Id { get; set; }

        [MaxLength(250)]
        public string Title { get; set; }
        public int year { get; set; }
        public double Rate { get; set; }
        public string Storeline { get; set; }
        public byte[] Poster { get; set; } // for files of images

        public byte GenreId {  get; set; } // foreign key for table Genre [Database] 

        //shearch for navigation property in entity framework
        public Genre Genre { get; set; }

    }
}
