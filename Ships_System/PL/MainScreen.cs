using Ships_System.BL;
using Ships_System.DAL;
using Ships_System.Utilities;
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
        private readonly IAgentService agentService;
        private readonly IProductService productService;
        private readonly IPortService portService;
        private readonly IPlatformService platformService;

        public MainScreen(UnitOfWork unitOfWork, IShipService shipService, IAgentService agentService, IProductService productService, IPortService portService, IPlatformService platformService)
        {
            InitializeComponent();
            this.unitOfWork = unitOfWork;
            this.shipService = shipService;
            this.agentService = agentService;
            this.productService = productService;
            this.portService = portService;
            this.platformService = platformService;
            AddShip_Typecmb.DataSource = Enum.GetValues(typeof(ShipTypes));
            statustxt.DataSource = Enum.GetValues(typeof(TripStatus));
        }

        async void FillShipsGridView()
        {
            var data = (await shipService.GetAllShipsAsync()).Select(s => new { ShipId = s.ShipId, Name = s.Name, IMO = s.Imo, Type = Enum.GetName(typeof(ShipTypes), s.Type)}).ToList();
            ShipsGridView.DataSource = data;
            ShipsGridView.Columns[0].Visible = false;
            ShipsGridView.Columns[1].HeaderText = "اسم السفينة";
            ShipsGridView.Columns[3].HeaderText = "نوع السفينة";
            //todo: rename other columns

            //ShipsGridView.Columns.Add(new DataGridViewColumn {HeaderText = "تعديل", CellTemplate = new  });
        }

        async void FillAgentGridView()
        {
            var data = (await agentService.GetAllAgentsAsync()).Select(x => new
            {
                AgentID = x.AgentId,
                Name = x.Name
             }).ToList();
            
            agentsGridView4.DataSource = data;
            agentsGridView4.Columns[0].Visible = false;
            agentsGridView4.Columns[1].HeaderText = "اسم الوكيل";
        }
        async void FillPortGridView()
        {
            var data = (await portService.GetAllPortsAsync()).Select(x => new
            {
                PortId = x.PortId,
                Name = x.Name
                 }).ToList();
            dataGridView2.DataSource = data;
            dataGridView2.Columns[0].Visible = false;
            dataGridView2.Columns[1].HeaderText = "اسم الميناء";
            
        }
        private void AddShip_Savebtn_Click(object sender, EventArgs e)
        {
            
        }

        private void AddShip_Typecmb_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void statustxt_SelectedIndexChanged(object sender, EventArgs e)
        {
            statustxt.SelectedItem = TripStatus.LeftDGebouti;
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

        private void agentsBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void saveg_Click(object sender, EventArgs e)
        {
           
        }

        private void agentsGridView4_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void productBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void savepro_Click(object sender, EventArgs e)
        {
            Product product = new Product
            {
                Name = productBox6.Text,

            };
            productService.AddProduct(product);
            MessageBox.Show("تم الحفظ بنجاح");
        }

        private void savebutpl_Click(object sender, EventArgs e)
        {


        }

        private void saveport_Click(object sender, EventArgs e)
        {
            
        }

        private void linkLship_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //System.Diagnostics.Process.Start();
        }

        private void bunifuImageButton8_Click(object sender, EventArgs e)
        {

        }

        private void triptabControl_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void deleteproduct_Click(object sender, EventArgs e)
        {

        }

        private void updateproduct_Click(object sender, EventArgs e)
        {

        }

        private void cancelproduct_Click(object sender, EventArgs e)
        {

        }

        private void AddShip_Savebtn_Click_1(object sender, EventArgs e)
        {
            Ship ship = new Ship
            {
                Name = AddShip_Nametxt.Text.Trim(),
                Imo = AddShip_Imotxt.Text.Trim(),
                Type = Convert.ToInt32(AddShip_Typecmb.SelectedValue)
            };

            shipService.AddShip(ship);
            MessageBox.Show("تم الحفظ بنجاح");
            if (unitOfWork.Commit())
            {
                FillShipsGridView();
            }
        }

        private void saveg_Click_1(object sender, EventArgs e)
        {
            Agent agent = new Agent
            {
                Name = agentsBox.Text,

            };
            agentService.AddAgent(agent);
            MessageBox.Show("تم الحفظ بنجاح");
            if (unitOfWork.Commit())
            {
                FillAgentGridView();
            }
        }

        private void bunifuImageButton8_Click_1(object sender, EventArgs e)
        {
            Port port = new Port
            {
                Name = textBox1.Text
            };
            portService.AddPort(port);
            MessageBox.Show("تم الحفظ بنجاح");
            if (unitOfWork.Commit())
            {
                FillPortGridView();

            }
        }

        private void savebutpl12_Click(object sender, EventArgs e)
        {

        }
    }
    }

