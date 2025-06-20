using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace Сетевое_программирование
{
    public partial class DocumentationForm : Form
    {
        private DocumentationService? _documentationService;
        private DataGridView _methodsGrid;
        private Button _loadButton;
        private Button _weatherButton;
        private Label _statusLabel;
        private Panel controlPanel;
        private DataGridViewTextBoxColumn nameColumn;
        private DataGridViewTextBoxColumn descriptionColumn;
        private DataGridViewLinkColumn urlColumn;
        private Panel infoPanel;
        private Label infoLabel;
        private ProgressBar _progressBar;

        public DocumentationForm()
        {
            InitializeComponent();
            _documentationService = new DocumentationService();
        }

        private void InitializeComponent()
        {
            this.Text = "Документация System.Int32 - Методы";
            this.Size = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(800, 500);

            // Панель управления
            controlPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(1200, 80),
                BackColor = Color.FromArgb(240, 240, 240),
                Dock = DockStyle.Top
            };
            this.Controls.Add(controlPanel);

            // Кнопка загрузки
            _loadButton = new Button
            {
                Text = "Загрузить методы System.Int32",
                Location = new Point(20, 25),
                Size = new Size(220, 35),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            _loadButton.FlatAppearance.BorderSize = 0;
            _loadButton.Click += LoadButton_Click;
            controlPanel.Controls.Add(_loadButton);

            // Кнопка перехода к погоде
            _weatherButton = new Button
            {
                Text = "Открыть погоду",
                Location = new Point(260, 25),
                Size = new Size(140, 35),
                BackColor = Color.FromArgb(46, 125, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9)
            };
            _weatherButton.FlatAppearance.BorderSize = 0;
            _weatherButton.Click += WeatherButton_Click;
            controlPanel.Controls.Add(_weatherButton);

            // Статус
            _statusLabel = new Label
            {
                Text = "Нажмите 'Загрузить методы System.Int32' для начала",
                Location = new Point(420, 32),
                Size = new Size(500, 20),
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 9)
            };
            controlPanel.Controls.Add(_statusLabel);

            // Прогресс-бар
            _progressBar = new ProgressBar
            {
                Location = new Point(20, 55),
                Size = new Size(1160, 10),
                Style = ProgressBarStyle.Marquee,
                Visible = false,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            controlPanel.Controls.Add(_progressBar);

            // Таблица методов
            _methodsGrid = new DataGridView
            {
                Location = new Point(20, 100),
                Size = new Size(1160, 550),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D,
                GridColor = Color.LightGray,
                Font = new Font("Segoe UI", 9),
                RowHeadersVisible = false,
                MultiSelect = false,
                AlternatingRowsDefaultCellStyle = { BackColor = Color.FromArgb(248, 248, 248) }
            };

            // Настройка колонок
            nameColumn = new DataGridViewTextBoxColumn
            {
                Name = "Name",
                HeaderText = "Имя метода",
                Width = 250,
                DefaultCellStyle = { Font = new Font("Consolas", 9, FontStyle.Bold) }
            };
            _methodsGrid.Columns.Add(nameColumn);

            descriptionColumn = new DataGridViewTextBoxColumn
            {
                Name = "Description",
                HeaderText = "Краткое описание метода",
                Width = 600,
                DefaultCellStyle = { WrapMode = DataGridViewTriState.True }
            };
            _methodsGrid.Columns.Add(descriptionColumn);

            urlColumn = new DataGridViewLinkColumn
            {
                Name = "Url",
                HeaderText = "URL ссылки для перехода",
                Width = 300,
                DefaultCellStyle = { 
                    ForeColor = Color.Blue,
                    Font = new Font("Segoe UI", 8, FontStyle.Underline)
                }
            };
            _methodsGrid.Columns.Add(urlColumn);

            _methodsGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            _methodsGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(230, 230, 230);
            _methodsGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            _methodsGrid.ColumnHeadersHeight = 35;

            // Обработчик клика по ячейке URL
            _methodsGrid.CellContentClick += MethodsGrid_CellContentClick;

            this.Controls.Add(_methodsGrid);

            // Добавляем информационную панель внизу
            infoPanel = new Panel
            {
                Height = 30,
                Dock = DockStyle.Bottom,
                BackColor = Color.FromArgb(250, 250, 250)
            };

            infoLabel = new Label
            {
                Text = "Совет: Кликните по URL для открытия документации в браузере",
                Location = new Point(20, 8),
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 8),
                AutoSize = true
            };
            infoPanel.Controls.Add(infoLabel);
            this.Controls.Add(infoPanel);
        }

        private async void LoadButton_Click(object? sender, EventArgs e)
        {
            _loadButton.Enabled = false;
            _progressBar.Visible = true;
            _statusLabel.Text = "Загрузка и парсинг документации Microsoft Learn...";
            _statusLabel.ForeColor = Color.Blue;

            try
            {
                var result = await _documentationService!.ParseInt32DocumentationAsync();
                
                _progressBar.Visible = false;

                if (result.Success)
                {
                    _methodsGrid.Rows.Clear();
                    
                    foreach (var method in result.Methods)
                    {
                        _methodsGrid.Rows.Add(method.Name, method.Description, method.Url);
                    }
                    
                    _statusLabel.Text = $"Успешно загружено {result.Methods.Count} методов System.Int32";
                    _statusLabel.ForeColor = Color.Green;

                    // Автоматически подстраиваем высоту строк
                    foreach (DataGridViewRow row in _methodsGrid.Rows)
                    {
                        row.Height = Math.Max(25, row.Cells[1].PreferredSize.Height + 10);
                    }
                }
                else
                {
                    _statusLabel.Text = $"Ошибка: {result.ErrorMessage}";
                    _statusLabel.ForeColor = Color.Red;
                    MessageBox.Show(result.ErrorMessage, "Ошибка загрузки", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                _progressBar.Visible = false;
                _statusLabel.Text = "Ошибка загрузки данных";
                _statusLabel.ForeColor = Color.Red;
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _loadButton.Enabled = true;
            }
        }

        private void WeatherButton_Click(object? sender, EventArgs e)
        {
            try
            {
                var weatherForm = new WeatherForm();
                weatherForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось открыть форму погоды: {ex.Message}", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void MethodsGrid_CellContentClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == 2) // URL колонка
            {
                var url = _methodsGrid.Rows[e.RowIndex].Cells[2].Value?.ToString();
                if (!string.IsNullOrEmpty(url))
                {
                    try
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = url,
                            UseShellExecute = true
                        });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Не удалось открыть ссылку: {ex.Message}", "Ошибка", 
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _documentationService?.Dispose();
            base.OnFormClosing(e);
        }
    }
} 