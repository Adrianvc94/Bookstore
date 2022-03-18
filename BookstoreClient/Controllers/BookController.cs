using BookstoreClient.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace BookstoreClient.Controllers
{
    public class BookController : Controller
    {

        private string UrlBook = "http://bookstoreapi.eastus.azurecontainer.io/api/Books/page/";
        private string UrlGenre = "http://bookstoreapi.eastus.azurecontainer.io/api/Genre/";
        string UrlCollections = "http://bookstoreapi.eastus.azurecontainer.io/api/Collections/";

        public async Task<IActionResult> Index(int? page)
        {
             return View(await GetAllBooks(page));
        }

        private HttpClient GetClient(string token)
        {
            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            return client;
        }

        public async Task<List<Book>> GetAllBooks(int? page)
        {
            HttpClient httpClient = new HttpClient();
            int? currentPage = ViewBag.CurrentPage ?? 1;

            if (page != null)
            {
                currentPage = page;
            }

            try
            {
                var response = await httpClient.GetStringAsync($"{UrlBook}{currentPage}");

                var books = JsonConvert.DeserializeObject<BookDto>(response);

                if (books == null)
                {
                    ViewBag.CollectionsNames = new List<string>();
                    return new List<Book>();
                }

                ViewBag.CurrentPage = books.CurrentPage;
                ViewBag.TotalPages = books.Pages;
                ViewBag.CollectionsNames = await GetCollections();

                return books.Books;
            }
            catch (Exception)
            {
                ViewBag.CollectionsNames = new List<string>();
                return new List<Book>();
            }
        }

        public async Task<IActionResult> BooksGenre()
        {
            HttpClient client = new();

            try
            {
                var response = await client.GetStringAsync(UrlGenre);

                return View(JsonConvert.DeserializeObject<List<string>>(response));
            }
            catch (Exception)
            {

                return View();
            }
        }

        public async Task<IActionResult> BooksByGenre(string genre)
        {
            HttpClient client = new();

            try
            {
                var response = await client.GetStringAsync(UrlGenre + genre);

                ViewBag.CollectionsNames = await GetCollections();
                return View("Index", JsonConvert.DeserializeObject<List<Book>>(response));
            }
            catch (Exception)
            {
                ViewBag.CollectionsNames = new List<string>();
                return View("Index");
            }
        }

        private async Task<List<string>> GetCollections()
        {

            var token = HttpContext.Session.GetString("Token");
            var userId = HttpContext.Session.GetString("Id");

            if (string.IsNullOrEmpty(token))
            {
                return new List<string>();
            }
            else
            {
                var client = GetClient(token);

                try
                {
                    var response = await client.GetStringAsync($"{UrlCollections}{userId}");

                    return JsonConvert.DeserializeObject<List<string>>(response) ?? new List<string>();


                }
                catch (Exception)
                {
                    return new List<string>();

                }
            }

        }

    }
}
