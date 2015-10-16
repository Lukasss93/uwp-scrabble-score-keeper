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
        JsonSave save;

        public MainPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
            titlebar.Margin = Utilities.SetMarginTop(titlebar, StatusBar.GetForCurrentView().OccludedRect.Height);

            p1_name.Click += P1_name_Click;
            p2_name.Click += P2_name_Click;
            p3_name.Click += P3_name_Click;
            p4_name.Click += P4_name_Click;

            p1_add.Click += P1_add_Click;

            ab_clear.Click += Ab_clear_Click;
        }

        private void Ab_clear_Click(object sender, RoutedEventArgs e)
        {
            JsonSave newsave = new JsonSave();
            newsave.Player1.Name = "Player 1";
            newsave.Player2.Name = "Player 2";
            newsave.Player3.Name = "Player 2";
            newsave.Player4.Name = "Player 2";
            SettingsHelper.Set("save", Json.Serialize(newsave));

            save = Json.Deserialize<JsonSave>((string)SettingsHelper.Get("save"));

            Update();
        }

        int i = 1;
        private void P1_add_Click(object sender, RoutedEventArgs e)
        {
            Button button = new Button();
            button.Content = "" + i;
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

            i++;
        }

        #region AZIONI PULSANTI NOMI DEI PLAYER
        private async void P1_name_Click(object sender, RoutedEventArgs e)
        {
            var res = await MessageDialogHelper.DialogTextBox("Inserisci il nome per il Player 1", "Inserisci nome");
            if(res.result)
            {
                save.Player1.Name = res.output;
                Save();
                Update();
            }

        }

        private async void P2_name_Click(object sender, RoutedEventArgs e)
        {
            var res = await MessageDialogHelper.DialogTextBox("Inserisci il nome per il Player 2", "Inserisci nome");
            if(res.result)
            {
                save.Player2.Name = res.output;
                Save();
                Update();
            }
        }

        private async void P3_name_Click(object sender, RoutedEventArgs e)
        {
            var res = await MessageDialogHelper.DialogTextBox("Inserisci il nome per il Player 3", "Inserisci nome");
            if(res.result)
            {
                save.Player3.Name = res.output;
                Save();
                Update();
            }
        }

        private async void P4_name_Click(object sender, RoutedEventArgs e)
        {
            var res = await MessageDialogHelper.DialogTextBox("Inserisci il nome per il Player 4", "Inserisci nome");
            if(res.result)
            {
                save.Player4.Name = res.output;
                Save();
                Update();
            }
        }
        #endregion

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
            p1_name.Content = save.Player1.Name;
            p2_name.Content = save.Player2.Name;
            p3_name.Content = save.Player3.Name;
            p4_name.Content = save.Player4.Name;
        }
    }
}
