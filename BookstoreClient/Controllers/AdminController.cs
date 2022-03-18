using BookstoreClient.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace BookstoreClient.Controllers
{
    public class AdminController : Controller
    {

        private string UrlUsers = "http://bookstoreapi.eastus.azurecontainer.io/api/Users/";
        private string UrlGetBooks = "http://bookstoreapi.eastus.azurecontainer.io/api/Books/page/";
        private string UrlBaseBooks = "http://bookstoreapi.eastus.azurecontainer.io/api/Books/";



        private HttpClient GetClient(string token)
        {
            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            return client;
        }


        public async Task<IActionResult> Users()
        {
            var token = HttpContext.Session.GetString("Token");
            var verifyUser = HttpContext.Session.GetString("Rol");

            if (token == null || verifyUser != "admin")
            {
                return RedirectToAction("Index", "Book");
            }

            var client = GetClient(token);

            try
            {
                var response = await client.GetStringAsync(UrlUsers);

                if (string.IsNullOrEmpty(response))
                {
                    return View(new List<User>());
                }

                return View(JsonConvert.DeserializeObject<List<User>>(response));

            }
            catch (Exception)
            {
                return View(new List<User>());
            }
        }

        //Modificar usuarios
        public async Task<IActionResult> UpdateUser(int Id, string FirstName, string LastName, string Email, string Rol)
        {
            var token = HttpContext.Session.GetString("Token");
            var verifyUser = HttpContext.Session.GetString("Rol");

            if (token == null || verifyUser != "admin")
            {
                return RedirectToAction("Index", "Book");
            }

            var client = GetClient(token);

            try
            {
                User updateUser = new User()
                {
                    Id = Convert.ToInt32(Id),
                    FirstName = FirstName,
                    LastName = LastName,
                    Email = Email,
                    Rol = Rol,
                    Token = ""
                };

                var response = await client.PutAsync($"{UrlUsers}{Id}", new StringContent(JsonConvert.SerializeObject(updateUser),
                    Encoding.UTF8, "application/json"));

                return RedirectToAction("Users", "Admin");

            }
            catch (Exception)
            {

                return RedirectToAction("Users", "Admin");
            }
        }

        //Eliminar usuarios 
        public async Task<IActionResult> DeleteUser(int Id)
        {
            var token = HttpContext.Session.GetString("Token");
            var verifyUser = HttpContext.Session.GetString("Rol");

            if (token == null || verifyUser != "admin")
            {
                return RedirectToAction("Index", "Book");
            }

            var client = GetClient(token);

            try
            {
                var response = await client.DeleteAsync($"{UrlUsers}{Id}");

                return RedirectToAction("Users", "Admin");
            }
            catch (Exception)
            {

                return RedirectToAction("Users", "Admin");
            }

        }



        /////////////////////////
        /// Books
        ////////////////////////

        public async Task<IActionResult> Books(int? page)
        {
            var token = HttpContext.Session.GetString("Token");
            var verifyUser = HttpContext.Session.GetString("Rol");

            if (token == null || verifyUser != "admin")
            {
                return RedirectToAction("Index", "Book");
            }

            int? currentPage = ViewBag.CurrentPage ?? 1;
            if (page != null)
            {
                currentPage = page;
            }
            var client = GetClient(token);

            try
            {
                var response = await client.GetStringAsync($"{UrlGetBooks}{currentPage}");

                var books = JsonConvert.DeserializeObject<BookDto>(response);

                if (books == null)
                {
                    return View(new List<Book>());
                }

                ViewBag.CurrentPage = books.CurrentPage;
                ViewBag.TotalPages = books.Pages;

                return View(books.Books);

            }
            catch (Exception)
            {
                return View(new List<User>());
            }

        }

        //Agregar libros 
        public async Task<IActionResult> AddBook(string Title, string Authors, string Genre, string PublishedDate, 
            string SmallThumbnail, string Thumbnail, string InfoLink, string Description)
        {
            var token = HttpContext.Session.GetString("Token");
            var verifyUser = HttpContext.Session.GetString("Rol");

            if (token == null || verifyUser != "admin")
            {
                return RedirectToAction("Index", "Book");
            }

            var client = GetClient(token);
            Book newBook = new Book()
            {
                Id = 0,
                Titles = Title,
                Authors = Authors,
                Genre = Genre,
                PublishedDate = PublishedDate,
                SmallThumbnail = SmallThumbnail,
                Thumbnail = Thumbnail,
                InfoLink = InfoLink,
                Description = Description,
            };


            try
            {
                var response = await client.PostAsync($"{UrlBaseBooks}", new StringContent(JsonConvert.SerializeObject(newBook),
                    Encoding.UTF8, "application/json"));

                return RedirectToAction("Books", "Admin");

            }
            catch (Exception)
            {
                return RedirectToAction("Books", "Admin");
            }

        }



        //Modificar libros 
        public async Task<IActionResult> UpdateBook(int Id, string Title, string Authors, string Genre, string PublishedDate,
            string SmallThumbnail, string Thumbnail, string InfoLink, string Description)
        {
            var token = HttpContext.Session.GetString("Token");
            var verifyUser = HttpContext.Session.GetString("Rol");

            if (token == null || verifyUser != "admin")
            {
                return RedirectToAction("Index", "Book");
            }

            var client = GetClient(token);

            try
            {
                Book newBook = new Book()
                {
                    Id = Id,
                    Titles = Title,
                    Authors = Authors,
                    Genre = Genre,
                    PublishedDate = PublishedDate,
                    SmallThumbnail = SmallThumbnail,
                    Thumbnail = Thumbnail,
                    InfoLink = InfoLink,
                    Description = Description,
                };

                var response = await client.PutAsync($"{UrlBaseBooks}{Id}", new StringContent(JsonConvert.SerializeObject(newBook),
                    Encoding.UTF8, "application/json"));

                return RedirectToAction("Books", "Admin");

            }
            catch (Exception)
            {
                return RedirectToAction("Books", "Admin");
            }
        }


        //Eliminar libros 
        public async Task<IActionResult> DeleteBook(int Id)
        {
            var token = HttpContext.Session.GetString("Token");
            var verifyUser = HttpContext.Session.GetString("Rol");

            if (token == null || verifyUser != "admin")
            {
                return RedirectToAction("Index", "Book");
            }

            var client = GetClient(token);

            try
            {
                var response = await client.DeleteAsync($"{UrlBaseBooks}{Id}");

                return RedirectToAction("Books", "Admin");
            }
            catch (Exception)
            {
                return RedirectToAction("Books", "Admin");
            }

        }
    }
}
