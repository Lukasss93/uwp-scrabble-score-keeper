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

        public MessageDialogPlayer(string player_num)
        {
            this.InitializeComponent();
            Result = new MessageDialogPlayerResult();
            Result.status = false;

            name.Header = "Inserisci un nome per il Player " + player_num;
            color.Header = "Scegli un colore per il Player " + player_num;

            List<AppColors> colors = new List<AppColors>();
            colors.Add(new AppColors(new SolidColorBrush(Color.FromArgb(255, 11, 108, 248)), "Blu"));
            colors.Add(new AppColors(new SolidColorBrush(Color.FromArgb(255, 201, 0, 0)), "Rosso"));
            colors.Add(new AppColors(new SolidColorBrush(Color.FromArgb(255, 0, 183, 3)), "Verde"));
            colors.Add(new AppColors(new SolidColorBrush(Color.FromArgb(255, 221, 188, 0)), "Giallo"));

            color.ItemsSource = colors;
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
