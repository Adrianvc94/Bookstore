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
    public class CollectionsController : ControllerBase
    {
        private readonly BookStoreContext _context;

        public CollectionsController(BookStoreContext context)
        {
            _context = context;
        }

        // GET: api/Collections
        [HttpGet("{idUser}")]
        public async Task<ActionResult<IEnumerable<string>>> GetCollections(int idUser)
        {
            var collections = await _context.Collections.Where(colletion => colletion.IdUser == idUser).Select(c => c.Name).Distinct().ToListAsync();

            return Ok(collections);
        }

        // GET: api/Collections/5
        [HttpGet("{idUser}/{name}")]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooksByCollection(int idUser, string name)
        {
            var collections = await (from c in _context.Collections
                               where c.IdUser == idUser && c.Name == name
                               select (from b in _context.Books
                                       where b.Id == c.IdBook
                                       select b).ToList()
                                             ).SelectMany(c => c).ToListAsync();

            if (collections == null)
            {
                return NotFound("No collection found.");
            }

            return Ok(collections);
        }

        // PUT: api/Collections/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCollection(int id, Collection collection)
        {
            if (id != collection.Id)
            {
                return BadRequest();
            }

            _context.Entry(collection).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CollectionExists(id))
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

        // POST: api/Collections
        [HttpPost]
        public async Task<ActionResult<Collection>> PostCollection(Collection collection)
        {
            var ifBookExist = await _context.Collections.Where(c => c.IdUser == collection.IdUser && c.Name == collection.Name && c.IdBook == collection.IdBook).CountAsync();

            if (ifBookExist >= 1)
            {
                return BadRequest("Book already exist in the collection.");
            }

            _context.Collections.Add(collection);
            await _context.SaveChangesAsync();

            return Ok("Collection created.");
        }

        // DeleteBookFromCollection: api/Collections/1/collectionName/4
        [HttpDelete("{idUser}/{collectionName}/{bookId}")]
        public async Task<IActionResult> DeleteBookFromCollection(int idUser, string collectionName, int bookId)
        {
            var collection = await _context.Collections.Where(c => c.IdUser == idUser && c.Name == collectionName && c.IdBook == bookId).FirstAsync();


            if (collection == null)
            {
                return NotFound();
            }

            _context.Collections.Remove(collection);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DeleteCollection: api/Collections/1/collectionName
        [HttpDelete("{idUser}/{collectionName}")]
        public async Task<IActionResult> DeleteCollection(int idUser, string collectionName)
        {
            var collection = await _context.Collections.Where(c => c.IdUser == idUser && c.Name == collectionName).ToListAsync();


            if (collection == null)
            {
                return NotFound();
            }

            _context.Collections.RemoveRange(collection);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CollectionExists(int id)
        {
            return _context.Collections.Any(e => e.Id == id);
        }
    }
}
