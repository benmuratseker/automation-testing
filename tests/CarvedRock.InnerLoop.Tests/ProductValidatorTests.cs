using CarvedRock.Core;
using CarvedRock.Data;
using CarvedRock.Domain;
using NSubstitute;
using Xunit.Abstractions;

namespace CarvedRock.InnerLoop.Tests;

public class ProductValidatorTests(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public async Task NameValidationErrors()
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
}