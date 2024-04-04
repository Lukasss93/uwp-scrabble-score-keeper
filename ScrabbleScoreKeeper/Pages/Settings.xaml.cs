using Aura.Globalization;
using Aura.Storage;
using GoogleAnalytics;
using NotificationsExtensions.Tiles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace ScrabbleScoreKeeper.Pages
{
    public sealed partial class Settings : Page
    {
        public Settings()
        {
            this.InitializeComponent();

            //titolo header
            title.Text = LocalizedString.Get("settings");

            //combobox scelta del tema
            List<string> themes = new List<string>();
            themes.Add(LocalizedString.Get("automatic"));
            themes.Add(LocalizedString.Get("dark"));
            themes.Add(LocalizedString.Get("light"));
            setting_theme.Header = LocalizedString.Get("theme");
            setting_theme.ItemsSource = themes;
            setting_theme.SelectedIndex = (int)AppSettings.Get("theme");
            setting_theme.SelectionChanged += (s, e) => 
            {
                App.analytics.Send(HitBuilder.CreateCustomEvent("combobox selection changed", "scelta del tema").Build());
                AppSettings.Set("theme", (s as ComboBox).SelectedIndex);
            };
            theme_warning.Text = LocalizedString.Get("theme_warning");

            //switch tile colorata/trasparente
            setting_tile.Header = LocalizedString.Get("transparent_tile");
            setting_tile.IsOn = (bool)AppSettings.Get("transparent_tile");
            setting_tile.Toggled += (s, e) => 
            {
                App.analytics.Send(HitBuilder.CreateCustomEvent("toggleswitch toggled", "tile colorata/trasparente", null, Convert.ToInt64((s as ToggleSwitch).IsOn)).Build());
                AppSettings.Set("transparent_tile", (s as ToggleSwitch).IsOn);
                CheckTile();
            };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            App.analytics.Send(HitBuilder.CreateScreenView("Settings").Build());
            CheckTile();
        }

        private void CheckTile()
        {
            string folderName = (bool)AppSettings.Get("transparent_tile") ? "transparent" : "colored";

            TileContent content = new TileContent()
            {
                Visual = new TileVisual()
                {
                    TileSmall = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            BackgroundImage = new TileBackgroundImage()
                            {
                                Source = "ms-appx:///Assets/tiles/" + folderName + "/170x170.png"
                            }
                        }
                    },

                    TileMedium = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            BackgroundImage = new TileBackgroundImage()
                            {
                                Source = "ms-appx:///Assets/tiles/" + folderName + "/360x360.png"
                            }
                        }
                    }
                }
            };


            var notification = new TileNotification(content.GetXml());
            TileUpdateManager.CreateTileUpdaterForApplication().Update(notification);
        }
    }
}
