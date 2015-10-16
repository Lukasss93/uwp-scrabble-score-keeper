using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Aura.Net.Extensions;
using Aura.Net;
using Windows.UI.Text;
using System.Diagnostics;
using Aura.Net.Storage;
using Scrabble_Scoreboard.Classes;
using Aura.Net.Serializer;

namespace Scrabble_Scoreboard
{
    public sealed partial class MainPage : Page
    {
        private enum Players { Player1, Player2, Player3, Player4 }
        JsonSave save;

        public MainPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
            titlebar.Margin = Utilities.SetMarginTop(titlebar, StatusBar.GetForCurrentView().OccludedRect.Height);

            p1_name.Click += (sender, e) => { EditName(Players.Player1); };
            p2_name.Click += (sender, e) => { EditName(Players.Player2); };
            p3_name.Click += (sender, e) => { EditName(Players.Player3); };
            p4_name.Click += (sender, e) => { EditName(Players.Player4); };

            p1_add.Click += (sender, e) => { AddPoints(Players.Player1); };
            p2_add.Click += (sender, e) => { AddPoints(Players.Player2); };
            p3_add.Click += (sender, e) => { AddPoints(Players.Player3); };
            p4_add.Click += (sender, e) => { AddPoints(Players.Player4); };

            ab_clear.Click += Ab_clear_Click;
        }

        #region AZIONI COMMAND BAR
        private void Ab_clear_Click(object sender, RoutedEventArgs e)
        {
            JsonSave newsave = new JsonSave();
            newsave.Player1.Name = "Player 1";
            newsave.Player2.Name = "Player 2";
            newsave.Player3.Name = "Player 3";
            newsave.Player4.Name = "Player 4";
            SettingsHelper.Set("save", Json.Serialize(newsave));

            save = Json.Deserialize<JsonSave>((string)SettingsHelper.Get("save"));

            Update();
        }
        #endregion
        
        private async void EditName(Players player)
        {
            string player_num = "1";
            switch(player)
            {
                case Players.Player1: player_num = "1"; break;
                case Players.Player2: player_num = "2"; break;
                case Players.Player3: player_num = "3"; break;
                case Players.Player4: player_num = "4"; break;
            }

            var res = await MessageDialogHelper.DialogTextBox("Inserisci il nome per il Player "+ player_num, "Inserisci nome");
            if(res.result)
            {
                switch(player)
                {
                    case Players.Player1: save.Player1.Name = res.output; break;
                    case Players.Player2: save.Player2.Name = res.output; break;
                    case Players.Player3: save.Player3.Name = res.output; break;
                    case Players.Player4: save.Player4.Name = res.output; break;
                }

                Save();
                Update();
            }
        }

        private async void AddPoints(Players player)
        {
            string player_num = "1";
            switch(player)
            {
                case Players.Player1: player_num = "1"; break;
                case Players.Player2: player_num = "2"; break;
                case Players.Player3: player_num = "3"; break;
                case Players.Player4: player_num = "4"; break;
            }

            var res = await MessageDialogHelper.DialogTextBox("Inserisci il punteggio per il Player " + player_num, "Inserisci punteggio","","ok","annulla",null,null,InputScopeNameValue.NumberFullWidth);
            if(res.result)
            {
                int value;
                if(int.TryParse(res.output, out value))
                {
                    switch(player)
                    {
                        case Players.Player1: save.Player1.Points.Add(value); break;
                        case Players.Player2: save.Player2.Points.Add(value); break;
                        case Players.Player3: save.Player3.Points.Add(value); break;
                        case Players.Player4: save.Player4.Points.Add(value); break;
                    }
                    Save();
                    Update();
                }
                else
                {
                    MessageDialogHelper.Show("Sono consentiti solo numeri interi.", "Attenzione");
                }
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Debug.WriteLine((string)SettingsHelper.Get("save"));

            save = Json.Deserialize<JsonSave>((string)SettingsHelper.Get("save"));
            Update();
        }
        
        private void Save()
        {
            SettingsHelper.Set("save",Json.Serialize(save));
        }

        private void Update()
        {
            //imposta i nomi ai player
            p1_name.Content = save.Player1.Name;
            p2_name.Content = save.Player2.Name;
            p3_name.Content = save.Player3.Name;
            p4_name.Content = save.Player4.Name;

            //imposta i punti ai player e fai la somma
            int somma1 = 0;
            int somma2 = 0;
            int somma3 = 0;
            int somma4 = 0;

            p1_stack.Children.Clear();
            foreach(int point in save.Player1.Points)
            {
                Button button = new Button();
                button.Content = "" + point;
                button.FontWeight = FontWeights.Bold;
                button.Style = (Style)Resources["PlayerButton"];
                button.Margin = new Thickness(0, 0, 1, 1);
                button.HorizontalAlignment = HorizontalAlignment.Stretch;
                button.VerticalAlignment = VerticalAlignment.Stretch;
                button.Height = 40;
                button.MinHeight = 30;
                button.MinWidth = 90;
                
                p1_stack.Children.Add(button);
                myscroll.ChangeView(0.0f, double.MaxValue, 1.0f);

                somma1 += point;                
            }

            p2_stack.Children.Clear();
            foreach(int point in save.Player2.Points)
            {
                Button button = new Button();
                button.Content = "" + point;
                button.FontWeight = FontWeights.Bold;
                button.Style = (Style)Resources["PlayerButton"];
                button.Margin = new Thickness(0, 0, 1, 1);
                button.HorizontalAlignment = HorizontalAlignment.Stretch;
                button.VerticalAlignment = VerticalAlignment.Stretch;
                button.Height = 40;
                button.MinHeight = 30;
                button.MinWidth = 90;

                p2_stack.Children.Add(button);
                myscroll.ChangeView(0.0f, double.MaxValue, 1.0f);

                somma2 += point;                
            }

            p3_stack.Children.Clear();
            foreach(int point in save.Player3.Points)
            {
                Button button = new Button();
                button.Content = "" + point;
                button.FontWeight = FontWeights.Bold;
                button.Style = (Style)Resources["PlayerButton"];
                button.Margin = new Thickness(0, 0, 1, 1);
                button.HorizontalAlignment = HorizontalAlignment.Stretch;
                button.VerticalAlignment = VerticalAlignment.Stretch;
                button.Height = 40;
                button.MinHeight = 30;
                button.MinWidth = 90;

                p3_stack.Children.Add(button);
                myscroll.ChangeView(0.0f, double.MaxValue, 1.0f);

                somma3 += point;                
            }

            p4_stack.Children.Clear();
            foreach(int point in save.Player4.Points)
            {
                Button button = new Button();
                button.Content = "" + point;
                button.FontWeight = FontWeights.Bold;
                button.Style = (Style)Resources["PlayerButton"];
                button.Margin = new Thickness(0, 0, 0, 1);
                button.HorizontalAlignment = HorizontalAlignment.Stretch;
                button.VerticalAlignment = VerticalAlignment.Stretch;
                button.Height = 40;
                button.MinHeight = 30;
                button.MinWidth = 90;

                p4_stack.Children.Add(button);
                myscroll.ChangeView(0.0f, double.MaxValue, 1.0f);

                somma4 += point;                
            }

            //aggiorna somma se vuota
            p1_tot.Text = somma1 + "";
            p2_tot.Text = somma2 + "";
            p3_tot.Text = somma3 + "";
            p4_tot.Text = somma4 + "";

        }
    }
}
