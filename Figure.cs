// Written by: Fekete András
// Date: 7/31/2009
// No warranties or guarantees of any kind.
// Released under the GNU public license

using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ZedGraph;

namespace LNL_Interface {
	public partial class Figure : Form {
		private static object figsLock = new object();
		private static List<Figure> figures = new List<Figure>();
		private bool connectedInt = true;
		public bool connected {
			get { return connectedInt; }
			set {
				for (int i = 0; i < numCurves; i++) ((LineItem)zgrPlot.GraphPane.CurveList[i]).Line.IsVisible = value;
				connectedInt = value;
			}
		}

		public Figure() {
			InitializeComponent();
			lock (figsLock) { figures.Add(this); }
			this.Disposed += new EventHandler(Figure_Disposed);
			zgrPlot.GraphPane.Title.IsVisible = false;
			zgrPlot.GraphPane.YAxis.Title.Text = "";
			zgrPlot.GraphPane.XAxis.Title.Text = "";
			this.Show();
		}
		public Figure(string Title) : this() { this.Text = Title; }
		public Figure(string Title, double[] data) : this(Title) { this.addPlot(data); }
		public Figure(string Title, PointPairList data) : this(Title) { this.addPlot(data); }
		private void Figure_Disposed(object sender, EventArgs e) { lock (figsLock) { figures.Remove(this); } }

		/// <summary>Close all the figures ever created by this class</summary>
		public static void closeAll() { while (figures.Count != 0) figures[0].Dispose(); }

		/// <summary>Number of different plots on this figure</summary>
		public int numCurves { get { return zgrPlot.GraphPane.CurveList.Count; } }

		public void setYAxisLabel(string label) { zgrPlot.GraphPane.YAxis.Title.Text = label; }

		public void setLabel(int curveNo, string Title) {
			if (curveNo > numCurves) throw new Exception("Tried to access curve that doesn't exist in figure");
			zgrPlot.GraphPane.CurveList[curveNo].Label.Text = Title;
			if (Title.Equals("")) zgrPlot.GraphPane.CurveList[curveNo].Label.IsVisible = false;
			updateGraph();
		}

		public void setTitle(string Title) { this.Text = Title; }

		public void setGraphTitle(string Title) {
			zgrPlot.GraphPane.Title.Text = Title;
			zgrPlot.GraphPane.Title.IsVisible = !Title.Equals("");
			updateGraph();
		}

        public static PointPairList doubleArr2PPL(double[] data)
        {
            double[] x = new double[data.Length];
            for (int i = 0; i < data.Length; i++) x[i] = i;
            return new PointPairList(x, data);
        }

        public void plot(int curveNo, double[] data) { plot(curveNo, doubleArr2PPL(data)); }
		public void plot(int curveNo, PointPairList data) {
			if (curveNo > numCurves) throw new Exception("Tried to access curve that doesn't exist in figure");
			zgrPlot.GraphPane.CurveList[curveNo].Points = data;
			updateGraph();
		}

		public void addPlot(double[] data) { addPlot("", data); }
		public void addPlot(PointPairList data) { addPlot("", data); }
		public void addPlot(string Title, double[] data) { addPlot(Title, doubleArr2PPL(data)); }
		public void addPlot(string Title, PointPairList data) {
			LineItem cur = zgrPlot.GraphPane.AddCurve(Title, data, getColor(zgrPlot.GraphPane.CurveList.Count),SymbolType.None);
			if (Title.Equals("")) cur.Label.IsVisible = false;
			cur.Line.IsVisible = connected;
			updateGraph();
		}

		public void setXAxisType(AxisType type) {
			zgrPlot.GraphPane.XAxis.Type = type;
			updateGraph();
		}

		public static Color getColor(int i) {
			switch (i % 6) {
				case 0: return Color.Blue;
				case 1: return Color.Red;
				case 2: return Color.Green;
				case 3: return Color.Orange;
				case 4: return Color.Purple;
				case 5: return Color.Black;
			}
			return Color.Gray;
		}

		private void updateGraph() { zgrPlot.AxisChange(); zgrPlot.Refresh(); }

		private void Figure_Resize(object sender, EventArgs e) {
			zgrPlot.Width = this.Width - 10;
			zgrPlot.Height = this.Height - 28;
		}
	}
}