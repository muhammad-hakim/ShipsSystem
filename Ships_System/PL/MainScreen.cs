using Ships_System.BL;
using Ships_System.DAL;
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
        private readonly UnitOfWork unitOfWork;
        private readonly IShipService shipService;

        public MainScreen(UnitOfWork unitOfWork, IShipService shipService)
        {
            InitializeComponent();
            this.unitOfWork = unitOfWork;
            this.shipService = shipService;
        }

        void FillShipsGridView() 
        {
            var data = shipService.GetAllShipsAsync();
            ShipsGridView.DataSource = data;
            
        }

        private void AddShip_Savebtn_Click(object sender, EventArgs e)
        {
            Ship ship = new Ship {Name = AddShip_Nametxt.Text, Imo = AddShip_Imotxt.Text, Type = Convert.ToInt32(AddShip_Typecmb.SelectedValue)};
            shipService.AddShip(ship);
            if (unitOfWork.Commit())
            { 
                   
            }
        }
    }
}
