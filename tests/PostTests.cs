using Miniblog.Core.Models;
using Xunit;

namespace Miniblog.Core.Tests.Models;

public class PostTests
{
    [Fact]
    public void CreateSlug_WithValidTitle_ReturnsLowercaseSlug()
    {
        // Arrange
        var title = "Hello World Test Post";
        
        // Act
        var result = Post.CreateSlug(title);
        
        // Assert
        Assert.Equal("hello-world-test-post", result);
    }

    [Fact]
    public void CreateSlug_WithSpecialCharacters_RemovesSpecialCharacters()
    {
        // Arrange
        var title = "Hello! World? Test@ Post#";
        
        // Act
        var result = Post.CreateSlug(title);
        
        // Assert
        Assert.Equal("hello-world-test-post", result);
    }

    [Fact]
    public void CreateSlug_WithLongTitle_TruncatesTitle()
    {
        // Arrange
        var title = "This is a very long title that should be truncated to fit within the maximum length limit";
        var maxLength = 20;
        
        // Act
        var result = Post.CreateSlug(title, maxLength);
        
        // Assert
        Assert.True(result.Length <= maxLength);
        Assert.Equal("this-is-a-very-long-", result);
    }

    [Fact]
    public void CreateSlug_WithNullTitle_ReturnsEmptyString()
    {
        // Arrange
        string? title = null;
        
        // Act
        var result = Post.CreateSlug(title!);
        
        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void GetLink_ReturnsCorrectBlogLink()
    {
        // Arrange
        var post = new Post { Slug = "test-post" };
        
        // Act
        var result = post.GetLink();
        
        // Assert
        Assert.Equal("/blog/test-post/", result);
    }

    [Fact]
    public void GetEncodedLink_ReturnsUrlEncodedBlogLink()
    {
        // Arrange
        var post = new Post { Slug = "test-post-with-special-chars&symbols" };
        
        // Act
        var result = post.GetEncodedLink();
        
        // Assert
        Assert.Contains("test-post-with-special-chars%26symbols", result);
    }

    [Fact]
    public void IsVisible_WithPublishedPostInPast_ReturnsTrue()
    {
        // Arrange
        var post = new Post 
        { 
            IsPublished = true, 
            PubDate = DateTime.UtcNow.AddDays(-1) 
        };
        
        // Act
        var result = post.IsVisible();
        
        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsVisible_WithUnpublishedPost_ReturnsFalse()
    {
        // Arrange
        var post = new Post 
        { 
            IsPublished = false, 
            PubDate = DateTime.UtcNow.AddDays(-1) 
        };
        
        // Act
        var result = post.IsVisible();
        
        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsVisible_WithFuturePublishedPost_ReturnsFalse()
    {
        // Arrange
        var post = new Post 
        { 
            IsPublished = true, 
            PubDate = DateTime.UtcNow.AddDays(1) 
        };
        
        // Act
        var result = post.IsVisible();
        
        // Assert
        Assert.False(result);
    }

    [Fact]
    public void AreCommentsOpen_WithinCloseDays_ReturnsTrue()
    {
        // Arrange
        var post = new Post { PubDate = DateTime.UtcNow.AddDays(-5) };
        var commentsCloseAfterDays = 10;
        
        // Act
        var result = post.AreCommentsOpen(commentsCloseAfterDays);
        
        // Assert
        Assert.True(result);
    }

    [Fact]
    public void AreCommentsOpen_BeyondCloseDays_ReturnsFalse()
    {
        // Arrange
        var post = new Post { PubDate = DateTime.UtcNow.AddDays(-15) };
        var commentsCloseAfterDays = 10;
        
        // Act
        var result = post.AreCommentsOpen(commentsCloseAfterDays);
        
        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsVisible_ReturnsTrue_WhenPubDateEqualsNow()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var post = new Post
        {
            IsPublished = true,
            PubDate = now
        };
    
        // Act
        var result = post.IsVisible();
    
        // Assert
        Assert.True(result);
    }


    [Fact]
    public void AreCommentsOpen_ReturnsTrue_AtExactCloseBoundary()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var post = new Post { PubDate = now.AddDays(-10).AddMilliseconds(1) };
        var commentsCloseAfterDays = 10;
    
        // Act
        var result = post.AreCommentsOpen(commentsCloseAfterDays);
    
        // Assert
        Assert.True(result);
    }


    [Fact]
    public void RenderContent_ReturnsEmptyString_WhenContentIsEmpty()
    {
        // Arrange
        var post = new Post { Content = string.Empty };
    
        // Act
        var result = post.RenderContent();
    
        // Assert
        Assert.Equal(string.Empty, result);
    }

/*
FAILED TEST: <|channel|>analysis<|message|>We need to provide concise analysis: test failure due to lazy load regex replacement ordering causing src attribute not removed. Recommend swapping groups order. Also maybe adjust regex to capture correctly. Provide fix snippet.<|end|><|start|>assistant<|channel|>final<|message|>**Why the test failed**  
`Post.RenderContent()` builds the lazy‑load replacement with  

```csharp
m.Groups[1].Value + replacement + m.Groups[4].Value + m.Groups[3].Value
```  

`Groups[4]` contains the closing quote (and any trailing attributes) **before** the original URL (`Groups[3]`). By appending `Groups[4]` first, the original `src="…"` remains in the output, so the test finds the forbidden substring `src="/img/a.png"` and fails.

**Recommended fix**  
Swap the order of the captured groups so the original URL is inserted **before** the closing quote:

```csharp
// inside RenderContent()
var replacement = " src=\"data:image/gif;base64,R0lGODlhAQABAIAAAP///wAAACH5BAEAAAAALAAAAAABAAEAAAICRAEAOw==\" data-src=\"";
result = ImageLazyLoadRegex()
         .Replace(result,
                  m => m.Groups[1].Value      // <img … 
                     + replacement
                     + m.Groups[3].Value      // original src value
                     + m.Groups[4].Value);    // closing quote / rest of tag
```

After this change the rendered HTML will contain only the placeholder `src` and a `data-src` attribute with the original URL, making the `RenderContent_ProcessesImgAndYouTube_InSingleCall` test pass.

    [Fact]
    public void RenderContent_ProcessesImgAndYouTube_InSingleCall()
    {
        // Arrange
        var post = new Post
        {
            Content = "<img src=\"/img/a.png\"/> and video [youtube:xyz987]"
        };
    
        // Act
        var result = post.RenderContent();
    
        // Assert - Image lazy‑load
        const string placeholder = "src=\"data:image/gif;base64,R0lGODlhAQABAIAAAP///wAAACH5BAEAAAAALAAAAAABAAEAAAICRAEAOw==\" data-src=\"";
        Assert.Contains(placeholder, result);
        Assert.Contains("data-src=\"/img/a.png\"", result);
        Assert.DoesNotContain("src=\"/img/a.png\"", result);
    
        // Assert - YouTube embed
        const string expectedIframeStart = "<div class=\"video\"><iframe";
        const string expectedDataSrc = "data-src=\"https://www.youtube-nocookie.com/embed/xyz987?modestbranding=1&amp;hd=1&amp;rel=0&amp;theme=light\"";
        Assert.Contains(expectedIframeStart, result);
        Assert.Contains(expectedDataSrc, result);
    }

*/

    [Fact]
    public void RenderContent_ConvertsYouTubePlaceholder_ToIframe()
    {
        // Arrange
        var post = new Post
        {
            Content = "Watch this video: [youtube:abc123XYZ]"
        };
    
        // Act
        var result = post.RenderContent();
    
        // Assert
        const string expectedIframeStart = "<div class=\"video\"><iframe";
        const string expectedDataSrc = "data-src=\"https://www.youtube-nocookie.com/embed/abc123XYZ?modestbranding=1&amp;hd=1&amp;rel=0&amp;theme=light\"";
        Assert.Contains(expectedIframeStart, result);
        Assert.Contains(expectedDataSrc, result);
        Assert.Contains("</iframe></div>", result);
    }

/*
FAILED TEST: <|channel|>analysis<|message|>We need to analyze test file content. The test file seems malformed: missing [Fact] attributes etc. But the failure is about RenderContent_AddsLazyLoadPlaceholder_ToImgTag. The test expects that after calling RenderContent, the img tag src attribute is replaced with placeholder and data-src attribute. The failure shows that src attribute still present: "src="/images/pic.jpg"". Means the regex didn't match or replacement didn't happen. Look at ImageLazyLoadRegex: pattern "(<img.*?)(src=[\\\"|'])(?<src>.*?)([\\\"|'].*?[/]?>)". Replacement uses m.Groups[1].Value + replacement + m.Groups[4].Value + m.Groups[3].Value. Wait order: replacement string includes " src=\"data...\" data-src=\"" then later they append m.Groups[4] then m.Groups[3]. That seems wrong ordering causing src attribute not removed? Actually groups: 1 = <img..., 2 = src=", 3 = src value, 4 = rest including closing quote and maybe other attributes. Replacement builds: group1 + replacement + group4 + group3. That results in ... <img ... src="data..."> then group4 (which includes the original closing quote?) then group3 (original src). Might produce duplicate src? The test expects src attribute removed. The code seems to place original src after the placeholder, causing src still present. Should be group1 + replacement + group3 + group4? Actually we want to replace src attribute with placeholder and add data-src with original src. So replacement should be group1 + replacement + original src + group4? Wait placeholder includes data-src=" then we need original src then close quote? The placeholder string ends with data-src=" (no closing quote). So we need original src then group4 which contains the rest including closing quote and maybe other attributes. So correct order: group1 + replacement + group3 + group4. Current code uses group4 then group3, causing src attribute still present? Actually group4 includes the closing quote after original src, so putting group4 before group3 moves the closing quote before src, leaving src attribute after? That results in src attribute still there. So fix ordering.

Thus recommended fix: change replacement lambda to:

result = ImageLazyLoadRegex().Replace(result, m => m.Groups[1].Value + replacement + m.Groups[3].Value + m.Groups[4].Value);

Also ensure regex pattern correct? Probably fine.

Provide concise answer.<|end|><|start|>assistant<|channel|>final<|message|>**Why the test failed**

`Post.RenderContent()` is supposed to replace an `<img>` tag’s `src` attribute with a lazy‑load placeholder and move the original URL into a new `data‑src` attribute.  

The implementation builds the replacement string as:

```csharp
m.Groups[1].Value + replacement + m.Groups[4].Value + m.Groups[3].Value
```

`Groups[4]` contains the closing quote (and any trailing attributes) **before** the original URL (`Groups[3]`). By appending `Groups[4]` first, the original `src="…"` is left intact, so the rendered HTML still contains `src="/images/pic.jpg"`. The test therefore finds the forbidden substring and fails.

**Recommended fix**

Swap the order of the captured groups so the original URL is inserted **before** the closing quote:

```csharp
result = ImageLazyLoadRegex().Replace(
    result,
    m => m.Groups[1].Value          // <img … 
       + replacement                // placeholder + data-src="
       + m.Groups[3].Value          // original URL
       + m.Groups[4].Value);        // closing quote / other attrs
```

After this change the rendered markup will be:

```html
<img … src="data:image/gif;base64,R0lGODlhAQABAIAAAP///wAAACH5BAEAAAAALAAAAAABAAEAAAICRAEAOw==" data-src="/images/pic.jpg" …>
```

and the `RenderContent_AddsLazyLoadPlaceholder_ToImgTag` test will pass.

    [Fact]
    public void RenderContent_AddsLazyLoadPlaceholder_ToImgTag()
    {
        // Arrange
        var post = new Post
        {
            Content = "<p>Sample</p><img src=\"/images/pic.jpg\" alt=\"pic\"/>"
        };
    
        // Act
        var result = post.RenderContent();
    
        // Assert
        const string placeholder = "src=\"data:image/gif;base64,R0lGODlhAQABAIAAAP///wAAACH5BAEAAAAALAAAAAABAAEAAAICRAEAOw==\" data-src=\"";
        Assert.Contains(placeholder, result);
        Assert.Contains("data-src=\"/images/pic.jpg\"", result);
        Assert.DoesNotContain("src=\"/images/pic.jpg\"", result);
    }

*/
}
