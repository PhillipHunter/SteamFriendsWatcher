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
        private const String LOG_FILE_PATH = "log.txt";
        private const String OLD_FRIENDS_FOLDER_PATH = "data";
        private String oldFriendsFilePath;
        private ISteamFriendsWatcher _ISteamFriendsWatcher;
        private BackgroundWorker checkWorker = new BackgroundWorker();
        private BackgroundWorker nameGeneratorWorker = new BackgroundWorker();

        private String apiKey;
        private String userSteamID;
        private List<Friend> oldFriends = null;
        private List<Friend> currentFriends = new List<Friend>();
        public Boolean namesGenerated = false;

        public SteamFriendsWatcher(ISteamFriendsWatcher _ISteamFriendsWatcher)
        {
            if (!File.Exists(SETTINGS_FILE_PATH))
            {
                File.WriteAllText(SETTINGS_FILE_PATH, "APIKEY,STEAMID");
            }

            if (!File.Exists(LOG_FILE_PATH))
            {
                File.Create(LOG_FILE_PATH);
            }

            this._ISteamFriendsWatcher = _ISteamFriendsWatcher;
            _ISteamFriendsWatcher.InitializeUserSettings(File.ReadAllText(SETTINGS_FILE_PATH).Split(',')[0], File.ReadAllText(SETTINGS_FILE_PATH).Split(',')[1]);
            checkWorker.WorkerReportsProgress = true;
            checkWorker.DoWork += CheckWorker_DoWork;
            checkWorker.RunWorkerCompleted += CheckWorker_RunWorkerCompleted;
            checkWorker.ProgressChanged += CheckWorker_ProgressChanged;

            nameGeneratorWorker.DoWork += NameGeneratorWorker_DoWork;
            nameGeneratorWorker.WorkerReportsProgress = true;
            nameGeneratorWorker.ProgressChanged += NameGeneratorWorker_ProgressChanged;
            nameGeneratorWorker.RunWorkerCompleted += NameGeneratorWorker_RunWorkerCompleted;
        }

        private void CheckWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            AddMessageLine($"Checking friends of user: {GetNameFromSteamID(userSteamID)} SteamID: {userSteamID}");

            try
            {
                using (WebClient _WebClient = new WebClient())
                {
                    String oldFriendsJSON = null;

                    if (File.Exists(oldFriendsFilePath))
                    {
                        oldFriendsJSON = File.ReadAllText(oldFriendsFilePath);
                    }

                    String currentFriendsJSON = _WebClient.DownloadString($"http://api.steampowered.com/ISteamUser/GetFriendList/v0001/?key={apiKey}&steamid={userSteamID}&relationship=friend");
                    Directory.CreateDirectory(OLD_FRIENDS_FOLDER_PATH);
                    File.WriteAllText(oldFriendsFilePath, currentFriendsJSON);

                    currentFriends = GetFriendsFromJSON(currentFriendsJSON);
                    oldFriends = (oldFriendsJSON == null) ? currentFriends : GetFriendsFromJSON(oldFriendsJSON);

                    AddMessageLine($"Old Friends Count: {oldFriends.Count}");
                    AddMessageLine($"Current Friends Count: {currentFriends.Count}");

                    if (oldFriends.SequenceEqual<Friend>(currentFriends))
                    {
                        AddMessageLine("No changes to friends list.\n");
                    }
                    else
                    {
                        AddMessageLine("Friends list has changed!", "orange");

                        List<Friend> friendsAdded = new List<Friend>();
                        foreach (Friend curr in currentFriends)
                        {
                            if (!oldFriends.Contains(curr))
                            {
                                curr.name = GetNameFromSteamID(curr.steamid);
                                friendsAdded.Add(curr);
                            }
                        }
                        AddMessageLine($"\nFriends Added - {friendsAdded.Count}", "green");
                        PrintListOfFriends(friendsAdded);

                        List<Friend> friendsRemoved = new List<Friend>();
                        foreach (Friend curr in oldFriends)
                        {
                            if (!currentFriends.Contains(curr))
                            {
                                curr.name = GetNameFromSteamID(curr.steamid);
                                friendsRemoved.Add(curr);
                            }
                        }
                        AddMessageLine($"\nFriends Removed - {friendsRemoved.Count}", "red");
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
                        AddMessageLine($"Could not retreive friends list. Is your API key correct?\n", "red");
                    }
                    else if (code == HttpStatusCode.InternalServerError)
                    {
                        AddMessageLine($"Could not retreive friends list. Is your SteamID correct?\n", "red");
                    }
                    else
                    {
                        AddMessageLine($"Could not retreive friends list. {ex.Message}\n", "red");
                    }
                }
            }
            catch (Exception ex)
            {
                AddMessageLine($"Could not retreive friends list. {ex.Message}\n", "red");
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

        public void Check(String apiKey, String steamID, Boolean saveUserSettings = true)
        {
            if (saveUserSettings)
            {
                File.WriteAllText(SETTINGS_FILE_PATH, $"{apiKey},{steamID}");
            }

            this.apiKey = apiKey;
            this.userSteamID = steamID;
            oldFriendsFilePath = $"{OLD_FRIENDS_FOLDER_PATH}\\{userSteamID}.json";

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

        public List<Friend> GetUsersFriends()
        {
            return currentFriends;
        }

        public void GenerateAllFriendNames()
        {
            nameGeneratorWorker.RunWorkerAsync();
        }

        private void NameGeneratorWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            for (int i = 0; i < currentFriends.Count; i++)
            {
                nameGeneratorWorker.ReportProgress((int)(100 * ((float)(i + 1)) / (currentFriends.Count)));
                currentFriends[i].name = GetNameFromSteamID(currentFriends[i].steamid);
            }
        }

        private void NameGeneratorWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            _ISteamFriendsWatcher.UpdateProgress(e.ProgressPercentage);
        }

        private void NameGeneratorWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _ISteamFriendsWatcher.StopNameGeneration();
            namesGenerated = true;
        }

        public void PrintListOfFriends(List<Friend> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                AddMessageLine($"{i + 1} / {list.Count}: {list[i]}");
            }
        }

        public void AddMessageLine(String line, String color = "black")
        {
            if (!String.IsNullOrWhiteSpace(line))
            {
                File.AppendAllText(LOG_FILE_PATH, $"{DateTime.Now.ToString()} - {line}\n", System.Text.Encoding.UTF8);
            }

            Console.WriteLine(line);

            _ISteamFriendsWatcher.AddMessageLine(line, color);
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
        void StopNameGeneration();
    }
}
