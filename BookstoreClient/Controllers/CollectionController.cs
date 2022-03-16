using BookstoreClient.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace BookstoreClient.Controllers
{
    public class CollectionController : Controller
    {
        string UrlGetCollections = "https://localhost:7219/api/Collections/";

        public async Task<IActionResult> Index()
        {
            var token = HttpContext.Session.GetString("Token");
            if (token == null)
            {
                return RedirectToAction("LogIn", "User");
            }
            return View(await CollectionsByUser());
        }

        private HttpClient GetClient(string token)
        {
            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            return client;
        }

        public async Task<List<string>> CollectionsByUser()
        {

            var token = HttpContext.Session.GetString("Token");
            if (token == null)
            {
                return new List<string>();
            }

            var userId = HttpContext.Session.GetString("Id");

            var client = GetClient(token);

            try
            {
                var response = await client.GetStringAsync($"{UrlGetCollections}{userId}");


                return JsonConvert.DeserializeObject<List<string>>(response) ?? new List<string>();

            }
            catch (Exception)
            {
                return new List<string>();
            }

        }

        public async Task<ActionResult> BooksFromCollection(string collectionName)
        {
            var token = HttpContext.Session.GetString("Token");
            if (token == null)
            {
                return RedirectToAction("LogIn", "User");
            }

            var userId = HttpContext.Session.GetString("Id");
            ViewBag.CollectionName = collectionName;

            var client = GetClient(token);

            try
            {
                var response = await client.GetStringAsync($"{UrlGetCollections}{userId}/{collectionName}");

                if (response == null)
                {
                    return RedirectToAction("Index", "Collection");
                }

                return View(JsonConvert.DeserializeObject<List<Book>>(response));

            }
            catch (Exception)
            {

                return RedirectToAction("Index", "Collection");
            }

        }

        public async Task<IActionResult> DeleteFromCollection(int bookId, string collectionName)
        {
            var token = HttpContext.Session.GetString("Token");
            var userId = HttpContext.Session.GetString("Id");

            if (token == null)
            {
                return RedirectToAction("LogIn", "User");
            }
            else
            {
                var client = GetClient(token);

                try
                {
                    var response = await client.DeleteAsync($"{UrlGetCollections}{userId}/{collectionName}/{bookId}");

                    return View("Index", await CollectionsByUser());

                }
                catch (Exception)
                {
                    return View("Index", await CollectionsByUser());
                }
            }

        }

        public async Task<IActionResult> DeleteCollection(string collectionName)
        {
            var token = HttpContext.Session.GetString("Token");
            var userId = HttpContext.Session.GetString("Id");

            if (token == null)
            {
                return RedirectToAction("LogIn", "User");
            }
            else
            {
                var client = GetClient(token);

                try
                {
                    var response = await client.DeleteAsync($"{UrlGetCollections}{userId}/{collectionName}");

                    return View("Index", await CollectionsByUser());

                }
                catch (Exception)
                {
                    return View("Index", await CollectionsByUser());
                }
            }

        }

        public async Task<IActionResult> AddToCollection(string collectionName, int bookId)
        {
            var token = HttpContext.Session.GetString("Token");
            var userId = HttpContext.Session.GetString("Id");

            if (token != null && userId != null)
            {

                var client = GetClient(token);
                Collection collection = new Collection()
                {
                    Id = 0,
                    Name = collectionName,
                    IdBook = bookId,
                    IdUser = Convert.ToInt32(userId)
                };

                try
                {
                    var response = await client.PostAsync(UrlGetCollections, new StringContent(JsonConvert.SerializeObject(collection), 
                        Encoding.UTF8, "application/json"));

                    return RedirectToAction("Index", "Book");


                }
                catch (Exception)
                {
                    return RedirectToAction("Index", "Book");
                }

            }
            else
            {
                return RedirectToAction("LogIn", "User");
            }
        }

        public async Task<IActionResult> AddCollection(string collectionName)
        {
            var token = HttpContext.Session.GetString("Token");
            var userId = HttpContext.Session.GetString("Id");

            if (token != null && userId != null)
            {

                var client = GetClient(token);
                Collection collection = new Collection()
                {
                    Id = 0,
                    Name = collectionName,
                    IdBook = 0,
                    IdUser = Convert.ToInt32(userId)
                };

                try
                {
                    var response = await client.PostAsync(UrlGetCollections, new StringContent(JsonConvert.SerializeObject(collection),
                        Encoding.UTF8, "application/json"));

                    return View("Index", await CollectionsByUser());


                }
                catch (Exception)
                {
                    return RedirectToAction("Index", "Book");
                }

            }
            else
            {
                return RedirectToAction("LogIn", "User");
            }
        }

    }
}
