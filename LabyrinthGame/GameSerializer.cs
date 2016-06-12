using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace LabyrinthGame
{
    [XmlRoot("Results")]
    public class GameSerializer
    {
        public List<Game> Games = new List<Game>();
    }
}
