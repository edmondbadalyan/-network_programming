using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Сетевое_программирование
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private const string BaseUrl = "https://api.openweathermap.org/data/2.5/weather";

        public WeatherService(string apiKey)
        {
            _httpClient = new HttpClient();
            _apiKey = apiKey;
        }

        public async Task<WeatherResponse?> GetWeatherAsync(string cityName = "Moscow")
        {
            try
            {
                var url = $"{BaseUrl}?q={cityName}&appid={_apiKey}&units=metric&lang=ru";
                var response = await _httpClient.GetStringAsync(url);
                return JsonConvert.DeserializeObject<WeatherResponse>(response);
            }
            catch (Exception ex)
            {
                // В реальном приложении здесь должно быть логирование
                Console.WriteLine($"Ошибка получения данных о погоде: {ex.Message}");
                return null;
            }
        }

        public async Task<byte[]?> GetWeatherIconAsync(string iconCode)
        {
            try
            {
                var iconUrl = $"https://openweathermap.org/img/wn/{iconCode}@2x.png";
                return await _httpClient.GetByteArrayAsync(iconUrl);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки иконки: {ex.Message}");
                return null;
            }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
} 