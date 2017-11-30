using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdultEmby.Plugins.Base;
using AdultEmby.Plugins.Iafd;
using AdultEmby.Plugins.TestLogging;
using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using MediaBrowser.Model.Logging;
using NSubstitute;
using Xunit;

namespace AdultEmby.Plugins.Iafd.Test
{
    public class IafdMovieHtmlMetadataExtractorTest
    {
        [Fact]
        public void ShouldExtractTitle()
        {
            IHtmlMetadataExtractor htmlMetadataExtractor =
                new IafdMovieHtmlMetadataExtractor(LogManager());

            string title = htmlMetadataExtractor.GetTitle(loadHtmlDocument());

            Assert.Equal("Anal Lessons 1 (2012)", title);
        }

        [Fact]
        public void ShouldExtractPlot()
        {
            IHtmlMetadataExtractor htmlMetadataExtractor =
                new IafdMovieHtmlMetadataExtractor(LogManager());

            String plot = htmlMetadataExtractor.GetSynopsis(loadHtmlDocument());

            Assert.Null(plot);
        }

        [Fact]
        public void ShouldExtractGenres()
        {
            IHtmlMetadataExtractor htmlMetadataExtractor =
                new IafdMovieHtmlMetadataExtractor(LogManager());

            List<string> genres = htmlMetadataExtractor.GetGenres(loadHtmlDocument());

            Assert.True(!genres.Any());
        }

        [Fact]
        public void ShouldExtractStudio()
        {
            IHtmlMetadataExtractor htmlMetadataExtractor =
                new IafdMovieHtmlMetadataExtractor(LogManager());

            String studio = htmlMetadataExtractor.GetStudio(loadHtmlDocument());

            Assert.Equal("Mike Adriano Media", studio);
        }

        [Fact]
        public void ShouldExtractDirector()
        {
            IHtmlMetadataExtractor htmlMetadataExtractor =
                new IafdMovieHtmlMetadataExtractor(LogManager());

            MoviePerson director = htmlMetadataExtractor.GetDirector(loadHtmlDocument());

            Assert.Equal("Mike Adriano", director.Name);
        }

        [Fact]
        public void ShouldExtractReleaseDate()
        {
            IHtmlMetadataExtractor htmlMetadataExtractor =
                new IafdMovieHtmlMetadataExtractor(LogManager());

            DateTime? releaseDate = htmlMetadataExtractor.GetReleaseDate(loadHtmlDocument());

            Assert.Equal(null, releaseDate);
        }

        [Fact]
        public void ShouldExtractProductionYear()
        {
            IHtmlMetadataExtractor htmlMetadataExtractor =
                new IafdMovieHtmlMetadataExtractor(LogManager());

            int? productionYear = htmlMetadataExtractor.GetProductionYear(loadHtmlDocument());

            Assert.Equal(null, productionYear);
        }

        [Fact]
        public void ShouldExtractSet()
        {
            IHtmlMetadataExtractor htmlMetadataExtractor =
                new IafdMovieHtmlMetadataExtractor(LogManager());

            String set = htmlMetadataExtractor.GetSet(loadHtmlDocument());

            Assert.Equal(null, set);
        }

        [Fact]
        public void ShouldExtractUpc()
        {
            IHtmlMetadataExtractor htmlMetadataExtractor =
                new IafdMovieHtmlMetadataExtractor(LogManager());

            String upc = htmlMetadataExtractor.GetUpcCode(loadHtmlDocument());

            Assert.Equal(null, upc);
        }

        [Fact]
        public void ShouldExtractPrimaryImageUrl()
        {
            IHtmlMetadataExtractor htmlMetadataExtractor =
                new IafdMovieHtmlMetadataExtractor(LogManager());

            String primaryImageUrl = htmlMetadataExtractor.GetPrimaryImageUrl(loadHtmlDocument());

            Assert.Null(primaryImageUrl);
        }

        [Fact]
        public void ShouldExtractActors()
        {
            IHtmlMetadataExtractor htmlMetadataExtractor =
                new IafdMovieHtmlMetadataExtractor(LogManager());

            List<MoviePerson> actors = htmlMetadataExtractor.GetActors(loadHtmlDocument());

            validateActor("Christy Mack", "perfid=christymack/gender=f", actors[0]);
            validateActor("Gabriella Paltrova", "perfid=gabriellapaltrova/gender=f", actors[1]);
            validateActor("Jessie Rogers", "perfid=jessierogers/gender=f", actors[2]);
            validateActor("Melina Mason", "perfid=melinamason/gender=f", actors[3]);
            validateActor("Mike Adriano", "perfid=mikeadriano/gender=m", actors[4]);
            validateActor("Penny Pax", "perfid=pennypax/gender=f", actors[5]);
            validateActor("Scarlett Wild", "perfid=scarlettwild/gender=f", actors[6]);
            validateActor("Sheena Shaw", "perfid=sheenashaw/gender=f", actors[7]);
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
