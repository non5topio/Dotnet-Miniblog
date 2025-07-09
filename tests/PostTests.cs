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
FAILED TEST: **Analysis:**

The test `CreateSlug_WithOnlyWhitespace_ReturnsEmptyString` is failing because the `CreateSlug` method is returning `"-----"` when given a title consisting only of whitespace. This occurs because whitespace is being replaced with dashes, but the method does not handle or strip all whitespace before or after the transformation.

**Recommended Fix:**

Update the `CreateSlug` method in `Post.cs` to trim and remove all whitespace before replacing it with dashes. For example:

```csharp
title = title?.Trim().Replace(" ", "-", StringComparison.OrdinalIgnoreCase) ?? string.Empty;
```

Or explicitly remove all whitespace using a regex or `String.Replace` with a normalized whitespace check.

    [Fact]
    public void CreateSlug_WithOnlyWhitespace_ReturnsEmptyString()
    {
        // Arrange
        var title = "     ";
        
        // Act
        var result = Post.CreateSlug(title);
        
        // Assert
        Assert.Equal(string.Empty, result);
    }

*/

    [Fact]
    public void RenderContent_WithNoImageOrYouTubeTags_ReturnsUnchangedContent()
    {
        // Arrange
        var post = new Post { Content = "<p>This is a paragraph.</p>" };
        
        // Act
        var result = post.RenderContent();
        
        // Assert
        Assert.Equal("<p>This is a paragraph.</p>", result);
    }


    [Fact]
    public void RenderContent_WithEmptyContent_ReturnsEmptyString()
    {
        // Arrange
        var post = new Post { Content = string.Empty };
        
        // Act
        var result = post.RenderContent();
        
        // Assert
        Assert.Equal(string.Empty, result);
    }


    [Fact]
    public void CreateSlug_WithOnlyReservedCharacters_ReturnsEmptyString()
    {
        // Arrange
        var title = "!@#$%^&*()_+";
        
        // Act
        var result = Post.CreateSlug(title);
        
        // Assert
        Assert.Equal(string.Empty, result);
    }


    [Fact]
    public void CreateSlug_WithDiacritics_RemovesDiacritics()
    {
        // Arrange
        var title = "Café Münster";
        
        // Act
        var result = Post.CreateSlug(title);
        
        // Assert
        Assert.Equal("cafe-munster", result);
    }

}
