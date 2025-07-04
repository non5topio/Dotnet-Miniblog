namespace Miniblog.Core.Models;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

public partial class Post
{
    public IList<string> Categories { get; } = [];

    public IList<Comment> Comments { get; } = [];

    [Required]
    public string Content { get; set; } = string.Empty;

    [Required]
    public string Excerpt { get; set; } = string.Empty;

    [Required]
    public string ID { get; set; } = DateTime.UtcNow.Ticks.ToString(CultureInfo.InvariantCulture);

    public bool IsPublished { get; set; } = true;
    public DateTime LastModified { get; set; } = DateTime.UtcNow;
    public DateTime PubDate { get; set; } = DateTime.UtcNow;

    [DisplayFormat(ConvertEmptyStringToNull = false)]
    public string Slug { get; set; } = string.Empty;

    public IList<string> Tags { get; } = [];

    [Required]
    public string Title { get; set; } = string.Empty;

    [SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase", Justification = "The slug should be lower case.")]
    public static string CreateSlug(string title, int maxLength = 50)
    {
        title = title?.ToLowerInvariant().Replace(
            Constants.Space, Constants.Dash, StringComparison.OrdinalIgnoreCase) ?? string.Empty;
        title = RemoveDiacritics(title);
        title = RemoveReservedUrlCharacters(title);

        // Truncate the title if it exceeds the maximum length
        if (title.Length > maxLength)
        {
            title = title[..maxLength];
        }

        return title.ToLowerInvariant();
    }

    public bool AreCommentsOpen(int commentsCloseAfterDays) =>
        this.PubDate.AddDays(commentsCloseAfterDays) >= DateTime.UtcNow;

    public string GetEncodedLink() => $"/blog/{System.Net.WebUtility.UrlEncode(this.Slug)}/";

    public string GetLink() => $"/blog/{this.Slug}/";

    public bool IsVisible() => this.PubDate <= DateTime.UtcNow && this.IsPublished;

    public string RenderContent()
    {
        var result = this.Content;

        // Set up lazy loading of images/iframes
        if (!string.IsNullOrEmpty(result))
        {
            // Set up lazy loading of images/iframes
            var replacement = " src=\"data:image/gif;base64,R0lGODlhAQABAIAAAP///wAAACH5BAEAAAAALAAAAAABAAEAAAICRAEAOw==\" data-src=\"";
            result = ImageLazyLoadRegex().Replace(result, m => m.Groups[1].Value + replacement + m.Groups[4].Value + m.Groups[3].Value);

            // Youtube content embedded using this syntax: [youtube:xyzAbc123]
            var video = "<div class=\"video\"><iframe width=\"560\" height=\"315\" title=\"YouTube embed\" src=\"about:blank\" data-src=\"https://www.youtube-nocookie.com/embed/{0}?modestbranding=1&amp;hd=1&amp;rel=0&amp;theme=light\" allowfullscreen></iframe></div>";
            result = YouTubeEmbedRegex().Replace(result, m => string.Format(CultureInfo.InvariantCulture, video, m.Groups[1].Value));
        }

        return result;
    }

    [GeneratedRegex("(<img.*?)(src=[\\\"|'])(?<src>.*?)([\\\"|'].*?[/]?>)")]
    private static partial Regex ImageLazyLoadRegex();

    private static string RemoveDiacritics(string text)
    {
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                _ = stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }

    private static string RemoveReservedUrlCharacters(string text)
    {
        var reservedCharacters = new List<string> { "!", "#", "$", "&", "'", "(", ")", "*", ",", "/", ":", ";", "=", "?", "@", "[", "]", "\"", "%", ".", "<", ">", "\\", "^", "_", "'", "{", "}", "|", "~", "`", "+" };

        foreach (var chr in reservedCharacters)
        {
            text = text.Replace(chr, string.Empty, StringComparison.OrdinalIgnoreCase);
        }

        return text;
    }

    [GeneratedRegex(@"\[youtube:(.*?)\]")]
    private static partial Regex YouTubeEmbedRegex();
}
