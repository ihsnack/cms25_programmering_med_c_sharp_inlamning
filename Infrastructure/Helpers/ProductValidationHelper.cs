using System.Globalization;

namespace Infrastructure.Helpers;

public static class ProductValidationHelper
{
    /// <summary>
    /// I've used Copilot to generate some of these methods
    /// </summary>
    /// 
    public static bool IsValidPrice(decimal price)
    {
        return price > 0 && price != decimal.MaxValue && price != decimal.MinValue;
    }

    public static bool TryParsePrice(string input, out decimal price)
    {
        price = 0;

        if (string.IsNullOrWhiteSpace(input))
        {
            return false;
        }

        var allowedStyles = NumberStyles.AllowDecimalPoint |
                           NumberStyles.AllowLeadingWhite |
                           NumberStyles.AllowTrailingWhite |
                           NumberStyles.AllowLeadingSign;

        if (decimal.TryParse(input, allowedStyles, CultureInfo.InvariantCulture, out decimal parsedPrice))
        {
            if (IsValidPrice(parsedPrice))
            {
                price = parsedPrice;
                return true;
            }
        }

        return false;
    }

    public static bool IsValidTitle(string? title)
    {
        return !string.IsNullOrWhiteSpace(title);
    }

    public static bool IsValidCategoryName(string? categoryName)
    {
        return !string.IsNullOrWhiteSpace(categoryName);
    }

    public static bool IsValidManufacturerName(string? manufacturerName)
    {
        return !string.IsNullOrWhiteSpace(manufacturerName);
    }

    public static bool IsValidId(string? id)
    {
        return !string.IsNullOrWhiteSpace(id);
    }
}