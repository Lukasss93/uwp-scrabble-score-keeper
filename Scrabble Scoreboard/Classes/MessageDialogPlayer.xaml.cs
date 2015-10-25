using Aura.Net.Localization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers.Provider;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Il modello di elemento per la finestra di dialogo del contenuto è documentato all'indirizzo http://go.microsoft.com/fwlink/?LinkID=390556

namespace Scrabble_Scoreboard.Classes
{
    public sealed partial class MessageDialogPlayer : ContentDialog
    {
        public MessageDialogPlayerResult Result { get; private set; }
        private Players player;
        private List<AppColors> colors = new List<AppColors>()
        {
            new AppColors(new SolidColorBrush(Color.FromArgb(255, 11, 108, 248)), Translate.Get("color_blue")),
            new AppColors(new SolidColorBrush(Color.FromArgb(255, 201, 0, 0)), Translate.Get("color_red")),
            new AppColors(new SolidColorBrush(Color.FromArgb(255, 0, 183, 3)), Translate.Get("color_green")),
            new AppColors(new SolidColorBrush(Color.FromArgb(255, 221, 188, 0)), Translate.Get("color_yellow")),
        };

        public MessageDialogPlayer(Players p, JsonSavePlayer saveplayer)
        {
            this.InitializeComponent();
            Result = new MessageDialogPlayerResult();
            Result.status = false;

            this.Title = Translate.Get("enter_name_title");
            this.PrimaryButtonText = Translate.Get("ok");
            this.SecondaryButtonText = Translate.Get("cancel");

            reset.Content = Translate.Get("reset");
            reset.Click += Reset_Click;

            player = p;


            switch(player)
            {
                case Players.Player1:
                    name.Header = String.Format(Translate.Get("enter_name"), "Player 1");
                    color.Header = String.Format(Translate.Get("enter_color"), "Player 1");
                    break;
                case Players.Player2:
                    name.Header = String.Format(Translate.Get("enter_name"), "Player 2");
                    color.Header = String.Format(Translate.Get("enter_color"), "Player 2");
                    break;
                case Players.Player3:
                    name.Header = String.Format(Translate.Get("enter_name"), "Player 3");
                    color.Header = String.Format(Translate.Get("enter_color"), "Player 3");
                    break;
                case Players.Player4:
                    name.Header = String.Format(Translate.Get("enter_name"), "Player 4");
                    color.Header = String.Format(Translate.Get("enter_color"), "Player 4");
                    break;
            }

            name.Text = saveplayer.Name;            
            color.ItemsSource = colors;


            if(saveplayer.PlayerColor.Equals(Color.FromArgb(255, 11, 108, 248)))
            {
                color.SelectedIndex = 0;
            }
            else if(saveplayer.PlayerColor.Equals(Color.FromArgb(255, 201, 0, 0)))
            {
                color.SelectedIndex = 1;
            }
            else if(saveplayer.PlayerColor.Equals(Color.FromArgb(255, 0, 183, 3)))
            {
                color.SelectedIndex = 2;
            }
            else if(saveplayer.PlayerColor.Equals(Color.FromArgb(255, 221, 188, 0)))
            {
                color.SelectedIndex = 3;
            }

            name.GotFocus += (s, e) => { name.SelectAll(); };
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            switch(player)
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

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Result.status = true;
            Result.name = name.Text;
            Result.color = ((AppColors)color.SelectedItem).Color.Color;
        }
    }

    public class MessageDialogPlayerResult
    {
        public bool status { get; set; }
        public string name { get; set; }
        public Color color { get; set; }
    }
}
