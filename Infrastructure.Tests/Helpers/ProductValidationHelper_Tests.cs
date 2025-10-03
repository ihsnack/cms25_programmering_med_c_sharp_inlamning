using System.Globalization;
using Infrastructure.Helpers;
using Xunit;

namespace Infrastructure.Tests.Helpers;

/// <summary>
/// I've used Copilot to improve these tests and asked it adjust tests after refactorings
/// </summary>
/// 
public class ProductValidationHelper_Tests
{
    [Fact]
    public void IsValidPrice_ValidPrice_ReturnsTrue()
    {
        // arrange
        var validPrice = 10.99m;

        // act
        var result = ProductValidationHelper.IsValidPrice(validPrice);

        // assert
        Assert.True(result);
    }

    [Fact]
    public void IsValidPrice_ZeroPrice_ReturnsFalse()
    {
        // arrange
        var zeroPrice = 0m;

        // act
        var result = ProductValidationHelper.IsValidPrice(zeroPrice);

        // assert
        Assert.False(result);
    }

    [Fact]
    public void IsValidPrice_NegativePrice_ReturnsFalse()
    {
        // arrange
        var negativePrice = -5.99m;

        // act
        var result = ProductValidationHelper.IsValidPrice(negativePrice);

        // assert
        Assert.False(result);
    }

    [Fact]
    public void IsValidPrice_MaxValue_ReturnsFalse()
    {
        // arrange
        var maxPrice = decimal.MaxValue;

        // act
        var result = ProductValidationHelper.IsValidPrice(maxPrice);

        // assert
        Assert.False(result);
    }

    [Fact]
    public void IsValidPrice_MinValue_ReturnsFalse()
    {
        // arrange
        var minPrice = decimal.MinValue;

        // act
        var result = ProductValidationHelper.IsValidPrice(minPrice);

        // assert
        Assert.False(result);
    }

    [Fact]
    public void TryParsePrice_ValidPriceString_10_99_ReturnsTrueAndCorrectPrice()
    {
        // arrange
        string input = "10.99";
        decimal expectedPrice = 10.99m;

        // act
        var result = ProductValidationHelper.TryParsePrice(input, out decimal actualPrice);

        // assert
        Assert.True(result);
        Assert.Equal(expectedPrice, actualPrice);
    }

    [Fact]
    public void TryParsePrice_ValidPriceString_1000_00_ReturnsTrueAndCorrectPrice()
    {
        // arrange
        string input = "1000.00";
        decimal expectedPrice = 1000.00m;

        // act
        var result = ProductValidationHelper.TryParsePrice(input, out decimal actualPrice);

        // assert
        Assert.True(result);
        Assert.Equal(expectedPrice, actualPrice);
    }

    [Fact]
    public void TryParsePrice_ValidPriceString_0_01_ReturnsTrueAndCorrectPrice()
    {
        // arrange
        string input = "0.01";
        decimal expectedPrice = 0.01m;

        // act
        var result = ProductValidationHelper.TryParsePrice(input, out decimal actualPrice);

        // assert
        Assert.True(result);
        Assert.Equal(expectedPrice, actualPrice);
    }

    [Fact]
    public void TryParsePrice_ValidPriceString_999999_99_ReturnsTrueAndCorrectPrice()
    {
        // arrange
        string input = "999999.99";
        decimal expectedPrice = 999999.99m;

        // act
        var result = ProductValidationHelper.TryParsePrice(input, out decimal actualPrice);

        // assert
        Assert.True(result);
        Assert.Equal(expectedPrice, actualPrice);
    }

    [Fact]
    public void TryParsePrice_InvalidPriceString_Zero_ReturnsFalse()
    {
        // arrange
        string input = "0";

        // act
        var result = ProductValidationHelper.TryParsePrice(input, out decimal price);

        // assert
        Assert.False(result);
        Assert.Equal(0, price);
    }

    [Fact]
    public void TryParsePrice_InvalidPriceString_ZeroDecimal_ReturnsFalse()
    {
        // arrange
        string input = "0.00";

        // act
        var result = ProductValidationHelper.TryParsePrice(input, out decimal price);

        // assert
        Assert.False(result);
        Assert.Equal(0, price);
    }

    [Fact]
    public void TryParsePrice_InvalidPriceString_Negative_ReturnsFalse()
    {
        // arrange
        string input = "-5.99";

        // act
        var result = ProductValidationHelper.TryParsePrice(input, out decimal price);

        // assert
        Assert.False(result);
        Assert.Equal(0, price);
    }

    [Fact]
    public void TryParsePrice_InvalidPriceString_Letters_ReturnsFalse()
    {
        // arrange
        string input = "abc";

        // act
        var result = ProductValidationHelper.TryParsePrice(input, out decimal price);

        // assert
        Assert.False(result);
        Assert.Equal(0, price);
    }

    [Fact]
    public void TryParsePrice_InvalidPriceString_Empty_ReturnsFalse()
    {
        // arrange
        string input = "";

        // act
        var result = ProductValidationHelper.TryParsePrice(input, out decimal price);

        // assert
        Assert.False(result);
        Assert.Equal(0, price);
    }

    [Fact]
    public void TryParsePrice_InvalidPriceString_Whitespace_ReturnsFalse()
    {
        // arrange
        string input = " ";

        // act
        var result = ProductValidationHelper.TryParsePrice(input, out decimal price);

        // assert
        Assert.False(result);
        Assert.Equal(0, price);
    }

    [Fact]
    public void TryParsePrice_InvalidPriceString_MultipleCommas_ReturnsFalse()
    {
        // arrange
        string input = "10,99,99"; // Multiple commas

        // act
        var result = ProductValidationHelper.TryParsePrice(input, out decimal price);

        // assert
        Assert.False(result);
        Assert.Equal(0, price);
    }

    [Fact]
    public void TryParsePrice_InvalidPriceString_NotANumber_ReturnsFalse()
    {
        // arrange
        string input = "not a number";

        // act
        var result = ProductValidationHelper.TryParsePrice(input, out decimal price);

        // assert
        Assert.False(result);
        Assert.Equal(0, price);
    }

    [Fact]
    public void IsValidTitle_ValidTitle_ReturnsTrue()
    {
        // arrange
        string title = "Valid Title";

        // act
        var result = ProductValidationHelper.IsValidTitle(title);

        // assert
        Assert.True(result);
    }

    [Fact]
    public void IsValidTitle_SingleCharacter_ReturnsTrue()
    {
        // arrange
        string title = "A";

        // act
        var result = ProductValidationHelper.IsValidTitle(title);

        // assert
        Assert.True(result);
    }

    [Fact]
    public void IsValidTitle_ProductWithNumbers_ReturnsTrue()
    {
        // arrange
        string title = "Product with Numbers 123";

        // act
        var result = ProductValidationHelper.IsValidTitle(title);

        // assert
        Assert.True(result);
    }

    [Fact]
    public void IsValidTitle_ProductWithSpecialCharacters_ReturnsTrue()
    {
        // arrange
        string title = "Product with Special Characters едц!@#";

        // act
        var result = ProductValidationHelper.IsValidTitle(title);

        // assert
        Assert.True(result);
    }

    [Fact]
    public void IsValidTitle_EmptyString_ReturnsFalse()
    {
        // arrange
        string? title = "";

        // act
        var result = ProductValidationHelper.IsValidTitle(title);

        // assert
        Assert.False(result);
    }

    [Fact]
    public void IsValidTitle_Whitespace_ReturnsFalse()
    {
        // arrange
        string? title = " ";

        // act
        var result = ProductValidationHelper.IsValidTitle(title);

        // assert
        Assert.False(result);
    }

    [Fact]
    public void IsValidTitle_Tab_ReturnsFalse()
    {
        // arrange
        string? title = "\t";

        // act
        var result = ProductValidationHelper.IsValidTitle(title);

        // assert
        Assert.False(result);
    }

    [Fact]
    public void IsValidTitle_Newline_ReturnsFalse()
    {
        // arrange
        string? title = "\n";

        // act
        var result = ProductValidationHelper.IsValidTitle(title);

        // assert
        Assert.False(result);
    }

    [Fact]
    public void IsValidTitle_Null_ReturnsFalse()
    {
        // arrange
        string? title = null;

        // act
        var result = ProductValidationHelper.IsValidTitle(title);

        // assert
        Assert.False(result);
    }

    [Fact]
    public void IsValidCategoryName_Electronics_ReturnsTrue()
    {
        // arrange
        string categoryName = "Electronics";

        // act
        var result = ProductValidationHelper.IsValidCategoryName(categoryName);

        // assert
        Assert.True(result);
    }

    [Fact]
    public void IsValidCategoryName_Clothing_ReturnsTrue()
    {
        // arrange
        string categoryName = "Clothing";

        // act
        var result = ProductValidationHelper.IsValidCategoryName(categoryName);

        // assert
        Assert.True(result);
    }

    [Fact]
    public void IsValidCategoryName_Books_ReturnsTrue()
    {
        // arrange
        string categoryName = "Books";

        // act
        var result = ProductValidationHelper.IsValidCategoryName(categoryName);

        // assert
        Assert.True(result);
    }

    [Fact]
    public void IsValidCategoryName_EmptyString_ReturnsFalse()
    {
        // arrange
        string? categoryName = "";

        // act
        var result = ProductValidationHelper.IsValidCategoryName(categoryName);

        // assert
        Assert.False(result);
    }

    [Fact]
    public void IsValidCategoryName_Whitespace_ReturnsFalse()
    {
        // arrange
        string? categoryName = " ";

        // act
        var result = ProductValidationHelper.IsValidCategoryName(categoryName);

        // assert
        Assert.False(result);
    }

    [Fact]
    public void IsValidCategoryName_Null_ReturnsFalse()
    {
        // arrange
        string? categoryName = null;

        // act
        var result = ProductValidationHelper.IsValidCategoryName(categoryName);

        // assert
        Assert.False(result);
    }

    [Fact]
    public void IsValidManufacturerName_Apple_ReturnsTrue()
    {
        // arrange
        string manufacturerName = "Apple";

        // act
        var result = ProductValidationHelper.IsValidManufacturerName(manufacturerName);

        // assert
        Assert.True(result);
    }

    [Fact]
    public void IsValidManufacturerName_Samsung_ReturnsTrue()
    {
        // arrange
        string manufacturerName = "Samsung";

        // act
        var result = ProductValidationHelper.IsValidManufacturerName(manufacturerName);

        // assert
        Assert.True(result);
    }

    [Fact]
    public void IsValidManufacturerName_Microsoft_ReturnsTrue()
    {
        // arrange
        string manufacturerName = "Microsoft";

        // act
        var result = ProductValidationHelper.IsValidManufacturerName(manufacturerName);

        // assert
        Assert.True(result);
    }

    [Fact]
    public void IsValidManufacturerName_EmptyString_ReturnsFalse()
    {
        // arrange
        string? manufacturerName = "";

        // act
        var result = ProductValidationHelper.IsValidManufacturerName(manufacturerName);

        // assert
        Assert.False(result);
    }

    [Fact]
    public void IsValidManufacturerName_Whitespace_ReturnsFalse()
    {
        // arrange
        string? manufacturerName = " ";

        // act
        var result = ProductValidationHelper.IsValidManufacturerName(manufacturerName);

        // assert
        Assert.False(result);
    }

    [Fact]
    public void IsValidManufacturerName_Null_ReturnsFalse()
    {
        // arrange
        string? manufacturerName = null;

        // act
        var result = ProductValidationHelper.IsValidManufacturerName(manufacturerName);

        // assert
        Assert.False(result);
    }

    [Fact]
    public void IsValidId_Guid_ReturnsTrue()
    {
        // arrange
        string id = "12345678-1234-1234-1234-123456789012";

        // act
        var result = ProductValidationHelper.IsValidId(id);

        // assert
        Assert.True(result);
    }

    [Fact]
    public void IsValidId_Alphanumeric_ReturnsTrue()
    {
        // arrange
        string id = "abc123";

        // act
        var result = ProductValidationHelper.IsValidId(id);

        // assert
        Assert.True(result);
    }

    [Fact]
    public void IsValidId_ProductIdWithUnderscore_ReturnsTrue()
    {
        // arrange
        string id = "Product_ID_001";

        // act
        var result = ProductValidationHelper.IsValidId(id);

        // assert
        Assert.True(result);
    }

    [Fact]
    public void IsValidId_EmptyString_ReturnsFalse()
    {
        // arrange
        string? id = "";

        // act
        var result = ProductValidationHelper.IsValidId(id);

        // assert
        Assert.False(result);
    }

    [Fact]
    public void IsValidId_Whitespace_ReturnsFalse()
    {
        // arrange
        string? id = " ";

        // act
        var result = ProductValidationHelper.IsValidId(id);

        // assert
        Assert.False(result);
    }

    [Fact]
    public void IsValidId_Null_ReturnsFalse()
    {
        // arrange
        string? id = null;

        // act
        var result = ProductValidationHelper.IsValidId(id);

        // assert
        Assert.False(result);
    }

    [Fact]
    public void TryParsePrice_UsesInvariantCulture()
    {
        // arrange
        var originalCulture = CultureInfo.CurrentCulture;

        try
        {
            // Set a culture that uses comma as decimal separator
            CultureInfo.CurrentCulture = new CultureInfo("sv-SE");

            // act - should still parse dot as decimal separator due to InvariantCulture
            var result = ProductValidationHelper.TryParsePrice("10.99", out decimal price);

            // assert
            Assert.True(result);
            Assert.Equal(10.99m, price);
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }
}