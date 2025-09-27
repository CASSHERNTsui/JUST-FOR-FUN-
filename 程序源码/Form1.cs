using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace 工师校园地貌
{
    [System.Runtime.InteropServices.ComVisible(true)]
    public partial class WelcomeForm : DevComponents.DotNetBar.Office2007Form
    {
        frmweb web;
        Map1 m1;
        Map2 m2;
        public WelcomeForm()
        {
            InitializeComponent();
        }

        private void 校园地址_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(System.Windows.Forms.Application.StartupPath + "\\网页\\学校简介-吉林工程技术师范学院.htm");
        }

        private void 校园新闻_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://news.jlenu.edu.cn/");
        }

        private void 学校历史_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(System.Windows.Forms.Application.StartupPath + "\\网页\\历史沿革-吉林工程技术师范学院.htm");
        }

        private void 专业介绍_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://zs.jlenu.edu.cn/info/1030/1592.htm");
        }

        private void 退出_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void vlcControl1_VlcLibDirectoryNeeded(object sender, Vlc.DotNet.Forms.VlcLibDirectoryNeededEventArgs e)
        {
            var currentAssembly = Assembly.GetEntryAssembly();
            var currentDirectory = new FileInfo(currentAssembly.Location).DirectoryName;
            // Default installation path of VideoLAN.LibVLC.Windows
            e.VlcLibDirectory = new DirectoryInfo(Path.Combine(currentDirectory, "libvlc", IntPtr.Size == 4 ? "win-x86" : "win-x64"));

            if (!e.VlcLibDirectory.Exists)
            {
                var folderBrowserDialog = new FolderBrowserDialog();
                folderBrowserDialog.Description = "Select Vlc libraries folder.";
                folderBrowserDialog.RootFolder = Environment.SpecialFolder.Desktop;
                folderBrowserDialog.ShowNewFolderButton = true;
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    e.VlcLibDirectory = new DirectoryInfo(folderBrowserDialog.SelectedPath);
                }
            }

        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "mp4 | *.mp4| avi|*.avi";
                if (ofd.ShowDialog(this) == DialogResult.OK)
                {
                    vlcControl1.Play(new System.IO.FileInfo(ofd.FileName));
                }
            }
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            vlcControl1.Play();
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            vlcControl1.Pause();
        }

        private void 服务指南_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://ehall.jlenu.edu.cn/new/index.html");
        }

        private void 国际交流_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://gjjlc.jlenu.edu.cn/index.htm");
        }

        private void 教务系统_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://jwcsys.jlenu.edu.cn/");
        }

        private void 学生系统_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://ehall.jlenu.edu.cn/new/index.html");
        }

        private void VPN_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://ids.vpn.jlenu.edu.cn/authserver/login?service=http%3A%2F%2Fvpn.jlenu.edu.cn%2Fusers%2Fauth%2Fcas%2Fcallback%3Furl");
        }

        private void 校园地图_Click(object sender, EventArgs e)
        {
            CampusGISFrma f = new CampusGISFrma();
            f.biaoshi = "校园地图";
            f.Show();
            this.Hide();
            
        }

        private void 三维地图_Click(object sender, EventArgs e)
        {
            m1 = new Map1();
            m1.Show();
            m1.Text = "3D浏览";
        }

        private void 路线规划_Click(object sender, EventArgs e)
        {
            m2 = new Map2();
            m2.Show();
            m2.Text = "路线规划";

        }

        private void vlcControl1_Click(object sender, EventArgs e)
        {

        }

    }

}
