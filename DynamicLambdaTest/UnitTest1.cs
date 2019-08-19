﻿using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DynamicLambda;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LambdaTest
{
    [TestClass]
    public class UnitTest1
    {
        private List<Person> _people = new List<Person>();


        [TestInitialize]
        public void init()
        {
            var person = new Person();
            person.FirstName = "John";
            person.LastName = "Doe";
            person.Age = 29;
            person.Address.Number = "1234";
            person.Address.Street = "Main Street";
            person.Address.City = "Philadelphia";
            person.Address.State = "PA";
            person.Address.Zipcode = "19101";

            _people.Add(person);

            person = new Person();
            person.FirstName = "Bob";
            person.LastName = "Smith";
            person.Age = 65;
            person.Address.Number = "123";
            person.Address.Street = "Lancaster Avenue";
            person.Address.City = "Paoli";
            person.Address.State = "PA";
            person.Address.Zipcode = "19301";

            _people.Add(person);


        }


        [TestMethod]
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
                .GetLambda().Compile();
            var result = _people.Where(lambda).ToList();

            Assert.AreEqual("Smith",result[0].LastName);


        }

    }


    public class Address
    {
        public string Number { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zipcode { get; set; }
    }
    public class Person
    {
        public Person()
        {
            this.Address = new Address();
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Address Address { get; set; }
        public int Age { get; set; }
    }


}