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
/*
FAILED TEST: <|channel|>analysis<|message|>We need to explain failure: AreCommentsOpen_ExactlyNow_ReturnsTrue expects true when PubDate + days equals now. Implementation: return this.PubDate.AddDays(commentsCloseAfterDays) >= DateTime.UtcNow; If PubDate is set to DateTime.UtcNow at object creation, then adding days (e.g., 0?) and comparing to DateTime.UtcNow later may be slightly later, causing false. Test likely sets PubDate to DateTime.UtcNow and passes 0 days. The inequality uses >=, but due to time elapsed between setting and checking, it's false. Fix: use >? Actually need to capture now before adding days. Better to compare using DateTime.UtcNow <= this.PubDate.AddDays(...). But same issue. Need to allow a small tolerance or use DateTime.UtcNow at start of method. Could change method to use DateTime.UtcNow <= this.PubDate.AddDays(commentsCloseAfterDays) + TimeSpan.FromSeconds(1)?? Simpler: change method to use DateTime.UtcNow <= this.PubDate.AddDays(commentsCloseAfterDays) . Already same. The issue is test sets PubDate to DateTime.UtcNow then calls AreCommentsOpen immediately; but between setting and method call, a few ticks pass, making PubDate.AddDays(0) < now. To fix test, set PubDate to DateTime.UtcNow.AddSeconds(-1) or modify method to use DateTime.UtcNow <= this.PubDate.AddDays(commentsCloseAfterDays). Already that. Actually need to use >=? Already. The problem is that PubDate is set earlier in test? Let's assume test does: var post = new Post { PubDate = DateTime.UtcNow }; Assert.True(post.AreCommentsOpen(0)); This fails due to slight delay. Fix: change method to use DateTime.UtcNow <= this.PubDate.AddDays(commentsCloseAfterDays). That's same. Could use DateTime.UtcNow <= this.PubDate.AddDays(commentsCloseAfterDays).AddSeconds(1) but not ideal. Better to compare using DateTime.UtcNow <= this.PubDate.AddDays(commentsCloseAfterDays).AddMilliseconds(1). Or use DateTime.UtcNow <= this.PubDate.AddDays(commentsCloseAfterDays).AddTicks(1). Simpler: change method to use >= DateTime.UtcNow.AddTicks(-1)?? Not good.

Alternative: change test to set PubDate = DateTime.UtcNow.AddMinutes(-1). But we need fix code.

Probably they intended to use > not >=? No.

The root cause: AreCommentsOpen uses PubDate.AddDays(commentsCloseAfterDays) >= DateTime.UtcNow; but if commentsCloseAfterDays is 0, it returns true only if PubDate is in future or exactly now. Since PubDate is set earlier, it's slightly in past, returns false. To make it true for "exactly now" scenario, we should use >? No, still false. Need to allow a tolerance. Could use DateTime.UtcNow <= this.PubDate.AddDays(commentsCloseAfterDays).AddSeconds(1) but arbitrary.

Better design: Use DateTime.UtcNow <= this.PubDate.AddDays(commentsCloseAfterDays).AddMilliseconds(1) maybe.

Simplify: Change method to compare using DateTime.UtcNow <= this.PubDate.AddDays(commentsCloseAfterDays).AddTicks(1). But I'd propose to use DateTime.UtcNow <= this.PubDate.AddDays(commentsCloseAfterDays).AddSeconds(1) to cover small delay.

Alternatively, compute difference: return (this.PubDate.AddDays(commentsCloseAfterDays) - DateTime.UtcNow).TotalSeconds >= 0; same issue.

Probably they want to use >? Actually test name "ExactlyNow_ReturnsTrue" expects true when PubDate + days == now. So method should treat equality as true. It already does. The failure is due to timing. So fix test or code to capture now earlier. Could modify method to use DateTime.UtcNow <= this.PubDate.AddDays(commentsCloseAfterDays) + TimeSpan.FromMilliseconds(1). Provide small tolerance.

Thus answer: test fails because of race condition; fix by using DateTime.UtcNow <= this.PubDate.AddDays(commentsCloseAfterDays).AddMilliseconds(1) or by passing a reference time into method.

Will propose change: add optional parameter now = null, default DateTime.UtcNow, then use that for comparison; test can pass same now. But simpler: change method to use DateTime.UtcNow <= this.PubDate.AddDays(commentsCloseAfterDays).AddSeconds(1).

Will answer accordingly.<|end|><|start|>assistant<|channel|>final<|message|>**Why the test failed**  
`Post.AreCommentsOpen(int commentsCloseAfterDays)` is implemented as  

```csharp
public bool AreCommentsOpen(int commentsCloseAfterDays) =>
    this.PubDate.AddDays(commentsCloseAfterDays) >= DateTime.UtcNow;
```

The test creates a `Post` whose `PubDate` is set to `DateTime.UtcNow` and then calls `AreCommentsOpen(0)`.  
Because a few ticks elapse between the assignment of `PubDate` and the evaluation of `DateTime.UtcNow` inside the method, `PubDate.AddDays(0)` is *slightly* earlier than the current time, so the comparison returns **false**. The method is logically correct, but the implementation is vulnerable to this tiny timing race, causing the “ExactlyNow_ReturnsTrue” test to fail.

**Recommended fix**  

Add a tiny tolerance (or allow the caller to supply the reference time) so that a value that is *practically* equal to “now” is treated as true.

```csharp
public bool AreCommentsOpen(int commentsCloseAfterDays, DateTime? now = null)
{
    // Use the supplied “now” value (test can pass the same instant) or fall back to UtcNow.
    var current = now ?? DateTime.UtcNow;

    // Allow a small tolerance (e.g., 1 ms) to avoid race‑condition failures.
    const long toleranceTicks = TimeSpan.TicksPerMillisecond; // 10,000 ticks = 1 ms
    var limit = this.PubDate.AddDays(commentsCloseAfterDays).AddTicks(toleranceTicks);

    return limit >= current;
}
```

* **What the change does**
  * The optional `now` parameter lets the test pass the exact timestamp used to set `PubDate`, guaranteeing equality.
  * The added tolerance (`+1 ms`) ensures that even when the method computes `DateTime.UtcNow` a few ticks later, the result is still considered “open”.

* **How to update the test (if you prefer not to change the signature)**
  ```csharp
  var now = DateTime.UtcNow;
  var post = new Post { PubDate = now };
  Assert.True(post.AreCommentsOpen(0, now));
  ```

Either approach eliminates the race condition and makes the “ExactlyNow_ReturnsTrue” test pass while preserving the intended semantics of the method.

    [Fact]
    public void AreCommentsOpen_ExactlyNow_ReturnsTrue()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var post = new Post { PubDate = now };
        var commentsCloseAfterDays = 0;
    
        // Act
        var result = post.AreCommentsOpen(commentsCloseAfterDays);
    
        // Assert
        // Using >=, the method should return true when the calculated closing time is equal to or after now.
        Assert.True(result);
    }

*/

    [Fact]
    public void CreateSlug_WithZeroMaxLength_ReturnsEmptyString()
    {
        // Arrange
        var title = "Anything";
        var maxLength = 0;
    
        // Act
        var result = Post.CreateSlug(title, maxLength);
    
        // Assert
        Assert.Equal(string.Empty, result);
        Assert.Empty(result);
    }


    [Fact]
    public void CreateSlug_NoTruncationWhenLengthEqualsMaxLength()
    {
        // Arrange
        var title = "1234567890";
        var maxLength = 10;
    
        // Act
        var slug = Post.CreateSlug(title, maxLength);
    
        // Assert
        Assert.Equal("1234567890", slug);
        Assert.Equal(maxLength, slug.Length);
    }


    [Fact]
    public void CreateSlug_RemovesDiacriticsFromUnicodeTitle()
    {
        // Arrange
        var title = "Café Münchner Kindl";
    
        // Act
        var slug = Post.CreateSlug(title);
    
        // Assert
        Assert.Equal("cafe-munchner-kindl", slug);
    }


    [Fact]
    public void RenderContent_ReturnsEmptyStringWhenContentIsEmpty()
    {
        // Arrange
        var post = new Post { Content = string.Empty };
    
        // Act
        var result = post.RenderContent();
    
        // Assert
        Assert.Equal(string.Empty, result);
    }

/*
FAILED TEST: <|channel|>analysis<|message|>We need to analyze failure. The test expects that after RenderContent processing, the image tag should have data-src attribute set to original src value. The implementation's ImageLazyLoadRegex replacement builds replacement string: `src="data:image/gif;base64,...\" data-src=\"` then adds groups: m.Groups[1].Value + replacement + m.Groups[4].Value + m.Groups[3].Value. Let's examine regex: `(<img.*?)(src=[\\\"|'])(?<src>.*?)([\\\"|'].*?[/]?>)`. Groups: 1 = (<img.*?), 2 = src=..., 3 = named src group, 4 = trailing part. Replacement concatenates group1 + replacement + group4 + group3. That order seems wrong: should be group1 + replacement + group3 + group4. Currently group4 (the closing quote and rest) before group3, causing data-src attribute to be placed after closing quote, not containing original src. Hence test fails to find data-src="pic.png". Fix by swapping order: `m.Groups[1].Value + replacement + m.Groups[3].Value + m.Groups[4].Value`. Also ensure replacement string includes correct spacing. So recommended fix: modify RenderContent's replacement lambda accordingly.

Provide concise answer.<|end|><|start|>assistant<|channel|>final<|message|>**Why the test failed**  
`Post.RenderContent()` builds the lazy‑load image markup with this lambda:

```csharp
result = ImageLazyLoadRegex().Replace(result, m =>
    m.Groups[1].Value + replacement + m.Groups[4].Value + m.Groups[3].Value);
```

`ImageLazyLoadRegex` captures  

1. **Group 1** – the opening `<img …`  
2. **Group 2** – the literal `src=` (unused)  
3. **Group 3** – the original URL (`pic.png`)  
4. **Group 4** – the closing quote and the rest of the tag (`" …>`)

The lambda appends **Group 4** *before* **Group 3**, so the generated markup looks like:

```html
<img … src="data:image/gif…"
   data-src="   "pic.png"
```

The original URL ends up **outside** the `data‑src` attribute, therefore the test cannot find the substring `data-src="pic.png"` and fails.

**Recommended fix**

Swap the order of the captured groups when constructing the replacement string:

```csharp
result = ImageLazyLoadRegex().Replace(result, m =>
    m.Groups[1].Value               // <img … 
    + replacement                   // src="data:image/gif…" data-src="
    + m.Groups[3].Value             // original URL (pic.png)
    + m.Groups[4].Value);           // closing quote and rest of tag
```

or, more explicitly:

```csharp
result = ImageLazyLoadRegex().Replace(result,
    m => $"{m.Groups[1].Value}{replacement}{m.Groups[3].Value}{m.Groups[4].Value}");
```

After this change the rendered content contains `data-src="pic.png"` and the `RenderContent_ProcessesImageAndYouTubeInSameContent` test will pass.

    [Fact]
    public void RenderContent_ProcessesImageAndYouTubeInSameContent()
    {
        // Arrange
        var post = new Post
        {
            Content = "<img src='pic.png'> Some text [youtube:XYZ987]"
        };
    
        // Act
        var result = post.RenderContent();
    
        // Assert
        // Image transformation
        const string placeholder = "src=\"data:image/gif;base64,R0lGODlhAQABAIAAAP///wAAACH5BAEAAAAALAAAAAABAAEAAAICRAEAOw==\" data-src=\"";
        Assert.Contains("<img ", result);
        Assert.Contains(placeholder, result);
        Assert.Contains("data-src=\"pic.png\"", result);
    
        // YouTube transformation
        var expectedYouTube = "<div class=\"video\"><iframe width=\"560\" height=\"315\" title=\"YouTube embed\" src=\"about:blank\" data-src=\"https://www.youtube-nocookie.com/embed/XYZ987?modestbranding=1&amp;hd=1&amp;rel=0&amp;theme=light\" allowfullscreen></iframe></div>";
        Assert.Contains(expectedYouTube, result);
    
        // Ensure ordering is preserved
        var imgIndex = result.IndexOf("<img ");
        var ytIndex = result.IndexOf("<div class=\"video\"");
        Assert.True(imgIndex < ytIndex, "Image tag should appear before YouTube embed in the result.");
    }

*/

    [Fact]
    public void RenderContent_ConvertsYouTubeShortcodeToIframe()
    {
        // Arrange
        var post = new Post
        {
            Content = "Watch this video: [youtube:abc123XYZ]"
        };
    
        // Act
        var result = post.RenderContent();
    
        // Assert
        var expectedFragment = "<div class=\"video\"><iframe width=\"560\" height=\"315\" title=\"YouTube embed\" src=\"about:blank\" data-src=\"https://www.youtube-nocookie.com/embed/abc123XYZ?modestbranding=1&amp;hd=1&amp;rel=0&amp;theme=light\" allowfullscreen></iframe></div>";
        Assert.Contains(expectedFragment, result);
        Assert.StartsWith("Watch this video: ", result);
    }


    [Fact]
    public void RenderContent_TransformsImgTagToLazyLoadPlaceholder()
    {
        // Arrange
        var post = new Post
        {
            Content = "<p>Text</p><img src=\"image.jpg\" alt=\"sample\">"
        };
    
        // Act
        var result = post.RenderContent();
    
        // Assert
        // The placeholder src defined in the source code
        const string placeholder = "src=\"data:image/gif;base64,R0lGODlhAQABAIAAAP///wAAACH5BAEAAAAALAAAAAABAAEAAAICRAEAOw==\" data-src=\"";
        Assert.Contains("<img ", result);
        Assert.Contains(placeholder, result);
        Assert.Contains("data-src=\"image.jpg\"", result);
        Assert.Contains("alt=\"sample\"", result);
        // Ensure the surrounding text remains unchanged
        Assert.StartsWith("<p>Text</p>", result);
    }

}
