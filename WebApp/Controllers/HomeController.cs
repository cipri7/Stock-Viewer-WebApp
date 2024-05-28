using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using WebApp;

public class HomeController : Controller
{


    public IActionResult Index()
    {
        ViewBag.ShowNavigation = true;
    return View();
    }
    
    
    public async Task<IActionResult> Chart(string symbol)
    {
        if (string.IsNullOrEmpty(symbol))
        {
            symbol = "IBM"; // default symbol
        }
        ViewBag.Symbol = symbol.ToUpper();
        string ApiKey = "JRAFTWW6YQ77U1AU";
        
        //In usage when using API Data
        string apiUrl = $"https://www.alphavantage.co/query?function=TIME_SERIES_DAILY&symbol={symbol}&apikey={ApiKey}";
        
        //IBM demo stock
        //string apiUrl = $"https://www.alphavantage.co/query?function=TIME_SERIES_DAILY&symbol=IBM&apikey=demo";    
        
        using (HttpClient client = new HttpClient())
        {
            string jsonResult = await client.GetStringAsync(apiUrl);
            dynamic jsonData = JsonSerializer.Deserialize<JsonElement>(jsonResult);

            ViewBag.ShowNavigation = true;
            return View(jsonData);
        }
    }
    
}