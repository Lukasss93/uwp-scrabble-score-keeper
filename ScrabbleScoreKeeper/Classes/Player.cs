using System.Collections.Generic;
using System.Runtime.Serialization;
using Windows.UI;

namespace ScrabbleScoreKeeper.Classes
{
    [DataContract]
    public class Player
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public Color PlayerColor { get; set; }
        [DataMember]
        public List<int> Points { get; set; }

        public Player(string name)
        {
            this.Name = name;
            this.PlayerColor = Color.FromArgb(255, 11, 108, 248);
            this.Points = new List<int>();
        }
    }
}
