using System;
using System.Linq;
using System.Text;
using Aura.Serializer;
using Aura.Storage;
using Windows.UI;

namespace ScrabbleScoreKeeper.Classes
{
    public enum Players
    {
        Player1,
        Player2,
        Player3,
        Player4
    }

    public class Scoreboard
    {
        
        public delegate void ScoreboadChangeHandler(object sender, EventArgs e);
        public event ScoreboadChangeHandler ScoreboardChange;

        private Session session;
        public Session ScoreSession
        {
            get { return session; }
            private set { session = value; }
        }

        public Scoreboard()
        {
            AppSettings.Initialize("save", Json.Serialize(new Session()));
            ScoreSession = Json.Deserialize<Session>((string)AppSettings.Get("save"));
        }

        /// <summary>
        /// Aggiunge punti al giocatore selezionato
        /// </summary>
        /// <param name="player">Giocatore</param>
        /// <param name="points">Punti</param>
        public void AddPoints(Players player, int points)
        {
            switch(player)
            {
                case Players.Player1:
                    ScoreSession.Player1.Points.Add(points);
                    break;
                case Players.Player2:
                    ScoreSession.Player2.Points.Add(points);
                    break;
                case Players.Player3:
                    ScoreSession.Player3.Points.Add(points);
                    break;
                case Players.Player4:
                    ScoreSession.Player4.Points.Add(points);
                    break;
            }

            Save();
        }

        /// <summary>
        /// Modifica punti al giocatore selezionato
        /// </summary>
        /// <param name="player">Giocatore</param>
        /// <param name="index">Indice dei punti da modificare</param>
        /// <param name="newpoints">Punti da inserire</param>
        public void EditPoints(Players player, int index, int newpoints)
        {
            switch(player)
            {
                case Players.Player1:
                    ScoreSession.Player1.Points[index] = newpoints;
                    break;
                case Players.Player2:
                    ScoreSession.Player2.Points[index] = newpoints;
                    break;
                case Players.Player3:
                    ScoreSession.Player3.Points[index] = newpoints;
                    break;
                case Players.Player4:
                    ScoreSession.Player4.Points[index] = newpoints;
                    break;
            }

            Save();
        }

        /// <summary>
        /// Elimina punti al giocatore selezionato
        /// </summary>
        /// <param name="player">Giocatore</param>
        /// <param name="index">Indice dei punti da eliminare</param>
        public void DeletePoints(Players player, int index)
        {
            switch(player)
            {
                case Players.Player1:
                    ScoreSession.Player1.Points.RemoveAt(index);
                    break;
                case Players.Player2:
                    ScoreSession.Player2.Points.RemoveAt(index);
                    break;
                case Players.Player3:
                    ScoreSession.Player3.Points.RemoveAt(index);
                    break;
                case Players.Player4:
                    ScoreSession.Player4.Points.RemoveAt(index);
                    break;
            }

            Save();
        }

        /// <summary>
        /// Modifica il nome e il colore del giocatore
        /// </summary>
        /// <param name="player">Giocatore</param>
        /// <param name="name">Nome</param>
        /// <param name="color">Colore</param>
        public void EditPlayer(Players player, string name, Color color)
        {
            switch(player)
            {
                case Players.Player1:
                    ScoreSession.Player1.Name = name;
                    ScoreSession.Player1.PlayerColor = color;
                    break;
                case Players.Player2:
                    ScoreSession.Player2.Name = name;
                    ScoreSession.Player2.PlayerColor = color;
                    break;
                case Players.Player3:
                    ScoreSession.Player3.Name = name;
                    ScoreSession.Player3.PlayerColor = color;
                    break;
                case Players.Player4:
                    ScoreSession.Player4.Name = name;
                    ScoreSession.Player4.PlayerColor = color;
                    break;
            }

            Save();
        }

        /// <summary>
        /// Salva la sessione nelle impostazioni
        /// </summary>
        private void Save()
        {
            AppSettings.Set("save", Json.Serialize(ScoreSession));
            ScoreboardChange?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Pulisce la sessione
        /// </summary>
        public void Clear()
        {
            ScoreSession = new Session();
            Save();
        }

        /// <summary>
        /// Ottiene il nome del giocatore
        /// </summary>
        /// <param name="player">giocatore</param>
        /// <returns>nome del giocatore</returns>
        public string GetPlayerName(Players player)
        {
            switch(player)
            {
                case Players.Player1: return ScoreSession.Player1.Name;
                case Players.Player2: return ScoreSession.Player2.Name;
                case Players.Player3: return ScoreSession.Player3.Name;
                case Players.Player4: return ScoreSession.Player4.Name;
                default: return "404";
            }
        }

        /// <summary>
        /// Ottiene il colore del giocatore
        /// </summary>
        /// <param name="player">giocatore</param>
        /// <returns>colore</returns>
        public Color GetPlayerColor(Players player)
        {
            switch(player)
            {
                case Players.Player1: return ScoreSession.Player1.PlayerColor;
                case Players.Player2: return ScoreSession.Player2.PlayerColor;
                case Players.Player3: return ScoreSession.Player3.PlayerColor;
                case Players.Player4: return ScoreSession.Player4.PlayerColor;
                default: return Colors.Black;
            }
        }

        public override string ToString()
        {
            StringBuilder output = new StringBuilder();
            output.AppendLine("------Scoreboard ToString()------");
            output.AppendLine("Player1:");
            output.AppendFormat("Nome: {0}\n",ScoreSession.Player1.Name);
            output.AppendFormat("Colore: {0}\n", ColorToRGBString(ScoreSession.Player1.PlayerColor));
            output.AppendFormat("Punti: {0}\n", string.Join(",", ScoreSession.Player1.Points));
            output.AppendLine("----------------------------------");
            output.AppendLine("Player2:");
            output.AppendFormat("Nome: {0}\n", ScoreSession.Player2.Name);
            output.AppendFormat("Colore: {0}\n", ColorToRGBString(ScoreSession.Player2.PlayerColor));
            output.AppendFormat("Punti: {0}\n", string.Join(",", ScoreSession.Player2.Points));
            output.AppendLine("----------------------------------");
            output.AppendLine("Player3:");
            output.AppendFormat("Nome: {0}\n", ScoreSession.Player3.Name);
            output.AppendFormat("Colore: {0}\n", ColorToRGBString(ScoreSession.Player3.PlayerColor));
            output.AppendFormat("Punti: {0}\n", string.Join(",", ScoreSession.Player3.Points));
            output.AppendLine("----------------------------------");
            output.AppendLine("Player4:");
            output.AppendFormat("Nome: {0}\n", ScoreSession.Player4.Name);
            output.AppendFormat("Colore: {0}\n", ColorToRGBString(ScoreSession.Player4.PlayerColor));
            output.AppendFormat("Punti: {0}\n", string.Join(",", ScoreSession.Player4.Points));
            output.AppendLine("----------------------------------");

            return output.ToString();
        }

        private string ColorToRGBString(Color color)
        {
            return color.R.ToString() + "," + color.G.ToString() + "," + color.B.ToString();
        }
    }
}
