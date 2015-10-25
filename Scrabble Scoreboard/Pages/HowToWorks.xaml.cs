using Aura.Net;
using Aura.Net.Localization;
using Aura.Net.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Text;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Il modello di elemento per la pagina vuota è documentato all'indirizzo http://go.microsoft.com/fwlink/?LinkID=390556

namespace Scrabble_Scoreboard.Pages
{
    /// <summary>
    /// Pagina vuota che può essere utilizzata autonomamente oppure esplorata all'interno di un frame.
    /// </summary>
    public sealed partial class HowToWorks : Page
    {
        public HowToWorks()
        {
            this.InitializeComponent();
            titlebar.Margin = Utilities.SetMarginTop(titlebar, StatusBar.GetForCurrentView().OccludedRect.Height);

            GoogleAnalytics.EasyTracker.GetTracker().SendView("HowToWorks.xaml.cs");

            title.Text = Translate.Get("help");

            desc.Inlines.Clear();
            desc.Inlines.Add(new Run() { Text=Translate.Get("help_desc") });
            desc.Inlines.Add(new LineBreak());
            desc.Inlines.Add(new LineBreak());

            desc.Inlines.Add(new Run() { Text = Translate.Get("features")+":", FontWeight=FontWeights.Bold, Foreground=MyColors.PhoneAccentBrush });
            desc.Inlines.Add(new LineBreak());
            desc.Inlines.Add(new Run() { Text = "• " + Translate.Get("feature_1") });
            desc.Inlines.Add(new LineBreak());
            desc.Inlines.Add(new Run() { Text = "• " + Translate.Get("feature_2") });
            desc.Inlines.Add(new LineBreak());
            desc.Inlines.Add(new Run() { Text = "• " + Translate.Get("feature_3") });
            desc.Inlines.Add(new LineBreak());
            desc.Inlines.Add(new Run() { Text = "• " + Translate.Get("feature_4") });
            desc.Inlines.Add(new LineBreak());
            desc.Inlines.Add(new LineBreak());

            desc.Inlines.Add(new Run() { Text = Translate.Get("note") + ":", FontWeight = FontWeights.Bold, Foreground = MyColors.PhoneAccentBrush });
            desc.Inlines.Add(new LineBreak());
            desc.Inlines.Add(new Run() { Text = Translate.Get("note_desc") });
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            GoogleAnalytics.EasyTracker.GetTracker().SendEvent("HowToWorks.xaml.cs", "OnNavigatedTo", null, 0);
        }
    }
}
