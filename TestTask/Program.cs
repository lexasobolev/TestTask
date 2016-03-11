using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTask
{
    class Program
    {
        static void Main(string[] args)
        {
            Game.UpdatePlayers(new List<Player>()
            {
                new Player() {Id = 1,Name = "Deadpool" },
                new Player() {Id = 2,Name = "Thanos" },
                new Player() {Id = 3,Name = "Ultron" },
            });
            var players = Game.GetPlayers();
            Game.AddActivity(new Activity() { PlayerId = 1, ActivityType = ActivityType.Enter });
            Game.AddActivity(new Activity() { PlayerId = 1, ActivityType = ActivityType.Exit });
            Game.AddActivity(new Activity() { PlayerId = 1, ActivityType = ActivityType.GameProcess });
            Game.AddActivity(new Activity() { PlayerId = 2, ActivityType = ActivityType.Enter });
            Game.AddActivity(new Activity() { PlayerId = 2, ActivityType = ActivityType.Exit });

            var act = Game.GetActivities(2);
            act = Game.GetActivities(1);
            act = Game.GetActivities(3);

        }
    }
}
