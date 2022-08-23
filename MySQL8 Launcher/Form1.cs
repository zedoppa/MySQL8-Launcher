using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.ServiceProcess;
using System.Windows.Forms;

namespace MySQL8_Launcher
{
    public partial class Form1 : Form
    {
        private IContainer components;
        private string pathBin = Path.GetFullPath("bin");
        private string pathData = Path.GetFullPath("data\\mysql");
        private Button btnInstall;
        private Button btnRemove;
        private RichTextBox txtLogs;
        private Label customMsg;
        private Label appTitle;
        private Button btnClose;
        private Button btnMinimize;

        private bool mouseDown;
        private Point lastLocation;
        private bool mySQLRunning = false;

        public Form1() => this.InitializeComponent();

        private void RunCmd(string command)
        {
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.Arguments = "/c " + command;
                process.Start();
                process.StandardInput.AutoFlush = true;
                string end = process.StandardOutput.ReadToEnd();
                this.txtLogs.AppendText("\n"+ DateTime.Now.ToString() + "\r\n" + end);
                process.WaitForExit();
                process.Close();
            }
            catch (Exception ex)
            {
                this.txtLogs.AppendText(ex.Message);
            }
        }

        private void btnInstall_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(this.pathData))
                this.RunCmd(this.pathBin + "\\mysqld --install mysql8 && net start mysql8");
            else
                this.RunCmd(this.pathBin + "\\mysqld --initialize-insecure && " + this.pathBin + "\\mysqld --install mysql8 && net start mysql8");
            foreach (ServiceController service in ServiceController.GetServices())
            {
                if (service.ServiceName == "mysql8")
                {
                    if (service.Status == ServiceControllerStatus.Running)
                    {
                        this.txtLogs.AppendText("MySQL has been successfully started, username: root, password is *****, please set your own password!\r\n", Color.Lime);
                        mySQLRunning = true;    // set mySQLRunning true if MySQL is started
                        return;
                    }
                    this.txtLogs.AppendText("The MySQL8 service failed to start, please try to run this program as an administrator!\r\n\n", Color.Red);
                    return;
                }
            }
            this.txtLogs.AppendText("MySQL8 service installation failed, please try to run this program as an administrator!\r\n", Color.Yellow);
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            foreach (ServiceController service in ServiceController.GetServices())
            {
                if (service.ServiceName == "mysql8")
                {
                    this.RunCmd("net stop mysql8 && " + this.pathBin + "\\mysqld --remove mysql8");
                    this.txtLogs.AppendText("\nMySQL has been successfully stopped!\r\n", Color.Lime);
                    mySQLRunning = false;    // set mySQLRunning true if MySQL is started
                    return;
                }
            }
            this.txtLogs.AppendText("\nThe MySQL8 service does not exist, please do not uninstall it repeatedly. \r\n", Color.Yellow);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (mySQLRunning)
                this.txtLogs.AppendText("\nPlease stop MySQL8 service first. \r\n", Color.Yellow);
            else
                Application.Exit();
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        #region Buttons Hover Effects
        private void btnInstall_MouseEnter(object sender, EventArgs e)
        {
            btnInstall.BackColor = Color.ForestGreen;
            btnInstall.ForeColor = Color.Black;
        }

        private void btnInstall_MouseLeave(object sender, EventArgs e)
        {
            btnInstall.BackColor = Color.Black;
            btnInstall.ForeColor = Color.ForestGreen;
        }

        private void btnRemove_MouseEnter(object sender, EventArgs e)
        {
            btnRemove.BackColor = Color.OrangeRed;
            btnRemove.ForeColor = Color.Black;
        }

        private void btnRemove_MouseLeave(object sender, EventArgs e)
        {
            btnRemove.BackColor = Color.Black;
            btnRemove.ForeColor = Color.OrangeRed;
        }

        private void btnClose_MouseEnter(object sender, EventArgs e)
        {
            btnClose.BackColor = Color.Red;
            btnClose.ForeColor = Color.Black;
        }

        private void btnClose_MouseLeave(object sender, EventArgs e)
        {
            btnClose.BackColor = Color.Black;
            btnClose.ForeColor = Color.Red;
        }

        private void btnMinimize_MouseEnter(object sender, EventArgs e)
        {
            btnMinimize.BackColor = Color.White;
            btnMinimize.ForeColor = Color.Black;
        }

        private void btnMinimize_MouseLeave(object sender, EventArgs e)
        {
            btnMinimize.BackColor = Color.Black;
            btnMinimize.ForeColor = Color.White;
        }
        #endregion

        #region Window Draggable
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseDown = true;
                lastLocation = e.Location;
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseDown = false;
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                this.Location = new Point((this.Location.X - lastLocation.X) + e.X, (this.Location.Y - lastLocation.Y) + e.Y);

                this.Update();
            }
        }
        #endregion

        #region Auto Scroll RichTextBox
        private void txtLogs_TextChanged(object sender, EventArgs e)
        {
            txtLogs.SelectionStart = txtLogs.Text.Length;
            txtLogs.ScrollToCaret();
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
                this.components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.btnInstall = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.txtLogs = new System.Windows.Forms.RichTextBox();
            this.customMsg = new System.Windows.Forms.Label();
            this.appTitle = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnMinimize = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnInstall
            // 
            this.btnInstall.BackColor = System.Drawing.Color.Black;
            this.btnInstall.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnInstall.ForeColor = System.Drawing.Color.ForestGreen;
            this.btnInstall.Location = new System.Drawing.Point(12, 49);
            this.btnInstall.Name = "btnInstall";
            this.btnInstall.Size = new System.Drawing.Size(94, 31);
            this.btnInstall.TabIndex = 0;
            this.btnInstall.Text = "Start MySQL8";
            this.btnInstall.UseVisualStyleBackColor = false;
            this.btnInstall.Click += new System.EventHandler(this.btnInstall_Click);
            this.btnInstall.MouseEnter += new System.EventHandler(this.btnInstall_MouseEnter);
            this.btnInstall.MouseLeave += new System.EventHandler(this.btnInstall_MouseLeave);
            // 
            // btnRemove
            // 
            this.btnRemove.BackColor = System.Drawing.Color.Black;
            this.btnRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemove.ForeColor = System.Drawing.Color.OrangeRed;
            this.btnRemove.Location = new System.Drawing.Point(229, 49);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(94, 31);
            this.btnRemove.TabIndex = 1;
            this.btnRemove.Text = "Stop MySQL8";
            this.btnRemove.UseVisualStyleBackColor = false;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            this.btnRemove.MouseEnter += new System.EventHandler(this.btnRemove_MouseEnter);
            this.btnRemove.MouseLeave += new System.EventHandler(this.btnRemove_MouseLeave);
            // 
            // txtLogs
            // 
            this.txtLogs.BackColor = System.Drawing.Color.Black;
            this.txtLogs.ForeColor = System.Drawing.Color.White;
            this.txtLogs.Location = new System.Drawing.Point(12, 97);
            this.txtLogs.Name = "txtLogs";
            this.txtLogs.ReadOnly = true;
            this.txtLogs.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.txtLogs.Size = new System.Drawing.Size(311, 153);
            this.txtLogs.TabIndex = 2;
            this.txtLogs.Text = "";
            this.txtLogs.TextChanged += new System.EventHandler(this.txtLogs_TextChanged);
            // 
            // customMsg
            // 
            this.customMsg.AutoSize = true;
            this.customMsg.ForeColor = System.Drawing.Color.DodgerBlue;
            this.customMsg.Location = new System.Drawing.Point(9, 263);
            this.customMsg.Name = "customMsg";
            this.customMsg.Size = new System.Drawing.Size(187, 13);
            this.customMsg.TabIndex = 3;
            this.customMsg.Text = "Fixed && Customized by GMZekrom";
            this.customMsg.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // appTitle
            // 
            this.appTitle.AutoSize = true;
            this.appTitle.ForeColor = System.Drawing.Color.Azure;
            this.appTitle.Location = new System.Drawing.Point(2, 9);
            this.appTitle.Name = "appTitle";
            this.appTitle.Size = new System.Drawing.Size(181, 13);
            this.appTitle.TabIndex = 4;
            this.appTitle.Text = "MySQL8 Startup Tool - by 碎天";
            this.appTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.Black;
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.ForeColor = System.Drawing.Color.Red;
            this.btnClose.Location = new System.Drawing.Point(290, 0);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(45, 20);
            this.btnClose.TabIndex = 5;
            this.btnClose.Text = "X";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            this.btnClose.MouseEnter += new System.EventHandler(this.btnClose_MouseEnter);
            this.btnClose.MouseLeave += new System.EventHandler(this.btnClose_MouseLeave);
            // 
            // btnMinimize
            // 
            this.btnMinimize.BackColor = System.Drawing.Color.Black;
            this.btnMinimize.FlatAppearance.BorderSize = 0;
            this.btnMinimize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMinimize.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMinimize.ForeColor = System.Drawing.Color.White;
            this.btnMinimize.Location = new System.Drawing.Point(245, 0);
            this.btnMinimize.Name = "btnMinimize";
            this.btnMinimize.Size = new System.Drawing.Size(45, 20);
            this.btnMinimize.TabIndex = 6;
            this.btnMinimize.Text = "-";
            this.btnMinimize.UseVisualStyleBackColor = false;
            this.btnMinimize.Click += new System.EventHandler(this.btnMinimize_Click);
            this.btnMinimize.MouseEnter += new System.EventHandler(this.btnMinimize_MouseEnter);
            this.btnMinimize.MouseLeave += new System.EventHandler(this.btnMinimize_MouseLeave);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlText;
            this.ClientSize = new System.Drawing.Size(335, 287);
            this.Controls.Add(this.btnMinimize);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.appTitle);
            this.Controls.Add(this.customMsg);
            this.Controls.Add(this.txtLogs);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnInstall);
            this.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseUp);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
