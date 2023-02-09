
using BOT_Client;
using System;
using System.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KeyboardMouseAPI;


namespace UnitTestProject1 {
    [TestClass]
    public class InputHelperTest {

        /// <summary>
        /// 测试类：ScreenHelper
        /// </summary>
        [TestMethod]
        public void TestScreenHelper() {

            var screenHelper = new ScreenHelper();

            float x = ScreenHelper.ScaleX;
            float y = ScreenHelper.ScaleY;
            Console.WriteLine("缩放百分比:");
            Console.WriteLine("ScaleX = " + x);
            Console.WriteLine("ScaleY = " + y);
            Console.WriteLine();
            
            int dw = (int)ScreenHelper.DESKTOP.Width;
            int dh = (int)ScreenHelper.DESKTOP.Height;
            Console.WriteLine("真实设置的桌面分辨率大小:");
            Console.WriteLine("DESKTOP.Width: " + dw);
            Console.WriteLine("DESKTOP.Height: " + dh);
            Console.WriteLine();

            int ww = (int)ScreenHelper.WorkingArea.Width;
            int wh = (int)ScreenHelper.WorkingArea.Height;
            Console.WriteLine("屏幕分辨率当前物理大小:");
            Console.WriteLine("WorkingArea.Width: " + ww);
            Console.WriteLine("WorkingArea.Height: " + wh);

        }


    }



}
