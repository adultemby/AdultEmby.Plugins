using System;
using System.IO;
using AdultEmby.Plugins.Core;
using AdultEmby.Plugins.Data18;
using AdultEmby.Plugins.Test.Logging;
using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using MediaBrowser.Model.Logging;
using NSubstitute;
using Xunit;

namespace AdultEmby.Plugins.Test.Data18
{
    public class Data18PersonHtmlExtractorTest
    {
        [Fact]
        public void ShouldHaveMetadata()
        {
            IHtmlPersonExtractor htmlMetadataExtractor =
                new Data18PersonHtmlExtractor(LogManager());

            bool hasMetadata = htmlMetadataExtractor.HasMetadata(loadHtmlDocument());

            Assert.True(hasMetadata);
        }

        [Fact]
        public void ShouldNotHaveMetadataIfHeaderIsFalse()
        {
            IHtmlPersonExtractor htmlMetadataExtractor =
                new Data18PersonHtmlExtractor(LogManager());

            bool hasMetadata = htmlMetadataExtractor.HasMetadata(loadHtmlDocument(@"TestResponses\NoMetadata.html"));

            Assert.False(hasMetadata);
        }


        [Fact]
        public void ShouldExtractName()
        {
            IHtmlPersonExtractor htmlMetadataExtractor =
                new Data18PersonHtmlExtractor(LogManager());

            String title = htmlMetadataExtractor.GetName(loadHtmlDocument());

            Assert.Equal("Bree Olson", title);
        }

        [Fact]
        public void ShouldExtractBirthdate()
        {
            IHtmlPersonExtractor htmlMetadataExtractor =
                new Data18PersonHtmlExtractor(LogManager());

            DateTime? birthdate = htmlMetadataExtractor.GetBirthdate(loadHtmlDocument());

            Assert.Equal(new DateTime(1986, 10, 7), birthdate);
        }

        [Fact]
        public void ShouldExtractNationality()
        {
            IHtmlPersonExtractor htmlMetadataExtractor =
                new Data18PersonHtmlExtractor(LogManager());

            String birthdate = htmlMetadataExtractor.GetNationality(loadHtmlDocument());

            Assert.Equal("United States", birthdate);
        }

        [Fact]
        public void ShouldExtractBirthplace()
        {
            IHtmlPersonExtractor htmlMetadataExtractor =
                new Data18PersonHtmlExtractor(LogManager());

            String birthplace = htmlMetadataExtractor.GetBirthplace(loadHtmlDocument());

            Assert.Equal("Houston, TX", birthplace);
        }

        [Fact]
        public void ShouldExtractStarSign()
        {
            IHtmlPersonExtractor htmlMetadataExtractor =
                new Data18PersonHtmlExtractor(LogManager());

            String starSign = htmlMetadataExtractor.GetStarSign(loadHtmlDocument());

            Assert.Equal("Libra", starSign);
        }

        [Fact]
        public void ShouldExtractEthnicity()
        {
            IHtmlPersonExtractor htmlMetadataExtractor =
                new Data18PersonHtmlExtractor(LogManager());

            String ethnicity = htmlMetadataExtractor.GetEthnicity(loadHtmlDocument());

            Assert.Equal("Caucasian", ethnicity);
        }

        [Fact]
        public void ShouldExtractMeasurements()
        {
            IHtmlPersonExtractor htmlMetadataExtractor =
                new Data18PersonHtmlExtractor(LogManager());

            String ethnicity = htmlMetadataExtractor.GetMeasurements(loadHtmlDocument());

            Assert.Equal("34C-28-36", ethnicity);
        }

        [Fact]
        public void ShouldExtractHeight()
        {
            IHtmlPersonExtractor htmlMetadataExtractor =
                new Data18PersonHtmlExtractor(LogManager());

            String height = htmlMetadataExtractor.GetHeight(loadHtmlDocument());

            Assert.Equal("5ft 3in 161.5 cm.", height);
        }

        [Fact]
        public void ShouldExtractWeight()
        {
            IHtmlPersonExtractor htmlMetadataExtractor =
                new Data18PersonHtmlExtractor(LogManager());

            String weight = htmlMetadataExtractor.GetWeight(loadHtmlDocument());

            Assert.Equal("124 lbs 56 kg", weight);
        }

        [Fact]
        public void ShouldExtractTwitter()
        {
            IHtmlPersonExtractor htmlMetadataExtractor =
                new Data18PersonHtmlExtractor(LogManager());

            String twitter = htmlMetadataExtractor.GetTwitter(loadHtmlDocument());

            Assert.Equal("http://www.twitter.com/BreeOlson", twitter);
        }

        [Fact]
        public void ShouldExtractPrimaryImageUrl()
        {
            IHtmlPersonExtractor htmlMetadataExtractor =
                new Data18PersonHtmlExtractor(LogManager());

            String url = htmlMetadataExtractor.GetPrimaryImageUrl(loadHtmlDocument());

            Assert.Equal("http://img.data18.com/images/stars/120/255.jpg", url);
        }

        private IHtmlDocument loadHtmlDocument()
        {
            return loadHtmlDocument(@"TestResponses\Data18\PersonResponse.html");
        }

        private IHtmlDocument loadHtmlDocument(string path)
        {
            Stream responseStream = File.OpenRead(path);
            var parser = new HtmlParser();
            return parser.Parse(responseStream);
        }

        private ILogManager LogManager()
        {
            return TestLogManager.Instance;
        }
    }
}
