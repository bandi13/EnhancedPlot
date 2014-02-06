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
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

namespace LNL_Interface {
	public partial class Mesh : Form {
		private static object figsLock = new object();
		private static List<Mesh> figures = new List<Mesh>();
		private double curObsX, curObsY, curObsZ; // current observer coordinates
		private int osX, osY; // screen offsets
		private double screenDistance, sf, cf, st, ct, R, A, B, C, D, baseB, baseD; //transformations coeficients
		private double[,] curDat;
		private Color penColor = Color.Black;
		private SolidBrush[] brushes;
#if DEBUG
		private Label lblCoords = new Label();
#endif
		public bool isFilled = false;

		private Mesh() {
			InitializeComponent();
			lock (figsLock) { figures.Add(this); }
#if DEBUG
			lblCoords.Location = new Point(0, 0);
			this.Controls.Add(lblCoords);
#endif
			this.Disposed += new EventHandler(Mesh_Disposed);
			this.MouseWheel += new MouseEventHandler(Mesh_MouseWheel);
			osX = 0;
			osY = 0;
			this.ResizeRedraw = true;
			this.DoubleBuffered = true;
			this.Show();
		}

		/// <summary>Uses the default observer coordinates of [100,100,100] at a distance of 0.5</summary>
		/// <param name="Title"></param>
		public Mesh(string Title) : this(Title,100,100,100,0.5) { }
		/// <param name="x">Observer X coordinate</param>
		/// <param name="y">Observer Y coordinate</param>
		/// <param name="z">Observer Z coordinate</param>
		public Mesh(double x, double y, double z, double dist) : this("",x,y,z,dist) { }
		/// <param name="x">Observer X coordinate</param>
		/// <param name="y">Observer Y coordinate</param>
		/// <param name="z">Observer Z coordinate</param>
		public Mesh(string Title, double x, double y, double z, double dist)
			: this() {
			this.Text = Title;
			this.calcTransCoeff(x, y, z);
			this.curObsX = x;
			this.curObsY = y;
			this.curObsZ = z;
			this.screenDistance = dist;
			brushes = new SolidBrush[255];
			for (int i = 0; i < brushes.Length; i++) brushes[i] = new SolidBrush(Color.FromArgb(i,0,255-i));
		}
		private void Mesh_Disposed(object sender, EventArgs e) {
			lock (figsLock) { figures.Remove(this); }
			for (int i = 0; i < brushes.Length; i++) brushes[i].Dispose();
		}

		private void calcTransCoeff(double obsX, double obsY, double obsZ) {
			double r1, a;

			r1 = obsX * obsX + obsY * obsY;
			a = Math.Sqrt(r1);//distance in XY plane
			R = Math.Sqrt(r1 + obsZ * obsZ);//distance from observator to center
			if (a != 0) //rotation matrix coeficients calculation
            {
				sf = obsY / a;//sin( fi)
				cf = obsX / a;//cos( fi)
			} else {
				sf = 0;
				cf = 1;
			}
			st = a / R;//sin( teta)
			ct = obsZ / R;//cos( teta)

			double screenWidthPhys = ClientRectangle.Width * 0.0257 / 72.0;        //0.0257 m = 1 inch. Screen has 72 px/inch
			double screenHeightPhys = ClientRectangle.Height * 0.0257 / 72.0;

			//linear tranfrormation coeficients
			A = (double)ClientRectangle.Width / screenWidthPhys;
			baseB = A * screenWidthPhys / 2.0;
			B = baseB + osX;
			C = -(double)ClientRectangle.Height / screenHeightPhys;
			baseD = -C * screenHeightPhys / 2.0;
			D = baseD + osY;
		}

		private void updateOffset(int x, int y) { B = baseB + x; D = baseD + y; }

		/// <summary>
		/// Performs projection. Calculates screen coordinates for 3D point.
		/// </summary>
		/// <param name="x">Point's x coordinate.</param>
		/// <param name="y">Point's y coordinate.</param>
		/// <param name="z">Point's z coordinate.</param>
		/// <returns>Point in 2D space of the screen.</returns>
		public PointF Project(double x, double y, double z) {
			double xn, yn, zn;//point coordinates in computer's frame of reference

			//transformations
			xn = sf * x - cf * y;
			yn = cf * ct * x + sf * ct * y + st * z;
			zn = cf * st * x + sf * st * y - ct * z + R;

			if (zn == 0) zn = 0.01;

			//Tales' theorem
			return new PointF((float)(A * xn * screenDistance / zn + B), (float)(C * yn * screenDistance / zn + D));
		}

		private const int NORMALIZEDHEIGHT = 20;
		public void RenderSurface(Graphics graphics) {
			int lenX = curDat.GetLength(0), lenY = curDat.GetLength(1);
			int lenX_2 = lenX >> 1, lenY_2 = lenY >> 1;
			PointF[,] meshF = new PointF[lenX, lenY];
			int x, y;
			for (x = 0; x < lenX; x++) for (y = 0; y < lenY; y++) meshF[x, y] = Project(y - lenY_2, x - lenX_2, curDat[x, y]); // notice the reversal of X and Y

			Pen pen = new Pen(penColor);
			PointF[] polygon = new PointF[4];
			graphics.SmoothingMode = SmoothingMode.HighSpeed;
			double cc = ((double)brushes.Length - 1.0) / NORMALIZEDHEIGHT;
			for (x = 0; x < lenX - 1; x++) {
				for (y = 0; y < lenY - 1; y++) {
					polygon[0] = meshF[x, y];
					polygon[1] = meshF[x, y + 1];
					polygon[2] = meshF[x + 1, y + 1];
					polygon[3] = meshF[x + 1, y];

					if (isFilled) graphics.FillPolygon(brushes[(byte)(((curDat[x, y] + curDat[x, y + 1]) / 2.0) * cc)], polygon);
					graphics.DrawPolygon(pen, polygon);
				}
			}
			graphics.DrawString("0", this.Font, Brushes.Black, Project(-lenX_2 - 1, -lenY_2 - 1, NORMALIZEDHEIGHT * 0.5));
			graphics.DrawString(lenX.ToString(), this.Font, Brushes.Black, Project(lenX_2 + 1, -lenY_2 - 1, NORMALIZEDHEIGHT * 0.5));
			graphics.DrawString(lenY.ToString(), this.Font, Brushes.Black, Project(-lenX_2 - 1, lenY_2 + 1, NORMALIZEDHEIGHT * 0.5));

			if (mbStart != MouseButtons.None) graphics.DrawRectangle(pen, mpStart.X, mpStart.Y, 2, 2);
		}

		/// <summary>Close all the figures ever created by this class</summary>
		public static void closeAll() { while (figures.Count != 0) figures[0].Dispose(); }

		public void setTitle(string Title) { this.Text = Title; }

		public void plot(double[,] z) { addPlot("",z); }
		public void addPlot(string Title, double[,] z) {
			// Normalize the data first
			double curPt, minZ = double.PositiveInfinity, maxZ = double.NegativeInfinity;
			int lenX = z.GetLength(0), lenY = z.GetLength(1);
			for (int x = 0; x < lenX; x++) {
				for (int y = 0; y < lenY; y++) {
					curPt = z[x, y];

					if (minZ > curPt) minZ = curPt;
					if (maxZ < curPt) maxZ = curPt;
				}
			}
			for (int x = 0; x < lenX; x++) for (int y = 0; y < lenY; y++) z[x, y] = NORMALIZEDHEIGHT * (z[x, y] - minZ) / (maxZ - minZ);
			curDat = z;
			Invalidate();
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

		private void Mesh_Resize(object sender, EventArgs e) { calcTransCoeff(curObsX, curObsY, curObsZ); }

		private void Mesh_Paint(object sender, PaintEventArgs e) {
			e.Graphics.Clear(BackColor);
			RenderSurface(e.Graphics);
		}

		private Point mpStart;
		private MouseButtons mbStart;
		private void Mesh_MouseDown(object sender, MouseEventArgs e) {
			if ((e.Button == MouseButtons.Left) && (mbStart == MouseButtons.Right)) { isFilled = !isFilled; Invalidate(); }
			else {
				mpStart = e.Location;
				mbStart = e.Button;
			}
		}

		private void Mesh_MouseMove(object sender, MouseEventArgs e) {
			if (mbStart == MouseButtons.Left) {
				if ((Control.ModifierKeys & Keys.Control) == Keys.Control) {
					updateOffset(osX + e.Location.X - mpStart.X, osY + e.Location.Y - mpStart.Y);
					Invalidate();
				} else {
					calcTransCoeff(curObsX + (mpStart.X - e.Location.X), curObsY + (mpStart.Y - e.Location.Y), curObsZ);
					Invalidate();
#if DEBUG
					lblCoords.Text = '[' + Convert.ToString(curObsX + (mpStart.X - e.Location.X)) + ',' + Convert.ToString(curObsY + (mpStart.Y - e.Location.Y)) + ',' + curObsZ.ToString() + ']';
#endif
				}
			} else if (mbStart == MouseButtons.Right) {
				calcTransCoeff(curObsX, curObsY, curObsZ - (mpStart.Y - e.Location.Y));
				Invalidate();
#if DEBUG
				lblCoords.Text = '[' + curObsX.ToString() + ',' + curObsY.ToString() + ',' + Convert.ToString(curObsZ + (mpStart.Y - e.Location.Y)) + ']';
#endif
			}
		}

		private void Mesh_MouseUp(object sender, MouseEventArgs e) {
			if (mbStart == MouseButtons.Left) {
				if ((Control.ModifierKeys & Keys.Control) == Keys.Control) {
					osX = osX + e.Location.X - mpStart.X;
					osY = osY + e.Location.Y - mpStart.Y;
				} else {
					curObsX += (mpStart.X - e.Location.X); curObsY += (mpStart.Y - e.Location.Y);
					calcTransCoeff(curObsX, curObsY, curObsZ);
					Invalidate();
				}
			} else if (mbStart == MouseButtons.Right) {
				curObsZ -= (mpStart.Y - e.Location.Y);
				calcTransCoeff(curObsX, curObsY, curObsZ);
				Invalidate();
			}
			if ((e.Button != MouseButtons.Left) || (mbStart != MouseButtons.Right)) { mbStart = MouseButtons.None; }
		}

		private void Mesh_MouseWheel(object sender, MouseEventArgs e) {
			screenDistance += screenDistance * ((e.Delta > 0) ? 0.1 : -0.1);
			if (screenDistance < 0) screenDistance = 0;
			Invalidate();
		}
	}
}