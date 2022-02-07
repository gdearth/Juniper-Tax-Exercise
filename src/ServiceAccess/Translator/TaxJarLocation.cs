using System.Web;
using BusinessEntity;

namespace ServiceAccess.Translator;

internal static class TaxJarLocation
{
    internal static string ToQueryString(this Location location)
    {
        var queryString = HttpUtility.ParseQueryString(string.Empty);
        if (!string.IsNullOrWhiteSpace(location.CountryCode))
            queryString.Add("country", location.CountryCode);
        if (!string.IsNullOrWhiteSpace(location.State))
            queryString.Add("state", location.State);
        if (!string.IsNullOrWhiteSpace(location.City))
            queryString.Add("city", location.City);
        if (!string.IsNullOrWhiteSpace(location.Street))
            queryString.Add("street", location.Street);
        return queryString.ToString();
    }
}