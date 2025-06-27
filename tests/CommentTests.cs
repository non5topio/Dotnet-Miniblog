using Miniblog.Core.Models;
using Xunit;

namespace Miniblog.Core.Tests.Models;

public class CommentTests
{
    [Fact]
    public void GetGravatar_WithValidEmail_ReturnsGravatarUrl()
    {
        // Arrange
        var comment = new Comment 
        { 
            Email = "test@example.com" 
        };
        
        // Act
        var result = comment.GetGravatar();
        
        // Assert
        Assert.StartsWith("https://www.gravatar.com/avatar/", result);
        Assert.Contains("s=60&d=blank", result);
    }

    [Fact]
    public void GetGravatar_WithEmailWithSpaces_TrimsSpaces()
    {
        // Arrange
        var comment1 = new Comment { Email = "test@example.com" };
        var comment2 = new Comment { Email = " test@example.com " };
        
        // Act
        var result1 = comment1.GetGravatar();
        var result2 = comment2.GetGravatar();
        
        // Assert
        Assert.Equal(result1, result2);
    }

    [Fact]
    public void GetGravatar_WithUppercaseEmail_ReturnsLowercaseHash()
    {
        // Arrange
        var comment1 = new Comment { Email = "TEST@EXAMPLE.COM" };
        var comment2 = new Comment { Email = "test@example.com" };
        
        // Act
        var result1 = comment1.GetGravatar();
        var result2 = comment2.GetGravatar();
        
        // Assert
        Assert.Equal(result1, result2);
    }

    [Fact]
    public void RenderContent_ReturnsContentAsIs()
    {
        // Arrange
        var comment = new Comment 
        { 
            Content = "This is a test comment content." 
        };
        
        // Act
        var result = comment.RenderContent();
        
        // Assert
        Assert.Equal("This is a test comment content.", result);
    }

    [Fact]
    public void Comment_DefaultValues_AreSetCorrectly()
    {
        // Arrange & Act
        var comment = new Comment();
        
        // Assert
        Assert.False(comment.IsAdmin);
        Assert.NotNull(comment.ID);
        Assert.NotEmpty(comment.ID);
        Assert.True(comment.PubDate <= DateTime.UtcNow);
        Assert.True(comment.PubDate >= DateTime.UtcNow.AddSeconds(-5)); // Allow for slight time difference
    }

    [Fact]
    public void Comment_PropertiesCanBeSet()
    {
        // Arrange
        var comment = new Comment();
        var testDate = new DateTime(2023, 1, 1);
        
        // Act
        comment.Author = "Test Author";
        comment.Content = "Test Content";
        comment.Email = "test@test.com";
        comment.IsAdmin = true;
        comment.PubDate = testDate;
        comment.ID = "test-id";
        
        // Assert
        Assert.Equal("Test Author", comment.Author);
        Assert.Equal("Test Content", comment.Content);
        Assert.Equal("test@test.com", comment.Email);
        Assert.True(comment.IsAdmin);
        Assert.Equal(testDate, comment.PubDate);
        Assert.Equal("test-id", comment.ID);
    }
}
