using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FfplayTest
{
    public partial class frmSplashSyste : Form
    {
        public frmSplashSyste()
        {
            InitializeComponent();
        }

        private void tmrLoader_Tick(object sender, EventArgs e)
        {
            tmrCloser.Enabled = true;
            tmrLoader.Enabled = false;
        }

        private void tmrCloser_Tick(object sender, EventArgs e)
        {
            this.Hide();
            frmDisplayScreen frmDisplay = new frmDisplayScreen();
            tmrCloser.Enabled = false;
            frmDisplay.ShowDialog();
            this.Close();
        }
    }
}
