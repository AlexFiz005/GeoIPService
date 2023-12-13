using GeoIPService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using GeoIPService.Data;
using System.Net;

namespace GeoIPService.Controllers
{
    /// <summary>
    /// Контроллер для получения информации о геолокации по IP-адресу.
    /// Использует внешний сервис ipinfo.io для получения данных.
    /// </summary>
    public class GeoIPController : Controller
    {
        private readonly MyDbContext _context;

        private readonly IHttpClientFactory _httpClientFactory;

        /// <summary>
        /// Инициализирует новый экземпляр GeoIPController с необходимой фабрикой HttpClient.
        /// </summary>
        public GeoIPController(MyDbContext context, IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _context = context;
        }

        /// <summary>
        /// Получает геолокационную информацию для заданного IP-адреса.
        /// </summary>
        /// <param name="ip">IP-адрес, для которого требуется получить информацию.</param>
        /// <returns>ActionResult, содержащий геолокационную информацию или NotFound, если информация не найдена.</returns>
        [HttpGet("{ip}")]
        public async Task<IActionResult> GetGeoIPInfo(string ip)
        {
            //Проверка Валидности.
            if (!IPAddress.TryParse(ip, out _))
            {
                return BadRequest("Invalid IP address.");
            }

            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var response = await httpClient.GetAsync($"https://ipinfo.io/{ip}/geo");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var geoIPInfo = JsonConvert.DeserializeObject<GeoIPInfo>(content);

                    if (geoIPInfo == null)
                    {
                        return StatusCode(500, "Error parsing response data.");
                    }

                    //Сохраняем полученные данные в базу.
                    _context.GeoIPInfos.Add(geoIPInfo);
                    await _context.SaveChangesAsync();

                    return Ok(geoIPInfo);
                }
            }
            catch (HttpRequestException ex)
            {
                // Логирование ошибки
                return StatusCode(500, "Error accessing external service.");
            }
            catch (Exception ex)
            {
                // Логирование ошибки
                return StatusCode(500, "An unexpected error occurred.");
            }

            return NotFound();
        }
  
        /// Получает список всех записей GeoIPInfo из базы данных.
        /// </summary>
        /// <returns>Список всех геолокационных данных, сохраненных в базе данных.</returns>
        [HttpGet("all")]
        public async Task<IActionResult> GetAllGeoIPInfo()
        {
            var allGeoIPInfo = await _context.GeoIPInfos.ToListAsync();
            return Ok(allGeoIPInfo);
        }
    }
}
