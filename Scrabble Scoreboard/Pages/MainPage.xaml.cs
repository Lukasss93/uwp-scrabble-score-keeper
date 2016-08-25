using System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Text;
using Scrabble_Scoreboard.Classes;
using AuraRT.Serializer;
using AuraRT.Display;
using AuraRT.Globalization;
using AuraRT.Storage;
using Lukasss93.Controls;
using System.Linq;
using System.Reflection;
using AuraRT.Imaging;
using Windows.UI;

namespace Scrabble_Scoreboard.Pages
{
    public sealed partial class MainPage : Page
    {
        private static Enough.Async.AsyncLock asyncLock = new Enough.Async.AsyncLock();
        JsonSave save;

        bool isEdit = false;
        bool isDelete = false;

        public MainPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
            CommandBarSetup(COMMANDBAR);

            StatusBarHelper.SetBackground(ColorUtilities.PhoneChromeBrush.Color);

            GoogleAnalytics.EasyTracker.GetTracker().SendView("Mainpage.xaml.cs");

            p1_name.Click += (sender, e) => { EditName(Players.Player1); };
            p2_name.Click += (sender, e) => { EditName(Players.Player2); };
            p3_name.Click += (sender, e) => { EditName(Players.Player3); };
            p4_name.Click += (sender, e) => { EditName(Players.Player4); };

            p1_add.Click += (sender, e) => { AddPoints(Players.Player1); };
            p2_add.Click += (sender, e) => { AddPoints(Players.Player2); };
            p3_add.Click += (sender, e) => { AddPoints(Players.Player3); };
            p4_add.Click += (sender, e) => { AddPoints(Players.Player4); };

            //commandbar
            UpdateActionButtonIcon();

            cb_edit.Click += (s, e) => { SetActionButtons(true); UpdateActionButtonIcon(); };
            cb_delete.Click += (s, e) => { SetActionButtons(false); UpdateActionButtonIcon(); };
            cb_clear.Click += Ab_clear_Click;
            cb_info.Click += (s, e) =>
            {
                GoogleAnalytics.EasyTracker.GetTracker().SendEvent("Mainpage.xaml.cs", "cb_info.Click", null, 0);
                Frame.Navigate(typeof(Lukasss93.Pages.Information),Json.Serialize(MyConstants.GenerateParameter()));
            };

            cb_edit.Label = LocalizedString.Get("edit");
            cb_delete.Label = LocalizedString.Get("delete");
            cb_clear.Label = LocalizedString.Get("clear");
            cb_info.Label = LocalizedString.Get("info");

        }

        private void SetActionButtons(bool v)
        {
            if(v) //edit
            {
                isEdit = !isEdit;

                if(isEdit && isDelete)
                    isDelete = false;
            }
            else //delete
            {
                isDelete = !isDelete;

                if(isDelete && isEdit)
                    isEdit = false;
            }
        }

        private void UpdateActionButtonIcon()
        {
            cb_edit.Background = isEdit ? MyConstants.appColor : new SolidColorBrush(Colors.Transparent);
            cb_delete.Background = isDelete ? MyConstants.appColor : new SolidColorBrush(Colors.Transparent);
        }

        private async void Ab_clear_Click(object sender, RoutedEventArgs e)
        {
            GoogleAnalytics.EasyTracker.GetTracker().SendEvent("Mainpage.xaml.cs", "Ab_clear_Click", null, 0);

            if(await MessageDialogHelper.Confirm(LocalizedString.Get("clear_1") + "\n" + LocalizedString.Get("clear_2"), LocalizedString.Get("warning"), LocalizedString.Get("ok"), LocalizedString.Get("cancel")))
            {

                JsonSave newsave = new JsonSave();
                newsave.Player1.Name = "Player 1";
                newsave.Player2.Name = "Player 2";
                newsave.Player3.Name = "Player 3";
                newsave.Player4.Name = "Player 4";
                AppSettings.Set("save", Json.Serialize(newsave));

                save = Json.Deserialize<JsonSave>((string)AppSettings.Get("save"));

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
                            String.Format(LocalizedString.Get("enter_score_player_generic"), "Player 1") :
                            String.Format(LocalizedString.Get("enter_score_player_name"), save.Player1.Name);
                        break;
                    case Players.Player2:
                        phrase = save.Player2.Name == "Player 2" ?
                            String.Format(LocalizedString.Get("enter_score_player_generic"), "Player 2") :
                            String.Format(LocalizedString.Get("enter_score_player_name"), save.Player2.Name);
                        break;
                    case Players.Player3:
                        phrase = save.Player3.Name == "Player 3" ?
                            String.Format(LocalizedString.Get("enter_score_player_generic"), "Player 3") :
                            String.Format(LocalizedString.Get("enter_score_player_name"), save.Player3.Name);
                        break;
                    case Players.Player4:
                        phrase = save.Player4.Name == "Player 4" ?
                            String.Format(LocalizedString.Get("enter_score_player_generic"), "Player 4") :
                            String.Format(LocalizedString.Get("enter_score_player_name"), save.Player4.Name);
                        break;
                }

                var res = await MessageDialogHelper.DialogTextBox(phrase, LocalizedString.Get("enter_score"), "", LocalizedString.Get("ok"), LocalizedString.Get("cancel"), null, null, InputScopeNameValue.NumberFullWidth);
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
                        MessageDialogHelper.Show(LocalizedString.Get("allowed_int_num"), LocalizedString.Get("warning"));
                    }
                }
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //Debug.WriteLine((string)SettingsHelper.Get("save"));
            GoogleAnalytics.EasyTracker.GetTracker().SendEvent("Mainpage.xaml.cs", "OnNavigatedTo", null, 0);

            save = Json.Deserialize<JsonSave>((string)AppSettings.Get("save"));
            Update();

            cb_edit.IsCompact = true;
            cb_delete.IsCompact = true;
            cb_clear.IsCompact = true;
        }

        private void Save()
        {
            AppSettings.Set("save", Json.Serialize(save));
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
            if(save.Player1.Points.Count + save.Player2.Points.Count + save.Player3.Points.Count + save.Player4.Points.Count == 0)
            {
                isEdit = false;
                isDelete = false;
                UpdateActionButtonIcon();
            }

        }

        private async void PointButton(object sender, RoutedEventArgs e)
        {
            if(isEdit || isDelete)
            {
                Button thisbutton = (sender as Button);
                PlayerPoint playerpoint = (PlayerPoint)thisbutton.Tag;

                if(isEdit)
                {
                    var res = await MessageDialogHelper.DialogTextBox(LocalizedString.Get("edit_text"), LocalizedString.Get("edit_title"), (string)thisbutton.Content, LocalizedString.Get("ok"), LocalizedString.Get("cancel"), null, null, InputScopeNameValue.NumberFullWidth);
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
                        else
                        {
                            MessageDialogHelper.Show(LocalizedString.Get("allowed_int_num"), LocalizedString.Get("warning"));
                        }
                    }

                }
                else if(isDelete)
                {


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

        private void CommandBarSetup(CommandBarBar commandBar)
        {
            var children = commandBar.PrimaryCommands.Union(commandBar.SecondaryCommands);
            var runtimeFields = this.GetType().GetRuntimeFields();

            foreach(DependencyObject i in children)
            {
                var info = i.GetType().GetRuntimeProperty("Name");

                if(info != null)
                {
                    string name = (string)info.GetValue(i);

                    if(name != null)
                    {
                        foreach(FieldInfo j in runtimeFields)
                        {
                            if(j.Name == name)
                            {
                                j.SetValue(this, i);
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}
