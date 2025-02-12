using Bogus;
using CarvedRock.Core;
using CarvedRock.Data;
using CarvedRock.Domain;
using NSubstitute;
using Xunit.Abstractions;

namespace CarvedRock.InnerLoop.Tests;

public class ProductValidatorTests(ITestOutputHelper testOutputHelper)
{
    //generate test data from Bogus
    private readonly Faker _faker = new();
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
    [InlineData("__too_long__", "Name must not exceed 50 characters.")]
    public async Task NameValidationErrors(string nameToValidate, string errorMessage)
    {
        //Arrange
        var newProduct = new NewProductModel
        {
            Name = nameToValidate == "__too_long__" ? _faker.Lorem.Letter(51) : nameToValidate,
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