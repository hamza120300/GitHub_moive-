namespace WebAppMovie.Dtos
{
    public class MovieDto
    {
        [MaxLength(250)]
        public string Title { get; set; }
        public int year { get; set; }
        public double Rate { get; set; }
        public string Storeline { get; set; }
        public IFormFile? Poster { get; set; } // for files of images

        public byte GenreId { get; set; } // foreign key for table Genre [Database] 
    }
}
