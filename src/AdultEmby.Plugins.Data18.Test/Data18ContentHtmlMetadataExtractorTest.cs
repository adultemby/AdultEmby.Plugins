using System;
using System.Collections.Generic;
using System.IO;
using AdultEmby.Plugins.Base;
using AdultEmby.Plugins.Data18;
using AdultEmby.Plugins.TestLogging;
using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Logging;
using NSubstitute;
using Xunit;

namespace AdultEmby.Plugins.Data18.Test
{
    public class Data18ContentHtmlMetadataExtractorTest
    {
        [Fact]
        public void ShouldExtractTitle()
        {
            IHtmlMetadataExtractor htmlMetadataExtractor =
                new Data18ContentHtmlMetadataExtractor(LogManager());

            String title = htmlMetadataExtractor.GetTitle(LoadHtmlDocument());

            Assert.Equal("He Walked In", title);
        }

        [Fact]
        public void ShouldExtractPlot()
        {
            IHtmlMetadataExtractor htmlMetadataExtractor =
                new Data18ContentHtmlMetadataExtractor(LogManager());

            String plot = htmlMetadataExtractor.GetSynopsis(LoadHtmlDocument());

            Assert.Equal("Natasha and Chad start getting down with it in the office and Toby walks right in! Not to worry, he can join. There's plenty of horny Natasha's deepthroating mouth to go around as the boys soon discover. She's worn her lacy black pantyhose stockings and hoisters that highlight her hourglass figure perfectly during this threesome. Enjoy the sights!", plot);
        }

        [Fact]
        public void ShouldExtractGenres()
        {
            IHtmlMetadataExtractor htmlMetadataExtractor =
                new Data18ContentHtmlMetadataExtractor(LogManager());

            List<string> genres = htmlMetadataExtractor.GetGenres(LoadHtmlDocument());

            Assert.Equal("Heterosexual", genres[0]);
            Assert.Equal("Hardcore", genres[1]);
        }

        [Fact]
        public void ShouldUseSiteIfPresentWhenExtractingStudio()
        {
            IHtmlMetadataExtractor htmlMetadataExtractor =
                new Data18ContentHtmlMetadataExtractor(LogManager());

            String studio = htmlMetadataExtractor.GetStudio(LoadHtmlDocument(@"TestResponses\ContentWithStudioResponse.html"));

            Assert.Equal("Private", studio);
        }

        [Fact]
        public void ShouldUseStudioIfSiteNotPresentWhenExtractingStudio()
        {
            IHtmlMetadataExtractor htmlMetadataExtractor =
                new Data18ContentHtmlMetadataExtractor(LogManager());

            String studio = htmlMetadataExtractor.GetStudio(LoadHtmlDocument());

            Assert.Equal("Dpfanatics", studio);
        }

        [Fact]
        public void ShouldExtractDirector()
        {
            IHtmlMetadataExtractor htmlMetadataExtractor =
                new Data18ContentHtmlMetadataExtractor(LogManager());

            MoviePerson director = htmlMetadataExtractor.GetDirector(LoadHtmlDocument());

            Assert.Equal(null, director);
        }

        [Fact]
        public void ShouldExtractReleaseDate()
        {
            IHtmlMetadataExtractor htmlMetadataExtractor =
                new Data18ContentHtmlMetadataExtractor(LogManager());

            DateTime? releaseDate = htmlMetadataExtractor.GetReleaseDate(LoadHtmlDocument());

            Assert.Equal(new DateTime(2016, 8, 7), releaseDate);
        }

        [Fact]
        public void ShouldExtractProductionYear()
        {
            IHtmlMetadataExtractor htmlMetadataExtractor =
                new Data18ContentHtmlMetadataExtractor(LogManager());

            int? productionYear = htmlMetadataExtractor.GetProductionYear(LoadHtmlDocument());

            Assert.Equal(null, productionYear);
        }

        [Fact]
        public void ShouldExtractSet()
        {
            IHtmlMetadataExtractor htmlMetadataExtractor =
                new Data18ContentHtmlMetadataExtractor(LogManager());

            String set = htmlMetadataExtractor.GetSet(LoadHtmlDocument());

            Assert.Equal(null, set);
        }

        [Fact]
        public void ShouldExtractUpc()
        {
            IHtmlMetadataExtractor htmlMetadataExtractor =
                new Data18ContentHtmlMetadataExtractor(LogManager());

            String set = htmlMetadataExtractor.GetUpcCode(LoadHtmlDocument());

            Assert.Equal(null, set);
        }

        [Fact]
        public void ShouldExtractPrimaryImageUrl()
        {
            IHtmlMetadataExtractor htmlMetadataExtractor =
                new Data18ContentHtmlMetadataExtractor(LogManager());

            String primaryImageUrl = htmlMetadataExtractor.GetPrimaryImageUrl(LoadHtmlDocument());

            Assert.Equal("http://media.data18.com/scenes/1/7/165227-natasha-starr-toby-chad-rockwell-dpfanatics.jpg", primaryImageUrl);
        }

        [Fact]
        public void ShouldExtractActorsUsingWhosWhoSectionIsPresent()
        {
            IHtmlMetadataExtractor htmlMetadataExtractor =
                new Data18ContentHtmlMetadataExtractor(LogManager());

            List<MoviePerson> actors = htmlMetadataExtractor.GetActors(LoadHtmlDocument());

            validateActor("Natasha Starr", "natasha_starr", actors[0]);
            validateActor("Toby", "toby", actors[1]);
            validateActor("Chad Rockwell", "chad_rockwell", actors[2]);
        }

        [Fact]
        public void ShouldExtractActorsUsingStaringBulletIfWhosWhoSectionIsNotPresent()
        {
            IHtmlMetadataExtractor htmlMetadataExtractor =
                new Data18ContentHtmlMetadataExtractor(LogManager());

            List<MoviePerson> actors = htmlMetadataExtractor.GetActors(LoadHtmlDocument(@"TestResponses\ContentWithoutWhosWhoResponse.html"));

            validateActor("Yuliana", "yuliana", actors[0]);
        }

        private void validateActor(string name, string id, MoviePerson actor)
        {
            Assert.Equal(id, actor.Id);
            Assert.Equal(name, actor.Name);
        }

        private IHtmlDocument LoadHtmlDocument()
        {
            return LoadHtmlDocument(@"TestResponses\ContentResponse.html");
        }

        private IHtmlDocument LoadHtmlDocument(string filename)
        {
            Stream responseStream = File.OpenRead(filename);
            var parser = new HtmlParser();
            return parser.Parse(responseStream);
        }

        private ILogManager LogManager()
        {
            return TestLogManager.Instance;
        }
    }
}
