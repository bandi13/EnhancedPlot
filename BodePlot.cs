// Written by: Fekete András
// Date: 7/31/2009
// No warranties or guarantees of any kind.
// Released under the GNU public license

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ZedGraph;
using CSML;

namespace LNL_Interface {
	public partial class BodePlot : Form {
		private static object figsLock = new object();
		private static List<BodePlot> figures = new List<BodePlot>();
		private bool connectedInt = false;
		public bool connected {
			get { return connectedInt; }
			set {
				for (int i = 0; i < numCurves; i++) ((LineItem)zgrGraph.GraphPane.CurveList[i]).Line.IsVisible = value;
				connectedInt = value;
			}
		}

		public BodePlot() {
			InitializeComponent();
			lock (figsLock) { figures.Add(this); }
			this.Disposed += new EventHandler(BodePlot_Disposed);
			GraphPane myPane = zgrGraph.GraphPane;
			myPane.YAxis.Title.Text = "Magnitude (dB)";
			myPane.Y2Axis.Title.Text = "Phase (Deg)";
			myPane.Y2Axis.IsVisible = true;
			// turn off the opposite tics so the Y tics don't show up on the Y2 axis
			myPane.YAxis.MajorTic.IsOpposite = false;
			myPane.YAxis.MinorTic.IsOpposite = false;
			myPane.YAxis.Scale.FontSpec.FontColor = Color.Blue;
			myPane.YAxis.Title.FontSpec.FontColor = Color.Blue;
			myPane.Y2Axis.MajorTic.IsOpposite = false;
			myPane.Y2Axis.MinorTic.IsOpposite = false;
			myPane.Y2Axis.Scale.FontSpec.FontColor = Color.Red;
			myPane.Y2Axis.Title.FontSpec.FontColor = Color.Red;
			myPane.Title.IsVisible = false;
			myPane.XAxis.Title.Text = "";
			updateGraph();
			this.Show();
		}
		public BodePlot(string Title) : this() { this.Text = Title; }
		public BodePlot(string Title, double[] freqs, double[] mag, double[] phase) : this(Title) { this.addPlot(freqs, mag, phase); }

		private void BodePlot_Disposed(object sender, EventArgs e) { lock (figsLock) { figures.Remove(this); } }

		/// <summary>Close all the figures ever created by this class</summary>
		public static void closeAll() { while (figures.Count != 0) figures[0].Dispose(); }

		/// <summary>Number of different plots on this figure</summary>
		public int numCurves { get { return zgrGraph.GraphPane.CurveList.Count; } }

		/// <summary>Converts the magnitude to dB scale</summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static double[] mag2dB(double[] input) {
			double[] output = new double[input.Length];
			for (int i = 0; i < input.Length; i++) output[i] = 20 * Math.Log10(input[i]);
			return output;
		}

		public static double[] unwrap(double[] input) {
			double[] output = new double[input.Length];
			output[0] = input[0];
			for(int i = 1; i < input.Length; i++) {
				if (Math.Abs(output[i - 1] - input[i]) >= Math.PI) {
					if (Math.Abs(output[i - 1] - input[i] - 2 * Math.PI) < Math.PI) output[i] = input[i] + 2 * Math.PI;
					else if (Math.Abs(output[i - 1] - input[i] + 2 * Math.PI) < Math.PI) output[i] = input[i] - 2 * Math.PI;
				} else output[i] = input[i];
			}
			return output;
		}

		/// <summary>Converts an angle from radians to degrees</summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static double[] phase2Deg(double[] input) {
			double[] output = new double[input.Length];
			for (int i = 0; i < input.Length; i++) output[i] = 180 * input[i] / Math.PI;
			return output;
		}

		public void plot(int curveNo, double[] freqs, Complex[] data) { plot(curveNo, freqs, Complex.Abs(data), Complex.Angle(data)); }
		public void plot(int curveNo, double[] freqs, double[] mag, double[] phase) {
			curveNo = curveNo * 2;
			if (curveNo > numCurves) throw new Exception("Tried to access curve that doesn't exist in figure");
			if ((freqs.Length != mag.Length) || (freqs.Length != phase.Length)) throw new Exception("Number of points differ in passed arguments");
			zgrGraph.GraphPane.CurveList[curveNo].Points = new PointPairList(freqs, mag2dB(mag));
			zgrGraph.GraphPane.CurveList[curveNo+1].Points = new PointPairList(freqs, phase2Deg(unwrap(phase)));
			updateGraph();
		}

		public int addPlot(double[] freqs, Complex[] data) { return addPlot("", freqs, data); }
		public int addPlot(string Title, double[] freqs, Complex[] data) { return addPlot(Title, freqs, Complex.Abs(data), Complex.Angle(data)); }
		public int addPlot(double[] freqs, double[] mag, double[] phase) { return addPlot("", freqs, mag, phase); }
		public int addPlot(string Title, double[] freqs, double[] mag, double[] phase) {
			if ((freqs.Length != mag.Length) || (freqs.Length != phase.Length)) throw new Exception("Number of points differ in passed arguments");
			LineItem magCurve = zgrGraph.GraphPane.AddCurve(Title, freqs, mag2dB(mag), Color.Blue, getSymbol(numCurves/2));
			magCurve.Line.IsVisible = connected;
			LineItem phCurve = zgrGraph.GraphPane.AddCurve(Title, freqs, phase2Deg(unwrap(phase)), Color.Red, getSymbol((numCurves-1)/2));
			phCurve.Line.IsVisible = connected;
			phCurve.IsY2Axis = true;
//			if (Title.Equals("")) { phCurve.Label.IsVisible = magCurve.Label.IsVisible = false; }
			updateGraph();
			return numCurves - 2;
		}

		private void updateGraph() { zgrGraph.AxisChange(); zgrGraph.Refresh(); }

		public void setXAxisType(AxisType type) {
			zgrGraph.GraphPane.XAxis.Type = type;
			updateGraph();
		}

		private void BodePlot_Resize(object sender, EventArgs e) {
			zgrGraph.Width = this.Width - 10;
			zgrGraph.Height = this.Height - 28;
		}

		private SymbolType getSymbol(int num) {
			switch (num % 4) {
				case 0: return SymbolType.XCross;
				case 1: return SymbolType.Diamond;
				case 2: return SymbolType.Circle;
				case 3: return SymbolType.Triangle;
			}
			return SymbolType.Default;
		}
	}
}