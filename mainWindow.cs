// Written by: Fekete András
// Date: 7/31/2009
// No warranties or guarantees of any kind.
// Released under the GNU public license

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ZedGraph;
using CSML;

namespace LNL_Interface {
	public partial class mainWindow : Form {
		public mainWindow() {
			InitializeComponent();

			Mesh test = new Mesh("Mesh Example");
			double[,] tData = new double[40, 40];
			for (int i = 0; i < tData.GetLength(0); i++) for (int j = 0; j < tData.GetLength(1); j++) tData[i, j] = Math.Sin(i) + Math.Cos(j);
			test.plot(tData);

            Figure test2 = new Figure("Figure Example");
            double[] t2Data = new double[40];
            for (int i = 0; i < t2Data.Length; i++) t2Data[i] = Math.Sin(i);
            test2.addPlot("plot1", t2Data);

            BodePlot test3 = new BodePlot("BodePlot Example");
            double[] t3Data = new double[40];
            for (int i = 0; i < t3Data.Length; i++) t3Data[i] = 2*Math.PI*i;
            test3.addPlot("plotA", t3Data, Complex.array(t2Data));
        }

        private void button1_Click(object sender, EventArgs e) { this.Close(); }
	}
}