using GeoIPService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Newtonsoft.Json;

namespace GeoIPService.Controllers
{
    /// <summary>
    /// Контроллер для получения информации о геолокации по IP-адресу.
    /// Использует внешний сервис ipinfo.io для получения данных.
    /// </summary>
    public class GeoIPController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        /// <summary>
        /// Инициализирует новый экземпляр GeoIPController с необходимой фабрикой HttpClient.
        /// </summary>
        /// <param name="httpClientFactory">Фабрика для создания экземпляров HttpClient.</param>
        public GeoIPController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Получает геолокационную информацию для заданного IP-адреса.
        /// </summary>
        /// <param name="ip">IP-адрес, для которого требуется получить информацию.</param>
        /// <returns>ActionResult, содержащий геолокационную информацию или NotFound, если информация не найдена.</returns>
        [HttpGet("{ip}")]
        public async Task<IActionResult> GetGeoIPInfo(string ip)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync($"https://ipinfo.io/{ip}/geo");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var geoIPInfo = JsonConvert.DeserializeObject<GeoIPInfo>(content);
                return Ok(geoIPInfo);
            }
            return NotFound();
        }
    }
}
