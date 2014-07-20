using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BotVisualServer
{
    public partial class frmLoading : Form
    {
        public frmLoading()
        {
            InitializeComponent();
        }

        private void frmLoading_Load(object sender, EventArgs e)
        {

            this.Cursor = Cursors.WaitCursor;
            this.TopMost = true;
            this.BringToFront();
        }

        public void loadStrategy()
        {
            this.BackgroundImage = BotVisualServer.Properties.Resources.loading2;
            this.Cursor = Cursors.WaitCursor;
            this.TopMost = true;
            this.BringToFront();

        }
    }
}
