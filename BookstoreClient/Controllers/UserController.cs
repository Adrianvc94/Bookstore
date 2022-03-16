using BookstoreClient.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace BookstoreClient.Controllers
{
    public class UserController : Controller
    {

        string UrlLogin = "https://localhost:7219/api/Users/login/";
        string UrlSignin = "https://localhost:7219/api/Users/signin/";

        public IActionResult SignIn()
        {
            return View();
        }

        public IActionResult LogIn()
        {
            return View();
        }


        public async Task<IActionResult> OnSignIn(string FirstName, string LastName, string Email, string Password)
        {
            RegistrationRequest registration = new RegistrationRequest()
            {
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
                Password = Password
            };

            HttpClient client = new HttpClient();

            try
            {
                var response = await client.PostAsync(UrlSignin, new StringContent(JsonConvert.SerializeObject(registration),
                      Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    ViewBag.labelVisbility = "display: none;";
                    ViewBag.labelText = "";
                    return RedirectToAction("LogIn", "User");
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    ViewBag.labelVisbility = "display: block;";
                    ViewBag.labelText = errorMessage ?? "There was an error with the request.";
                }

            }
            catch (Exception)
            {
                
                ViewBag.labelVisbility = "display: block;";
                ViewBag.labelText = "There was an error with the request.";
            }

            return View("SignIn");

        }

        public async Task<IActionResult> OnLogIn(string Email, string Password)
        {
            LoginRequest login = new LoginRequest()
            {
                Email = Email,
                Password = Password
            };

            HttpClient client = new HttpClient();

            try
            {
                var response = await client.PostAsync(UrlLogin, new StringContent(JsonConvert.SerializeObject(login),
                        Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    User? user = new User();
                    user = JsonConvert.DeserializeObject<User>(await response.Content.ReadAsStringAsync());

                    if (user != null)
                    {
                        HttpContext.Session.SetString("Id", Convert.ToString(user.Id));
                        HttpContext.Session.SetString("FirstName", user.FirstName);
                        HttpContext.Session.SetString("LastName", user.LastName);
                        HttpContext.Session.SetString("Email", user.Email);
                        HttpContext.Session.SetString("Rol", user.Rol);
                        HttpContext.Session.SetString("Token", user.Token);

                        ViewBag.labelVisbility = "display: none;";
                        ViewBag.labelText = "";

                        if (user.Rol == "admin")
                        {
                            return RedirectToAction("Users", "Admin");
                        }
                        return RedirectToAction("Index", "Book");
                    }
                    else
                    {
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        ViewBag.labelVisbility = "display: block;";
                        ViewBag.labelText = errorMessage ?? "Wrong credentials";
                    }
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    ViewBag.labelVisbility = "display: block;";
                    ViewBag.labelText = errorMessage ?? "Wrong credentials";
                }
            }
            catch (Exception)
            {
                ViewBag.labelVisbility = "display: block;";
                ViewBag.labelText = "There was an error with the request.";
            }

            return View("LogIn");

        }

        public IActionResult LogOut()
        {
            HttpContext.Session.Remove("Id");
            HttpContext.Session.Remove("FirstName");
            HttpContext.Session.Remove("LastName");
            HttpContext.Session.Remove("Email");
            HttpContext.Session.Remove("Rol");
            HttpContext.Session.Remove("Token");

            return RedirectToAction("Index", "Book");
        }

    }
}
