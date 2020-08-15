using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;


public sealed class ProcessHelper {

	/// <summary>
	/// 用命令行启动外部程序
	/// </summary>
	/// <param name="fullName"></param>
	public void StartProcess(string fullFileName) {
		string name, tempName, workingDir;
		Process cmd = new Process();
		cmd.StartInfo.UseShellExecute = false;
		cmd.StartInfo.CreateNoWindow = true;
		cmd.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;

		// 从文本框获得文件路径
        cmd.StartInfo.FileName = fullFileName;
		// 路径的字符串转义处理
        name = fullFileName;
		tempName = name.Replace(@"\", @"\\");
		var gang = tempName.LastIndexOf(@"\\");

		workingDir = tempName.Substring(0, gang);
		// 设置程序工作路径
		cmd.StartInfo.WorkingDirectory = workingDir;
		//cmd.StartInfo.WorkingDirectory = "C:\\Program Files\\151103\\";

		// 命令行启动
		cmd.Start();
	}

	/// <summary>
	/// 根据进程名结束进程
	/// </summary>
	/// <param name="processName"></param>
	public void KillProcess(string processName) {
		try {
			foreach (var process in Process.GetProcessesByName(processName)) {
				process.Kill();
			}
		}
		catch (Exception ex) {
			throw ex;
		}
	}

    /// <summary>
    /// 判断进程数量
    /// </summary>
    /// <param name="exeName"></param>
    /// <returns></returns>
    public int ProcessesCount(string exeName) {
		int num = 0;
		try {
			foreach (var process in Process.GetProcessesByName(exeName)) {
				num += 1;
			}
			return num;
		}
		catch (Exception ex) {
			return 0;
			throw ex;
		}
	}


}

