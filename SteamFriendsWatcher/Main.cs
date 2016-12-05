using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SteamFriendsWatcher
{
    public partial class Main : Form, ISteamFriendsWatcher
    {
        SteamFriendsWatcher _SteamFriendsWatcher;

        public Main()
        {
            InitializeComponent();
            _SteamFriendsWatcher = new SteamFriendsWatcher(this);
        }

        public void InitializeUserSettings(String apiKey, String userSteamid)
        {
            txtAPIKey.Text = apiKey;
            txtSteamID.Text = userSteamid;
        }

        public void StartCheck()
        {
            btnCheck.Enabled = false;
        }

        public void UpdateProgress(int progress)
        {
            progressBar1.Value = progress;
        }

        public void StopCheck()
        {
            btnCheck.Enabled = true;
            chkFriendsFriends.Enabled = true;
        }

        private void btnCheck_Click(object sender, EventArgs e)
        {
            if (!chkFriendsFriends.Checked)
            {
                _SteamFriendsWatcher.Check(txtAPIKey.Text, txtSteamID.Text);
            }
            else
            {
                new SteamFriendsWatcher(this).Check(txtAPIKey.Text, _SteamFriendsWatcher.GetUsersFriends()[cboFriends.SelectedIndex].steamid, false);
            }
        }

        public void AddMessageLine(String line, String color)
        {
            this.Invoke(new MethodInvoker(
                delegate
                {
                    webBrowser1.Document.Body.InnerHtml += $"<b><font face='Arial', font color=\"{color}\">{line.Replace("\n", "<br/>")}</font><br/></b>";
                }));
        }

        public void ClearMessages()
        {
            webBrowser1.Document.Body.InnerHtml = String.Empty;
        }

        private void RefreshFriendsComboBox()
        {
            if (chkFriendsFriends.Checked)
            {
                cboFriends.Items.Clear();
                _SteamFriendsWatcher.GenerateAllFriendNames();
            }
        }

        private void chkFriendsFriends_CheckedChanged(object sender, EventArgs e)
        {
            cboFriends.Visible = chkFriendsFriends.Checked;
            btnRefreshFriendsCBO.Visible = chkFriendsFriends.Checked;

            if (!_SteamFriendsWatcher.namesGenerated)
            {
                List<Friend> currFriends = _SteamFriendsWatcher.GetUsersFriends();
                cboFriends.Items.Clear();
                foreach (Friend curr in currFriends)
                {
                    cboFriends.Items.Add(curr.steamid);
                }
            }
        }

        public void StopNameGeneration()
        {
            foreach (Friend curr in _SteamFriendsWatcher.GetUsersFriends())
            {
                cboFriends.Items.Add(curr.name);
            }
        }

        private void btnRefreshFriendsCBO_Click(object sender, EventArgs e)
        {
            RefreshFriendsComboBox();
        }
    }
}
