using System.Collections.Generic;
using Aura.Globalization;
using ScrabbleScoreKeeper.Classes;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace ScrabbleScoreKeeper.Dialogs
{
    public sealed partial class EditPlayer : ContentDialog
    {
        public bool Result = false;
        public string PlayerName;
        public Color PlayerColor;
        private Players playert;

        private List<AppColors> colors = new List<AppColors>()
        {
            new AppColors(new SolidColorBrush(Color.FromArgb(255, 11, 108, 248)), LocalizedString.Get("color_blue")),
            new AppColors(new SolidColorBrush(Color.FromArgb(255, 201, 0, 0)), LocalizedString.Get("color_red")),
            new AppColors(new SolidColorBrush(Color.FromArgb(255, 0, 183, 3)), LocalizedString.Get("color_green")),
            new AppColors(new SolidColorBrush(Color.FromArgb(255, 221, 188, 0)), LocalizedString.Get("color_yellow")),
        };

        public EditPlayer(Players player, string playername, Color playercolor)
        {
            this.InitializeComponent();
            this.Title = LocalizedString.Get("edit_player");
            this.PrimaryButtonText = LocalizedString.Get("save");
            this.SecondaryButtonText = LocalizedString.Get("cancel");
            this.PrimaryButtonClick += EditPlayer_PrimaryButtonClick;
            this.SecondaryButtonClick += (s, e) => { Result = false; };

            name.Header = LocalizedString.Get("name");
            color.Header = LocalizedString.Get("color");
            reset.Content = LocalizedString.Get("reset");
            reset.Click += Reset_Click;

            playert = player;
            name.Text = playername;
            color.ItemsSource = colors;

            if(playercolor.Equals(colors[0].Color.Color))
            {
                color.SelectedIndex = 0;
            }
            else if(playercolor.Equals(colors[1].Color.Color))
            {
                color.SelectedIndex = 1;
            }
            else if(playercolor.Equals(colors[2].Color.Color))
            {
                color.SelectedIndex = 2;
            }
            else if(playercolor.Equals(colors[3].Color.Color))
            {
                color.SelectedIndex = 3;
            }

            name.GotFocus += (s, e) => { name.SelectAll(); };
        }

        private void Reset_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            switch(playert)
            {
                case Players.Player1:
                    name.Text = "Player 1";
                    break;
                case Players.Player2:
                    name.Text = "Player 2";
                    break;
                case Players.Player3:
                    name.Text = "Player 3";
                    break;
                case Players.Player4:
                    name.Text = "Player 4";
                    break;
            }

            color.SelectedIndex = 0;
        }

        private void EditPlayer_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Result = true;
            PlayerName = name.Text;
            PlayerColor = ((AppColors)color.SelectedItem).Color.Color;
        }
    }
}
