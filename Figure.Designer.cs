namespace LNL_Interface {
	partial class Figure {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Figure));
			this.zgrPlot = new ZedGraph.ZedGraphControl();
			this.SuspendLayout();
			// 
			// zgrPlot
			// 
			this.zgrPlot.Location = new System.Drawing.Point(1, 1);
			this.zgrPlot.Name = "zgrPlot";
			this.zgrPlot.ScrollGrace = 0;
			this.zgrPlot.ScrollMaxX = 0;
			this.zgrPlot.ScrollMaxY = 0;
			this.zgrPlot.ScrollMaxY2 = 0;
			this.zgrPlot.ScrollMinX = 0;
			this.zgrPlot.ScrollMinY = 0;
			this.zgrPlot.ScrollMinY2 = 0;
			this.zgrPlot.Size = new System.Drawing.Size(637, 451);
			this.zgrPlot.TabIndex = 0;
			// 
			// Figure
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(640, 453);
			this.Controls.Add(this.zgrPlot);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.MinimizeBox = false;
			this.Name = "Figure";
			this.Resize += new System.EventHandler(this.Figure_Resize);
			this.ResumeLayout(false);

		}

		#endregion

		private ZedGraph.ZedGraphControl zgrPlot;
	}
}