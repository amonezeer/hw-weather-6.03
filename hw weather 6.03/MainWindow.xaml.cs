using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

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
            string url = $"http://api.openweathermap.org/data/2.5/weather?q={cityName}&appid={apiKey}&units=metric&lang=uk";

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
                            string iconCode = weatherData.Weather[0].Icon;
                            string iconUrl = $"http://openweathermap.org/img/wn/{iconCode}@2x.png";
                            WeatherIcon.Source = new BitmapImage(new Uri(iconUrl, UriKind.Absolute));

                            WeatherInfoTextBlock.Text = $"📍 {cityName}\n" +
                            $"🌡 Температура: {weatherData.Main.Temp}°C \n" +
                            $"🌫 Опис: {char.ToUpper(weatherData.Weather[0].Description[0]) + weatherData.Weather[0].Description.Substring(1)}\n" +
                            $"💨 Вітер: {weatherData.Wind.Speed} м/с {GetWindDirection(weatherData.Wind.Speed)}\n" +
                            $"💧 Вологість: {weatherData.Main.Humidity}%\n" +
                            $"🌡 Тиск: {weatherData.Main.Pressure} гПа\n" +
                            $"👁 Видимість: {weatherData.Visibility / 1000.0} км";

                        }
                        else
                        {
                            WeatherInfoTextBlock.Text = "Не вдалося отримати інформацію про погоду.";
                        }
                    }
                    else
                    {
                        WeatherInfoTextBlock.Text = $"Помилка при отриманні даних. Код статусу: {response.StatusCode}";
                    }
                }
            }
            catch (Exception ex)
            {
                WeatherInfoTextBlock.Text = "Сталася помилка: " + ex.Message;
            }
        }

        private string GetWindDirection(float speed)
        {
            if (speed < 1) return "штиль";
            else if (speed < 3) return "легкий бриз";
            else if (speed < 8) return "помірний вітер";
            else if (speed < 14) return "сильний вітер";
            else return "штормовий вітер";
        }

        public class WeatherData
        {
            public MainData Main { get; set; }
            public Weather[] Weather { get; set; }
            public WindData Wind { get; set; }
            public int Visibility { get; set; }
        }

        public class MainData
        {
            public float Temp { get; set; }
            public float FeelsLike { get; set; }
            public int Humidity { get; set; }
            public int Pressure { get; set; }
        }

        public class Weather
        {
            public string Description { get; set; }
            public string Icon { get; set; }
        }

        public class WindData
        {
            public float Speed { get; set; }
        }
    }
}
