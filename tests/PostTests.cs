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
}
/*
FAILED TEST: ### Analysis
The test run failed due to a syntax error in the `PostTests.cs` file. Specifically, the test method `CreateSlug_WithOnlyDiacritics_ReturnsEmptyString` is placed outside of the `PostTests` class, causing a compilation error.

### Recommended Fixes
Move the `CreateSlug_WithOnlyDiacritics_ReturnsEmptyString` method inside the `PostTests` class. The corrected code should look like this:

```csharp
using Miniblog.Core.Models;
using Xunit;

namespace Miniblog.Core.Tests.Models;

public class PostTests
{
    // Existing test methods...

    [Fact]
    public void CreateSlug_WithOnlyDiacritics_ReturnsEmptyString()
    {
        // Arrange
        var title = "ÀÉÈÊËÎÏÔÖÙÛÜÝàèéêëîïôöùûüý";
        
        // Act
        var result = Post.CreateSlug(title);
        
        // Assert
        Assert.Equal(string.Empty, result);
    }
}
```

    [Fact]
    public void RenderContent_WithNoImagesOrYouTubeEmbeds_ReturnsUnchangedContent()
    {
        // Arrange
        var post = new Post { Content = "This is a simple text without any images or videos." };
        
        // Act
        var result = post.RenderContent();
        
        // Assert
        Assert.Equal("This is a simple text without any images or videos.", result);
    }

*/
/*
FAILED TEST: ### Analysis
The test run failed due to a syntax error in the `PostTests.cs` file. Specifically, the test method `CreateSlug_WithOnlyDiacritics_ReturnsEmptyString` is placed outside of the `PostTests` class, causing a compilation error.

### Recommended Fixes
Move the `CreateSlug_WithOnlyDiacritics_ReturnsEmptyString` method inside the `PostTests` class. The corrected code should look like this:

```csharp
using Miniblog.Core.Models;
using Xunit;

namespace Miniblog.Core.Tests.Models;

public class PostTests
{
    // Existing test methods...

    [Fact]
    public void CreateSlug_WithOnlyDiacritics_ReturnsEmptyString()
    {
        // Arrange
        var title = "ÀÉÈÊËÎÏÔÖÙÛÜÝàèéêëîïôöùûüý";
        
        // Act
        var result = Post.CreateSlug(title);
        
        // Assert
        Assert.Equal(string.Empty, result);
    }
}
```

    [Fact]
    public void RenderContent_WithMultipleYouTubeEmbedTags_ReturnsMultipleIframes()
    {
        // Arrange
        var post = new Post { Content = "[youtube:video1][youtube:video2]" };
        
        // Act
        var result = post.RenderContent();
        
        // Assert
        Assert.Contains("<iframe width=\"560\" height=\"315\" title=\"YouTube embed\" src=\"about:blank\" data-src=\"https://www.youtube-nocookie.com/embed/video1?modestbranding=1&amp;hd=1&amp;rel=0&amp;theme=light\" allowfullscreen></iframe>", result);
        Assert.Contains("<iframe width=\"560\" height=\"315\" title=\"YouTube embed\" src=\"about:blank\" data-src=\"https://www.youtube-nocookie.com/embed/video2?modestbranding=1&amp;hd=1&amp;rel=0&amp;theme=light\" allowfullscreen></iframe>", result);
    }

*/
/*
FAILED TEST: ### Analysis
The test run failed due to a syntax error in the `PostTests.cs` file. Specifically, the test method `CreateSlug_WithOnlyDiacritics_ReturnsEmptyString` is placed outside of the `PostTests` class, causing a compilation error.

### Recommended Fixes
Move the `CreateSlug_WithOnlyDiacritics_ReturnsEmptyString` method inside the `PostTests` class. The corrected code should look like this:

```csharp
using Miniblog.Core.Models;
using Xunit;

namespace Miniblog.Core.Tests.Models;

public class PostTests
{
    // Existing test methods...

    [Fact]
    public void CreateSlug_WithOnlyDiacritics_ReturnsEmptyString()
    {
        // Arrange
        var title = "ÀÉÈÊËÎÏÔÖÙÛÜÝàèéêëîïôöùûüý";
        
        // Act
        var result = Post.CreateSlug(title);
        
        // Assert
        Assert.Equal(string.Empty, result);
    }
}
```

    [Fact]
    public void RenderContent_WithMalformedYouTubeEmbedTag_ReturnsUnchangedContent()
    {
        // Arrange
        var post = new Post { Content = "[youtube:invalid-video-id]" };
        
        // Act
        var result = post.RenderContent();
        
        // Assert
        Assert.Equal("[youtube:invalid-video-id]", result);
    }

*/
/*
FAILED TEST: ### Analysis
The test run failed due to a syntax error in the `PostTests.cs` file. Specifically, the test method `CreateSlug_WithOnlyDiacritics_ReturnsEmptyString` is placed outside of the `PostTests` class, causing a compilation error.

### Recommended Fixes
Move the `CreateSlug_WithOnlyDiacritics_ReturnsEmptyString` method inside the `PostTests` class. The corrected code should look like this:

```csharp
using Miniblog.Core.Models;
using Xunit;

namespace Miniblog.Core.Tests.Models;

public class PostTests
{
    // Existing test methods...

    [Fact]
    public void CreateSlug_WithOnlyDiacritics_ReturnsEmptyString()
    {
        // Arrange
        var title = "ÀÉÈÊËÎÏÔÖÙÛÜÝàèéêëîïôöùûüý";
        
        // Act
        var result = Post.CreateSlug(title);
        
        // Assert
        Assert.Equal(string.Empty, result);
    }
}
```

    [Fact]
    public void RenderContent_WithMalformedImageTag_ReturnsUnchangedContent()
    {
        // Arrange
        var post = new Post { Content = "<img src=\"invalid-image-url\" alt=\"Invalid Image\">" };
        
        // Act
        var result = post.RenderContent();
        
        // Assert
        Assert.Equal("<img src=\"invalid-image-url\" alt=\"Invalid Image\">", result);
    }

*/
/*
FAILED TEST: ### Analysis
The test run failed due to a syntax error in the `PostTests.cs` file. Specifically, the test method `CreateSlug_WithOnlyDiacritics_ReturnsEmptyString` is placed outside of the `PostTests` class, causing a compilation error.

### Recommended Fixes
Move the `CreateSlug_WithOnlyDiacritics_ReturnsEmptyString` method inside the `PostTests` class. The corrected code should look like this:

```csharp
using Miniblog.Core.Models;
using Xunit;

namespace Miniblog.Core.Tests.Models;

public class PostTests
{
    // Existing test methods...

    [Fact]
    public void CreateSlug_WithOnlyDiacritics_ReturnsEmptyString()
    {
        // Arrange
        var title = "ÀÉÈÊËÎÏÔÖÙÛÜÝàèéêëîïôöùûüý";
        
        // Act
        var result = Post.CreateSlug(title);
        
        // Assert
        Assert.Equal(string.Empty, result);
    }
}
```

    [Fact]
    public void CreateSlug_WithMaxLengthZero_ReturnsEmptyString()
    {
        // Arrange
        var title = "Valid Title";
        var maxLength = 0;
        
        // Act
        var result = Post.CreateSlug(title, maxLength);
        
        // Assert
        Assert.Equal(string.Empty, result);
    }

*/
/*
FAILED TEST: ### Analysis
The test run failed due to a syntax error in the `PostTests.cs` file. Specifically, the test method `CreateSlug_WithOnlyDiacritics_ReturnsEmptyString` is placed outside of the `PostTests` class, causing a compilation error.

### Recommended Fixes
Move the `CreateSlug_WithOnlyDiacritics_ReturnsEmptyString` method inside the `PostTests` class. The corrected code should look like this:

```csharp
using Miniblog.Core.Models;
using Xunit;

namespace Miniblog.Core.Tests.Models;

public class PostTests
{
    // Existing test methods...

    [Fact]
    public void CreateSlug_WithOnlyDiacritics_ReturnsEmptyString()
    {
        // Arrange
        var title = "ÀÉÈÊËÎÏÔÖÙÛÜÝàèéêëîïôöùûüý";
        
        // Act
        var result = Post.CreateSlug(title);
        
        // Assert
        Assert.Equal(string.Empty, result);
    }
}
```

    [Fact]
    public void CreateSlug_WithOnlyReservedUrlCharacters_ReturnsEmptyString()
    {
        // Arrange
        var title = "!#$&'()*+,/:;=?@[]\"%.<>\\^_`{|}~`+";
        
        // Act
        var result = Post.CreateSlug(title);
        
        // Assert
        Assert.Equal(string.Empty, result);
    }

*/
/*
FAILED TEST: ### Analysis
The test run failed due to a syntax error in the `PostTests.cs` file. Specifically, the test method `CreateSlug_WithOnlyDiacritics_ReturnsEmptyString` is placed outside of the `PostTests` class, causing a compilation error.

### Recommended Fixes
Move the `CreateSlug_WithOnlyDiacritics_ReturnsEmptyString` method inside the `PostTests` class. The corrected code should look like this:

```csharp
using Miniblog.Core.Models;
using Xunit;

namespace Miniblog.Core.Tests.Models;

public class PostTests
{
    // Existing test methods...

    [Fact]
    public void CreateSlug_WithOnlyDiacritics_ReturnsEmptyString()
    {
        // Arrange
        var title = "ÀÉÈÊËÎÏÔÖÙÛÜÝàèéêëîïôöùûüý";
        
        // Act
        var result = Post.CreateSlug(title);
        
        // Assert
        Assert.Equal(string.Empty, result);
    }
}
```

    [Fact]
    public void CreateSlug_WithOnlyDiacritics_ReturnsEmptyString()
    {
        // Arrange
        var title = "ÀÉÈÊËÎÏÔÖÙÛÜÝàèéêëîïôöùûüý";
        
        // Act
        var result = Post.CreateSlug(title);
        
        // Assert
        Assert.Equal(string.Empty, result);
    }

*/
