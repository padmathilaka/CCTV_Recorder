using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace FfplayTest
{
    public partial class frmSettings : Form
    {
        private string _configPath;
        private BindingList<CameraSettings> _cameraBindingList =
            new BindingList<CameraSettings>();

        // Global setting controls
        private TextBox txtRecordingRoot;
        private TextBox txtSegmentSeconds;
        private TextBox txtFFmpegThreads;
        private TextBox txtRtspTransport;
        private TextBox txtFileNamePattern;
        private TextBox txtMinFreeSpaceMb;
        private TextBox txtRetentionDays;
        private TextBox txtLogRetentionDays;
        private TextBox txtLogRoot;

        // Camera grid
        private DataGridView dgvCameras;

        // Buttons
        private Button btnSaveSettings;
        private Button btnClose;
        private Button btnAddCamera;
        private Button btnRemoveCamera;

        public frmSettings()
        {
            InitializeComponentDynamic();

            _configPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "nvrsettings.json");

            this.Load += frmSettings_Load;
        }

        // ============================================
        // Dynamic UI construction (no Designer needed)
        // ============================================
        private void InitializeComponentDynamic()
        {
            this.Text = "NVR Settings";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Width = 1000;
            this.Height = 700;

            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MinimizeBox = true;
            this.MaximizeBox = true;

            // Main layout: vertical
            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3
            };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));        // global config
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));    // cameras grid
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));        // buttons

            this.Controls.Add(mainLayout);

            // 1) Global settings panel (TableLayoutPanel 2 columns)
            var globalsPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                ColumnCount = 4,
                AutoSize = true
            };
            globalsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            globalsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            globalsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            globalsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

            int row = 0;
            void AddLabeledControl(string labelText, out TextBox textBox, int colOffset)
            {
                var lbl = new Label
                {
                    Text = labelText,
                    AutoSize = true,
                    Anchor = AnchorStyles.Left
                };
                textBox = new TextBox
                {
                    Anchor = AnchorStyles.Left | AnchorStyles.Right
                };

                globalsPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                globalsPanel.Controls.Add(lbl, colOffset, row);
                globalsPanel.Controls.Add(textBox, colOffset + 1, row);
                row++;
            }

            // Left column
            AddLabeledControl("RecordingRoot:", out txtRecordingRoot, 0);
            AddLabeledControl("SegmentSeconds:", out txtSegmentSeconds, 0);
            AddLabeledControl("FFmpegThreads:", out txtFFmpegThreads, 0);
            AddLabeledControl("RtspTransport:", out txtRtspTransport, 0);
            AddLabeledControl("FileNamePattern:", out txtFileNamePattern, 0);

            // Right column (reuse helper but adjust column index)
            row = 0;
            void AddRight(string labelText, out TextBox txt)
            {
                var lbl = new Label
                {
                    Text = labelText,
                    AutoSize = true,
                    Anchor = AnchorStyles.Left
                };
                txt = new TextBox
                {
                    Anchor = AnchorStyles.Left | AnchorStyles.Right
                };

                globalsPanel.Controls.Add(lbl, 2, row);
                globalsPanel.Controls.Add(txt, 3, row);
                row++;
            }

            AddRight("MinFreeSpaceMb:", out txtMinFreeSpaceMb);
            AddRight("RetentionDays:", out txtRetentionDays);
            AddRight("LogRetentionDays:", out txtLogRetentionDays);
            AddRight("LogRoot:", out txtLogRoot);

            mainLayout.Controls.Add(globalsPanel, 0, 0);

            // 2) Cameras DataGridView
            dgvCameras = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = true,
                AllowUserToAddRows = true,
                AllowUserToDeleteRows = true
            };

            mainLayout.Controls.Add(dgvCameras, 0, 1);

            // 3) Button panel
            var buttonPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.RightToLeft,
                Dock = DockStyle.Fill,
                AutoSize = true,
                Padding = new Padding(5)
            };

            btnSaveSettings = new Button
            {
                Text = "Save",
                AutoSize = true
            };
            btnSaveSettings.Click += btnSaveSettings_Click;

            btnClose = new Button
            {
                Text = "Close",
                AutoSize = true
            };
            btnClose.Click += btnClose_Click;

            btnAddCamera = new Button
            {
                Text = "Add Camera",
                AutoSize = true
            };
            btnAddCamera.Click += btnAddCamera_Click;

            btnRemoveCamera = new Button
            {
                Text = "Remove Selected",
                AutoSize = true
            };
            btnRemoveCamera.Click += btnRemoveCamera_Click;

            buttonPanel.Controls.Add(btnSaveSettings);
            buttonPanel.Controls.Add(btnClose);
            buttonPanel.Controls.Add(btnRemoveCamera);
            buttonPanel.Controls.Add(btnAddCamera);

            mainLayout.Controls.Add(buttonPanel, 0, 2);
        }

        // ============================
        // Models (must match frmDisplayScreen)
        // ============================

        private class AppSettings
        {
            public string RecordingRoot { get; set; }
            public int? SegmentSeconds { get; set; }
            public int? FFmpegThreads { get; set; }
            public string RtspTransport { get; set; }
            public string FileNamePattern { get; set; }
            public long? MinFreeSpaceMb { get; set; }
            public int? RetentionDays { get; set; }
            public int? LogRetentionDays { get; set; }
            public string LogRoot { get; set; }

            public BindingList<CameraSettings> Cameras { get; set; }
        }

        private class CameraSettings
        {
            public string Name { get; set; }
            public string IpAddress { get; set; }
            public string FolderName { get; set; }
            public string Tag { get; set; }

            public string RtspHost { get; set; }
            public int? RtspPort { get; set; }
            public string RtspPath { get; set; }
            public string RtspQuery { get; set; }

            public string Username { get; set; }
            public string Password { get; set; } // can be "data@#4" or "data(at)(hash)4"

            // Optional full RTSP URL (if user wants to override split)
            public string RtspUrl { get; set; }
        }

        // ============================
        // Load / Init
        // ============================

        private void frmSettings_Load(object sender, EventArgs e)
        {
            LoadSettingsIntoUi();
        }

        private void LoadSettingsIntoUi()
        {
            try
            {
                if (!File.Exists(_configPath))
                {
                    MessageBox.Show(
                        $"Settings file not found:\n{_configPath}\n\n" +
                        "A new file will be created when you save.",
                        "Info",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    InitEmptyUi();
                    return;
                }

                var json = File.ReadAllText(_configPath);
                if (string.IsNullOrWhiteSpace(json))
                {
                    InitEmptyUi();
                    return;
                }

                var settings = JsonConvert.DeserializeObject<AppSettings>(json);
                if (settings == null)
                {
                    InitEmptyUi();
                    return;
                }

                // Global fields
                txtRecordingRoot.Text = settings.RecordingRoot ?? "";
                txtSegmentSeconds.Text = settings.SegmentSeconds?.ToString() ?? "";
                txtFFmpegThreads.Text = settings.FFmpegThreads?.ToString() ?? "";
                txtRtspTransport.Text = settings.RtspTransport ?? "";
                txtFileNamePattern.Text = settings.FileNamePattern ?? "";
                txtMinFreeSpaceMb.Text = settings.MinFreeSpaceMb?.ToString() ?? "";
                txtRetentionDays.Text = settings.RetentionDays?.ToString() ?? "";
                txtLogRetentionDays.Text = settings.LogRetentionDays?.ToString() ?? "";
                txtLogRoot.Text = settings.LogRoot ?? "";

                _cameraBindingList = settings.Cameras ?? new BindingList<CameraSettings>();

                dgvCameras.AutoGenerateColumns = true;
                dgvCameras.DataSource = _cameraBindingList;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Failed to load settings:\n\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                InitEmptyUi();
            }
        }

        private void InitEmptyUi()
        {
            txtRecordingRoot.Text = @"D:\Recordings";
            txtSegmentSeconds.Text = "300";
            txtFFmpegThreads.Text = "3";
            txtRtspTransport.Text = "tcp";
            txtFileNamePattern.Text = "out%Y-%m-%d_%H-%M-%S.mp4";
            txtMinFreeSpaceMb.Text = "10000";
            txtRetentionDays.Text = "30";
            txtLogRetentionDays.Text = "30";
            txtLogRoot.Text = @"C:\CameraLog";

            _cameraBindingList = new BindingList<CameraSettings>();
            dgvCameras.AutoGenerateColumns = true;
            dgvCameras.DataSource = _cameraBindingList;
        }

        // ============================
        // Save JSON
        // ============================

        private bool SaveUiToJson()
        {
            try
            {
                if (!int.TryParse(txtSegmentSeconds.Text.Trim(), out int segmentSeconds) || segmentSeconds <= 0)
                {
                    MessageBox.Show("SegmentSeconds must be a positive integer.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                if (!int.TryParse(txtFFmpegThreads.Text.Trim(), out int threads) || threads <= 0)
                {
                    MessageBox.Show("FFmpegThreads must be a positive integer.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                if (!long.TryParse(txtMinFreeSpaceMb.Text.Trim(), out long minFreeSpace) || minFreeSpace <= 0)
                {
                    MessageBox.Show("MinFreeSpaceMb must be a positive number.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                if (!int.TryParse(txtRetentionDays.Text.Trim(), out int retentionDays) || retentionDays <= 0)
                {
                    MessageBox.Show("RetentionDays must be a positive integer.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                if (!int.TryParse(txtLogRetentionDays.Text.Trim(), out int logRetentionDays) || logRetentionDays <= 0)
                {
                    MessageBox.Show("LogRetentionDays must be a positive integer.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                var settings = new AppSettings
                {
                    RecordingRoot = txtRecordingRoot.Text.Trim(),
                    SegmentSeconds = segmentSeconds,
                    FFmpegThreads = threads,
                    RtspTransport = txtRtspTransport.Text.Trim(),
                    FileNamePattern = txtFileNamePattern.Text.Trim(),
                    MinFreeSpaceMb = minFreeSpace,
                    RetentionDays = retentionDays,
                    LogRetentionDays = logRetentionDays,
                    LogRoot = txtLogRoot.Text.Trim(),
                    Cameras = _cameraBindingList
                };

                if (settings.Cameras == null)
                    settings.Cameras = new BindingList<CameraSettings>();

                // Cleanup each camera row a bit
                foreach (var cam in settings.Cameras.ToList())
                {
                    if (string.IsNullOrWhiteSpace(cam.Name))
                    {
                        // Remove empty rows
                        settings.Cameras.Remove(cam);
                        continue;
                    }

                    cam.Name = (cam.Name ?? "").Trim();
                    cam.IpAddress = (cam.IpAddress ?? "").Trim();
                    cam.FolderName = (cam.FolderName ?? "").Trim();
                    cam.Tag = (cam.Tag ?? "").Trim();
                    cam.RtspHost = (cam.RtspHost ?? "").Trim();
                    cam.RtspPath = (cam.RtspPath ?? "").Trim();
                    cam.RtspQuery = (cam.RtspQuery ?? "").Trim();
                    cam.Username = (cam.Username ?? "").Trim();
                    cam.Password = (cam.Password ?? "").Trim();
                    cam.RtspUrl = (cam.RtspUrl ?? "").Trim();

                    if (cam.RtspPort.HasValue && cam.RtspPort.Value <= 0)
                        cam.RtspPort = null;
                }

                string json = JsonConvert.SerializeObject(settings, Formatting.Indented);
                File.WriteAllText(_configPath, json);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Failed to save settings:\n\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }
        }

        // ============================
        // Button handlers
        // ============================

        private void btnSaveSettings_Click(object sender, EventArgs e)
        {
            if (SaveUiToJson())
            {
                MessageBox.Show(
                    "Settings saved successfully.\nRestart recordings to apply changes.",
                    "Saved",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnAddCamera_Click(object sender, EventArgs e)
        {
            _cameraBindingList.Add(new CameraSettings
            {
                Name = "New Camera",
                RtspPort = 554
            });
        }

        private void btnRemoveCamera_Click(object sender, EventArgs e)
        {
            if (dgvCameras.CurrentRow == null)
                return;

            var cam = dgvCameras.CurrentRow.DataBoundItem as CameraSettings;
            if (cam != null)
            {
                _cameraBindingList.Remove(cam);
            }
        }
    }
}
