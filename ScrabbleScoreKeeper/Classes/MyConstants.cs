using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aura.Globalization;
using Lukasss93.Classes;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace ScrabbleScoreKeeper.Classes
{
    public class MyConstants
    {
        public static readonly SolidColorBrush appColor = new SolidColorBrush(Color.FromArgb(255, 16, 124, 16));

        public static InfoParameter GenerateParameter()
        {
            InfoParameter parameter = new InfoParameter();
            parameter.Icon = new Uri("ms-appx:///Assets/img/logo.png");
            parameter.Name = "Scrabble Score Keeper";
            parameter.Description = LocalizedString.Get("appdescription");
            parameter.Features = new List<string>()
            {
                LocalizedString.Get("feature1"),
                LocalizedString.Get("feature2"),
                LocalizedString.Get("feature3"),
                LocalizedString.Get("feature4")
            };

            parameter.ExistPRO = false;

            parameter.Changelog = new List<InfoChangelog>()
            {
                new InfoChangelog(new Version(2,0,1,0),new DateTime(2017,4,25),new List<InfoLog>()
                {
                    new InfoLog(LocalizedString.Get("changelog2"),LogType.Fixed),
                }),
                new InfoChangelog(new Version(2,0,0,0),new DateTime(2017,4,20),new List<InfoLog>()
                {
                    new InfoLog(LocalizedString.Get("changelog4"),LogType.Removed),
                    new InfoLog(LocalizedString.Get("changelog5"),LogType.Added)
                }),
                new InfoChangelog(new Version(1,2,0,0),new DateTime(2016,8,25),new List<InfoLog>()
                {
                    new InfoLog(LocalizedString.Get("changelog3"),LogType.Fixed)
                }),
                new InfoChangelog(new Version(1,1,0,0),new DateTime(2016,3,15),new List<InfoLog>()
                {
                    new InfoLog(LocalizedString.Get("changelog2"),LogType.Fixed)
                }),
                new InfoChangelog(new Version(1,0,0,0),new DateTime(2015,11,17),new List<InfoLog>()
                {
                    new InfoLog(LocalizedString.Get("changelog1"),LogType.Default)
                })
            };

            parameter.StoreUrl = "https://www.microsoft.com/store/apps/9nblggh67rg4";
            parameter.Disclaimer = LocalizedString.Get("disclaimer");

            return parameter;
        }
    }
}
