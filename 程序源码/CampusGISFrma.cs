/*using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geometry;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesFile;
using stdole;
using System.IO;
using System.Data.OleDb;
using ESRI.ArcGIS.NetworkAnalysis;
using ESRI.ArcGIS.GeoDatabaseUI;
using ESRI.ArcGIS.SystemUI;
using System.Configuration;
using ESRI.ArcGIS.AnalysisTools;
using ESRI.ArcGIS.Geoprocessing;
using ESRI.ArcGIS.Geoprocessor;
using ESRI.ArcGIS.Output;
using ESRI.ArcGIS.DisplayUI;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
namespace 工师校园地貌
{
    public partial class CampusGISFrma : DevComponents.DotNetBar.Office2007Form
    {
        #region 变量
        WelcomeForm wfrm1;
        public string biaoshi;
        string strDir;
        string strshuxing;
        string strjianzhu;
        bool blfangda = false;//放大
        bool blsuoxiao = false;//缩小
        bool blmanyou = false;//漫游
        bool blshuxing = false;//属性
        bool bllouceng = false;//楼层
        public bool bljianzhu = false;//建筑
        bool blhuadian = false;//画点
        bool blhuaxian = false;//画线
        DataTable tabledata;//定义一个数据表的变量
        private int identify = 0;
        ITOCControl m_TOCControl;//定义ITOCControl类型的变量
        ILayer pMoveLayer;//定义图层变量
        ILayer pLayer = null;
        private IMapControl3 m_mapControl = null;
        private ESRI.ArcGIS.Controls.IPageLayoutControl2 m_pageLayoutControl = null;
        private IMapDocument pMapDocument;
        private Attribute frmOpenAttributeTable;//定义属性类型的变量
        #endregion

        #region 变量2
        private IActiveView m_ipActiveView;
        private IMap m_ipMap;
        private bool clicked;
        private IPolyline m_ipPolyline;
        IGraphicsContainer pGC;
        private int identifier = 0;
        private IGeometricNetwork m_ipGeometricNetwork;
        private IPointToEID m_ipPointToEID;
        private IPointCollection m_ipPoints;
        private IEnumNetEID m_ipEnumNetEID_Junctions;
        private IEnumNetEID m_ipEnumNetEID_Edges;
        private double m_dblPathCost = 0;
        int clickedcount = 0;
        Form aboutGIS;
        Form feedback;
        Form help;
        #endregion


        public CampusGISFrma()
        {
            this.EnableGlass = false;
            InitializeComponent();
        }

        private void 空间分析ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void CampusGISFrma_Load(object sender, EventArgs e)
        {
            //取得MapControl和PageControl的引用
            axMapControldaohang1.ClearLayers();//清除axMapControldaohang1的原有图层
            axMapControl1.ClearLayers();//清除axMapControl1的原有图层
            axMapControldaohang1.AutoMouseWheel = false;//屏蔽鼠标滚轮放大缩小事件
            AddData();//加载地图数据
            AddLayerToOverViewMap();//将地图加到鹰眼里
            axMapControl1.Extent = axMapControl1.FullExtent;//设置全图
            m_TOCControl = axTOCControl1.Object as ITOCControl;//获取TOCControl的接口
            frmOpenAttributeTable = new Attribute();//建立新的Attribute类
            m_mapControl = (IMapControl3)this.axMapControl1.Object;//获取IMapControl3的接口
            m_pageLayoutControl = (IPageLayoutControl2)this.axPageLayoutControl1.Object;//获取IpaeLayoutControl2的接口
            MessageBox.Show("吉林工师校园信息系统！");//对话框进行显示
            WelcomeForm f = new WelcomeForm();//定义对象f
        }

        private void MessageBoxEx(string p)
        {
            throw new Exception("The method or operation is not implemented.");//抛出异常
        }

        private void AddLayerToOverViewMap()
        {
            axMapControldaohang1.ClearLayers();
            for (int i = 0; i < axMapControl1.LayerCount; i++)
            {
                IObjectCopy objectcopy = new ObjectCopyClass();
                ILayer toCopyLayer = axMapControl1.get_Layer(i);
                IFeatureLayer toFL = toCopyLayer as IFeatureLayer;
                IFeatureClass toFC = toFL.FeatureClass;
                if (toFC.ShapeType == esriGeometryType.esriGeometryPoint)
                    continue;
                object copiedLayer = objectcopy.Copy(toCopyLayer);
                ILayer C = (new FeatureLayerClass()) as ILayer;
                object toOverwriteLayer = C;
                objectcopy.Overwrite(copiedLayer, ref toOverwriteLayer);
                axMapControldaohang1.AddLayer(C);
                Marshal.ReleaseComObject(objectcopy);
                Marshal.ReleaseComObject(toCopyLayer);
                Marshal.ReleaseComObject(C);
                toCopyLayer = null;
                objectcopy = null;
                C = null;
            }
        }
        private void OpenAttributeTable(IGeoFeatureLayer pLayer)
        {
            ITableView2 pTableView3;
            ITable pTable;
            tagRECT initialExtent;
            initialExtent = new tagRECT();
            initialExtent.left = 0;
            initialExtent.right = 400;
            initialExtent.top = 0;
            initialExtent.bottom = 550;
            pTableView3 = new TableViewClass();
            pTable = (ITable)pLayer.FeatureClass;
            IFeatureSelection curSel = (IFeatureSelection)pLayer;
            ISelectionSet curSet = curSel.SelectionSet;
            IFeatureClass curClass = pLayer.FeatureClass;
            pTableView3.Table = pTable;
            pTableView3.SelectionSet = curSet;
            if (!(frmOpenAttributeTable.Created)) frmOpenAttributeTable = new Attribute();
            System.Windows.Forms.DataGridView dataGrid;
            dataGrid = frmOpenAttributeTable.GetDataGrid;
            pTableView3.Show(dataGrid.Handle.ToInt32(), ref initialExtent, true);
            frmOpenAttributeTable.Text = pLayer.Name + "-Attribute Table";
            frmOpenAttributeTable.Show();
            frmOpenAttributeTable.TopMost = true;
        }
        public bool ExportMapToImage(IActiveView pActiveView, string fileName, int filterIndex)
        {
            try
            {
                IExport pExporter = null;
                switch (filterIndex)
                {
                    case 1:
                        pExporter = new ExportJPEGClass();
                        break;
                    case 2:
                        pExporter = new ExportBMPClass();
                        break;
                    case 3:
                        pExporter = new ExportEMFClass();
                        break;
                    case 4:
                        pExporter = new ExportGIFClass();
                        break;
                    case 5:
                        pExporter = new ExportAIClass();
                        break;
                    case 6:
                        pExporter = new ExportPDFClass();
                        break;
                    case 7:
                        pExporter = new ExportPNGClass();
                        break;
                    case 8:
                        pExporter = new ExportPSClass();
                        break;
                    case 9:
                        pExporter = new ExportSVGClass();
                        break;
                    case 10:
                        pExporter = new ExportTIFFClass();
                        break;
                    default:
                        MessageBox.Show("输出格式错误");
                        return false;
                }
                IEnvelope pEnvelope = new EnvelopeClass();
                ITrackCancel pTrackCancel = new CancelTrackerClass();
                tagRECT ptagRECT;
                ptagRECT.left = 0;
                ptagRECT.top = 0;
                ptagRECT.right = 20 * (int)pActiveView.Extent.Width;
                ptagRECT.bottom = 20 * (int)pActiveView.Extent.Height;
                int pResolution = (int)(pActiveView.ScreenDisplay.DisplayTransformation.Resolution);
                pEnvelope.PutCoords(ptagRECT.left, ptagRECT.bottom, ptagRECT.right, ptagRECT.top);
                pExporter.Resolution = pResolution;
                pExporter.ExportFileName = fileName;
                pExporter.PixelBounds = pEnvelope;
                pActiveView.Output(pExporter.StartExporting(), pResolution, ref ptagRECT, pActiveView.Extent, pTrackCancel);
                pExporter.FinishExporting();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(pExporter);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "输出图片", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
        }





        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void axToolbarControl1_OnMouseDown(object sender, ESRI.ArcGIS.Controls.IToolbarControlEvents_OnMouseDownEvent e)
        {

        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void CampusGISFrma_Shown(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            axMapControl1.Extent = axMapControl1.FullExtent;
            axMapControldaohang1.Extent = axMapControldaohang1.FullExtent;
            axMapControldaohang1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
            axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);

        }

    /*  private void axMapControl1_OnMouseMove(object sender, IMapControlEvents2_OnMouseMoveEvent e)
        {
            //显示当前比例尺
            ScaleLabel.Text = "比例尺1:" + ((long)this.axMapControl1.MapScale).ToString();
            //显示当前坐标
            CoordinateLabel.Text = " 当前坐标 X = " + e.mapX.ToString() + " Y = " + e.mapY.ToString() + " " + this.axMapControl1.MapUnits.ToString().Substring(4);

        }*/

        private void 打开Shape数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_mapControl = (IMapControl3)axMapControl1.Object;//使用IMapControl3接口
            IWorkspaceFactory workspaceFactory = new ShapefileWorkspaceFactory();//用IWorkspaceFactory接口建立新类
            //打开shapefile文件对话框
            System.Windows.Forms.OpenFileDialog openFileDialog = new OpenFileDialog(); openFileDialog.Filter = "shapefile文件(*.shp)|*.shp";
            openFileDialog.Title = "打开shapefile文件";
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;
            string sFilePath = openFileDialog.FileName;
            string sFolder = System.IO.Path.GetDirectoryName(sFilePath);
            string sFileName = System.IO.Path.GetFileName(sFilePath);
            IWorkspace workspace = workspaceFactory.OpenFromFile(sFolder, 0);
            IFeatureWorkspace featureworkspace = (IFeatureWorkspace)workspace;
            IFeatureClass featureClass = featureworkspace.OpenFeatureClass(sFileName);
            IFeatureLayer featureLayer = new FeatureLayerClass();
            featureLayer.FeatureClass = featureClass;
            featureLayer.Name = featureClass.AliasName;
            ILayer layer = (ILayer)featureLayer;
            m_mapControl.AddLayer(layer, 0);
            m_mapControl.ActiveView.Refresh();
        }

        private void 打开MXDToolStripMenuItem_Click(object sender, EventArgs e)
        {

            System.Windows.Forms.OpenFileDialog openFileDialog2 = new OpenFileDialog();
            openFileDialog2.Title = "打开地图文件";
            openFileDialog2.Filter = "地图文件 (*.mxd)|*.mxd";
            openFileDialog2.ShowDialog();
            string sFilePath = openFileDialog2.FileName;
            if (axMapControl1.CheckMxFile(sFilePath))
            {
                axMapControl1.MousePointer = esriControlsMousePointer.esriPointerHourglass;
                axMapControl1.LoadMxFile(sFilePath, 0, Type.Missing);
                axMapControl1.MousePointer = esriControlsMousePointer.esriPointerDefault;
            }
            else
            {
                MessageBox.Show(sFilePath + " 为非法地图文件！");
                return;
            }
            //获取IMapControl3
            m_mapControl = (IMapControl3)axMapControl1.Object;
        }

        private void 新建文档ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //执行新建文档命令，询问用户是否保存当前文档
            DialogResult res = MessageBox.Show("是否保存当前文档？", "保存文档", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res == DialogResult.Yes)
            {
                ESRI.ArcGIS.SystemUI.ICommand command = new ControlsSaveAsDocCommandClass();
                command.OnCreate(m_mapControl.Object);
                command.OnClick();
            }
            //创建新地图
            IMap map = new MapClass();
            map.Name = "Map";
            //将地图放入控件中
            m_mapControl.DocumentFilename = string.Empty;
            m_mapControl.Map = map;
        }

        private void 保存ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // ESRI.ArcGIS.SystemUI.ICommand command = new ControlsSaveAsDocCommandClass();
            //  command.OnCreate(m_mapControl.Object);
            // command.OnClick();
            // 首先确认当前地图文档是否有效 
            if (null != m_pageLayoutControl.DocumentFilename && m_mapControl.CheckMxFile(m_pageLayoutControl.DocumentFilename))
            {
                // 创建一个新的地图文档实例 
                IMapDocument mapDoc = new MapDocumentClass();
                // 打开当前地图文档 
                mapDoc.Open(m_pageLayoutControl.DocumentFilename, string.Empty);
                // 用 PageLayout 中的文档替换当前文档中的 PageLayout 部分 
                mapDoc.ReplaceContents((IMxdContents)m_pageLayoutControl.PageLayout);
                // 保存地图文档 
                mapDoc.Save(mapDoc.UsesRelativePaths, false);
                mapDoc.Close();
            }
        }

        private void 另存为ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 调用另存为命令 
            ICommand command = new ControlsSaveAsDocCommandClass();
            command.OnCreate(this.axMapControl1.Object);
            command.OnClick();
        }

        private void 输出地图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //打开输出地图对话框
            saveFileDialog1.Filter = "JPEG(*.jpg)|*.jpg|BMP(*.BMP)|*.bmp|EMF(*.emf)|*.emf|GIF(*.gif)|*.gif|AI(*.ai)|*.ai|PDF(*.pdf)|*.pdf|PNG(*.png)|*.png|EPS(*.eps)|*.eps|SVG(*.svg)|*.svg|TIFF(*.tif)|*.tif";
            saveFileDialog1.Title = "输出地图";
            saveFileDialog1.RestoreDirectory = true;
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.ShowDialog();
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void 放大ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OrigronSet();//调用函数，还原初始设置
            axMapControl1.MousePointer = esriControlsMousePointer.esriPointerZoomIn;//放大
            blfangda = true;
            toolStripLabel2.Text = "当前操作状态:放大";
        }

        private void 缩小_Click(object sender, EventArgs e)
        {
            OrigronSet();//调用函数，还原初始设置
            axMapControl1.MousePointer = esriControlsMousePointer.esriPointerZoomOut;//缩小
            blsuoxiao = true;
            toolStripLabel2.Text = "当前操作状态:缩小";
        }

        private void 全图_Click(object sender, EventArgs e)
        {
            OrigronSet();//调用函数，还原初始设置
            axMapControl1.Extent = axMapControl1.FullExtent;//全图
            axMapControl1.ActiveView.Refresh();
            toolStripLabel2.Text = "当前操作状态:全图";
        }

        private void 漫游_Click(object sender, EventArgs e)
        {
            OrigronSet();//调用函数，还原初始设置
            axMapControl1.MousePointer = esriControlsMousePointer.esriPointerPan;//漫游
            blmanyou = true;
            toolStripLabel2.Text = "当前操作状态:漫游";
        }

        private void 属性_Click(object sender, EventArgs e)
        {
            OrigronSet();//调用函数，还原初始设置
            axMapControl1.MousePointer = esriControlsMousePointer.esriPointerIdentify;//设置鼠标的样式为属性
            blshuxing = true;
            toolStripLabel2.Text = "当前操作状态:属性查询";
        }

        private void OrigronSet()   //自定义函数，还原初始设置
        {
            axMapControl1.MousePointer = esriControlsMousePointer.esriPointerDefault;//刷新
            blfangda = false;
            blsuoxiao = false;
            blmanyou = false;
            blshuxing = false;
            bllouceng = false;
            bljianzhu = false;
            blhuadian = false;
            blhuaxian = false;
            identify = 0;
            axMapControl1.Map.ClearSelection();//清除地图上选择项
        }

        private void 放大ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OrigronSet();
            axMapControl1.MousePointer = esriControlsMousePointer.esriPointerZoomIn;//放大
            blfangda = true;
            toolStripLabel2.Text = "当前操作状态:放大";
        }

        private void 缩小ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OrigronSet();
            axMapControl1.MousePointer = esriControlsMousePointer.esriPointerZoomOut;//缩小
            blsuoxiao = true;
            toolStripLabel2.Text = "当前操作状态:缩小";
        }

        private void 全图ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OrigronSet();
            axMapControl1.Extent = axMapControl1.FullExtent;
            axMapControl1.ActiveView.Refresh();
            toolStripLabel2.Text = "当前操作状态:全图";
        }

        private void 漫游ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OrigronSet();
            axMapControl1.MousePointer = esriControlsMousePointer.esriPointerPan;
            blmanyou = true;
            toolStripLabel2.Text = "当前操作状态:漫游";
        }

        private void 刷新ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            axMapControl1.MousePointer = esriControlsMousePointer.esriPointerDefault;
            axMapControl1.ActiveView.Refresh();
            toolStripLabel2.Text = "当前操作状态:刷新";
        }

        private void axTOCControl1_OnMouseDown(object sender, ITOCControlEvents_OnMouseDownEvent e)
        {
            try
            {
                if (e.button == 1)
                {
                    esriTOCControlItem item = esriTOCControlItem.esriTOCControlItemNone;
                    IBasicMap map = null; ILayer layer = null;
                    object other = null; object index = null;
                    m_TOCControl.HitTest(e.x, e.y, ref item, ref map, ref layer, ref other, ref index);
                    if (item == esriTOCControlItem.esriTOCControlItemLayer)
                    {
                        if (layer is IAnnotationSublayer)
                        {
                            return;
                        }
                        else
                        {
                            pMoveLayer = layer;
                        }
                    }
                }
                if (e.button == 2)
                {
                    esriTOCControlItem pItem = esriTOCControlItem.esriTOCControlItemNone;
                    IBasicMap pMap = null;
                    object pOther = new object();
                    object pIndex = new object();
                    axTOCControl1.HitTest(e.x, e.y, ref pItem, ref pMap, ref pLayer, ref pOther, ref pIndex);
                    if (pItem == esriTOCControlItem.esriTOCControlItemLayer)
                    {
                        System.Drawing.Point p = new System.Drawing.Point();
                        p.X = e.x;
                        p.Y = e.y;
                        contextMenuStrip1.Show(axTOCControl1, p);
                        contextMenuStrip1.Show(axTOCControl1, p);
                    }
                }
            }
            catch
            {

            }

        }
        private IRgbColor getRGB(int r, int g, int b)
        {
            IRgbColor pColor;
            pColor = new RgbColorClass();
            pColor.Red = r;
            pColor.Green = g;
            pColor.Blue = b;
            return pColor;
        }

        private void axTOCControl1_OnMouseUp(object sender, ITOCControlEvents_OnMouseUpEvent e)
        {
            try
            {
                if (e.button == 1)
                {
                    int Toindex = 0;
                    esriTOCControlItem item = esriTOCControlItem.esriTOCControlItemNone;
                    IBasicMap map = null;
                    ILayer layer = null;
                    object other = null;
                    object index = null;
                    m_TOCControl.HitTest(e.x, e.y, ref item, ref map, ref layer, ref other, ref index);
                    IMap pMap = axMapControl1.ActiveView.FocusMap;
                    if (item == esriTOCControlItem.esriTOCControlItemLayer || layer != null)
                    {
                        if (pMoveLayer != layer)
                        {
                            ILayer pTempLayer;
                            for (int i = 0; i < pMap.LayerCount; i++)
                            {
                                pTempLayer = pMap.get_Layer(i);
                                if (pTempLayer == layer)
                                {
                                    Toindex = i;
                                }
                            }
                            pMap.MoveLayer(pMoveLayer, Toindex);
                            axMapControl1.ActiveView.Refresh();
                            m_TOCControl.Update();
                        }
                    }
                }
            }
            catch
            {

            }
        }

        private void axTOCControl1_OnDoubleClick(object sender, ITOCControlEvents_OnDoubleClickEvent e)
        {
            esriTOCControlItem toccItem = esriTOCControlItem.esriTOCControlItemNone;
            ILayer iLayer = null;
            IBasicMap iBasicMap = null;
            object unk = null;
            object data = null;
            if (e.button == 1)
            {
                axTOCControl1.HitTest(e.x, e.y, ref toccItem, ref iBasicMap, ref iLayer, ref unk, ref data);
                System.Drawing.Point pos = new System.Drawing.Point(e.x, e.y);
                if (toccItem == esriTOCControlItem.esriTOCControlItemLegendClass)
                {
                    ESRI.ArcGIS.Carto.ILegendClass pLC = new LegendClassClass();
                    ESRI.ArcGIS.Carto.ILegendGroup pLG = new LegendGroupClass();
                    if (unk is ILegendGroup)
                    {
                        pLG = (ILegendGroup)unk;
                    }
                    pLC = pLG.get_Class((int)data);
                    ISymbol pSym;
                    pSym = pLC.Symbol;
                    ESRI.ArcGIS.DisplayUI.ISymbolSelector pSS = new ESRI.ArcGIS.DisplayUI.SymbolSelectorClass();
                    bool bOK = false;
                    pSS.AddSymbol(pSym);
                    bOK = pSS.SelectSymbol(0);
                    if (bOK)
                    {
                        pLC.Symbol = pSS.GetSymbolAt(0);
                    }
                    this.axMapControl1.ActiveView.Refresh();
                    this.axTOCControl1.Refresh();
                }
            }
        }

        private void 添加图层ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //添加Shapefile图层
            toolStripLabel2.Text = "当前操作状态:添加图层";
            IWorkspaceFactory workspaceFactory = new ShapefileWorkspaceFactory();
            System.Windows.Forms.OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "shapefile文件(*.shp)|*.shp";
            openFileDialog.Title = "打开shapefile文件";
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;
            string sFilePath = openFileDialog.FileName;
            string sFolder = System.IO.Path.GetDirectoryName(sFilePath);
            string sFileName = System.IO.Path.GetFileName(sFilePath);
            IWorkspace workspace = workspaceFactory.OpenFromFile(sFolder, 0);
            IFeatureWorkspace featureworkspace = (IFeatureWorkspace)workspace;
            IFeatureClass featureClass = featureworkspace.OpenFeatureClass(sFileName);
            IFeatureLayer featureLayer = new FeatureLayerClass();
            featureLayer.FeatureClass = featureClass;
            featureLayer.Name = featureClass.AliasName;
            ILayer layer = (ILayer)featureLayer;
            m_mapControl.AddLayer(layer, 0);
            m_mapControl.ActiveView.Refresh();
        }

        private void 加载所有图层ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //pLayer为右键当前图层 ，删除  
            axMapControl1.Map.DeleteLayer(pLayer);
        }

        private void 删除当前图层ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int laycount = axMapControl1.Map.LayerCount;
            for (int i = laycount - 1; i >= 0; i--)
            {
                axMapControl1.Map.DeleteLayer(axMapControl1.Map.get_Layer(i));
            }
            axMapControl1.ActiveView.Refresh();//更新
        }

        private void 打开图层属性ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IGeoFeatureLayer pGeoFeatureLayer;
            pGeoFeatureLayer = (IGeoFeatureLayer)pLayer;
            OpenAttributeTable(pGeoFeatureLayer);//调用打开图层属性
        }

        private void 重新加载图层ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddData();
        }

        public void AddData()
        {
            #region 凯旋校区
      
            if (biaoshi == "系统查询")
            {
                IWorkspaceFactory pWorkspaceFactory = new ShapefileWorkspaceFactoryClass();
                IWorkspace pWorkspace = pWorkspaceFactory.OpenFromFile(Application.StartupPath + "\\数据\\凯旋校区", 0);
                strDir = Application.StartupPath + "\\数据\\凯旋校区\\楼层数据";
                strshuxing = Application.StartupPath + "\\数据\\凯旋校区\\文本数据";
                strjianzhu = Application.StartupPath + "\\数据\\凯旋校区\\建筑照片";
                IFeatureWorkspace pFeatureWorkspace = pWorkspace as IFeatureWorkspace;
                IFeatureClass pFeatureClass;
                IDataset pDataset;
                IFeatureLayer pFeatureLayer;
                ILayer pLayer;
                ISimpleRenderer pSRenderer;
                ISimpleFillSymbol pSFSymbol;
                IRgbColor rgbclr;
                IFeatureRenderer pFRenderer;
                ISymbol sym;
                IGeoFeatureLayer pGFLyr;
                ISimpleLineSymbol pSlSymbol;
                //添加砖路面数据
                pFeatureClass = pFeatureWorkspace.OpenFeatureClass("砖路面.shp");
                pDataset = pFeatureClass as IDataset;
                pFeatureLayer = new FeatureLayerClass();
                pFeatureLayer.FeatureClass = pFeatureClass;
                pFeatureLayer.Name = pDataset.Name;
                pLayer = pFeatureLayer as ILayer;
                pSRenderer = new SimpleRendererClass();
                pSFSymbol = new SimpleFillSymbolClass();
                rgbclr = new RgbColorClass();
                rgbclr.Red = 255;
                rgbclr.Green = 255;
                rgbclr.Blue = 115;
                pSFSymbol.Color = rgbclr;
                sym = pSFSymbol as ISymbol;
                pSRenderer.Symbol = sym;
                pGFLyr = pLayer as IGeoFeatureLayer;
                pFRenderer = pSRenderer as IFeatureRenderer;
                pGFLyr.Renderer = pFRenderer;
                axMapControl1.AddLayer(pLayer);
                Marshal.ReleaseComObject(pFeatureClass);
                Marshal.ReleaseComObject(pDataset);
                Marshal.ReleaseComObject(pFeatureLayer);
                Marshal.ReleaseComObject(pLayer);
                Marshal.ReleaseComObject(pSRenderer);
                Marshal.ReleaseComObject(pSFSymbol);
                Marshal.ReleaseComObject(rgbclr);
                Marshal.ReleaseComObject(pFRenderer);
                Marshal.ReleaseComObject(sym);
                Marshal.ReleaseComObject(pGFLyr);
                //添加道路数据
                pFeatureClass = pFeatureWorkspace.OpenFeatureClass("道路.shp");
                pDataset = pFeatureClass as IDataset;
                pFeatureLayer = new FeatureLayerClass();
                pFeatureLayer.FeatureClass = pFeatureClass;
                pFeatureLayer.Name = pDataset.Name;
                pLayer = pFeatureLayer as ILayer;
                pSRenderer = new SimpleRendererClass();
                pSFSymbol = new SimpleFillSymbolClass();
                rgbclr = new RgbColorClass();
                rgbclr.Red = 178;
                rgbclr.Green = 178;
                rgbclr.Blue = 178;
                pSFSymbol.Color = rgbclr;
                sym = pSFSymbol as ISymbol;
                pSRenderer.Symbol = sym;
                pGFLyr = pLayer as IGeoFeatureLayer;
                pFRenderer = pSRenderer as IFeatureRenderer;
                pGFLyr.Renderer = pFRenderer;
                axMapControl1.AddLayer(pLayer);
                Marshal.ReleaseComObject(pFeatureClass);
                Marshal.ReleaseComObject(pDataset);
                Marshal.ReleaseComObject(pFeatureLayer);
                Marshal.ReleaseComObject(pLayer);
                Marshal.ReleaseComObject(pSRenderer);
                Marshal.ReleaseComObject(pSFSymbol);
                Marshal.ReleaseComObject(rgbclr);
                Marshal.ReleaseComObject(pFRenderer);
                Marshal.ReleaseComObject(sym);
                Marshal.ReleaseComObject(pGFLyr);
                //添加服务设施数据
                pFeatureClass = pFeatureWorkspace.OpenFeatureClass("服务设施.shp");
                pDataset = pFeatureClass as IDataset;
                pFeatureLayer = new FeatureLayerClass();
                pFeatureLayer.FeatureClass = pFeatureClass;
                pFeatureLayer.Name = pDataset.Name;
                pLayer = pFeatureLayer as ILayer;
                pSRenderer = new SimpleRendererClass();
                pSFSymbol = new SimpleFillSymbolClass();
                rgbclr = new RgbColorClass();
                rgbclr.Red = 205;
                rgbclr.Green = 205;
                rgbclr.Blue = 100;
                pSFSymbol.Color = rgbclr;
                sym = pSFSymbol as ISymbol;
                pSRenderer.Symbol = sym;
                pGFLyr = pLayer as IGeoFeatureLayer;
                pFRenderer = pSRenderer as IFeatureRenderer;
                pGFLyr.Renderer = pFRenderer;
                axMapControl1.AddLayer(pLayer);
                Marshal.ReleaseComObject(pFeatureClass);
                Marshal.ReleaseComObject(pDataset);
                Marshal.ReleaseComObject(pFeatureLayer);
                Marshal.ReleaseComObject(pLayer);
                Marshal.ReleaseComObject(pSRenderer);
                Marshal.ReleaseComObject(pSFSymbol);
                Marshal.ReleaseComObject(rgbclr);
                Marshal.ReleaseComObject(pFRenderer);
                Marshal.ReleaseComObject(sym);
                Marshal.ReleaseComObject(pGFLyr);
                //添加居民楼数据
                pFeatureClass = pFeatureWorkspace.OpenFeatureClass("居民楼.shp");
                pDataset = pFeatureClass as IDataset;
                pFeatureLayer = new FeatureLayerClass();
                pFeatureLayer.FeatureClass = pFeatureClass;
                pFeatureLayer.Name = pDataset.Name;
                pLayer = pFeatureLayer as ILayer;
                pSRenderer = new SimpleRendererClass();
                pSFSymbol = new SimpleFillSymbolClass();
                rgbclr = new RgbColorClass();
                rgbclr.Red = 112;
                rgbclr.Green = 68;
                rgbclr.Blue = 137;
                pSFSymbol.Color = rgbclr;
                sym = pSFSymbol as ISymbol;
                pSRenderer.Symbol = sym;
                pGFLyr = pLayer as IGeoFeatureLayer;
                pFRenderer = pSRenderer as IFeatureRenderer;
                pGFLyr.Renderer = pFRenderer;
                axMapControl1.AddLayer(pLayer);
                Marshal.ReleaseComObject(pFeatureClass);
                Marshal.ReleaseComObject(pDataset);
                Marshal.ReleaseComObject(pFeatureLayer);
                Marshal.ReleaseComObject(pLayer);
                Marshal.ReleaseComObject(pSRenderer);
                Marshal.ReleaseComObject(pSFSymbol);
                Marshal.ReleaseComObject(rgbclr);
                Marshal.ReleaseComObject(pFRenderer);
                Marshal.ReleaseComObject(sym);
                Marshal.ReleaseComObject(pGFLyr);
                //添加文体设施数据
                pFeatureClass = pFeatureWorkspace.OpenFeatureClass("文体设施.shp");
                pDataset = pFeatureClass as IDataset;
                pFeatureLayer = new FeatureLayerClass();
                pFeatureLayer.FeatureClass = pFeatureClass;
                pFeatureLayer.Name = pDataset.Name;
                pLayer = pFeatureLayer as ILayer;
                pSRenderer = new SimpleRendererClass();
                pSFSymbol = new SimpleFillSymbolClass();
                rgbclr = new RgbColorClass();
                rgbclr.Red = 255;
                rgbclr.Green = 128;
                rgbclr.Blue = 223;
                pSFSymbol.Color = rgbclr;
                sym = pSFSymbol as ISymbol;
                pSRenderer.Symbol = sym;
                pGFLyr = pLayer as IGeoFeatureLayer;
                pFRenderer = pSRenderer as IFeatureRenderer;
                pGFLyr.Renderer = pFRenderer;
                axMapControl1.AddLayer(pLayer);
                Marshal.ReleaseComObject(pFeatureClass);
                Marshal.ReleaseComObject(pDataset);
                Marshal.ReleaseComObject(pFeatureLayer);
                Marshal.ReleaseComObject(pLayer);
                Marshal.ReleaseComObject(pSRenderer);
                Marshal.ReleaseComObject(pSFSymbol);
                Marshal.ReleaseComObject(rgbclr);
                Marshal.ReleaseComObject(pFRenderer);
                Marshal.ReleaseComObject(sym);
                Marshal.ReleaseComObject(pGFLyr);
                //添加其他设施数据
                pFeatureClass = pFeatureWorkspace.OpenFeatureClass("其他设施.shp");
                pDataset = pFeatureClass as IDataset;
                pFeatureLayer = new FeatureLayerClass();
                pFeatureLayer.FeatureClass = pFeatureClass;
                pFeatureLayer.Name = pDataset.Name;
                pLayer = pFeatureLayer as ILayer;
                pSRenderer = new SimpleRendererClass();
                pSFSymbol = new SimpleFillSymbolClass();
                rgbclr = new RgbColorClass();
                rgbclr.Red = 68;
                rgbclr.Green = 137;
                rgbclr.Blue = 112;
                pSFSymbol.Color = rgbclr;
                sym = pSFSymbol as ISymbol;
                pSRenderer.Symbol = sym;
                pGFLyr = pLayer as IGeoFeatureLayer;
                pFRenderer = pSRenderer as IFeatureRenderer;
                pGFLyr.Renderer = pFRenderer;
                axMapControl1.AddLayer(pLayer);
                Marshal.ReleaseComObject(pFeatureClass);
                Marshal.ReleaseComObject(pDataset);
                Marshal.ReleaseComObject(pFeatureLayer);
                Marshal.ReleaseComObject(pLayer);
                Marshal.ReleaseComObject(pSRenderer);
                Marshal.ReleaseComObject(pSFSymbol);
                Marshal.ReleaseComObject(rgbclr);
                Marshal.ReleaseComObject(pFRenderer);
                Marshal.ReleaseComObject(sym);
                Marshal.ReleaseComObject(pGFLyr);
                //添加教学楼数据
                pFeatureClass = pFeatureWorkspace.OpenFeatureClass("教学楼.shp");
                pDataset = pFeatureClass as IDataset;
                pFeatureLayer = new FeatureLayerClass();
                pFeatureLayer.FeatureClass = pFeatureClass;
                pFeatureLayer.Name = pDataset.Name;
                pLayer = pFeatureLayer as ILayer;
                pSRenderer = new SimpleRendererClass();
                pSFSymbol = new SimpleFillSymbolClass();
                rgbclr = new RgbColorClass();
                rgbclr.Red = 168;
                rgbclr.Green = 110;
                rgbclr.Blue = 0;
                pSFSymbol.Color = rgbclr;
                sym = pSFSymbol as ISymbol;
                pSRenderer.Symbol = sym;
                pGFLyr = pLayer as IGeoFeatureLayer;
                pFRenderer = pSRenderer as IFeatureRenderer;
                pGFLyr.Renderer = pFRenderer;
                axMapControl1.AddLayer(pLayer);
                Marshal.ReleaseComObject(pFeatureClass);
                Marshal.ReleaseComObject(pDataset);
                Marshal.ReleaseComObject(pFeatureLayer);
                Marshal.ReleaseComObject(pLayer);
                Marshal.ReleaseComObject(pSRenderer);
                Marshal.ReleaseComObject(pSFSymbol);
                Marshal.ReleaseComObject(rgbclr);
                Marshal.ReleaseComObject(pFRenderer);
                Marshal.ReleaseComObject(sym);
                Marshal.ReleaseComObject(pGFLyr);
                //添加学生公寓数据
                pFeatureClass = pFeatureWorkspace.OpenFeatureClass("学生公寓.shp");
                pDataset = pFeatureClass as IDataset;
                pFeatureLayer = new FeatureLayerClass();
                pFeatureLayer.FeatureClass = pFeatureClass;
                pFeatureLayer.Name = pDataset.Name;
                pLayer = pFeatureLayer as ILayer;
                pSRenderer = new SimpleRendererClass();
                pSFSymbol = new SimpleFillSymbolClass();
                rgbclr = new RgbColorClass();
                rgbclr.Red = 202;
                rgbclr.Green = 120;
                rgbclr.Blue = 245;
                pSFSymbol.Color = rgbclr;
                sym = pSFSymbol as ISymbol;
                pSRenderer.Symbol = sym;
                pGFLyr = pLayer as IGeoFeatureLayer;
                pFRenderer = pSRenderer as IFeatureRenderer;
                pGFLyr.Renderer = pFRenderer;
                axMapControl1.AddLayer(pLayer);
                Marshal.ReleaseComObject(pFeatureClass);
                Marshal.ReleaseComObject(pDataset);
                Marshal.ReleaseComObject(pFeatureLayer);
                Marshal.ReleaseComObject(pLayer);
                Marshal.ReleaseComObject(pSRenderer);
                Marshal.ReleaseComObject(pSFSymbol);
                Marshal.ReleaseComObject(rgbclr);
                Marshal.ReleaseComObject(pFRenderer);
                Marshal.ReleaseComObject(sym);
                Marshal.ReleaseComObject(pGFLyr);
                //添加花坛数据
                pFeatureClass = pFeatureWorkspace.OpenFeatureClass("花坛.shp");
                pDataset = pFeatureClass as IDataset;
                pFeatureLayer = new FeatureLayerClass();
                pFeatureLayer.FeatureClass = pFeatureClass;
                pFeatureLayer.Name = pDataset.Name;
                pLayer = pFeatureLayer as ILayer;
                pSRenderer = new SimpleRendererClass();
                pSFSymbol = new SimpleFillSymbolClass();
                rgbclr = new RgbColorClass();
                rgbclr.Red = 220;
                rgbclr.Green = 0;
                rgbclr.Blue = 10;
                pSFSymbol.Color = rgbclr;
                sym = pSFSymbol as ISymbol;
                pSRenderer.Symbol = sym;
                pGFLyr = pLayer as IGeoFeatureLayer;
                pFRenderer = pSRenderer as IFeatureRenderer;
                pGFLyr.Renderer = pFRenderer;
                axMapControl1.AddLayer(pLayer);
                Marshal.ReleaseComObject(pFeatureClass);
                Marshal.ReleaseComObject(pDataset);
                Marshal.ReleaseComObject(pFeatureLayer);
                Marshal.ReleaseComObject(pLayer);
                Marshal.ReleaseComObject(pSRenderer);
                Marshal.ReleaseComObject(pSFSymbol);
                Marshal.ReleaseComObject(rgbclr);
                Marshal.ReleaseComObject(pFRenderer);
                Marshal.ReleaseComObject(sym);
                Marshal.ReleaseComObject(pGFLyr);
                //添加阶梯数据
                pFeatureClass = pFeatureWorkspace.OpenFeatureClass("阶梯.shp");
                pDataset = pFeatureClass as IDataset;
                pFeatureLayer = new FeatureLayerClass();
                pFeatureLayer.FeatureClass = pFeatureClass;
                pFeatureLayer.Name = pDataset.Name;
                pLayer = pFeatureLayer as ILayer;
                pSRenderer = new SimpleRendererClass();
                pSlSymbol = new SimpleLineSymbolClass();
                rgbclr = new RgbColorClass();
                rgbclr.Red = 128;
                rgbclr.Green = 128;
                rgbclr.Blue = 128;
                pSlSymbol.Color = rgbclr;
                sym = pSlSymbol as ISymbol;
                pSRenderer.Symbol = sym;
                pGFLyr = pLayer as IGeoFeatureLayer;
                pFRenderer = pSRenderer as IFeatureRenderer;
                pGFLyr.Renderer = pFRenderer;
                axMapControl1.AddLayer(pLayer);
                Marshal.ReleaseComObject(pFeatureClass);
                Marshal.ReleaseComObject(pDataset);
                Marshal.ReleaseComObject(pFeatureLayer);
                Marshal.ReleaseComObject(pLayer);
                Marshal.ReleaseComObject(pSRenderer);
                Marshal.ReleaseComObject(pSlSymbol);
                Marshal.ReleaseComObject(rgbclr);
                Marshal.ReleaseComObject(pFRenderer);
                Marshal.ReleaseComObject(sym);
                Marshal.ReleaseComObject(pGFLyr);
                //添加绿地数据
                pFeatureClass = pFeatureWorkspace.OpenFeatureClass("绿地.shp");
                pDataset = pFeatureClass as IDataset;
                pFeatureLayer = new FeatureLayerClass();
                pFeatureLayer.FeatureClass = pFeatureClass;
                pFeatureLayer.Name = pDataset.Name;
                pLayer = pFeatureLayer as ILayer;
                pSRenderer = new SimpleRendererClass();
                pSFSymbol = new SimpleFillSymbolClass();
                rgbclr = new RgbColorClass();
                rgbclr.Red = 56;
                rgbclr.Green = 168;
                rgbclr.Blue = 0;
                pSFSymbol.Color = rgbclr;
                sym = pSFSymbol as ISymbol;
                pSRenderer.Symbol = sym;
                pGFLyr = pLayer as IGeoFeatureLayer;
                pFRenderer = pSRenderer as IFeatureRenderer;
                pGFLyr.Renderer = pFRenderer;
                axMapControl1.AddLayer(pLayer);
                Marshal.ReleaseComObject(pFeatureClass);
                Marshal.ReleaseComObject(pDataset);
                Marshal.ReleaseComObject(pFeatureLayer);
                Marshal.ReleaseComObject(pLayer);
                Marshal.ReleaseComObject(pSRenderer);
                Marshal.ReleaseComObject(pSFSymbol);
                Marshal.ReleaseComObject(rgbclr);
                Marshal.ReleaseComObject(pFRenderer);
                Marshal.ReleaseComObject(sym);
                Marshal.ReleaseComObject(pGFLyr);
                //添加注释数据
                pFeatureClass = pFeatureWorkspace.OpenFeatureClass("注释");
                pDataset = pFeatureClass as IDataset;
                pFeatureLayer = new FeatureLayerClass();
                pFeatureLayer.FeatureClass = pFeatureClass;
                pFeatureLayer.Name = pDataset.Name;
                pLayer = pFeatureLayer as ILayer;
                axMapControl1.AddLayer(pLayer);
                Marshal.ReleaseComObject(pFeatureClass);
                Marshal.ReleaseComObject(pDataset);
                Marshal.ReleaseComObject(pFeatureLayer);
                Marshal.ReleaseComObject(pLayer);
                Marshal.ReleaseComObject(pWorkspaceFactory);
                Marshal.ReleaseComObject(pWorkspace);
                Marshal.ReleaseComObject(pFeatureWorkspace);
                pWorkspaceFactory = null;
                pWorkspace = null;
                pFeatureWorkspace = null;
                pFeatureClass = null;
                pSlSymbol = null;
                pDataset = null;
                pFeatureLayer = null;
                pLayer = null;
                pSRenderer = null;
                pSFSymbol = null;
                rgbclr = null;
                pFRenderer = null;
                sym = null;
                pGFLyr = null;
                //添加属性
                cmbselect.Items.Clear();
                DataTable table = Table(Application.StartupPath + "\\数据\\凯旋校区\\文本数据", "属性数据.xls");
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    ComboBox comboBox1 = new ComboBox();
                    ComboBox comboBox2 = new ComboBox();
                    cmbselect.Items.Add(table.Rows[i][0].ToString());
                    comboBox1.Items.Add(table.Rows[i][0].ToString());
                    comboBox2.Items.Add(table.Rows[i][0].ToString());
                }
                richtxtshuxing.Text = "";
                DataTable tabledata = new DataTable();
                tabledata = Table(Application.StartupPath + "\\数据\\凯旋校区\\文本数据", "基本情况.xls");
                for (int i = 0; i < tabledata.Rows.Count; i++)
                {
                    richtxtshuxing.Text += tabledata.Rows[i][0].ToString() + "\r\n";
                }
            }
        }
            #endregion

             private DataTable Table(string openPathName, string openName)
        {
            string ConnectionString = "Provider=Microsoft.Jet.Oledb.4.0;Data Source=" + openPathName + "\\" + openName.Substring(0, openName.Length - 4) + ".xls;" + "Extended Properties=\"Excel 8.0;HDR=No;IMEX=1\"";
            OleDbConnection connection = new OleDbConnection(ConnectionString);
            OleDbDataAdapter da = new OleDbDataAdapter("select * from [Sheet1$]", connection);
            DataSet ds = new DataSet();
            da.Fill(ds, "Book1");
            DataTable table = ds.Tables["Book1"];
            da.Dispose();
            ds.Dispose();
            connection.Close();
            return table;
        }

             private void axMapControldaohang1_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
             {
                 #region 鹰眼
                 if (e.button == 2)
                 {
                     try
                     {
                         IPoint pPt = new PointClass();
                         pPt.X = e.mapX;
                         pPt.Y = e.mapY;
                         IEnvelope pEnvelope = axMapControldaohang1.TrackRectangle();
                         axMapControl1.Extent = pEnvelope;
                     }
                     catch
                     {
                     }
                 }
                 #endregion
             }

             private void axMapControldaohang1_OnMouseMove(object sender, IMapControlEvents2_OnMouseMoveEvent e)
             {
                 if (e.button == 1)
                 {
                     try
                     {
                         IPoint pPt = new PointClass();
                         pPt.X = e.mapX;
                         pPt.Y = e.mapY;
                         //
                         IEnvelope pEnvelope = axMapControl1.Extent as IEnvelope;
                         pEnvelope.CenterAt(pPt);
                         IActiveView pAv;
                         IGraphicsContainer pGraphicsContainer = axMapControldaohang1.Map as IGraphicsContainer;
                         pAv = pGraphicsContainer as IActiveView;
                         pGraphicsContainer.DeleteAllElements();
                         IRectangleElement pRecElement = new RectangleElementClass();
                         IElement pEle = pRecElement as IElement;
                         pEle.Geometry = pEnvelope;
                         //颜色
                         IRgbColor pColor = new RgbColorClass();
                         pColor.Red = 200;
                         pColor.Green = 0;
                         pColor.Blue = 0;
                         pColor.Transparency = 255;
                         //产生一个线符号对象
                         ILineSymbol pLineSymbol = new SimpleLineSymbolClass();
                         pLineSymbol.Width = 2;
                         pLineSymbol.Color = pColor;
                         //设置填充符号的属性
                         IFillSymbol pFillSymbol = new SimpleFillSymbolClass();
                         // 设置透明颜色
                         pColor.Transparency = 0;
                         pFillSymbol.Color = pColor;
                         pFillSymbol.Outline = pLineSymbol;
                         //
                         IFillShapeElement pFillShapeElement = pRecElement as IFillShapeElement;
                         pFillShapeElement.Symbol = pFillSymbol;
                         pGraphicsContainer.AddElement(pEle, 0);
                         axMapControldaohang1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
                     }
                     catch
                     {
                     }
                 }
             }

             private void axMapControldaohang1_OnMouseUp(object sender, IMapControlEvents2_OnMouseUpEvent e)
             {
                 if (e.button == 1)
                 {
                     try
                     {
                         IPoint pPt = new PointClass();
                         pPt.X = e.mapX;
                         pPt.Y = e.mapY;
                         IEnvelope pEnvelope = axMapControl1.Extent as IEnvelope;
                         pEnvelope.CenterAt(pPt);
                         axMapControl1.Extent = pEnvelope;
                     }
                     catch
                     {
                     }
                 }
                 // #endregion
             }

             private void 添加图层ToolStripMenuItem1_Click(object sender, EventArgs e)
             {
                 toolStripLabel2.Text = "当前操作状态:添加图层";
                 IWorkspaceFactory workspaceFactory = new ShapefileWorkspaceFactory();
                 System.Windows.Forms.OpenFileDialog openFileDialog = new OpenFileDialog();
                 openFileDialog.Filter = "shapefile文件(*.shp)|*.shp";
                 openFileDialog.Title = "打开shapefile文件";
                 openFileDialog.Multiselect = true;
                 if (openFileDialog.ShowDialog() != DialogResult.OK)
                     return;
                 string sFilePath = openFileDialog.FileName;
                 string sFolder = System.IO.Path.GetDirectoryName(sFilePath);
                 string sFileName = System.IO.Path.GetFileName(sFilePath);
                 IWorkspace workspace = workspaceFactory.OpenFromFile(sFolder, 0);
                 IFeatureWorkspace featureworkspace = (IFeatureWorkspace)workspace;
                 IFeatureClass featureClass = featureworkspace.OpenFeatureClass(sFileName);
                 IFeatureLayer featureLayer = new FeatureLayerClass();
                 featureLayer.FeatureClass = featureClass;
                 featureLayer.Name = featureClass.AliasName;
                 ILayer layer = (ILayer)featureLayer;
                 m_mapControl.AddLayer(layer, 0);
                 m_mapControl.ActiveView.Refresh();
             }

             private void 加载所有图层ToolStripMenuItem1_Click(object sender, EventArgs e)
             {
                 AddData();
             }

             private void 删除当前图层ToolStripMenuItem1_Click(object sender, EventArgs e)
             {
                 //pLayer为右键当前图层   
                 axMapControl1.Map.DeleteLayer(pLayer);
             }

             private void 删除所有图层ToolStripMenuItem1_Click(object sender, EventArgs e)
             {
                 int laycount = axMapControl1.Map.LayerCount;
                 for (int i = laycount - 1; i >= 0; i--)
                 {
                     axMapControl1.Map.DeleteLayer(axMapControl1.Map.get_Layer(i));
                 }
                 axMapControl1.ActiveView.Refresh();
             }

             private void 打开图层属性ToolStripMenuItem1_Click(object sender, EventArgs e)
             {
                 IGeoFeatureLayer pGeoFeatureLayer;
                 pGeoFeatureLayer = (IGeoFeatureLayer)pLayer;
                 OpenAttributeTable(pGeoFeatureLayer);
             }

             private void 缓冲区分析ToolStripMenuItem_Click(object sender, EventArgs e)
             {
                 BaseCommand bc = new Command1();
                 bc.OnCreate(this.axMapControl1.Object);
                 bc.OnClick();
                 toolStripLabel2.Text = "当前操作状态:缓冲区分析";
             }

           /*  private void axMapControl1_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
             {

                 //显示当前比例尺
                 ScaleLabel.Text = "比例尺1:" + ((long)this.axMapControl1.MapScale).ToString();
                 //显示当前坐标
                 CoordinateLabel.Text = " 当前坐标 X = " + e.mapX.ToString() + " Y = " + e.mapY.ToString() + " " + this.axMapControl1.MapUnits.ToString().Substring(4);

             }*/

             private void 返回主界面ToolStripMenuItem_Click(object sender, EventArgs e)
             {
                 WelcomeForm F = new WelcomeForm();
                 F.Show();
                 this.Hide();
             }

             private void 画点ToolStripMenuItem_Click(object sender, EventArgs e)
             {
                 OrigronSet();
                 blhuadian = true;
                 m_mapControl.MousePointer = esriControlsMousePointer.esriPointerCrosshair;
                 toolStripLabel2.Text = "当前操作状态:画点";
             }

             private void 画线ToolStripMenuItem_Click(object sender, EventArgs e)
             {
                 OrigronSet();
                 blhuaxian = true;
                 m_mapControl.MousePointer = esriControlsMousePointer.esriPointerCrosshair;
                 toolStripLabel2.Text = "当前操作状态:画线";
             }

             private void 测量距离ToolStripMenuItem_Click(object sender, EventArgs e)
             {
                 OrigronSet();
                 identifier = 1;
                 m_mapControl.MousePointer = esriControlsMousePointer.esriPointerCrosshair;
                 toolStripLabel2.Text = "当前操作状态:距离测量";
             }

             private void 面积测量ToolStripMenuItem_Click(object sender, EventArgs e)
             {
                 OrigronSet();
                 identifier = 2;
                 m_mapControl.MousePointer = esriControlsMousePointer.esriPointerCrosshair;
                 toolStripLabel2.Text = "当前操作状态:面积测量";
             }

             private void 指北针ToolStripMenuItem_Click(object sender, EventArgs e)
             {
                 AddNorthArrow(axPageLayoutControl1.PageLayout);
             }

             private void 图例ToolStripMenuItem_Click(object sender, EventArgs e)
             {
                 AddLegend(axPageLayoutControl1.PageLayout);
             }

             public void AddNorthArrow(IPageLayout pageLayout)
             {
                 IGraphicsContainer container = pageLayout as IGraphicsContainer;
                 IActiveView activeView = pageLayout as IActiveView;
                 // 获得MapFrame
                 IFrameElement frameElement = container.FindFrame(activeView.FocusMap);
                 IMapFrame mapFrame = frameElement as IMapFrame;
                 //根据MapSurround的uid，创建相应的MapSurroundFrame和MapSurround
                 UID uid = new UIDClass();
                 uid.Value = "esriCarto.MarkerNorthArrow";
                 IMapSurroundFrame mapSurroundFrame = mapFrame.CreateSurroundFrame(uid, null);
                 //设置MapSurroundFrame中指北针的点符号
                 IMapSurround mapSurround = mapSurroundFrame.MapSurround;
                 IMarkerNorthArrow markerNorthArrow = mapSurround as IMarkerNorthArrow;
                 IMarkerSymbol markerSymbol = markerNorthArrow.MarkerSymbol;
                 markerSymbol.Size = 115;
                 markerNorthArrow.MarkerSymbol = markerSymbol;
                 //QI，确定mapSurroundFrame的位置
                 IElement element = mapSurroundFrame as IElement;
                 IEnvelope envelope = new EnvelopeClass();
                 envelope.PutCoords(14.5, 18, 16.5, 20);
                 element.Geometry = envelope;
                 //使用IGraphicsContainer接口添加显示
                 container.AddElement(element, 0);
                 activeView.Refresh();
             }

             private void AddLegend(IPageLayout pageLayout)
             {
                 IActiveView pActiveView = pageLayout as IActiveView;
                 IGraphicsContainer container = pageLayout as IGraphicsContainer;
                 // 获得MapFrame
                 IMapFrame mapFrame = container.FindFrame(pActiveView.FocusMap) as IMapFrame;
                 //根据MapSurround的uid，创建相应的MapSurroundFrame和MapSurround
                 UID uid = new UIDClass();
                 uid.Value = "esriCarto.Legend";
                 IMapSurroundFrame mapSurroundFrame = mapFrame.CreateSurroundFrame(uid, null);
                 //设置图例的Title
                 ILegend2 legend = mapSurroundFrame.MapSurround as ILegend2;
                 legend.Title = "地图图例";
                 ILegendFormat format = new LegendFormatClass();
                 ITextSymbol symbol = new TextSymbolClass();
                 symbol.Size = 21;
                 format.TitleSymbol = symbol;
                 legend.Format = format;
                 //QI，确定mapSurroundFrame的位置
                 IElement element = mapSurroundFrame as IElement;
                 IEnvelope envelope = new EnvelopeClass();
                 envelope.PutCoords(14.5, 9, 16.5, 11);
                 element.Geometry = envelope;
                 //使用IGraphicsContainer接口添加显示
                 container.AddElement(element, 0);
                 pActiveView.Refresh();
             }

             private void CopyAndOverwriteMap()
             {
                 AxPageLayoutControl plc = new AxPageLayoutControl();
                 ((System.ComponentModel.ISupportInitialize)(plc)).BeginInit();
                 this.Controls.Add(plc);
                 ((System.ComponentModel.ISupportInitialize)(plc)).EndInit();
                 IObjectCopy objectCopy = new ObjectCopyClass();
                 object toCopyMap = axMapControl1.Map;
                 object copiedMap = objectCopy.Copy(toCopyMap);
                 object toOverwriteMap = axPageLayoutControl1.ActiveView.FocusMap;
                 objectCopy.Overwrite(copiedMap, ref toOverwriteMap);
             }

             private void axMapControl1_OnAfterScreenDraw(object sender, IMapControlEvents2_OnAfterScreenDrawEvent e)
             {
                 IActiveView activeView = (IActiveView)axPageLayoutControl1.ActiveView.FocusMap;
                 IDisplayTransformation displayTransformation = activeView.ScreenDisplay.DisplayTransformation;
                 displayTransformation.VisibleBounds = axMapControl1.Extent;
                 axPageLayoutControl1.ActiveView.Refresh();
                 CopyAndOverwriteMap();
             }

             private void axMapControl1_OnViewRefreshed(object sender, IMapControlEvents2_OnViewRefreshedEvent e)
             {
                 CopyAndOverwriteMap();
             }

             private void axMapControl1_OnMouseDown_1(object sender, IMapControlEvents2_OnMouseDownEvent e)
             {
                  #region 右键菜单
                    if (e.button == 2)
                    {
                        System.Drawing.Point p = new System.Drawing.Point();
                        p.X = e.x;
                        p.Y = e.y;
                        contextMenuStrip1.Show(axMapControl1, p);
                    }
                    #endregion
                    #region 放大
                    if (blfangda == true && axMapControl1.MousePointer == esriControlsMousePointer.esriPointerZoomIn && e.button == 1)
                    {
                        IPoint pt = new PointClass();
                        pt = axMapControl1.ToMapPoint(e.x, e.y);
                        IEnvelope IEnvTrc;
                        IEnvTrc = axMapControl1.TrackRectangle();
                        if (IEnvTrc.IsEmpty == true)
                        {
                            IEnvTrc = axMapControl1.ActiveView.Extent;
                            IEnvTrc.Expand(0.7, 0.7, true);
                            IEnvTrc.CenterAt(pt);
                        }
                        else
                        {
                            double XMin = IEnvTrc.XMin;
                            double YMin = IEnvTrc.YMin;
                            double XMax = IEnvTrc.XMax;
                            double YMax = IEnvTrc.YMax;
                            if (XMin == XMax || YMin == YMax)
                            {
                                IEnvTrc = axMapControl1.ActiveView.Extent;
                                IEnvTrc.Expand(0.7, 0.7, true);
                                IEnvTrc.CenterAt(pt);
                            }
                        }
                        axMapControl1.Extent = IEnvTrc;
                        Marshal.ReleaseComObject(pt);
                        Marshal.ReleaseComObject(IEnvTrc);
                        pt = null;
                        IEnvTrc = null;
                        return;
                    }
                    #endregion
                    #region 缩小
                    else if (blsuoxiao == true && axMapControl1.MousePointer == esriControlsMousePointer.esriPointerZoomOut && e.button == 1)
                    {
                        IPoint pt = new PointClass();
                        pt = axMapControl1.ToMapPoint(e.x, e.y);
                        IEnvelope IEnvTrc = axMapControl1.Extent;
                        IEnvTrc.Expand(1.5, 1.5, true);
                        axMapControl1.Extent = IEnvTrc;
                        Marshal.ReleaseComObject(pt);
                        Marshal.ReleaseComObject(IEnvTrc);
                        return;
                    }
                    #endregion
                    #region 漫游
                    else if (blmanyou == true && axMapControl1.MousePointer == esriControlsMousePointer.esriPointerPan && e.button == 1)
                    {
                        axMapControl1.Pan();
                        return;
                    }
                    #endregion
                    #region 属性
                    if (blshuxing == true && axMapControl1.MousePointer == esriControlsMousePointer.esriPointerIdentify && e.button == 1)
                    {
                        try
                        {
                            ILayer idenfyLayer;
                            IPoint point;
                            IFeature pfea;
                            IFields pfields;
                            IFeatureLayer idenfyFLayer;
                            IFeatureClass idenfyFClass;
                            ISpatialFilter ipSFilter;
                            IFeatureCursor ipFCursor;
                            IGeometry pGeo;
                            for (int k = 0; k < axMapControl1.LayerCount; k++)
                            {
                                idenfyLayer = axMapControl1.Map.get_Layer(k);
                                idenfyFLayer = idenfyLayer as IFeatureLayer;
                                idenfyFClass = idenfyFLayer.FeatureClass;
                                if (idenfyFClass.ShapeType == esriGeometryType.esriGeometryPolygon)
                                {
                                    point = axMapControl1.ActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(e.x, e.y);
                                    ipSFilter = new SpatialFilterClass();
                                    ipSFilter.Geometry = point as IGeometry;
                                    ipSFilter.GeometryField = "Shape";
                                    ipSFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                                    ipFCursor = idenfyFLayer.Search(ipSFilter as IQueryFilter, false);
                                    pfea = ipFCursor.NextFeature();
                                    while (pfea != null)
                                    {
                                        pfields = pfea.Fields;
                                        int index = pfields.FindField("居民楼");
                                        int index1 = pfields.FindField("文体设施");
                                        int index2 = pfields.FindField("教学楼");
                                        int index3 = pfields.FindField("学生公寓");
                                        int index4 = pfields.FindField("服务设施");
                                        int index5 = pfields.FindField("其他设施");
                                        int index6 = pfields.FindField("花坛");
                                        int index7 = pfields.FindField("绿地");
                                        int index8 = pfields.FindField("柏油路");
                                        int index9 = pfields.FindField("砖路面");
                                        if (index == -1 && index1 == -1 && index2 == -1 && index3 == -1 && index4 == -1 && index5 == -1 && index6 == -1 && index7 == -1 && index8 == -1 && index9 == -1)
                                        {
                                            Marshal.ReleaseComObject(idenfyLayer);
                                            Marshal.ReleaseComObject(idenfyFLayer);
                                            Marshal.ReleaseComObject(idenfyFClass);
                                            Marshal.ReleaseComObject(point);
                                            Marshal.ReleaseComObject(ipSFilter);
                                            Marshal.ReleaseComObject(ipFCursor);
                                            Marshal.ReleaseComObject(pfields);
                                            break;
                                        }
                                        richtxtshuxing.Text = "";
                                        pGeo = pfea.Shape;
                                        axMapControl1.FlashShape(pGeo, 2, 300, Type.Missing);
                                        if (index != -1)
                                        {
                                            string strLSMC = pfea.get_Value(pfields.FindField("居民楼")).ToString();
                                            DataTable table = Table(strshuxing, "属性数据.xls");
                                            for (int i = 1; i < table.Rows.Count; i++)
                                            {
                                                if (strLSMC == table.Rows[i][0].ToString())
                                                {
                                                    for (int j = 0; j < table.Columns.Count; j++)
                                                    {
                                                        richtxtshuxing.Text += table.Rows[0][j].ToString() + ":" + table.Rows[i][j].ToString() + "\n";
                                                    }
                                                    Marshal.ReleaseComObject(idenfyLayer);
                                                    Marshal.ReleaseComObject(idenfyFLayer);
                                                    Marshal.ReleaseComObject(idenfyFClass);
                                                    Marshal.ReleaseComObject(point);
                                                    Marshal.ReleaseComObject(ipSFilter);
                                                    Marshal.ReleaseComObject(ipFCursor);
                                                    Marshal.ReleaseComObject(pfields);
                                                    return;
                                                }
                                                else
                                                {
                                                    if (i == table.Rows.Count - 1)
                                                    {
                                                        MessageBox.Show("所选地物无属性信息");
                                                    }
                                                }
                                            }
                                            Marshal.ReleaseComObject(point);
                                            Marshal.ReleaseComObject(ipSFilter);
                                            Marshal.ReleaseComObject(ipFCursor);
                                            Marshal.ReleaseComObject(pfields);
                                            return;
                                        }
                                        if (index1 != -1)
                                        {
                                            string strLSMC = pfea.get_Value(pfields.FindField("文体设施")).ToString();
                                            DataTable table = Table(strshuxing, "属性数据.xls");
                                            for (int i = 1; i < table.Rows.Count; i++)
                                            {
                                                if (strLSMC == table.Rows[i][0].ToString())
                                                {
                                                    for (int j = 0; j < table.Columns.Count; j++)
                                                    {
                                                        richtxtshuxing.Text += table.Rows[0][j].ToString() + ":" + table.Rows[i][j].ToString() + "\n";
                                                    }
                                                    Marshal.ReleaseComObject(idenfyLayer);
                                                    Marshal.ReleaseComObject(idenfyFLayer);
                                                    Marshal.ReleaseComObject(idenfyFClass);
                                                    Marshal.ReleaseComObject(point);
                                                    Marshal.ReleaseComObject(ipSFilter);
                                                    Marshal.ReleaseComObject(ipFCursor);
                                                    Marshal.ReleaseComObject(pfields);
                                                    return;
                                                }
                                                else
                                                {
                                                    if (i == table.Rows.Count - 1)
                                                    {
                                                        MessageBox.Show("所选地物无属性信息");
                                                    }
                                                }
                                            }
                                            Marshal.ReleaseComObject(point);
                                            Marshal.ReleaseComObject(ipSFilter);
                                            Marshal.ReleaseComObject(ipFCursor);
                                            Marshal.ReleaseComObject(pfields);
                                            return;
                                        }
                                        if (index2 != -1)
                                        {
                                            string strLSMC = pfea.get_Value(pfields.FindField("教学楼")).ToString();
                                            DataTable table = Table(strshuxing, "属性数据.xls");
                                            for (int i = 1; i < table.Rows.Count; i++)
                                            {
                                                if (strLSMC == table.Rows[i][0].ToString())
                                                {
                                                    for (int j = 0; j < table.Columns.Count; j++)
                                                    {
                                                        richtxtshuxing.Text += table.Rows[0][j].ToString() + ":" + table.Rows[i][j].ToString() + "\n";
                                                    }
                                                    Marshal.ReleaseComObject(idenfyLayer);
                                                    Marshal.ReleaseComObject(idenfyFLayer);
                                                    Marshal.ReleaseComObject(idenfyFClass);
                                                    Marshal.ReleaseComObject(point);
                                                    Marshal.ReleaseComObject(ipSFilter);
                                                    Marshal.ReleaseComObject(ipFCursor);
                                                    Marshal.ReleaseComObject(pfields);
                                                    return;
                                                }
                                                else
                                                {
                                                    if (i == table.Rows.Count - 1)
                                                    {
                                                        MessageBox.Show("所选地物无属性信息");
                                                    }
                                                }
                                            }
                                            Marshal.ReleaseComObject(point);
                                            Marshal.ReleaseComObject(ipSFilter);
                                            Marshal.ReleaseComObject(ipFCursor);
                                            Marshal.ReleaseComObject(pfields);
                                            return;
                                        }
                                        if (index3 != -1)
                                        {
                                            string strLSMC = pfea.get_Value(pfields.FindField("学生公寓")).ToString();
                                            DataTable table = Table(strshuxing, "属性数据.xls");
                                            for (int i = 1; i < table.Rows.Count; i++)
                                            {
                                                if (strLSMC == table.Rows[i][0].ToString())
                                                {
                                                    for (int j = 0; j < table.Columns.Count; j++)
                                                    {
                                                        richtxtshuxing.Text += table.Rows[0][j].ToString() + ":" + table.Rows[i][j].ToString() + "\n";
                                                    }
                                                    Marshal.ReleaseComObject(idenfyLayer);
                                                    Marshal.ReleaseComObject(idenfyFLayer);
                                                    Marshal.ReleaseComObject(idenfyFClass);
                                                    Marshal.ReleaseComObject(point);
                                                    Marshal.ReleaseComObject(ipSFilter);
                                                    Marshal.ReleaseComObject(ipFCursor);
                                                    Marshal.ReleaseComObject(pfields);
                                                    return;
                                                }
                                                else
                                                {
                                                    if (i == table.Rows.Count - 1)
                                                    {
                                                        MessageBox.Show("所选地物无属性信息");
                                                    }
                                                }
                                            }
                                            Marshal.ReleaseComObject(point);
                                            Marshal.ReleaseComObject(ipSFilter);
                                            Marshal.ReleaseComObject(ipFCursor);
                                            Marshal.ReleaseComObject(pfields);
                                            return;
                                        }
                                        if (index4 != -1)
                                        {
                                            string strLSMC = pfea.get_Value(pfields.FindField("服务设施")).ToString();
                                            DataTable table = Table(strshuxing, "属性数据.xls");
                                            for (int i = 1; i < table.Rows.Count; i++)
                                            {
                                                if (strLSMC == table.Rows[i][0].ToString())
                                                {
                                                    for (int j = 0; j < table.Columns.Count; j++)
                                                    {
                                                        richtxtshuxing.Text += table.Rows[0][j].ToString() + ":" + table.Rows[i][j].ToString() + "\n";
                                                    }
                                                    Marshal.ReleaseComObject(idenfyLayer);
                                                    Marshal.ReleaseComObject(idenfyFLayer);
                                                    Marshal.ReleaseComObject(idenfyFClass);
                                                    Marshal.ReleaseComObject(point);
                                                    Marshal.ReleaseComObject(ipSFilter);
                                                    Marshal.ReleaseComObject(ipFCursor);
                                                    Marshal.ReleaseComObject(pfields);
                                                    return;
                                                }
                                                else
                                                {
                                                    if (i == table.Rows.Count - 1)
                                                    {
                                                        MessageBox.Show("所选地物无属性信息");
                                                    }
                                                }
                                            }
                                            Marshal.ReleaseComObject(point);
                                            Marshal.ReleaseComObject(ipSFilter);
                                            Marshal.ReleaseComObject(ipFCursor);
                                            Marshal.ReleaseComObject(pfields);
                                            return;
                                        }
                                        if (index5 != -1)
                                        {
                                            string strLSMC = pfea.get_Value(pfields.FindField("其他设施")).ToString();
                                            DataTable table = Table(strshuxing, "属性数据.xls");
                                            for (int i = 1; i < table.Rows.Count; i++)
                                            {
                                                if (strLSMC == table.Rows[i][0].ToString())
                                                {
                                                    for (int j = 0; j < table.Columns.Count; j++)
                                                    {
                                                        richtxtshuxing.Text += table.Rows[0][j].ToString() + ":" + table.Rows[i][j].ToString() + "\n";
                                                    }
                                                    Marshal.ReleaseComObject(idenfyLayer);
                                                    Marshal.ReleaseComObject(idenfyFLayer);
                                                    Marshal.ReleaseComObject(idenfyFClass);
                                                    Marshal.ReleaseComObject(point);
                                                    Marshal.ReleaseComObject(ipSFilter);
                                                    Marshal.ReleaseComObject(ipFCursor);
                                                    Marshal.ReleaseComObject(pfields);
                                                    return;
                                                }
                                                else
                                                {
                                                    if (i == table.Rows.Count - 1)
                                                    {
                                                        MessageBox.Show("所选地物无属性信息");
                                                    }
                                                }
                                            }
                                            Marshal.ReleaseComObject(point);
                                            Marshal.ReleaseComObject(ipSFilter);
                                            Marshal.ReleaseComObject(ipFCursor);
                                            Marshal.ReleaseComObject(pfields);
                                            return;
                                        }
                                        if (index6 != -1 || index7 != -1 || index8 != -1 || index9 != -1)
                                        {
                                            MessageBox.Show("所选地物无属性信息");
                                            return;
                                        }

                                    }
                                }
                                Marshal.ReleaseComObject(idenfyLayer);
                                Marshal.ReleaseComObject(idenfyFLayer);
                                Marshal.ReleaseComObject(idenfyFClass);
                            }
                            return;
                        }
                        catch
                        {
                        }
                    }
                    #endregion
                   /* #region 建筑照片
                    frmpicture frmP = new frmpicture(this);
                    if (bljianzhu == true && axMapControl1.MousePointer == esriControlsMousePointer.esriPointerDefault && e.button == 1)
                    {
                        if (bljianzhu == true)
                        {
                            try
                            {
                                frmP = new frmpicture(this);
                                frmP.Owner = this;
                                frmP.TopLevel = true;
                                frmP.StartPosition = FormStartPosition.Manual;
                                System.Drawing.Point pt = new System.Drawing.Point(e.x, e.y);
                                frmP.Location = this.PointToClient(pt);
                                frmP.Show();
                                bljianzhu = false;
                            }
                            catch
                            {
                            }
                        }
                        try
                        {
                            ILayer idenfyLayer;
                            IPoint point;
                            IFeature pfea;
                            IFields pfields;
                            IFeatureLayer idenfyFLayer;
                            ISpatialFilter ipSFilter;
                            IFeatureCursor ipFCursor;
                            IGeometry pGeo;
                            IFeatureClass pFC;
                            for (int k = 0; k < axMapControl1.LayerCount; k++)
                            {
                                idenfyLayer = axMapControl1.Map.get_Layer(k);
                                idenfyFLayer = idenfyLayer as IFeatureLayer;
                                pFC = idenfyFLayer.FeatureClass;
                                if (pFC.ShapeType == esriGeometryType.esriGeometryPolygon)
                                {
                                    point = axMapControl1.ActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(e.x, e.y);
                                    ipSFilter = new SpatialFilterClass();
                                    ipSFilter.Geometry = point as IGeometry;
                                    ipSFilter.GeometryField = "Shape";
                                    ipSFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                                    ipFCursor = idenfyFLayer.Search(ipSFilter as IQueryFilter, false);
                                    pfea = ipFCursor.NextFeature();
                                    while (pfea != null)
                                    {
                                        pGeo = pfea.Shape;
                                        axMapControl1.FlashShape(pGeo, 2, 300, Type.Missing);
                                        pfields = pfea.Fields;
                                        int index = pfields.FindField("教学楼");
                                        int index1 = pfields.FindField("学生公寓");
                                        int index2 = pfields.FindField("服务设施");
                                        int index3 = pfields.FindField("文体设施");
                                        int index4 = pfields.FindField("其他设施");
                                        int index5 = pfields.FindField("居民楼");
                                        if (index == -1 && index1 == -1 && index2 == -1 && index3 == -1 && index4 == -1 && index5 == -1)
                                        {
                                            MessageBox.Show("所选区域不是建筑！");
                                            break;
                                        }
                                        if (index != -1)
                                        {
                                            string strLSMC = pfea.get_Value(pfields.FindField("教学楼")).ToString();
                                            DirectoryInfo di = new DirectoryInfo(strjianzhu);
                                            if (di == null)
                                            {
                                                return;
                                            }
                                            if (strLSMC == "")
                                            {
                                                frmP.Hide();
                                                MessageBox.Show("该建筑无照片");
                                                break;
                                            }
                                            FileSystemInfo[] fs = di.GetFileSystemInfos();
                                            foreach (FileSystemInfo f2 in fs)
                                            {
                                                if (System.IO.Path.GetFileNameWithoutExtension(f2.FullName) == strLSMC)
                                                {
                                                    Bitmap bit = new Bitmap(f2.FullName);
                                                    frmP.pictureBox1.Image = bit;
                                                    frmP.Text = strLSMC;
                                                }
                                            }
                                            pfea = ipFCursor.NextFeature();
                                        }
                                        if (index1 != -1)
                                        {
                                            string strLSMC = pfea.get_Value(pfields.FindField("学生公寓")).ToString();
                                            DirectoryInfo di = new DirectoryInfo(strjianzhu);
                                            if (di == null)
                                            {
                                                return;
                                            }
                                            if (strLSMC == "")
                                            {
                                                frmP.Hide();
                                                MessageBox.Show("该建筑无照片");
                                                break;
                                            }
                                            FileSystemInfo[] fs = di.GetFileSystemInfos();
                                            foreach (FileSystemInfo f2 in fs)
                                            {
                                                if (System.IO.Path.GetFileNameWithoutExtension(f2.FullName) == strLSMC)
                                                {
                                                    Bitmap bit = new Bitmap(f2.FullName);
                                                    frmP.pictureBox1.Image = bit;
                                                    frmP.Text = strLSMC;
                                                }
                                            }
                                            pfea = ipFCursor.NextFeature();
                                        }
                                        if (index2 != -1)
                                        {
                                            string strLSMC = pfea.get_Value(pfields.FindField("服务设施")).ToString();
                                            DirectoryInfo di = new DirectoryInfo(strjianzhu);
                                            if (di == null)
                                            {
                                                return;
                                            }
                                            if (strLSMC == "")
                                            {
                                                frmP.Hide();
                                                MessageBox.Show("该建筑无照片");
                                                break;
                                            }
                                            FileSystemInfo[] fs = di.GetFileSystemInfos();
                                            foreach (FileSystemInfo f2 in fs)
                                            {
                                                if (System.IO.Path.GetFileNameWithoutExtension(f2.FullName) == strLSMC)
                                                {
                                                    Bitmap bit = new Bitmap(f2.FullName);
                                                    frmP.pictureBox1.Image = bit;
                                                    frmP.Text = strLSMC;
                                                }
                                            }
                                            pfea = ipFCursor.NextFeature();
                                        }
                                        if (index3 != -1)
                                        {
                                            string strLSMC = pfea.get_Value(pfields.FindField("文体设施")).ToString();
                                            DirectoryInfo di = new DirectoryInfo(strjianzhu);
                                            if (di == null)
                                            {
                                                return;
                                            }
                                            if (strLSMC == "")
                                            {
                                                frmP.Hide();
                                                MessageBox.Show("该建筑无照片");
                                                break;
                                            }
                                            FileSystemInfo[] fs = di.GetFileSystemInfos();
                                            foreach (FileSystemInfo f2 in fs)
                                            {
                                                if (System.IO.Path.GetFileNameWithoutExtension(f2.FullName) == strLSMC)
                                                {
                                                    Bitmap bit = new Bitmap(f2.FullName);
                                                    frmP.pictureBox1.Image = bit;
                                                    frmP.Text = strLSMC;
                                                }
                                            }
                                            pfea = ipFCursor.NextFeature();
                                        }
                                        if (index4 != -1)
                                        {
                                            string strLSMC = pfea.get_Value(pfields.FindField("其他设施")).ToString();
                                            DirectoryInfo di = new DirectoryInfo(strjianzhu);
                                            if (di == null)
                                            {
                                                return;
                                            }
                                            if (strLSMC == "")
                                            {
                                                frmP.Hide();
                                                MessageBox.Show("该建筑无照片");
                                                break;
                                            }
                                            FileSystemInfo[] fs = di.GetFileSystemInfos();
                                            foreach (FileSystemInfo f2 in fs)
                                            {
                                                if (System.IO.Path.GetFileNameWithoutExtension(f2.FullName) == strLSMC)
                                                {
                                                    Bitmap bit = new Bitmap(f2.FullName);
                                                    frmP.pictureBox1.Image = bit;
                                                    frmP.Text = strLSMC;
                                                }
                                            }
                                            pfea = ipFCursor.NextFeature();
                                        }
                                        if (index5 != -1)
                                        {
                                            string strLSMC = pfea.get_Value(pfields.FindField("居民楼")).ToString();
                                            DirectoryInfo di = new DirectoryInfo(strjianzhu);
                                            if (di == null)
                                            {
                                                return;
                                            }
                                            if (strLSMC == "")
                                            {
                                                frmP.Hide();
                                                MessageBox.Show("该建筑无照片");
                                                break;
                                            }
                                            FileSystemInfo[] fs = di.GetFileSystemInfos();
                                            foreach (FileSystemInfo f2 in fs)
                                            {
                                                if (System.IO.Path.GetFileNameWithoutExtension(f2.FullName) == strLSMC)
                                                {
                                                    Bitmap bit = new Bitmap(f2.FullName);
                                                    frmP.pictureBox1.Image = bit;
                                                    frmP.Text = strLSMC;
                                                }
                                            }
                                            pfea = ipFCursor.NextFeature();
                                        }
                                        Marshal.ReleaseComObject(point);
                                        Marshal.ReleaseComObject(ipSFilter);
                                        Marshal.ReleaseComObject(ipFCursor);
                                        idenfyFLayer = null;
                                        point = null;
                                        ipSFilter = null;
                                        ipFCursor = null;
                                    }
                                }
                                Marshal.ReleaseComObject(idenfyLayer);
                            }
                            return;
                        }
                        catch
                        {
                        }
                        return;
                    }
                    #endregion*/
                    #region 距离量算
                    if (identify == 1 && axMapControl1.MousePointer == esriControlsMousePointer.esriPointerCrosshair && e.button == 1)
                    {
                        IPolyline ployline = m_mapControl.TrackLine() as IPolyline;
                        MessageBox.Show("所测距离为：" + Convert.ToString(ployline.Length));
                    }
                    else if (blhuadian == true && axMapControl1.MousePointer == esriControlsMousePointer.esriPointerCrosshair && e.button == 1)
                    {
                        //􀑻􂫳􀏔􀏾􃅔􀤩􃃺􀧋
                        ISimpleMarkerSymbol pMarkerSymbol;
                        pMarkerSymbol = new SimpleMarkerSymbolClass();
                        //Symbol􂱘􁸋􁓣􀋈􀐎􀳚􁔶
                        pMarkerSymbol.Style = esriSimpleMarkerStyle.esriSMSCircle;
                        //Symbol􂱘􄹰􃡆
                        pMarkerSymbol.Color = getRGB(60, 100, 50);
                        //Symbol􂱘􁮟􄕀􃾦􁑺
                        pMarkerSymbol.Angle = 60;
                        //Symbol􂱘􀻻􁇣
                        pMarkerSymbol.Size = 6;
                        //Symbol􁰃􀧺􁳝􀻪􄕂􁒧􃒓
                        pMarkerSymbol.Outline = true;
                        //Symbol􀻪􄕂􁒧􃒓􂱘􀻻􁇣􀐎2
                        pMarkerSymbol.OutlineSize = 2;
                        pMarkerSymbol.OutlineColor = getRGB(166, 122, 166);
                        IPoint pPoint;
                        pPoint = new PointClass();
                        pPoint.PutCoords(e.mapX, e.mapY);
                        object oMarkerSymbol = pMarkerSymbol;
                        axMapControl1.DrawShape(pPoint, ref oMarkerSymbol);
                    }
                    #endregion
                    #region 面积量算
                    if (identifier == 2 && axMapControl1.MousePointer == esriControlsMousePointer.esriPointerCrosshair && e.button == 1)
                    {
                        IPolygon ipPlygn = m_mapControl.TrackPolygon() as IPolygon;
                        IArea ipArea = ipPlygn as IArea;
                        MessageBox.Show("所测面积为：" + Convert.ToString(System.Math.Abs(ipArea.Area)));
                    }
                    else if (blhuaxian == true && axMapControl1.MousePointer == esriControlsMousePointer.esriPointerCrosshair && e.button == 1)
                    {
                        //􁮄􁓎􀏔􀏾ISimpleLineSymbol􁇍􄈵
                        ISimpleLineSymbol pSimpleLineSymbol;
                        pSimpleLineSymbol = new SimpleLineSymbolClass();
                        pSimpleLineSymbol.Color = getRGB(100, 112, 103);
                        pSimpleLineSymbol.Style = esriSimpleLineStyle.esriSLSDot;
                        pSimpleLineSymbol.Width = 3;
                        IGeometry pGeo;
                        pGeo = axMapControl1.TrackLine();
                        object oLineSymbol = pSimpleLineSymbol;
                        axMapControl1.DrawShape(pGeo, ref oLineSymbol);
                    }
                    #endregion        
                }

             

             private void axMapControl1_OnMapReplaced(object sender, IMapControlEvents2_OnMapReplacedEvent e)
             {
                 AddLayerToOverViewMap();
                 CopyAndOverwriteMap();
             }

             private void axTOCControl1_OnMouseDown_1(object sender, ITOCControlEvents_OnMouseDownEvent e)
             {
                 try
                 {
                     if (e.button == 1)
                     {
                         esriTOCControlItem item = esriTOCControlItem.esriTOCControlItemNone;
                         IBasicMap map = null; ILayer layer = null;
                         object other = null; object index = null;
                         m_TOCControl.HitTest(e.x, e.y, ref item, ref map, ref layer, ref other, ref index);
                         if (item == esriTOCControlItem.esriTOCControlItemLayer)
                         {
                             if (layer is IAnnotationSublayer)
                             {
                                 return;
                             }
                             else
                             {
                                 pMoveLayer = layer;
                             }
                         }
                     }
                     if (e.button == 2)
                     {
                         esriTOCControlItem pItem = esriTOCControlItem.esriTOCControlItemNone;
                         IBasicMap pMap = null;
                         object pOther = new object();
                         object pIndex = new object();
                         axTOCControl1.HitTest(e.x, e.y, ref pItem, ref pMap, ref pLayer, ref pOther, ref pIndex);
                         if (pItem == esriTOCControlItem.esriTOCControlItemLayer)
                         {
                             System.Drawing.Point p = new System.Drawing.Point();
                             p.X = e.x;
                             p.Y = e.y;
                             contextMenuStrip1.Show(axTOCControl1, p);
                             contextMenuStrip1.Show(axTOCControl1, p);
                         }
                     }
                 }
                 catch
                 {
                 }
             }

             private void 面积测量_Click(object sender, EventArgs e)
             {
                 OrigronSet();
                 identifier = 2;
                 m_mapControl.MousePointer = esriControlsMousePointer.esriPointerCrosshair;
                 ScaleLabel.Text = "当前操作状态:面积测量";
             }

             private void 距离测量_Click(object sender, EventArgs e)
             {
                 OrigronSet();
                 identify = 1;
                 m_mapControl.MousePointer = esriControlsMousePointer.esriPointerCrosshair;
                 toolStripLabel2.Text = "当前操作状态:距离测量";
             }










        }
    }



