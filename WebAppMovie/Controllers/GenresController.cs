using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppMovie.Models;

namespace WebAppMovie.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly AppDbContext _context;
        // [ ctrl + . ] to ganarate constractor  
        public GenresController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GettAllAsync() 
        { 
            var genre = await _context.Genres.OrderBy(g => g.Name).ToListAsync();
            return Ok(genre); 
        }
        

        [HttpPost]
        public async Task<IActionResult> CreatAsync(GenreDto dto) 
        {
            var genre = new Genre { Name = dto.Name };
            await _context.AddAsync(genre);
            _context.SaveChanges();
             
            return Ok(genre);
        }
        //update 
        // api/Genre/1
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody]GenreDto dto)
        {
            var genre = await _context.Genres.SingleOrDefaultAsync( g=>g.Id==id);
            if (genre == null) { 
            return NotFound($"No genre was Found with this id {id}");
            }
            genre.Name = dto.Name;
            _context.SaveChanges();
            return Ok(genre);
        }

        [HttpDelete("{id}")] 
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var genre = await _context.Genres.SingleOrDefaultAsync(g => g.Id == id);
            if (genre == null)
                return NotFound($"No genre was Found with this id {id} to delete");
            _context.Remove(genre);
            _context.SaveChanges();
            return Ok(genre);
        }
    }
}
