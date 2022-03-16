using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public HomeController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var httpClient = _httpClientFactory.CreateClient();
        var httpResponseMessage = await httpClient.GetAsync("https://localhost:5001/api/Items");
        var stream = await httpResponseMessage.Content.ReadAsStreamAsync();
        var items = await JsonSerializer.DeserializeAsync<List<ItemViewModel>>(stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return View(items);
    }

    [HttpPost]
    public async Task<IActionResult> Create(List<ItemViewModel> items)
    {
        var httpClient = _httpClientFactory.CreateClient();
        await httpClient.PostAsync("https://localhost:5001/api/Items", JsonContent.Create(items));

        return RedirectToAction("Index");
    }
}
