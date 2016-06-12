using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace LabyrinthGame
{
    public class Game
    {
        public string PlayerName { get; set; }
        public int CoinsCount { get; set; }
        [XmlIgnore]
        public string TimeString { get { return (DateTime.Now.TimeOfDay - startTime.TimeOfDay + oldStartTime).ToString(@"mm\:ss"); } }
        [XmlIgnore]
        public string DateString { get { return startTime.ToString(@"dd\.MM\.yyyy"); } }
        public string Time { get; set; }
        public string Date { get; set; }
        public string EndingReason { get; set; }
        private TimeSpan oldStartTime = new TimeSpan();
        private DateTime startTime { get; set; }

        public Game() { }

        public Game(string playerName)
        {
            PlayerName = playerName;
            startTime = DateTime.Now;
        }

        public void StartTimer()
        {
            startTime = DateTime.Now;
        }

        public void PauseTimer()
        {
            oldStartTime = DateTime.Now - startTime + oldStartTime;   
        }
    }
}
