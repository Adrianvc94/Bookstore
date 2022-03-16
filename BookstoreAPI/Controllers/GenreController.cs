using BookstoreAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookstoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenreController : ControllerBase
    {
        private readonly BookStoreContext _bookStoreContext;

        public GenreController(BookStoreContext bookStoreContext)
        {
            _bookStoreContext = bookStoreContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> Get()
        {
            var result = await _bookStoreContext.Books.Select(b => b.Genre).Distinct().ToListAsync();

            return Ok(result);
        }

        [HttpGet("{genre}")]
        public async Task<ActionResult<IEnumerable<Book>>> GetByGenre(string genre)
        {
            var result = await _bookStoreContext.Books.Where(b => b.Genre == genre).ToListAsync();

            return Ok(result);
        }
    }
}
