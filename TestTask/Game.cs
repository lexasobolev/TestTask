using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Serialization;

namespace TestTask
{
    public static class Game
    {
        static Dictionary<int, DateTime> activityFileReadTime = new Dictionary<int, DateTime>();
        public const int FileIOTimeout = 1000;


        static public List<Player> Players { get; private set; }
        public static void UpdatePlayers(List<Player> players)
        {
            while (true)
            {
                try
                {
                    using (StreamWriter sr = new StreamWriter("players.xml"))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(List<Player>));
                        serializer.Serialize(sr, players);
                    }
                    break;
                }
                catch (IOException)
                {
                    Console.WriteLine("Cannot read file players.xml ...");
                    Thread.Sleep(FileIOTimeout);
                }
            }
            Players = players;
        }
        public static List<Player> GetPlayers()
        {
            if (!File.Exists("players.xml"))
                return new List<Player>();
            while (true)
            {
                try
                {
                    using (StreamReader sr = new StreamReader("players.xml"))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(List<Player>));
                        Players = (List<Player>)serializer.Deserialize(sr);
                    }
                    return Players;
                }
                catch (IOException)
                {
                    Console.WriteLine("Cannot read file players.xml ...");
                    Thread.Sleep(FileIOTimeout);
                }
            }
        }

        static Activity ParceActivityCSVLine(string str)
        {
            string[] vals = str.Split('\t');
            return new Activity()
            {
                Time = DateTime.Parse(vals[0]),
                ActivityType = Utils.ParseEnum<ActivityType>(vals[1]),
            };
        }
        public static List<Activity> GetActivities(int playerId)
        {
            string fn = string.Format("activity_{0}.xml", playerId);
            Player player = Players.Where(x => x.Id == playerId).FirstOrDefault();
            if (player == null)
                throw new Exception("No player with id " + playerId + " found");
            FileInfo fi = new FileInfo(fn);
            if (!fi.Exists)
                return new List<Activity>();
            List<Activity> result = new List<Activity>();
            if (!activityFileReadTime.ContainsKey(playerId) || fi.LastWriteTimeUtc > activityFileReadTime[playerId])
            {
                while (true)
                {
                    try
                    {
                        using (StreamReader sr = new StreamReader(fn))
                        {
                            while (!sr.EndOfStream)
                            {
                                try
                                {
                                    Activity act = ParceActivityCSVLine(sr.ReadLine());
                                    act.PlayerId = playerId;
                                    player.Activities.Add(act);
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception("Wrong file format " + ex);
                                }
                            }
                            activityFileReadTime[playerId] = DateTime.Now;
                            return player.Activities;
                        }
                    }
                    catch (IOException)
                    {
                        Console.WriteLine("Cannot read file " + fn + " ...");
                        Thread.Sleep(FileIOTimeout);
                    }
                }
            }
            else
                return player.Activities;
        }
        public static void AddActivity(Activity activity)
        {
            if (Players == null || !Players.Any(x => x.Id == activity.PlayerId))
                throw new Exception("No player with id " + activity.PlayerId + " found");
            string fn = string.Format("activity_{0}.xml", activity.PlayerId);
            activity.Time = DateTime.Now;
            while (true)
            {
                try
                {
                    using (StreamWriter sr = new StreamWriter(fn, true))
                    {
                        sr.WriteLine(string.Join("\t", new string[]
                        {
                            activity.Time.ToString(),
                            activity.ActivityType.ToString()
                        }));
                    }
                    break;
                }
                catch (IOException)
                {
                    Console.WriteLine("Cannot write file " + fn + " ...");
                    Thread.Sleep(FileIOTimeout);
                }
            }
        }

    }
}
