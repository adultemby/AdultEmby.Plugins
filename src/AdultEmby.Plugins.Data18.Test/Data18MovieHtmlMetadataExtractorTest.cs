using System;
using System.Collections.Generic;
using System.IO;
using AdultEmby.Plugins.Core;
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
    public class Data18MovieHtmlMetadataExtractorTest
    {
        [Fact]
        public void ShouldExtractTitle()
        {
            IHtmlMetadataExtractor htmlMetadataExtractor =
                new Data18MovieHtmlMetadataExtractor(LogManager());

            String title = htmlMetadataExtractor.GetTitle(loadHtmlDocument());

            Assert.Equal("Anal Lessons #1", title);
        }

        [Fact]
        public void ShouldExtractPlot()
        {
            IHtmlMetadataExtractor htmlMetadataExtractor =
                new Data18MovieHtmlMetadataExtractor(LogManager());

            String plot = htmlMetadataExtractor.GetSynopsis(loadHtmlDocument());

            Assert.Equal("Director and stud Mike Adriano is not only an obsessive devotee of anal fucking; he considers himself a teacher, as well. When frisky females visit his home for a few of his trademark activities - POV-style blow jobs, nasty ass-to-mouth taste testing, extreme anal gapes, rim jobs and all manner of rectal fun - they know that Mike's tireless cock and perverse enthusiasm are bound to satisfy their thirst for knowledge. The double-disc \"Anal Lessons\" offers five filthy, intense butt-ramming scenes (plus a bonus session featuring Melina Mason) filling nearly six hours, and stars some of the most exquisitely talented backdoor beauties in the adult industry. Even experienced butt-sluts can learn s...", plot);
        }

        [Fact]
        public void ShouldExtractGenres()
        {
            IHtmlMetadataExtractor htmlMetadataExtractor =
                new Data18MovieHtmlMetadataExtractor(LogManager());

            List<string> genres = htmlMetadataExtractor.GetGenres(loadHtmlDocument());

            Assert.Equal("Anal", genres[0]);
            Assert.Equal("Gonzo", genres[1]);
        }

        [Fact]
        public void ShouldExtractStudio()
        {
            IHtmlMetadataExtractor htmlMetadataExtractor =
                new Data18MovieHtmlMetadataExtractor(LogManager());

            String studio = htmlMetadataExtractor.GetStudio(loadHtmlDocument());

            Assert.Equal("Evil Angel", studio);
        }

        [Fact]
        public void ShouldExtractDirector()
        {
            IHtmlMetadataExtractor htmlMetadataExtractor =
                new Data18MovieHtmlMetadataExtractor(LogManager());

            MoviePerson director = htmlMetadataExtractor.GetDirector(loadHtmlDocument());

            Assert.Equal("Mike Adriano", director.Name);
        }

        [Fact]
        public void ShouldExtractReleaseDate()
        {
            IHtmlMetadataExtractor htmlMetadataExtractor =
                new Data18MovieHtmlMetadataExtractor(LogManager());

            DateTime? releaseDate = htmlMetadataExtractor.GetReleaseDate(loadHtmlDocument());

            Assert.Equal(new DateTime(2012, 7, 1), releaseDate);
        }

        [Fact]
        public void ShouldExtractProductionYear()
        {
            IHtmlMetadataExtractor htmlMetadataExtractor =
                new Data18MovieHtmlMetadataExtractor(LogManager());

            int? productionYear = htmlMetadataExtractor.GetProductionYear(loadHtmlDocument());

            Assert.Equal(2012, productionYear.Value);
        }

        [Fact]
        public void ShouldExtractSet()
        {
            IHtmlMetadataExtractor htmlMetadataExtractor =
                new Data18MovieHtmlMetadataExtractor(LogManager());

            String set = htmlMetadataExtractor.GetSet(loadHtmlDocument());

            Assert.Equal("Anal Lessons", set);
        }

        [Fact]
        public void ShouldExtractUpc()
        {
            IHtmlMetadataExtractor htmlMetadataExtractor =
                new Data18MovieHtmlMetadataExtractor(LogManager());

            String set = htmlMetadataExtractor.GetUpcCode(loadHtmlDocument());

            Assert.Equal("746183005806", set);
        }

        [Fact]
        public void ShouldExtractPrimaryImageUrl()
        {
            IHtmlMetadataExtractor htmlMetadataExtractor =
                new Data18MovieHtmlMetadataExtractor(LogManager());

            String primaryImageUrl = htmlMetadataExtractor.GetPrimaryImageUrl(loadHtmlDocument());

            Assert.Equal("http://img.data18.com/covers/1/1/22604_front.jpg", primaryImageUrl);
        }

        [Fact]
        public void ShouldExtractActors()
        {
            IHtmlMetadataExtractor htmlMetadataExtractor =
                new Data18MovieHtmlMetadataExtractor(LogManager());

            List<MoviePerson> actors = htmlMetadataExtractor.GetActors(loadHtmlDocument());

            validateActor("Christy Mack", "christy_mack", actors[0]);
            validateActor("Gabriella Paltrova", "gabriella_paltrova", actors[1]);
            validateActor("Jessie Rogers", "jessie_rogers", actors[2]);
            validateActor("Melina Mason", "melina_mason", actors[3]);
            validateActor("Mike Adriano", "mike_adriano", actors[4]);
            validateActor("Penny Pax", "penny_pax", actors[5]);
            validateActor("Scarlett Wild", "scarlett_wild", actors[6]);
            validateActor("Sheena Shaw", "sheena_shaw", actors[7]);
        }

        private void validateActor(string name, string id, MoviePerson actor)
        {
            Assert.Equal(id, actor.Id);
            Assert.Equal(name, actor.Name);
        }

        private IHtmlDocument loadHtmlDocument()
        {
            Stream responseStream = File.OpenRead(@"TestResponses\MovieResponse.html");
            var parser = new HtmlParser();
            return parser.Parse(responseStream);
        }

        private ILogManager LogManager()
        {
            return TestLogManager.Instance;
        }
    }
}
