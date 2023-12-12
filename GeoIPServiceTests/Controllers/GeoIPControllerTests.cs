using Microsoft.VisualStudio.TestTools.UnitTesting;
using GeoIPService.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Moq.Protected;
using System.Net;

namespace GeoIPService.Controllers.Tests
{
    [TestClass()]
    public class GeoIPControllerTests
    {
        /// <summary>
        /// Тест проверяет, что контроллер GeoIPController успешно инициализируется.
        /// Это базовая проверка, которая гарантирует, что контроллер может быть создан с помощью мокированного IHttpClientFactory.
        /// Это важно, поскольку в реальной ситуации контроллер будет зависеть от IHttpClientFactory для выполнения HTTP-запросов.
        /// </summary>
        [TestMethod()]
        public void GeoIPControllerTest()
        {
            // Создание мокированного IHttpClientFactory
            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            // Инициализация контроллера с мокированным IHttpClientFactory
            var controller = new GeoIPController(httpClientFactoryMock.Object);
            // Проверка, что контроллер не равен null
            Assert.IsNotNull(controller);
        }


        /// <summary>
        /// Тест для метода GetGeoIPInfo в GeoIPController.
        /// Проверяет, что метод корректно обрабатывает запрос и возвращает объект OkObjectResult
        /// при получении валидного IP-адреса и успешном ответе от внешнего сервиса.
        /// В тесте используется мокирование для имитации внешних HTTP-запросов, что позволяет
        /// изолировать тест от реальных внешних зависимостей и гарантировать его стабильность и воспроизводимость.
        /// </summary>
        [TestMethod()]
        public async Task GetGeoIPInfoTestAsync()
        {
            // Формируем мокированный JSON ответ
            var jsonString = "{\"ip\":\"8.8.8.8\", \"city\":\"Mountain View\", \"region\":\"California\", \"country\":\"US\", \"loc\":\"37.4056,-122.0775\", \"org\":\"AS15169 Google LLC\", \"postal\":\"94043\", \"timezone\":\"America/Los_Angeles\", \"readme\":\"https://ipinfo.io/missingauth\"}";
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonString, Encoding.UTF8, "application/json")
            };

            // Мокируем HttpMessageHandler, чтобы возвращать мокированный ответ
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
               )
               .ReturnsAsync(responseMessage);

            // Создаем HttpClient с мокированным обработчиком
            var httpClient = new HttpClient(handlerMock.Object);

            // Мокируем IHttpClientFactory для возврата созданного HttpClient
            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

            // Создаем экземпляр контроллера с мокированным IHttpClientFactory
            var controller = new GeoIPController(httpClientFactoryMock.Object);

            // Вызываем тестируемый метод
            var result = await controller.GetGeoIPInfo("8.8.8.8");

            // Проверяем результат
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }
     
    }
}