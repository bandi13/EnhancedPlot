namespace LNL_Interface {
	partial class BodePlot {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BodePlot));
			this.zgrGraph = new ZedGraph.ZedGraphControl();
			this.SuspendLayout();
			// 
			// zgrGraph
			// 
			this.zgrGraph.Location = new System.Drawing.Point(1, 1);
			this.zgrGraph.Name = "zgrGraph";
			this.zgrGraph.ScrollGrace = 0;
			this.zgrGraph.ScrollMaxX = 0;
			this.zgrGraph.ScrollMaxY = 0;
			this.zgrGraph.ScrollMaxY2 = 0;
			this.zgrGraph.ScrollMinX = 0;
			this.zgrGraph.ScrollMinY = 0;
			this.zgrGraph.ScrollMinY2 = 0;
			this.zgrGraph.Size = new System.Drawing.Size(606, 440);
			this.zgrGraph.TabIndex = 0;
			// 
			// BodePlot
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.ClientSize = new System.Drawing.Size(611, 444);
			this.Controls.Add(this.zgrGraph);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.Name = "BodePlot";
			this.Text = "BodePlot";
			this.Resize += new System.EventHandler(this.BodePlot_Resize);
			this.ResumeLayout(false);

		}

		#endregion

		private ZedGraph.ZedGraphControl zgrGraph;
	}
}