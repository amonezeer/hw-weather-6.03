using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

namespace WeatherApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            WatermarkTextBlock.Visibility = Visibility.Visible;
            CityTextBox.TextChanged += CityTextBox_TextChanged;
        }

        private void CityTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            WatermarkTextBlock.Visibility = string.IsNullOrEmpty(CityTextBox.Text) ? Visibility.Visible : Visibility.Hidden;
        }

        private async void GetWeatherButton_Click(object sender, RoutedEventArgs e)
        {
            string cityName = CityTextBox.Text;
            if (string.IsNullOrEmpty(cityName))
            {
                MessageBox.Show("Будь ласка, введіть назву міста.");
                return;
            }

            string apiKey = "077e7a7c778d64721df26f58c896c48b";
            string baseUrl = "http://api.openweathermap.org/data/2.5/weather";
            string url = $"{baseUrl}?q={cityName}&appid={apiKey}&units=metric&lang=uk";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResponse = await response.Content.ReadAsStringAsync();
                        WeatherData weatherData = JsonConvert.DeserializeObject<WeatherData>(jsonResponse);

                        if (weatherData != null && weatherData.Main != null && weatherData.Weather != null)
                        {
                            WeatherInfoTextBlock.Text = $"Температура: {weatherData.Main.Temp}°C\n" +
                                                      $"Опис: {weatherData.Weather[0].Description}\n" +
                                                      $"Вологість: {weatherData.Main.Humidity}%\n" +
                                                      $"Швидкість вітру: {weatherData.Wind?.Speed} м/с";
                        }
                        else
                        {
                            WeatherInfoTextBlock.Text = "Не вдалося отримати інформацію про погоду. Перевірте правильність введеного міста.";
                        }
                    }
                    else
                    {
                        WeatherInfoTextBlock.Text = $"Помилка при отриманні даних. Код статусу: {response.StatusCode}";
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                WeatherInfoTextBlock.Text = "Помилка при здійсненні запиту: " + ex.Message;
            }
            catch (Exception ex)
            {
                WeatherInfoTextBlock.Text = "Сталася загальна помилка: " + ex.Message;
            }
        }

        public class WeatherData
        {
            public MainData Main { get; set; }
            public Weather[] Weather { get; set; }
            public WindData Wind { get; set; }
        }

        public class MainData
        {
            public float Temp { get; set; }
            public int Humidity { get; set; }
        }

        public class Weather
        {
            public string Description { get; set; }
        }

        public class WindData
        {
            public float Speed { get; set; }
        }
    }
}