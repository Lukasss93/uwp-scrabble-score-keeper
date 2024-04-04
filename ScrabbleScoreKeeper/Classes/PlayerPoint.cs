namespace ScrabbleScoreKeeper.Classes
{
    public class PlayerPoint
    {
        public Players player { get; set; }
        public int index { get; set; }

        public PlayerPoint(Players p, int i)
        {
            player = p;
            index = i;
        }
    }
}
