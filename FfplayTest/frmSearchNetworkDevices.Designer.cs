namespace FfplayTest
{
    partial class frmSearchNetworkDevices
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
            this.btnCameraSearch = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnCameraSearch
            // 
            this.btnCameraSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCameraSearch.Location = new System.Drawing.Point(12, 12);
            this.btnCameraSearch.Name = "btnCameraSearch";
            this.btnCameraSearch.Size = new System.Drawing.Size(140, 38);
            this.btnCameraSearch.TabIndex = 0;
            this.btnCameraSearch.Text = "Search ";
            this.btnCameraSearch.UseVisualStyleBackColor = true;
            this.btnCameraSearch.Click += new System.EventHandler(this.button1_Click);
            // 
            // frmSearchNetworkDevices
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(714, 374);
            this.Controls.Add(this.btnCameraSearch);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmSearchNetworkDevices";
            this.Text = "Search Devices";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCameraSearch;
    }
}