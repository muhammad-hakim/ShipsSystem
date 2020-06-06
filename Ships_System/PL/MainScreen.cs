using Ships_System.BL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ships_System.PL
{
    public partial class MainScreen : Form
    {
        private readonly IShipService shipService;

        public MainScreen(IShipService shipService)
        {
            InitializeComponent();
            this.shipService = shipService;
        }

        private void AddShip_Savebtn_Click(object sender, EventArgs e)
        {

        }
    }
}
