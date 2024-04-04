using Windows.UI.Xaml.Media;

namespace ScrabbleScoreKeeper.Classes
{
    public class AppColors
    {
        public SolidColorBrush Color { get; set; }
        public string Name { get; set; }

        public AppColors(SolidColorBrush color, string name)
        {
            Color = color;
            Name = name;
        }
    }
}
