using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Collections;
using System.Windows.Threading;
using System.Threading;
using System.Windows;
using System.Windows.Input;

/// <summary>
/// 模拟鼠标/键盘 
/// </summary>
namespace KeyboardMouseAPI
{

    public sealed class InputHelper
    {
        /// <summary>
        /// 模拟：输入1个按键
        /// </summary>
        /// <param name="keyValue">用按键值表示</param>
        public static void InputOneKey(byte keyValue)
        {
            keybd_event(keyValue, 0, 0, 0);        //按下
            keybd_event(keyValue, 0, 2, 0);        //释放
        }

        /// <summary>
        /// 模拟：同时按下多个按键
        /// </summary>
        /// <param name="keyValues"></param>
        public static void InputKeys(byte[] keyValues) {
            for(int i = 0; i < keyValues.Length; i++) {
                keybd_event(keyValues[i], 0, 0, 0);        //按下 按键 i
            }
            for (int i = 0; i < keyValues.Length; i++) {
                keybd_event(keyValues[i], 0, 2, 0);        //释放 按键 i
            }
        }

        /// <summary>
        /// 最小化当前窗口：ALT + Esc
        /// </summary>
        public static void MinimizeThisWindow() {
            // 快捷键是：ALT + Esc
            byte[] minimizeWindow = { vbKeyAlt, vbKeyEscape };
            InputKeys(minimizeWindow);
        }

        /// <summary>
        /// 最大化当前窗口：ALT + 空格 + X
        /// </summary>
        public static void MaxmizeThisWindow() {
            // 快捷键是：ALT + 空格 + X
            byte[] maxmizeWindow = { vbKeyAlt, vbKeySpace, vbKeyX };
            InputKeys(maxmizeWindow);
        } 

        /// <summary>
        /// 最小化所有窗口：WIN + M
        /// </summary>
        public static void MinimizeAllWindows() {
            //// 按下
            //InputHelper.keybd_event(vbKeyLWin, 0, 0, 0);       // Lwin;
            //InputHelper.keybd_event(vbKeyM, 0, 0, 0);		// M
            //// 释放
            //InputHelper.keybd_event(vbKeyM, 0, 2, 0);
            //InputHelper.keybd_event(vbKeyLWin, 0, 2, 0);

            byte[] minimizeAllWindows = { vbKeyRWin, vbKeyM };
            InputKeys(minimizeAllWindows);
        }

        /// <summary>
        /// 粘贴剪切板内容，到当前输入区域：Ctrl + V
        /// </summary>
        public static void CopyToClipboard() {
            // 快捷键：Ctrl + V
            byte[] copyToClipboard = { vbKeyControl, vbKeyV };
            InputKeys(copyToClipboard);
        }


        /// <summary>
        /// 单击坐标 坐标数组{ x, y }
        /// </summary>
        /// <param name="coord"></param>坐标数组
        public static void ClickOnceAt(int[] coord) {
            SetCursorPos(coord[0], coord[1]);
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }


        /// <summary>
        /// 按顺序，单击3个坐标
        /// </summary>
        /// <param name="coordA">坐标a</param>
        /// <param name="milliSec1">延时1</param>
        /// <param name="coordB">坐标b</param>
        /// <param name="milliSec2">延时2</param>
        /// <param name="coordC">坐标c</param>
        public static void ClickThreeCOOR(
            int[] coordA, int milliSec1, 
            int[] coordB, int milliSec2, 
            int[] coordC) {
            ClickOnceAt(coordA);
            Delay(milliSec1);
            ClickOnceAt(coordB);
            Delay(milliSec2);
            ClickOnceAt(coordC);
        }

        #region 延时函数：毫秒
        /// <summary>
        /// 延时子函数
        /// </summary>
        static void DoEvents() {
            DispatcherFrame frame = new DispatcherFrame(true);
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, (SendOrPostCallback)delegate (object arg) {
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


        /// <summary>
        /// 鼠标单击当前位置
        /// </summary>
        public static void ClickOnce()
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }
        /// <summary>
        /// 鼠标双击当前位置
        /// </summary>
        public static void ClickTwice()
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }

        /// <summary>
        /// 屏蔽鼠标和键盘的输入：true/屏蔽；false/解锁
        /// </summary>
        /// <param name="b">true/屏蔽 // false/解锁</param> 
        public static void BlockKeyMouse(bool b)
        {
            BlockInput(b);
        }

        /// <summary>
        /// 鼠标移动到坐标（x, y）
        /// </summary>
        /// <param name="x"></param> H
        /// <param name="y"></param> V
        public static void MouseMoveTo(int x, int y)
        {
            SetCursorPos(x, y);
        }

        #region 鼠标键盘的系统API、常量定义

        /// <summary>
        /// 模拟鼠标 //API：mouse_event
        /// </summary>
        /// <param name="dwFlags"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="cButtons"></param>
        /// <param name="dwExtraInfo"></param>
        [System.Runtime.InteropServices.DllImport("user32")]
        public static extern void mouse_event(
            int dwFlags,
            int dx,
            int dy,
            int cButtons,
            int dwExtraInfo
        );
        /// <summary>
        /// 鼠标移动到 x,y  //API：SetCursorPos
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        public static extern int SetCursorPos(int x, int y);

        /// <summary>
        /// 模拟键盘输入API：keybd_event
        /// </summary>
        /// <param name="bVk">虚拟键值</param>
        /// <param name="bScan">一般为0</param>
        /// <param name="dwFlags">整数类型：按下0，释放2</param>
        /// <param name="dwExtraInfo">整数类型：一般情况下设成为0</param>
        [DllImport("user32.dll", EntryPoint = "keybd_event")]
        public static extern void keybd_event(
            byte bVk,    //虚拟键值
            byte bScan,  // 一般为0
            int dwFlags,  //这里是整数类型  0 为按下，2为释放
            int dwExtraInfo  //这里是整数类型 一般情况下设成为 0
        );
        /// <summary>
        /// 屏蔽鼠标键盘：true屏蔽，false解锁
        /// </summary>
        /// <param name="Block">true屏蔽，false解锁</param>
        [DllImport("user32.dll")]
        public static extern void BlockInput(bool Block);

        #region 鼠标的按键常量
        public const int MOUSEEVENTF_MOVE = 0x0001;        // 移动鼠标 
        public const int MOUSEEVENTF_LEFTDOWN = 0x0002;    // 模拟鼠标左键按下 
        public const int MOUSEEVENTF_LEFTUP = 0x0004;      // 模拟鼠标左键抬起 
        public const int MOUSEEVENTF_RIGHTDOWN = 0x0008;   // 模拟鼠标右键按下 
        public const int MOUSEEVENTF_RIGHTUP = 0x0010;     // 模拟鼠标右键抬起 
        public const int MOUSEEVENTF_MIDDLEDOWN = 0x0020;  // 模拟鼠标中键按下 
        public const int MOUSEEVENTF_MIDDLEUP = 0x0040;    // 模拟鼠标中键抬起 
        public const int MOUSEEVENTF_ABSOLUTE = 0x8000;    // 标示是否采用绝对坐标
        #endregion


        #region 键码 bVk参数 常量定义
        /// <summary>
        /// 鼠标左键
        /// </summary>
        public const byte vbKeyLButton = 0x1;    // 鼠标左键
        /// <summary>
        /// 鼠标右键
        /// </summary>
        public const byte vbKeyRButton = 0x2;    // 鼠标右键
        /// <summary>
        /// CANCEL 键
        /// </summary>
        public const byte vbKeyCancel = 0x3;     // CANCEL 键
        /// <summary>
        /// 鼠标中键
        /// </summary>
        public const byte vbKeyMButton = 0x4;    // 鼠标中键
        /// <summary>
        /// BACKSPACE 键
        /// </summary>
        public const byte vbKeyBack = 0x8;       // BACKSPACE 键
        /// <summary>
        /// TAB 键
        /// </summary>
        public const byte vbKeyTab = 0x9;        // TAB 键
        public const byte vbKeyClear = 0xC;      // CLEAR 键
        /// <summary>
        /// ENTER 键
        /// </summary>
        public const byte vbKeyReturn = 0xD;     // ENTER 键
        public const byte vbKeyShift = 0x10;     // SHIFT 键
        public const byte vbKeyControl = 0x11;   // CTRL 键
        public const byte vbKeyAlt = 18;         // Alt 键  (键码18)
        public const byte vbKeyMenu = 0x12;      // MENU 键
        public const byte vbKeyPause = 0x13;     // PAUSE 键
        /// <summary>
        /// CAPS LOCK 键
        /// </summary>
        public const byte vbKeyCapital = 0x14;   // CAPS LOCK 键
        public const byte vbKeyEscape = 0x1B;    // ESC 键
        public const byte vbKeySpace = 0x20;     // SPACE 键
        public const byte vbKeyPageUp = 0x21;    // PAGE UP 键
        public const byte vbKeyEnd = 0x23;       // End 键
        public const byte vbKeyHome = 0x24;      // HOME 键
        public const byte vbKeyLeft = 0x25;      // LEFT ARROW 键
        public const byte vbKeyUp = 0x26;        // UP ARROW 键
        public const byte vbKeyRight = 0x27;     // RIGHT ARROW 键
        public const byte vbKeyDown = 0x28;      // DOWN ARROW 键
        public const byte vbKeySelect = 0x29;    // Select 键
        public const byte vbKeyPrint = 0x2A;     // PRINT SCREEN 键
        public const byte vbKeyExecute = 0x2B;   // EXECUTE 键
        public const byte vbKeySnapshot = 0x2C;  // SNAPSHOT 键
        public const byte vbKeyDelete = 0x2E;    // Delete 键
        public const byte vbKeyHelp = 0x2F;      // HELP 键
        public const byte vbKeyNumlock = 0x90;   // NUM LOCK 键

        /// <summary>
        /// 左边的 WIN 键
        /// </summary>
        public const byte vbKeyLWin = 0x5b;     // 左边的 WIN 键
        /// <summary>
        /// 右边的 WIN 键
        /// </summary>
        public const byte vbKeyRWin = 0x5C;     // 右边的 WIN 键
        /// <summary>
        /// 右Ctrl左边键，点击相当于点击鼠标右键，会弹出快捷菜单
        /// </summary>
        public const byte vbKeyApps = 0x93;     // 右Ctrl左边键，点击相当于点击鼠标右键，会弹出快捷菜单
        /// <summary>
        /// 分号 ; 
        /// </summary>
        public const byte vbKeySemicolon = 0xBA;    // ;(分号)
        /// <summary>
        /// 等号/加号 键
        /// </summary>
        public const byte vbKeyEqual = 0xBB;        // 等号/加号键

        // 常用键：字母键 A-Z
        public const byte vbKeyA = 65;
        public const byte vbKeyB = 66;
        public const byte vbKeyC = 67;
        public const byte vbKeyD = 68;
        public const byte vbKeyE = 69;
        public const byte vbKeyF = 70;
        public const byte vbKeyG = 71;
        public const byte vbKeyH = 72;
        public const byte vbKeyI = 73;
        public const byte vbKeyJ = 74;
        public const byte vbKeyK = 75;
        public const byte vbKeyL = 76;
        public const byte vbKeyM = 77;
        public const byte vbKeyN = 78;
        public const byte vbKeyO = 79;
        public const byte vbKeyP = 80;
        public const byte vbKeyQ = 81;
        public const byte vbKeyR = 82;
        public const byte vbKeyS = 83;
        public const byte vbKeyT = 84;
        public const byte vbKeyU = 85;
        public const byte vbKeyV = 86;
        public const byte vbKeyW = 87;
        public const byte vbKeyX = 88;
        public const byte vbKeyY = 89;
        public const byte vbKeyZ = 90;

        // 数字键盘：0 - 9
        public const byte vbKey0 = 48;    // 0 键
        public const byte vbKey1 = 49;    // 1 键
        public const byte vbKey2 = 50;    // 2 键
        public const byte vbKey3 = 51;    // 3 键
        public const byte vbKey4 = 52;    // 4 键
        public const byte vbKey5 = 53;    // 5 键
        public const byte vbKey6 = 54;    // 6 键
        public const byte vbKey7 = 55;    // 7 键
        public const byte vbKey8 = 56;    // 8 键
        public const byte vbKey9 = 57;    // 9 键

        // 小键盘
        public const byte vbKeyNumpad0 = 0x60;    //0 键
        public const byte vbKeyNumpad1 = 0x61;    //1 键
        public const byte vbKeyNumpad2 = 0x62;    //2 键
        public const byte vbKeyNumpad3 = 0x63;    //3 键
        public const byte vbKeyNumpad4 = 0x64;    //4 键
        public const byte vbKeyNumpad5 = 0x65;    //5 键
        public const byte vbKeyNumpad6 = 0x66;    //6 键
        public const byte vbKeyNumpad7 = 0x67;    //7 键
        public const byte vbKeyNumpad8 = 0x68;    //8 键
        public const byte vbKeyNumpad9 = 0x69;    //9 键
        /// <summary>
        /// 小键盘，MULTIPLICATIONSIGN(*)键
        /// </summary>
        public const byte vbKeyMultiply = 0x6A;   // MULTIPLICATIONSIGN(*)键
        /// <summary>
        /// 小键盘，PLUS SIGN(+) 键
        /// </summary>
        public const byte vbKeyAdd = 0x6B;        // PLUS SIGN(+) 键
        /// <summary>
        /// 小键盘，ENTER 键
        /// </summary>
        public const byte vbKeySeparator = 0x6C;  // ENTER 键
        /// <summary>
        /// 小键盘，MINUS SIGN(-) 键
        /// </summary>
        public const byte vbKeySubtract = 0x6D;   // MINUS SIGN(-) 键
        /// <summary>
        /// 小键盘，DECIMAL POINT(.) 键
        /// </summary>
        public const byte vbKeyDecimal = 0x6E;    // DECIMAL POINT(.) 键
        /// <summary>
        /// 小键盘，DIVISION SIGN(/) 键
        /// </summary>
        public const byte vbKeyDivide = 0x6F;     // DIVISION SIGN(/) 键
        /// <summary>
        /// ,键(逗号)
        /// </summary>
        public const byte vbKeyComma = 0xBC;      // ,键(逗号)
        /// <summary>
        /// -键(减号)
        /// </summary>
        public const byte vbKeyMinus = 0xBD;      // -键(减号)
        /// <summary>
        /// .键(句号)
        /// </summary>
        public const byte vbKeyPeriod = 0xBE;   // .键(句号)
        /// <summary>
        /// 斜杠/和问号？ 键
        /// </summary>
        public const byte vbKeySlash = 0xBF;    // 斜杠/和问号？ 键
        /// <summary>
        /// `键(Esc下面)
        /// </summary>
        public const byte vbKeyTilde = 0xC0;    // `键(Esc下面)
        /// <summary>
        /// [ 键
        /// </summary>
        public const byte vbKeyLeftBracket = 0xDB;  // [ 键
        /// <summary>
        /// ] 键
        /// </summary>
        public const byte vbKeyRightBracket = 0xDD;  // ] 键
        /// <summary>
        /// 反斜杠\ 键
        /// </summary>
        public const byte vbKeyBackSlash = 0xDC;    // 反斜杠\键
        /// <summary>
        /// 单/双引号 ' " 键
        /// </summary>
        public const byte vbKeyQuote = 0xDE;        // 单/双引号 ' " 键


        //F1到F12按键
        public const byte vbKeyF1 = 0x70;   //F1 键
        public const byte vbKeyF2 = 0x71;   //F2 键
        public const byte vbKeyF3 = 0x72;   //F3 键
        public const byte vbKeyF4 = 0x73;   //F4 键
        public const byte vbKeyF5 = 0x74;   //F5 键
        public const byte vbKeyF6 = 0x75;   //F6 键
        public const byte vbKeyF7 = 0x76;   //F7 键
        public const byte vbKeyF8 = 0x77;   //F8 键
        public const byte vbKeyF9 = 0x78;   //F9 键
        public const byte vbKeyF10 = 0x79;  //F10 键
        public const byte vbKeyF11 = 0x7A;  //F11 键
        public const byte vbKeyF12 = 0x7B;  //F12 键

        #endregion

        #endregion

        #region 转换字符串

        /// <summary>
        /// 枚举类型，键盘按键的键码值
        /// </summary>
        public enum KeyValueEnum : byte
        {
            //常用键：字母键A到Z
            vbKeyA = 65,
            vbKeyB = 66,
            vbKeyC = 67,
            vbKeyD = 68,
            vbKeyE = 69,
            vbKeyF = 70,
            vbKeyG = 71,
            vbKeyH = 72,
            vbKeyI = 73,
            vbKeyJ = 74,
            vbKeyK = 75,
            vbKeyL = 76,
            vbKeyM = 77,
            vbKeyN = 78,
            vbKeyO = 79,
            vbKeyP = 80,
            vbKeyQ = 81,
            vbKeyR = 82,
            vbKeyS = 83,
            vbKeyT = 84,
            vbKeyU = 85,
            vbKeyV = 86,
            vbKeyW = 87,
            vbKeyX = 88,
            vbKeyY = 89,
            vbKeyZ = 90,

            // 数字键盘：0 - 9
            vbKey0 = 48,    // 0 键
            vbKey1 = 49,    // 1 键
            vbKey2 = 50,    // 2 键
            vbKey3 = 51,    // 3 键
            vbKey4 = 52,    // 4 键
            vbKey5 = 53,    // 5 键
            vbKey6 = 54,    // 6 键
            vbKey7 = 55,    // 7 键
            vbKey8 = 56,    // 8 键
            vbKey9 = 57,    // 9 键

            // 小键盘按键
            vbKeyNumpad0 = 0x60,    // 0 键
            vbKeyNumpad1 = 0x61,    // 1 键
            vbKeyNumpad2 = 0x62,    // 2 键
            vbKeyNumpad3 = 0x63,    // 3 键
            vbKeyNumpad4 = 0x64,    // 4 键
            vbKeyNumpad5 = 0x65,    // 5 键
            vbKeyNumpad6 = 0x66,    // 6 键
            vbKeyNumpad7 = 0x67,    // 7 键
            vbKeyNumpad8 = 0x68,    // 8 键
            vbKeyNumpad9 = 0x69,    // 9 键
            vbKeyMultiply = 0x6A,   // MULTIPLICATIONSIGN(*)键
            vbKeyAdd = 0x6B,        // PLUS SIGN(+) 键
            vbKeySeparator = 0x6C,  // ENTER 键
            vbKeySubtract = 0x6D,   // MINUS SIGN(-) 键
            vbKeyDecimal = 0x6E,    // DECIMAL POINT(.) 键
            vbKeyDivide = 0x6F,     // DIVISION SIGN(/) 键


            //F1到F12按键
            vbKeyF1 = 0x70,   //F1 键
            vbKeyF2 = 0x71,   //F2 键
            vbKeyF3 = 0x72,   //F3 键
            vbKeyF4 = 0x73,   //F4 键
            vbKeyF5 = 0x74,   //F5 键
            vbKeyF6 = 0x75,   //F6 键
            vbKeyF7 = 0x76,   //F7 键
            vbKeyF8 = 0x77,   //F8 键
            vbKeyF9 = 0x78,   //F9 键
            vbKeyF10 = 0x79,  //F10 键
            vbKeyF11 = 0x7A,  //F11 键
            vbKeyF12 = 0x7B,  //F12 键

            // 其他常用按键
            vbKeyLButton = 0x1,    // 鼠标左键
            vbKeyRButton = 0x2,    // 鼠标右键
            vbKeyCancel = 0x3,     // CANCEL 键
            vbKeyMButton = 0x4,    // 鼠标中键
            vbKeyBack = 0x8,       // BACKSPACE 键
            vbKeyTab = 0x9,        // TAB 键
            vbKeyClear = 0xC,      // CLEAR 键
            vbKeyReturn = 0xD,     // ENTER 键
            vbKeyShift = 0x10,     // SHIFT 键
            vbKeyControl = 0x11,   // CTRL 键
            vbKeyAlt = 18,         // Alt 键  (键码18)
            vbKeyMenu = 0x12,      // MENU 键
            vbKeyPause = 0x13,     // PAUSE 键
            vbKeyCapital = 0x14,   // CAPS LOCK 键
            vbKeyEscape = 0x1B,    // ESC 键
            vbKeySpace = 0x20,     // SPACEBAR 键
            vbKeyPageUp = 0x21,    // PAGE UP 键
            vbKeyEnd = 0x23,       // End 键
            vbKeyHome = 0x24,      // HOME 键
            vbKeyLeft = 0x25,      // LEFT ARROW 键
            vbKeyUp = 0x26,        // UP ARROW 键
            vbKeyRight = 0x27,     // RIGHT ARROW 键
            vbKeyDown = 0x28,      // DOWN ARROW 键
            vbKeySelect = 0x29,    // Select 键
            vbKeyPrint = 0x2A,     // PRINT SCREEN 键
            vbKeyExecute = 0x2B,   // EXECUTE 键
            vbKeySnapshot = 0x2C,  // SNAPSHOT 键
            vbKeyDelete = 0x2E,    // Delete 键
            vbKeyHelp = 0x2F,      // HELP 键
            vbKeyNumlock = 0x90   // NUM LOCK 键
        }


        /// <summary>
        /// 字符串转换为键码值的集合
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>返回键码值的集合</returns>
        public ArrayList GetKeyValues(string str)
        {
            ArrayList kvList = new ArrayList();
            for (int i = 0; i < str.Length; i++)
            {
                kvList.Add((byte)Enum.Parse(
                    typeof(KeyValueEnum), string.Concat("vbKey", str[i].ToString().ToUpper())));
            }
            return kvList;
        }

        /// <summary>
        /// 根据单个字符，获取枚举KeyValueEnum里的对应键码值
        /// </summary>
        /// <param name="c">单个字符</param>
        /// <returns></returns>
        public int GetKeyValuesOfChar(char c)
        {
            if (IsNumber(c.ToString()))
            {
                return (int)Enum.Parse(typeof(KeyValueEnum), string.Concat("vbKey", c.ToString()));
            }
            else
                return (int)Enum.Parse(typeof(KeyValueEnum), c.ToString());
        }



        /// <summary>
        /// 判断字符串是否是数字
        /// </summary>
        /// <param name="s">需要判断的字符串</param>
        /// <returns>判断结果</returns>
        public bool IsNumber(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return false;
            const string pattern = "^[0-9]*$";
            Regex rx = new Regex(pattern);
            return rx.IsMatch(str);
        }

        /// <summary>
        /// 判断字符串中是否包含中文
        /// </summary>
        /// <param name="str">需要判断的字符串</param>
        /// <returns>判断结果</returns>
        public bool HasChinese(string str)
        {
            return Regex.IsMatch(str, @"[\u4e00-\u9fa5]");
        }

        #endregion



        #region 输入法相关

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        static extern uint GetWindowThreadProcessId(IntPtr hwnd, IntPtr proccess);
        [DllImport("user32.dll")]
        static extern IntPtr GetKeyboardLayout(uint thread);
        /// <summary>
        /// 获取 WPF 输入法的语言区域
        /// </summary>
        public static int GetCurrentKeyboardLayout() {
            IntPtr foregroundWindow = GetForegroundWindow();
            uint foregroundProcess = GetWindowThreadProcessId(foregroundWindow, IntPtr.Zero);
            int keyboardLayout = GetKeyboardLayout(foregroundProcess).ToInt32() & 0xFFFF;
            return keyboardLayout; 
        }
        /// <summary>
        /// 切换为英文输入法
        /// win10：无效 2021.05
        /// </summary>
        public void SwtichKeyboardLayoutToEnglish(Window window) {
            byte[] switchKeyboardLayou = { vbKeyAlt, vbKeyShift };
            if (GetCurrentKeyboardLayout() != 1033) {
                InputKeys(switchKeyboardLayou);
            }

            System.Windows.Input.InputMethod.SetPreferredImeState(window, InputMethodState.Off);

        }

        #endregion

    }

}