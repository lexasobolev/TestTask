using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;



namespace TestTask
{
    public enum ActivityType
    {
        Enter,
        GameProcess,
        Exit
    }
    public class Activity
    {
        public int PlayerId;
        public ActivityType ActivityType;
        public DateTime Time;
    }
}
