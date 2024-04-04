using Aura.Globalization;
using Windows.UI.Xaml.Controls;

namespace ScrabbleScoreKeeper.Dialogs
{
    public sealed partial class AddScore : ContentDialog
    {
        public bool Result = false;
        public string Text;

        public AddScore(string playerName)
        {
            this.InitializeComponent();
            this.Title = string.Format(LocalizedString.Get("enter_score"), playerName);
            this.PrimaryButtonText = LocalizedString.Get("add");
            this.SecondaryButtonText = LocalizedString.Get("cancel");

            this.PrimaryButtonClick += (s, e) => { Result = true; Text = input.Text; };
            this.SecondaryButtonClick += (s, e) => { Result = false; };
            input.KeyDown += Input_KeyDown;
        }

        private void Input_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if(e.Key==Windows.System.VirtualKey.Enter)
            {
                e.Handled = true;
                Text = input.Text;
                Result = true;
                this.Hide();
            }
        }
    }
}
