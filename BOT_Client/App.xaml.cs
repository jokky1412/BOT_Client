using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Runtime.InteropServices;

namespace BOT_Client {
	/// <summary>
	/// App.xaml 的交互逻辑
	/// </summary>
	public partial class App : Application {

		// 利用 System.Threading.Mutex  来实现控制程序的单例运行。
		System.Threading.Mutex mutex;

		public App() {
			this.Startup += new StartupEventHandler(App_Startup);
		}

		void App_Startup(object sender, StartupEventArgs e) {
			bool ret;
			mutex = new System.Threading.Mutex(true, "BOT_Client", out ret);		// 项目名称在中间

			// 注意mutex不能被回收，否则就无法发挥作用，如下被回收
			//using (System.Threading.Mutex mutex = new System.Threading.Mutex(true, "Singleton", out ret)) { }

			if (!ret) {
				//MessageBox.Show("already runing");
				Environment.Exit(0);
			}
		}



		[DllImport("kernel32.dll", EntryPoint = "SetProcessWorkingSetSize")]
		public static extern int SetProcessWorkingSetSize(IntPtr process, int minSize, int maxSize);
		/// <summary>
		/// 释放内存
		/// </summary>
		public static void ClearMemory() {
			GC.Collect();
			GC.WaitForPendingFinalizers();
			if (Environment.OSVersion.Platform == PlatformID.Win32NT) {
				App.SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);
			}
		}

	}

}
