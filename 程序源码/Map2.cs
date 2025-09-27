using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace 工师校园地貌
{
    [System.Runtime.InteropServices.ComVisible(true)]
    public partial class Map2 : Form
    {
        public Map2()
        {
            InitializeComponent();
            SetWebBrowserFeatures(11);
            webBrowser2.ScriptErrorsSuppressed = true;
            // webBrowser2.Navigate("https://uri.amap.com/marker?position=125.32031,43.939395");
            /* string str_url2 = Application.StartupPath + "\\map.html"; //地图的路径+名称
             Uri url2 = new Uri("http://snowywar.top/1.html");
             webBrowser2.Url = url2;
             webBrowser2.ObjectForScripting = this;*/

            string str_url2 = Application.StartupPath + "\\省内方位.html"; //地图的路径+名称
            Uri url2 = new Uri(str_url2);
            webBrowser2.Url = url2;
            webBrowser2.ObjectForScripting = this;


            string str_url = Application.StartupPath + "\\导航.html"; //地图的路径+名称
            Uri url = new Uri(str_url);
            webBrowser1.Url = url;
            webBrowser1.ObjectForScripting = this;
     
        }

        /// <summary>  
        /// 修改注册表信息来兼容当前程序  
        ///   
        /// </summary>  
        static void SetWebBrowserFeatures(int ieVersion)
        {
            // don't change the registry if running in-proc inside Visual Studio  
            if (LicenseManager.UsageMode != LicenseUsageMode.Runtime)
                return;
            //获取程序及名称  
            var appName = System.IO.Path.GetFileName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            //得到浏览器的模式的值  
            UInt32 ieMode = GeoEmulationModee(ieVersion);
            var featureControlRegKey = @"HKEY_CURRENT_USER\Software\Microsoft\Internet Explorer\Main\FeatureControl\";
            //设置浏览器对应用程序（appName）以什么模式（ieMode）运行  
            Registry.SetValue(featureControlRegKey + "FEATURE_BROWSER_EMULATION",
                appName, ieMode, RegistryValueKind.DWord);
            // enable the features which are "On" for the full Internet Explorer browser  
            //不晓得设置有什么用  
            Registry.SetValue(featureControlRegKey + "FEATURE_ENABLE_CLIPCHILDREN_OPTIMIZATION",
                appName, 1, RegistryValueKind.DWord);


            //Registry.SetValue(featureControlRegKey + "FEATURE_AJAX_CONNECTIONEVENTS",  
            //    appName, 1, RegistryValueKind.DWord);  


            //Registry.SetValue(featureControlRegKey + "FEATURE_GPU_RENDERING",  
            //    appName, 1, RegistryValueKind.DWord);  


            //Registry.SetValue(featureControlRegKey + "FEATURE_WEBOC_DOCUMENT_ZOOM",  
            //    appName, 1, RegistryValueKind.DWord);  


            //Registry.SetValue(featureControlRegKey + "FEATURE_NINPUT_LEGACYMODE",  
            //    appName, 0, RegistryValueKind.DWord);  
        }
        /// <summary>  
        /// 获取浏览器的版本  
        /// </summary>  
        /// <returns></returns>  
        static int GetBrowserVersion()
        {
            int browserVersion = 0;
            using (var ieKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Internet Explorer",
                RegistryKeyPermissionCheck.ReadSubTree,
                System.Security.AccessControl.RegistryRights.QueryValues))
            {
                var version = ieKey.GetValue("svcVersion");
                if (null == version)
                {
                    version = ieKey.GetValue("Version");
                    if (null == version)
                        throw new ApplicationException("Microsoft Internet Explorer is required!");
                }
                int.TryParse(version.ToString().Split('.')[0], out browserVersion);
            }
            //如果小于7  
            if (browserVersion < 7)
            {
                throw new ApplicationException("不支持的浏览器版本!");
            }
            return browserVersion;
        }
        /// <summary>  
        /// 通过版本得到浏览器模式的值  
        /// </summary>  
        /// <param name="browserVersion"></param>  
        /// <returns></returns>  
        static UInt32 GeoEmulationModee(int browserVersion)
        {
            UInt32 mode = 11000; // Internet Explorer 11. Webpages containing standards-based !DOCTYPE directives are displayed in IE11 Standards mode.   
            switch (browserVersion)
            {
                case 7:
                    mode = 7000; // Webpages containing standards-based !DOCTYPE directives are displayed in IE7 Standards mode.   
                    break;
                case 8:
                    mode = 8000; // Webpages containing standards-based !DOCTYPE directives are displayed in IE8 mode.   
                    break;
                case 9:
                    mode = 9000; // Internet Explorer 9. Webpages containing standards-based !DOCTYPE directives are displayed in IE9 mode.                      
                    break;
                case 10:
                    mode = 10000; // Internet Explorer 10.  
                    break;
                case 11:
                    mode = 11000; // Internet Explorer 11  
                    break;
            }
            return mode;
        }

    }

}
