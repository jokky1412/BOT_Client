using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BOT_Client {
    /// <summary>
    /// 集合：客户端窗口内部各个控件的坐标
    /// </summary>
    class CoordinateSettings {

        #region 左侧的显示系统按钮：调频、电视、中波、电力、环境
        // 在所有分辨率下都相同
        /// <summary>
        /// 左侧按钮 信号源系统：60, 70
        /// </summary>
        public static int[] btnXHY = { 60, 70 };
        /// <summary>
        /// 左侧按钮 调频系统：60, 140
        /// </summary>
        public static int[] btnFM = { 60, 140 };
        /// <summary>
        /// 左侧按钮 电视系统：60, 210
        /// </summary>
        public static int[] btnTV = { 60, 210 };
        /// <summary>
        /// 左侧按钮 中波系统：60, 280
        /// </summary>
        public static int[] btnAM = { 60, 280 };
        /// <summary>
        /// 左侧按钮 电力系统：60, 350
        /// </summary>
        public static int[] btnDL = { 60, 350 };
        /// <summary>
        /// 左侧按钮 环境系统：60, 420
        /// </summary>
        public static int[] btnENV = { 60, 420 };
        #endregion

        #region 分辨率4:3：下方信号源系统按钮和禁止报警
        /// <summary>
        /// 下方按钮 信号源系统 4:3      1024/760
        /// </summary>
        public static int[] ALM_XHY_1024 = { 220, 710 };
        /// <summary>
        /// 下方按钮 信号源系统 禁止报警 4:3     1024/760
        /// </summary>
        public static int[] ALM_XHY_MUTE_1024 = { 220, 640 };
        /// <summary>
        /// 下方按钮 信号源系统 允许报警 4:3     1024/760
        /// </summary>
        public static int[] ALM_XHY_UNMUTE_1024 = { 280, 640 };
        /// <summary>
        /// 下方按钮 信号源系统 确定 4:3       1024/760
        /// </summary>
        public static int[] ALM_XHY_YES_1024 = { 340, 670 };

        /// <summary>
        /// 下方按钮 信号源系统 4:3      1152/864
        /// </summary>
        public static int[] ALM_XHY_1152 = { 220, 790 };
        /// <summary>
        /// 下方按钮 信号源系统 禁止报警 4:3     1152/864
        /// </summary>
        public static int[] ALM_XHY_MUTE_1152 = { 220, 725 };
        /// <summary>
        /// 下方按钮 信号源系统 允许报警 4:3     1152/864
        /// </summary>
        public static int[] ALM_XHY_UNMUTE_1152 = { 280, 725 };
        /// <summary>
        /// 下方按钮 信号源系统 确定 4:3       1152/864
        /// </summary>
        public static int[] ALM_XHY_YES_1152 = { 340, 760 };

        /// <summary>
        /// 下方按钮 信号源系统 4:3      1280/960
        /// </summary>
        public static int[] ALM_XHY_1280_43 = { 220, 900 };
        /// <summary>
        /// 下方按钮 信号源系统 禁止报警 4:3     1280/960
        /// </summary>
        public static int[] ALM_XHY_MUTE_1280_43 = { 220, 830 };
        /// <summary>
        /// 下方按钮 信号源系统 允许报警 4:3     1280/960
        /// </summary>
        public static int[] ALM_XHY_UNMUTE_1280_43 = { 280, 830 };
        /// <summary>
        /// 下方按钮 信号源系统 确定 4:3       1280/960
        /// </summary>
        public static int[] ALM_XHY_YES_1280_43 = { 340, 860 };
        #endregion

        #region 分辨率16:9：下方信号源系统按钮和禁止报警
        /// <summary>
        /// 下方按钮 信号源系统 16:9     1280/720
        /// </summary>
        public static int[] ALM_XHY_1280_169 = { 220, 660 };
        /// <summary>
        /// 下方按钮 信号源系统 禁止报警 16:9        1280/720
        /// </summary>
        public static int[] ALM_XHY_MUTE_1280_169 = { 220, 590 };
        /// <summary>
        /// 下方按钮 信号源系统 允许报警 16:9        1280/720
        /// </summary>
        public static int[] ALM_XHY_UNMUTE_1280_169 = { 280, 580 };
        /// <summary>
        /// 下方按钮 信号源系统 确定 16:9      1280/720
        /// </summary>
        public static int[] ALM_XHY_YES_1280_169 = { 340, 620 };

        /// <summary>
        /// 下方按钮 信号源系统 16:9     1600/900
        /// </summary>
        public static int[] ALM_XHY_1600 = { 220, 840 };
        /// <summary>
        /// 下方按钮 信号源系统 禁止报警 16:9     1600/900
        /// </summary>
        public static int[] ALM_XHY_MUTE_1600 = { 220, 770 };
        /// <summary>
        /// 下方按钮 信号源系统 允许报警 16:9     1600/900
        /// </summary>
        public static int[] ALM_XHY_UNMUTE_1600 = { 280, 770 };
        /// <summary>
        /// 下方按钮 信号源系统 确定 16:9     1600/900
        /// </summary>
        public static int[] ALM_XHY_YES_1600 = { 340, 800 };

        /// <summary>
        /// 下方按钮 信号源系统 16:9     1920/1080
        /// </summary>
        public static int[] ALM_XHY_1920 = { 220, 1020 };
        /// <summary>
        /// 下方按钮 信号源系统 禁止报警 16:9     1920/1080
        /// </summary>
        public static int[] ALM_XHY_MUTE_1920 = { 220, 950 };
        /// <summary>
        /// 下方按钮 信号源系统 允许报警 16:9     1920/1080
        /// </summary>
        public static int[] ALM_XHY_UNMUTE_1920 = { 280, 950 };
        /// <summary>
        /// 下方按钮 信号源系统 确定 16:9     1920/1080
        /// </summary>
        public static int[] ALM_XHY_YES_1920 = { 340, 980 };
        #endregion


    }

}
