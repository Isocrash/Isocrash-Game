namespace Voxine
{
    partial class GameWindow
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GameWindow));
            this.Render = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.Render)).BeginInit();
            this.SuspendLayout();
            // 
            // Render
            // 
            this.Render.BackColor = System.Drawing.Color.Black;
            this.Render.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Render.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Render.Location = new System.Drawing.Point(0, 0);
            this.Render.Margin = new System.Windows.Forms.Padding(0);
            this.Render.Name = "Render";
            this.Render.Size = new System.Drawing.Size(1280, 719);
            this.Render.TabIndex = 0;
            this.Render.TabStop = false;
            this.Render.Click += new System.EventHandler(this.RenderPictureBox_Click);
            // 
            // GameWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1280, 719);
            this.Controls.Add(this.Render);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "GameWindow";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Raymarcher";
            this.LostFocus += new System.EventHandler(this.GameWindow_LostFocus);
            this.Resize += new System.EventHandler(this.GameWindow_Resize);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GameWindow_Closing);
            ((System.ComponentModel.ISupportInitialize)(this.Render)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        internal System.Windows.Forms.PictureBox Render;
    }
}

