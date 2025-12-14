using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace NVRRecordingSystem
{
    public partial class frmCameraManager : Form
    {
        private const string SettingsPath = "nvrsettings.json";

        private GroupBox grpCameras;
        private DataGridView grdCameras;
        private Panel pnlButtons;
        private Button btnAdd;
        private Button btnDelete; // NEW
        private Button btnSave;
        private Button btnClose;

        private BindingList<CameraItem> _cameraList = new BindingList<CameraItem>();
        private JObject _fullJson; // full JSON (top settings + Cameras)

        public frmCameraManager()
        {
            InitializeComponent();
        }

        private void frmCameraManager_Load(object sender, EventArgs e)
        {
            BuildUi();
            LoadCameras();
        }

        // Camera row model
        private class CameraItem
        {
            public string Name { get; set; }
            public string IpAddress { get; set; }
            public string RtspUrl { get; set; }
            public string FolderName { get; set; }
            public string Tag { get; set; }
            public bool Enabled { get; set; } = true;
        }

        private void BuildUi()
        {
            // Group box
            grpCameras = new GroupBox
            {
                Text = "Cameras",
                Dock = DockStyle.Fill
            };
            Controls.Add(grpCameras);

            // Grid
            grdCameras = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false, // we control deletion via button
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = false
            };
            grpCameras.Controls.Add(grdCameras);

            grdCameras.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Name",
                DataPropertyName = nameof(CameraItem.Name),
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });
            grdCameras.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "IP Address",
                DataPropertyName = nameof(CameraItem.IpAddress),
                Width = 120
            });
            grdCameras.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "RTSP URL",
                DataPropertyName = nameof(CameraItem.RtspUrl),
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });
            grdCameras.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Folder Name",
                DataPropertyName = nameof(CameraItem.FolderName),
                Width = 140
            });
            grdCameras.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Tag",
                DataPropertyName = nameof(CameraItem.Tag),
                Width = 140
            });
            grdCameras.Columns.Add(new DataGridViewCheckBoxColumn
            {
                HeaderText = "Enabled",
                DataPropertyName = nameof(CameraItem.Enabled),
                Width = 80
            });

            // Buttons panel
            pnlButtons = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 48
            };
            Controls.Add(pnlButtons);

            btnAdd = new Button
            {
                Text = "Add",
                Width = 90,
                Height = 28,
                Left = 12,
                Top = 10
            };
            btnAdd.Click += BtnAdd_Click;
            pnlButtons.Controls.Add(btnAdd);

            // NEW: Delete button
            btnDelete = new Button
            {
                Text = "Delete",
                Width = 90,
                Height = 28,
                Left = btnAdd.Right + 10,
                Top = 10
            };
            btnDelete.Click += BtnDelete_Click;
            pnlButtons.Controls.Add(btnDelete);

            btnSave = new Button
            {
                Text = "Save",
                Width = 90,
                Height = 28,
                Left = btnDelete.Right + 10,
                Top = 10
            };
            btnSave.Click += BtnSave_Click;
            pnlButtons.Controls.Add(btnSave);

            btnClose = new Button
            {
                Text = "Close",
                Width = 90,
                Height = 28,
                Top = 10,
                Anchor = AnchorStyles.Right | AnchorStyles.Top
            };
            btnClose.Left = pnlButtons.Width - btnClose.Width - 12;
            pnlButtons.Resize += (s, e) =>
            {
                btnClose.Left = pnlButtons.Width - btnClose.Width - 12;
            };
            btnClose.Click += (s, e) => Close();
            pnlButtons.Controls.Add(btnClose);

            // Bind list
            grdCameras.DataSource = _cameraList;
        }

        private void LoadCameras()
        {
            try
            {
                if (!File.Exists(SettingsPath))
                {
                    _fullJson = new JObject();
                    _fullJson["Cameras"] = new JArray();
                }
                else
                {
                    var jsonText = File.ReadAllText(SettingsPath);
                    _fullJson = JObject.Parse(jsonText);

                    if (_fullJson["Cameras"] == null || _fullJson["Cameras"].Type != JTokenType.Array)
                    {
                        _fullJson["Cameras"] = new JArray();
                    }
                }

                var cams = (JArray)_fullJson["Cameras"];

                _cameraList.RaiseListChangedEvents = false;
                _cameraList.Clear();

                foreach (var c in cams)
                {
                    _cameraList.Add(new CameraItem
                    {
                        Name = (string)c["Name"] ?? "",
                        IpAddress = (string)c["IpAddress"] ?? "",
                        RtspUrl = (string)c["RtspUrl"] ?? "",
                        FolderName = (string)c["FolderName"] ?? "",
                        Tag = (string)c["Tag"] ?? "",
                        Enabled = c["Enabled"] != null ? (bool)c["Enabled"] : true
                    });
                }

                _cameraList.RaiseListChangedEvents = true;
                _cameraList.ResetBindings();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load cameras: {ex.Message}", "Camera Manager", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            _cameraList.Add(new CameraItem
            {
                Name = "New Camera",
                IpAddress = "",
                RtspUrl = "",
                FolderName = "",
                Tag = "",
                Enabled = true
            });

            grdCameras.ClearSelection();
            int index = _cameraList.Count - 1;
            if (index >= 0)
            {
                grdCameras.Rows[index].Selected = true;
                grdCameras.CurrentCell = grdCameras.Rows[index].Cells[0];
                grdCameras.BeginEdit(true);
            }
        }

        // NEW: Delete selected camera
        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (grdCameras.CurrentRow == null)
            {
                MessageBox.Show("Select a camera row to delete.", "Camera Manager", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int idx = grdCameras.CurrentRow.Index;
            if (idx < 0 || idx >= _cameraList.Count) return;

            var cam = _cameraList[idx];
            var confirm = MessageBox.Show(
                $"Delete camera '{cam?.Name}'?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm == DialogResult.Yes)
            {
                _cameraList.RemoveAt(idx);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                grdCameras.EndEdit();

                // Rebuild Cameras array from binding list (only update Cameras; keep top-level settings intact)
                var newCams = new JArray();
                foreach (var cam in _cameraList)
                {
                    var obj = new JObject
                    {
                        ["Name"] = cam.Name ?? "",
                        ["IpAddress"] = cam.IpAddress ?? "",
                        ["RtspUrl"] = cam.RtspUrl ?? "",
                        ["FolderName"] = cam.FolderName ?? "",
                        ["Tag"] = cam.Tag ?? "",
                        ["Enabled"] = cam.Enabled
                    };
                    newCams.Add(obj);
                }

                if (_fullJson == null) _fullJson = new JObject();
                _fullJson["Cameras"] = newCams;

                File.WriteAllText(SettingsPath, _fullJson.ToString(Newtonsoft.Json.Formatting.Indented));
                MessageBox.Show("Camera details saved.", "Camera Manager", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save cameras: {ex.Message}", "Camera Manager", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}