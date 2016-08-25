using AuraRT.Display;
using AuraRT.Globalization;
using AuraRT.Imaging;
using Windows.UI.Text;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
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
            titlebar.Margin = StatusBarHelper.SetTopMargin(titlebar);

            GoogleAnalytics.EasyTracker.GetTracker().SendView("HowToWorks.xaml.cs");

            title.Text = LocalizedString.Get("help");

            desc.Inlines.Clear();
            desc.Inlines.Add(new Run() { Text=LocalizedString.Get("help_desc") });
            desc.Inlines.Add(new LineBreak());
            desc.Inlines.Add(new LineBreak());

            desc.Inlines.Add(new Run() { Text = LocalizedString.Get("features")+":", FontWeight=FontWeights.Bold, Foreground=ColorUtilities.PhoneAccentBrush });
            desc.Inlines.Add(new LineBreak());
            desc.Inlines.Add(new Run() { Text = "• " + LocalizedString.Get("feature_1") });
            desc.Inlines.Add(new LineBreak());
            desc.Inlines.Add(new Run() { Text = "• " + LocalizedString.Get("feature_2") });
            desc.Inlines.Add(new LineBreak());
            desc.Inlines.Add(new Run() { Text = "• " + LocalizedString.Get("feature_3") });
            desc.Inlines.Add(new LineBreak());
            desc.Inlines.Add(new Run() { Text = "• " + LocalizedString.Get("feature_4") });
            desc.Inlines.Add(new LineBreak());
            desc.Inlines.Add(new LineBreak());

            desc.Inlines.Add(new Run() { Text = LocalizedString.Get("note") + ":", FontWeight = FontWeights.Bold, Foreground = ColorUtilities.PhoneAccentBrush });
            desc.Inlines.Add(new LineBreak());
            desc.Inlines.Add(new Run() { Text = LocalizedString.Get("note_desc") });
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            GoogleAnalytics.EasyTracker.GetTracker().SendEvent("HowToWorks.xaml.cs", "OnNavigatedTo", null, 0);
        }
    }
}
