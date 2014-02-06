using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using ZedGraph;

namespace LNL_Interface {
	static class Program {
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			try { Application.Run(new mainWindow()); } catch (System.ComponentModel.Win32Exception) { MessageBox.Show("Hot damn you need more RAM"); }
		}
	}
}