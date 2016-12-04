using System;
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
        }

        private void btnCheck_Click(object sender, EventArgs e)
        {
            _SteamFriendsWatcher.Check(txtAPIKey.Text, txtSteamID.Text);
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
    }
}
