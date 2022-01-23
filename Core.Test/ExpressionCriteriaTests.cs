using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DynamicLambda.Core;
using Xunit;

namespace Core.Test;

public class ExpressionCriteriaTests
{
    private List<Person> _people = new List<Person>();


    public ExpressionCriteriaTests()
    {
        _people.Add(new Person()
        {
            Id = "149197af-579a-4ea9-931f-599dbdb2aade",
            FirstName = "John",
            LastName = "Doe",
            Age = 29,
            Address = new Address()
            {
                Id = "b3a57261-0eb6-4d5f-934c-f8b11a15f438",
                Number = "1234",
                Street = "Main Street",
                City = "Philadelphia",
                State = "PA",
                Zipcode = "19101",
            }
        });

        _people.Add(new Person()
        {
            Id = "53fe5e68-506d-447f-89e7-ad7386819b50",
            FirstName = "Bob",
            LastName = "Smith",
            Age = 65,
            Address = new Address()
            {
                Id = "cf7e83b6-5cec-41d2-a100-2ff079392f7a",
                Number = "123",
                Street = "Lancaster Avenue",
                City = "Paoli",
                State = "PA",
                Zipcode = "19301",
            }
        });
    }


    [Fact]
    public void TestMethod1()
    {
        var lambda = new ExpressionCriteria<Person>()
            .Add("Age", 60, ExpressionType.GreaterThan)
            .And()
            .Add("Address.City", "Paoli",
                ExpressionType.Equal)
            .Or()
            .Add("Address.Street", "Market Street",
                ExpressionType.Equal)
            .Or()
            .Add("Address.Id", "cf7e83b6-5cec-41d2-a100-2ff079392f7a", ExpressionType.Equal)
            .GetLambda();


        var expected = "person => ((((person.Age > 60) And (person.Address.City == \"Paoli\")) Or (person.Address.Street == \"Market Street\")) Or (person.Address.Id == \"cf7e83b6-5cec-41d2-a100-2ff079392f7a\"))";
        Assert.Equal(expected, lambda.ToString());
        var result = _people.Where(lambda.Compile()).ToList();
        Assert.Equal("Smith", result[0].LastName);
    }
}
public record Address
{
    public string Id { get; init; }
    public string Number { get; init; }
    public string Street { get; init; }
    public string City { get; init; }
    public string State { get; init; }
    public string Zipcode { get; init; }
}
public record Person
{

    public string Id { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public Address Address { get; init; }
    public int Age { get; init; }
}
