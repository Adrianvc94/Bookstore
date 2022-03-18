using BookstoreClient.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace BookstoreClient.Controllers
{
    public class WishlistController : Controller
    {

        string UrlWishlist = "http://bookstoreapi.eastus.azurecontainer.io/api/Wishlists/";

        private HttpClient GetClient(string token)
        {
            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            return client;
        }

        public async Task<IActionResult> Index()
        {
            var token = HttpContext.Session.GetString("Token");
            var userId = HttpContext.Session.GetString("Id");

            if (token == null)
            {
                return RedirectToAction("LogIn", "User");
            }

            var client = GetClient(token);

            try
            {
                var response = await client.GetStringAsync($"{UrlWishlist}{userId}");

                if (response == null)
                {
                    return View(new List<Book>());
                }

                return View(JsonConvert.DeserializeObject<List<Book>>(response));

            }
            catch (Exception)
            {
                return View(new List<Book>());
            }

        }

        public async Task<IActionResult> AddToWishlist(int bookId)
        {
            var token = HttpContext.Session.GetString("Token");
            var userId = HttpContext.Session.GetString("Id");

            if (token != null && userId != null)
            {

                var client = GetClient(token);
                Wishlist wishlist = new Wishlist()
                {
                    Id = 0,
                    IdBook = bookId,
                    IdUser = Convert.ToInt32(userId)
                };

                try
                {
                    var response = await client.PostAsync(UrlWishlist, new StringContent(JsonConvert.SerializeObject(wishlist),
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


        public async Task<IActionResult> DeleteFromWishlist(int bookId)
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
                    var response = await client.DeleteAsync($"{UrlWishlist}{userId}/{bookId}");

                    return RedirectToAction("Index", "Wishlist");

                }
                catch (Exception)
                {
                    return RedirectToAction("Index", "Wishlist");
                }
            }

        }

    }
}
