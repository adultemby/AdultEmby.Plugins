using System;
using AdultEmby.Plugins.Core;
using AngleSharp.Dom.Html;
using MediaBrowser.Model.Logging;

namespace AdultEmby.Plugins.Core.Test
{
    public class TestSourcePersonHtmlExtractor : IHtmlPersonExtractor
    {
        private ILogger _logger;

        public TestSourcePersonHtmlExtractor(ILogManager logManager)
        {
            _logger = logManager.GetLogger(GetType().FullName);
        }

        public bool HasMetadata(IHtmlDocument htmlDocument)
        {
            return true;
        }

        public string GetName(IHtmlDocument htmlDocument)
        {
            return "NAME";
        }

        public DateTime? GetBirthdate(IHtmlDocument htmlDocument)
        {
            return new DateTime(2001, 1, 1);
        }

        public string GetStarSign(IHtmlDocument htmlDocument)
        {
            return "STARSIGN";
        }

        public string GetMeasurements(IHtmlDocument htmlDocument)
        {
            return "MEASUREMENTS";
        }

        public string GetHeight(IHtmlDocument htmlDocument)
        {
            return "HEIGHT";
        }

        public string GetWeight(IHtmlDocument htmlDocument)
        {
            return "WEIGHT";
        }

        public string GetTwitter(IHtmlDocument htmlDocument)
        {
            return "TWITTER";
        }

        //string GetOfficialSite(IHtmlDocument htmlDocument)
        //{
        //    return "";
       // }

        public string GetNationality(IHtmlDocument htmlDocument)
        {
            return "NATIONALITY";
        }

        public string GetBirthplace(IHtmlDocument htmlDocument)
        {
            return "BIRTH_PLACE";
        }

        public string GetEthnicity(IHtmlDocument htmlDocument)
        {
            return "ETHNICITY";
        }

        public string GetPrimaryImageUrl(IHtmlDocument htmlDocument)
        {
            return "IMAGE_URL";
        }
    }
}
