using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.EntityFrameworkCore;
using WebAppMovie.Models;
namespace WebAppMovie.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {

        private readonly AppDbContext _context;
        private new List<string> allowedExtenstions = new List<string> { ".jpg", ".png" };
        private long _mixAllowedPosterSize = 1048576; // that is _mixAllowedPosterSize for file in EndUser

        //ctrl + . to Generate Constractor 
        public MoviesController(AppDbContext context)  
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync() { 
            var movie = await _context.Movies. // to select movie from database 
                OrderByDescending(x=>x.Rate). // to order them 
                Include(m=>m.Genre). // to get what genre is it 
                ToListAsync(); // toAsync
            return Ok(movie);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id) 
        {
            var movie = await _context.Movies.Include(m=> m.Genre).SingleOrDefaultAsync(g=>g.Id==id);
            //var movie = await _context.Movies.FindAsync(id);

            if (movie==null)
                return NotFound($"no moive my that id {id}");

            return Ok(movie);
        }
        [HttpGet("GetByGenreId")]
        public async Task<IActionResult> GetByGenreId(int genreId) 
        {
            var movie = await _context.Movies.        // to select movie from database 
                    Where(m=>m.GenreId==genreId).    // to select just genre 
                    OrderByDescending(x => x.Rate). // to order them 
                    Include(m => m.Genre).         // to get what genre is it 
                    ToListAsync();                // toAsync
            return Ok(movie);
        }

        // [HttpGet("{id}")]
        [HttpGet("GetGenreByMovieId")]
        public async Task<IActionResult> GetGenreByMovieId(int movieId) 
        {
            var movie = await _context.Movies.
                Include(m => m.Genre).
                FirstOrDefaultAsync(m => m.Id == movieId);
            if (movie == null)
            {
                return NotFound($"bro no moive with that id  {movieId} ");
              
            }
            // return Ok(movie);

            // Retrieve the genre associated with the movie
            var genre = movie.Genre;
            int genreid = genre.Id;
            if (genre == null)
            {
                return NotFound($"there no genre for this mive, genreid is:{genreid} .");
                
            }

            return Ok(genre);
        }


       /* [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id,[FromBody] MovieDto movieDto  ) {
            var movie = await _context.Movies.SingleOrDefaultAsync(g => g.Id == id);
            if (movie == null)
                return NotFound($"No genre was Found with this id {id}");
            movie.GenreId= movieDto.GenreId;
            movie.Title = movieDto.Title;
            movie.Storeline = movieDto.Storeline;
            movie.Rate = movieDto.Rate;
            movie.year = movieDto.year;

            await _context.SaveChangesAsync();
            return Ok(movie);
        }*/

       

        [HttpPost] 
        public async Task<IActionResult> CreateAsync([FromForm] MovieDto dto ) {

            if (dto.Poster == null)
                return BadRequest("Poster is required !");

            if (!allowedExtenstions.Contains(Path.GetExtension(dto.Poster.FileName).ToLower())) {
                return BadRequest("only for jpg , png .");
            }

            if (dto.Poster.Length > _mixAllowedPosterSize)
                return BadRequest("Max size allowed is 1MB .");

            // to know is id of genre valid or not 
            var IsValidGenre = await _context.Genres.AnyAsync(g=>g.Id==dto.GenreId);
            if (!IsValidGenre)
                return BadRequest("id of genre is not valid ");

            _context.SaveChanges();
            return Ok();

            // we need add changes in poster datatype 
            // now i need to convert type of poster to IformFile
            using var dataStream = new MemoryStream();
            await dto.Poster.CopyToAsync(dataStream);
            //await _context.AddAsync(genre);
            var movie = new Movie
            { 
                GenreId = dto.GenreId,
                Title = dto.Title,
                Poster=dataStream.ToArray(),
                Rate = dto.Rate,
                year = dto.year,
                Storeline = dto.Storeline
            };
            await _context.AddAsync(movie);
            _context.SaveChanges();
            return Ok(movie);

        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id,[FromForm]MovieDto dto ) {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
                return NotFound($"(update) No movie with id:{id}");

            // to know is id of genre valid or not 
            var IsValidGenre = await _context.Genres.AnyAsync(g => g.Id == dto.GenreId);
            if (!IsValidGenre)
                return BadRequest("id of genre is not valid ");

            // to edit poter i need to ......
            if (dto.Poster != null) {

                if (!allowedExtenstions.Contains(Path.GetExtension(dto.Poster.FileName).ToLower()))
                {
                    return BadRequest("only for jpg , png .");
                }

                if (dto.Poster.Length > _mixAllowedPosterSize)
                    return BadRequest("Max size allowed is 1MB .");
                // we need add changes in poster datatype 
                // now i need to convert type of poster to IformFile
                using var dataStream = new MemoryStream();
                await dto.Poster.CopyToAsync(dataStream);
                movie.Poster= dataStream.ToArray();
            }

          

           
            movie.Title = dto.Title;
            movie.GenreId = dto.GenreId;
            movie.Rate = dto.Rate;
            movie.year = dto.year;
            movie.Storeline = dto.Storeline;

             _context.SaveChanges();
            return Ok(movie);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovieAsync(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
                return NotFound($"there is no movie with id {id}");
            _context.Remove(movie);
            _context.SaveChanges();
            return Ok(movie);
        }
    }


}
