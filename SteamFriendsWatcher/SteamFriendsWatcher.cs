using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;

namespace SteamFriendsWatcher
{
    class SteamFriendsWatcher
    {
        private const String SETTINGS_FILE_PATH = "settings.txt";
        private ISteamFriendsWatcher _ISteamFriendsWatcher;
        private BackgroundWorker checkWorker = new BackgroundWorker();

        private String apiKey;
        private String userSteamID;
        private List<Friend> friends = new List<Friend>();

        public SteamFriendsWatcher(ISteamFriendsWatcher _ISteamFriendsWatcher)
        {
            if (!File.Exists(SETTINGS_FILE_PATH))
            {
                File.WriteAllText(SETTINGS_FILE_PATH, "APIKEY,STEAMID");
            }

            this._ISteamFriendsWatcher = _ISteamFriendsWatcher;
            _ISteamFriendsWatcher.InitializeUserSettings(File.ReadAllText(SETTINGS_FILE_PATH).Split(',')[0], File.ReadAllText(SETTINGS_FILE_PATH).Split(',')[1]);
            checkWorker.WorkerReportsProgress = true;
            checkWorker.DoWork += CheckWorker_DoWork;
            checkWorker.RunWorkerCompleted += CheckWorker_RunWorkerCompleted;
            checkWorker.ProgressChanged += CheckWorker_ProgressChanged;
        }

        private void CheckWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Console.WriteLine("Checking friends of SteamID " + userSteamID);

            try
            {
                using (WebClient _WebClient = new WebClient())
                {
                    Console.WriteLine();
                    String full = _WebClient.DownloadString($"http://api.steampowered.com/ISteamUser/GetFriendList/v0001/?key={apiKey}&steamid={userSteamID}&relationship=friend");
                    JToken Jfriends = (JObject.Parse(full)["friendslist"])["friends"];
                    List<JToken> JfriendsList = Jfriends.Children().ToList();

                    friends.Clear();
                    foreach (JToken curr in JfriendsList)
                    {
                        Friend currFriend = new Friend();
                        currFriend.steamid = (String)curr["steamid"];
                        currFriend.friendsince = Int64.Parse((String)curr["friend_since"]);

                        String personaFull = _WebClient.DownloadString($"http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key={apiKey}&steamids={currFriend.steamid}");
                        currFriend.name = (String)(((JObject.Parse(personaFull)["response"])["players"])[0])["personaname"];

                        Console.WriteLine($"{friends.Count + 1} / {JfriendsList.Count}:  {currFriend}");
                        checkWorker.ReportProgress((int)(100 * ((float)(friends.Count + 1)) / (JfriendsList.Count)));
                        friends.Add(currFriend);
                    }
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpStatusCode code = ((HttpWebResponse)ex.Response).StatusCode;
                    if (code == HttpStatusCode.Forbidden)
                    {
                        Console.WriteLine($"Could not retreive friends list. Is your API key correct?\n");
                    }
                    else if (code == HttpStatusCode.InternalServerError)
                    {
                        Console.WriteLine($"Could not retreive friends list. Is your SteamID correct?\n");
                    }
                    else
                    {
                        Console.WriteLine($"Could not retreive friends list. {ex.Message}\n");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not retreive friends list. {ex.Message}\n");
            }
        }

        private void CheckWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            _ISteamFriendsWatcher.UpdateProgress(e.ProgressPercentage);
        }

        private void CheckWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _ISteamFriendsWatcher.StopCheck();
        }

        public void Check(String apiKey, String steamID)
        {
            File.WriteAllText(SETTINGS_FILE_PATH, $"{apiKey},{steamID}");

            this.apiKey = apiKey;
            this.userSteamID = steamID;

            _ISteamFriendsWatcher.StartCheck();
            checkWorker.RunWorkerAsync();
        }
    }

    public interface ISteamFriendsWatcher
    {
        void InitializeUserSettings(String apiKey, String userSteamid);
        void StartCheck();
        void StopCheck();
        void UpdateProgress(int progress);
    }
}
