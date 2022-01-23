using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DynamicLambda.Core;
using Xunit;

namespace Core.Test;

    public class UnitTest1
    {
        private List<Person> _people = new List<Person>();


        public UnitTest1()
        {
            _people.Add(new Person() {
               FirstName = "John",
               LastName = "Doe",
               Age = 29,
            Address = new Address() {
                Number = "1234",
                Street = "Main Street",
                City = "Philadelphia",
                State = "PA",
                Zipcode = "19101",
               }
            });

            _people.Add(new Person() {
               FirstName = "Bob",
               LastName = "Smith",
               Age = 65,
            Address = new Address() {
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
                .GetLambda();

    
                var expected = "person => (((person.Age > 60) And (person.Address.City == \"Paoli\")) Or (person.Address.Street == \"Market Street\"))";
                Assert.Equal(expected,lambda.ToString());
                var result = _people.Where(lambda.Compile()).ToList();
                Assert.Equal("Smith", result[0].LastName);
        }

    }


    public record Address
    {
        public string Number { get; init; }
        public string Street { get; init; }
        public string City { get; init; }
        public string State { get; init; }
        public string Zipcode { get; init; }
    }
    public record Person
    {

        public string FirstName { get; init; }
        public string LastName { get; init; }
        public Address Address { get; init; }
        public int Age { get; init; }
    }
