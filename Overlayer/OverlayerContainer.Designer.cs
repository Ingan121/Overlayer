namespace Overlayer
{
    partial class OverlayerContainer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // OverlayerContainer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(173)))), ((int)(((byte)(147)))), ((int)(((byte)(239)))));
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "OverlayerContainer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "OverlayerContainer";
            this.TransparencyKey = System.Drawing.Color.FromArgb(((int)(((byte)(173)))), ((int)(((byte)(147)))), ((int)(((byte)(239)))));
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OverlayerContainer_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion
    }
}