using CarvedRock.Core;
using CarvedRock.Data;
using CarvedRock.Domain;
using NSubstitute;
using Xunit.Abstractions;

namespace CarvedRock.InnerLoop.Tests;

public class ProductValidatorTests(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public async Task NameValidationError_Spaces()
    {
        //Arrange
        var newProduct = new NewProductModel
        {
            Name = "",
            Description = "A new product",
            Category = "boots",
            Price = 100,
            ImgUrl = "https://www.example.com/image.jpg",
        };
        var repo = Substitute.For<ICarvedRockRepository>();
        repo.IsProductNameUniqueAsync(Arg.Any<string>()).Returns(true);

        var validator = new NewProductValidator(repo);

        //Act
        var result = await validator.ValidateAsync(newProduct);
        testOutputHelper.WriteLine(result.ToString());
        //Assert
        Assert.False(result.IsValid);
    }
    
    [Theory]
    [InlineData("", "Name is required.")]
    [InlineData(" ", "Name is required.")]
    [InlineData("duplicate", "A product with the same name already exists.")]
    public async Task NameValidationErrors(string nameToValidate, string errorMessage)
    {
        //Arrange
        var newProduct = new NewProductModel
        {
            Name = nameToValidate,
            Description = "A new product",
            Category = "boots",
            Price = 100,
            ImgUrl = "https://www.example.com/image.jpg",
        };
        var repo = Substitute.For<ICarvedRockRepository>();
        repo.IsProductNameUniqueAsync(Arg.Any<string>()).Returns(true);
        repo.IsProductNameUniqueAsync("duplicate").Returns(false);

        var validator = new NewProductValidator(repo);

        //Act
        var result = await validator.ValidateAsync(newProduct);
        testOutputHelper.WriteLine(result.ToString());
        //Assert
        Assert.False(result.IsValid);
        Assert.Equal(errorMessage, result.Errors[0].ErrorMessage);
    }
}