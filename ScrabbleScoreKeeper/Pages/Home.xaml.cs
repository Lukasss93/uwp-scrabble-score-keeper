using Aura.Display;
using Aura.Globalization;
using Aura.Serializer;
using ScrabbleScoreKeeper.Classes;
using ScrabbleScoreKeeper.Dialogs;
using System;
using Windows.Foundation.Metadata;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Enough.Async;
using GoogleAnalytics;

namespace ScrabbleScoreKeeper.Pages
{
    public sealed partial class Home : Page
    {
        private static AsyncLock asyncLock = new AsyncLock();
        private Scoreboard score = new Scoreboard();

        public Home()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
            Window.Current.VisibilityChanged += Current_VisibilityChanged;
            Window.Current.Activated += (s, e) => { SetTitleBarIfPresent(); };
            SetTitleBarIfPresent();

            //traduci
            cb_info.Label = LocalizedString.Get("info");
            cb_edit.Label = LocalizedString.Get("edit");
            cb_delete.Label = LocalizedString.Get("delete");
            cb_clear.Label = LocalizedString.Get("clear");

            //evento al cambiare della sessione
            score.ScoreboardChange += (s, e) => { Update(); };

            //azioni bottoni modifica nomi giocatori
            p1_name.Click += (s, e) => { EditPlayerName(Players.Player1); };
            p2_name.Click += (s, e) => { EditPlayerName(Players.Player2); };
            p3_name.Click += (s, e) => { EditPlayerName(Players.Player3); };
            p4_name.Click += (s, e) => { EditPlayerName(Players.Player4); };

            //azioni bottoni aggiungi punti
            p1_add.Click += (s, e) => { AddPoints(Players.Player1); };
            p2_add.Click += (s, e) => { AddPoints(Players.Player2); };
            p3_add.Click += (s, e) => { AddPoints(Players.Player3); };
            p4_add.Click += (s, e) => { AddPoints(Players.Player4); };

            //trasformo le appbartogglebutton in radiobutton
            cb_edit.Checked += (s, e) => { cb_delete.IsChecked = false; };
            cb_delete.Checked += (s, e) => { cb_edit.IsChecked = false; };

            //pulisco sessione
            cb_clear.Click += ClearSession;

            //pulsanti impostazioni, informazioni
            cb_settings.Click += (s, e) => 
            {
                App.analytics.Send(HitBuilder.CreateCustomEvent("click pulsante", "impostazioni").Build());
                Frame.Navigate(typeof(Settings));
            };
            cb_info.Click += (s, e) => 
            {
                App.analytics.Send(HitBuilder.CreateCustomEvent("click pulsante", "informazioni").Build());
                Frame.Navigate(typeof(Lukasss93.Pages.Information), Json.Serialize(MyConstants.GenerateParameter()));
            };

            //miglioro lo scroll nella board
            BOARD.SizeChanged += (s, e) => { ScrollToBottom(); };

            //aggiorno l'interfaccia
            Update();
        }

        private async void EditPlayerName(Players player)
        {
            using(await asyncLock.LockAsync())
            {
                App.analytics.Send(HitBuilder.CreateCustomEvent("click pulsante", "modifica "+player.ToString()).Build());
                var dialog = new EditPlayer(player, score.GetPlayerName(player), score.GetPlayerColor(player));
                await dialog.ShowAsync();

                if(dialog.Result)
                {
                    score.EditPlayer(player, dialog.PlayerName, dialog.PlayerColor);
                }
            }
        }

        private async void AddPoints(Players player)
        {
            using(await asyncLock.LockAsync())
            {
                App.analytics.Send(HitBuilder.CreateCustomEvent("click pulsante", "aggiungi punti "+player.ToString()).Build());
                string playername = score.GetPlayerName(player);
                var dialog = new AddScore(playername);
                await dialog.ShowAsync();

                if(dialog.Result)
                {
                    int value;
                    if(int.TryParse(dialog.Text, out value))
                    {
                        score.AddPoints(player, value);
                    }
                    else
                    {
                        MessageDialogHelper.Show(LocalizedString.Get("allowed_int_num"), LocalizedString.Get("warning"));
                    }
                }
            }
        }

        private async void ClearSession(object sender, RoutedEventArgs e)
        {
            App.analytics.Send(HitBuilder.CreateCustomEvent("click pulsante", "pulisci sessione").Build());
            if (await MessageDialogHelper.Confirm(LocalizedString.Get("clear_1") + "\n" + LocalizedString.Get("clear_2"), LocalizedString.Get("warning"), LocalizedString.Get("ok"), LocalizedString.Get("cancel")))
            {
                score.Clear();
            }
        }

        private void Update()
        {
            //imposta i nomi ai player
            p1_name.Content = score.ScoreSession.Player1.Name;
            p2_name.Content = score.ScoreSession.Player2.Name;
            p3_name.Content = score.ScoreSession.Player3.Name;
            p4_name.Content = score.ScoreSession.Player4.Name;

            //imposta i colori
            p1_name.Background = new SolidColorBrush(score.ScoreSession.Player1.PlayerColor);
            p2_name.Background = new SolidColorBrush(score.ScoreSession.Player2.PlayerColor);
            p3_name.Background = new SolidColorBrush(score.ScoreSession.Player3.PlayerColor);
            p4_name.Background = new SolidColorBrush(score.ScoreSession.Player4.PlayerColor);

            p1_tot_grid.Background = new SolidColorBrush(score.ScoreSession.Player1.PlayerColor);
            p2_tot_grid.Background = new SolidColorBrush(score.ScoreSession.Player2.PlayerColor);
            p3_tot_grid.Background = new SolidColorBrush(score.ScoreSession.Player3.PlayerColor);
            p4_tot_grid.Background = new SolidColorBrush(score.ScoreSession.Player4.PlayerColor);

            //imposta i punti ai player e fai la somma
            int somma1 = 0;
            int somma2 = 0;
            int somma3 = 0;
            int somma4 = 0;

            p1_stack.Children.Clear();
            int index = 0;
            foreach(int point in score.ScoreSession.Player1.Points)
            {
                Button button = new Button();
                button.Content = point.ToString();
                button.Style = Application.Current.Resources["PointsButtonStyle"] as Style;
                button.Tag = new PlayerPoint(Players.Player1, index);
                button.Click += PointButton;
                p1_stack.Children.Add(button);
                somma1 += point;
                index++;
            }

            p2_stack.Children.Clear();
            index = 0;
            foreach(int point in score.ScoreSession.Player2.Points)
            {
                Button button = new Button();
                button.Content = point.ToString();
                button.Style = Application.Current.Resources["PointsButtonStyle"] as Style;
                button.Tag = new PlayerPoint(Players.Player2, index);
                button.Click += PointButton;
                p2_stack.Children.Add(button);
                somma2 += point;
                index++;
            }

            p3_stack.Children.Clear();
            index = 0;
            foreach(int point in score.ScoreSession.Player3.Points)
            {
                Button button = new Button();
                button.Content = point.ToString();
                button.Style = Application.Current.Resources["PointsButtonStyle"] as Style;
                button.Tag = new PlayerPoint(Players.Player3, index);
                button.Click += PointButton;
                p3_stack.Children.Add(button);
                somma3 += point;
                index++;
            }

            p4_stack.Children.Clear();
            index = 0;
            foreach(int point in score.ScoreSession.Player4.Points)
            {
                Button button = new Button();
                button.Content = point.ToString();
                button.Style = Application.Current.Resources["PointsButtonStyle"] as Style;
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

            ScrollToBottom();
        }

        private async void PointButton(object sender, RoutedEventArgs e)
        {
            Button thisbutton = (sender as Button);
            PlayerPoint playerpoint = (PlayerPoint)thisbutton.Tag;

            if((bool)cb_edit.IsChecked)
            {
                App.analytics.Send(HitBuilder.CreateCustomEvent("click punti", "modifica").Build());
                var res = await MessageDialogHelper.DialogTextBox(LocalizedString.Get("edit_text"), LocalizedString.Get("edit_title"), (string)thisbutton.Content, LocalizedString.Get("ok"), LocalizedString.Get("cancel"), null, null, InputScopeNameValue.NumberFullWidth);
                if(res.result)
                {
                    int value;
                    if(int.TryParse(res.output, out value))
                    {
                        score.EditPoints(playerpoint.player, playerpoint.index, value);
                    }
                    else
                    {
                        MessageDialogHelper.Show(LocalizedString.Get("allowed_int_num"), LocalizedString.Get("warning"));
                    }
                }
            }
            else if((bool)cb_delete.IsChecked)
            {
                App.analytics.Send(HitBuilder.CreateCustomEvent("click punti", "elimina").Build());
                score.DeletePoints(playerpoint.player, playerpoint.index);
            }
        }

        private void ScrollToBottom()
        {            
            BOARD.UpdateLayout();
            BOARD.ChangeView(null, BOARD.ScrollableHeight, null, false);
        }

        private void SetTitleBarIfPresent()
        {
            //colora la titlebar con background chrome e foreground base
            if(ApiInformation.IsTypePresent("Windows.UI.ViewManagement.ApplicationView"))
            {
                var titleBar = ApplicationView.GetForCurrentView().TitleBar;
                titleBar.ButtonBackgroundColor = ((SolidColorBrush)Application.Current.Resources["SystemControlBackgroundChromeMediumBrush"]).Color;
                titleBar.ButtonForegroundColor = ((SolidColorBrush)Application.Current.Resources["SystemControlForegroundBaseHighBrush"]).Color;
                titleBar.BackgroundColor = ((SolidColorBrush)Application.Current.Resources["SystemControlBackgroundChromeMediumBrush"]).Color;
                titleBar.ForegroundColor = ((SolidColorBrush)Application.Current.Resources["SystemControlForegroundBaseHighBrush"]).Color;
            }
        }

        private void Current_VisibilityChanged(object sender, Windows.UI.Core.VisibilityChangedEventArgs e)
        {
            SetTitleBarIfPresent();

            //colora la statusbar con background chrome e foreground base
            if(ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                var statusBar = StatusBar.GetForCurrentView();
                statusBar.BackgroundOpacity = 1;
                statusBar.BackgroundColor = ((SolidColorBrush)Application.Current.Resources["SystemControlBackgroundChromeMediumBrush"]).Color;
                statusBar.ForegroundColor = ((SolidColorBrush)Application.Current.Resources["SystemControlForegroundBaseHighBrush"]).Color;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            App.analytics.Send(HitBuilder.CreateScreenView("Home").Build());
        }
    }
}
