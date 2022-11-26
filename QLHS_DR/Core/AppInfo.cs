using System;
using System.IO;
using System.Windows.Forms;

namespace QLHS_DR.Core
{
    internal static class AppInfo
    {
        public static string FolderPath;

        //[CompilerGenerated]
        //private static SvgImageCollection m_b;

        //[SpecialName]
        //[CompilerGenerated]
        //public static SvgImageCollection c()
        //{
        //	return h.m_b;
        //}

        //[SpecialName]
        //[CompilerGenerated]
        //private static void a(SvgImageCollection A_0)
        //{
        //	h.m_b = A_0;
        //}

        static AppInfo()
        {
            AppInfo.FolderPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}{Path.DirectorySeparatorChar}EEMCDR{Path.DirectorySeparatorChar}{AppDomain.CurrentDomain.FriendlyName}.config";
            if (!SystemInformation.TerminalServerSession && Screen.AllScreens.Length > 1)
            {
                //WindowsFormsSettings.SetPerMonitorDpiAware();
            }
            else
            {
                //WindowsFormsSettings.SetDPIAware();
            }
            //WindowsFormsSettings.EnableFormSkins();
            //WindowsFormsSettings.ForceDirectXPaint();
            //WindowsFormsSettings.ScrollUIMode = ScrollUIMode.Fluent;
            //WindowsFormsSettings.FontBehavior = WindowsFormsFontBehavior.ForceSegoeUI;
            //WindowsFormsSettings.DefaultFont = new Font("SegoeUI", 10f);
            //WindowsFormsSettings.DefaultLookAndFeel.SetSkinStyle(SkinStyle.Bezier, SkinSvgPalette.Bezier.VSLight);
            //a(SvgImageCollection.FromResources("EEMC_DR.Resources.Svg", typeof(h).Assembly));
            //b();
            //t.b("WinChatClientApp");
        }

        //private static void b()
        //{
        //	j.e();
        //}

        //[STAThread]
        //private static void a()
        //{
        //	string value = ((GuidAttribute)typeof(AppInfo).Assembly.GetCustomAttributes(typeof(GuidAttribute), inherit: true)[0]).Value;
        //	Mutex mutex = new Mutex(initiallyOwned: false, value);
        //	try
        //	{
        //		if (mutex.WaitOne(0, exitContext: false))
        //		{
        //			Application.EnableVisualStyles();
        //			Application.SetCompatibleTextRenderingDefault(defaultValue: false);
        //			a obj = new a();
        //			obj.IconOptions.SvgImage = c()["AppIcon"];
        //			Application.Run(obj);
        //		}
        //		else
        //		{
        //			MessageBox.Show("Một phiên bản của phần mềm đang chạy.", "EEMC DR");
        //		}
        //	}
        //	finally
        //	{
        //		if (mutex != null)
        //		{
        //			mutex.ReleaseMutex();
        //			mutex.Close();
        //		}
        //	}
    }
}
