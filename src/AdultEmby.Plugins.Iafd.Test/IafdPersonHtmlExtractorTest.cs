using System;
using System.IO;
using AdultEmby.Plugins.Core;
using AdultEmby.Plugins.Iafd;
using AdultEmby.Plugins.TestLogging;
using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using MediaBrowser.Model.Logging;
using Xunit;

namespace AdultEmby.Plugins.Iafd.Test
{
    public class IafdPersonHtmlExtractorTest
    {
        [Fact]
        public void ShouldHaveMetadata()
        {
            IHtmlPersonExtractor htmlMetadataExtractor =
                new IafdPersonHtmlExtractor(LogManager());

            bool hasMetadata = htmlMetadataExtractor.HasMetadata(LoadHtmlDocument());

            Assert.True(hasMetadata);
        }

        [Fact]
        public void ShouldNotHaveMetadataIfHeaderIsFalse()
        {
            IHtmlPersonExtractor htmlMetadataExtractor =
                new IafdPersonHtmlExtractor(LogManager());

            bool hasMetadata = htmlMetadataExtractor.HasMetadata(LoadHtmlDocument(@"TestResponses\NoMetadata.html"));

            Assert.False(hasMetadata);
        }

        [Fact]
        public void ShouldExtractName()
        {
            IHtmlPersonExtractor htmlMetadataExtractor =
                new IafdPersonHtmlExtractor(LogManager());

            String title = htmlMetadataExtractor.GetName(LoadHtmlDocument());

            Assert.Equal("Jessie Rogers", title);
        }

        [Fact]
        public void ShouldExtractBirthdate()
        {
            IHtmlPersonExtractor htmlMetadataExtractor =
                new IafdPersonHtmlExtractor(LogManager());

            DateTime? birthdate = htmlMetadataExtractor.GetBirthdate(LoadHtmlDocument());

            Assert.Equal(new DateTime(1993, 8, 8), birthdate);
        }

        [Fact]
        public void ShouldExtractNationality()
        {
            IHtmlPersonExtractor htmlMetadataExtractor =
                new IafdPersonHtmlExtractor(LogManager());

            String birthdate = htmlMetadataExtractor.GetNationality(LoadHtmlDocument());

            Assert.Equal("Brazilian", birthdate);
        }

        [Fact]
        public void ShouldExtractBirthplace()
        {
            IHtmlPersonExtractor htmlMetadataExtractor =
                new IafdPersonHtmlExtractor(LogManager());

            String birthplace = htmlMetadataExtractor.GetBirthplace(LoadHtmlDocument());

            Assert.Equal("Goiania, Brazil", birthplace);
        }

        [Fact]
        public void ShouldExtractStarSign()
        {
            IHtmlPersonExtractor htmlMetadataExtractor =
                new IafdPersonHtmlExtractor(LogManager());

            String starSign = htmlMetadataExtractor.GetStarSign(LoadHtmlDocument());

            Assert.Equal("Leo", starSign);
        }

        [Fact]
        public void ShouldExtractEthnicity()
        {
            IHtmlPersonExtractor htmlMetadataExtractor =
                new IafdPersonHtmlExtractor(LogManager());

            String ethnicity = htmlMetadataExtractor.GetEthnicity(LoadHtmlDocument());

            Assert.Equal("Latin", ethnicity);
        }

        [Fact]
        public void ShouldExtractMeasurements()
        {
            IHtmlPersonExtractor htmlMetadataExtractor =
                new IafdPersonHtmlExtractor(LogManager());

            String measurements = htmlMetadataExtractor.GetMeasurements(LoadHtmlDocument());

            Assert.Equal("34D-24-37", measurements);
        }

        [Fact]
        public void ShouldExtractHeight()
        {
            IHtmlPersonExtractor htmlMetadataExtractor =
                new IafdPersonHtmlExtractor(LogManager());

            String height = htmlMetadataExtractor.GetHeight(LoadHtmlDocument());

            Assert.Equal("5 feet, 7 inches (170 cm)", height);
        }

        [Fact]
        public void ShouldExtractWeight()
        {
            IHtmlPersonExtractor htmlMetadataExtractor =
                new IafdPersonHtmlExtractor(LogManager());

            String weight = htmlMetadataExtractor.GetWeight(LoadHtmlDocument());

            Assert.Equal("121 lbs (55 kg)", weight);
        }

        [Fact]
        public void ShouldExtractTwitter()
        {
            IHtmlPersonExtractor htmlMetadataExtractor =
                new IafdPersonHtmlExtractor(LogManager());

            String twitter = htmlMetadataExtractor.GetTwitter(LoadHtmlDocument());

            Assert.Equal(null, twitter);
        }

        [Fact]
        public void ShouldExtractPrimaryImageUrlFromStarGalleryIfPresent()
        {
            IHtmlPersonExtractor htmlMetadataExtractor =
                new IafdPersonHtmlExtractor(LogManager());

            string url = htmlMetadataExtractor.GetPrimaryImageUrl(LoadHtmlDocument());

            Assert.Equal("http://www.iafd.com/graphics/headshots/jessierogers_f_jessierogers_twisty.jpg", url);
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
