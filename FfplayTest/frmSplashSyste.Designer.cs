namespace FfplayTest
{
    partial class frmSplashSyste
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
            this.components = new System.ComponentModel.Container();
            this.lblNotice = new System.Windows.Forms.Label();
            this.lblTrialVersion = new System.Windows.Forms.Label();
            this.tmrLoader = new System.Windows.Forms.Timer(this.components);
            this.tmrCloser = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // lblNotice
            // 
            this.lblNotice.AutoSize = true;
            this.lblNotice.BackColor = System.Drawing.Color.Transparent;
            this.lblNotice.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNotice.Location = new System.Drawing.Point(62, 332);
            this.lblNotice.Name = "lblNotice";
            this.lblNotice.Size = new System.Drawing.Size(288, 19);
            this.lblNotice.TabIndex = 0;
            this.lblNotice.Text = "All Rights received by devsharp.NET Corp";
            // 
            // lblTrialVersion
            // 
            this.lblTrialVersion.AutoSize = true;
            this.lblTrialVersion.BackColor = System.Drawing.Color.Transparent;
            this.lblTrialVersion.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTrialVersion.ForeColor = System.Drawing.Color.Maroon;
            this.lblTrialVersion.Location = new System.Drawing.Point(62, 372);
            this.lblTrialVersion.Name = "lblTrialVersion";
            this.lblTrialVersion.Size = new System.Drawing.Size(263, 19);
            this.lblTrialVersion.TabIndex = 3;
            this.lblTrialVersion.Text = "4.01 - Recorder Version - Beta Version";
            // 
            // tmrLoader
            // 
            this.tmrLoader.Enabled = true;
            this.tmrLoader.Interval = 5000;
            this.tmrLoader.Tick += new System.EventHandler(this.tmrLoader_Tick);
            // 
            // tmrCloser
            // 
            this.tmrCloser.Interval = 500;
            this.tmrCloser.Tick += new System.EventHandler(this.tmrCloser_Tick);
            // 
            // frmSplashSyste
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::NVRRecordingSystem.Properties.Resources.CCTVEnterprise;
            this.ClientSize = new System.Drawing.Size(400, 400);
            this.Controls.Add(this.lblTrialVersion);
            this.Controls.Add(this.lblNotice);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmSplashSyste";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmSplashSyste";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblNotice;
        private System.Windows.Forms.Label lblTrialVersion;
        private System.Windows.Forms.Timer tmrLoader;
        private System.Windows.Forms.Timer tmrCloser;
    }
}