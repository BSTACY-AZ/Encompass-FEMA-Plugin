using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web;
using System.Runtime.InteropServices;
using EllieMae.Encompass.Automation;
using TMLEnSFNetworking;
using EllieMae.Encompass.BusinessObjects.Loans;

namespace TMLFemaPlugin
{
    public partial class MainForm : Form
    {
        private Loan _loan;
        private DataTable _items = new DataTable();
        private List<SavedDisaster> _savedDisasters = new List<SavedDisaster>();
        private bool _hasRetried = false;
        private readonly string _DISASTERLISTFIELD = "CX.FEMA.DISASTERS";

        public MainForm(Loan loan)
        {
            this.FormClosing += new FormClosingEventHandler(SaveDisastersToLoan);

            _loan = loan;
            InitializeComponent();
            InitializeDisasterComboBox();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            this.lblResult.Text = "";
            LoadSavedDisasters();
        }

        ~MainForm()
        {
            this.FormClosing -= new FormClosingEventHandler(SaveDisastersToLoan);
        }

        //  Basic functions

        private void SaveDisastersToLoan(object sender, FormClosingEventArgs e)
        {
            string saveText = "";

            foreach(SavedDisaster sd in _savedDisasters)
            {
                saveText += sd.TextToSave() + "\n";
            }

            _loan.Fields[_DISASTERLISTFIELD].Value = saveText;
            _loan.Fields[Main.FEMAHASRUNEFIELDNAME].Value = "Y";
            //_loan.Commit();
        }

        private void LoadSavedDisasters()
        {
            string fieldText = _loan.Fields[_DISASTERLISTFIELD].UnformattedValue;
            if (string.IsNullOrEmpty(fieldText))
                return;

            string[] fieldLines = fieldText.Split('\n');

            foreach(string line in fieldLines)
            {
                SavedDisaster sd = null;

                if (SavedDisaster.TryParse(line, out sd))
                {
                    _savedDisasters.Add(sd);
                }
                else
                {
                    Macro.Alert("ERROR: Could not parse line - " + line);
                }
            }
        }

        private async void SetIgnoreDisaster(string disasterSFId)
        {
            if (string.IsNullOrEmpty(NetworkingController.SFSessionToken))
                await NetworkingController.GetSFSessionAsync();

            var clientHandler = new HttpClientHandler
            {
                UseCookies = false,
            };
            var client = new HttpClient(clientHandler);
            var request = new HttpRequestMessage
            {
                Method = new HttpMethod("PATCH"),
                RequestUri = new Uri(NetworkingController.Settings.BaseURL + "/data/v52.0/sobjects/FEMA_Disaster_Declaration_Summary__c/" + disasterSFId),
                Headers =
                {
                    { "cookie", "BrowserId=tH4JD-TJEeunru1dL9tSew; CookieConsentPolicy=0%3A1" },
                    { "Authorization", "Bearer " + NetworkingController.SFSessionToken },
                },
                Content = new StringContent("{\n\t\"Ignore__c\":true\n}")
                {
                    Headers =
                    {
                        ContentType = new MediaTypeHeaderValue("application/json")
                    }
                }
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                Macro.Alert("Ignore Set " + body);
            }
        }

        private async void ResetIgnoreDisaster(string disasterSFId)
        {
            if (string.IsNullOrEmpty(NetworkingController.SFSessionToken))
                await NetworkingController.GetSFSessionAsync();

            var clientHandler = new HttpClientHandler
            {
                UseCookies = false,
            };
            var client = new HttpClient(clientHandler);
            var request = new HttpRequestMessage
            {
                Method = new HttpMethod("PATCH"),
                RequestUri = new Uri(NetworkingController.Settings.BaseURL + "/data/v52.0/sobjects/FEMA_Disaster_Declaration_Summary__c/" + disasterSFId),
                Headers =
                {
                    { "cookie", "BrowserId=tH4JD-TJEeunru1dL9tSew; CookieConsentPolicy=0%3A1" },
                    { "Authorization", "Bearer " + NetworkingController.SFSessionToken },
                },
                Content = new StringContent("{\n\t\"Ignore__c\":false\n}")
                {
                    Headers =
                    {
                        ContentType = new MediaTypeHeaderValue("application/json")
                    }
                }
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                Macro.Alert("Reset Ignore Set " + body);
            }
        }

        private FIP GetFIP(Loan loan)
        {
            string countyFip = loan.Fields["1396"].UnformattedValue;
            string stateFip = loan.Fields["1395"].UnformattedValue;

            if (string.IsNullOrEmpty(countyFip))
            {
                Macro.Alert("ERROR: County FIP could not be determined - please check fields 1395 & 1396!");
                return null;
            }

            return new FIP(countyFip, stateFip);
        }

        //  UI functions

        private void InitializeDisasterComboBox()
        {
            _items.Columns.AddRange(new DataColumn[] { new DataColumn("Disaster Title"), new DataColumn("Incident Start Date"), new DataColumn("Incident End Date"), new DataColumn("Disaster String"), new DataColumn("Id") });

            cbox1.DrawMode = DrawMode.OwnerDrawVariable;
            cbox1.DrawItem += new DrawItemEventHandler(comboBox_DrawItem);
            cbox1.MeasureItem += new MeasureItemEventHandler(comboBox_MeasureItem);
            cbox1.SelectedIndexChanged += new EventHandler(comboBox_SelectedIndexChanged);
            
            cbox1.ValueMember = "Id";
            cbox1.DisplayMember = "Disaster Title";
            cbox1.DataSource = new BindingSource(_items, null);
            cbox1.SelectedIndex = -1;
        }

        private void LoadDisasterComboBox(List<Disaster> disastersFound)
        {
            _items.Clear();
            foreach (Disaster disaster in disastersFound)
            {
                _items.Rows.Add(new object[] { disaster.Title, disaster.StartDate, disaster.EndDate, disaster.DisString, disaster.SFAppID });
            }
            cbox1.Refresh();
            cbox1.SelectedIndex = 0;
        }

        //  UI events

        private void comboBox_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            ComboxBoxEx cbox = (ComboxBoxEx)sender;
            DataRowView item = (DataRowView)cbox.Items[e.Index];
            string txt = item["Disaster Title"].ToString() + "\r\nStarted: " + item["Incident Start Date"].ToString() + "\r\nEnded: " + item["Incident End Date"].ToString() + "\r\nDisaster String: " + item["Disaster String"].ToString();

            int height = Convert.ToInt32(e.Graphics.MeasureString(txt, cbox.Font).Height);

            e.ItemHeight = height + 4;
            e.ItemWidth = cbox.DropDownWidth;

            cbox.ItemHeights.Add(e.ItemHeight);
        }

        private void comboBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            ComboxBoxEx cbox = (ComboxBoxEx)sender;
            DataRowView item = (DataRowView)cbox.Items[e.Index];
            string txt = item["Disaster Title"].ToString() + "\r\nStarted: " + item["Incident Start Date"].ToString() + "\r\nEnded: " + item["Incident End Date"].ToString() + "\r\nDisaster String: " + item["Disaster String"].ToString();

            e.DrawBackground();
            e.Graphics.DrawString(txt, cbox.Font, System.Drawing.Brushes.Black, new RectangleF(e.Bounds.X + 2, e.Bounds.Y + 2, e.Bounds.Width, e.Bounds.Height));
            e.Graphics.DrawLine(new Pen(Color.LightGray), e.Bounds.X, e.Bounds.Top + e.Bounds.Height - 1, e.Bounds.Width, e.Bounds.Top + e.Bounds.Height - 1);
            e.DrawFocusRectangle();
        }

        private void comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboxBoxEx cbox = (ComboxBoxEx)sender;
            if (cbox.SelectedItem == null) 
                return;

            DataRowView item = (DataRowView)cbox.SelectedItem;
        }

        private async void fetchDisasters_Click(object sender, EventArgs e)
        {
            lblResult.Text = "";

            if (string.IsNullOrEmpty(NetworkingController.SFSessionToken))
                await NetworkingController.GetSFSessionAsync();

            FIP fip = GetFIP(_loan);
            
            if (fip == null)
                return;

            string soql = "SELECT%20Declaration_Date__c%2CDeclared_Close_Out_Date__c%2CId%2CIncident_Begin_Date__c%2CIncident_End_Date__c%2CIncident_Title__c%2CFEMA_Disaster_String__c%20FROM%20FEMA_Disaster_Declaration_Summary__c%20WHERE%20FIPS_County_Code__c%20%3D%20'" + fip.CountyFIP + "'%20AND%20FIPS_State_Code__c%20%3D%20'" + fip.StateFIP + "'";
            
            //filter in only non SF ignored disasters
            soql += "%20AND%20Ignore__c%20%3D%20false";
            
            //filter in only IHA true disasters
            soql += "%20AND%20Individual_and_Household_Assistance__c%20%3D%20true";
            
            //filter by date, excluding null close dates if disaster started longer than a year ago
            string datefilter = " AND Incident_Begin_Date__c > " + DateTime.Now.AddDays(-365).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'") + " AND (Incident_End_Date__c = null OR Incident_End_Date__c > " + DateTime.Now.AddDays(-365).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'") + ")";
            string datefilter_encoded = HttpUtility.UrlEncode(datefilter);

            var clientHandler = new HttpClientHandler
            {
                UseCookies = false,
            };
            var client = new HttpClient(clientHandler);
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(NetworkingController.Settings.BaseURL + "/data/v53.0/query/?q=" + soql + datefilter_encoded),
                Headers =
                {
                    { "cookie", "BrowserId=tH4JD-TJEeunru1dL9tSew; CookieConsentPolicy=0%3A1" },
                    { "Authorization", "Bearer " + NetworkingController.SFSessionToken },
                },
            };
            using (var response = await client.SendAsync(request))
            {
                var body = await response.Content.ReadAsStringAsync();
                int statusCode = (int)response.StatusCode;

                //  Token issue
                if (statusCode >= 400 && statusCode <= 499)
                {
                    NetworkingController.SFSessionToken = "";

                    if (!_hasRetried)
                    {
                        _hasRetried = true;
                        fetchDisasters_Click(sender, e);
                    }
                    else
                    {
                        _hasRetried = false;
                        Macro.Alert($"ERROR {statusCode}: Unauthorized exception on retried request!");
                        return;
                    }
                }
                //  Connection issue
                else if (statusCode >= 500 && statusCode <= 599)
                {
                    if (!_hasRetried)
                    {
                        _hasRetried = true;
                        fetchDisasters_Click(sender, e);
                    }
                    else
                    {
                        _hasRetried = false;
                        Macro.Alert($"ERROR {statusCode}: Internal error on retried request!");
                        return;
                    }
                }
                //  Success
                else if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var jsonbody = JObject.Parse(body);
                    int size = Convert.ToInt32(jsonbody["totalSize"]);

                    if (size > 0)
                    {
                        lblResult.Text = "Disasters Found (" + size.ToString() + ")";
                        var recs = JArray.Parse(jsonbody["records"].ToString());

                        List<Disaster> disastersFound = new List<Disaster>();
                        foreach (var r in recs)
                        {
                            disastersFound.Add(new Disaster(r["Incident_Title__c"].ToString(), r["Incident_Begin_Date__c"].ToString(), r["Incident_End_Date__c"].ToString(), r["FEMA_Disaster_String__c"].ToString(), r["Id"].ToString()));
                        }

                        LoadDisasterComboBox(disastersFound);
                    }
                    else
                    {
                        lblResult.Text = "No Disasters Found";
                    }

                    _hasRetried = false;
                }
                else
                {
                    _hasRetried = false;
                    Macro.Alert($"ERROR {statusCode}: Unhandled exception on retried request!");
                    return;
                }
            }

            //  Enable save disaster button
            btnSaveDisaster.Enabled = true;
        }

        private void btnSaveDisaster_Click(object sender, EventArgs e)
        {
            if (_items.Rows.Count == 0)
            {
                Macro.Alert("There is no data to add!");
                return;
            }

            //  Mark selected disaster to be saved
            var obj = _items.Rows[cbox1.SelectedIndex];
            string disId = obj.ItemArray[4] as string;

            if (_savedDisasters.Exists(dis => dis.SfId.CompareTo(disId) == 0))
            {
                Macro.Alert("Cannot add duplicate disaster with id: " + disId);
                return;
            }

            SavedDisaster sd = new SavedDisaster();
            sd.SfId = disId;
            sd.Date = obj.ItemArray[2] as string;
            sd.FemaString = obj.ItemArray[3] as string;
            _savedDisasters.Add(sd);

            //  Update disaster data for UI
            Macro.Alert($"{(string)obj.ItemArray[0]} has been saved to the loan!");
            _items.Rows.RemoveAt(cbox1.SelectedIndex);
            cbox1.Refresh();
            cbox1.SelectedIndex = _items.Rows.Count > 0 ? 0 : -1;
        }

        private void cbox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }
    }

    public class FIP
    {
        public string StateFIP;
        public string CountyFIP;

        public FIP(string countyFip, string stateFip)
        {
            this.CountyFIP = countyFip;
            this.StateFIP = stateFip;
        }
    }

    public class Disaster
    {
        public string Title { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string SFAppID { get; set; }
        public string DisString { get; set; }

        public Disaster(string _title, string _startDate, string _endDate, string _disString, string _sfAppId)
        {
            Title = _title;
            StartDate = _startDate;
            EndDate = _endDate;
            DisString = _disString;
            SFAppID = _sfAppId;
        }
    }

    public class SavedDisaster
    {
        public string FemaString { get; set; }
        public string SfId { get; set; }
        public string Date { get; set; }

        public SavedDisaster() { }
        public SavedDisaster(string femaString, string id, string date)
        {
            this.FemaString = femaString;
            this.SfId = id;
            this.Date = date;
        }

        public static bool TryParse(string line, out SavedDisaster sd)
        {
            sd = new SavedDisaster();

            if (string.IsNullOrEmpty(line))
            {
                sd = null;
                return false;
            }

            line = line.Replace("\r", "").Replace("\n", "");
            string[] properties = line.Split(';');

            if(properties.Length == 3)
            {
                sd.FemaString = properties[0];
                sd.SfId = properties[1];
                sd.Date = properties[2];
                return true;
            }
            else
            {
                sd = null;
                return false;
            }
        }

        public string TextToSave()
        {
            return FemaString + ';' + SfId + ';' + Date;
        }

        public override string ToString()
        {
            return $"SavedDisaster [ Fema String: {FemaString}, Id: {SfId}, Date: {Date} ]";
        }
    }

    public partial class ComboxBoxEx : ComboBox
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;        // x position of upper-left corner 
            public int Top;         // y position of upper-left corner 
            public int Right;       // x position of lower-right corner 
            public int Bottom;      // y position of lower-right corner 
        }

        public const int SWP_NOZORDER = 0x0004;
        public const int SWP_NOACTIVATE = 0x0010;
        public const int SWP_FRAMECHANGED = 0x0020;
        public const int SWP_NOOWNERZORDER = 0x0200;

        public const int WM_CTLCOLORLISTBOX = 0x0134;

        private int _hwndDropDown = 0;

        internal List<int> ItemHeights = new List<int>();

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_CTLCOLORLISTBOX)
            {
                if (_hwndDropDown == 0)
                {
                    _hwndDropDown = m.LParam.ToInt32();

                    RECT r;
                    GetWindowRect((IntPtr)_hwndDropDown, out r);

                    int newHeight = 0;
                    int n = (Items.Count > MaxDropDownItems) ? MaxDropDownItems : Items.Count;
                    for (int i = 0; i < n; i++)
                    {
                        newHeight += ItemHeights[i];
                    }
                    newHeight += 5; //to stop scrollbars showing

                    SetWindowPos((IntPtr)_hwndDropDown, IntPtr.Zero,
                        r.Left,
                                 r.Top,
                                 DropDownWidth,
                                 newHeight,
                                 SWP_FRAMECHANGED |
                                     SWP_NOACTIVATE |
                                     SWP_NOZORDER |
                                     SWP_NOOWNERZORDER);
                }
            }

            base.WndProc(ref m);
        }

        protected override void OnDropDownClosed(EventArgs e)
        {
            _hwndDropDown = 0;
            base.OnDropDownClosed(e);
        }
    }
}
