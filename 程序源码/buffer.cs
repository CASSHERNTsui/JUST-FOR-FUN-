/*using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geoprocessor;
using ESRI.ArcGIS.Geoprocessing;
using ESRI.ArcGIS.AnalysisTools;

namespace 工师校园地貌
{
   // [DllImport("user32.dll")]

    public partial class buffer : DevComponents.DotNetBar.Office2007Form
    {
        [DllImport("user32.dll")]
        private static extern int PostMessage(IntPtr wnd, uint Msg, IntPtr wParam, IntPtr lParam);
        private IHookHelper m_hookHelper = null;
        private const uint WM_VSCROLL = 0x0115;
        private const uint SB_BOTTOM = 7;
        
        public buffer()
        {
            InitializeComponent();
            m_hookHelper = hookHelper;

        }

        public buffer(IHookHelper m_hookHelper)
        {
            // TODO: Complete member initialization
            this.m_hookHelper = m_hookHelper;
        }

        private void buffer_Load(object sender, EventArgs e)
        {
            if (null == m_hookHelper || null == m_hookHelper.Hook || 0 == m_hookHelper.FocusMap.LayerCount)
                return;
            //load all the feature layers in the map to the layers combo
            IEnumLayer layers = GetLayers();
            layers.Reset();
            ILayer layer = null;
            while ((layer = layers.Next()) != null)
            {
                cblayers.Items.Add(layer.Name);
            }
            //select the first layer
            if (cblayers.Items.Count > 0)
                cblayers.SelectedIndex = 0;
            string tempDir = System.IO.Path.GetTempPath();
            TextOutPutPath.Text = System.IO.Path.Combine(tempDir, ((string)cblayers.SelectedItem + "_buffer.shp"));
            cbunits.Items.Add("Meters");
        }

        private void Browes_Click(object sender, EventArgs e)
        {
            //set the output layer
            SaveFileDialog saveDlg = new SaveFileDialog();
            saveDlg.CheckPathExists = true;
            saveDlg.Filter = "Shapefile (*.shp)|*.shp";
            saveDlg.OverwritePrompt = true;
            saveDlg.Title = "Output Layer";
            saveDlg.RestoreDirectory = true;
            saveDlg.FileName = (string)cblayers.SelectedItem + "_buffer.shp";
            DialogResult dr = saveDlg.ShowDialog();
            if (dr == DialogResult.OK)
                TextOutPutPath.Text = saveDlg.FileName;
        }

        private void btBuffer_Click(object sender, EventArgs e)
        {
            double bufferDistance;
            double.TryParse(distance.Text, out bufferDistance);
            if (0.0 == bufferDistance)
            {
                MessageBox.Show("Bad buffer distance!");
                return;
            }
            if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(TextOutPutPath.Text)) ||
              ".shp" != System.IO.Path.GetExtension(TextOutPutPath.Text))
            {
                MessageBox.Show("Bad output filename!");
                return;
            }
            if (m_hookHelper.FocusMap.LayerCount == 0)
                return;
            IFeatureLayer layer = GetFeatureLayer((string)cblayers.SelectedItem);
            if (null == layer)
            {
                richTextBox1.Text += "Layer " + (string)cblayers.SelectedItem + "cannot be found!\r\n";
                return;
            }
            ScrollToBottom();
            richTextBox1.Text += "\r\n分析开始，这可能需要几分钟时间,请稍候..\r\n";
            richTextBox1.Update();
            Geoprocessor gp = new Geoprocessor();
            gp.OverwriteOutput = true;
            ESRI.ArcGIS.AnalysisTools.Buffer buffer = new ESRI.ArcGIS.AnalysisTools.Buffer(layer, TextOutPutPath.Text, Convert.ToString(bufferDistance) + " " + (string)cbunits.SelectedItem);
            buffer.dissolve_option = "ALL";
            IGeoProcessorResult results = null;
            try
            {
                results = (IGeoProcessorResult)gp.Execute(buffer, null);
            }
            catch (Exception ex)
            {
                richTextBox1.Text += "Failed to buffer layer: " + layer.Name + "\r\n";
            }
            if (results.Status != esriJobStatus.esriJobSucceeded)
            {
                richTextBox1.Text += "Failed to buffer layer: " + layer.Name + "\r\n";
            }
            ScrollToBottom();
            richTextBox1.Text += "\r\n分析完成.\r\n";
            richTextBox1.Text += "-----------------------\r\n";
            ScrollToBottom();
        }

        private void close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private string ReturnMessages(Geoprocessor gp)
        {
            StringBuilder sb = new StringBuilder();
            if (gp.MessageCount > 0)
            {
                for (int Count = 0; Count <= gp.MessageCount - 1; Count++)
                {
                    System.Diagnostics.Trace.WriteLine(gp.GetMessage(Count));
                    sb.AppendFormat("{0}\n", gp.GetMessage(Count));
                }
            }
            return sb.ToString();
        }

        private IFeatureLayer GetFeatureLayer(string layerName)
        {
            //get the layers from the maps
            IEnumLayer layers = GetLayers();
            layers.Reset();
            ILayer layer = null;
            while ((layer = layers.Next()) != null)
            {
                if (layer.Name == layerName)
                    return layer as IFeatureLayer;
            }
            return null;
        }

        private IEnumLayer GetLayers()
        {
            UID uid = new UIDClass();
            uid.Value = "{40A9E885-5533-11d0-98BE-00805F7CED21}";
            IEnumLayer layers = m_hookHelper.FocusMap.get_Layers(uid, true);
            return layers;
        }

        private void ScrollToBottom()
        {
            PostMessage((IntPtr)richTextBox1.Handle, WM_VSCROLL, (IntPtr)SB_BOTTOM, (IntPtr)IntPtr.Zero);
        }


        public IHookHelper hookHelper { get; set; }
    }
}
