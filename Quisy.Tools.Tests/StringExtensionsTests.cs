using Quisy.Tools.Extensions;
using Xunit;

namespace Quisy.Tools.Tests
{
    public class StringExtensionsTests
    {
        [Theory]
        [InlineData("USD $ 219.99 dollars")]
        [InlineData("$ 219.00 dollars")]
        [InlineData("$219")]
        [InlineData("219")]
        public void FormatPrice_WithAdditionalSymbolsAndNumbers(string price)
        {
            Assert.Equal("219", price.FormatCurrency());
        }
    }
}
