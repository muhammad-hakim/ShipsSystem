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
        private readonly IDbService dbService;
        private readonly ITripService tripService;
        private readonly IShipService shipService;
        private readonly IAgentService agentService;
        private readonly IProductService productService;
        private readonly IPortService portService;
        private readonly IPlatformService platformService;
        

        public MainScreen(IDbService dbService,ITripService tripService, IShipService shipService, IAgentService agentService, IProductService productService, IPortService portService, IPlatformService platformService)
        {
            InitializeComponent();
            this.dbService = dbService;
            this.tripService = tripService;
            this.shipService = shipService;
            this.agentService = agentService;
            this.productService = productService;
            this.portService = portService;
            this.platformService = platformService;
        }

        void FillShipsGridView()
        {
            var data = shipService.GetAllShips().Select(s => new { ShipId = s.ShipId, Name = s.Name, IMO = s.Imo, Type = Enum.GetName(typeof(ShipTypes), s.Type) }).ToList();
            ShipsGridView.DataSource = data;
            ShipsGridView.Columns[0].Visible = false;
            ShipsGridView.Columns[1].HeaderText = "اسم السفينة";
            ShipsGridView.Columns[3].HeaderText = "نوع السفينة";
            ShipsGridView.Columns[1].Width = ShipsGridView.Columns[2].Width = ShipsGridView.Columns[3].Width = 165;
        }

        void FillAgentsList()
        {
            var data = agentService.GetAllAgents().Select(a => new { AgentId = a.AgentId, AgentName = a.Name }).ToList();
            Agents_lstAgents.ValueMember = "AgentId";
            Agents_lstAgents.DisplayMember = "AgentName";
            Agents_lstAgents.DataSource = data;
        }
        void FillPortGridView()
        {
            var data = portService.GetAllPorts().Select(x=>new { x.Name }).ToList();
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
                FillAddTripCmbShips();
                AddShip_Imotxt.Clear();
                AddShip_Nametxt.Clear();
                AddShip_Typecmb.SelectedIndex = 0;
                MessageBox.Show("تم الحفظ بنجاح", "تم الحفظ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("لم يتم الحفظ", "فشل الحفظ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void statustxt_SelectedIndexChanged(object sender, EventArgs e)
        {
            AddTrip_CmbStatus.SelectedItem = TripStatus.LeftDGebouti;
        }

        private void MainScreen_Load(object sender, EventArgs e)
        {
            FillShipsGridView();
            FillAddTripCmbShips();
            FillAddTripCmbAgents();
            FillAddTripCmbPorts();
            FillAddTripCmbProducts();
            AddTrip_CmbStatus.DataSource = Enum.GetValues(typeof(TripStatus));
            AddShip_Typecmb.DataSource = Enum.GetValues(typeof(ShipTypes));
        }

        private void Agents_btnSave_Click(object sender, EventArgs e)
        {
            Agent agent = new Agent
            {
                Name = Agents_txtAgentName.Text,
            };

            if (Agents_btnSave.Tag == null)
            {
                agentService.AddAgent(agent);
            }
            else
            {
                agent.AgentId = Convert.ToInt32(Agents_btnSave.Tag);
                agentService.UpdateAgent(agent);
                Agents_btnSave.Tag = null;
            }
            if (dbService.Commit())
            {
                FillAgentsList();
                FillAddTripCmbAgents();
                MessageBox.Show("تم الحفظ بنجاح", "تم الحفظ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("لم يتم الحفظ", "فشل الحفظ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void FillProductsList()
        {
            var products = productService.GetAllProducts().Select(p =>new {ProductId = p.ProductId, ProdcutName = p.Name }).ToList();
            Products_lstProducts.Items.Clear();
            Products_lstProducts.ValueMember = "ProductId";
            Products_lstProducts.DisplayMember = "ProductName";
            Products_lstProducts.DataSource = products;
        }

        private void Products_btnSave_Click(object sender, EventArgs e)
        {
            Product product = new Product
            {
                Name = Products_txtProductName.Text,
            };
            if (Products_btnSave.Tag==null)
            {
                productService.AddProduct(product);  
            }
            else
            {
                product.ProductId = Convert.ToInt32(Products_btnSave.Tag);
                productService.UpdateProduct(product); ;
                Products_btnSave.Tag = null;
            }
            if (dbService.Commit())
            {
                FillProductsList();
                FillAddTripCmbProducts();
                Products_txtProductName.Clear();
                MessageBox.Show("تم الحفظ بنجاح", "تم الحفظ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("لم يتم الحفظ", "فشل الحفظ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        private void savebutpl_Click(object sender, EventArgs e)
        {
            //var platform = new Platform {PortId = Convert.ToInt32(PlatformTab_cmbPort.SelectedValue), Name = PlatformTab_txtName.Text };
            //platformService.AddPlatform(platform);

            //if (dbService.Commit())
            //{
            //    FillAddTripCmbPlatforms();
            //    FillPlatformsDataGridView();
            //    MessageBox.Show("تم الحفظ بنجاح", "تم الحفظ", MessageBoxButtons.OK , MessageBoxIcon.Information);
            //}
            //else
            //{
            //    MessageBox.Show("لم يتم الحفظ", "فشل الحفظ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
        }

        void FillPlatformsDataGridView()
        {
            var platforms = platformService.GetAllPlatforms().Select(p => new { platformId = p.PlatformId, PlatformName = p.Name});
            //PlatformTab_DGVPlatforms.DataSource = platforms;
        }

        private void saveport_Click(object sender, EventArgs e)
        {
            Port port = new Port
            {
                Name = textBox1.Text
            };
            portService.AddPort(port);
            if (dbService.Commit())
            {
                FillPortGridView();
                FillAddTripCmbPorts();
                MessageBox.Show("تم الحفظ بنجاح", "تم الحفظ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("لم يتم الحفظ", "فشل الحفظ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void linkLship_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            switch ((sender as LinkLabel).Name)
            {
                case "AddTrip_lnkAddShip":
            triptabControl.SelectedTab = shipsTab;
                    break;
                case "AddTrip_lnkAddPort":
                    triptabControl.SelectedTab = portTab;
                    break;
                case "AddTrip_lnkAddProduct":
                    //triptabControl.SelectedTab = goodsTab;
                    break;
                case "AddTrip_lnkAddAgent":
                    triptabControl.SelectedTab = agentsTab;
                    break;
            }
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
                if (MessageBox.Show("هل تريد حذف السفينة?", "حذف السفينة", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    var shipId = Convert.ToInt32(ShipsGridView.CurrentRow.Cells[0].Value);
                    shipService.DeleteShip(shipId);

                    if (dbService.Commit())
                    {
                        FillShipsGridView();
                        FillAddTripCmbShips();
                        MessageBox.Show("تم الحذف بنجاح", "تم الحذف", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("لم يتم الحذف", "فشل الحذف", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        void FillAddTripCmbAgents()
        {
            var agents = agentService.GetAllAgents().Select(a => new { AgentId = a.AgentId, AgentName = a.Name }).ToList();
            FillComboBox(AddTrip_CmbAgents, agents, "AgentId", "AgentName");
        }

        void FillPorts(ComboBox cmb)
        {
            var ports = portService.GetAllPorts().Select(p => new { PortId = p.PortId, PortName = p.Name }).ToList();
            FillComboBox(cmb, ports, "PortId", "PortName");
        }

        void FillAddTripCmbPorts()
        {
            FillPorts(AddTrip_CmbPorts);
        }

        void FillAddTripCmbPlatforms()
        {
            if (AddTrip_CmbPorts.SelectedValue != null)
            {
                var platforms = platformService.GetByPortId(Convert.ToInt32(AddTrip_CmbPorts.SelectedValue)).Select(p => new { PlatformId = p.PlatformId, PlatformName = p.Name }).ToList();
                FillComboBox(AddTrip_CmbPlatforms, platforms, "PlatformId", "PlatformName");
            }
        }

        void FillAddTripCmbProducts()
        {
            var products = productService.GetAllProducts().Select(p => new { ProductId = p.ProductId, ProductName = p.Name }).ToList();
            FillComboBox(AddTrip_CmbProducts, products, "ProductId" , "ProductName");
        }

        private void AddTrip_lnkAddPlatform_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if(AddTrip_CmbPorts.SelectedValue != null && Convert.ToInt32(AddTrip_CmbPorts.SelectedValue) > 0)
            {
                //FillPorts(PlatformTab_cmbPort);
                //PlatformTab_cmbPort.SelectedValue = AddTrip_CmbPorts.SelectedValue;
                //triptabControl.SelectedTab = platformTab;
            }
        }

        private void AddTrip_CmbPorts_SelectedIndexChanged(object sender, EventArgs e)
        {
            AddTrip_CmbPlatforms.DataSource = null;
            FillAddTripCmbPlatforms();
        }

        void FillComboBox(ComboBox cmb, object dataSource, string dataFieldName, string textFieldName)
        {
            cmb.ValueMember = dataFieldName;
            cmb.DisplayMember = textFieldName;
            cmb.DataSource = dataSource;
        }

        Dictionary<int, int> TripGoodsQuantities = new Dictionary<int, int>();

        private void AddTrip_btnAddProduct_Click(object sender, EventArgs e)
        {
            TripGoodsQuantities.Add(Convert.ToInt32(AddTrip_CmbProducts.SelectedValue), Convert.ToInt32(AddTrip_nudProductQuantity.Value));
        }

        private void AddTrip_btnSaveTrip_Click(object sender, EventArgs e)
        {
            var trip = new Trip
            {
                AgentId = Convert.ToInt32(AddTrip_CmbAgents.SelectedValue),
                Notes = AddTrip_txtNotes.Text,
                ShipId = Convert.ToInt32(AddTrip_CmbShips.SelectedValue),
                Status = Convert.ToInt32(AddTrip_CmbStatus.SelectedValue)
            };

            foreach (var item in TripGoodsQuantities)
            {
                trip.TripsLoads.Add(new TripsLoad { ProductId = item.Key, Quantity = item.Value, TripId = trip.TripId });
            }

            trip.TripsStatus.Add(new TripsStatu { TripId = trip.TripId, Status = trip.Status, Date = AddTrip_dtpDate.Value });

            tripService.AddTrip(trip);

            if (dbService.Commit())
            {
                MessageBox.Show("تم الحفظ بنجاح", "تم الحفظ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("لم يتم الحفظ", "فشل الحفظ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddShip_cancelbtn_Click(object sender, EventArgs e)
        {
            AddShip_Imotxt.Clear();
            AddShip_Nametxt.Clear();
            AddShip_Typecmb.SelectedIndex = 0;
        }

        private void Agents_btnCancel_Click(object sender, EventArgs e)
        {
            Agents_txtAgentName.Clear();
        }

        private void Products_btnCancel_Click(object sender, EventArgs e)
        {
            Products_txtProductName.Clear();
        }

        private void Products_btnEdit_Click(object sender, EventArgs e)
        {
            if (Products_lstProducts.SelectedItem != null)
                Products_txtProductName.Text = Products_lstProducts.Text.ToString();
        }

        private void Products_btnDelete_Click(object sender, EventArgs e)
        {
            if (Products_lstProducts.SelectedItem != null)
            {
                if (MessageBox.Show("هل تريد حذف المنتج?", "حذف المنتج", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    productService.DeleteProduct(Convert.ToInt32(Products_lstProducts.SelectedValue));

                    if (dbService.Commit())
                    {
                        FillProductsList();
                        FillAddTripCmbProducts();
                        MessageBox.Show("تم الحذف بنجاح", "تم الحذف", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("لم يتم الحذف", "فشل الحذف", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void Agents_btnEdit_Click(object sender, EventArgs e)
        {
            if (Agents_lstAgents.SelectedItem != null)
                Agents_txtAgentName.Text = Agents_lstAgents.Text.ToString();
        }

        private void Agents_btnDelete_Click(object sender, EventArgs e)
        {
            if (Agents_lstAgents.SelectedItem != null)
            {
                if (MessageBox.Show("هل تريد حذف الوكيل الملاحى?", "حذف الوكيل الملاحى", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    agentService.DeleteAgent(Convert.ToInt32(Agents_lstAgents.SelectedValue));

                    if (dbService.Commit())
                    {
                        FillAgentsList();
                        FillAddTripCmbAgents();
                        MessageBox.Show("تم الحذف بنجاح", "تم الحذف", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("لم يتم الحذف", "فشل الحذف", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}

