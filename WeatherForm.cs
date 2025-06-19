using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace Сетевое_программирование
{
    public partial class WeatherForm : Form
    {
        private WeatherService? _weatherService;
        private System.Windows.Forms.Timer _updateTimer;
        private NotifyIcon _notifyIcon;

        // Элементы интерфейса
        private PictureBox _iconPictureBox;
        private Label _temperatureLabel;
        private Label _descriptionLabel;
        private Label _cityLabel;
        private Label _humidityLabel;
        private Label _lastUpdateLabel;
        private Button _refreshButton;
        private TextBox _apiKeyTextBox;
        private Button _setApiKeyButton;
        private Label _apiKeyLabel;

        public WeatherForm()
        {
            InitializeComponent();
            InitializeTimer();
            InitializeNotifyIcon();
        }

        private void InitializeComponent()
        {
            this.Text = "Индикатор погоды - Москва";
            this.Size = new Size(400, 450);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // API Key input
            _apiKeyLabel = new Label
            {
                Text = "API ключ OpenWeatherMap:",
                Location = new Point(20, 20),
                Size = new Size(200, 20)
            };
            this.Controls.Add(_apiKeyLabel);

            _apiKeyTextBox = new TextBox
            {
                Location = new Point(20, 45),
                Size = new Size(250, 23),
                PlaceholderText = "Введите ваш API ключ"
            };
            this.Controls.Add(_apiKeyTextBox);

            _setApiKeyButton = new Button
            {
                Text = "Установить",
                Location = new Point(280, 45),
                Size = new Size(80, 23)
            };
            _setApiKeyButton.Click += SetApiKeyButton_Click;
            this.Controls.Add(_setApiKeyButton);

            // City label
            _cityLabel = new Label
            {
                Text = "Москва",
                Location = new Point(20, 90),
                Size = new Size(360, 30),
                Font = new Font("Arial", 16, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(_cityLabel);

            // Weather icon
            _iconPictureBox = new PictureBox
            {
                Location = new Point(160, 130),
                Size = new Size(80, 80),
                SizeMode = PictureBoxSizeMode.CenterImage
            };
            this.Controls.Add(_iconPictureBox);

            // Temperature
            _temperatureLabel = new Label
            {
                Text = "--°C",
                Location = new Point(20, 220),
                Size = new Size(360, 40),
                Font = new Font("Arial", 24, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.Blue
            };
            this.Controls.Add(_temperatureLabel);

            // Description
            _descriptionLabel = new Label
            {
                Text = "Загрузка...",
                Location = new Point(20, 270),
                Size = new Size(360, 30),
                Font = new Font("Arial", 12),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(_descriptionLabel);

            // Humidity
            _humidityLabel = new Label
            {
                Text = "Влажность: --%",
                Location = new Point(20, 300),
                Size = new Size(360, 20),
                Font = new Font("Arial", 10),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(_humidityLabel);

            // Last update
            _lastUpdateLabel = new Label
            {
                Text = "Последнее обновление: --",
                Location = new Point(20, 330),
                Size = new Size(360, 20),
                Font = new Font("Arial", 8),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.Gray
            };
            this.Controls.Add(_lastUpdateLabel);

            // Refresh button
            _refreshButton = new Button
            {
                Text = "Обновить",
                Location = new Point(160, 360),
                Size = new Size(80, 30)
            };
            _refreshButton.Click += RefreshButton_Click;
            this.Controls.Add(_refreshButton);
        }

        private void InitializeTimer()
        {
            _updateTimer = new System.Windows.Forms.Timer
            {
                Interval = 5 * 60 * 1000 // 5 минут
            };
            _updateTimer.Tick += async (s, e) => await UpdateWeatherAsync();
        }

        private void InitializeNotifyIcon()
        {
            _notifyIcon = new NotifyIcon
            {
                Text = "Индикатор погоды",
                Visible = true,
                Icon = SystemIcons.Information
            };

            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Показать", null, (s, e) => { this.Show(); this.WindowState = FormWindowState.Normal; });
            contextMenu.Items.Add("Обновить", null, async (s, e) => await UpdateWeatherAsync());
            contextMenu.Items.Add("-");
            contextMenu.Items.Add("Выход", null, (s, e) => Application.Exit());

            _notifyIcon.ContextMenuStrip = contextMenu;
            _notifyIcon.DoubleClick += (s, e) => { this.Show(); this.WindowState = FormWindowState.Normal; };
        }

        private async void SetApiKeyButton_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_apiKeyTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, введите API ключ!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _weatherService?.Dispose();
            _weatherService = new WeatherService(_apiKeyTextBox.Text.Trim());

            _updateTimer.Start();
            await UpdateWeatherAsync();
        }

        private async void RefreshButton_Click(object? sender, EventArgs e)
        {
            await UpdateWeatherAsync();
        }

        private async Task UpdateWeatherAsync()
        {
            if (_weatherService == null)
            {
                _descriptionLabel.Text = "Введите API ключ для получения данных";
                return;
            }

            try
            {
                _refreshButton.Enabled = false;
                _descriptionLabel.Text = "Обновление...";

                var weather = await _weatherService.GetWeatherAsync("Moscow");
                if (weather != null)
                {
                    UpdateUI(weather);
                    await UpdateWeatherIcon(weather.Weather[0].Icon);
                    UpdateNotifyIcon(weather);
                }
                else
                {
                    _descriptionLabel.Text = "Ошибка получения данных";
                }
            }
            finally
            {
                _refreshButton.Enabled = true;
            }
        }

        private void UpdateUI(WeatherResponse weather)
        {
            _temperatureLabel.Text = $"{Math.Round(weather.Main.Temp)}°C";
            _descriptionLabel.Text = char.ToUpper(weather.Weather[0].Description[0]) + weather.Weather[0].Description.Substring(1);
            _humidityLabel.Text = $"Влажность: {weather.Main.Humidity}%";
            _lastUpdateLabel.Text = $"Последнее обновление: {DateTime.Now:HH:mm:ss}";
        }

        private async Task UpdateWeatherIcon(string iconCode)
        {
            if (_weatherService == null) return;

            var iconData = await _weatherService.GetWeatherIconAsync(iconCode);
            if (iconData != null)
            {
                using var ms = new MemoryStream(iconData);
                _iconPictureBox.Image = Image.FromStream(ms);
            }
        }

        private void UpdateNotifyIcon(WeatherResponse weather)
        {
            _notifyIcon.Text = $"Москва: {Math.Round(weather.Main.Temp)}°C, {weather.Weather[0].Description}";

            // Создаем простую иконку с температурой
            var bitmap = new Bitmap(16, 16);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.LightBlue);
                g.DrawString($"{Math.Round(weather.Main.Temp)}", new Font("Arial", 6), Brushes.Black, 0, 4);
            }

            var icon = Icon.FromHandle(bitmap.GetHicon());
            _notifyIcon.Icon = icon;
        }

        protected override void SetVisibleCore(bool value)
        {
            base.SetVisibleCore(value);
            if (!value)
            {
                this.WindowState = FormWindowState.Minimized;
                this.ShowInTaskbar = false;
            }
            else
            {
                this.ShowInTaskbar = true;
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
            else
            {
                _updateTimer?.Stop();
                _weatherService?.Dispose();
                _notifyIcon?.Dispose();
                base.OnFormClosing(e);
            }
        }

    }
} 