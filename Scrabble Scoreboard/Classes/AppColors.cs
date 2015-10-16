using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace Scrabble_Scoreboard.Classes
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
