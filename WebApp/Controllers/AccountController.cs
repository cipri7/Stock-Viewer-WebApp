using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel newUser)
        {
            string username = newUser.Username;
            string password = newUser.Password;

            using (MySqlConnection conn = new MySqlConnection("Server=localhost;Database=WebApp;User=root;Password=root1234"))
            {
                conn.Open();
                MySqlCommand insert = new MySqlCommand("INSERT INTO users (username, password) VALUES (@username, @password);", conn);
                insert.Parameters.AddWithValue("@username", username);
                insert.Parameters.AddWithValue("@password", password);
                int rowsAffected = insert.ExecuteNonQuery(); // Execute the insert command

                if (rowsAffected > 0)
                {
                    Console.WriteLine("User added successfully!");
                }
                else
                {
                    // Failed to insert the user
                    ModelState.AddModelError(string.Empty, "Failed to insert the user.");
                    return View();
                }
            }

            // Redirect to login after successful registration
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login()
        {
            ViewBag.ShowNavigation = false;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel user)
        {
            List<CustomUser> users = new List<CustomUser>();

            try
            {
                using (MySqlConnection conn = new MySqlConnection("Server=localhost;Database=WebApp;User=root;Password=root1234"))
                {
                    conn.Open();

                    MySqlCommand cmd = new MySqlCommand("select * from users", conn);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        CustomUser dbUser = new CustomUser();
                        dbUser.id = Convert.ToInt32(reader["id"]);
                        dbUser.username = reader["username"].ToString();
                        dbUser.password = reader["password"].ToString();
                        users.Add(dbUser);
                    }

                    reader.Close();
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, "Error connecting to the database.");
                return View(user);
            }

            if (!string.IsNullOrEmpty(user?.Username) && !string.IsNullOrEmpty(user?.Password))
            {
                bool isValidUser = users.Any(u => u.username == user.Username && u.password == user.Password);

                if (isValidUser)
                {
                    HttpContext.Session.SetString("Username", user.Username);
                    return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError(string.Empty, "Invalid username or password.");
            return View(user);
        }
    }
}
