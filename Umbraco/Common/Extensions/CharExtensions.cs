namespace Umbraco.Common.Extensions;

public static class CharExtensions
{
    /// <summary>
    /// Remaps international characters to ascii compatible ones
    /// based of: https://meta.stackexchange.com/questions/7435/non-us-ascii-characters-dropped-from-full-profile-url/7696#7696
    /// </summary>
    /// <param name="c">Charcter to remap</param>
    /// <returns>Remapped character</returns>
    public static string CharToAscii(this char c)
    {
        var s = c.ToString().ToLowerInvariant();
        if ("àåáâäãåą".Contains(s))
        {
            return "a";
        }

        if ("èéêëę".Contains(s))
        {
            return "e";
        }

        if ("ìíîïı".Contains(s))
        {
            return "i";
        }

        if ("òóôõöøőð".Contains(s))
        {
            return "o";
        }

        if ("ùúûüŭů".Contains(s))
        {
            return "u";
        }

        if ("çćčĉ".Contains(s))
        {
            return "c";
        }

        if ("żźž".Contains(s))
        {
            return "z";
        }

        if ("śşšŝ".Contains(s))
        {
            return "s";
        }

        if ("ñń".Contains(s))
        {
            return "n";
        }

        if ("ýÿ".Contains(s))
        {
            return "y";
        }

        if ("ğĝ".Contains(s))
        {
            return "g";
        }

        return c switch
        {
            'ř' => "r",
            'ł' => "l",
            'đ' => "d",
            'ß' => "ss",
            'þ' => "th",
            'ĥ' => "h",
            'ĵ' => "j",
            _ => ""
        };
    }
}