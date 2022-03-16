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
    [Authorize]
    public class WishlistsController : ControllerBase
    {
        private readonly BookStoreContext _context;

        public WishlistsController(BookStoreContext context)
        {
            _context = context;
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<Book>>> GetWishlistbyUser(int userId)
        {
            var books = await (from w in _context.Wishlists
                               where w.IdUser == userId
                               select (from b in _context.Books
                                       where b.Id == w.IdBook
                                       select b).ToList()
                                        ).SelectMany(w => w).ToListAsync();

            if (books == null)
            {
                return NotFound();
            }

            return Ok(books);
        }

        // POST: api/Wishlists
        [HttpPost]
        public async Task<ActionResult<Wishlist>> PostWishlist(Wishlist wishlist)
        {
            var ifBookExist = await _context.Wishlists.Where(w => w.IdUser == wishlist.IdUser && w.IdBook == wishlist.IdBook).CountAsync();

            if (ifBookExist > 0)
            {
                return BadRequest("The book is already in the wishlist.");
            }

            _context.Wishlists.Add(wishlist);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // DELETE: api/Wishlists/5
        [HttpDelete("{userId}/{bookId}")]
        public async Task<IActionResult> DeleteWishlist(int userId, int bookId)
        {
            try
            {
                var book = await _context.Wishlists.Where(w => w.IdUser == userId && w.IdBook == bookId).FirstAsync();

                if (book == null)
                {
                    return NotFound();
                }

                _context.Wishlists.Remove(book);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception)
            {
                return NotFound();
            }
        }
    }
}
