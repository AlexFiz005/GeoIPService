using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json.Serialization;

namespace GeoIPService.Models
{
    /// <summary>
    /// Модель для представления геолокационной информации IP-адреса.
    /// Содержит детали, такие как IP-адрес, город, регион, страна,
    /// координаты местоположения, организация, почтовый индекс, часовой пояс,
    /// а также ссылку на дополнительную информацию.
    /// </summary>
    public class GeoIPInfo
    {
        [Key]
        [JsonIgnore]
        public int Id { get; set; }
        public string Ip { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
        public string Loc { get; set; }
        public string Org { get; set; }
        public string Postal { get; set; }
        public string Timezone { get; set; }
        public string Readme { get; set; }
    }

}
