using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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

        #region app.settings：配置字段和相关变量

        /// <summary>
        /// 系统屏幕分辨率，宽
        /// </summary>
        private int scrWidth = (int)SystemParameters.PrimaryScreenWidth;
        /// <summary>
        /// 系统屏幕分辨率，高
        /// </summary>
        private int scrHeight = (int)SystemParameters.PrimaryScreenHeight;
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
        private int rebootSec = 0;
        /// <summary>
        /// 读取到的用户名 / 密码
        /// </summary>
        private string userName = null;

        #endregion


        #region 定义字符串常量

        /// <summary>
        /// 默认的初始路径，C盘根目录："C:\\）"
        /// </summary>
        const string DEFAULT_FILE_FOLDER_C = "C:\\）";
        /// <summary>
        /// "台站值班客户端"
        /// </summary>
        const string STATION_EXE_NAME = "台站值班客户端";
        /// <summary>
        /// "台站客户端"
        /// </summary>
        const string STATION_EXE_NAME_SHORT = "台站客户端";

        /// <summary>
        /// "MemEmpty"
        /// </summary>
        const string MEMEMPTY_NAME = "MemEmpty";
        //const string MemEmptyFullName = "MemEmpty.exe";
        /// <summary>
        /// "fm"
        /// </summary>
        const string STR_FM = "fm";
        /// <summary>
        /// "tv"
        /// </summary>
        const string STR_TV = "tv";
        /// <summary>
        /// "am"
        /// </summary>
        const string STR_AM = "am";
        /// <summary>
        /// "dl"
        /// </summary>
        const string STR_DL = "dl";

        /// <summary>
        /// 字符串："开始挂机"
        /// </summary>
        const string CONTENT_HANG_READY = "开始挂机";
        //const string ContentHanging = "正在挂机";
        /// <summary>
        /// 字符串："停止挂机"
        /// </summary>
        const string CONTENT_HANG_UP = "停止挂机";
        /// <summary>
        /// "true"
        /// </summary>
        const string STR_TRUE = "true";
        /// <summary>
        /// "false"
        /// </summary>
        const string STR_FALSE = "false";
        /// <summary>
        /// "fullFilePath"
        /// </summary>
        const string STR_FULL_FILE_PATH = "fullFilePath";
        /// <summary>
        /// "autoReatart"
        /// </summary>
        const string STR_AUTO_RESTART = "autoReatart";
        /// <summary>
        /// "silent"
        /// </summary>
        const string STR_SILENT = "silent";
        /// <summary>
        /// "sysShow"
        /// </summary>
        const string STR_SYS_SHOW = "sysShow";
        /// <summary>
        /// "stationVer"
        /// </summary>
        const string STR_STATION_VER = "stationVer";
        /// <summary>
        /// "version"
        /// </summary>
        const string STR_VERSION = "version";
        /// <summary>
        /// "releaseDate"
        /// </summary>
        const string STR_RELEASE_DATE = "releaseDate";
        /// <summary>
        /// "userName"
        /// </summary>
        const string STR_USERNAME = "userName";
        /// <summary>
        /// "passWord"
        /// </summary>
        const string STR_PASSWORD = "passWord";
        /// <summary>
        /// "rebootHour"
        /// </summary>
        const string STR_REBOOT_HOUR = "rebootHour";
        /// <summary>
        /// "rebootMin"
        /// </summary>
        const string STR_REBOOT_MIN = "rebootMin";
        /// <summary>
        /// "rebootSec"
        /// </summary>
        const string STR_REBOOT_SEC = "rebootSec";

        #endregion


        // 对 app.config 读写配置
        Configuration cfg = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        // 辅助工具类
        /// <summary>
        /// 进程相关动作，启动外部程序、结束进程、判断进程数量
        /// </summary>
        ProcessHelper ProcessHelper = new ProcessHelper();
        /// <summary>
        /// 模拟鼠标和键盘的单个动作；将字符串转换为对应的键盘键码
        /// </summary>
        InputHelper InputHelper = new InputHelper();
        /// <summary>
        /// 集合：客户端窗口内部各个控件的坐标
        /// </summary>
        CoordinateSettings CoordinateSettings = new CoordinateSettings();

        /// <summary>
        /// 定义两个定时器：系统时间定时器 和 用于挂机操作的定时器
        /// </summary>
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
            // 获取分辨率比例
            this.scrRatio = this.GetScreenRatio();

            // MessageBox.Show(DESKTOP.Width.ToString() + " " + DESKTOP.Height.ToString());

            // 开始挂机 按键 绿色
            this.BtnStart.Foreground = Brushes.Green;

            this.TxtUserName.IsReadOnly = false;

			// 定时器
			// 系统时间 cleanTimer
			if (cleanTimer == null) {
				cleanTimer = new System.Windows.Threading.DispatcherTimer();
                // TimeSpan.FromSeconds() 的CPU使用率达到25
                // new TimeSpan(0, 0, 0)  的CPU使用率达到25
                // new TimeSpan(0, 0, 1)  的CPU使用率最低，几乎为0

                // 间隔1秒，即每秒执行
                cleanTimer.Interval = new TimeSpan(0, 0, 1);	
				cleanTimer.Tick += new EventHandler(cleanTimer_Tick);
			}

            // 定时器
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
                this.detectProcess();
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
		void detectProcess() {
            // 检测正在运行的进程当中，是否有“台站客户端.exe”或“台站值班客户端.exe”
            // 若检测不到，则启动登陆客户端动作
            if (!this.ProcessHelper.ContainsProcess(STATION_EXE_NAME_SHORT, STATION_EXE_NAME))
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

        // 按键功能：选择文件：台站（值班）客户端.exe
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

            // 检测文件名。不再检测sha1值。       // 2020.10
            // 读取到的完整路径+客户端文件名字符串，是否包含“台站客户端”或“台站值班客户端”，
            // 并且该字符串以“客户端.exe”结尾
            if(this.ProcessHelper.ContainsString(fullFilePath ,STATION_EXE_NAME_SHORT, STATION_EXE_NAME)
                && fullFilePath.EndsWith("客户端.exe")
                ) {
                // 是否正在挂机
                switch (this.hangFlag) {
                    case 0: // 若不在挂机，则实现挂机功能
                        if (fullFilePath != string.Empty) {
                            // 如果有台站（值班）客户端正在运行，则结束进程
                            this.ProcessHelper.KillProcess(STATION_EXE_NAME_SHORT);
                            this.ProcessHelper.KillProcess(STATION_EXE_NAME);

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
            } else {
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
            // 不影响挂机状态，再进行启动/重启动作
            int tempHangFlag = this.hangFlag;
            this.hangFlag = 0;

            // 改为检测文件名。不再检测sha1值。       // 2020.10
            // 检测字符串 读取到的完整路径+客户端文件名：
            // 是否包含“台站客户端”或“台站值班客户端”，并且该字符串以“客户端.exe”结尾
            if (this.ProcessHelper.ContainsString(fullFilePath, STATION_EXE_NAME_SHORT, STATION_EXE_NAME)
                && fullFilePath.EndsWith("客户端.exe")
                ) {
                // 判断是否有进程名为“台站客户端”或“台站值班客户端”的进程正在运行
                bool processContains = this.ProcessHelper.ContainsProcess(STATION_EXE_NAME_SHORT, STATION_EXE_NAME);
                if (!processContains) {
                    // 没有检测到客户端进程，则启动客户端进程
                    if (fullFilePath != string.Empty) {
                        this.LogInActions(fullFilePath, silent, sysShow);
                    }
                    // 恢复挂机状态标志位
                    this.hangFlag = tempHangFlag;
                } else {
                    // 如果检测到台站（值班）客户端进程，则先结束进程后，再次启动进程，即重启客户端
                    if (fullFilePath != string.Empty) {
                        this.ProcessHelper.KillProcess(STATION_EXE_NAME_SHORT);
                        this.ProcessHelper.KillProcess(STATION_EXE_NAME);
                        this.ProcessHelper.KillProcess(MEMEMPTY_NAME);
                        this.LogInActions(fullFilePath, silent, sysShow);
                        // 恢复挂机状态标志位
                        this.hangFlag = tempHangFlag;
                    }
                }
            }
        }
                   
        // 按键功能：退出
		private void BtnQuit_Click(object sender, RoutedEventArgs e) {
            // 写入配置
            this.WriteAppConfig(
                this.TxtFilePath.Text, 
                autoReatart, 
                silent, 
                sysShow,
                this.TxtUserName.Text
                );
            // 保存配置
            this.cfg.Save();		
            // 垃圾回收
			GC.Collect();
			//GC.WaitForPendingFinalizers();
			//GC.SuppressFinalize(this);
			System.Environment.Exit(0);
		}

        #endregion

        /// <summary>
        /// 启动挂机动作
        /// </summary>
        private void HangActions() {
            // 禁止部分控件动作
            this.DenyOperation();
            // 登陆动作
            this.LogInActions(fullFilePath, silent, sysShow);
            // 挂机标志位置为1，表示正在挂机
            this.hangFlag = 1;
            // 第3个按键内容：显示为“停止挂机”
            this.BtnStart.Content = CONTENT_HANG_UP;
            // “停止挂机”按键的字样 变成红色
            this.BtnStart.Foreground = Brushes.Red;

            // 挂机Timer启动
            this.dTimer.Start();
            // 显示窗口底部的“正在挂机...”字样
            this.lblStats.Visibility = Visibility.Visible;
        }


        /// <summary>
        /// 登陆过程的鼠标和键盘动作
        /// 不影响挂机状态；不操作定时器
        /// </summary>
        /// <param name="fullFileName">文件路径+文件名</param>
        /// <param name="silent">是否静音信号源系统</param>
        /// <param name="sysShow">显示FM或DL系统</param>
        private void LogInActions(string fullFileName, string silent, string sysShow) {
            // 系统静音
            SethMute();

            // 获得屏幕分辨率
            scrRatio = this.GetScreenRatio();

            // 屏蔽鼠标键盘动作
            this.InputHelper.BlockKeyMouse(true);

            // 最小化本挂机软件窗口
			this.WindowState = System.Windows.WindowState.Minimized;
            // 最小化所有软件和程序的窗口
			this.InputHelper.MinimizeAllWindows();

            // 必要延时
            Delay(1000);

            // 启动客户端进程
            this.ProcessHelper.StartProcess(fullFileName);

            // 必要延时 > 1200	
			Delay(2500);

            // 键盘动作
            KeyBoardActions();
            // 必要延时 1300
			Delay(1300);
            // 鼠标动作
			MouseActions(scrWidth, scrHeight, silent, sysShow);

            // 解锁鼠标键盘
			this.InputHelper.BlockKeyMouse(false);

            // 必要延时，等待播报的“******远程网络监控系统”音频结束
			Delay(5000);
            // 解除系统静音
			SethMute();				
		}


        /// <summary>
        /// 禁止界面部分控件改变动作
        /// </summary>
		private void DenyOperation() {
            // 挂机期间，防止更改客户端文件路径
            this.BtnFileDialog.IsEnabled = false;
            this.TxtFilePath.IsEnabled = false;
            // 挂机期间，防止按错，导致关闭客户端	//	2018.02.20
            this.BtnStop.IsEnabled = false;
            // 挂机期间，防止修改用户名密码       //	2020.10.18
            this.TxtUserName.IsReadOnly = true;
            // 挂机期间，防止更改定时重启选项
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
            this.ProcessHelper.KillProcess(STATION_EXE_NAME_SHORT);
            this.ProcessHelper.KillProcess(STATION_EXE_NAME);
            this.ProcessHelper.KillProcess(MEMEMPTY_NAME);
            // 更改窗口控件属性
            this.BtnStart.Content = CONTENT_HANG_READY;
			this.BtnStart.Foreground = Brushes.Green;
			this.BtnStop.IsEnabled = true;		// 2018.02.20
			this.BtnFileDialog.IsEnabled = true;
			this.TxtFilePath.IsEnabled = true;
            this.TxtUserName.IsReadOnly = false;  //	2020.10.18
            this.chkAutoRestart.IsEnabled = true;
			this.lblStats.Visibility = Visibility.Hidden;
		}


        /// <summary>
        /// 判断屏幕分辨率的比例。未设置16:10
        /// </summary>
        private String GetScreenRatio() {
            // 屏幕分辨率 宽
            scrWidth = (int)DESKTOP.Width;
            // 屏幕分辨率 高
            scrHeight = (int)DESKTOP.Height;	

			switch (scrWidth) {
				case 1024:
                    return "4";
                case 1280:
					switch (scrHeight) {
						case 720:
                            return "16";
						case 960:
                            return "4";
						default:
                            return "4";
					}
                case 1600:
                    return "16";
				case 1920:
                    return "16";
                default:
                    return "4";
			}
		}

        #region 获取屏幕的真实分辨率
        // Win32 API
        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr ptr);
        [DllImport("gdi32.dll")]
        static extern int GetDeviceCaps(
        IntPtr hdc, // handle to DC
        int nIndex // index of capability
        );
        [DllImport("user32.dll", EntryPoint = "ReleaseDC")]
        static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDc);
        // DeviceCaps常量
        const int HORZRES = 8;
        const int VERTRES = 10;
        const int LOGPIXELSX = 88;
        const int LOGPIXELSY = 90;
        const int DESKTOPVERTRES = 117;
        const int DESKTOPHORZRES = 118;

        /// <summary>
        /// 获取屏幕的真实分辨率
        /// </summary>
        public static Size DESKTOP {
            get {
                IntPtr hdc = GetDC(IntPtr.Zero);
                Size size = new Size();
                size.Width = GetDeviceCaps(hdc, DESKTOPHORZRES);
                size.Height = GetDeviceCaps(hdc, DESKTOPVERTRES);
                ReleaseDC(IntPtr.Zero, hdc);
                return size;
            }
        }
        #endregion



        #region 键盘/鼠标动作

        /// <summary>
        /// 登陆时的键盘动作
        /// </summary>
        private void KeyBoardActions() 
        {
            // 最大化已启动的当前客户端软件窗口
            this.InputHelper.MaxmizeThisWindow();

            // 必要延时 1000
            Delay(1000);
            // 单击Tab，选中“进入”
            this.InputHelper.InputOneKey(KeyboardMouseAPI.InputHelper.vbKeyTab);
            // 必要延时 1000
            Delay(1000);
            // 单击Enter：回车“进入”            
			this.InputHelper.InputOneKey(KeyboardMouseAPI.InputHelper.vbKeyReturn);
            // 必要延时 1000,等待输入用户名密码的窗口的弹出
            Delay(1500);

            // 从文本框读取用户名和密码     // 2020.10
            userName = this.TxtUserName.Text;

            // 输入用户名：输入用户名字符串的键码集合
            for (int i = 0; i < userName.Length; i++)
            {
                this.InputHelper.InputOneKey((byte)this.InputHelper.GetKeyValues(userName)[i]);
            }

            // 单击Tab，切换到密码栏
            this.InputHelper.InputOneKey(KeyboardMouseAPI.InputHelper.vbKeyTab);

            // 输入密码：输入密码字符串的键码集合
            for (int i = 0; i < userName.Length; i++)
            {
                this.InputHelper.InputOneKey((byte)this.InputHelper.GetKeyValues(userName)[i]);
            }

            // 单击回车键，确认登陆
            this.InputHelper.InputOneKey(KeyboardMouseAPI.InputHelper.vbKeyReturn);	
		}


        /// <summary>
        /// 登陆后的鼠标动作
        /// </summary>
        /// <param name="width">屏幕宽</param>
        /// <param name="height">高</param>
        /// <param name="silent">是否静音信号源系统</param>
        /// <param name="sysShow">显示FM或DL系统</param>
        private void MouseActions(double width, double height, string silent, string sysShow) {
            GetScreenRatio();
            // 分辨率double转为int
            int scrWidth_int = (int)scrWidth;
            int scrHeight_int = (int)scrHeight;

            // 分辨率：4:3
            if (scrRatio == "4") {
                // 信号源系统是否静音
                if (silent == STR_TRUE) {
                    switch (scrWidth_int) {
                        case 1024:
                            this.InputHelper.ClickThreeKeys(
                                // 下方的信号源系统按钮
                                CoordinateSettings.ALM_XHY_1024,
                                // 禁止声音
                                CoordinateSettings.ALM_XHY_MUTE_1024,
                                // 确定
                                CoordinateSettings.ALM_XHY_YES_1024);
                            break;
                        case 1152:
                            this.InputHelper.ClickThreeKeys(
                                CoordinateSettings.ALM_XHY_1152,
                                CoordinateSettings.ALM_XHY_MUTE_1152,
                                CoordinateSettings.ALM_XHY_YES_1152);
                            break;
                        case 1280:
                            this.InputHelper.ClickThreeKeys(
                                CoordinateSettings.ALM_XHY_1280_43,
                                CoordinateSettings.ALM_XHY_MUTE_1280_43,
                                CoordinateSettings.ALM_XHY_YES_1280_43);
                            break;
                        default:
                            this.InputHelper.ClickThreeKeys(
                                CoordinateSettings.ALM_XHY_1024,
                                CoordinateSettings.ALM_XHY_MUTE_1024,
                                CoordinateSettings.ALM_XHY_YES_1024);
                            break;
                    }   // switch end
                }   // if silent end
            }   // if ratio end

            // 分辨率：16:9
            if (scrRatio == "16") {
                // 信号源系统是否静音
                if (silent == STR_TRUE) {
                    switch (scrWidth_int) {
                        case 1280:  
                            this.InputHelper.ClickThreeKeys(
                                // 下方的信号源系统按钮
                                CoordinateSettings.ALM_XHY_1280_169,
                                // 禁止声音
                                CoordinateSettings.ALM_XHY_MUTE_1280_169,
                                // 确定
                                CoordinateSettings.ALM_XHY_YES_1280_169);
                            break;
                        case 1600:
                            this.InputHelper.ClickThreeKeys(
                                CoordinateSettings.ALM_XHY_1600,
                                CoordinateSettings.ALM_XHY_MUTE_1600,
                                CoordinateSettings.ALM_XHY_YES_1600);
                            break;
                        case 1920:
                            this.InputHelper.ClickThreeKeys(
                                CoordinateSettings.ALM_XHY_1920,
                                CoordinateSettings.ALM_XHY_MUTE_1920,
                                CoordinateSettings.ALM_XHY_YES_1920);
                            break;
                        default:
                            this.InputHelper.ClickThreeKeys(
                                CoordinateSettings.ALM_XHY_1600,
                                CoordinateSettings.ALM_XHY_MUTE_1600,
                                CoordinateSettings.ALM_XHY_YES_1600);
                            break;
                    }   // switch end
                }   // if silent end
            }   // if ratio end

            // 选择显示系统
            switch (sysShow) {
                case STR_FM:
                    this.InputHelper.ClickOnceAt(CoordinateSettings.btnFM);
                    break;
                case STR_TV:
                    this.InputHelper.ClickOnceAt(CoordinateSettings.btnTV);
                    break;
                case STR_AM:
                    this.InputHelper.ClickOnceAt(CoordinateSettings.btnAM);
                    break;
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

		// 客户端文件路径
		private void TxtFilePath_TextChanged(object sender, TextChangedEventArgs e) {
            fullFilePath = this.TxtFilePath.Text;
		}

		// 显示 fm / tv / am
		private void rdbFM_Checked(object sender, RoutedEventArgs e) {
            sysShow = STR_FM;
		}
        private void rdbTV_Checked(object sender, RoutedEventArgs e) {
            sysShow = STR_TV;
        }
        private void rdbAM_Checked(object sender, RoutedEventArgs e) {
            sysShow = STR_AM;
		}

		// 单选框：关闭信号源系统声音
		private void chkSilent_Checked(object sender, RoutedEventArgs e) {
            silent = STR_TRUE;
		}
		private void chkSilent_Unchecked(object sender, RoutedEventArgs e) {
            silent = STR_FALSE;
		}

        // 单选框：定时自动重启
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
                // 是否自动重启
                autoReatart = ConfigurationManager.AppSettings[STR_AUTO_RESTART];
                // 是否静音信号源系统
                silent = ConfigurationManager.AppSettings[STR_SILENT];
                // 选择显示哪个系统
                sysShow = ConfigurationManager.AppSettings[STR_SYS_SHOW];

                // 用户名 / 密码
                userName = ConfigurationManager.AppSettings[STR_USERNAME];
                // 将 读取配置得到的用户名， 填入挂机软件界面的 用户名/密码文本框     // 2020.10
                this.TxtUserName.Text = userName;

                // 自动重启时间：时分秒
                rebootHour = Int16.Parse(ConfigurationManager.AppSettings[STR_REBOOT_HOUR]);
                rebootMin = Int16.Parse(ConfigurationManager.AppSettings[STR_REBOOT_MIN]);
                //rebootSec = Int16.Parse(ConfigurationManager.AppSettings[STR_REBOOT_SEC]);
                // 文字：x : x 重启客户端
                this.chkAutoRestart.Content = rebootHour + " : " + rebootMin + " 重启客户端";

                // 文字：窗口标题栏
                this.Title = "台站客户端挂机程序 V" + ConfigurationManager.AppSettings[STR_VERSION];
                // 文字：在窗口中显示版本号
                this.lblVer.Content = "[版本：" + ConfigurationManager.AppSettings[STR_STATION_VER] + " ]";
            }
            catch (ConfigurationErrorsException)
            {
                // 读取出错，则导入默认配置
                MessageBox.Show("读取配置文件错误");
                autoReatart = STR_TRUE;
                silent = STR_TRUE;
                sysShow = STR_FM;
                userName = null;
                rebootHour = 2;
                rebootMin = 30;
                rebootSec = 0;
                this.Title = "台站客户端挂机程序 V1.14";
                this.lblVer.Content = null;
                this.chkAutoRestart.Content = "每天02：30自动重启客户端";
            }

            // 调试用：弹出窗口显示用户名的键码
            //for (int i = 0; i < this.userName.Length; i++)
            //{
            //    MessageBox.Show(
            //        this.userName.ToString()[i]
            //        + ": "
            //        + this.InputHelper.GetKeyValues(this.userName)[i].ToString());
            //}


			//***界面上的控件状态，根据配置初始化***//
			// 是否自动重启
			if (autoReatart == STR_TRUE) 
                this.chkAutoRestart.IsChecked = true;
			else 
                this.chkAutoRestart.IsChecked = false;
			// 是否关闭信号源系统声音
			if (silent == STR_TRUE) 
                this.chkSilent.IsChecked = true;
			else 
                this.chkSilent.IsChecked = false;
            // 选择显示系统界面
            switch (sysShow) {
                case STR_FM:
                    this.rdbFM.IsChecked = true;
                    break;
                case STR_TV:
                    this.rdbTV.IsChecked = true;
                    break;
                case STR_AM:
                    this.rdbAM.IsChecked = true;
                    break;
                default:
                    this.rdbFM.IsChecked = true;
                    break;
            }
			//***界面上的控件状态根据配置初始化 END***//
		}

        /// <summary>
        /// 写入配置文件
        /// </summary>
        /// <param name="path">台站客户端路径</param>
        /// <param name="exeName">台站客户端文件名</param>
        /// <param name="auto">是否自动重启</param>
        /// <param name="silent">是否静音信号源系统</param>
        /// <param name="show">选择显示的系统</param>
        void WriteAppConfig(string path, string auto, string silent, string show, string userName) {
            try
            {
                cfg.AppSettings.Settings[STR_FULL_FILE_PATH].Value = path;
                cfg.AppSettings.Settings[STR_AUTO_RESTART].Value = auto;
                cfg.AppSettings.Settings[STR_SILENT].Value = silent;
			    cfg.AppSettings.Settings[STR_SYS_SHOW].Value = show;
                cfg.AppSettings.Settings[STR_USERNAME].Value = userName;
            } catch(ConfigurationErrorsException) {
                // 写入出错，则写入默认配置
                cfg.AppSettings.Settings[STR_FULL_FILE_PATH].Value = DEFAULT_FILE_FOLDER_C;
                cfg.AppSettings.Settings[STR_AUTO_RESTART].Value = STR_TRUE;
                cfg.AppSettings.Settings[STR_SILENT].Value = STR_TRUE;
                cfg.AppSettings.Settings[STR_SYS_SHOW].Value = STR_FM;
                cfg.AppSettings.Settings[STR_USERNAME].Value = "bs247t";
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
