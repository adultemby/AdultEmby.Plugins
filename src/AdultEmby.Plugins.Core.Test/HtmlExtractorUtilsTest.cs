using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using Xunit;

namespace AdultEmby.Plugins.Core.Test
{
    public class HtmlExtractorUtilsTest
    {
        [Fact]
        public void ShouldConvertValidStringRepresentionOfIntegerToInt()
        {
            Assert.Equal(123, HtmlExtractorUtils.ToInt("123"));
        }

        [Fact]
        public void ShouldConvertInvalidStringRepresentionOfIntegerToNull()
        {
            Assert.Equal(null, HtmlExtractorUtils.ToInt("ABC"));
        }

        [Fact]
        public void ShouldConvertNullStringToNull()
        {
            Assert.Equal(null, HtmlExtractorUtils.ToInt(null));
        }

        [Fact]
        public void ShouldConvertEmptyStringToInt()
        {
            Assert.Equal(null, HtmlExtractorUtils.ToInt(""));
        }

        [Fact]
        public void ShouldStripPrefixFromStringThatStartsWithPrefix()
        {
            Assert.Equal("value", HtmlExtractorUtils.StripPrefix("prefix", "prefixvalue"));
        }

        [Fact]
        public void ShouldNotStripPrefixIfValueDoesntStartWithPrefix()
        {
            Assert.Equal("value", HtmlExtractorUtils.StripPrefix("prefix", "value"));
        }

        [Fact]
        public void ShouldNotStripPrefixIfValueIsNull()
        {
            Assert.Equal(null, HtmlExtractorUtils.StripPrefix("prefix", null));
        }

        [Fact]
        public void ShouldNotStripPrefixIfPrefixIsNull()
        {
            Assert.Equal("value", HtmlExtractorUtils.StripPrefix(null, "value"));
        }

        [Fact]
        public void ShouldTrimWhitespaceFromValueWithLeadingWhitespace()
        {
            Assert.Equal("value", HtmlExtractorUtils.Trim("  value"));
        }

        [Fact]
        public void ShouldTrimWhitespaceFromValueWithTrailingWhitespace()
        {
            Assert.Equal("value", HtmlExtractorUtils.Trim("value  "));
        }

        [Fact]
        public void ShouldNotTrimWhitespaceFromValueWithoutLeadingOrTrailingWhitespace()
        {
            Assert.Equal("val ue", HtmlExtractorUtils.Trim("val ue"));
        }

        [Fact]
        public void ShouldNotTrimWhitespaceFromNullValue()
        {
            Assert.Equal(null, HtmlExtractorUtils.Trim(null));
        }

        [Fact]
        public void ShouldGetTrimmedAttributeValueFromElementIfAttributeExists()
        {
            HtmlParser htmlParser = new HtmlParser();
            IHtmlDocument htmlDocument = htmlParser.Parse("<html><img alt=' text '/></html>");
            IHtmlImageElement imageElement = (IHtmlImageElement) htmlDocument.QuerySelector("img");
            Assert.Equal("text", HtmlExtractorUtils.AttributeValue(imageElement, "alt"));
        }

        [Fact]
        public void ShouldGetNullFromElementIfAttributeDoesNotExist()
        {
            HtmlParser htmlParser = new HtmlParser();
            IHtmlDocument htmlDocument = htmlParser.Parse("<html><img alt=' text'/></html>");
            IHtmlImageElement imageElement = (IHtmlImageElement)htmlDocument.QuerySelector("img");
            Assert.Equal(null, HtmlExtractorUtils.AttributeValue(imageElement, "src"));
        }

        [Fact]
        public void ShouldGetTrimmedTextFromElementIfAttributeExists()
        {
            HtmlParser htmlParser = new HtmlParser();
            IHtmlDocument htmlDocument = htmlParser.Parse("<html><p> TEXT </p></html>");
            IHtmlParagraphElement paragraphElement = (IHtmlParagraphElement)htmlDocument.QuerySelector("p");
            Assert.Equal("TEXT", HtmlExtractorUtils.Text(paragraphElement));
        }

        [Fact]
        public void ShouldReturnNullFromElementIfAttributeDoesNotExist()
        {
            HtmlParser htmlParser = new HtmlParser();
            IHtmlDocument htmlDocument = htmlParser.Parse("<html><p> TEXT </p></html>");
            IHtmlImageElement imageElement = (IHtmlImageElement)htmlDocument.QuerySelector("img");
            Assert.Equal(null, HtmlExtractorUtils.Text(imageElement));
        }

        [Fact]
        public void HasSuffixShouldReturnTrueIfValueEndsWithSuffix()
        {
            Assert.Equal(true, HtmlExtractorUtils.HasSuffix("valuesuffix", "suffix"));
        }

        [Fact]
        public void HasSuffixShouldReturnFalseIfValueEndsWithSuffix()
        {
            Assert.Equal(false, HtmlExtractorUtils.HasSuffix("value", "suffix"));
        }

        [Fact]
        public void HasSuffixShouldReturnFalseIfValueIsNull()
        {
            Assert.Equal(false, HtmlExtractorUtils.HasSuffix(null, "suffix"));
        }

        [Fact]
        public void HasSuffixShouldReturnFalseIfSuffixIsNull()
        {
            Assert.Equal(false, HtmlExtractorUtils.HasSuffix("value", null));
        }

        [Fact]
        public void ShouldExtractPartFromPathWithSeparator()
        {
            Assert.Equal("first", HtmlExtractorUtils.Part("first/second", '/', 0));
            Assert.Equal("second", HtmlExtractorUtils.Part("first/second", '/', 1));
        }

        [Fact]
        public void ShouldExtractPartFromPathWithoutSeparator()
        {
            Assert.Equal("first", HtmlExtractorUtils.Part("first", '/', 0));
        }

        [Fact]
        public void ShouldFindElementContainingTextIfOneExists()
        {
            HtmlParser htmlParser = new HtmlParser();
            IHtmlDocument htmlDocument = htmlParser.Parse("<html><p><ul><li class='one'> TEXT </li><li class='two'> BLANK </li></ul></html>");
            IHtmlCollection<IElement> elements = htmlDocument.QuerySelectorAll("li");
            Assert.Equal("two", HtmlExtractorUtils.FindElementContainingText(elements, "BLANK").ClassName);
        }
    }
}
