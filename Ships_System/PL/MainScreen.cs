using Ships_System.BL;
using Ships_System.DAL;
using Ships_System.Enums;
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
            AddShip_Typecmb.DataSource = Enum.GetValues(typeof(Enums.ShipTypes));
            statustxt.DataSource = Enum.GetValues(typeof(Enums.TripStatus));


        }

        async void FillShipsGridView() 
        {
            var data = await shipService.GetAllShipsAsync();
            ShipsGridView.DataSource = data;
        }

        private void AddShip_Savebtn_Click(object sender, EventArgs e)
        {
            Ship ship = new Ship {
                Name = AddShip_Nametxt.Text.Trim(),
                Imo = AddShip_Imotxt.Text.Trim(),
                Type = Convert.ToInt32(AddShip_Typecmb.SelectedValue)};

            shipService.AddShip(ship);
            if (unitOfWork.Commit())
            {
                FillShipsGridView();
            }
        }

        private void AddShip_Typecmb_SelectedIndexChanged(object sender, EventArgs e)
        {
 
        }

        private void statustxt_SelectedIndexChanged(object sender, EventArgs e)
        {
            statustxt.SelectedItem = Enums.TripStatus.LeftDGebouti;
        }

        private void AddShip_Nametxt_TextChanged(object sender, EventArgs e)
        {

        }

        private void ShipsGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void AddShip_Imotxt_TextChanged(object sender, EventArgs e)
        {

        }

        private void MainScreen_Load(object sender, EventArgs e)
        {
            FillShipsGridView();
        }

        private void shipsTab_Click(object sender, EventArgs e)
        {
            FillShipsGridView();
        }

        private void shipsTab_MouseClick(object sender, MouseEventArgs e)
        {
            FillShipsGridView();
        }
    }
}
