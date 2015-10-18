﻿using System;
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

namespace Scrabble_Scoreboard.Pages
{
    public sealed partial class MainPage : Page
    {
        JsonSave save;
        ActionButton actionbutton = ActionButton.None;

        public MainPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
            titlebar.Margin = Utilities.SetMarginTop(titlebar, StatusBar.GetForCurrentView().OccludedRect.Height);

            UpdateActionButtonIcon();
            headeractionbutton.Click += Headeractionbutton_Click;

            p1_name.Click += (sender, e) => { EditName(Players.Player1); };
            p2_name.Click += (sender, e) => { EditName(Players.Player2); };
            p3_name.Click += (sender, e) => { EditName(Players.Player3); };
            p4_name.Click += (sender, e) => { EditName(Players.Player4); };

            p1_add.Click += (sender, e) => { AddPoints(Players.Player1); };
            p2_add.Click += (sender, e) => { AddPoints(Players.Player2); };
            p3_add.Click += (sender, e) => { AddPoints(Players.Player3); };
            p4_add.Click += (sender, e) => { AddPoints(Players.Player4); };

            ab_clear.Click += Ab_clear_Click;
            ab_help.Click += (s, e) => { Frame.Navigate(typeof(HowToWorks)); };
            
        }

        private void Headeractionbutton_Click(object sender, RoutedEventArgs e)
        {
            if(actionbutton==ActionButton.None)
            {
                actionbutton = ActionButton.Edit;
            }
            else if(actionbutton==ActionButton.Edit)
            {
                actionbutton = ActionButton.Delete;
            }
            else if(actionbutton==ActionButton.Delete)
            {
                actionbutton = ActionButton.None;
            }
            UpdateActionButtonIcon();
        }

        private void UpdateActionButtonIcon()
        {
            switch(actionbutton)
            {
                case ActionButton.None:
                    headeractionbutton.Content = "";
                    break;
                case ActionButton.Edit:
                    headeractionbutton.Content = "";
                    break;
                case ActionButton.Delete:
                    headeractionbutton.Content = "";
                    break;
            }
        }

        #region AZIONI COMMAND BAR
        private async void Ab_clear_Click(object sender, RoutedEventArgs e)
        {
            if(await MessageDialogHelper.Confirm("Sicuro di voler pulire tutto?\nTutti i salvataggi verranno eliminati.", "Attenzione"))
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
        }
        #endregion
        
        private async void EditName(Players player)
        {
            JsonSavePlayer saveplayer=null;
            
            switch(player)
            {
                case Players.Player1: saveplayer = save.Player1; break;
                case Players.Player2: saveplayer = save.Player2; break;
                case Players.Player3: saveplayer = save.Player3; break;
                case Players.Player4: saveplayer = save.Player4; break;
            }

            MessageDialogPlayer dialog = new MessageDialogPlayer(player, saveplayer);
            await dialog.ShowAsync();

            if(dialog.Result.status)
            {
                switch(player)
                {
                    case Players.Player1: save.Player1.Name = dialog.Result.name; save.Player1.PlayerColor = dialog.Result.color; break;
                    case Players.Player2: save.Player2.Name = dialog.Result.name; save.Player2.PlayerColor = dialog.Result.color; break;
                    case Players.Player3: save.Player3.Name = dialog.Result.name; save.Player3.PlayerColor = dialog.Result.color; break;
                    case Players.Player4: save.Player4.Name = dialog.Result.name; save.Player4.PlayerColor = dialog.Result.color; break;
                }

                Save();
                Update();
            }
        }

        private async void AddPoints(Players player)
        {
            string player_name="";
            switch(player)
            {
                case Players.Player1: player_name = save.Player1.Name == "Player 1" ? "il Player 1" : save.Player1.Name; break;
                case Players.Player2: player_name = save.Player2.Name == "Player 2" ? "il Player 2" : save.Player2.Name; break;
                case Players.Player3: player_name = save.Player3.Name == "Player 3" ? "il Player 3" : save.Player3.Name; break;
                case Players.Player4: player_name = save.Player4.Name == "Player 4" ? "il Player 4" : save.Player4.Name; break;
            }

            var res = await MessageDialogHelper.DialogTextBox("Inserisci il punteggio per " + player_name, "Inserisci punteggio","","ok","annulla",null,null,InputScopeNameValue.NumberFullWidth);
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

            //imposta i colori
            p1_name.Background = new SolidColorBrush(save.Player1.PlayerColor);
            p2_name.Background = new SolidColorBrush(save.Player2.PlayerColor);
            p3_name.Background = new SolidColorBrush(save.Player3.PlayerColor);
            p4_name.Background = new SolidColorBrush(save.Player4.PlayerColor);

            p1_tot_grid.Background = new SolidColorBrush(save.Player1.PlayerColor);
            p2_tot_grid.Background = new SolidColorBrush(save.Player2.PlayerColor);
            p3_tot_grid.Background = new SolidColorBrush(save.Player3.PlayerColor);
            p4_tot_grid.Background = new SolidColorBrush(save.Player4.PlayerColor);
            
            //imposta i punti ai player e fai la somma
            int somma1 = 0;
            int somma2 = 0;
            int somma3 = 0;
            int somma4 = 0;

            p1_stack.Children.Clear();
            int index = 0;
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
                button.Tag = new PlayerPoint(Players.Player1, index);
                button.Click += PointButton;

                
                p1_stack.Children.Add(button);

                somma1 += point;
                index++;             
            }

            p2_stack.Children.Clear();
            index = 0;
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
                button.Tag = new PlayerPoint(Players.Player2, index);
                button.Click += PointButton;

                p2_stack.Children.Add(button);                

                somma2 += point;
                index++;           
            }

            p3_stack.Children.Clear();
            index = 0;
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
                button.Tag = new PlayerPoint(Players.Player3, index);
                button.Click += PointButton;

                p3_stack.Children.Add(button);

                somma3 += point;
                index++;               
            }

            p4_stack.Children.Clear();
            index = 0;
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
                button.Tag = new PlayerPoint(Players.Player4, index);
                button.Click += PointButton;

                p4_stack.Children.Add(button);

                somma4 += point;
                index++;
            }

            //aggiorna somma se vuota
            p1_tot.Text = somma1 + "";
            p2_tot.Text = somma2 + "";
            p3_tot.Text = somma3 + "";
            p4_tot.Text = somma4 + "";

            //mostra/nascondi pulsante modifica/elimina
            if(save.Player1.Points.Count+save.Player2.Points.Count+save.Player3.Points.Count+save.Player4.Points.Count==0)
            {
                headeractionbutton.Visibility = Visibility.Collapsed;
                actionbutton = ActionButton.None;
                UpdateActionButtonIcon();
            }
            else
            {
                headeractionbutton.Visibility = Visibility.Visible;
            }

            //scrollo fino alla fine
            myscroll.ChangeView(0.0f, myscroll.ExtentHeight, 1.0f);
            myscroll.UpdateLayout();

        }

        private async void PointButton(object sender, RoutedEventArgs e)
        {
            if(actionbutton != ActionButton.None)
            {
                Button thisbutton = (sender as Button);
                PlayerPoint playerpoint = (PlayerPoint)thisbutton.Tag;

                switch(actionbutton)
                {
                    case ActionButton.Edit:

                        var res = await MessageDialogHelper.DialogTextBox("Inserisci il nuovo valore", "Modifica valore", (string)thisbutton.Content);
                        if(res.result)
                        {
                            int value;
                            if(int.TryParse(res.output, out value))
                            {
                                switch(playerpoint.player)
                                {
                                    case Players.Player1:
                                        save.Player1.Points[playerpoint.index] = value;
                                        break;
                                    case Players.Player2:
                                        save.Player2.Points[playerpoint.index] = value;
                                        break;
                                    case Players.Player3:
                                        save.Player3.Points[playerpoint.index] = value;
                                        break;
                                    case Players.Player4:
                                        save.Player4.Points[playerpoint.index] = value;
                                        break;
                                }
                            }
                        }

                        break;

                    case ActionButton.Delete:

                        switch(playerpoint.player)
                        {
                            case Players.Player1:
                                save.Player1.Points.RemoveAt(playerpoint.index);
                                break;
                            case Players.Player2:
                                save.Player2.Points.RemoveAt(playerpoint.index);
                                break;
                            case Players.Player3:
                                save.Player3.Points.RemoveAt(playerpoint.index);
                                break;
                            case Players.Player4:
                                save.Player4.Points.RemoveAt(playerpoint.index);
                                break;
                        }

                        break;
                }

                Save();
                Update();
            }
        }
    }
}