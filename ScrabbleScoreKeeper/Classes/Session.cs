using System.Runtime.Serialization;

namespace ScrabbleScoreKeeper.Classes
{
    [DataContract]
    public class Session
    {
        [DataMember]
        public Player Player1 { get; set; }
        [DataMember]
        public Player Player2 { get; set; }
        [DataMember]
        public Player Player3 { get; set; }
        [DataMember]
        public Player Player4 { get; set; }

        public Session()
        {
            Player1 = new Player("Player 1");
            Player2 = new Player("Player 2");
            Player3 = new Player("Player 3");
            Player4 = new Player("Player 4");
        }

    }
}
