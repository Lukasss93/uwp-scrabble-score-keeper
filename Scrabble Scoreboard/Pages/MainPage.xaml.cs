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
using Aura.Net.Pages;
using Aura.Net.Controls;
using Aura.Net.Common;
using System.Threading.Tasks;
using Aura.Net.Localization;

namespace Scrabble_Scoreboard.Pages
{
    public sealed partial class MainPage : Page
    {
        private static Enough.Async.AsyncLock asyncLock = new Enough.Async.AsyncLock();
        JsonSave save;
        ActionButton actionbutton = ActionButton.None;

        public MainPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
            titlebar.Margin = Utilities.SetMarginTop(titlebar, StatusBar.GetForCurrentView().OccludedRect.Height);

            GoogleAnalytics.EasyTracker.GetTracker().SendView("Mainpage.xaml.cs");

            UpdateActionButtonIcon();
            mybar_action.Click += Headeractionbutton_Click;
            
            p1_name.Click += (sender, e) => { EditName(Players.Player1); };
            p2_name.Click += (sender, e) => { EditName(Players.Player2); };
            p3_name.Click += (sender, e) => { EditName(Players.Player3); };
            p4_name.Click += (sender, e) => { EditName(Players.Player4); };

            p1_add.Click += (sender, e) => { AddPoints(Players.Player1); };
            p2_add.Click += (sender, e) => { AddPoints(Players.Player2); };
            p3_add.Click += (sender, e) => { AddPoints(Players.Player3); };
            p4_add.Click += (sender, e) => { AddPoints(Players.Player4); };

            mybar_clear.Click += Ab_clear_Click;
            mybar_help.Click += (s, e) => 
            {
                GoogleAnalytics.EasyTracker.GetTracker().SendEvent("Mainpage.xaml.cs", "mybar_help.Click", null, 0);
                Frame.Navigate(typeof(HowToWorks));
            };
            mybar_info.Click += (s, e) => 
            {
                GoogleAnalytics.EasyTracker.GetTracker().SendEvent("Mainpage.xaml.cs", "mybar_info.Click", null, 0);
                List<Changelog> changes = new List<Changelog>();
                changes.Add(new Changelog(
                    new Version(1, 0, 0, 0),
                    new List<string>()
                    {
                        Translate.Get("change_001")
                    }
                ));

                List<MyApps> myapp = new List<MyApps>();
                myapp.Add(new MyApps("04d27365-fe13-4f2a-85f7-222dec8c0392", new Uri("ms-appx:///Assets/app/mojang_service_statuses.png"), "Mojang Service Statuses"));
                myapp.Add(new MyApps("858ce4f2-86c1-41c7-8733-f2d0009add2d", new Uri("ms-appx:///Assets/app/cookie.png"), "Cookie Clicker"));
                myapp.Add(new MyApps("becfe05d-2aa6-48c4-ba9b-2b1817366f5a", new Uri("ms-appx:///Assets/app/files_locker.png"), "Files Locker"));
                myapp.Add(new MyApps("c57e88d1-8ff9-4249-945a-9aeac1e7af51", new Uri("ms-appx:///Assets/app/brt.png"), "BRT"));
                myapp.Add(new MyApps("2787dc44-ffae-4594-b1ca-a507c5748d80", new Uri("ms-appx:///Assets/app/suonerie.png"), "Ringtones++"));

                InformationOptions iopt = new InformationOptions();
                iopt.AboutMePage.Header = Translate.Get("aboutme");
                iopt.AboutMePage.Avatar = new Uri("ms-appx:///Assets/img/lukasss93.png");
                iopt.AboutMePage.FullName = "Luca Patera";
                iopt.AboutMePage.Nickname = "@Lukasss93";
                iopt.AboutMePage.Links = new List<Link>()
                {
                    new Link(Translate.Get("email"),"windowsphone@lucapatera.it",new Uri("mailto:windowsphone@lucapatera.it")),
                    new Link(Translate.Get("website"),"www.lucapatera.it",new Uri("http://www.lucapatera.it")),
                    new Link("facebook","www.facebook.com/Lukasss93Dev",new Uri("http://www.facebook.com/Lukasss93Dev")),
                    new Link("twitter","www.twitter.com/Lukasss93",new Uri("http://twitter.com/Lukasss93")),
                };

                iopt.ChangelogPage.Header = Translate.Get("changelog");
                iopt.ChangelogPage.AppLogo = new Uri("ms-appx:///Assets/img/logo.png");
                iopt.ChangelogPage.AppName = "Scrabble Score Keeper";
                iopt.ChangelogPage.Changes = changes;
                iopt.ChangelogPage.Current = Translate.Get("current");
                iopt.ChangelogPage.Rate = Translate.Get("rate");

                iopt.MyAppsPage.Header = Translate.Get("myapps");
                iopt.MyAppsPage.MyAppsList = myapp;

                iopt.ProPage.Header = Translate.Get("pro");
                iopt.ProPage.ProEnabled = false;

                string serializedopt = Json.Serialize(iopt);
                Debug.WriteLine(serializedopt);


                Frame.Navigate(typeof(Aura.Net.Pages.Information), serializedopt);
            };

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
                    mybar_action.Content = "";
                    break;
                case ActionButton.Edit:
                    mybar_action.Content = "";
                    break;
                case ActionButton.Delete:
                    mybar_action.Content = "";
                    break;
            }
        }
        
        private async void Ab_clear_Click(object sender, RoutedEventArgs e)
        {
            GoogleAnalytics.EasyTracker.GetTracker().SendEvent("Mainpage.xaml.cs", "Ab_clear_Click", null, 0);

            if(await MessageDialogHelper.Confirm(Translate.Get("clear_1")+"\n"+Translate.Get("clear_2"), Translate.Get("warning"),Translate.Get("ok"),Translate.Get("cancel")))
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
        
        private async void EditName(Players player)
        {
            using(await asyncLock.LockAsync())
            {
                JsonSavePlayer saveplayer = null;

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
        }

        private async void AddPoints(Players player)
        {
            using(await asyncLock.LockAsync())
            {
                string phrase = "";
                switch(player)
                {
                    case Players.Player1:
                        phrase = save.Player1.Name == "Player 1" ?
                            String.Format(Translate.Get("enter_score_player_generic"), "Player 1") :
                            String.Format(Translate.Get("enter_score_player_name"), save.Player1.Name);
                        break;
                    case Players.Player2:
                        phrase = save.Player2.Name == "Player 2" ?
                            String.Format(Translate.Get("enter_score_player_generic"), "Player 2") :
                            String.Format(Translate.Get("enter_score_player_name"), save.Player2.Name);
                        break;
                    case Players.Player3:
                        phrase = save.Player3.Name == "Player 3" ?
                            String.Format(Translate.Get("enter_score_player_generic"), "Player 3") :
                            String.Format(Translate.Get("enter_score_player_name"), save.Player3.Name);
                        break;
                    case Players.Player4:
                        phrase = save.Player4.Name == "Player 4" ?
                            String.Format(Translate.Get("enter_score_player_generic"), "Player 4") :
                            String.Format(Translate.Get("enter_score_player_name"), save.Player4.Name);
                        break;
                }
                
                var res = await MessageDialogHelper.DialogTextBox(phrase, Translate.Get("enter_score"), "", Translate.Get("ok"), Translate.Get("cancel"), null, null, InputScopeNameValue.NumberFullWidth);
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
                        ScrollToBottom();
                    }
                    else
                    {
                        MessageDialogHelper.Show(Translate.Get("allowed_int_num"), Translate.Get("warning"));
                    }
                }
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //Debug.WriteLine((string)SettingsHelper.Get("save"));
            GoogleAnalytics.EasyTracker.GetTracker().SendEvent("Mainpage.xaml.cs", "OnNavigatedTo", null, 0);

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

            //inizializza pulsante azione
            if(save.Player1.Points.Count+save.Player2.Points.Count+save.Player3.Points.Count+save.Player4.Points.Count==0)
            {
                actionbutton = ActionButton.None;
                UpdateActionButtonIcon();
            }

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

                        var res = await MessageDialogHelper.DialogTextBox(Translate.Get("edit_text"), Translate.Get("edit_title"), (string)thisbutton.Content,Translate.Get("ok"),Translate.Get("cancel"));
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
        
        private void ScrollToBottom()
        {
            myscroll.ChangeView(null, myscroll.ScrollableHeight, null, false);
            myscroll.UpdateLayout();
        }
    }
}
