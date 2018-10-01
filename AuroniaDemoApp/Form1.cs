using Neosmartpen.Net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Drawing.Imaging;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Neosmartpen.Net.Support;
using Neosmartpen.Net.Bluetooth;
using Neosmartpen.Net.Protocol.v1;
using Neosmartpen.Net.Protocol.v2;

namespace AuroniaDemoApp
{
    public partial class Form1 : Form, PenCommV1Callbacks, PenCommV2Callbacks
    {
        public PressureFilter mFilter;

        public static int[] UsingNote = new int[] { 301, 302, 303 };

        //public static string DefaultPassword = "0000";

        private Bitmap mBitmap;

        private Stroke mStroke;

        private int mWidth, mHeight;

        private object mDrawLock = new object();

        //private PasswordInputForm mPwdForm;

        //private ProgressForm mPrgForm;

        // 블루투스 통신 제어를 위한 어댑터
        private BluetoothAdapter mBtAdt;

        // F110 과의 통신 담당
        private PenCommV1 mPenCommV1;

        // F50 과의 통신 담당
        private PenCommV2 mPenCommV2;

        private double ht=35.16;
        private double wd = 61.01;

        private Color mColor ;

        public delegate void RequestDele();

        public Form1()
        {
            InitializeComponent();
            mBtAdt = new BluetoothAdapter();

            mPenCommV1 = new PenCommV1(this);
            mPenCommV2 = new PenCommV2(this);

            mWidth = pictureBox1.Width;
            mHeight = pictureBox1.Height;

            mBitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);

            mColor = Color.Blue;

            //Set the background to white.
            Graphics g = Graphics.FromImage(mBitmap);
            g.Clear(Color.White);
            g.Dispose();

            //add items to both the combobox
            loadcbNb();
            loadcbCol();

            //Set background color for labels
            label1_blue.BackColor = Color.Blue;
            label1_blue.ForeColor = Color.Blue;
            label1_blue.BorderStyle = BorderStyle.FixedSingle;

            label2_green.BackColor = Color.Green;
            label2_green.ForeColor = Color.Green;

            label3_black.BackColor = Color.Black;
            label3_black.ForeColor = Color.Black;

            label4_purple.BackColor = Color.Purple;
            label4_purple.ForeColor = Color.Purple;

            label5_brown.BackColor = Color.Brown;
            label5_brown.ForeColor = Color.Brown;

            label6_red.BackColor = Color.Red;
            label6_red.ForeColor = Color.Red;
        }

        #region ComboBox Nb and Color
        //Notebooks
        private void loadcbNb()
        {
            cbNb.Items.Add("Pocket");
            cbNb.Items.Add("Planner");
            cbNb.Items.Add("Ring");
            cbNb.Items.Add("Professional");
            cbNb.Items.Add("Idea Pad");
            cbNb.Items.Add("Plain");
            cbNb.Items.Add("College");
            cbNb.Items.Add("Handy");
            cbNb.Items.Add("NCode A4");
            cbNb.Items.Add("Portfolio");
        }
        //Handle Index Change
        private void cbNb_SelectedIndexChanged(object sender, EventArgs e)
        {
            var item = cbNb.SelectedItem;
            switch(item)
            {
                case "Pocket":
                    ht = 35.16;
                    wd = 61.01;
                    break;

                case "Planner":
                    //MessageBox.Show("2");
                    ht = 88.98;
                    wd = 63.56;
                    break;

                case "Ring":
                    //MessageBox.Show("3");
                    ht = 88.98;
                    wd = 63.56;
                    break;

                case "Professional":
                    //MessageBox.Show("4");
                    ht = 86.86;
                    wd = 59.32;
                    break;

                case "Idea Pad":
                    //MessageBox.Show("5");
                    ht = 125.84;
                    wd = 88.98;
                    break;

                case "Plain":
                    //MessageBox.Show("6");
                    ht = 105.93;
                    wd = 74.57;
                    break;

                case "College":
                    //MessageBox.Show("6");
                    ht = 118.64;
                    wd = 91.52;
                    break;

                case "Handy":
                    //MessageBox.Show("7");
                    ht = 76.27;
                    wd = 48.72;
                    break;

                case "NCode A4":
                    //MessageBox.Show("8");
                    ht = 125.84;
                    wd = 88.98;
                    break;

                case "Portfolio":
                    //MessageBox.Show("9");
                    ht = 93.22;
                    wd = 69.91;
                    break;

                default:
                    ht = 61.01;
                    wd = 35.16;
                    break;


            }
        }

        //Color
        private void loadcbCol()
        {
            cbCol.Items.Add("Blue");
            cbCol.Items.Add("Black");
            cbCol.Items.Add("Brown");
            cbCol.Items.Add("Green");
            cbCol.Items.Add("Purple");
            cbCol.Items.Add("Red");
        }
        //Handle color chanage
        private void cbCol_SelectedIndexChanged(object sender, EventArgs e)
        {
            var item = cbCol.SelectedItem;
            switch(item)
            {
                case "Blue":
                    mColor = Color.Blue;
                    break;

                case "Black":
                    mColor = Color.Black;
                    break;

                case "Brown":
                    mColor = Color.Brown;
                    break;

                case "Green":
                    mColor = Color.Green;
                    break;

                case "Purple":
                    mColor = Color.Purple;
                    break;

                case "Red":
                    mColor = Color.Red;
                    break;

                default:
                    mColor = Color.Blue;
                    break;
            }
        }
        #endregion

        #region Label Colors
        private void label1_blue_Click(object sender, EventArgs e)
        {
            label1_blue.BorderStyle = BorderStyle.FixedSingle;
            label2_green.BorderStyle = BorderStyle.None;
            label3_black.BorderStyle = BorderStyle.None;
            label4_purple.BorderStyle = BorderStyle.None;
            label5_brown.BorderStyle = BorderStyle.None;
            label6_red.BorderStyle = BorderStyle.None;
            mColor = Color.Blue;
            //MessageBox.Show("Blue");
        }

        private void label1_blue_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, label1_blue.DisplayRectangle, Color.BlanchedAlmond, ButtonBorderStyle.Solid);
        }

        private void label2_green_Click(object sender, EventArgs e)
        {
            label1_blue.BorderStyle = BorderStyle.None;
            label2_green.BorderStyle = BorderStyle.FixedSingle;
            label3_black.BorderStyle = BorderStyle.None;
            label4_purple.BorderStyle = BorderStyle.None;
            label5_brown.BorderStyle = BorderStyle.None;
            label6_red.BorderStyle = BorderStyle.None;
            mColor = Color.Green;
            //MessageBox.Show("Green");
        }

        private void label2_green_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, label2_green.DisplayRectangle, Color.BlanchedAlmond, ButtonBorderStyle.Solid);
        }

        private void label3_black_Click(object sender, EventArgs e)
        {
            label1_blue.BorderStyle = BorderStyle.None;
            label2_green.BorderStyle = BorderStyle.None;
            label3_black.BorderStyle = BorderStyle.FixedSingle;
            label4_purple.BorderStyle = BorderStyle.None;
            label5_brown.BorderStyle = BorderStyle.None;
            label6_red.BorderStyle = BorderStyle.None;
            mColor = Color.Black;
            //MessageBox.Show("Black");
        }

        private void label3_black_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, label3_black.DisplayRectangle, Color.BlanchedAlmond, ButtonBorderStyle.Solid);
        }

        private void label4_purple_Click(object sender, EventArgs e)
        {
            label1_blue.BorderStyle = BorderStyle.None;
            label2_green.BorderStyle = BorderStyle.None;
            label3_black.BorderStyle = BorderStyle.None;
            label4_purple.BorderStyle = BorderStyle.FixedSingle;
            label5_brown.BorderStyle = BorderStyle.None;
            label6_red.BorderStyle = BorderStyle.None;
            mColor = Color.Purple;
            //MessageBox.Show("Purple");
        }

        private void label4_purple_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, label4_purple.DisplayRectangle, Color.BlanchedAlmond, ButtonBorderStyle.Solid);
        }

        private void label5_brown_Click(object sender, EventArgs e)
        {
            label1_blue.BorderStyle = BorderStyle.None;
            label2_green.BorderStyle = BorderStyle.None;
            label3_black.BorderStyle = BorderStyle.None;
            label4_purple.BorderStyle = BorderStyle.None;
            label5_brown.BorderStyle = BorderStyle.FixedSingle;
            label6_red.BorderStyle = BorderStyle.None;
            mColor = Color.Brown;
            //MessageBox.Show("Brown");
        }

        private void label5_brown_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, label5_brown.DisplayRectangle, Color.BlanchedAlmond, ButtonBorderStyle.Solid);
        }

        private void label6_red_Click(object sender, EventArgs e)
        {
            label1_blue.BorderStyle = BorderStyle.None;
            label2_green.BorderStyle = BorderStyle.None;
            label3_black.BorderStyle = BorderStyle.None;
            label4_purple.BorderStyle = BorderStyle.None;
            label5_brown.BorderStyle = BorderStyle.None;
            label6_red.BorderStyle = BorderStyle.FixedSingle;
            mColor = Color.Red;
            //MessageBox.Show("Red");
        }

        private void label6_red_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, label6_red.DisplayRectangle, Color.BlanchedAlmond, ButtonBorderStyle.Solid);
        }
        #endregion


        //InitImage()
        private void InitImage()
        {
            Graphics g = Graphics.FromImage(mBitmap);
            g.Clear(Color.White);
            g.Dispose();
            pictureBox1.Image = AuroniaDemoApp.Properties.Resources.background1;
            pictureBox1.Invalidate();
        }
        

        #region Button Clicks
        //Search
        private void btnSearch_Click(object sender, EventArgs e)
        {
            btnSearch.Enabled = false;

            Thread thread = new Thread(unused =>
            {
                PenDevice[] devices = mBtAdt.FindAllDevices();

                this.BeginInvoke(new MethodInvoker(delegate ()
                {
                    lbDevices.Items.Clear();

                    if (devices == null || devices.Length <= 0)
                    {
                        MessageBox.Show("device is not exists");
                    }
                    else
                    {
                        lbDevices.Items.AddRange(devices);
                    }

                    btnSearch.Enabled = true;
                }));
            });

            thread.IsBackground = true;
            thread.Start();
        }

        //Connect
        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (txtMacAddress.Text == "")
            {
                MessageBox.Show("Select Mac Address of Pen!!");
                return;
            }

            btnConnect.Enabled = false;

            Thread thread = new Thread(unused =>
            {
                // 블루투스 인터페이스로 펜과 연결하여 생성한 소켓을 Device class에 따라 바인딩
                bool result = mBtAdt.Connect(txtMacAddress.Text, delegate (uint deviceClass)
                {
                    // device class가 f110 일 경우에 바인딩
                    if (deviceClass == mPenCommV1.DeviceClass)
                    {
                        mBtAdt.Bind(mPenCommV1);
                    }
                    // device class가 f50 일 경우에 바인딩
                    else if (deviceClass == mPenCommV2.DeviceClass)
                    {
                        mBtAdt.Bind(mPenCommV2);
                    }
                });

                if (!result)
                {
                    MessageBox.Show("Fail to connect");

                    this.BeginInvoke(new MethodInvoker(delegate ()
                    {
                        btnConnect.Enabled = true;
                    }));
                }
            });

            thread.IsBackground = true;
            thread.Start();
        }

        //Select Device
        private void lbDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListBox lbEvent = sender as ListBox;
            PenDevice dev = lbEvent.SelectedItem as PenDevice;
            if (dev != null)
                txtMacAddress.Text = dev.Address;
        }

        //Disconnect
        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            btnDisconnect.Enabled = false;

            if (!mBtAdt.Disconnect())
            {
                btnDisconnect.Enabled = true;
            }
        }

        //Form Close 
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (mBtAdt != null)
            {
                if (mBtAdt.Connected)
                {
                    mBtAdt.Disconnect();
                }
            }
        }

        //Download
        private void btnDownload_Click(object sender, EventArgs e)
        {
            if (lbOfflineData.SelectedItem == null)
            {
                return;
            }

            OfflineDataInfo data = lbOfflineData.SelectedItem as OfflineDataInfo;

            Request(
                delegate { mPenCommV1.ReqOfflineData(data); },
                delegate {
                    mPenCommV2.ReqOfflineData(data.Section, data.Owner, data.Note, false, data.Pages);
                });
        }

        //Delete
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lbOfflineData.SelectedItem == null)
            {
                return;
            }

            OfflineDataInfo data = lbOfflineData.SelectedItem as OfflineDataInfo;

            Request(
                delegate { mPenCommV1.ReqRemoveOfflineData(data.Section, data.Owner); },
                delegate {
                    mPenCommV2.ReqRemoveOfflineData(data.Section, data.Owner, new int[] { data.Note });
                });
        }

        //Save
        private void btnSave_Click(object sender, EventArgs e)
        {
            bool isNullOrEmpty = pictureBox1 == null || pictureBox1.Image == null;
            if(!isNullOrEmpty)
            {
                Bitmap b = new Bitmap(pictureBox1.Image);
                Image i = (Image)b;
                SaveFileDialog f = new SaveFileDialog();
                f.Filter = "JPeg Image|*.jpg|Bitmap Image|*.bmp|Png Image|*.png";
                f.Title = "Save an Image File";
                ImageFormat format = ImageFormat.Png;
                f.ShowDialog();
                if (f.FileName != "")
                {
                    switch (f.FilterIndex)
                    {
                        case 1:
                            format = ImageFormat.Jpeg;
                            break;
                        case 2:
                            format = ImageFormat.Bmp;
                            break;
                        default:
                            format = ImageFormat.Png;
                            break;

                    }
                    //pictureBox1.Image.Save(f.FileName , format);
                    i.Save(f.FileName, format);
                    MessageBox.Show("Image Successfully Stored");
                }
            }
            else
            {
                MessageBox.Show("Please Draw or Select File to Save!");
            }

        }

        //Clear
        private void btnClear_Click(object sender, EventArgs e)
        {
            this.BeginInvoke(new MethodInvoker(delegate ()
            {
                InitImage();
            }));
        }

        #endregion



        //Request()
        private void Request(RequestDele onRequestToV1, RequestDele onRequestToV2, RequestDele onFailure = null, RequestDele onAfter = null)
        {
            if (!mBtAdt.Connected)
            {
                MessageBox.Show("Device is not connected.");

                if (onFailure != null)
                {
                    onFailure();
                }
            }
            else if (mBtAdt.DeviceClass == mPenCommV1.DeviceClass)
            {
                onRequestToV1();

                if (onAfter != null)
                {
                    onAfter();
                }
            }
            else if (mBtAdt.DeviceClass == mPenCommV2.DeviceClass)
            {
                onRequestToV2();

                if (onAfter != null)
                {
                    onAfter();
                }
            }
        }



        //Process Dot
        private void ProcessDot(Dot dot)
        {
            // 필터링 된 필압
            dot.Force = mFilter.Filter(dot.Force);

            // TODO: Drawing sample code
            if (dot.DotType == DotTypes.PEN_DOWN)
            {
                mStroke = new Stroke(dot.Section, dot.Owner, dot.Note, dot.Page);
                mStroke.Add(dot);
            }
            else if (dot.DotType == DotTypes.PEN_MOVE)
            {
                mStroke.Add(dot);
            }
            else if (dot.DotType == DotTypes.PEN_UP)
            {
                mStroke.Add(dot);

                DrawStroke(mStroke);

                mFilter.Reset();
            }
        }

        //Draw Stroke
        private void DrawStroke(Stroke stroke)
        {
            this.Invoke(new MethodInvoker(delegate ()
            {
                lock (mDrawLock)
                {
                    //603 Ring Note Height  5.52  5.41 	63.46 	88.88 
                    int dx = (int)((5.52 * mWidth) / ht);
                    int dy = (int)((5.41 * mHeight) / wd);

                    Renderer.draw(mBitmap, stroke, (float)(mWidth / (float) ht), (float)(mHeight / (float) wd), -dx, -dy, 1, Color.FromArgb(200, mColor));

                    pictureBox1.Image = mBitmap;
                }
            }));
        }



        #region PenCommV1Callbacks

        void PenCommV1Callbacks.onConnected(IPenComm sender, int maxForce, string swVersion)
        {
            mFilter = new PressureFilter(maxForce);

            this.BeginInvoke(new MethodInvoker(delegate ()
            {
                btnConnect.Enabled = false;
                btnDisconnect.Enabled = true;
                //tbPenInfo.Text = String.Format("Firmware Version : {0}", swVersion);

                //ToggleOption(true);
            }));
        }

        void PenCommV1Callbacks.onDisconnected(IPenComm sender)
        {
            this.BeginInvoke(new MethodInvoker(delegate ()
            {
                lbOfflineData.Items.Clear();

                btnConnect.Enabled = true;
                btnDisconnect.Enabled = false;
                //tbPenInfo.Text = "";

                //ToggleOption(false);
            }));

            CloseProgress();
        }

        void PenCommV1Callbacks.onPenPasswordRequest(IPenComm sender, int retryCount, int resetCount)
        {
            //this.BeginInvoke(new MethodInvoker(delegate ()
            //{
            //    mPwdForm.ShowDialog();
            //}));
        }

        void PenCommV1Callbacks.onPenAuthenticated(IPenComm sender)
        {
            mPenCommV1.ReqAddUsingNote();
            mPenCommV1.ReqOfflineDataList();
            mPenCommV1.ReqPenStatus();
        }

        void PenCommV1Callbacks.onReceiveDot(IPenComm sender, Dot dot)
        {
            ProcessDot(dot);
        }

        void PenCommV1Callbacks.onUpDown(IPenComm sender, bool isUp)
        {
        }

        void PenCommV1Callbacks.onReceivedPenStatus(IPenComm sender, int timeoffset, long timetick, int forcemax, int battery, int usedmem, int pencolor, bool autopower, bool accel, bool hovermode, bool beep, short autoshutdownTime, short penSensitivity, string modelName)
        {
            //this.BeginInvoke(new MethodInvoker(delegate ()
            //{
            //    nmPowerOffTime.Value = autoshutdownTime;
            //    tbFsrStep.Value = penSensitivity;
            //    cbBeep.Checked = beep;
            //    cbHover.Checked = hovermode;

            //    cbOfflineData.Checked = false;
            //    cbOfflineData.Enabled = false;
            //    cbPenCapPower.Checked = false;
            //    cbPenCapPower.Enabled = false;

            //    cbPenTipPowerOn.Checked = autopower;

            //    prgPower.Maximum = 100;
            //    prgPower.Value = battery;

            //    prgStorage.Maximum = 100;
            //    prgStorage.Value = usedmem;
            //}));
        }

        void PenCommV1Callbacks.onPenPasswordSetUpResponse(IPenComm sender, bool result)
        {
            //if (!result)
            //{
            //    MessageBox.Show("Can not change password.");
            //}
            //else
            //{
            //    tbOldPassword.Text = "";
            //    tbNewPassword.Text = "";
            //}
        }

        void PenCommV1Callbacks.onPenSensitivitySetUpResponse(IPenComm sender, bool isSuccess)
        {
            mPenCommV1.ReqPenStatus();
        }

        void PenCommV1Callbacks.onPenAutoShutdownTimeSetUpResponse(IPenComm sender, bool isSuccess)
        {
            mPenCommV1.ReqPenStatus();
        }

        void PenCommV1Callbacks.onPenBeepSetUpResponse(IPenComm sender, bool isSuccess)
        {
            mPenCommV1.ReqPenStatus();
        }

        void PenCommV1Callbacks.onPenAutoPowerOnSetUpResponse(IPenComm sender, bool isSuccess)
        {
            mPenCommV1.ReqPenStatus();
        }

        void PenCommV1Callbacks.onPenColorSetUpResponse(IPenComm sender, bool isSuccess)
        {
            mPenCommV1.ReqPenStatus();
        }

        void PenCommV1Callbacks.onPenHoverSetUpResponse(IPenComm sender, bool result)
        {
            mPenCommV1.ReqPenStatus();
        }

        void PenCommV1Callbacks.onReceivedFirmwareUpdateResult(IPenComm sender, bool isSuccess)
        {
            CloseProgress();
        }

        private object mProgressLock = new object();

        private void DisplayProgress(string title, int total, int amountDone)
        {
            //this.BeginInvoke(new MethodInvoker(delegate ()
            //{
            //    lock (mProgressLock)
            //    {
            //        if (mPrgForm == null)
            //        {
            //            mPrgForm = new ProgressForm();
            //        }

            //        if (mPrgForm != null)
            //        {
            //            mPrgForm.SetStatus(title, total, amountDone);
            //        }

            //        if (!mPrgForm.Visible)
            //        {
            //            mPrgForm.ShowDialog();
            //        }
            //    }
            //}));
        }

        private void CloseProgress()
        {
            //this.BeginInvoke(new MethodInvoker(delegate ()
            //{
            //    lock (mProgressLock)
            //    {
            //        if (mPrgForm != null && mPrgForm.Visible)
            //        {
            //            mPrgForm.Close();
            //            mPrgForm.Dispose();
            //            mPrgForm = null;
            //        }
            //    }
            //}));
        }

        public const string ProgressTitleOffline = "Download Offline Data";

        public const string ProgressTitleFirmware = "Firmware Update";

        void PenCommV1Callbacks.onReceivedFirmwareUpdateStatus(IPenComm sender, int total, int progress)
        {
            DisplayProgress(ProgressTitleFirmware, total, progress);
        }

        void PenCommV1Callbacks.onOfflineDataList(IPenComm sender, OfflineDataInfo[] notes)
        {
            this.BeginInvoke(new MethodInvoker(delegate ()
            {
                lbOfflineData.Items.Clear();

                foreach (OfflineDataInfo item in notes)
                {
                    lbOfflineData.Items.Add(item);
                }
            }));
        }

        void PenCommV1Callbacks.onStartOfflineDownload(IPenComm sender)
        {
            DisplayProgress(ProgressTitleOffline, 100, 0);
        }

        void PenCommV1Callbacks.onUpdateOfflineDownload(IPenComm sender, int total, int progress)
        {
            DisplayProgress(ProgressTitleOffline, total, progress);
        }

        void PenCommV1Callbacks.onFinishedOfflineDownload(IPenComm sender, bool status)
        {
            CloseProgress();
            mPenCommV1.ReqOfflineDataList();
        }

        void PenCommV1Callbacks.onReceiveOfflineStrokes(IPenComm sender, Stroke[] strokes)
        {
            foreach (Stroke stroke in strokes)
            {
                DrawStroke(stroke);
            }
        }

        #endregion


        #region PenCommV2Callbacks

        void PenCommV2Callbacks.onConnected(IPenComm sender, string macAddress, string deviceName, string fwVersion, string protocolVersion, string subName, int maxForce)
        {
            mFilter = new PressureFilter(maxForce);

            //this.BeginInvoke(new MethodInvoker(delegate ()
            //{
            //    btnConnect.Enabled = false;
            //    btnDisconnect.Enabled = true;

            //    tbPenInfo.Text = String.Format("Mac : {0}\r\n\r\nName : {1}\r\n\r\nSubName : {2}\r\n\r\nFirmware Version : {3}\r\n\r\nProtocol Version : {4}", macAddress, deviceName, subName, fwVersion, protocolVersion);

            //    ToggleOption(true);
            //}));
        }

        void PenCommV2Callbacks.onPenAuthenticated(IPenComm sender)
        {
            mPenCommV2.ReqAddUsingNote();
            mPenCommV2.ReqOfflineDataList();
            mPenCommV2.ReqPenStatus();
        }

        void PenCommV2Callbacks.onDisconnected(IPenComm sender)
        {
            this.BeginInvoke(new MethodInvoker(delegate ()
            {
                lbOfflineData.Items.Clear();

                btnConnect.Enabled = true;
                btnDisconnect.Enabled = false;
                //tbPenInfo.Text = "";

                //ToggleOption(false);
            }));

            CloseProgress();
        }

        void PenCommV2Callbacks.onReceiveDot(IPenComm sender, Dot dot, ImageProcessingInfo info)
        {
            ProcessDot(dot);
        }

        void PenCommV2Callbacks.onReceiveOfflineDataList(IPenComm sender, params OfflineDataInfo[] offlineNotes)
        {
            this.BeginInvoke(new MethodInvoker(delegate ()
            {
                lbOfflineData.Items.Clear();

                foreach (OfflineDataInfo item in offlineNotes)
                {
                    lbOfflineData.Items.Add(item);
                }
            }));
        }

        void PenCommV2Callbacks.onStartOfflineDownload(IPenComm sender)
        {
            DisplayProgress(ProgressTitleOffline, 100, 0);
        }

        void PenCommV2Callbacks.onReceiveOfflineStrokes(IPenComm sender, int total, int progress, Stroke[] strokes)
        {
            foreach (Stroke stroke in strokes)
            {
                DrawStroke(stroke);
            }

            DisplayProgress(ProgressTitleOffline, total, progress);

            Array.Clear(strokes, 0, strokes.Length);
        }

        void PenCommV2Callbacks.onFinishedOfflineDownload(IPenComm sender, bool isSuccess)
        {
            CloseProgress();
            mPenCommV2.ReqOfflineDataList();
        }

        void PenCommV2Callbacks.onRemovedOfflineData(IPenComm sender, bool result)
        {
            mPenCommV2.ReqOfflineDataList();
        }

        void PenCommV2Callbacks.onReceivePenStatus(IPenComm sender, bool locked, int passwdMaxReTryCount, int passwdRetryCount, long timestamp, short autoShutdownTime, int maxForce, int battery, int usedmem, bool useOfflineData, bool autoPowerOn, bool penCapPower, bool hoverMode, bool beep, short penSensitivity, PenCommV2.UsbMode usbmode, bool downsampling, string btLocalName, PenCommV2.DataTransmissionType dataTransmissionType)
        {
            //this.BeginInvoke(new MethodInvoker(delegate ()
            //{
            //    nmPowerOffTime.Value = autoShutdownTime;
            //    tbFsrStep.Value = penSensitivity;
            //    cbBeep.Checked = beep;
            //    cbHover.Checked = hoverMode;
            //    cbOfflineData.Enabled = true;
            //    cbOfflineData.Checked = useOfflineData;
            //    cbPenCapPower.Enabled = true;
            //    cbPenCapPower.Checked = penCapPower;
            //    cbPenTipPowerOn.Checked = autoPowerOn;

            //    prgPower.Maximum = 100;
            //    prgPower.Value = battery > 100 ? 100 : battery;

            //    prgStorage.Maximum = 100;
            //    prgStorage.Value = usedmem;
            //}));
        }

        void PenCommV2Callbacks.onPenPasswordRequest(IPenComm sender, int retryCount, int resetCount)
        {
            //this.BeginInvoke(new MethodInvoker(delegate ()
            //{
            //    mPwdForm.ShowDialog();
            //}));
        }

        void PenCommV2Callbacks.onPenPasswordSetUpResponse(IPenComm sender, bool result)
        {
            //if (!result)
            //{
            //    MessageBox.Show("Can not change password.");
            //}
            //else
            //{
            //    this.BeginInvoke(new MethodInvoker(delegate ()
            //    {
            //        tbOldPassword.Text = "";
            //        tbNewPassword.Text = "";
            //    }));
            //}
        }

        void PenCommV2Callbacks.onPenOfflineDataSetUpResponse(IPenComm sender, bool result)
        {
            mPenCommV2.ReqPenStatus();
        }

        void PenCommV2Callbacks.onPenTimestampSetUpResponse(IPenComm sender, bool result)
        {
            mPenCommV2.ReqPenStatus();
        }

        void PenCommV2Callbacks.onPenSensitivitySetUpResponse(IPenComm sender, bool result)
        {
            mPenCommV2.ReqPenStatus();
        }

        void PenCommV2Callbacks.onPenAutoShutdownTimeSetUpResponse(IPenComm sender, bool result)
        {
            mPenCommV2.ReqPenStatus();
        }

        void PenCommV2Callbacks.onPenAutoPowerOnSetUpResponse(IPenComm sender, bool result)
        {
            mPenCommV2.ReqPenStatus();
        }

        void PenCommV2Callbacks.onPenCapPowerOnOffSetupResponse(IPenComm sender, bool result)
        {
            mPenCommV2.ReqPenStatus();
        }

        void PenCommV2Callbacks.onPenBeepSetUpResponse(IPenComm sender, bool result)
        {
            mPenCommV2.ReqPenStatus();
        }

        void PenCommV2Callbacks.onPenHoverSetUpResponse(IPenComm sender, bool result)
        {
            mPenCommV2.ReqPenStatus();
        }

        void PenCommV2Callbacks.onPenColorSetUpResponse(IPenComm sender, bool result)
        {
            mPenCommV2.ReqPenStatus();
        }

        void PenCommV2Callbacks.onReceiveFirmwareUpdateStatus(IPenComm sender, int total, int progress)
        {
            DisplayProgress(ProgressTitleFirmware, total, progress);
        }

        void PenCommV2Callbacks.onReceiveFirmwareUpdateResult(IPenComm sender, bool result)
        {
            CloseProgress();
        }

        void PenCommV2Callbacks.onReceiveBatteryAlarm(IPenComm sender, int battery)
        {
            //this.BeginInvoke(new MethodInvoker(delegate ()
            //{
            //    prgPower.Maximum = 100;
            //    prgPower.Value = battery;
            //}));
        }

        void PenCommV2Callbacks.onPenUsbModeSetUpResponse(IPenComm sender, bool result)
        {
            mPenCommV2.ReqPenStatus();
        }

        void PenCommV2Callbacks.onPenDownSamplingSetUpResponse(IPenComm sender, bool result)
        {
            mPenCommV2.ReqPenStatus();
        }

        void PenCommV2Callbacks.onPenBtLocalNameSetUpResponse(IPenComm sender, bool result)
        {
            mPenCommV2.ReqPenStatus();
        }

        void PenCommV2Callbacks.onPenFscSensitivitySetUpResponse(IPenComm sender, bool result)
        {
            mPenCommV2.ReqPenStatus();
        }

        void PenCommV2Callbacks.onPenDataTransmissionTypeSetUpResponse(IPenComm sender, bool result)
        {
            mPenCommV2.ReqPenStatus();
        }

        #endregion






    }
}
