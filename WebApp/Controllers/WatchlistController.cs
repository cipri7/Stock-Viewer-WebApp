using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace WebApp.Controllers
{
    public class WatchlistController : Controller
    {
        [HttpPost]
        public IActionResult AddToWatchlist(string stockName)
        {
            string username = HttpContext.Session.GetString("Username");
            Console.WriteLine("in WatchlistController " + username + " " + stockName);
            if (string.IsNullOrEmpty(username))
            {
                return Json(new { success = false, message = "User not logged in" });
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection("Server=localhost;Database=WebApp;User=root;Password=root1234"))
                {
                    conn.Open();
                    MySqlCommand insert = new MySqlCommand("INSERT INTO watchlist (username, stock) VALUES (@username, @stockName);", conn);
                    insert.Parameters.AddWithValue("@username", username);
                    insert.Parameters.AddWithValue("@stockName", stockName);

                    int rowsAffected = insert.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        return Json(new { success = true });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Failed to add to watchlist" });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error adding to watchlist: " + ex.Message);
                return Json(new { success = false, message = "Error adding to watchlist" });
            }
        }

        [HttpGet]
        public IActionResult ViewWatchlist()
        {
            string username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");
            }

            List<string> watchlist = new List<string>();

            try
            {
                using (MySqlConnection conn = new MySqlConnection("Server=localhost;Database=WebApp;User=root;Password=root1234"))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("SELECT stock FROM watchlist WHERE username = @username", conn);
                    cmd.Parameters.AddWithValue("@username", username);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        watchlist.Add(reader["stock"].ToString());
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving watchlist: " + ex.Message);
                return View("Error"); // Display an error view
            }

            return View(watchlist); // Pass the watchlist to the view
        }
    }
}
