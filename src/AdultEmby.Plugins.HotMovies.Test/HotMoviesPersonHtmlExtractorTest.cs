using System;
using System.IO;
using AdultEmby.Plugins.Base;
using AdultEmby.Plugins.HotMovies;
using AdultEmby.Plugins.TestLogging;
using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using MediaBrowser.Model.Logging;
using NSubstitute;
using NSubstitute.Core;
using Xunit;

namespace AdultEmby.Plugins.HotMovies.Test
{
    public class HotMoviesPersonHtmlExtractorTest
    {
        [Fact]
        public void ShouldHaveMetadata()
        {
            IHtmlPersonExtractor htmlMetadataExtractor =
                new HotMoviesPersonHtmlExtractor(LogManager());

            bool hasMetadata = htmlMetadataExtractor.HasMetadata(loadHtmlDocument());

            Assert.True(hasMetadata);
        }

        [Fact]
        public void ShouldNotHaveMetadataIfHeaderIsFalse()
        {
            IHtmlPersonExtractor htmlMetadataExtractor =
                new HotMoviesPersonHtmlExtractor(LogManager());

            bool hasMetadata = htmlMetadataExtractor.HasMetadata(loadHtmlDocument(@"TestResponses\NoMetadata.html"));

            Assert.False(hasMetadata);
        }

        [Fact]
        public void ShouldExtractName()
        {
            IHtmlPersonExtractor htmlMetadataExtractor =
                new HotMoviesPersonHtmlExtractor(LogManager());

            String title = htmlMetadataExtractor.GetName(loadHtmlDocument());

            Assert.Equal(null, title);
        }

        [Fact]
        public void ShouldExtractBirthdate()
        {
            IHtmlPersonExtractor htmlMetadataExtractor =
                new HotMoviesPersonHtmlExtractor(LogManager());

            DateTime? birthdate = htmlMetadataExtractor.GetBirthdate(loadHtmlDocument());

            Assert.Equal(null, birthdate);
        }

        [Fact]
        public void ShouldExtractNationality()
        {
            IHtmlPersonExtractor htmlMetadataExtractor =
                new HotMoviesPersonHtmlExtractor(LogManager());

            String birthdate = htmlMetadataExtractor.GetNationality(loadHtmlDocument());

            Assert.Equal(null, birthdate);
        }

        [Fact]
        public void ShouldExtractBirthplace()
        {
            IHtmlPersonExtractor htmlMetadataExtractor =
                new HotMoviesPersonHtmlExtractor(LogManager());

            String birthplace = htmlMetadataExtractor.GetBirthplace(loadHtmlDocument());

            Assert.Equal("Houston, Texas United States", birthplace);
        }

        [Fact]
        public void ShouldExtractStarSign()
        {
            IHtmlPersonExtractor htmlMetadataExtractor =
                new HotMoviesPersonHtmlExtractor(LogManager());

            String starSign = htmlMetadataExtractor.GetStarSign(loadHtmlDocument());

            Assert.Equal("Libra", starSign);
        }

        [Fact]
        public void ShouldExtractEthnicity()
        {
            IHtmlPersonExtractor htmlMetadataExtractor =
                new HotMoviesPersonHtmlExtractor(LogManager());

            String ethnicity = htmlMetadataExtractor.GetEthnicity(loadHtmlDocument());

            Assert.Equal("Caucasian", ethnicity);
        }

        [Fact]
        public void ShouldExtractMeasurements()
        {
            IHtmlPersonExtractor htmlMetadataExtractor =
                new HotMoviesPersonHtmlExtractor(LogManager());

            String measurements = htmlMetadataExtractor.GetMeasurements(loadHtmlDocument());

            Assert.Equal("32D", measurements);
        }

        [Fact]
        public void ShouldExtractHeight()
        {
            IHtmlPersonExtractor htmlMetadataExtractor =
                new HotMoviesPersonHtmlExtractor(LogManager());

            String height = htmlMetadataExtractor.GetHeight(loadHtmlDocument());

            Assert.Equal("5ft 3in", height);
        }

        [Fact]
        public void ShouldExtractWeight()
        {
            IHtmlPersonExtractor htmlMetadataExtractor =
                new HotMoviesPersonHtmlExtractor(LogManager());

            String weight = htmlMetadataExtractor.GetWeight(loadHtmlDocument());

            Assert.Equal(null, weight);
        }

        [Fact]
        public void ShouldExtractTwitter()
        {
            IHtmlPersonExtractor htmlMetadataExtractor =
                new HotMoviesPersonHtmlExtractor(LogManager());

            String twitter = htmlMetadataExtractor.GetTwitter(loadHtmlDocument());

            Assert.Equal(null, twitter);
        }

        [Fact]
        public void ShouldExtractPrimaryImageUrlFromStarGalleryIfPresent()
        {
            IHtmlPersonExtractor htmlMetadataExtractor =
                new HotMoviesPersonHtmlExtractor(LogManager());

            String url = htmlMetadataExtractor.GetPrimaryImageUrl(loadHtmlDocument());

            Assert.Equal("https://img2.vod.com/image2/star/488/Bree_Olson-4887.10627212671415801793large.jpg", url);
        }

        [Fact]
        public void ShouldExtractPrimaryImageUrlFromStarImageIfGalleryNotPresent()
        {
            IHtmlPersonExtractor htmlMetadataExtractor =
                new HotMoviesPersonHtmlExtractor(LogManager());

            String url = htmlMetadataExtractor.GetPrimaryImageUrl(loadHtmlDocument(@"TestResponses\PersonResponseNoStarGallery.html"));

            Assert.Equal("https://img2.vod.com/image2/star/202/Bianca_aus-202498.1.jpg", url);
        }

        private IHtmlDocument loadHtmlDocument()
        {
            return loadHtmlDocument(@"TestResponses\PersonResponse.html");
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
