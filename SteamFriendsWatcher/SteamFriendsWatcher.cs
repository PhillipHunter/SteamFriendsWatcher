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
        private const String SETTINGS_FILE_PATH = "settings.csv";
        private const String OLD_FRIENDS_FILE_PATH = "oldFriends.json";
        private ISteamFriendsWatcher _ISteamFriendsWatcher;
        private BackgroundWorker checkWorker = new BackgroundWorker();

        private String apiKey;
        private String userSteamID;
        private List<Friend> oldFriends = null;
        private List<Friend> currentFriends = new List<Friend>();

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
            _ISteamFriendsWatcher.AddMessageLine("Checking friends of SteamID " + userSteamID);

            try
            {
                using (WebClient _WebClient = new WebClient())
                {
                    String oldFriendsJSON = null;

                    if (File.Exists(OLD_FRIENDS_FILE_PATH))
                    {
                        oldFriendsJSON = File.ReadAllText(OLD_FRIENDS_FILE_PATH);
                    }

                    String currentFriendsJSON = _WebClient.DownloadString($"http://api.steampowered.com/ISteamUser/GetFriendList/v0001/?key={apiKey}&steamid={userSteamID}&relationship=friend");
                    File.WriteAllText(OLD_FRIENDS_FILE_PATH, currentFriendsJSON);
                   
                    currentFriends = GetFriendsFromJSON(currentFriendsJSON);
                    oldFriends = (oldFriendsJSON == null) ? currentFriends : GetFriendsFromJSON(oldFriendsJSON);

                    _ISteamFriendsWatcher.AddMessageLine($"Old Friends Count: {oldFriends.Count}");
                    _ISteamFriendsWatcher.AddMessageLine($"Current Friends Count: {currentFriends.Count}");

                    if (oldFriends.SequenceEqual<Friend>(currentFriends))
                    {
                        _ISteamFriendsWatcher.AddMessageLine("No changes to friends list.\n");
                    }
                    else
                    {
                        _ISteamFriendsWatcher.AddMessageLine("Friends list has changed!", "orange");

                        List<Friend> friendsAdded = new List<Friend>();
                        foreach (Friend curr in currentFriends)
                        {
                            if (!oldFriends.Contains(curr))
                            {
                                curr.name = GetNameFromSteamID(curr.steamid);
                                friendsAdded.Add(curr);
                            }
                        }
                        _ISteamFriendsWatcher.AddMessageLine($"\nFriends Added - {friendsAdded.Count}", "green");
                        PrintListOfFriends(friendsAdded);

                        List<Friend> friendsRemoved = new List<Friend>();
                        foreach(Friend curr in oldFriends)
                        {
                            if(!currentFriends.Contains(curr))
                            {
                                curr.name = GetNameFromSteamID(curr.steamid);
                                friendsRemoved.Add(curr);
                            }
                        }
                        _ISteamFriendsWatcher.AddMessageLine($"\nFriends Removed - {friendsRemoved.Count}", "red");
                        PrintListOfFriends(friendsRemoved);                                            
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
            //TODO: Fix issue with switching userids.                        
            File.WriteAllText(SETTINGS_FILE_PATH, $"{apiKey},{steamID}");

            this.apiKey = apiKey;
            this.userSteamID = steamID;
            
            _ISteamFriendsWatcher.StartCheck();
            _ISteamFriendsWatcher.ClearMessages();
            checkWorker.RunWorkerAsync();
        }

        public List<Friend> GetFriendsFromJSON(String json)
        {
            JToken Jfriends = (JObject.Parse(json)["friendslist"])["friends"];
            List<JToken> JfriendsList = Jfriends.Children().ToList();

            List<Friend> result = new List<Friend>();
            foreach (JToken curr in JfriendsList)
            {
                Friend currFriend = new Friend();
                currFriend.steamid = (String)curr["steamid"];
                currFriend.friendsince = Int64.Parse((String)curr["friend_since"]);
                checkWorker.ReportProgress((int)(100 * ((float)(result.Count + 1)) / (JfriendsList.Count)));
                result.Add(currFriend);
            }

            return result;
        }

        public String GetNameFromSteamID(String steamID)
        {
            try
            {
                using (WebClient _WebClient = new WebClient())
                {
                    String personaFull = _WebClient.DownloadString($"http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key={apiKey}&steamids={steamID}");
                    return (String)(((JObject.Parse(personaFull)["response"])["players"])[0])["personaname"];
                }
            }
            catch (Exception)
            {
                return "NAME_UNKNOWN";
            }
        }

        public void PrintListOfFriends(List<Friend> list)
        {
            for(int i = 0; i < list.Count; i++)
            {
                _ISteamFriendsWatcher.AddMessageLine($"{i + 1} / {list.Count}: {list[i]}");
            }
        }
    }    

    public interface ISteamFriendsWatcher
    {
        void InitializeUserSettings(String apiKey, String userSteamid);
        void StartCheck();
        void StopCheck();
        void UpdateProgress(int progress);
        void AddMessageLine(String line, String color = "black");
        void ClearMessages();
    }
}
