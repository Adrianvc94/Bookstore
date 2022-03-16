#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookstoreAPI.Models;
using Microsoft.AspNetCore.Authorization;

namespace BookstoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly BookStoreContext _context;

        public BooksController(BookStoreContext context)
        {
            _context = context;
        }

        // GET: api/Books
        [HttpGet, AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            return await _context.Books.ToListAsync();
        }

        // GET: api/Books/page/1
        [HttpGet, AllowAnonymous]
        [Route("page/{page}")]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetBooksByPage(int page)
        {
            var pageResults = 20f;
            var pageCount = Math.Ceiling(_context.Books.Count() / pageResults);

            var books = await _context.Books.Skip((page - 1) * (int)pageResults).Take((int)pageResults).ToListAsync();

            var response = new BookDto()
            {
                Books = books,
                CurrentPage = page,
                Pages = (int)pageCount
            };

            return Ok(response);
        }

        // GET: api/Books/5
        [HttpGet("{id}"), AllowAnonymous]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            return book;
        }

        // PUT: api/Books/5
        [HttpPut("{id}"), Authorize(Roles = "admin")]
        public async Task<IActionResult> PutBook(int id, Book book)
        {
            if (id != book.Id)
            {
                return BadRequest();
            }
            try
            {
                _context.Entry(book).State = EntityState.Modified;
            }
            catch (Exception)
            {
                return BadRequest();
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Books
        [HttpPost, Authorize(Roles = "admin")]
        public async Task<ActionResult<Book>> PostBook(Book book)
        {
            try
            {
                _context.Books.Add(book);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return BadRequest();
            }
           

            return Ok(book);
        }

        // DELETE: api/Books/5
        [HttpDelete("{id}"), Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
    }
}
