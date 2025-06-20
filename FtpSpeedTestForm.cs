namespace Сетевое_программирование;

public partial class FtpSpeedTestForm : Form
{
    private readonly FtpSpeedService _ftpService;
    private CancellationTokenSource? _cancellationTokenSource;

    // UI Controls
    private Button _btnDownload = null!;
    private Button _btnUpload = null!;
    private Button _btnRefresh = null!;
    private Button _btnCancel = null!;
    private ComboBox _cmbFiles = null!;
    private NumericUpDown _numFileSize = null!;
    private ProgressBar _progressBar = null!;
    private Label _lblStatus = null!;
    private RichTextBox _txtResults = null!;

    public FtpSpeedTestForm()
    {
        var settings = new FtpSettings();
        _ftpService = new FtpSpeedService(settings);
        
        InitializeComponent();
        _ = LoadFilesAsync(); // Fire and forget
    }

    private void InitializeComponent()
    {
        Text = "FTP Speed Test - dlptest.com";
        Size = new Size(800, 650);
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(700, 500);

        SetupControls();
        SetupLayout();
    }

    private void SetupControls()
    {
        // Header
        var lblTitle = new Label
        {
            Text = "🚀 FTP Speed Test - dlptest.com",
            Font = new Font("Segoe UI", 16, FontStyle.Bold),
            ForeColor = Color.FromArgb(0, 102, 204),
            AutoSize = true,
            Location = new Point(20, 20)
        };

        var lblDescription = new Label
        {
            Text = "Тестирование скорости скачивания и загрузки файлов через FTP",
            Font = new Font("Segoe UI", 10),
            ForeColor = Color.Gray,
            AutoSize = true,
            Location = new Point(20, 50)
        };

        // Download section
        var lblDownloadSection = new Label
        {
            Text = "📥 Тест скачивания",
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            ForeColor = Color.FromArgb(33, 150, 243),
            AutoSize = true,
            Location = new Point(20, 90)
        };

        var lblFiles = new Label
        {
            Text = "Выберите файл для скачивания:",
            Location = new Point(20, 120),
            AutoSize = true
        };

        _cmbFiles = new ComboBox
        {
            Location = new Point(20, 145),
            Width = 350,
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font = new Font("Segoe UI", 9)
        };

        _btnRefresh = new Button
        {
            Text = "🔄 Обновить список",
            Location = new Point(380, 145),
            Size = new Size(130, 25),
            BackColor = Color.FromArgb(76, 175, 80),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9)
        };
        _btnRefresh.FlatAppearance.BorderSize = 0;
        _btnRefresh.Click += async (s, e) => await LoadFilesAsync();

        _btnDownload = new Button
        {
            Text = "📥 Начать тест скачивания",
            Location = new Point(20, 180),
            Size = new Size(180, 40),
            BackColor = Color.FromArgb(33, 150, 243),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        _btnDownload.FlatAppearance.BorderSize = 0;
        _btnDownload.Click += async (s, e) => await StartDownloadTest();

        // Upload section
        var lblUploadSection = new Label
        {
            Text = "📤 Тест загрузки",
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            ForeColor = Color.FromArgb(255, 152, 0),
            AutoSize = true,
            Location = new Point(400, 90)
        };

        var lblUpload = new Label
        {
            Text = "Размер тестового файла:",
            Location = new Point(400, 120),
            AutoSize = true
        };

        _numFileSize = new NumericUpDown
        {
            Location = new Point(400, 145),
            Width = 80,
            Minimum = 1,
            Maximum = 100,
            Value = 5,
            Font = new Font("Segoe UI", 9)
        };

        var lblMB = new Label
        {
            Text = "МБ",
            Location = new Point(490, 147),
            AutoSize = true
        };

        _btnUpload = new Button
        {
            Text = "📤 Начать тест загрузки",
            Location = new Point(400, 180),
            Size = new Size(180, 40),
            BackColor = Color.FromArgb(255, 152, 0),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        _btnUpload.FlatAppearance.BorderSize = 0;
        _btnUpload.Click += async (s, e) => await StartUploadTest();

        // Progress section
        var progressPanel = new Panel
        {
            Location = new Point(20, 240),
            Size = new Size(740, 80),
            BackColor = Color.FromArgb(248, 249, 250),
            BorderStyle = BorderStyle.FixedSingle
        };

        _lblStatus = new Label
        {
            Text = "Готов к тестированию",
            Location = new Point(10, 15),
            Size = new Size(500, 20),
            ForeColor = Color.Gray,
            Font = new Font("Segoe UI", 10)
        };

        _progressBar = new ProgressBar
        {
            Location = new Point(10, 40),
            Size = new Size(600, 25),
            Style = ProgressBarStyle.Continuous
        };

        _btnCancel = new Button
        {
            Text = "❌ Отмена",
            Location = new Point(630, 40),
            Size = new Size(90, 25),
            BackColor = Color.FromArgb(244, 67, 54),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Enabled = false,
            Font = new Font("Segoe UI", 9)
        };
        _btnCancel.FlatAppearance.BorderSize = 0;
        _btnCancel.Click += (s, e) => _cancellationTokenSource?.Cancel();

        progressPanel.Controls.AddRange([_lblStatus, _progressBar, _btnCancel]);

        // Results section
        var lblResults = new Label
        {
            Text = "📊 Результаты тестов",
            Location = new Point(20, 340),
            AutoSize = true,
            Font = new Font("Segoe UI", 12, FontStyle.Bold)
        };

        _txtResults = new RichTextBox
        {
            Location = new Point(20, 370),
            Size = new Size(740, 240),
            ReadOnly = true,
            Font = new Font("Consolas", 9),
            BackColor = Color.FromArgb(248, 249, 250),
            BorderStyle = BorderStyle.FixedSingle
        };

        // Add all controls to form
        Controls.AddRange([
            lblTitle, lblDescription,
            lblDownloadSection, lblFiles, _cmbFiles, _btnRefresh, _btnDownload,
            lblUploadSection, lblUpload, _numFileSize, lblMB, _btnUpload,
            progressPanel,
            lblResults, _txtResults
        ]);
    }

    private void SetupLayout()
    {
        // Make controls resize with form
        _txtResults.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _progressBar.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        _lblStatus.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        _btnCancel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
    }

    private async Task LoadFilesAsync()
    {
        _btnRefresh.Enabled = false;
        _btnRefresh.Text = "⏳ Загрузка...";

        try
        {
            var files = await _ftpService.GetFileListAsync();
            _cmbFiles.Items.Clear();
            _cmbFiles.Items.AddRange(files.ToArray());
            
            if (files.Count > 0 && !files[0].StartsWith("Ошибка"))
                _cmbFiles.SelectedIndex = 0;
        }
        finally
        {
            _btnRefresh.Enabled = true;
            _btnRefresh.Text = "🔄 Обновить список";
        }
    }

    private async Task StartDownloadTest()
    {
        if (_cmbFiles.SelectedItem?.ToString() is not string selectedFile)
        {
            MessageBox.Show("Выберите файл для скачивания!", "Предупреждение", 
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (selectedFile.StartsWith("Ошибка"))
        {
            MessageBox.Show("Сначала обновите список файлов!", "Предупреждение", 
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var fileName = selectedFile.Split(' ')[0]; // Get filename without size info
        await RunTest(() => _ftpService.TestDownloadAsync(fileName, CreateProgressHandler()), "Скачивание", fileName);
    }

    private async Task StartUploadTest()
    {
        var fileSizeMB = (int)_numFileSize.Value;
        await RunTest(() => _ftpService.TestUploadAsync(fileSizeMB, CreateProgressHandler()), "Загрузка", $"{fileSizeMB} МБ");
    }

    private async Task RunTest(Func<Task<SpeedTestResult>> testAction, string operationName, string details)
    {
        _cancellationTokenSource = new CancellationTokenSource();
        SetControlsEnabled(false);
        
        _lblStatus.Text = $"{operationName} в процессе... ({details})";
        _lblStatus.ForeColor = Color.Blue;
        _progressBar.Value = 0;

        try
        {
            var result = await testAction();
            DisplayResult(result, operationName, details);
        }
        catch (Exception ex)
        {
            _lblStatus.Text = $"Ошибка: {ex.Message}";
            _lblStatus.ForeColor = Color.Red;
            
            var errorText = $"""
                ❌ {operationName.ToUpper()} - ОШИБКА - {DateTime.Now:HH:mm:ss}
                Детали: {details}
                Ошибка: {ex.Message}
                ═══════════════════════════════════════════════════════════════


                """;

            _txtResults.AppendText(errorText);
            _txtResults.ScrollToCaret();
        }
        finally
        {
            SetControlsEnabled(true);
            _progressBar.Value = 0;
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }
    }

    private void DisplayResult(SpeedTestResult result, string operationName, string details)
    {
        if (result.IsSuccess)
        {
            _lblStatus.Text = $"✅ {operationName} завершено! Скорость: {result.SpeedMbps:F2} Мбит/с";
            _lblStatus.ForeColor = Color.Green;

            var emoji = operationName == "Скачивание" ? "📥" : "📤";
            var resultText = $"""
                {emoji} {operationName.ToUpper()} ЗАВЕРШЕНО - {DateTime.Now:HH:mm:ss}
                ═══════════════════════════════════════════════════════════════
                📋 Детали: {details}
                📁 Размер файла: {result.GetFormattedSize()}
                ⏱️ Время: {result.Duration.TotalSeconds:F2} сек
                🚀 Скорость: {result.SpeedMbps:F2} Мбит/с ({result.SpeedBytesPerSecond:F0} байт/с)
                ═══════════════════════════════════════════════════════════════


                """;

            _txtResults.AppendText(resultText);
            _txtResults.ScrollToCaret();
        }
        else
        {
            _lblStatus.Text = $"❌ Ошибка {operationName.ToLower()}";
            _lblStatus.ForeColor = Color.Red;
            
            var errorText = $"""
                ❌ {operationName.ToUpper()} - ОШИБКА - {DateTime.Now:HH:mm:ss}
                Детали: {details}
                Ошибка: {result.ErrorMessage}
                ═══════════════════════════════════════════════════════════════


                """;

            _txtResults.AppendText(errorText);
            _txtResults.ScrollToCaret();
        }
    }

    private IProgress<int> CreateProgressHandler() => new Progress<int>(value => 
    {
        if (InvokeRequired)
            Invoke(() => _progressBar.Value = Math.Min(100, Math.Max(0, value)));
        else
            _progressBar.Value = Math.Min(100, Math.Max(0, value));
    });

    private void SetControlsEnabled(bool enabled)
    {
        _btnDownload.Enabled = enabled;
        _btnUpload.Enabled = enabled;
        _btnRefresh.Enabled = enabled;
        _btnCancel.Enabled = !enabled;
        _cmbFiles.Enabled = enabled;
        _numFileSize.Enabled = enabled;
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        _cancellationTokenSource?.Cancel();
        base.OnFormClosing(e);
    }
} 