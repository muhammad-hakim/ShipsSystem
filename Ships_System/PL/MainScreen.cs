﻿using Ships_System.BL;
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
        private readonly IDbService dbService;
        private readonly IShipService shipService;
        private readonly IAgentService agentService;
        private readonly IProductService productService;
        private readonly IPortService portService;
        private readonly IPlatformService platformService;

        public MainScreen(IDbService dbService, IShipService shipService, IAgentService agentService, IProductService productService, IPortService portService, IPlatformService platformService)
        {
            InitializeComponent();
            this.dbService = dbService;
            this.shipService = shipService;
            this.agentService = agentService;
            this.productService = productService;
            this.portService = portService;
            this.platformService = platformService;
            AddShip_Typecmb.DataSource = Enum.GetValues(typeof(ShipTypes));
            statustxt.DataSource = Enum.GetValues(typeof(TripStatus));
        }

        void FillShipsGridView()
        {
            var data = shipService.GetAllShips().Select(s => new { ShipId = s.ShipId, Name = s.Name, IMO = s.Imo, Type = Enum.GetName(typeof(ShipTypes), s.Type) }).ToList();
            ShipsGridView.DataSource = data;
            ShipsGridView.Columns[0].Visible = false;
            ShipsGridView.Columns[1].HeaderText = "اسم السفينة";
            ShipsGridView.Columns[3].HeaderText = "نوع السفينة";
        }

        void FillAgentGridView()
        {
            var data = agentService.GetAllAgents();
            agentsGridView4.DataSource = data;
        }
        void FillPortGridView()
        {
            var data = portService.GetAllPorts();
            dataGridView2.DataSource = data;
        }
        private void AddShip_Savebtn_Click(object sender, EventArgs e)
        {
            Ship ship = new Ship
            {
                Name = AddShip_Nametxt.Text.Trim(),
                Imo = AddShip_Imotxt.Text.Trim(),
                Type = Convert.ToInt32(AddShip_Typecmb.SelectedValue)
            };
            if (AddShip_Savebtn.Tag == null)
            {
                shipService.AddShip(ship);
            }
            else
            {
                ship.ShipId = Convert.ToInt32(AddShip_Savebtn.Tag);
                shipService.UpdateShip(ship);
                AddShip_Savebtn.Tag = null;
            }
            if (dbService.Commit())
            {
                FillShipsGridView();
                MessageBox.Show("تم الحفظ بنجاح");
            }
            else
            {
                MessageBox.Show("لم يتم الحفظ");
            }
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
            FillAddTripCmbShips();
        }

        private void agentsBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void saveg_Click(object sender, EventArgs e)
        {
            Agent agent = new Agent
            {
                Name = agentsBox.Text,

            };
            agentService.AddAgent(agent);
            MessageBox.Show("تم الحفظ بنجاح");
            if (dbService.Commit())
            {
                FillAgentGridView();
            }
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
            Port port = new Port
            {
                Name = textBox1.Text
            };
            portService.AddPort(port);
            MessageBox.Show("تم الحفظ بنجاح");
            if (dbService.Commit())
            {
                FillPortGridView();
            }
        }

        private void linkLship_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //System.Diagnostics.Process.Start();
        }

        private void ShipsTab_btnEdit_Click(object sender, EventArgs e)
        {
            if (ShipsGridView.CurrentRow != null)
            {
                AddShip_Savebtn.Tag = ShipsGridView.CurrentRow.Cells[0].Value;
                AddShip_Nametxt.Text = ShipsGridView.CurrentRow.Cells[1].Value.ToString();
                AddShip_Imotxt.Text = ShipsGridView.CurrentRow.Cells[2].Value.ToString();
                AddShip_Typecmb.Text = ShipsGridView.CurrentRow.Cells[3].Value.ToString();
            }
        }

        private void ShipsTab_btnDelete_Click(object sender, EventArgs e)
        {
            if (ShipsGridView.CurrentRow != null)
            {
                if (MessageBox.Show("هل تريد حذف السفينة?","حذف السفينة",MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    var shipId = Convert.ToInt32(ShipsGridView.CurrentRow.Cells[0].Value);
                    shipService.DeleteShip(shipId);

                    if (dbService.Commit())
                    {
                        FillShipsGridView();
                        MessageBox.Show("تم الحذف بنجاح");
                    }
                }
            }
        }

        void FillAddTripCmbShips()
        {
            var ships = shipService.GetAllShips().Select(s => new {ShipId = s.ShipId, ShipName = s.Name }).ToList();
            AddTrip_CmbShips.ValueMember = "ShipId";
            AddTrip_CmbShips.DisplayMember = "ShipName";
            AddTrip_CmbShips.DataSource = ships;
        }

        private void triptabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }
    }
}

