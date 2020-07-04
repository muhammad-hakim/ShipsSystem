using iTextSharp.text;
using iTextSharp.text.pdf;
using Ships_System.BL;
using Ships_System.DAL;
using Ships_System.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
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
        private readonly IShipTypesService shipTypesService;

        List<Trip> allTrips;
        List<TripsForDGV> tripsForDGV;
        Dictionary<string, string> ArabicValues = new Dictionary<string, string>();

        public MainScreen(IDbService dbService, ITripService tripService, IShipService shipService,
                          IAgentService agentService, IProductService productService, IPortService portService,
                          IPlatformService platformService, IAccidentService accidentService, IShipTypesService shipTypesService)
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
            this.shipTypesService = shipTypesService;
        }

        void FillShipsGridView()
        {
            var data = shipService.GetAllShips().Select(s => new { ShipId = s.ShipId, Name = s.Name, IMO = s.Imo, Type = s.ShipType.Name }).ToList();
            ShipsGridView.DataSource = data;
            ShipsGridView.Columns[0].Visible = false;
            ShipsGridView.Columns[1].HeaderText = "اسم السفينة";
            ShipsGridView.Columns[3].HeaderText = "نوع السفينة";
            ShipsGridView.Columns[1].Width = ShipsGridView.Columns[2].Width = ShipsGridView.Columns[3].Width = 165;
            ShipsTab_btnDelete.Enabled = ShipsTab_btnEdit.Enabled = ShipsGridView.Rows.Count > 0;
        }
        void FillAgentsList()
        {
            var agents = agentService.GetAllAgents().Select(a => new { AgentId = a.AgentId, AgentName = a.Name }).ToList();
            FillList(Agents_lstAgents, agents, "AgentId", "AgentName");
            Agents_btnDelete.Enabled = Agents_btnEdit.Enabled = Agents_lstAgents.Items.Count > 0;
        }
        void FillPortsList()
        {
            var ports = portService.GetAllPorts().Select(p => new { PortId = p.PortId, PortName = p.Name }).ToList();
            FillList(Ports_lstPorts, ports, "PortId", "PortName");
            Ports_btnDelete.Enabled = Ports_btnEdit.Enabled = Ports_lstPorts.Items.Count > 0;
        }

        private void AddShip_Savebtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(AddShip_Nametxt.Text.Trim()) || string.IsNullOrEmpty(AddShip_Imotxt.Text.Trim()) || AddShip_Typecmb.SelectedValue == null
                || (int)AddShip_Typecmb.SelectedValue == -1 || AddShip_Typecmb.SelectedValue == null || (int)AddShip_Typecmb.SelectedValue == -1)
                MessageBox.Show("من فضلك أدخل الحقول المطلوبة", "حقول مطلوبة", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
            {
                Ship ship = new Ship
                {
                    ShipId = AddShip_Savebtn.Tag == null ? 0 : Convert.ToInt32(AddShip_Savebtn.Tag),
                    Name = AddShip_Nametxt.Text.Trim(),
                    Imo = AddShip_Imotxt.Text.Trim(),
                    Type = Convert.ToInt32(AddShip_Typecmb.SelectedValue)
                };

                AddShip_Savebtn.Tag = null;
                if (shipService.CheckUniqueness(ship))
                {
                    if (ship.ShipId == 0)
                    {
                        shipService.AddShip(ship);
                    }
                    else
                    {
                        shipService.UpdateShip(ship);
                    }
                    if (dbService.Commit())
                    {
                        FillShipsGridView();
                        FillCmbShips(AddTrip_CmbShips);
                        FillCmbShips(ManageAcc_cmbShipName);
                        FillReportsCmbShips();
                        MessageBox.Show("تم الحفظ بنجاح", "تم الحفظ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                        MessageBox.Show("لم يتم الحفظ", "فشل الحفظ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    AddShip_Imotxt.Clear();
                    AddShip_Nametxt.Clear();
                    AddShip_Typecmb.SelectedValue = -1;
                }
                else
                    MessageBox.Show("لم يتم الحفظ .. هذه البيانات مستخدمة مع سفينة أخرى من قبل", "بيانات مكررة", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void FillTripsDataGridView()
        {
            allTrips = tripService.GetAllTrips();
            tripsForDGV = allTrips.Select<Trip, TripsForDGV>(t => new TripsForDGV
            {
                TripId = t.TripId.ToString(),
                ShipName = t.Ship.Name,
                ShipIMO = t.Ship.Imo,
                ShipType = t.Ship.ShipType.Name,
                TripStatus = ArabicValues[Enum.GetName(typeof(TripStatus), t.Status)],
                TripStatusDate = t.TripsStatus.FirstOrDefault(s => s.TripId == t.TripId && s.Status == t.Status).Date.ToShortDateString(),
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
            TripsDGV.Columns[5].HeaderText = "التاريخ";
            TripsDGV.Columns[6].HeaderText = "الوكيل الملاحى";
            TripsDGV.Columns[7].HeaderText = "الميناء";
            TripsDGV.Columns[8].HeaderText = "الرصيف";
            TripsDGV.Columns[9].HeaderText = "ملاحظات";
            TripsDGV.Columns[10].Visible = false;
            TripsDGV.Columns[0].Width = 85;
            TripsDGV.Columns[2].Width = 75;
            TripsDGV.Columns[5].Width = 90;

            Trips_btnDelete.Enabled = Trips_btnEdit.Enabled = TripsDGV.Rows.Count > 0;
        }

        void FillCmbPort(ComboBox cmb)
        {
            var ports = portService.GetAllPorts().Select(p => new { PortId = p.PortId, PortName = p.Name }).ToList();
            ports.Insert(0, new { PortId = -1, PortName = "اختر ميناء" });
            FillList(cmb, ports, "PortId", "PortName");
        }

        void FillPlatformCmbPorts()
        {
            FillCmbPort(Platforms_cmbPort);
        }

        private void MainScreen_Load(object sender, EventArgs e)
        {
            FillTranslationDictionary();
            FillTripsDataGridView();
            FillShipsGridView();
            FillPlatformsDataGridView();
            FillAccidentsDGV();
            FillCmbShips(AddTrip_CmbShips);
            FillCmbShips(ManageAcc_cmbShipName);
            FillAddTripCmbAgents();
            FillAddTripCmbPorts();
            FillPlatformCmbPorts();
            FillAddTripCmbProducts();
            FillPortsList();
            FillProductsList();
            FillAgentsList();
            FillAddShipTypecmb();
            FillCmbAccidentArea();
            FillReportsCmbShips();
            FillReportsCmbAgents();
            FillReportsCmbPorts();
            FillReportsCmbPlatforms();
            FillReportsCmbProducts();
            FillCmbStatus();
            Trips_cmbSearchFields.SelectedIndex = 0;
            Reports_TripsReport_cmbPorts.SelectedValue = -1;
        }

        void FillCmbAccidentArea()
        {
            var items = new List<object>();
            foreach (var item in Enum.GetValues(typeof(AccidentArea)))
            {
                items.Add(new { Id = (int)item, Name = ArabicValues[item.ToString()] });
            }
            items.Insert(0, new { Id = -1, Name = "اختر منطقة" });
            FillList(ManageAcc_cmbArea, items, "Id", "Name");
        }

        void FillCmbStatus()
        {
            var items = new List<object>();
            foreach (var item in Enum.GetValues(typeof(TripStatus)))
            {
                items.Add(new { Id = (int)item, Name = ArabicValues[item.ToString()] });
            }

            FillList(AddTrip_CmbStatus, items, "Id", "Name");

            var TripsReportsItems = new List<object>();
            TripsReportsItems.Add(new { Id = -1, Name = "كل الحالات" });

            foreach (var item in Enum.GetValues(typeof(TripStatus)))
            {
                TripsReportsItems.Add(new { Id = (int)item, Name = ArabicValues[item.ToString()] });
            }
            FillList(Reports_TripsReport_cmbStatus, TripsReportsItems, "Id", "Name");
            FillList(Reports_ShipsStatus_cmbStatus, CloneList(TripsReportsItems), "Id", "Name");
        }

        void FillReportsCmbShips()
        {
            var ships = shipService.GetAllShips().Select(s => new { ShipId = s.ShipId, ShipName = s.Name }).ToList();
            ships.Insert(0, new { ShipId = -1, ShipName = "كل السفن" });

            FillList(Reports_TripsReport_cmbShips, ships, "ShipId", "ShipName");
            FillList(Reports_Visits_cmbShips, CloneList(ships), "ShipId", "ShipName");
        }

        void FillReportsCmbAgents()
        {
            var agents = agentService.GetAllAgents().Select(a => new { AgentId = a.AgentId, AgentName = a.Name }).ToList();
            agents.Insert(0, new { AgentId = -1, AgentName = "كل الوكلاء" });
            FillList(Reports_TripsReport_cmbAgents, agents, "AgentId", "AgentName");
        }

        void FillReportsCmbPorts()
        {
            var ports = portService.GetAllPorts().Select(p => new { PortId = p.PortId, PortName = p.Name }).ToList();
            ports.Insert(0, new { PortId = -1, PortName = "كل الموانئ" });

            FillList(Reports_TripsReport_cmbPorts, ports, "PortId", "PortName");
            FillList(Reports_Visits_cmbPorts, CloneList(ports), "PortId", "PortName");
            FillList(Reports_ShipStaus_cmbPorts, CloneList(ports), "PortId", "PortName");
            FillList(Reports_Visits_cmbPorts, CloneList(ports), "PortId", "PortName");
        }

        List<T> CloneList<T>(List<T> items) where T : class
        {
            List<T> newList = new List<T>();

            foreach (T item in items)
            {
                newList.Add(item);
            }

            return newList;
        }

        void FillReportsCmbPlatforms()
        {
            var platforms = platformService.GetByPortId((int)Reports_ShipStaus_cmbPorts.SelectedValue).Select(p => new { Id = p.PlatformId, Name = p.Name }).ToList();
            platforms.Insert(0, new { Id = -1, Name = "كل الأرصفة" });

            FillList(Reports_ShipStaus_cmbPlatforms, platforms, "Id", "Name");
        }

        void FillReportsCmbProducts()
        {
            var products = productService.GetAllProducts().Select(p => new { ProductId = p.ProductId, ProductName = p.Name }).ToList();
            products.Insert(0, new { ProductId = -1, ProductName = "كل المنتجات" });
            FillList(Reports_quantityReport_cmbProducts, products, "ProductId", "ProductName");
        }

        void FillTranslationDictionary()
        {
            ArabicValues.Add("LeftDGebouti", "غادرت جيبوتى");
            ArabicValues.Add("ReservationArea", "في منطقة الاحتجاز");
            ArabicValues.Add("AtGhates", "في الغاطس");
            ArabicValues.Add("ArriveAtPlatform", "وصلت الارصفة");
            ArabicValues.Add("WaitingAtGhatesAfterUnload", "منتظرة بالغاطس بعد التفريغ");
            ArabicValues.Add("EXecptedTOArrive", "متوقع وصولها");
            ArabicValues.Add("InPortArea", "في الميناء");
            ArabicValues.Add("InTerritorialWater", "في المياه الاقليمية");
            ArabicValues.Add("InInternationalWater", "في المياه الدولية");
        }

        void FillAddShipTypecmb()
        {
            var types = shipTypesService.GetAllShipTypes().Select(t => new { TypeId = t.TypeId, Name = t.Name }).ToList();
            types.Insert(0, new { TypeId = -1, Name = "اختر نوع" });
            AddShip_Typecmb.DisplayMember = "Name";
            AddShip_Typecmb.ValueMember = "TypeId";
            AddShip_Typecmb.DataSource = types;
        }

        private void Agents_btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Agents_txtAgentName.Text.Trim()))
                MessageBox.Show("من فضلك أدخل الحقول المطلوبة", "حقول مطلوبة", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
            {
                Agent agent = new Agent
                {
                    AgentId = Agents_btnSave.Tag == null ? 0 : Convert.ToInt32(Agents_btnSave.Tag),
                    Name = Agents_txtAgentName.Text,
                };

                Agents_btnSave.Tag = null;

                if (agentService.CheckUniqueness(agent))
                {
                    if (agent.AgentId == 0)
                    {
                        agentService.AddAgent(agent);
                    }
                    else
                    {
                        agentService.UpdateAgent(agent);
                    }
                    if (dbService.Commit())
                    {
                        FillAgentsList();
                        FillAddTripCmbAgents();
                        FillReportsCmbAgents();
                        MessageBox.Show("تم الحفظ بنجاح", "تم الحفظ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                        MessageBox.Show("لم يتم الحفظ", "فشل الحفظ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Agents_txtAgentName.Clear();
                }
                else
                    MessageBox.Show("لم يتم الحفظ .. هذه البيانات مستخدمة مع وكيل آخر من قبل", "بيانات مكررة", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void FillProductsList()
        {
            var products = productService.GetAllProducts().Select(p => new { ProductId = p.ProductId, ProdcutName = p.Name }).ToList();
            FillList(Products_lstProducts, products, "ProductId", "ProdcutName");
            Products_btnDelete.Enabled = Products_btnEdit.Enabled = Products_lstProducts.Items.Count > 0;
        }

        private void Products_btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Products_txtProductName.Text.Trim()))
                MessageBox.Show("من فضلك أدخل الحقول المطلوبة", "حقول مطلوبة", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
            {
                Product product = new Product
                {
                    ProductId = Products_btnSave.Tag == null ? 0 : Convert.ToInt32(Products_btnSave.Tag),
                    Name = Products_txtProductName.Text,
                };

                Products_btnSave.Tag = null;

                if (productService.CheckUniqueness(product))
                {
                    if (product.ProductId == 0)
                    {
                        productService.AddProduct(product);
                    }
                    else
                    {
                        productService.UpdateProduct(product); ;
                    }
                    if (dbService.Commit())
                    {
                        FillProductsList();
                        FillAddTripCmbProducts();
                        FillReportsCmbProducts();
                        MessageBox.Show("تم الحفظ بنجاح", "تم الحفظ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                        MessageBox.Show("لم يتم الحفظ", "فشل الحفظ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Products_txtProductName.Clear();
                }
                else
                    MessageBox.Show("لم يتم الحفظ .. هذه البيانات مستخدمة مع منتج آخر من قبل", "بيانات مكررة", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Platforms_btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Platforms_txtName.Text.Trim()) || Platforms_cmbPort.SelectedValue == null || (int)Platforms_cmbPort.SelectedValue == -1)
                MessageBox.Show("من فضلك أدخل الحقول المطلوبة", "حقول مطلوبة", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
            {
                var platform = new Platform
                {
                    PlatformId = Platforms_btnSave.Tag == null ? 0 : Convert.ToInt32(Platforms_btnSave.Tag),
                    PortId = Convert.ToInt32(Platforms_cmbPort.SelectedValue),
                    Name = Platforms_txtName.Text
                };
                Platforms_btnSave.Tag = null;

                if (platformService.CheckUniqueness(platform))
                {
                    if (platform.PlatformId == 0)
                    {
                        platformService.AddPlatform(platform);
                    }
                    else
                    {
                        platformService.UpdatePlatform(platform);
                    }

                    if (dbService.Commit())
                    {
                        FillAddTripCmbPlatforms();
                        FillPlatformsDataGridView();
                        FillReportsCmbPlatforms();
                        MessageBox.Show("تم الحفظ بنجاح", "تم الحفظ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                        MessageBox.Show("لم يتم الحفظ", "فشل الحفظ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Platforms_txtName.Clear();
                    Platforms_cmbPort.SelectedValue = -1;
                }
                else
                    MessageBox.Show("لم يتم الحفظ .. هذه البيانات مستخدمة مع رصيف آخر من قبل", "بيانات مكررة", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            Platforms_btnDelete.Enabled = Platforms_btnEdit.Enabled = Platforms_dgvPlatforms.Rows.Count > 0;
        }

        private void Ports_btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Ports_txtName.Text.Trim()))
                MessageBox.Show("من فضلك أدخل الحقول المطلوبة", "حقول مطلوبة", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
            {
                Port port = new Port
                {
                    PortId = Ports_btnSave.Tag == null ? 0 : Convert.ToInt32(Ports_btnSave.Tag),
                    Name = Ports_txtName.Text
                };
                Ports_btnSave.Tag = null;

                if (portService.CheckUniqueness(port))
                {
                    if (port.PortId == 0)
                    {
                        portService.AddPort(port);
                    }
                    else
                    {
                        portService.UpdatePort(port);
                    }
                    if (dbService.Commit())
                    {
                        FillPortsList();
                        FillAddTripCmbPorts();
                        FillReportsCmbPorts();
                        FillPlatformCmbPorts();
                        MessageBox.Show("تم الحفظ بنجاح", "تم الحفظ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                        MessageBox.Show("لم يتم الحفظ", "فشل الحفظ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Ports_txtName.Clear();
                }
                else
                    MessageBox.Show("لم يتم الحفظ .. هذه البيانات مستخدمة مع ميناء آخر من قبل", "بيانات مكررة", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                    if (shipService.CanDelete(shipId))
                    {
                        shipService.DeleteShip(shipId);

                        if (dbService.Commit())
                        {
                            FillShipsGridView();
                            FillCmbShips(AddTrip_CmbShips);
                            FillCmbShips(ManageAcc_cmbShipName);
                            FillReportsCmbShips();
                            MessageBox.Show("تم الحذف بنجاح", "تم الحذف", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("لم يتم الحذف", "فشل الحذف", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                        MessageBox.Show("لم يتم الحذف .. هذه السفينة مستخدمة حاليا", "فشل الحذف", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        void FillCmbShips(ComboBox cmb)
        {
            var ships = shipService.GetAllShips().Select(s => new { ShipId = s.ShipId, ShipName = s.Name }).ToList();
            ships.Insert(0, new { ShipId = -1, ShipName = "اختر سفينة" });
            cmb.ValueMember = "ShipId";
            cmb.DisplayMember = "ShipName";
            cmb.DataSource = ships;
        }

        void FillAddTripCmbAgents()
        {
            var agents = agentService.GetAllAgents().Select(a => new { AgentId = a.AgentId, AgentName = a.Name }).ToList();
            agents.Insert(0, new { AgentId = -1, AgentName = "اختر وكيل" });
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
                platforms.Insert(0, new { PlatformId = -1, PlatformName = "اختر رصيف" });
                FillList(AddTrip_CmbPlatforms, platforms, "PlatformId", "PlatformName");
            }
        }

        void FillAddTripCmbProducts()
        {
            var products = productService.GetAllProducts().Select(p => new { ProductId = p.ProductId, ProductName = p.Name }).ToList();
            products.Insert(0, new { ProductId = -1, ProductName = "اختر منتج" });
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
            AddTrip_CmbPlatforms.Enabled = AddTrip_CmbStatus.SelectedValue != null && (int)AddTrip_CmbStatus.SelectedValue == (int)TripStatus.ArriveAtPlatform &&
                                           AddTrip_CmbPorts.SelectedValue != null && (int)AddTrip_CmbPorts.SelectedValue != -1;

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
            if (shipLoad.Count() > 0)
            {
                AddTrip_DGVProducts.Columns[0].Visible = false;
                AddTrip_DGVProducts.Columns[1].HeaderText = "الصنف";
                AddTrip_DGVProducts.Columns[2].HeaderText = "الكمية";
                AddTrip_DGVProducts.Columns[1].Width = AddTrip_DGVProducts.Columns[2].Width = 120;
                AddTrip_DGVProducts.CurrentCell = AddTrip_DGVProducts.Rows[0].Cells[1];
            }
        }

        private void AddTrip_btnAddProduct_Click(object sender, EventArgs e)
        {
            if (AddTrip_CmbProducts.SelectedValue == null || (int)AddTrip_CmbProducts.SelectedValue == -1 || AddTrip_nudProductQuantity.Value < 0)
                MessageBox.Show("من فضلك أدخل الحقول المطلوبة", "حقول مطلوبة", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
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
                AddTrip_CmbProducts.SelectedValue = -1;
                AddTrip_nudProductQuantity.Value = AddTrip_nudProductQuantity.Minimum;
                FillAddTripDGVShipLoad();
            }
        }

        private void AddTrip_btnSaveTrip_Click(object sender, EventArgs e)
        {
            if (AddTrip_CmbShips.SelectedValue == null || (int)AddTrip_CmbShips.SelectedValue == -1 || AddTrip_CmbAgents.SelectedValue == null ||
               (int)AddTrip_CmbAgents.SelectedValue == -1 || AddTrip_CmbStatus.SelectedValue == null || (int)AddTrip_CmbStatus.SelectedValue == -1 ||
               AddTrip_CmbPorts.SelectedValue == null || (int)AddTrip_CmbPorts.SelectedValue == -1 || AddTrip_dtpDate.Value == null)
                MessageBox.Show("من فضلك أدخل الحقول المطلوبة", "حقول مطلوبة", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
            if (AddTrip_btnSaveTrip.Tag != null && DateTime.Compare(previousStatusDate.Date, AddTrip_dtpDate.Value.Date) > 0 && previousStatusDate != DateTime.MaxValue)
                MessageBox.Show("من فضلك أدخل تاريخ صالح", "تاريخ غير صالح", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
            {
                Trip trip = new Trip();

                if (AddTrip_btnSaveTrip.Tag != null)
                {
                    trip = tripService.GetTripById(Convert.ToInt32(AddTrip_btnSaveTrip.Tag));
                    AddTrip_btnSaveTrip.Tag = null;
                }

                trip.AgentId = Convert.ToInt32(AddTrip_CmbAgents.SelectedValue);
                trip.Notes = AddTrip_txtNotes.Text;
                trip.ShipId = Convert.ToInt32(AddTrip_CmbShips.SelectedValue);
                trip.Status = Convert.ToInt32(AddTrip_CmbStatus.SelectedValue);
                trip.PortId = Convert.ToInt32(AddTrip_CmbPorts.SelectedValue);
                trip.PlatformId = (AddTrip_CmbPlatforms.SelectedValue != null && (int)AddTrip_CmbPlatforms.SelectedValue != -1) ? Convert.ToInt32(AddTrip_CmbPlatforms.SelectedValue) : (int?)null;

                if (trip.TripId == 0)
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
                            load = new TripsLoad { TripId = trip.TripId, ProductId = item.Key, Quantity = item.Value };
                            trip.TripsLoads.Add(load);
                        }
                    }

                    tripService.UpdateTrip(trip);
                }

                if (dbService.Commit())
                {
                    triptabControl.SelectedTab = tripsTab;
                    FillTripsDataGridView();
                    MessageBox.Show("تم الحفظ بنجاح", "تم الحفظ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                    MessageBox.Show("لم يتم الحفظ", "فشل الحفظ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                AddTripRestControls();
            }
        }

        private void AddShip_cancelbtn_Click(object sender, EventArgs e)
        {
            AddShip_Imotxt.Clear();
            AddShip_Nametxt.Clear();
            AddShip_Typecmb.SelectedValue = -1;
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
                    if (productService.CanDelete(Convert.ToInt32(Products_lstProducts.SelectedValue)))
                    {
                        productService.DeleteProduct(Convert.ToInt32(Products_lstProducts.SelectedValue));

                        if (dbService.Commit())
                        {
                            FillProductsList();
                            FillAddTripCmbProducts();
                            FillReportsCmbProducts();
                            MessageBox.Show("تم الحذف بنجاح", "تم الحذف", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("لم يتم الحذف", "فشل الحذف", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                        MessageBox.Show("لم يتم الحذف .. هذا المنتج مستخدم حاليا", "فشل الحذف", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    if (agentService.CanDelete(Convert.ToInt32(Agents_lstAgents.SelectedValue)))
                    {
                        agentService.DeleteAgent(Convert.ToInt32(Agents_lstAgents.SelectedValue));

                        if (dbService.Commit())
                        {
                            FillAgentsList();
                            FillAddTripCmbAgents();
                            FillReportsCmbAgents();
                            MessageBox.Show("تم الحذف بنجاح", "تم الحذف", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("لم يتم الحذف", "فشل الحذف", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                        MessageBox.Show("لم يتم الحذف .. هذالوكيل الملاحى مستخدم حاليا", "فشل الحذف", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                    if (portService.CanDelete(portId))
                    {
                        portService.DeletePort(portId);

                        if (dbService.Commit())
                        {
                            FillPortsList();
                            FillAddTripCmbPorts();
                            FillReportsCmbPorts();
                            MessageBox.Show("تم الحذف بنجاح", "تم الحذف", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("لم يتم الحذف", "فشل الحذف", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                        MessageBox.Show("لم يتم الحذف .. هذالميناء مستخدم حاليا", "فشل الحذف", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    if (platformService.CanDelete(platformId))
                    {
                        platformService.DeletePlatform(platformId);

                        if (dbService.Commit())
                        {
                            FillAddTripCmbPlatforms();
                            FillPlatformsDataGridView();
                            FillReportsCmbPlatforms();
                            MessageBox.Show("تم الحذف بنجاح", "تم الحذف", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("لم يتم الحذف", "فشل الحذف", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                        MessageBox.Show("لم يتم الحذف .. هذه السفينة مستخدمة حاليا", "فشل الحذف", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void Platforms_btnCancel_Click(object sender, EventArgs e)
        {
            Platforms_btnSave.Tag = null;
            Platforms_txtName.Clear();
            Platforms_cmbPort.SelectedValue = -1;
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
                    FillAddTripDGVShipLoad();
                }
            }
        }

        void AddTripRestControls()
        {
            AddTrip_CmbShips.SelectedValue = -1;
            AddTrip_CmbAgents.SelectedValue = -1;
            AddTrip_CmbPorts.SelectedValue = -1;
            AddTrip_CmbPlatforms.SelectedValue = -1;
            AddTrip_CmbStatus.SelectedValue = -1;
            AddTrip_CmbStatus.Enabled = false;
            AddTrip_CmbProducts.SelectedValue = -1;
            AddTrip_nudProductQuantity.Value = AddTrip_nudProductQuantity.Minimum;
            AddTrip_txtNotes.Clear();
            TripShipLoad.Clear();
            AddTrip_dtpDate.ResetText();
            EditTrip_btnChangeStatus.Visible = false;
            TripShipLoad.Clear();
            FillAddTripDGVShipLoad();
            previousStatusDate = DateTime.MaxValue;
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
            AddTrip_CmbStatus.Text = ArabicValues[Enum.GetName(typeof(TripStatus), TripsDGV.CurrentRow.Cells[10].Value)];

            TripShipLoad.Clear();
            foreach (TripsLoad item in allTrips.Find(t => t.TripId == Convert.ToInt32(TripsDGV.CurrentRow.Cells[0].Value)).TripsLoads)
            {
                TripShipLoad.Add(item.ProductId, item.Quantity);
            }
            FillAddTripDGVShipLoad();
            triptabControl.SelectedTab = addingTripTab;
            EditTrip_btnChangeStatus.Visible = true;
        }

        private void Trips_btnDelete_Click(object sender, EventArgs e)
        {
            if (TripsDGV.CurrentRow != null)
            {
                if (MessageBox.Show("هل تريد حذف الرحلة?", "حذف الرحلة", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                {
                    var tripId = Convert.ToInt32(TripsDGV.CurrentRow.Cells[0].Value);


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

        private void ManageAcc_savebtn_Click(object sender, EventArgs e)
        {
            if (ManageAcc_cmbShipName.SelectedValue == null || (int)ManageAcc_cmbShipName.SelectedValue == -1 || ManageAcc_cmbArea.SelectedValue == null ||
                (int)ManageAcc_cmbArea.SelectedValue == -1 || string.IsNullOrEmpty(ManageAcc_txtLat.Text.Trim()) || string.IsNullOrEmpty(ManageAcc_txtLong.Text.Trim())
                || ManageAcc_dtpDate.Value == null || (ManageAcc_CheckReported.Checked && string.IsNullOrEmpty(ManageAcc_txtReportedTo.Text.Trim())))
                MessageBox.Show("من فضلك أدخل الحقول المطلوبة", "حقول مطلوبة", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
            {
                var accident = new Accident
                {
                    Details = ManageAcc_txtDetails.Text,
                    Area = Convert.ToInt32(ManageAcc_cmbArea.SelectedValue),
                    Date = ManageAcc_dtpDate.Value,
                    CrewConequences = ManageAcc_txtCrewConsequences.Text,
                    CrewAction = ManageAcc_txtCrewAction.Text,
                    CostalStateAction = ManageAcc_txtCoast.Text,
                    ReportedTo = ManageAcc_CheckReported.Checked ? ManageAcc_txtReportedTo.Text : "",
                    longitude = ManageAcc_txtLong.Text,
                    latitude = ManageAcc_txtLat.Text,
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
                    triptabControl.SelectedTab = AccidentManagementTab;
                    MessageBox.Show("تم الحفظ بنجاح", "تم الحفظ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                    MessageBox.Show("لم يتم الحفظ", "فشل الحفظ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ClearManageAccidentControls();
            }
        }

        void ClearManageAccidentControls()
        {

            ManageAcc_cmbShipName.SelectedValue = -1;
            ManageAcc_txtDetails.Clear();
            ManageAcc_txtLong.Clear();
            ManageAcc_txtLat.Clear();
            ManageAcc_txtCrewConsequences.Clear();
            ManageAcc_txtCrewAction.Clear();
            ManageAcc_txtCoast.Clear();
            ManageAcc_txtReportedTo.Clear();
            ManageAcc_cmbArea.SelectedValue = -1;
            ManageAcc_CheckReported.Checked = false;
            ManageAcc_dtpDate.ResetText();
        }

        void FillAccidentsDGV()
        {
            var accidents = accidentService.GetAllAccidents().Select(a => new
            {
                AccidentId = a.AccidentId,
                ShipName = a.Ship.Name,
                IMO = a.Ship.Imo,
                ShipType = a.Ship.ShipType.Name,
                Date = a.Date,
                Area = ArabicValues[Enum.GetName(typeof(AccidentArea), a.Area)],
                Lat = a.latitude,
                longi = a.longitude,
                Details = a.Details,
                CrewAction = a.CrewAction,
                CrewConsequence = a.CrewConequences,
                IsReported = a.IsReported.HasValue && a.IsReported.Value ? "تم الإبلاغ" : "لم يتم الإبلاغ",
                RportedTo = a.ReportedTo,
                ConstAction = a.CostalStateAction,
                IsReportedVal = a.IsReported,
                AreaVal = a.Area
            }).ToList();

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
            Accidents_DGV.Columns[14].Visible = false;
            Accidents_DGV.Columns[15].Visible = false;

            accidents_deletebtn.Enabled = Accident_Upadtebtn.Enabled = Accidents_DGV.Rows.Count > 0;
        }

        private void MangeAcc_linkShip_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            triptabControl.SelectedTab = shipsTab;
        }

        private void ManageAcc_canclbtn_Click(object sender, EventArgs e)
        {
            ClearManageAccidentControls();
        }

        private void Accident_Upadtebtn_Click(object sender, EventArgs e)
        {
            if (Accidents_DGV.CurrentRow != null)
            {
                ManageAcc_savebtn.Tag = Accidents_DGV.CurrentRow.Cells[0].Value;
                ManageAcc_cmbShipName.Text = Accidents_DGV.CurrentRow.Cells[1].Value.ToString();
                ManageAcc_dtpDate.Value = Convert.ToDateTime(Accidents_DGV.CurrentRow.Cells[4].Value.ToString());
                ManageAcc_cmbArea.SelectedItem = (AccidentArea)Accidents_DGV.CurrentRow.Cells[15].Value;
                ManageAcc_txtLat.Text = Accidents_DGV.CurrentRow.Cells[6].Value.ToString();
                ManageAcc_txtLong.Text = Accidents_DGV.CurrentRow.Cells[7].Value.ToString();
                ManageAcc_txtDetails.Text = Accidents_DGV.CurrentRow.Cells[8].Value.ToString();
                ManageAcc_txtCrewConsequences.Text = Accidents_DGV.CurrentRow.Cells[9].Value.ToString();
                ManageAcc_txtCrewAction.Text = Accidents_DGV.CurrentRow.Cells[10].Value.ToString();
                ManageAcc_CheckReported.Checked = (bool)Accidents_DGV.CurrentRow.Cells[14].Value;
                ManageAcc_txtReportedTo.Text = Accidents_DGV.CurrentRow.Cells[12].Value.ToString();
                ManageAcc_txtCoast.Text = Accidents_DGV.CurrentRow.Cells[13].Value.ToString();

                triptabControl.SelectedTab = AccidentManagementTab;
            }
        }

        private void ManageAcc_CheckReported_CheckedChanged(object sender, EventArgs e)
        {
            ManageAcc_lblReqReport.Visible = ManageAcc_txtReportedTo.Enabled = ManageAcc_CheckReported.Checked;
        }

        private void accidents_deletebtn_Click(object sender, EventArgs e)
        {
            if (Accidents_DGV.CurrentRow != null)
            {
                if (MessageBox.Show("هل تريد حذف الحادثة?", "حذف الحادثة", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                {
                    var accidentId = Convert.ToInt32(Accidents_DGV.CurrentRow.Cells[0].Value);
                    accidentService.DeleteAccident(accidentId);

                    if (dbService.Commit())
                    {
                        FillAccidentsDGV();
                        MessageBox.Show("تم الحذف بنجاح", "تم الحذف", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("لم يتم الحذف", "فشل الحذف", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        bool ContainsArabicText(string text)
        {
            foreach (char c in text.ToCharArray())
            {
                if (c >= 0x600 && c <= 0x6ff)
                {
                    return true;
                }
            }
            return false;
        }

        iTextSharp.text.Font titleFont = FontFactory.GetFont(fontname: "c:/windows/fonts/simpbdo.ttf", encoding: BaseFont.IDENTITY_H, size: 20, style: 1);
        iTextSharp.text.Font headerFont = FontFactory.GetFont(fontname: "c:/windows/fonts/arial.ttf", encoding: BaseFont.IDENTITY_H, size: 10, style: 1);
        iTextSharp.text.Font SubReportTitleFont = FontFactory.GetFont(fontname: "c:/windows/fonts/arial.ttf", encoding: BaseFont.IDENTITY_H, size: 12, style: 1);
        iTextSharp.text.Font cellFont = FontFactory.GetFont("c:/windows/fonts/arial.ttf", BaseFont.IDENTITY_H, 8);
        private void Trips_btnExportReport_Click(object sender, EventArgs e)
        {
            ReportSFD.FileName = "تقرير الرحلات";
            if (ReportSFD.ShowDialog() == DialogResult.OK)
            {
                headerFont.Color = BaseColor.WHITE;
                titleFont.SetStyle(4);

                Document document = new Document();
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(ReportSFD.FileName, FileMode.Create));
                writer.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                document.Open();

                PdfPTable titleTable = new PdfPTable(1);
                titleTable.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                titleTable.WidthPercentage = 100;
                titleTable.HorizontalAlignment = Element.ALIGN_CENTER;
                PdfPCell titleCell = new PdfPCell(new Phrase("تقرير الرحلات", titleFont));
                titleCell.HorizontalAlignment = Element.ALIGN_CENTER;
                titleCell.Border = 0;
                titleTable.AddCell(titleCell);
                document.Add(titleTable);

                document.Add(new Phrase(" "));

                PdfPTable table = new PdfPTable(TripsDGV.Columns.Count);
                table.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                table.WidthPercentage = 100;
                table.SetWidths(new int[] { 50, 45, 40, 40, 60, 30, 30, 30, 30, 35, 25 });
                foreach (DataGridViewColumn item in TripsDGV.Columns)
                {
                    if (item.Visible)
                    {
                        PdfPCell headerCell = new PdfPCell(new Phrase(item.HeaderText, headerFont));
                        headerCell.BackgroundColor = BaseColor.GRAY;
                        headerCell.HorizontalAlignment = Element.ALIGN_CENTER;
                        table.AddCell(headerCell);

                        if (item.DisplayIndex == 5)
                        {
                            PdfPCell ProductsHeaderCell = new PdfPCell(new Phrase("المنتجات", headerFont));
                            ProductsHeaderCell.BackgroundColor = BaseColor.GRAY;
                            ProductsHeaderCell.HorizontalAlignment = Element.ALIGN_CENTER;
                            table.AddCell(ProductsHeaderCell);
                        }
                    }
                }

                foreach (DataGridViewRow row in TripsDGV.Rows)
                {
                    var trip = tripService.GetTripById(Convert.ToInt32(row.Cells[0].Value));
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        if (cell.OwningColumn.Visible)
                        {
                            PdfPCell c = new PdfPCell(new Phrase(cell.Value.ToString(), cellFont));
                            c.HorizontalAlignment = Element.ALIGN_CENTER;
                            table.AddCell(c);
                        }
                        if (cell.ColumnIndex == 5)
                        {
                            PdfPTable productsTable = new PdfPTable(2);
                            productsTable.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                            productsTable.SetWidths(new int[] { 65, 65 });

                            PdfPCell productHeaderCell = new PdfPCell(new Phrase("البضاعة", cellFont));
                            productHeaderCell.HorizontalAlignment = Element.ALIGN_CENTER;
                            productsTable.AddCell(productHeaderCell);

                            PdfPCell quantityHeaderCell = new PdfPCell(new Phrase("الكمية", cellFont));
                            quantityHeaderCell.HorizontalAlignment = Element.ALIGN_CENTER;
                            productsTable.AddCell(quantityHeaderCell);

                            foreach (var load in trip.TripsLoads)
                            {
                                PdfPCell productCell = new PdfPCell(new Phrase(load.Product.Name, cellFont));
                                productCell.HorizontalAlignment = Element.ALIGN_CENTER;
                                productsTable.AddCell(productCell);

                                PdfPCell quantityCell = new PdfPCell(new Phrase(load.Quantity.ToString(), cellFont));
                                quantityCell.HorizontalAlignment = Element.ALIGN_CENTER;
                                productsTable.AddCell(quantityCell);
                            }
                            productsTable.HorizontalAlignment = Element.ALIGN_CENTER;
                            table.AddCell(productsTable);
                        }
                    }
                }

                document.Add(table);
                document.Close();
            }
        }

        private void AddShip_btnSaveType_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(AddShip_txtType.Text.Trim()))
                MessageBox.Show("من فضلك أدخل الحقول المطلوبة", "حقول مطلوبة", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
            {
                ShipType type = new ShipType
                {
                    TypeId = AddShip_btnSaveType.Tag == null ? 0 : Convert.ToInt32(AddShip_btnSaveType.Tag),
                    Name = AddShip_txtType.Text.Trim(),
                };

                AddShip_btnSaveType.Tag = null;
                if (shipTypesService.CheckUniqueness(type))
                {
                    if (type.TypeId == 0)
                    {
                        shipTypesService.AddShipType(type);
                    }
                    else
                    {
                        shipTypesService.UpdateShipType(type);
                    }
                    if (dbService.Commit())
                    {
                        FillAddShipTypecmb();

                        MessageBox.Show("تم الحفظ بنجاح", "تم الحفظ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                        MessageBox.Show("لم يتم الحفظ", "فشل الحفظ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    AddShip_txtType.Clear();
                }
                else
                    MessageBox.Show("لم يتم الحفظ .. هذا النوع موجود من قبل", "بيانات مكررة", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddShip_btnEditTypr_Click(object sender, EventArgs e)
        {
            AddShip_btnSaveType.Tag = AddShip_Typecmb.SelectedValue;
            AddShip_txtType.Text = AddShip_Typecmb.Text;
        }

        private void AddShip_btnDeleteType_Click(object sender, EventArgs e)
        {
            if (AddShip_Typecmb.SelectedValue != null)
            {
                if (MessageBox.Show("هل تريد حذف نوع السفينة?", "حذف نوع السفينة", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                {
                    var typeId = Convert.ToInt32(AddShip_Typecmb.SelectedValue);

                    if (shipTypesService.CanDelete(typeId))
                    {
                        shipTypesService.DeleteShipType(typeId);

                        if (dbService.Commit())
                        {
                            FillAddShipTypecmb();
                            MessageBox.Show("تم الحذف بنجاح", "تم الحذف", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("لم يتم الحذف", "فشل الحذف", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                        MessageBox.Show("لم يتم الحذف .. نوع السفينة هذا مستخدم حاليا", "فشل الحذف", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void AddShip_btnCancelType_Click(object sender, EventArgs e)
        {
            AddShip_btnSaveType.Tag = null;
            AddShip_txtType.Clear();
        }

        Document CreateReportPdfFile(string reportName)
        {
            ReportSFD.FileName = reportName;
            if (ReportSFD.ShowDialog() == DialogResult.OK)
            {
                headerFont.Color = BaseColor.WHITE;
                titleFont.SetStyle(4);

                try
                {
                    Document document = new Document();
                    PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(ReportSFD.FileName, FileMode.Create));
                    writer.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                    document.Open();

                    PdfPTable titleTable = new PdfPTable(1);
                    titleTable.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                    titleTable.WidthPercentage = 100;
                    titleTable.HorizontalAlignment = Element.ALIGN_CENTER;
                    PdfPCell titleCell = new PdfPCell(new Phrase(reportName, titleFont));
                    titleCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    titleCell.Border = 0;
                    titleTable.AddCell(titleCell);
                    document.Add(titleTable);

                    document.Add(new Phrase(" "));

                    return document;
                }
                catch (IOException ex)
                {
                    MessageBox.Show("لا يمكن إنشاء الملف .. من فضلك حاول مجددا", "فشل إنشاء الملف", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }
            }
            return null;
        }

        void CenterAllignPdfTableCells(PdfPTable table)
        {
            foreach (var row in table.Rows)
            {
                foreach (var cell in row.GetCells())
                {
                    if (cell != null)
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                }
            }
        }

        void SetTableHeaderCellsStyle(PdfPRow row)
        {
            foreach (var headerCell in row.GetCells())
            {
                if (headerCell != null)
                    headerCell.BackgroundColor = BaseColor.GRAY;
            }
        }

        int CalculateDateDiffernce(DateTime date1, DateTime date2)
        {
            return date1.Subtract(date2).Days;
        }

        PdfPTable GetSubTable(List<Trip> trips, string title, string type, int productId = 0, decimal total = 0)
        {
            PdfPTable table = new PdfPTable(type == "status" ? 7 : 8);
            SubReportTitleFont.Color = BaseColor.BLACK;
            table.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
            table.WidthPercentage = 100;

            if (type == "status")
                table.SetWidths(new int[] { 70, 45, 80, 30, 30, 35, 30 });
            else
                table.SetWidths(new int[] { 60, 40, 70, 30, 30, 30, 30, 30 });

            PdfPCell TitleCell = new PdfPCell(new Phrase(title, SubReportTitleFont));
            TitleCell.Border = 0;
            TitleCell.Colspan = table.NumberOfColumns;
            table.AddCell(TitleCell);

            PdfPCell headerCell1 = new PdfPCell(new Phrase("رقم الرحلة", headerFont));
            table.AddCell(headerCell1);

            PdfPCell headerCell2 = new PdfPCell(new Phrase("اسم السفينة", headerFont));
            table.AddCell(headerCell2);

            PdfPCell headerCell3 = new PdfPCell(new Phrase("IMO", headerFont));
            table.AddCell(headerCell3);

            if (type == "quantity" || type == "visits")
            {
                PdfPCell headerCell4 = new PdfPCell(new Phrase("الميناء", headerFont));
                table.AddCell(headerCell4);
            }

            PdfPCell headerCell5 = new PdfPCell(new Phrase("التاريخ", headerFont));
            table.AddCell(headerCell5);

            PdfPCell headerCell6 = new PdfPCell(new Phrase(type == "quantity" ? "كمية البضاعة" : "البضاعة", headerFont));
            table.AddCell(headerCell6);

            PdfPCell headerCell7 = new PdfPCell(new Phrase("الوكيل الملاحى", headerFont));
            table.AddCell(headerCell7);

            PdfPCell headerCell8 = new PdfPCell(new Phrase("ملاحظات", headerFont));
            table.AddCell(headerCell8);

            SetTableHeaderCellsStyle(table.Rows[1]);

            foreach (var trip in trips)
            {
                PdfPCell c1 = new PdfPCell(new Phrase(trip.TripId.ToString(), cellFont));
                table.AddCell(c1);

                PdfPCell c2 = new PdfPCell(new Phrase(trip.Ship.Name, cellFont));
                table.AddCell(c2);

                PdfPCell c3 = new PdfPCell(new Phrase(trip.Ship.Imo, cellFont));
                table.AddCell(c3);

                PdfPCell c4 = new PdfPCell(new Phrase(trip.Port.Name, cellFont));
                table.AddCell(c4);

                PdfPCell c5 = new PdfPCell(new Phrase(trip.TripsStatus.FirstOrDefault(s => s.Status == trip.Status).Date.ToShortDateString(), cellFont));
                table.AddCell(c5);

                if (type == "status" || type == "visits")
                {
                    PdfPTable productsTable = new PdfPTable(2);
                    productsTable.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                    productsTable.SetWidths(new int[] { 68, 67 });

                    PdfPCell productHeaderCell = new PdfPCell(new Phrase("البضاعة", cellFont));
                    productsTable.AddCell(productHeaderCell);

                    PdfPCell quantityHeaderCell = new PdfPCell(new Phrase("الكمية", cellFont));
                    productsTable.AddCell(quantityHeaderCell);

                    foreach (var load in trip.TripsLoads)
                    {
                        PdfPCell productCell = new PdfPCell(new Phrase(load.Product.Name, cellFont));
                        productsTable.AddCell(productCell);

                        PdfPCell quantityCell = new PdfPCell(new Phrase(load.Quantity.ToString(), cellFont));
                        productsTable.AddCell(quantityCell);
                    }
                    productsTable.HorizontalAlignment = Element.ALIGN_CENTER;
                    CenterAllignPdfTableCells(productsTable);
                    table.AddCell(productsTable);
                }
                else
                {
                    PdfPCell c6 = new PdfPCell(new Phrase(trip.TripsLoads.FirstOrDefault(x => x.ProductId == productId).Quantity.ToString(), cellFont));
                    table.AddCell(c6);
                }

                PdfPCell c7 = new PdfPCell(new Phrase(trip.Agent.Name, cellFont));
                table.AddCell(c7);

                PdfPCell c8 = new PdfPCell(new Phrase(trip.Notes, cellFont));
                table.AddCell(c8);
            }
            if (type == "quantity")
            {
                PdfPCell totalCell = new PdfPCell(new Phrase("إجمالى الكمية = " + total.ToString(), cellFont));
                totalCell.Colspan = table.NumberOfColumns;
                table.AddCell(totalCell);
            }

            if (type == "visits")
            {
                PdfPCell totalCell = new PdfPCell(new Phrase("إجمالى عدد الزيارات = " + trips.Count.ToString(), cellFont));
                totalCell.Colspan = table.NumberOfColumns;
                table.AddCell(totalCell);
            }

            CenterAllignPdfTableCells(table);

            return table;
        }

        private void Reports_TripsReport_btnExtract_Click(object sender, EventArgs e)
        {
            var document = CreateReportPdfFile("تقرير الرحلات");

            if (document != null)
            {
                PdfPTable table = new PdfPTable(TripsDGV.Columns.Count);
                table.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                table.WidthPercentage = 100;
                table.SetWidths(new int[] { 50, 45, 40, 40, 65, 36, 30, 30, 30, 35, 25 });
                foreach (DataGridViewColumn item in TripsDGV.Columns)
                {
                    if (item.Visible)
                    {
                        PdfPCell headerCell = new PdfPCell(new Phrase(item.HeaderText, headerFont));
                        headerCell.BackgroundColor = BaseColor.GRAY;
                        headerCell.HorizontalAlignment = Element.ALIGN_CENTER;
                        table.AddCell(headerCell);

                        if (item.DisplayIndex == 5)
                        {
                            PdfPCell ProductsHeaderCell = new PdfPCell(new Phrase("البضائع", headerFont));
                            ProductsHeaderCell.BackgroundColor = BaseColor.GRAY;
                            ProductsHeaderCell.HorizontalAlignment = Element.ALIGN_CENTER;
                            table.AddCell(ProductsHeaderCell);
                        }
                    }
                }

                var trips = tripService.GetAllTrips();

                if (Convert.ToInt32(Reports_TripsReport_cmbShips.SelectedValue) != -1)
                {
                    trips = trips.Where(t => t.ShipId == Convert.ToInt32(Reports_TripsReport_cmbShips.SelectedValue)).ToList();
                }

                if (Convert.ToInt32(Reports_TripsReport_cmbAgents.SelectedValue) != -1)
                {
                    trips = trips.Where(t => t.AgentId == Convert.ToInt32(Reports_TripsReport_cmbAgents.SelectedValue)).ToList();
                }

                if (Convert.ToInt32(Reports_TripsReport_cmbPorts.SelectedValue) != -1)
                {
                    trips = trips.Where(t => t.PortId == Convert.ToInt32(Reports_TripsReport_cmbPorts.SelectedValue)).ToList();
                }

                if (Convert.ToInt32(Reports_TripsReport_cmbStatus.SelectedValue) != -1)
                {
                    trips = trips.Where(t => t.Status == Convert.ToInt32(Reports_TripsReport_cmbStatus.SelectedValue)).ToList();
                }

                trips = trips.Where(t => (DateTime.Compare(Reports_TripsReport_dtpFrom.Value, t.TripsStatus.FirstOrDefault(s => s.Status == (int)TripStatus.LeftDGebouti).Date) <= 0) &&
                                         (DateTime.Compare(Reports_TripsReport_dtpTo.Value, t.TripsStatus.FirstOrDefault(s => s.Status == (int)TripStatus.LeftDGebouti).Date) >= 0)).ToList();


                foreach (var trip in trips)
                {
                    PdfPCell c1 = new PdfPCell(new Phrase(trip.TripId.ToString(), cellFont));
                    table.AddCell(c1);

                    PdfPCell c2 = new PdfPCell(new Phrase(trip.Ship.Name, cellFont));
                    table.AddCell(c2);

                    PdfPCell c3 = new PdfPCell(new Phrase(trip.Ship.Imo, cellFont));
                    table.AddCell(c3);

                    PdfPCell c4 = new PdfPCell(new Phrase(trip.Ship.ShipType.Name, cellFont));
                    table.AddCell(c4);

                    PdfPCell c5 = new PdfPCell(new Phrase(ArabicValues[((TripStatus)trip.Status).ToString()], cellFont));
                    table.AddCell(c5);

                    PdfPCell c6 = new PdfPCell(new Phrase(trip.TripsStatus.FirstOrDefault(s => s.Status == trip.Status).Date.ToShortDateString(), cellFont));
                    table.AddCell(c6);

                    PdfPTable productsTable = new PdfPTable(2);
                    productsTable.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                    productsTable.SetWidths(new int[] { 68, 67 });

                    PdfPCell productHeaderCell = new PdfPCell(new Phrase("البضاعة", cellFont));
                    productsTable.AddCell(productHeaderCell);

                    PdfPCell quantityHeaderCell = new PdfPCell(new Phrase("الكمية", cellFont));
                    productsTable.AddCell(quantityHeaderCell);

                    foreach (var load in trip.TripsLoads)
                    {
                        PdfPCell productCell = new PdfPCell(new Phrase(load.Product.Name, cellFont));
                        productsTable.AddCell(productCell);

                        PdfPCell quantityCell = new PdfPCell(new Phrase(load.Quantity.ToString(), cellFont));
                        productsTable.AddCell(quantityCell);
                    }
                    productsTable.HorizontalAlignment = Element.ALIGN_CENTER;
                    CenterAllignPdfTableCells(productsTable);
                    table.AddCell(productsTable);

                    PdfPCell c8 = new PdfPCell(new Phrase(trip.Agent != null ? trip.Agent.Name : "", cellFont));
                    table.AddCell(c8);

                    PdfPCell c9 = new PdfPCell(new Phrase(trip.Port != null ? trip.Port.Name : "", cellFont));
                    table.AddCell(c9);

                    PdfPCell c10 = new PdfPCell(new Phrase(trip.Platform != null ? trip.Platform.Name : "", cellFont));
                    table.AddCell(c10);

                    PdfPCell c11 = new PdfPCell(new Phrase(trip.Notes, cellFont));
                    table.AddCell(c11);
                }

                CenterAllignPdfTableCells(table);
                document.Add(table);
                document.Close();
            }
        }

        private void Reports_PeriodsReport_btnExtract_Click(object sender, EventArgs e)
        {
            Document document = CreateReportPdfFile("تقرير مدد الرحلات");

            if (document != null)
            {
                PdfPTable table = new PdfPTable(11);
                table.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                table.WidthPercentage = 100;
                table.SetWidths(new int[] { 40, 40, 40, 40, 40, 40, 40, 40, 40, 40, 40 });

                PdfPCell headerCell1 = new PdfPCell(new Phrase("رقم الرحلة", headerFont));
                table.AddCell(headerCell1);

                PdfPCell headerCell2 = new PdfPCell(new Phrase("اسم السفينة", headerFont));
                table.AddCell(headerCell2);

                PdfPCell headerCell3 = new PdfPCell(new Phrase("IMO", headerFont));
                table.AddCell(headerCell3);

                PdfPCell headerCell4 = new PdfPCell(new Phrase("تاريخ الخروج من جيبوتى", headerFont));
                table.AddCell(headerCell4);

                PdfPCell headerCell5 = new PdfPCell(new Phrase("تاريخ وصول منطقة الإحتجاز", headerFont));
                table.AddCell(headerCell5);

                PdfPCell headerCell6 = new PdfPCell(new Phrase("تاريخ وصول الغاطس", headerFont));
                table.AddCell(headerCell6);

                PdfPCell headerCell7 = new PdfPCell(new Phrase("تاريخ دخول الرصيف", headerFont));
                table.AddCell(headerCell7);

                PdfPCell headerCell8 = new PdfPCell(new Phrase("المدة من جيبوتى حتى الإحتجاز", headerFont));
                table.AddCell(headerCell8);

                PdfPCell headerCell9 = new PdfPCell(new Phrase("المدة من الإحتجاز حتى الغاطس", headerFont));
                table.AddCell(headerCell9);

                PdfPCell headerCell10 = new PdfPCell(new Phrase("المدة من الغاطس حتى دخول الرصيف", headerFont));
                table.AddCell(headerCell10);

                PdfPCell headerCell11 = new PdfPCell(new Phrase("إجمالى مدة الرحلة", headerFont));
                table.AddCell(headerCell11);

                SetTableHeaderCellsStyle(table.Rows[0]);

                var trips = tripService.GetAllTrips().Where(t =>
                                         (DateTime.Compare(Reports_PeriodsReport_dtpFrom.Value, t.TripsStatus.FirstOrDefault(s => s.Status == (int)TripStatus.LeftDGebouti).Date) <= 0) &&
                                         (DateTime.Compare(Reports_PeriodsReport_dtpTo.Value, t.TripsStatus.FirstOrDefault(s => s.Status == (int)TripStatus.LeftDGebouti).Date) >= 0)).ToList();

                foreach (var trip in trips)
                {
                    PdfPCell c1 = new PdfPCell(new Phrase(trip.TripId.ToString(), cellFont));
                    table.AddCell(c1);

                    PdfPCell c2 = new PdfPCell(new Phrase(trip.Ship.Name, cellFont));
                    table.AddCell(c2);

                    PdfPCell c3 = new PdfPCell(new Phrase(trip.Ship.Imo, cellFont));
                    table.AddCell(c3);

                    var _geboti = trip.TripsStatus.FirstOrDefault(s => s.Status == (int)TripStatus.LeftDGebouti).Date;

                    PdfPCell c4 = new PdfPCell(new Phrase(_geboti.ToShortDateString(), cellFont));
                    table.AddCell(c4);

                    var _reservation = trip.TripsStatus.FirstOrDefault(s => s.Status == (int)TripStatus.ReservationArea);
                    PdfPCell c5 = new PdfPCell(new Phrase(_reservation != null ? _reservation.Date.ToShortDateString() : "لم تصل منطقة الإحتجاز بعد", cellFont));
                    table.AddCell(c5);

                    var _ghates = trip.TripsStatus.FirstOrDefault(s => s.Status == (int)TripStatus.AtGhates);
                    PdfPCell c6 = new PdfPCell(new Phrase(_ghates != null ? _ghates.Date.ToShortDateString() : "لم تصل الغاطس بعد", cellFont));
                    table.AddCell(c6);

                    var _platform = trip.TripsStatus.FirstOrDefault(s => s.Status == (int)TripStatus.ArriveAtPlatform);
                    PdfPCell c7 = new PdfPCell(new Phrase(_platform != null ? _platform.Date.ToShortDateString() : "لم تدخل الرصيف بعد", cellFont));
                    table.AddCell(c7);

                    double _gebotiToReservation = 0;
                    if (_reservation != null)
                        _gebotiToReservation = CalculateDateDiffernce(_reservation.Date, _geboti);
                    PdfPCell c8 = new PdfPCell(new Phrase(_gebotiToReservation.ToString(), cellFont));
                    table.AddCell(c8);


                    double _reservationToGhates = 0;
                    if (_ghates != null && _reservation != null)
                        _reservationToGhates = CalculateDateDiffernce(_ghates.Date, _reservation.Date);
                    PdfPCell c9 = new PdfPCell(new Phrase(_reservationToGhates.ToString(), cellFont));
                    table.AddCell(c9);

                    double _ghatesToPlatform = 0;
                    if (_platform != null && _ghates != null)
                        _ghatesToPlatform = CalculateDateDiffernce(_platform.Date, _ghates.Date);
                    PdfPCell c10 = new PdfPCell(new Phrase(_ghatesToPlatform.ToString(), cellFont));
                    table.AddCell(c10);

                    PdfPCell c11 = new PdfPCell(new Phrase((_gebotiToReservation + _reservationToGhates + _ghatesToPlatform).ToString(), cellFont));
                    table.AddCell(c11);
                }
                CenterAllignPdfTableCells(table);
                document.Add(table);
                document.Close();
            }
        }

        private void Reports_ShipsStatus_btnExtract_Click(object sender, EventArgs e)
        {
            Document document = CreateReportPdfFile("تقرير حالات السفن");

            if (document != null)
            {
                List<PdfPTable> tables = new List<PdfPTable>();

                var trips = tripService.GetAllTrips();

                if (Convert.ToInt32(Reports_ShipsStatus_cmbStatus.SelectedValue) != -1)
                {
                    trips = trips.Where(t => t.Status == Convert.ToInt32(Reports_ShipsStatus_cmbStatus.SelectedValue)).ToList();

                    switch (Convert.ToInt32(Reports_ShipsStatus_cmbStatus.SelectedValue))
                    {
                        case (int)TripStatus.LeftDGebouti:
                            tables.Add(GetSubTable(trips, "السفن التى غادرت جيبوتى", "status"));
                            break;

                        case (int)TripStatus.ReservationArea:
                            tables.Add(GetSubTable(trips, "السفن فى منطقة الإحتجاز", "status"));
                            break;

                        case (int)TripStatus.AtGhates:
                            tables.Add(GetSubTable(trips, "السفن فى الغاطس", "status"));
                            break;

                        case (int)TripStatus.EXecptedTOArrive:
                            tables.Add(GetSubTable(trips, "السفن المتوقع وصولها", "status"));
                            break;

                        case (int)TripStatus.ArriveAtPlatform:
                            {
                                trips = trips.Where(t => t.Status == (int)TripStatus.ArriveAtPlatform).ToList();

                                if (Convert.ToInt32(Reports_ShipStaus_cmbPorts.SelectedValue) != -1)
                                {
                                    trips = trips.Where(t => t.PortId == Convert.ToInt32(Reports_ShipStaus_cmbPorts.SelectedValue)).ToList();

                                    if (Convert.ToInt32(Reports_ShipStaus_cmbPlatforms.SelectedValue) != -1)
                                    {  //specific platform
                                        trips = trips.Where(t => t.PlatformId == Convert.ToInt32(Reports_ShipStaus_cmbPlatforms.SelectedValue)).ToList();
                                        if (trips.Count > 0)
                                        {
                                            string title = " السفن الراسية فى " + trips.FirstOrDefault().Port.Name;
                                            title += " على " + trips.FirstOrDefault().Platform.Name;

                                            tables.Add(GetSubTable(trips, title, "status"));
                                        }
                                    }
                                    else
                                    {
                                        //all platforms
                                        if (trips.Count > 0)
                                        {
                                            var platformGroups = trips.GroupBy(p => p.PlatformId);

                                            foreach (var platformGroup in platformGroups)
                                            {
                                                string title = " السفن الراسية فى " + platformGroup.FirstOrDefault().Port.Name;
                                                title += " على " + platformGroup.FirstOrDefault().Platform.Name;
                                                tables.Add(GetSubTable(platformGroup.ToList(), title, "status"));
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    //all ports and all platforms
                                    var portGroups = trips.GroupBy(p => p.PortId);

                                    foreach (var portGroup in portGroups)
                                    {
                                        var platformGroups = portGroup.ToList().GroupBy(p => p.PlatformId);

                                        foreach (var platformGroup in platformGroups)
                                        {
                                            string title = " السفن الراسية فى " + platformGroup.FirstOrDefault().Port.Name;
                                            title += " على " + platformGroup.FirstOrDefault().Platform.Name;

                                            tables.Add(GetSubTable(platformGroup.ToList(), title, "status"));
                                        }
                                    }
                                }

                                break;
                            }

                        case (int)TripStatus.WaitingAtGhatesAfterUnload:
                            tables.Add(GetSubTable(trips, "السفن المنتظرة فى الغاطس بعد تفريغ الحمولة", "status"));
                            break;
                    }
                }
                else
                {
                    //all status reports
                    var tripsStatusGroups = trips.OrderBy(o => o.Status).GroupBy(t => t.Status);

                    foreach (var statusGroup in tripsStatusGroups)
                    {
                        switch (statusGroup.Key)
                        {
                            case (int)TripStatus.LeftDGebouti:
                                tables.Add(GetSubTable(trips.Where(t => t.Status == (int)TripStatus.LeftDGebouti).ToList(), "السفن التى غادرت جيبوتى", "status"));
                                break;

                            case (int)TripStatus.ReservationArea:
                                tables.Add(GetSubTable(trips.Where(t => t.Status == (int)TripStatus.ReservationArea).ToList(), "السفن فى منطقة الإحتجاز", "status"));
                                break;

                            case (int)TripStatus.AtGhates:
                                tables.Add(GetSubTable(trips.Where(t => t.Status == (int)TripStatus.AtGhates).ToList(), "السفن فى الغاطس", "status"));
                                break;

                            case (int)TripStatus.EXecptedTOArrive:
                                tables.Add(GetSubTable(trips.Where(t => t.Status == (int)TripStatus.EXecptedTOArrive).ToList(), "السفن المتوقع وصولها", "status"));
                                break;

                            case (int)TripStatus.ArriveAtPlatform:
                                {
                                    var portGroups = statusGroup.GroupBy(p => p.PortId);

                                    foreach (var portGroup in portGroups)
                                    {
                                        var platformGroups = portGroup.ToList().GroupBy(p => p.PlatformId);

                                        foreach (var platformGroup in platformGroups)
                                        {
                                            string title = " السفن الراسية فى " + platformGroup.FirstOrDefault().Port.Name;
                                            title += " على " + platformGroup.FirstOrDefault().Platform.Name;

                                            tables.Add(GetSubTable(platformGroup.ToList(), title, "status"));
                                        }
                                    }
                                    break;
                                }

                            case (int)TripStatus.WaitingAtGhatesAfterUnload:
                                tables.Add(GetSubTable(trips.Where(t => t.Status == (int)TripStatus.WaitingAtGhatesAfterUnload).ToList(), "السفن المنتظرة فى الغاطس بعد تفريغ الحمولة", "status"));
                                break;
                        }
                    }
                }

                foreach (var table in tables)
                {
                    document.Add(table);
                    document.Add(new Phrase(" "));
                }
                document.Close();
            }
        }

        private void Reports_ShipsStatus_cmbStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            Reports_ShipStaus_cmbPorts.Enabled = (int)Reports_ShipsStatus_cmbStatus.SelectedValue == (int)TripStatus.ArriveAtPlatform;
        }

        private void Reports_ShipStaus_cmbPorts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((int)Reports_ShipStaus_cmbPorts.SelectedValue != -1)
            {
                FillReportsCmbPlatforms();
                Reports_ShipStaus_cmbPlatforms.Enabled = true;
            }
        }

        private void addingTripTab_Enter(object sender, EventArgs e)
        {
            if (AddTrip_btnSaveTrip.Tag == null)
            {
                AddTrip_CmbStatus.SelectedValue = (int)TripStatus.LeftDGebouti;
                EditTrip_btnChangeStatus.Visible = false;
            }
            else
            {
                EditTrip_btnChangeStatus.Visible = true;
            }
        }

        private void AddShip_Typecmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            AddShip_btnEditTypr.Enabled = AddShip_btnDeleteType.Enabled = (int)AddShip_Typecmb.SelectedValue != -1;
        }

        private void txt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 'ى')
                e.KeyChar = 'ي';
            if (e.KeyChar == 'أ' || e.KeyChar == 'إ')
                e.KeyChar = 'ا';
            if (e.KeyChar == 'ة')
                e.KeyChar = 'ه';
        }

        private void AddTrip_CmbStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            AddTrip_CmbPlatforms.Enabled = AddTrip_CmbStatus.SelectedValue != null && (int)AddTrip_CmbStatus.SelectedValue == (int)TripStatus.ArriveAtPlatform &&
                                           AddTrip_CmbPorts.SelectedValue != null && (int)AddTrip_CmbPorts.SelectedValue != -1;

            if (AddTrip_btnSaveTrip.Tag != null && AddTrip_CmbStatus.SelectedValue != null)
            {
                switch ((int)AddTrip_CmbStatus.SelectedValue)
                {
                    case (int)TripStatus.LeftDGebouti:
                        EditTrip_btnChangeStatus.Text = "إلي منطقة الإحتجاز";
                        EditTrip_btnChangeStatus.Tag = (int)TripStatus.ReservationArea;
                        break;

                    case (int)TripStatus.ReservationArea:
                        EditTrip_btnChangeStatus.Text = "متوقع الوصول";
                        EditTrip_btnChangeStatus.Tag = (int)TripStatus.EXecptedTOArrive;
                        break;

                    case (int)TripStatus.EXecptedTOArrive:
                        EditTrip_btnChangeStatus.Text = "إلي الغاطس";
                        EditTrip_btnChangeStatus.Tag = (int)TripStatus.AtGhates;
                        break;

                    case (int)TripStatus.AtGhates:
                        EditTrip_btnChangeStatus.Text = "إلى الأرصفة";
                        EditTrip_btnChangeStatus.Tag = (int)TripStatus.ArriveAtPlatform;
                        break;

                    case (int)TripStatus.ArriveAtPlatform:
                        EditTrip_btnChangeStatus.Text = "إلى الغاطس بعد التفريغ";
                        EditTrip_btnChangeStatus.Tag = (int)TripStatus.WaitingAtGhatesAfterUnload;
                        break;

                    case (int)TripStatus.WaitingAtGhatesAfterUnload:
                        EditTrip_btnChangeStatus.Visible = false;
                        break;
                }
            }
        }

        DateTime previousStatusDate = DateTime.MaxValue;
        private void EditTrip_btnChangeStatus_Click(object sender, EventArgs e)
        {

            if (EditTrip_btnChangeStatus.Tag != null)
            {
                previousStatusDate = tripService.GetTripById(Convert.ToInt32(AddTrip_btnSaveTrip.Tag)).TripsStatus.FirstOrDefault(s => s.Status == (int)AddTrip_CmbStatus.SelectedValue).Date;
                AddTrip_CmbStatus.SelectedValue = EditTrip_btnChangeStatus.Tag;
            }
            MessageBox.Show("من فضلك اختر التاريخ", "تاريخ الحالة", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Reports_quantities_btnExtract_Click(object sender, EventArgs e)
        {
            Document document = CreateReportPdfFile("تقرير كميات البضائع ");

            if (document != null)
            {
                List<PdfPTable> tables = new List<PdfPTable>();

                var trips = tripService.GetAllTrips().
                    Where(t => (t.TripsStatus.FirstOrDefault(
                        x => x.Status == (int)TripStatus.AtGhates &&
                        DateTime.Compare(Reports_quantitiesReport_dtpFrom.Value.Date, x.Date) <= 0 &&
                        DateTime.Compare(Reports_quantitiesReport_dtpTo.Value.Date, x.Date) >= 0) != null) ||
                        (t.TripsStatus.FirstOrDefault(
                            x => x.Status == (int)TripStatus.ArriveAtPlatform &&
                            DateTime.Compare(Reports_quantitiesReport_dtpFrom.Value.Date, x.Date) <= 0 &&
                            DateTime.Compare(Reports_quantitiesReport_dtpTo.Value.Date, x.Date) >= 0) != null));

                if (Reports_quantityReport_cmbPorts.SelectedValue != null && (int)Reports_quantityReport_cmbPorts.SelectedValue != -1)
                    trips = trips.Where(t => t.PortId == Convert.ToInt32(Reports_quantityReport_cmbPorts.SelectedValue));

                if (Reports_quantityReport_cmbProducts.SelectedValue != null && (int)Reports_quantityReport_cmbProducts.SelectedValue != -1)
                {
                    int productId = Convert.ToInt32(Reports_quantityReport_cmbProducts.SelectedValue);
                    trips = trips.Where(t => t.TripsLoads.FirstOrDefault(l => l.ProductId == productId) != null);
                    var quantity = trips.Sum(t => t.TripsLoads.FirstOrDefault(l => l.ProductId == productId).Quantity);

                    if (quantity > 0)
                    {
                        var productName = trips.FirstOrDefault().TripsLoads.FirstOrDefault(x => x.ProductId == productId).Product.Name;
                        tables.Add(GetSubTable(trips.ToList(), "كمية " + productName + " التى وصلت خلال المدة من " + Reports_quantitiesReport_dtpFrom.Value.Date.ToShortDateString() + " إلى " + Reports_quantitiesReport_dtpTo.Value.Date.ToShortDateString(), "quantity", productId, quantity));
                    }
                }
                else
                {
                    foreach (var item in productService.GetAllProducts())
                    {
                        var productTrips = trips.Where(t => t.TripsLoads.FirstOrDefault(l => l.ProductId == item.ProductId) != null);
                        var quantity = productTrips.Sum(t => t.TripsLoads.FirstOrDefault(l => l.ProductId == item.ProductId).Quantity);
                        if (quantity > 0)
                            tables.Add(GetSubTable(productTrips.ToList(), "كمية " + item.Name + " التى وصلت خلال المدة من " + Reports_quantitiesReport_dtpFrom.Value.Date.ToShortDateString() + " إلى " + Reports_quantitiesReport_dtpTo.Value.Date.ToShortDateString(), "quantity", item.ProductId, quantity));
                    }
                }

                foreach (var table in tables)
                {
                    document.Add(table);
                    document.Add(new Phrase(" "));
                }
                document.Close();
            }
        }

        private void Reports_Visits_btnExtract_Click(object sender, EventArgs e)
        {
            Document document = CreateReportPdfFile("تقرير زيارات السفن للموانئ");

            if (document != null)
            {
                List<PdfPTable> tables = new List<PdfPTable>();

                var trips = tripService.GetAllTrips();

                if (Reports_Visits_cmbShips.SelectedValue != null && (int)Reports_Visits_cmbShips.SelectedValue != -1)
                    trips = trips.Where(t => t.ShipId == Convert.ToInt32(Reports_Visits_cmbShips.SelectedValue)).ToList();

                if (Reports_Visits_cmbPorts.SelectedValue != null && (int)Reports_Visits_cmbPorts.SelectedValue != -1)
                    trips = trips.Where(t => t.PortId == Convert.ToInt32(Reports_Visits_cmbPorts.SelectedValue)).ToList();

                trips = trips.Where(t => t.TripsStatus.FirstOrDefault(
                    s => (s.Status == (int)TripStatus.ArriveAtPlatform && DateTime.Compare(Reports_Visits_dtpFrom.Value.Date, s.Date) <= 0 && DateTime.Compare(Reports_Visits_dtpTo.Value.Date, s.Date) >= 0)
                    || (s.Status == (int)TripStatus.AtGhates && DateTime.Compare(Reports_Visits_dtpFrom.Value.Date, s.Date) <= 0 && DateTime.Compare(Reports_Visits_dtpTo.Value.Date, s.Date) >= 0)) != null).ToList();

                var shipGroups = trips.GroupBy(s => s.ShipId);

                foreach (var shipGroup in shipGroups)
                {
                    tables.Add(GetSubTable(shipGroup.ToList(), "تقرير زيارات السفينة " + shipGroup.First().Ship.Name, "visits"));
                }

                foreach (var table in tables)
                {
                    document.Add(table);
                    document.Add(new Phrase(" "));
                }
                document.Close();
            }
        }
    }
}
