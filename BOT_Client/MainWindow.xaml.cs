using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;		// md5
using System.Runtime.InteropServices;
using System.Windows.Threading;
using System.Threading;
using System.Configuration;

using KeyboardMouseAPI;


namespace BOT_Client {
	/// <summary>
	/// MainWindow.xaml 的交互逻辑
	/// </summary>
	public partial class MainWindow : Window {

        #region app.settings 的配置字段和相关变量

        /// <summary>
        /// 系统屏幕分辨率，宽
        /// </summary>
        private double scrWidth = SystemParameters.PrimaryScreenWidth;
        /// <summary>
        /// 系统屏幕分辨率，高
        /// </summary>
        private double scrHeight = SystemParameters.PrimaryScreenHeight;
        /// <summary>
        /// 分辨率比例 16或4
        /// </summary>
        private string scrRatio = null;
        /// <summary>
        /// 是否关闭信号源系统声音： true / false
        /// </summary>
        private string silent = null;
        /// <summary>
        /// 显示系统：FM / DL
        /// </summary>
        private string sysShow = null;
        /// <summary>
        /// 自动重启客户端： true / false
        /// </summary>
        private string autoReatart = null;
        /// <summary>
        /// 客户端程序完整路径 + 文件名
        /// </summary>
        private string fullFilePath = null;
        /// <summary>
        /// 客户端文件名
        /// </summary>
        private string stationExeName = null;
        /// <summary>
        /// 客户端文件sha1值
        /// </summary>
        private string sha1Value = null;

        /// <summary>
        /// 读取到的自动重启的时间：时/分/秒
        /// </summary>
        private int rebootHour;
        /// <summary>
        /// 自动重启的时间：分
        /// </summary>
        private int rebootMin;
        /// <summary>
        /// 自动重启的时间：秒
        /// </summary>
        private int rebootSec;
        /// <summary>
        /// 读取到的用户名
        /// </summary>
        private string userName = null;
        /// <summary>
        /// 读取到的密码
        /// </summary>
        private string passWord = null;

        #endregion


        #region 台站值班客户端.exe 2019 文件校验码sha1
        /// <summary>
        /// stationVer.2019.04.18 台站值班客户端.exe
        /// </summary>
        private static string sha1Code_20190418 = "4efe40eb8edb8ea6851e5e1bafdeea536702acfe";
        /// <summary>
        /// stationVer.2019.07.10 台站客户端.exe
        /// </summary>
        private static string sha1Code_20190710 = "54150a3aca2a1bada21171e3d7f8ad3623181d15";
        #endregion

        #region 客户端的各个按钮在屏幕上的坐标 (x, y)
        // 系统：调频、电视、电力
        /// <summary>
        /// 调频系统：60, 130
        /// </summary>
        private static int[] btnFM = { 60, 130 };
        /// <summary>
        /// 电视系统：60, 210
        /// </summary>
        private static int[] btnTV = { 60, 210 };
        /// <summary>
        /// 电力系统：60, 360
        /// </summary>
        private static int[] btnDL = { 60, 360 };

        // 按钮：信号源系统静音
        /// <summary>
        /// 下方信号源系统：220, 710
        /// </summary>
        private static int[] btnXHY = { 220, 710 };
        /// <summary>
        /// 禁止声音：220, 640
        /// </summary>
        private static int[] btnMUTE = { 220, 640 };
        /// <summary>
        /// 确定：340, 670
        /// </summary>
        private static int[] btnYES = { 340, 670 };
        #endregion


        #region 定义字符串常量

        /// <summary>
        /// 默认的初始路径，C盘根目录："C:\\）"
        /// </summary>
        private static string DEFAULT_FILE_FOLDER_C = "C:\\）";
        /// <summary>
        /// "台站值班客户端"
        /// </summary>
        private static string STATION_EXE_NAME = "台站值班客户端";
        //string ExeFullName = "台站值班客户端.exe";

        /// <summary>
        /// "MemEmpty"
        /// </summary>
        private static string MEMEMPTY_NAME = "MemEmpty";
        //string MemEmptyFullName = "MemEmpty.exe";
        /// <summary>
        /// "fm"
        /// </summary>
        private static string STR_FM = "fm";
        /// <summary>
        /// "tv"
        /// </summary>
        private static string STR_TV = "tv";
        /// <summary>
        /// "dl"
        /// </summary>
        private static string STR_DL = "dl";

        /// <summary>
        /// 字符串："开始挂机"
        /// </summary>
        private static string CONTENT_HANG_READY = "开始挂机";
        //string ContentHanging = "正在挂机";
        /// <summary>
        /// 字符串："停止挂机"
        /// </summary>
        private static string CONTENT_HANG_UP = "停止挂机";
        /// <summary>
        /// "true"
        /// </summary>
        private static string STR_TRUE = "true";
        /// <summary>
        /// "false"
        /// </summary>
        private static string STR_FALSE = "false";
        /// <summary>
        /// "fullFilePath"
        /// </summary>
        private static string STR_FULL_FILE_PATH = "fullFilePath";
        /// <summary>
        /// "stationExeName"
        /// </summary>
        private static string STR_STATION_EXE_NAME = "stationExeName";
        /// <summary>
        /// "autoReatart"
        /// </summary>
        private static string STR_AUTO_RESTART = "autoReatart";
        /// <summary>
        /// "silent"
        /// </summary>
        private static string STR_SILENT = "silent";
        /// <summary>
        /// "sysShow"
        /// </summary>
        private static string STR_SYS_SHOW = "sysShow";
        /// <summary>
        /// "stationVer"
        /// </summary>
        private static string STR_STATION_VER = "stationVer";
        /// <summary>
        /// "sha1"
        /// </summary>
        private static string STR_SHA1 = "sha1";
        /// <summary>
        /// "version"
        /// </summary>
        private static string STR_VERSION = "version";
        /// <summary>
        /// "releaseDate"
        /// </summary>
        private static string STR_RELEASE_DATE = "releaseDate";
        /// <summary>
        /// "userName"
        /// </summary>
        private static string STR_USERNAME = "userName";
        /// <summary>
        /// "passWord"
        /// </summary>
        private static string STR_PASSWORD = "passWord";
        /// <summary>
        /// "rebootHour"
        /// </summary>
        private static string STR_REBOOT_HOUR = "rebootHour";
        /// <summary>
        /// "rebootMin"
        /// </summary>
        private static string STR_REBOOT_MIN = "rebootMin";
        /// <summary>
        /// "rebootSec"
        /// </summary>
        private static string STR_REBOOT_SEC = "rebootSec";

        #endregion


        // 对 app.config 读写配置
        Configuration cfg = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        // 辅助工具类
        ProcessHelper ProcessHelper = new ProcessHelper();
        InputHelper InputHelper = new InputHelper();
        HashHelper HashHelper = new HashHelper();


        // 定义两个定时器：系统时间定时器 和 挂机操作用的定时器
        System.Windows.Threading.DispatcherTimer cleanTimer, dTimer;

        /// <summary>
        /// 挂机标志位，在挂机1，非挂机0
        /// </summary>
        private int hangFlag;
        /// <summary>
        /// 挂机时间计数子
        /// </summary>
        private int CNT60 = 1;


        /// <summary>
        /// 主程序窗口
        /// </summary>
		public MainWindow() {
			InitializeComponent();
			//********* 初始化 //*********//
			// 挂机标志位清零
			this.hangFlag = 0;

			// 读取配置
			this.ReadAppConfig();

            // 开始挂机 按键 绿色
			this.BtnStart.Foreground = Brushes.Green;

			// 定时器任务
			// 系统时间 cleanTimer
			if (cleanTimer == null) {
				cleanTimer = new System.Windows.Threading.DispatcherTimer();
				// TimeSpan.FromSeconds() 的CPU使用率达到25
				// new TimeSpan(0, 0, 0)  的CPU使用率达到25
				// new TimeSpan(0, 0, 1)  的CPU使用率最低，几乎为0

				//cleanTimer.Interval = TimeSpan.FromSeconds(0);		// 间隔1秒，即每秒执行
				cleanTimer.Interval = new TimeSpan(0, 0, 1);	
				cleanTimer.Tick += new EventHandler(cleanTimer_Tick);
			}
			
			// 挂机 dTimer
			if (dTimer == null) {
				dTimer = new System.Windows.Threading.DispatcherTimer();
				dTimer.Interval = new TimeSpan(0, 0, 1);			// 间隔1秒，即每秒执行
				dTimer.Tick += new EventHandler(dTimer_Tick);
			}
			
		}


		//  定时重启时间设置
		void dTimer_Tick(object sender, EventArgs e) {
            // 每分钟检测客户端是否运行，若没有则运行
			this.CNT60 += 1;
			if (this.CNT60 == 60) {
				this.CNT60 = 0;
                this.detectProcess(this.stationExeName);
			}

            // 一般从配置文件读取时间设置。否则默认重启 at 02:30:00。
			if (DateTime.Now.Hour == rebootHour 
                && DateTime.Now.Minute == rebootMin 
                && DateTime.Now.Second == rebootSec)
                // 触发重启客户端按键动作
				this.BtnRestart_Click(sender, new RoutedEventArgs());		
		}

        /// <summary>
        /// 检测客户端是否正在运行，若没有则运行
        /// </summary>
        /// <param name="exeName">程序名</param>
		void detectProcess(string exeName) {
			int processCnt = 0;
            processCnt = this.ProcessHelper.ProcessesCount(exeName);
			if (processCnt == 0) 
                this.LogInActions(fullFilePath, silent, sysShow);
		}

		/// <summary>
        /// 清理内存 时间计数子（秒）
        /// </summary>
		int clean;
        /// <summary>
        /// 显示系统时间 / 定时清理内存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		void cleanTimer_Tick(object sender, EventArgs e) {
            // 显示系统时间
			this.LblTimeNow.Content = DateTime.Now.ToString("HH:mm:ss");	
			//this.lbllNow.Content = this.hangFlag.ToString();
			// 清理内存
			this.clean += 1;
            // 每3秒清理一次内存
			if (this.clean == 3) {
				ClearMemory();
				this.clean = 0;
			}
		}

		#region  窗口内的按键功能

        // 按键功能：选择文件：台站值班客户端.exe
		private void BtnFileDialog_Click(object sender, RoutedEventArgs e) {
			var openFileDialog = new Microsoft.Win32.OpenFileDialog() {
				Filter = "Exe Files (*.exe)|*.exe"
			};
            // 弹框选择文件，显示初始路径为C盘根目录
            openFileDialog.InitialDirectory = DEFAULT_FILE_FOLDER_C;	
			var result = openFileDialog.ShowDialog();
			if (result == true) {
				this.TxtFilePath.Text = openFileDialog.FileName;
			}
		}

        // 按键功能：开始挂机 / 停止挂机
		private void BtnStart_Click(object sender, RoutedEventArgs e) {
            // 从对话框读取文件完整路径+文件名
            fullFilePath = this.TxtFilePath.Text;
            // 读取所选择的文件的sha1值
            sha1Value = this.HashHelper.ComputeSHA1(fullFilePath);
            // 字符串转为小写
            sha1Value = sha1Value.ToLower();
      
            // 对照app.setting里的sha1值，判断是否为台站值班客户端.exe
            if (sha1Value == ConfigurationManager.AppSettings[STR_SHA1])
            {
                // 是否正在挂机
                switch (this.hangFlag)
                {
                    case 0: // 若不在挂机，则实现挂机功能
                        if (this.TxtFilePath.Text != string.Empty)
                        {
                            // 如果有台站值班客户端正在运行，则结束进程
                            this.ProcessHelper.KillProcess(stationExeName);
                            this.ProcessHelper.KillProcess(MEMEMPTY_NAME);
                            // 开始挂机动作
                            this.HangActions();
                        }
                        break;
                    case 1: // 若正在挂机，则停止挂机功能
                        this.StopHangAction();
                        break;
                    default: // 默认，停止挂机功能
                        this.StopHangAction();
                        break;
                }
            }
            else
            {
                ;
            }

		}

        // 按键功能：关闭客户端
		private void BtnStop_Click(object sender, RoutedEventArgs e) {
			this.StopHangAction();
		}

		// 按键功能：启动 / 重启客户端
		private void BtnRestart_Click(object sender, RoutedEventArgs e) {
            fullFilePath = this.TxtFilePath.Text;
            // 不影响挂机状态，再动作
            int tempHangFlag = this.hangFlag;
			this.hangFlag = 0;

            sha1Value = this.HashHelper.ComputeSHA1(fullFilePath);
            // 字符串转为小写
            sha1Value = sha1Value.ToLower();

            // 比较 从配置读取到的sha1值 和 获取软件界面选定的exe文件sha1值
            // 判断是否为客户端.exe			// 2018.02.20 添加
            if (sha1Value == ConfigurationManager.AppSettings[STR_SHA1]) {
                switch (this.ProcessHelper.ProcessesCount(stationExeName)) {
					case 0: // 没有检测到台站客户端进程，则打开客户端
                        if (this.TxtFilePath.Text != string.Empty) {
							this.LogInActions(fullFilePath, silent, sysShow);
						}
                        // 挂机标志置为0
						this.hangFlag = tempHangFlag;
						break;
                    case 1:  // 如果检测到台站客户端进程，则先结束再打开，即为重启客户端
                        if (this.TxtFilePath.Text != string.Empty) {
                            this.ProcessHelper.KillProcess(stationExeName);
                            this.ProcessHelper.KillProcess(MEMEMPTY_NAME);
							this.LogInActions(fullFilePath, silent, sysShow);
							this.hangFlag = tempHangFlag;
						}
						break;
					default:  // 默认不在挂机状态
                        this.hangFlag = tempHangFlag;
						break;
				}
			}
            else {
                // 文件比对结果不匹配，则挂机软件不进行动作
            }
        }

        // 按键功能：退出
		private void BtnQuit_Click(object sender, RoutedEventArgs e) {
			this.WriteAppConfig(
                fullFilePath, stationExeName, 
                autoReatart, 
                silent, 
                sysShow
                );
            // 写入配置，保存
			this.cfg.Save();		
            // 垃圾回收
			GC.Collect();
			//GC.WaitForPendingFinalizers();
			//GC.SuppressFinalize(this);
			System.Environment.Exit(0);
		}

        #endregion

        #region 屏幕分辨率
        /// <summary>
        /// 分辨率：1024, 768
        /// </summary>
        private static int[] screen43 = { 1024, 768 };
        /// <summary>
        /// 分辨率：1600, 900
        /// </summary>
        private static int[] screen169 = { 1600, 900 };
        #endregion


        /// <summary>
        /// 登陆过程的鼠标和键盘动作
        /// 不影响挂机状态；不操作定时器
        /// </summary>
        /// <param name="fullFileName">文件路径+文件名</param>
        /// <param name="silent">是否静音信号源系统</param>
        /// <param name="sysShow">显示FM或DL系统</param>
        private void LogInActions(string fullFileName, string silent, string sysShow) {
            // 屏蔽鼠标键盘
            this.InputHelper.BlockKeyMouse(true);
            // 最小化挂机软件窗口
			this.WindowState = System.Windows.WindowState.Minimized;

            // 最小化所有窗口，相当于键盘同时按下：WIN + M
			this.MinAllWindows();
            // 启动客户端进程
            this.ProcessHelper.StartProcess(fullFileName);

            // 系统静音
			SethMute();
            // 必要延时 > 1200	
			Delay(2500);

            // 获得屏幕分辨率
			this.GetScreenRatio();
            // 根据分辨率，双击客户端右上角的最大化窗口
			switch (scrRatio) {	
				case "4": this.InputHelper.ClickOnceAt(979, 31, screen43[0], screen43[1]);
							this.InputHelper.ClickOnceAt(979, 31, screen43[0], screen43[1]);
					break;
				case "16": this.InputHelper.ClickOnceAt(1234, 31, screen169[0], screen169[1]);
							this.InputHelper.ClickOnceAt(1234, 31, screen169[0], screen169[1]);
					break;
				default:
					break;
			}

            // 键盘动作
			KeyBoardActions();
            // 必要延时 1000
			Delay(1300);
            // 鼠标动作
			MouseActions(scrWidth, scrHeight, silent, sysShow);

            // 解锁鼠标键盘
			this.InputHelper.BlockKeyMouse(false);
            // 必要延时，静音“******远程网络监控系统”
			Delay(5000);
            // 解除系统静音
			SethMute();				
		}

		// 启动挂机动作
		private void HangActions() {
            // 禁止部分控件动作
			this.DenyOperation();
            // 登陆动作
			this.LogInActions(fullFilePath, silent, sysShow);
            // 挂机标志位置为1，表示正在挂机
			this.hangFlag = 1;
            // 按键内容显示为“停止挂机”
			this.BtnStart.Content = CONTENT_HANG_UP; 
            // “停止挂机”按键 变成红色
            this.BtnStart.Foreground = Brushes.Red;
            // 挂机期间，防止按错，导致关闭客户端	//	2018.02.20添加
			this.BtnStop.IsEnabled = false;
            // 挂机Timer启动
			this.dTimer.Start();
            // 窗口底部显示“正在挂机...”字样
			this.lblStats.Visibility = Visibility.Visible;	
		}

        /// <summary>
        /// 禁止界面部分控件改变动作
        /// </summary>
		private void DenyOperation() {
			// 
			this.BtnFileDialog.IsEnabled = false;
			this.TxtFilePath.IsEnabled = false;
			this.chkAutoRestart.IsEnabled = false;
		}

		/// <summary>
        /// 停止挂机动作
		/// </summary>
		private void StopHangAction() {
            // 必须停止挂机计时器dTimer
			dTimer.Stop();
            // 挂机标志位置为0
			this.hangFlag = 0;
            // 结束相关进程
            this.ProcessHelper.KillProcess(stationExeName);
            this.ProcessHelper.KillProcess(MEMEMPTY_NAME);
            // 更改窗口控件属性
			this.BtnStart.Content = CONTENT_HANG_READY;
			this.BtnStart.Foreground = Brushes.Green;
			this.BtnStop.IsEnabled = true;		// 2018.02.20添加
			this.BtnFileDialog.IsEnabled = true;
			this.TxtFilePath.IsEnabled = true;
			this.chkAutoRestart.IsEnabled = true;
			this.lblStats.Visibility = Visibility.Hidden;
		}


		/// <summary>
        /// 桌面分辨率判断，目前没有16:10
		/// </summary>
		private void GetScreenRatio() {
            // 屏幕分辨率 宽
            scrWidth = SystemParameters.PrimaryScreenWidth;
            // 屏幕分辨率 高
            scrHeight = SystemParameters.PrimaryScreenHeight;	
			switch (Convert.ToInt16(scrWidth)) {
				case 1024:
                    scrRatio = "4";
					break;
				case 1280:
					switch (Convert.ToInt16(scrHeight)) {
						case 720:
                            scrRatio = "16";
							break;
						case 960:
                            scrRatio = "4";
							break;
						default:
                            scrRatio = "4";
							break;
					}
					break;
				case 1600: if (scrHeight == 900) scrRatio = "16";
					break;
				case 1920:
                    scrRatio = "16";
					break;
				default:
                    scrRatio = "4";
					break;
			}
		}


		#region 键盘/鼠标动作

		/// <summary>
        /// 登陆时的键盘动作
		/// </summary>
		private void KeyBoardActions() 
        {
            // 必要延时
			Delay(1000);
            // Tab，选中“进入”
            this.InputHelper.InputOneKey(KeyboardMouseAPI.InputHelper.vbKeyTab);
            // 必要延时
            Delay(1000);
            // Enter：回车“进入”            
			this.InputHelper.InputOneKey(KeyboardMouseAPI.InputHelper.vbKeyReturn);
            // 必要延时,等待输入用户名密码的窗口的弹出
            Delay(1500);

            // 输入用户名：输入用户名字符串的键码集合
            for (int i = 0; i < userName.Length; i++)
            {
                this.InputHelper.InputOneKey((byte)this.InputHelper.GetKeyValues(userName)[i]);
            }

            // Tab，切换到密码栏
            this.InputHelper.InputOneKey(KeyboardMouseAPI.InputHelper.vbKeyTab);

            // 输入密码：输入密码字符串的键码集合
            for (int i = 0; i < passWord.Length; i++)
            {
                this.InputHelper.InputOneKey((byte)this.InputHelper.GetKeyValues(passWord)[i]);
            }

            // 回车键，确认登陆
            this.InputHelper.InputOneKey(KeyboardMouseAPI.InputHelper.vbKeyReturn);	
		}

		/// <summary>
        /// 最小化所有窗口，相当于键盘同时按下：WIN + M
		/// </summary>
		private void MinAllWindows() {
            // 按下
			InputHelper.keybd_event(KeyboardMouseAPI.InputHelper.vbKeyLWin, 0, 0, 0);		// Lwin;
			InputHelper.keybd_event(KeyboardMouseAPI.InputHelper.vbKeyM, 0, 0, 0);		// M
            // 释放
            InputHelper.keybd_event(KeyboardMouseAPI.InputHelper.vbKeyM, 0, 2, 0);		
            InputHelper.keybd_event(KeyboardMouseAPI.InputHelper.vbKeyLWin, 0, 2, 0);	
	        // 必要延时
			Delay(1000);		
		}


		/// <summary>
        /// 登陆后的鼠标动作
		/// </summary>
		/// <param name="width">屏幕宽</param>
		/// <param name="height">高</param>
		/// <param name="silent">是否静音信号源系统</param>
		/// <param name="sysShow">显示FM或DL系统</param>
		private void MouseActions(double width, double height, string silent, string sysShow) {
            // 分辨率：4：3
			if (scrRatio == "4") {
                // 信号源系统是否静音
				if (silent == STR_TRUE) {
                    // 下方的信号源系统按钮
					this.InputHelper.ClickOnceAt(btnXHY[0], btnXHY[1], screen43[0], screen43[1]);	
                    // 禁止声音	
					this.InputHelper.ClickOnceAt(btnMUTE[0], btnMUTE[1], screen43[0], screen43[1]);		
                    // 确定
					this.InputHelper.ClickOnceAt(btnYES[0], btnYES[1], screen43[0], screen43[1]);	
				}
                // 是否显示FM系统
				if (sysShow == STR_FM)
					this.InputHelper.ClickOnceAt(btnFM[0], btnFM[1], screen43[0], screen43[1]);
				else
					this.InputHelper.ClickOnceAt(btnDL[0], btnDL[1], screen43[0], screen43[1]);	
			}
            // 分辨率：16：9
			if (scrRatio == "16") {
				if (silent == STR_TRUE) {
					this.InputHelper.ClickOnceAt(220, 660, screen169[0], screen169[1]);
					this.InputHelper.ClickOnceAt(210, 590, screen169[0], screen169[1]);
					this.InputHelper.ClickOnceAt(340, 625, screen169[0], screen169[1]);
				}
				if (sysShow == STR_FM) {
					this.InputHelper.ClickOnceAt(60, 140, screen169[0], screen169[1]);
				} else
					this.InputHelper.ClickOnceAt(60, 210, screen169[0], screen169[1]);
			}
		}

		#endregion


        /// <summary>
        /// 系统静音/取消系统静音
        /// </summary>
        public static void SethMute()
        {
            NAudio.CoreAudioApi.MMDeviceEnumerator MMDE = new NAudio.CoreAudioApi.MMDeviceEnumerator();
            //Get all the devices, no matter what condition or status
            NAudio.CoreAudioApi.MMDevice dev = MMDE.GetDefaultAudioEndpoint(NAudio.CoreAudioApi.DataFlow.Render, NAudio.CoreAudioApi.Role.Communications);

            dev.AudioEndpointVolume.Mute = !dev.AudioEndpointVolume.Mute;
        }


		#region 延时函数：毫秒

        /// <summary>
        /// 延时子函数
        /// </summary>
		static void DoEvents() {
			DispatcherFrame frame = new DispatcherFrame(true);
			Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, (SendOrPostCallback)delegate(object arg) {
				DispatcherFrame fr = arg as DispatcherFrame;
				fr.Continue = false;
			}, frame);
			Dispatcher.PushFrame(frame);
		}

		/// <summary>
        /// 延时函数
		/// </summary>
        /// <param name="milliSecond">1000 = 1秒</param>
		public static void Delay(int milliSecond) {
			int start = Environment.TickCount;
			while (Math.Abs(Environment.TickCount - start) < milliSecond) {
				DoEvents();
			}
		}

		#endregion 


		#region 根据控件的动作，改变设置项的值
		// 加载cleanTimer，定时清理程序内存
		private void LblTimeNow_Loaded(object sender, RoutedEventArgs e) {
			this.cleanTimer.Start();
		}

		// 文件路径
		private void TxtFilePath_TextChanged(object sender, TextChangedEventArgs e) {
            fullFilePath = this.TxtFilePath.Text;
		}

		// 显示 fm / dl
		private void rdbFM_Checked(object sender, RoutedEventArgs e) {
            sysShow = STR_FM;
		}
		private void rdbDL_Checked(object sender, RoutedEventArgs e) {
            sysShow = STR_DL;
		}

		// 单选框：关闭信号源系统声音
		private void chkSilent_Checked(object sender, RoutedEventArgs e) {
            silent = STR_TRUE;
		}
		private void chkSilent_Unchecked(object sender, RoutedEventArgs e) {
            silent = STR_FALSE;
		}

        // 单选框：自动重启
		private void chkAutoRestart_Checked(object sender, RoutedEventArgs e) {
            autoReatart = STR_TRUE;
		}
		private void chkAutoRestart_Unchecked(object sender, RoutedEventArgs e) {
            autoReatart = STR_FALSE;
		}
		#endregion 


		#region 读取/写入 文件中的配置

		/// <summary>
		/// 读取配置文件
		/// </summary>
		void ReadAppConfig() {
            try
            {
                // 客户端完整路径+文件名
                this.TxtFilePath.Text = ConfigurationManager.AppSettings[STR_FULL_FILE_PATH];
                // 客户端文件名
                stationExeName = ConfigurationManager.AppSettings[STR_STATION_EXE_NAME];
                // 是否自动重启
                autoReatart = ConfigurationManager.AppSettings[STR_AUTO_RESTART];
                // 是否静音信号源系统
                silent = ConfigurationManager.AppSettings[STR_SILENT];
                // 选择显示哪个系统
                sysShow = ConfigurationManager.AppSettings[STR_SYS_SHOW];

                // 文件sha1值
                sha1Value = ConfigurationManager.AppSettings[STR_SHA1];
                // 用户名和密码
                userName = ConfigurationManager.AppSettings[STR_USERNAME];
                passWord = ConfigurationManager.AppSettings[STR_PASSWORD];
                // 自动重启时间：时分秒
                rebootHour = Int16.Parse(ConfigurationManager.AppSettings[STR_REBOOT_HOUR]);
                rebootMin = Int16.Parse(ConfigurationManager.AppSettings[STR_REBOOT_MIN]);
                rebootSec = Int16.Parse(ConfigurationManager.AppSettings[STR_REBOOT_SEC]);
                // 每天 x：xx：x 重启客户端
                this.chkAutoRestart.Content = "每天"
                    + rebootHour + "：" + rebootMin + "：" + rebootSec + "重启客户端";
                // 窗口标题栏
                this.Title = "台站客户端挂机程序 V" + ConfigurationManager.AppSettings[STR_VERSION];
                // 在窗口中显示版本号
                this.lblVer.Content = "[版本：" + ConfigurationManager.AppSettings[STR_STATION_VER] + " ]";
            }
            catch (ConfigurationErrorsException)
            {
                MessageBox.Show("读取配置文件错误");
                stationExeName = STATION_EXE_NAME;
                autoReatart = STR_TRUE;
                silent = STR_TRUE;
                sysShow = STR_FM;
                sha1Value = sha1Code_20190710;
                userName = null;
                passWord = null;
                rebootHour = 2;
                rebootMin = 30;
                rebootSec = 0;
                this.Title = "台站客户端挂机程序 V1.13";
                this.lblVer.Content = null;
                this.chkAutoRestart.Content = "每天02：30自动重启客户端";
            }

            //for (int i = 0; i < this.userName.Length; i++)
            //{
            //    MessageBox.Show(
            //        this.userName.ToString()[i]
            //        + ": "
            //        + this.InputHelper.GetKeyValues(this.userName)[i].ToString());
            //}


			//***界面上的控件状态根据配置初始化***//
			// 自动重启
			if (autoReatart == STR_TRUE) 
                this.chkAutoRestart.IsChecked = true;
			else 
                this.chkAutoRestart.IsChecked = false;
			// 关闭信号源系统声音
			if (silent == STR_TRUE) 
                this.chkSilent.IsChecked = true;
			else 
                this.chkSilent.IsChecked = false;
			// 显示FM或DL系统界面
			if (sysShow == STR_FM) 
                this.rdbFM.IsChecked = true;
			else 
                this.rdbDL.IsChecked = true;
			//***界面上的控件状态根据配置初始化 END***//
		}

        /// <summary>
        /// 写入配置文件
        /// </summary>
        /// <param name="path">台站客户端路径</param>
        /// <param name="exeName">台站客户端文件名</param>
        /// <param name="auto">是否自动重启</param>
        /// <param name="silent">是否静音信号源系统</param>
        /// <param name="show">显示FM或DL系统</param>
        void WriteAppConfig(string path, string exeName, string auto, string silent, string show) {
            try
            {
                cfg.AppSettings.Settings[STR_FULL_FILE_PATH].Value = path;
                cfg.AppSettings.Settings[STR_STATION_EXE_NAME].Value = exeName;
                cfg.AppSettings.Settings[STR_AUTO_RESTART].Value = auto;
                cfg.AppSettings.Settings[STR_SILENT].Value = silent;
			    cfg.AppSettings.Settings[STR_SYS_SHOW].Value = show;
            } catch(ConfigurationErrorsException) {
                cfg.AppSettings.Settings[STR_FULL_FILE_PATH].Value = DEFAULT_FILE_FOLDER_C;
                cfg.AppSettings.Settings[STR_STATION_EXE_NAME].Value = STATION_EXE_NAME;
                cfg.AppSettings.Settings[STR_AUTO_RESTART].Value = STR_TRUE;
                cfg.AppSettings.Settings[STR_SILENT].Value = STR_TRUE;
                cfg.AppSettings.Settings[STR_SYS_SHOW].Value = STR_FM;
            }

		}

		#endregion


        #region 内存垃圾回收，释放内存

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

		#endregion

	}


}
