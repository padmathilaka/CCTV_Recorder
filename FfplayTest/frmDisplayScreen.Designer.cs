namespace FfplayTest
{
    partial class frmDisplayScreen
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnStartRecording = new System.Windows.Forms.Button();
            this.btnCloseApplication = new System.Windows.Forms.Button();
            this.btnStopRecordings = new System.Windows.Forms.Button();
            this.grpUpdateStatus = new System.Windows.Forms.GroupBox();
            this.grdProcessDisplay = new System.Windows.Forms.DataGridView();
            this.btnUpdateCameraTime = new System.Windows.Forms.Button();
            this.btnUpdateSettings = new System.Windows.Forms.Button();
            this.grpUpdateStatus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdProcessDisplay)).BeginInit();
            this.SuspendLayout();
            // 
            // btnStartRecording
            // 
            this.btnStartRecording.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStartRecording.Location = new System.Drawing.Point(12, 12);
            this.btnStartRecording.Name = "btnStartRecording";
            this.btnStartRecording.Size = new System.Drawing.Size(170, 43);
            this.btnStartRecording.TabIndex = 0;
            this.btnStartRecording.Text = "Start Recording";
            this.btnStartRecording.UseVisualStyleBackColor = true;
            this.btnStartRecording.Click += new System.EventHandler(this.btnStartRecording_Click);
            // 
            // btnCloseApplication
            // 
            this.btnCloseApplication.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCloseApplication.Location = new System.Drawing.Point(12, 319);
            this.btnCloseApplication.Name = "btnCloseApplication";
            this.btnCloseApplication.Size = new System.Drawing.Size(170, 43);
            this.btnCloseApplication.TabIndex = 1;
            this.btnCloseApplication.Text = "Close Application";
            this.btnCloseApplication.UseVisualStyleBackColor = true;
            this.btnCloseApplication.Click += new System.EventHandler(this.btnCloseApplication_Click);
            // 
            // btnStopRecordings
            // 
            this.btnStopRecordings.Enabled = false;
            this.btnStopRecordings.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStopRecordings.Location = new System.Drawing.Point(12, 110);
            this.btnStopRecordings.Name = "btnStopRecordings";
            this.btnStopRecordings.Size = new System.Drawing.Size(170, 43);
            this.btnStopRecordings.TabIndex = 2;
            this.btnStopRecordings.Text = "Stop Recording";
            this.btnStopRecordings.UseVisualStyleBackColor = true;
            this.btnStopRecordings.Click += new System.EventHandler(this.btnStopRecordings_Click);
            // 
            // grpUpdateStatus
            // 
            this.grpUpdateStatus.Controls.Add(this.grdProcessDisplay);
            this.grpUpdateStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpUpdateStatus.Location = new System.Drawing.Point(206, 12);
            this.grpUpdateStatus.Name = "grpUpdateStatus";
            this.grpUpdateStatus.Size = new System.Drawing.Size(532, 350);
            this.grpUpdateStatus.TabIndex = 3;
            this.grpUpdateStatus.TabStop = false;
            this.grpUpdateStatus.Text = "Recording Details";
            // 
            // grdProcessDisplay
            // 
            this.grdProcessDisplay.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdProcessDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdProcessDisplay.Location = new System.Drawing.Point(3, 16);
            this.grdProcessDisplay.Name = "grdProcessDisplay";
            this.grdProcessDisplay.Size = new System.Drawing.Size(526, 331);
            this.grdProcessDisplay.TabIndex = 0;
            // 
            // btnUpdateCameraTime
            // 
            this.btnUpdateCameraTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUpdateCameraTime.Location = new System.Drawing.Point(12, 61);
            this.btnUpdateCameraTime.Name = "btnUpdateCameraTime";
            this.btnUpdateCameraTime.Size = new System.Drawing.Size(170, 43);
            this.btnUpdateCameraTime.TabIndex = 4;
            this.btnUpdateCameraTime.Text = "Update Camera Time";
            this.btnUpdateCameraTime.UseVisualStyleBackColor = true;
            this.btnUpdateCameraTime.Click += new System.EventHandler(this.btnUpdateCameraTime_Click);
            // 
            // btnUpdateSettings
            // 
            this.btnUpdateSettings.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUpdateSettings.Location = new System.Drawing.Point(12, 159);
            this.btnUpdateSettings.Name = "btnUpdateSettings";
            this.btnUpdateSettings.Size = new System.Drawing.Size(170, 43);
            this.btnUpdateSettings.TabIndex = 5;
            this.btnUpdateSettings.Text = "Update System Settings";
            this.btnUpdateSettings.UseVisualStyleBackColor = true;
            this.btnUpdateSettings.Click += new System.EventHandler(this.btnUpdateSettings_Click);
            // 
            // frmDisplayScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(750, 374);
            this.ControlBox = false;
            this.Controls.Add(this.btnUpdateSettings);
            this.Controls.Add(this.btnUpdateCameraTime);
            this.Controls.Add(this.grpUpdateStatus);
            this.Controls.Add(this.btnStopRecordings);
            this.Controls.Add(this.btnCloseApplication);
            this.Controls.Add(this.btnStartRecording);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmDisplayScreen";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ONVIF Support NVR System";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.grpUpdateStatus.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdProcessDisplay)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnStartRecording;
        private System.Windows.Forms.Button btnCloseApplication;
        private System.Windows.Forms.Button btnStopRecordings;
        private System.Windows.Forms.GroupBox grpUpdateStatus;
        private System.Windows.Forms.Button btnUpdateCameraTime;
        private System.Windows.Forms.DataGridView grdProcessDisplay;
        private System.Windows.Forms.Button btnUpdateSettings;
    }
}

