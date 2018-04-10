namespace YouTubePlaylistsMaintenance
{
    partial class FormYouTubePlaylistSave
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
            this.buttonGetAllplaylists = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.progressBarDownloadRate = new System.Windows.Forms.ProgressBar();
            this.textBoxChannelID = new System.Windows.Forms.TextBox();
            this.listBoxPlaylists = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.labelCurrentPlaylist = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonGetAllplaylists
            // 
            this.buttonGetAllplaylists.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonGetAllplaylists.Location = new System.Drawing.Point(489, 27);
            this.buttonGetAllplaylists.Name = "buttonGetAllplaylists";
            this.buttonGetAllplaylists.Size = new System.Drawing.Size(158, 23);
            this.buttonGetAllplaylists.TabIndex = 13;
            this.buttonGetAllplaylists.Text = "Get Playlists";
            this.buttonGetAllplaylists.UseVisualStyleBackColor = true;
            this.buttonGetAllplaylists.Click += new System.EventHandler(this.ButtonGetAllplaylists_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 13);
            this.label1.TabIndex = 14;
            this.label1.Text = "Enter channel ID:";
            // 
            // progressBarDownloadRate
            // 
            this.progressBarDownloadRate.Location = new System.Drawing.Point(12, 525);
            this.progressBarDownloadRate.Name = "progressBarDownloadRate";
            this.progressBarDownloadRate.Size = new System.Drawing.Size(635, 20);
            this.progressBarDownloadRate.TabIndex = 15;
            // 
            // textBoxChannelID
            // 
            this.textBoxChannelID.Location = new System.Drawing.Point(117, 29);
            this.textBoxChannelID.Name = "textBoxChannelID";
            this.textBoxChannelID.Size = new System.Drawing.Size(356, 20);
            this.textBoxChannelID.TabIndex = 16;
            // 
            // listBoxPlaylists
            // 
            this.listBoxPlaylists.FormattingEnabled = true;
            this.listBoxPlaylists.Location = new System.Drawing.Point(12, 76);
            this.listBoxPlaylists.Name = "listBoxPlaylists";
            this.listBoxPlaylists.Size = new System.Drawing.Size(626, 420);
            this.listBoxPlaylists.TabIndex = 17;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(99, 13);
            this.label2.TabIndex = 18;
            this.label2.Text = "Playlists in channel:";
            // 
            // labelCurrentPlaylist
            // 
            this.labelCurrentPlaylist.AutoSize = true;
            this.labelCurrentPlaylist.Location = new System.Drawing.Point(9, 508);
            this.labelCurrentPlaylist.Name = "labelCurrentPlaylist";
            this.labelCurrentPlaylist.Size = new System.Drawing.Size(112, 13);
            this.labelCurrentPlaylist.TabIndex = 19;
            this.labelCurrentPlaylist.Text = "Currently Proccessing:";
            this.labelCurrentPlaylist.Visible = false;
            // 
            // FormYouTubePlaylistSave
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ClientSize = new System.Drawing.Size(658, 557);
            this.Controls.Add(this.labelCurrentPlaylist);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.listBoxPlaylists);
            this.Controls.Add(this.textBoxChannelID);
            this.Controls.Add(this.progressBarDownloadRate);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonGetAllplaylists);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormYouTubePlaylistSave";
            this.ShowIcon = false;
            this.Text = "Playlsits Maintanace";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button buttonGetAllplaylists;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ProgressBar progressBarDownloadRate;
        private System.Windows.Forms.TextBox textBoxChannelID;
        private System.Windows.Forms.ListBox listBoxPlaylists;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelCurrentPlaylist;
    }
}

