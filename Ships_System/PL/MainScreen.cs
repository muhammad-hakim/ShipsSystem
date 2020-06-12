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


        public MainScreen(IDbService dbService, ITripService tripService, IShipService shipService, IAgentService agentService, IProductService productService, IPortService portService, IPlatformService platformService)
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
            var agents = agentService.GetAllAgents().Select(a => new { AgentId = a.AgentId, AgentName = a.Name }).ToList();
            FillList(Agents_lstAgents, agents, "AgentId", "AgentName");
        }
        void FillPortsList()
        {
            var ports = portService.GetAllPorts().Select(p => new { PortId = p.PortId, PortName = p.Name }).ToList();
            FillList(Ports_lstPorts, ports, "PortId", "PortName");
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

        private void MainScreen_Load(object sender, EventArgs e)
        {
            FillShipsGridView();
            FillAddTripCmbShips();
            FillAddTripCmbAgents();
            FillAddTripCmbPorts();
            FillAddTripCmbProducts();
            FillPortsList();
            FillProductsList();
            FillPlatformsDataGridView();
            FillAgentsList();
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
            var products = productService.GetAllProducts().Select(p => new { ProductId = p.ProductId, ProdcutName = p.Name }).ToList();
            FillList(Products_lstProducts, products, "ProductId", "ProdcutName");
        }

        private void Products_btnSave_Click(object sender, EventArgs e)
        {
            Product product = new Product
            {
                Name = Products_txtProductName.Text,
            };
            if (Products_btnSave.Tag == null)
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

        private void Platforms_btnSave_Click(object sender, EventArgs e)
        {
            var platform = new Platform { PortId = Convert.ToInt32(Platforms_cmbPort.SelectedValue), Name = Platforms_txtName.Text };

            if (Platforms_btnSave.Tag == null)
            {
                platformService.AddPlatform(platform);
            }
            else
            {
                platform.PlatformId = Convert.ToInt32(Platforms_btnSave.Tag);
                platformService.UpdatePlatform(platform);
                Platforms_btnSave.Tag = null;
            }

            if (dbService.Commit())
            {
                FillAddTripCmbPlatforms();
                FillPlatformsDataGridView();
                MessageBox.Show("تم الحفظ بنجاح", "تم الحفظ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("لم يتم الحفظ", "فشل الحفظ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void FillPlatformsDataGridView()
        {
            var platforms = platformService.GetAllPlatforms().Select(p => new { platformId = p.PlatformId, PlatformName = p.Name , PortName = p.Port.Name}).ToList();
            Platforms_dgvPlatforms.DataSource = platforms;
            Platforms_dgvPlatforms.Columns[0].Visible = false;
            Platforms_dgvPlatforms.Columns[1].HeaderText = "اسم الرصيف";
            Platforms_dgvPlatforms.Columns[2].HeaderText = "اسم الميناء";
            Platforms_dgvPlatforms.Columns[1].Width = Platforms_dgvPlatforms.Columns[2].Width = 130;
        }

        private void Ports_btnSave_Click(object sender, EventArgs e)
        {
            Port port = new Port
            {
                Name = Ports_txtName.Text
            };
            if (Ports_btnSave.Tag == null)
            {
                portService.AddPort(port);
            }
            else
            {
                port.PortId = Convert.ToInt32(Ports_btnSave.Tag);
                portService.UpdatePort(port);
                Ports_btnSave.Tag = null;
            }
            if (dbService.Commit())
            {
                FillPortsList();
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
                if (MessageBox.Show("هل تريد حذف السفينة?", "حذف السفينة", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
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
            var ships = shipService.GetAllShips().Select(s => new { ShipId = s.ShipId, ShipName = s.Name }).ToList();
            AddTrip_CmbShips.ValueMember = "ShipId";
            AddTrip_CmbShips.DisplayMember = "ShipName";
            AddTrip_CmbShips.DataSource = ships;
        }

        void FillAddTripCmbAgents()
        {
            var agents = agentService.GetAllAgents().Select(a => new { AgentId = a.AgentId, AgentName = a.Name }).ToList();
            FillList(AddTrip_CmbAgents, agents, "AgentId", "AgentName");
        }

        void FillAddTripCmbPorts()
        {
            var ports = portService.GetAllPorts().Select(p => new { PortId = p.PortId, PortName = p.Name }).ToList();
            FillList(AddTrip_CmbPorts, ports, "PortId", "PortName");
        }

        void FillAddTripCmbPlatforms()
        {
            if (AddTrip_CmbPorts.SelectedValue != null)
            {
                var platforms = platformService.GetByPortId(Convert.ToInt32(AddTrip_CmbPorts.SelectedValue)).Select(p => new { PlatformId = p.PlatformId, PlatformName = p.Name }).ToList();
                FillList(AddTrip_CmbPlatforms, platforms, "PlatformId", "PlatformName");
            }
        }

        void FillAddTripCmbProducts()
        {
            var products = productService.GetAllProducts().Select(p => new { ProductId = p.ProductId, ProductName = p.Name }).ToList();
            FillList(AddTrip_CmbProducts, products, "ProductId", "ProductName");
        }

        private void AddTrip_lnkAddPlatform_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (AddTrip_CmbPorts.SelectedValue != null && Convert.ToInt32(AddTrip_CmbPorts.SelectedValue) > 0)
            {
                FillPortsList();
                Platforms_cmbPort.SelectedValue = AddTrip_CmbPorts.SelectedValue;
                triptabControl.SelectedTab = portTab;
            }
        }

        private void AddTrip_CmbPorts_SelectedIndexChanged(object sender, EventArgs e)
        {
            AddTrip_CmbPlatforms.DataSource = null;
            FillAddTripCmbPlatforms();
        }

        void FillList(ListControl lst, object dataSource, string dataFieldName, string textFieldName)
        {
            lst.ValueMember = dataFieldName;
            lst.DisplayMember = textFieldName;
            lst.DataSource = dataSource;
        }

        Dictionary<int, int> TripShipLoad = new Dictionary<int, int>();

        void FillAddTripDGVShipLoad()
        {
            var allProducts = productService.GetAllProducts();
            var shipLoad = new List<dynamic>();
            foreach (var item in TripShipLoad)
            {
                shipLoad.Add(new { productId = item.Key, ProductName = allProducts.Find(p => p.ProductId == item.Key).Name, Quantity = item.Value });
            }

            AddTrip_DGVProducts.DataSource = shipLoad;
            AddTrip_DGVProducts.Columns[0].Visible = false;
            AddTrip_DGVProducts.Columns[1].HeaderText = "الصنف";
            AddTrip_DGVProducts.Columns[2].HeaderText = "الكمية";
            AddTrip_DGVProducts.Columns[1].Width = AddTrip_DGVProducts.Columns[2].Width = 120;
        }

        private void AddTrip_btnAddProduct_Click(object sender, EventArgs e)
        {
            int productId = Convert.ToInt32(AddTrip_CmbProducts.SelectedValue);
            int quantity = Convert.ToInt32(AddTrip_nudProductQuantity.Value);

            if (TripShipLoad.ContainsKey(productId))
            {
                TripShipLoad[productId] = quantity;
                AddTrip_btnAddProduct.Text = "إضافة حمولة";
            }
            else
            {
                TripShipLoad.Add(productId, quantity);
            }
            AddTrip_CmbProducts.SelectedIndex = 0;
            AddTrip_nudProductQuantity.Value = AddTrip_nudProductQuantity.Minimum;
            FillAddTripDGVShipLoad();
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

            foreach (var item in TripShipLoad)
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
            AddShip_Savebtn.Tag = null;
        }

        private void Agents_btnCancel_Click(object sender, EventArgs e)
        {
            Agents_txtAgentName.Clear();
            Agents_btnSave.Tag = null;
        }

        private void Products_btnCancel_Click(object sender, EventArgs e)
        {
            Products_txtProductName.Clear();
            Products_btnSave.Tag = null;
        }

        private void Products_btnEdit_Click(object sender, EventArgs e)
        {
            if (Products_lstProducts.SelectedItem != null)
            {
                Products_btnSave.Tag = Products_lstProducts.SelectedValue;
                Products_txtProductName.Text = Products_lstProducts.Text.ToString();
            }
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
            {
                Agents_btnSave.Tag = Agents_lstAgents.SelectedValue;
                Agents_txtAgentName.Text = Agents_lstAgents.Text.ToString();
            }
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

        private void Ports_btnEdit_Click(object sender, EventArgs e)
        {
            if (Ports_lstPorts.SelectedItem != null)
            {
                Ports_btnSave.Tag = Ports_lstPorts.SelectedValue;
                Ports_txtName.Text = Ports_lstPorts.Text;
            }
        }

        private void Ports_btnCancel_Click(object sender, EventArgs e)
        {
            Ports_btnSave.Tag = null;
            Ports_txtName.Clear();
        }

        private void Ports_btnDelete_Click(object sender, EventArgs e)
        {
            if (Ports_lstPorts.SelectedItem != null)
            {
                if (MessageBox.Show("هل تريد حذف الميناء?", "حذف الميناء", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    var portId = Convert.ToInt32(Ports_lstPorts.SelectedValue);
                    portService.DeletePort(portId);

                    if (dbService.Commit())
                    {
                        FillPortsList();
                        FillAddTripCmbPorts();
                        MessageBox.Show("تم الحذف بنجاح", "تم الحذف", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("لم يتم الحذف", "فشل الحذف", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void Platforms_btnEdit_Click(object sender, EventArgs e)
        {
            if (Platforms_dgvPlatforms.CurrentRow != null)
            {
                Platforms_btnSave.Tag = Platforms_dgvPlatforms.CurrentRow.Cells[0].Value;
                Platforms_txtName.Text = Platforms_dgvPlatforms.CurrentRow.Cells[1].Value.ToString();
                Platforms_cmbPort.Text = Platforms_dgvPlatforms.CurrentRow.Cells[2].Value.ToString();
            }
        }

        private void Platforms_btnDelete_Click(object sender, EventArgs e)
        {
            if (Platforms_dgvPlatforms.CurrentRow != null)
            {
                if (MessageBox.Show("هل تريد حذف الرصيف?", "حذف الرصيف", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    var platformId = Convert.ToInt32(Platforms_dgvPlatforms.CurrentRow.Cells[0].Value);
                    platformService.DeletePlatform(platformId);

                    if (dbService.Commit())
                    {
                        FillPlatformsDataGridView();
                        MessageBox.Show("تم الحذف بنجاح", "تم الحذف", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("لم يتم الحذف", "فشل الحذف", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void Platforms_btnCancel_Click(object sender, EventArgs e)
        {
            Platforms_btnSave.Tag = null;
            Platforms_txtName.Clear();
            Platforms_cmbPort.SelectedIndex = 0;
        }

        private void AddTrip_btnEditProduct_Click(object sender, EventArgs e)
        {
            if (AddTrip_DGVProducts.CurrentRow != null)
            {
                AddTrip_btnAddProduct.Text = "تعديل الحمولة";
                AddTrip_CmbProducts.SelectedValue = AddTrip_DGVProducts.CurrentRow.Cells[0].Value;
                AddTrip_nudProductQuantity.Value = Convert.ToDecimal(AddTrip_DGVProducts.CurrentRow.Cells[2].Value);
            }
        }

        private void AddTrip_btnRemoveProduct_Click(object sender, EventArgs e)
        {
            if (AddTrip_DGVProducts.CurrentRow != null)
            {
                if (MessageBox.Show("هل تريد إزالة هذا المنتج؟", "إزالة منتح", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                {
                    TripShipLoad.Remove(Convert.ToInt32(AddTrip_DGVProducts.CurrentRow.Cells[0].Value));
                }
            }
        }
    }
}

