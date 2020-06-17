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
        private readonly IAccidentService accidentService;

        public MainScreen(IDbService dbService, ITripService tripService, IShipService shipService,
                          IAgentService agentService, IProductService productService, IPortService portService,
                          IPlatformService platformService, IAccidentService accidentService)
        {
            InitializeComponent();
            this.dbService = dbService;
            this.tripService = tripService;
            this.shipService = shipService;
            this.agentService = agentService;
            this.productService = productService;
            this.portService = portService;
            this.platformService = platformService;
            this.accidentService = accidentService;
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
                FillCmbShips(AddTrip_CmbShips);
                FillCmbShips(ManageAcc_cmbShipName); 
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

        //string TranslateEnum(Type enumType, int value)
        //{ 
        
        //}

        List<Trip> allTrips;
        List<TripsForDGV> tripsForDGV;

        void FillTripsDataGridView()
        {
            allTrips = tripService.GetAllTrips();
            tripsForDGV = allTrips.Select<Trip,TripsForDGV> (t => new TripsForDGV
            {
                TripId = t.TripId.ToString(),
                ShipName = t.Ship.Name,
                ShipIMO = t.Ship.Imo,
                ShipType = Enum.GetName(typeof(ShipTypes), t.Ship.Type),
                TripStatus = Enum.GetName(typeof(TripStatus), t.Status),
                TripStatusDate = t.TripsStatus.FirstOrDefault(s => s.TripId == t.TripId && s.Status == t.Status).Date.ToString("MMM dd yyyy"),
                Agent = t.Agent != null ? t.Agent.Name : "",
                Port = t.Port != null ? t.Port.Name : "",
                Platform = t.Platform != null ? t.Platform.Name : "",
                Notes = t.Notes,
                TripStatusVal = t.Status
            }).ToList();

            TripsDGV.DataSource = tripsForDGV;
            TripsDGV.Columns[0].HeaderText = "رقم الرحلة";
            TripsDGV.Columns[1].HeaderText = "السفينة";
            TripsDGV.Columns[2].HeaderText = "IMO";
            TripsDGV.Columns[3].HeaderText = "النوع";
            TripsDGV.Columns[4].HeaderText = "الحالة";
            TripsDGV.Columns[5].HeaderText = "تاريخ الحالة";
            TripsDGV.Columns[6].HeaderText = "الوكيل الملاحى";
            TripsDGV.Columns[7].HeaderText = "الميناء";
            TripsDGV.Columns[8].HeaderText = "الرصيف";
            TripsDGV.Columns[9].HeaderText = "ملاحظات";
            TripsDGV.Columns[10].Visible = false;
            TripsDGV.Columns[0].Width = 85;
            TripsDGV.Columns[2].Width = 75;
            TripsDGV.Columns[5].Width = 90;
        }

        void FillCmbPort(ComboBox cmb)
        {
            var ports = portService.GetAllPorts().Select(p => new { PortId = p.PortId, PortName = p.Name }).ToList();
            FillList(cmb, ports, "PortId", "PortName");
        }

        void FillPlatformCmbPorts()
        {
            FillCmbPort(Platforms_cmbPort);
        }

        private void MainScreen_Load(object sender, EventArgs e)
        {
            FillTripsDataGridView();
            FillShipsGridView();
            FillCmbShips(AddTrip_CmbShips);
            FillCmbShips(ManageAcc_cmbShipName);
            FillAddTripCmbAgents();
            FillAddTripCmbPorts();
            FillPlatformCmbPorts();
            FillAddTripCmbProducts();
            FillPortsList();
            FillProductsList();
            FillPlatformsDataGridView();
            FillAgentsList();
            FillAccidentsDGV();
            AddTrip_CmbStatus.DataSource = Enum.GetValues(typeof(TripStatus));
            AddShip_Typecmb.DataSource = Enum.GetValues(typeof(ShipTypes));
            ManageAcc_cmbArea.DataSource = Enum.GetValues(typeof(AccidentArea));
            Trips_cmbSearchFields.SelectedIndex = 0;
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
            var platforms = platformService.GetAllPlatforms().Select(p => new { platformId = p.PlatformId, PlatformName = p.Name, PortName = p.Port.Name }).ToList();
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
                        FillCmbShips(AddTrip_CmbShips);
                        FillCmbShips(ManageAcc_cmbShipName);
                        MessageBox.Show("تم الحذف بنجاح", "تم الحذف", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("لم يتم الحذف", "فشل الحذف", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        void FillCmbShips(ComboBox cmb)
        {
            var ships = shipService.GetAllShips().Select(s => new { ShipId = s.ShipId, ShipName = s.Name }).ToList();
            cmb.ValueMember = "ShipId";
            cmb.DisplayMember = "ShipName";
            cmb.DataSource = ships;
        }

        void FillAddTripCmbAgents()
        {
            var agents = agentService.GetAllAgents().Select(a => new { AgentId = a.AgentId, AgentName = a.Name }).ToList();
            FillList(AddTrip_CmbAgents, agents, "AgentId", "AgentName");
        }

        void FillAddTripCmbPorts()
        {
            FillCmbPort(AddTrip_CmbPorts);
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

        Dictionary<int, decimal> TripShipLoad = new Dictionary<int, decimal>();

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
            decimal quantity = AddTrip_nudProductQuantity.Value;

            if (TripShipLoad.ContainsKey(productId))
            {
                TripShipLoad[productId] = quantity;
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
                Status = Convert.ToInt32(AddTrip_CmbStatus.SelectedValue),
                PortId = Convert.ToInt32(AddTrip_CmbPorts.SelectedValue),
                PlatformId = Convert.ToInt32(AddTrip_CmbPlatforms.SelectedValue)
            };

            if (AddTrip_btnSaveTrip.Tag == null)
            {
                foreach (var item in TripShipLoad)
                {
                    trip.TripsLoads.Add(new TripsLoad { ProductId = item.Key, Quantity = item.Value, TripId = trip.TripId });
                }
                trip.TripsStatus.Add(new TripsStatu { TripId = trip.TripId, Status = trip.Status, Date = AddTrip_dtpDate.Value });

                tripService.AddTrip(trip);
            }
            else
            { 
                trip.TripId = Convert.ToInt32(AddTrip_btnSaveTrip.Tag);
                
                var currentStatus = trip.TripsStatus.FirstOrDefault(s => s.TripId == trip.TripId && s.Status == trip.Status);
                
                if (currentStatus == null)
                    trip.TripsStatus.Add(new TripsStatu { TripId = trip.TripId, Status = trip.Status, Date = AddTrip_dtpDate.Value });
                else
                {
                    if (currentStatus.Date.ToShortDateString() != AddTrip_dtpDate.Value.ToShortDateString())
                        currentStatus.Date = AddTrip_dtpDate.Value;
                }

                foreach (var item in TripShipLoad)
                {
                    var load = trip.TripsLoads.FirstOrDefault(l => l.TripId == trip.TripId && l.ProductId == item.Key);
                    if (load != null)
                    {
                        load.Quantity = item.Value;
                    }
                    else
                    {
                        load = new TripsLoad { TripId = trip.TripId, ProductId = item.Key, Quantity = item.Value};
                        trip.TripsLoads.Add(load);
                    }
                }

                tripService.UpdateTrip(trip);
                AddTrip_btnSaveTrip.Tag = null;
            }

            if (dbService.Commit())
            {
                AddTripRestControls();
                FillTripsDataGridView();
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

        void AddTripRestControls()
        {
            AddTrip_CmbShips.SelectedIndex = 0;
            AddTrip_CmbAgents.SelectedIndex = 0;
            AddTrip_CmbPorts.SelectedIndex = 0;
            AddTrip_CmbPlatforms.SelectedIndex = 0;
            AddTrip_CmbStatus.SelectedIndex = 0;
            AddTrip_CmbProducts.SelectedIndex = 0;
            AddTrip_nudProductQuantity.Value = AddTrip_nudProductQuantity.Minimum;
            AddTrip_txtNotes.Clear();
            TripShipLoad.Clear();
            AddTrip_dtpDate.ResetText();
        }

        private void AddTrip_btnCancelTrip_Click(object sender, EventArgs e)
        {
            AddTripRestControls();
        }

        private void Trips_btnEdit_Click(object sender, EventArgs e)
        {
            AddTrip_btnSaveTrip.Tag = TripsDGV.CurrentRow.Cells[0].Value;
            AddTrip_CmbShips.Text = TripsDGV.CurrentRow.Cells[1].Value.ToString();
            AddTrip_dtpDate.Value = Convert.ToDateTime(TripsDGV.CurrentRow.Cells[5].Value.ToString());
            AddTrip_CmbAgents.Text = TripsDGV.CurrentRow.Cells[6].Value.ToString();
            AddTrip_CmbPorts.Text = TripsDGV.CurrentRow.Cells[7].Value.ToString();
            AddTrip_CmbPlatforms.Text = TripsDGV.CurrentRow.Cells[8].Value.ToString();
            AddTrip_txtNotes.Text = TripsDGV.CurrentRow.Cells[9].Value.ToString();
            AddTrip_CmbStatus.SelectedItem = (TripStatus)TripsDGV.CurrentRow.Cells[10].Value;

            TripShipLoad.Clear();
            foreach (TripsLoad item in allTrips.Find(t => t.TripId == Convert.ToInt32(TripsDGV.CurrentRow.Cells[0].Value)).TripsLoads)
            {
                TripShipLoad.Add(item.ProductId, item.Quantity);
            }
            FillAddTripDGVShipLoad();
            triptabControl.SelectedTab = addingTripTab;
        }

        private void Trips_btnDelete_Click(object sender, EventArgs e)
        {
            if (TripsDGV.CurrentRow != null)
            {
                if (MessageBox.Show("هل تريد حذف الرحلة?", "حذف الرحلة", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                {
                    var tripId = Convert.ToInt32(ShipsGridView.CurrentRow.Cells[0].Value);
                    tripService.DeleteTrip(tripId);

                    if (dbService.Commit())
                    {
                        FillTripsDataGridView();
                        MessageBox.Show("تم الحذف بنجاح", "تم الحذف", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("لم يتم الحذف", "فشل الحذف", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        void FilterTripsDGV()
        {
            string text = Trips_txtSearch.Text.Trim();

            switch (Trips_cmbSearchFields.Text)
            {
                case "رقم الرحلة":
                    TripsDGV.DataSource = tripsForDGV.Where(t => t.TripId.Contains(text)).ToList();
                    break;
                case "السفينة":
                    TripsDGV.DataSource = tripsForDGV.Where(t => t.ShipName.Contains(text)).ToList();
                    break;
                case "IMO":
                    TripsDGV.DataSource = tripsForDGV.Where(t => t.ShipIMO.Contains(text)).ToList();
                    break;
                case "النوع":
                    TripsDGV.DataSource = tripsForDGV.Where(t => t.ShipType.Contains(text)).ToList();
                    break;
                case "الحالة":
                    TripsDGV.DataSource = tripsForDGV.Where(t => t.TripStatus.Contains(text)).ToList();
                    break;
                case "تاريخ الحالة":
                    TripsDGV.DataSource = tripsForDGV.Where(t => t.TripStatusDate.Contains(text)).ToList();
                    break;
                case "الوكيل الملاحى":
                    TripsDGV.DataSource = tripsForDGV.Where(t => t.Agent.Contains(text)).ToList();
                    break;
                case "الميناء":
                    TripsDGV.DataSource = tripsForDGV.Where(t => t.Port.Contains(text)).ToList();
                    break;
                case "الرصيف":
                    TripsDGV.DataSource = tripsForDGV.Where(t => t.Platform.Contains(text)).ToList();
                    break;
                case "ملاحظات":
                    TripsDGV.DataSource = tripsForDGV.Where(t => t.Notes.Contains(text)).ToList();
                    break;
            }
        }

        private void Trips_btnClearSearch_Click(object sender, EventArgs e)
        {
            Trips_txtSearch.Clear();
            Trips_cmbSearchFields.SelectedIndex = 0;
            TripsDGV.DataSource = tripsForDGV;
        }

        private void Trips_txtSearch_TextChanged(object sender, EventArgs e)
        {
            FilterTripsDGV();
        }

        private void Trips_cmbSearchFields_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterTripsDGV();
        }

        private void TripsDGV_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void ManageAcc_savebtn_Click(object sender, EventArgs e)
        {
            var accident = new Accident
            {
                Details = ManageAcc_txtDetails.Text,
                Area = Convert.ToInt32(ManageAcc_cmbArea.SelectedValue),
                Date = ManageAcc_dtpDate.Value,
                CrewConequences = ManageAcc_txtCrewConsequences.Text,
                CrewAction = ManageAcc_txtCrewAction.Text,
                CostalStateAction = ManageAcc_txtCoast.Text,
                ReportedTo = ManagAcc_txtReportedTo.Text,
                longitude = ManageAcc_txtLong.Text,
                latitude = ManagAcc_txtLati.Text,
                ShipId = Convert.ToInt32(ManageAcc_cmbShipName.SelectedValue),
                IsReported = ManageAcc_CheckReported.Checked,
            };

            if (ManageAcc_savebtn.Tag == null)
            {
                accidentService.AddAccident(accident);
            }
            else
            {
                accident.AccidentId = Convert.ToInt32(ManageAcc_savebtn.Tag);
                accidentService.UpdateAccident(accident);
                ManageAcc_savebtn.Tag = null;
            }
            if (dbService.Commit())
            {
                FillAccidentsDGV();
                ClearManageAccidentControls();
                MessageBox.Show("تم الحفظ بنجاح", "تم الحفظ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("لم يتم الحفظ", "فشل الحفظ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        void ClearManageAccidentControls()
        {
            
            ManageAcc_cmbShipName.SelectedIndex = 0;
            ManageAcc_txtDetails.Clear();
            ManageAcc_txtLong.Clear();
            ManagAcc_txtLati.Clear();
            ManageAcc_txtCrewConsequences.Clear();
            ManageAcc_txtCrewAction.Clear();
                
            ManageAcc_txtCrewConseq.Clear();
            ManageAcc_txtCoast.Clear();
            ManagAcc_txtReportedTo.Clear();
            ManageAcc_cmbArea.SelectedIndex = 0;
            ManageAcc_CheckReported.Checked = false;
            ManageAcc_dtpDate.ResetText();
        }

        void FillAccidentsDGV()
        {
          var accidents=  accidentService.GetAllAccidents().Select(a => new { AccidentId = a.AccidentId,
              ShipName = a.Ship.Name, IMO = a.Ship.Imo, ShipType = Enum.GetName(typeof(ShipTypes), a.Ship.Type), Date = a.Date, Area = Enum.GetName(typeof(AccidentArea), a.Area), Lat = a.latitude,
              longi = a.longitude, Details = a.Details,CrewAction = a.CrewAction, CrewConsequence = a.CrewConequences,
              IsReported = a.IsReported.HasValue && a.IsReported.Value?"تم الإبلاغ" : "لم يتم الإبلاغ", RportedTo = a.ReportedTo, ConstAction = a.CostalStateAction }).ToList();
            Accidents_DGV.DataSource = accidents;
            Accidents_DGV.Columns[0].Visible = false;
            Accidents_DGV.Columns[1].HeaderText = "السفينة";
            Accidents_DGV.Columns[2].HeaderText = "IMO";
            Accidents_DGV.Columns[3].HeaderText = "نوع السفينة";
            Accidents_DGV.Columns[4].HeaderText = "التاريخ";
            Accidents_DGV.Columns[5].HeaderText = "المنطقة";
            Accidents_DGV.Columns[6].HeaderText = "خط العرض";
            Accidents_DGV.Columns[7].HeaderText = "خط الطول";
            Accidents_DGV.Columns[8].HeaderText = "تفاصيل الحادث";
            Accidents_DGV.Columns[9].HeaderText = "اضرارالطاقم";
            Accidents_DGV.Columns[10].HeaderText = "الاجراء المتخذ من الطاقم";
            Accidents_DGV.Columns[11].HeaderText = "البلاغ";
            Accidents_DGV.Columns[12].HeaderText = "الجهةالمبلغة";
            Accidents_DGV.Columns[13].HeaderText = "الاجراء المتخذ من الدولة الساحلية";
        }

        private void MangeAcc_linkShip_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            triptabControl.SelectedTab = shipsTab;
        }

        private void AddTrip_CmbShips_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void ManageAcc_canclbtn_Click(object sender, EventArgs e)
        {
            ClearManageAccidentControls();
        }
    }
}
