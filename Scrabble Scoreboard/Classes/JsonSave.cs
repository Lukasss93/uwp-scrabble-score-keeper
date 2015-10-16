﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Scrabble_Scoreboard.Classes
{

    [DataContract]
    public class JsonSave
    {
        [DataMember]
        public JsonSavePlayer Player1 { get; set; }
        [DataMember]
        public JsonSavePlayer Player2 { get; set; }
        [DataMember]
        public JsonSavePlayer Player3 { get; set; }
        [DataMember]
        public JsonSavePlayer Player4 { get; set; }

        public JsonSave()
        {
            Player1 = new JsonSavePlayer();
            Player2 = new JsonSavePlayer();
            Player3 = new JsonSavePlayer();
            Player4 = new JsonSavePlayer();
        }
        
    }

    [DataContract]
    public class JsonSavePlayer
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public List<int> Points { get; set; }

        public JsonSavePlayer()
        {
            Name = null;
            Points = new List<int>();
        }
    }
}