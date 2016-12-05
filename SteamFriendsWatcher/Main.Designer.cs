namespace SteamFriendsWatcher
{
    partial class Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.label1 = new System.Windows.Forms.Label();
            this.txtSteamID = new System.Windows.Forms.TextBox();
            this.btnCheck = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.txtAPIKey = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.cboFriends = new System.Windows.Forms.ComboBox();
            this.chkFriendsFriends = new System.Windows.Forms.CheckBox();
            this.btnRefreshFriendsCBO = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Steam ID:";
            // 
            // txtSteamID
            // 
            this.txtSteamID.Location = new System.Drawing.Point(72, 6);
            this.txtSteamID.Name = "txtSteamID";
            this.txtSteamID.Size = new System.Drawing.Size(109, 20);
            this.txtSteamID.TabIndex = 1;
            this.txtSteamID.Text = "XXXXXXX";
            // 
            // btnCheck
            // 
            this.btnCheck.Location = new System.Drawing.Point(745, 410);
            this.btnCheck.Name = "btnCheck";
            this.btnCheck.Size = new System.Drawing.Size(75, 23);
            this.btnCheck.TabIndex = 2;
            this.btnCheck.Text = "Check";
            this.btnCheck.UseVisualStyleBackColor = true;
            this.btnCheck.Click += new System.EventHandler(this.btnCheck_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(15, 410);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(724, 23);
            this.progressBar1.TabIndex = 3;
            // 
            // txtAPIKey
            // 
            this.txtAPIKey.Location = new System.Drawing.Point(72, 32);
            this.txtAPIKey.Name = "txtAPIKey";
            this.txtAPIKey.Size = new System.Drawing.Size(109, 20);
            this.txtAPIKey.TabIndex = 5;
            this.txtAPIKey.Text = "XXXXXXXXXX";
            this.txtAPIKey.UseSystemPasswordChar = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "API Key:";
            // 
            // webBrowser1
            // 
            this.webBrowser1.AllowNavigation = false;
            this.webBrowser1.IsWebBrowserContextMenuEnabled = false;
            this.webBrowser1.Location = new System.Drawing.Point(15, 58);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.ScriptErrorsSuppressed = true;
            this.webBrowser1.Size = new System.Drawing.Size(805, 346);
            this.webBrowser1.TabIndex = 6;
            this.webBrowser1.Url = new System.Uri("", System.UriKind.Relative);
            // 
            // cboFriends
            // 
            this.cboFriends.FormattingEnabled = true;
            this.cboFriends.Location = new System.Drawing.Point(394, 32);
            this.cboFriends.Name = "cboFriends";
            this.cboFriends.Size = new System.Drawing.Size(121, 21);
            this.cboFriends.TabIndex = 7;
            this.cboFriends.Visible = false;
            // 
            // chkFriendsFriends
            // 
            this.chkFriendsFriends.AutoSize = true;
            this.chkFriendsFriends.Enabled = false;
            this.chkFriendsFriends.Location = new System.Drawing.Point(246, 34);
            this.chkFriendsFriends.Name = "chkFriendsFriends";
            this.chkFriendsFriends.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chkFriendsFriends.Size = new System.Drawing.Size(142, 17);
            this.chkFriendsFriends.TabIndex = 9;
            this.chkFriendsFriends.Text = "Check a Friend\'s Friends";
            this.chkFriendsFriends.UseVisualStyleBackColor = true;
            this.chkFriendsFriends.CheckedChanged += new System.EventHandler(this.chkFriendsFriends_CheckedChanged);
            // 
            // btnRefreshFriendsCBO
            // 
            this.btnRefreshFriendsCBO.Location = new System.Drawing.Point(521, 32);
            this.btnRefreshFriendsCBO.Name = "btnRefreshFriendsCBO";
            this.btnRefreshFriendsCBO.Size = new System.Drawing.Size(91, 23);
            this.btnRefreshFriendsCBO.TabIndex = 10;
            this.btnRefreshFriendsCBO.Text = "Refresh Names";
            this.btnRefreshFriendsCBO.UseVisualStyleBackColor = true;
            this.btnRefreshFriendsCBO.Visible = false;
            this.btnRefreshFriendsCBO.Click += new System.EventHandler(this.btnRefreshFriendsCBO_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(832, 445);
            this.Controls.Add(this.btnRefreshFriendsCBO);
            this.Controls.Add(this.chkFriendsFriends);
            this.Controls.Add(this.cboFriends);
            this.Controls.Add(this.webBrowser1);
            this.Controls.Add(this.txtAPIKey);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.btnCheck);
            this.Controls.Add(this.txtSteamID);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Main";
            this.Text = "Steam Friends Watcher";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtSteamID;
        private System.Windows.Forms.Button btnCheck;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.TextBox txtAPIKey;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.WebBrowser webBrowser1;
        private System.Windows.Forms.ComboBox cboFriends;
        private System.Windows.Forms.CheckBox chkFriendsFriends;
        private System.Windows.Forms.Button btnRefreshFriendsCBO;
    }
}

