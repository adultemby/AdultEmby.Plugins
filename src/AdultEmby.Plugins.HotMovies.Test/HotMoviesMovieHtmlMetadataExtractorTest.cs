using System;
using System.Collections.Generic;
using System.IO;
using AdultEmby.Plugins.Base;
using AdultEmby.Plugins.HotMovies;
using AdultEmby.Plugins.TestLogging;
using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Logging;
using NSubstitute;
using Xunit;

namespace AdultEmby.Plugins.HotMovies.Test
{
    public class HotMoviesMovieHtmlMetadataExtractorTest
    {
        [Fact]
        public void ShouldExtractTitle()
        {
            IHtmlMetadataExtractor htmlMetadataExtractor =
                new HotMoviesMovieHtmlMetadataExtractor(LogManager());

            String title = htmlMetadataExtractor.GetTitle(loadHtmlDocument());

            Assert.Equal("Anal Lessons (Disc 1)", title);
        }

        [Fact]
        public void ShouldExtractPlot()
        {
            IHtmlMetadataExtractor htmlMetadataExtractor =
                new HotMoviesMovieHtmlMetadataExtractor(LogManager());

            String plot = htmlMetadataExtractor.GetSynopsis(loadHtmlDocument());

            Assert.True(plot.StartsWith("This movie features women that need a lesson in getting"));
        }

        [Fact]
        public void ShouldExtractGenres()
        {
            IHtmlMetadataExtractor htmlMetadataExtractor =
                new HotMoviesMovieHtmlMetadataExtractor(LogManager());

            List<string> genres = htmlMetadataExtractor.GetGenres(loadHtmlDocument());

            Assert.Equal("Anal -> M On F", genres[0]);
            Assert.Equal("Gonzo -> Anal", genres[1]);
        }

        [Fact]
        public void ShouldExtractStudio()
        {
            IHtmlMetadataExtractor htmlMetadataExtractor =
                new HotMoviesMovieHtmlMetadataExtractor(LogManager());

            String studio = htmlMetadataExtractor.GetStudio(loadHtmlDocument());

            Assert.Equal("Mike Adriano Media", studio);
        }

        [Fact]
        public void ShouldExtractDirector()
        {
            IHtmlMetadataExtractor htmlMetadataExtractor =
                new HotMoviesMovieHtmlMetadataExtractor(LogManager());

            MoviePerson director = htmlMetadataExtractor.GetDirector(loadHtmlDocument());

            Assert.Equal("Mike Adriano", director.Name);
        }

        [Fact]
        public void ShouldExtractReleaseDate()
        {
            IHtmlMetadataExtractor htmlMetadataExtractor =
                new HotMoviesMovieHtmlMetadataExtractor(LogManager());

            DateTime? releaseDate = htmlMetadataExtractor.GetReleaseDate(loadHtmlDocument());

            Assert.Equal(null, releaseDate);
        }

        [Fact]
        public void ShouldExtractProductionYear()
        {
            IHtmlMetadataExtractor htmlMetadataExtractor =
                new HotMoviesMovieHtmlMetadataExtractor(LogManager());

            int? productionYear = htmlMetadataExtractor.GetProductionYear(loadHtmlDocument());

            Assert.Equal(2012, productionYear);
        }

        [Fact]
        public void ShouldExtractSet()
        {
            IHtmlMetadataExtractor htmlMetadataExtractor =
                new HotMoviesMovieHtmlMetadataExtractor(LogManager());

            String set = htmlMetadataExtractor.GetSet(loadHtmlDocument());

            Assert.Equal("Anal Lessons", set);
        }

        [Fact]
        public void ShouldExtractUpc()
        {
            IHtmlMetadataExtractor htmlMetadataExtractor =
                new HotMoviesMovieHtmlMetadataExtractor(LogManager());

            String upc = htmlMetadataExtractor.GetUpcCode(loadHtmlDocument());

            Assert.Equal(null, upc);
        }

        [Fact]
        public void ShouldExtractPrimaryImageUrl()
        {
            IHtmlMetadataExtractor htmlMetadataExtractor =
                new HotMoviesMovieHtmlMetadataExtractor(LogManager());

            String primaryImageUrl = htmlMetadataExtractor.GetPrimaryImageUrl(loadHtmlDocument());

            Assert.Equal("https://img2.vod.com/image2/cover/215/215223.cover.0.jpg", primaryImageUrl);
        }

        [Fact]
        public void ShouldExtractActors()
        {
            IHtmlMetadataExtractor htmlMetadataExtractor =
                new HotMoviesMovieHtmlMetadataExtractor(LogManager());

            List<MoviePerson> actors = htmlMetadataExtractor.GetActors(loadHtmlDocument());

            validateActor("Christy Mack", "156190", actors[0]);
            validateActor("Jessie Rogers", "149060", actors[1]);
            validateActor("Sheena Shaw", "151176", actors[2]);
            validateActor("Gabriella Paltrova", "152228", actors[3]);
            validateActor("Penny Pax", "154028", actors[4]);
            validateActor("Scarlett Wild", "154341", actors[5]);
            validateActor("Mike Adriano", "40925", actors[6]);
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
