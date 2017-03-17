using System;
using System.IO;
using AdultEmby.Plugins.Core;
using AdultEmby.Plugins.DorcelVision;
using AdultEmby.Plugins.TestLogging;
using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using MediaBrowser.Model.Logging;
using NSubstitute;
using Xunit;

namespace AdultEmby.Plugins.DorcelVision.Test
{
    public class DorcelVisionPersonHtmlExtractorTest
    {
        [Fact]
        public void ShouldHaveMetadata()
        {
            IHtmlPersonExtractor htmlMetadataExtractor =
                new DorcelVisionPersonHtmlExtractor(LogManager());

            bool hasMetadata = htmlMetadataExtractor.HasMetadata(LoadHtmlDocument());

            Assert.True(hasMetadata);
        }

        [Fact]
        public void ShouldNotHaveMetadataIfHeaderIsFalse()
        {
            IHtmlPersonExtractor htmlMetadataExtractor =
                new DorcelVisionPersonHtmlExtractor(LogManager());

            bool hasMetadata = htmlMetadataExtractor.HasMetadata(LoadHtmlDocument(@"TestResponses\NoMetadata.html"));

            Assert.False(hasMetadata);
        }

        [Fact]
        public void ShouldExtractName()
        {
            IHtmlPersonExtractor htmlMetadataExtractor =
                new DorcelVisionPersonHtmlExtractor(LogManager());

            String name = htmlMetadataExtractor.GetName(LoadHtmlDocument());

            Assert.Equal("Nikita Bellucci", name);
        }

        [Fact]
        public void ShouldExtractBirthdate()
        {
            IHtmlPersonExtractor htmlMetadataExtractor =
                new DorcelVisionPersonHtmlExtractor(LogManager());

            DateTime? birthdate = htmlMetadataExtractor.GetBirthdate(LoadHtmlDocument());

            Assert.Equal(null, birthdate);
        }

        [Fact]
        public void ShouldExtractNationality()
        {
            IHtmlPersonExtractor htmlMetadataExtractor =
                new DorcelVisionPersonHtmlExtractor(LogManager());

            String birthdate = htmlMetadataExtractor.GetNationality(LoadHtmlDocument());

            Assert.Equal("Française", birthdate);
        }

        [Fact]
        public void ShouldExtractBirthplace()
        {
            IHtmlPersonExtractor htmlMetadataExtractor =
                new DorcelVisionPersonHtmlExtractor(LogManager());

            String birthplace = htmlMetadataExtractor.GetBirthplace(LoadHtmlDocument());

            Assert.Equal(null, birthplace);
        }

        [Fact]
        public void ShouldExtractStarSign()
        {
            IHtmlPersonExtractor htmlMetadataExtractor =
                new DorcelVisionPersonHtmlExtractor(LogManager());

            String starSign = htmlMetadataExtractor.GetStarSign(LoadHtmlDocument());

            Assert.Equal(null, starSign);
        }

        [Fact]
        public void ShouldExtractEthnicity()
        {;
            IHtmlPersonExtractor htmlMetadataExtractor =
                new DorcelVisionPersonHtmlExtractor(LogManager());

            String ethnicity = htmlMetadataExtractor.GetEthnicity(LoadHtmlDocument());

            Assert.Equal(null, ethnicity);
        }

        [Fact]
        public void ShouldExtractMeasurements()
        {
            IHtmlPersonExtractor htmlMetadataExtractor =
                new DorcelVisionPersonHtmlExtractor(LogManager());

            String measurements = htmlMetadataExtractor.GetMeasurements(LoadHtmlDocument());

            Assert.Equal(null, measurements);
        }

        [Fact]
        public void ShouldExtractHeight()
        {
            IHtmlPersonExtractor htmlMetadataExtractor =
                new DorcelVisionPersonHtmlExtractor(LogManager());

            String height = htmlMetadataExtractor.GetHeight(LoadHtmlDocument());

            Assert.Equal(null, height);
        }

        [Fact]
        public void ShouldExtractWeight()
        {
            IHtmlPersonExtractor htmlMetadataExtractor =
                new DorcelVisionPersonHtmlExtractor(LogManager());

            String weight = htmlMetadataExtractor.GetWeight(LoadHtmlDocument());

            Assert.Equal(null, weight);
        }

        [Fact]
        public void ShouldExtractTwitter()
        {
            IHtmlPersonExtractor htmlMetadataExtractor =
                new DorcelVisionPersonHtmlExtractor(LogManager());

            String twitter = htmlMetadataExtractor.GetTwitter(LoadHtmlDocument());

            Assert.Equal(null, twitter);
        }

        [Fact]
        public void ShouldExtractPrimaryImageUrl()
        {
            IHtmlPersonExtractor htmlMetadataExtractor =
                new DorcelVisionPersonHtmlExtractor(LogManager());

            String url = htmlMetadataExtractor.GetPrimaryImageUrl(LoadHtmlDocument());

            Assert.Equal("https://www.dorcelvision.com/images/actors/812774.jpg", url);
        }

        private IHtmlDocument LoadHtmlDocument()
        {
            return LoadHtmlDocument(@"TestResponses\PersonResponse.html");
        }

        private IHtmlDocument LoadHtmlDocument(string path)
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
