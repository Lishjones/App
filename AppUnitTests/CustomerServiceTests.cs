using App;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Data.Entity;

namespace AppUnitTests
{
    [TestClass]
    public class CustomerServiceTests
    {
        private Mock<CompanyRepository> _mockRepository;
        private Mock<ICustomerCreditServiceFactory> _mockCreditServiceFactory;

        [TestInitialize]
        public void Setup()
        {
            var mockRepository = new Mock<CompanyRepository>();
            mockRepository.Setup(x => x.GetById(It.IsAny<int>())).Returns(new Company
            {
                Id = 1,
                Name = "Important Client",
                Classification = Classification.Bronze,
            });

            this._mockRepository = mockRepository;
        }

        [TestMethod]
        public void WhenAddCustomerIsCalledGivenNamesAreInvalidThenFalseIsReturned()
        {
           var customerService = new CustomerService(_mockRepository.Object, _mockCreditServiceFactory.Object);

            var actual = customerService.AddCustomer(null, null, "email", new DateTime(), 1);

            actual.Should().BeFalse();
        }

        [TestMethod]
        public void WhenAddCustomerIsCalledGivenEmailIsInvalidThenFalseIsReturned()
        {
            var customerService = new CustomerService(_mockRepository.Object, _mockCreditServiceFactory.Object);

            var actual = customerService.AddCustomer("first", "last", "email", new DateTime(), 1);

            actual.Should().BeFalse();
        }

        [TestMethod]
        public void WhenAddCustomerIsCalledGivenCustomerIsUnder21ThenFalseIsReturned()
        {
            var customerService = new CustomerService(_mockRepository.Object, _mockCreditServiceFactory.Object);

            var actual = customerService.AddCustomer("first", "last", "email@email.com", new DateTime(2020,01,01), 1);

            actual.Should().BeFalse();
        }

        [TestMethod]
        public void WhenAddCustomerIsCalledGivenValidationPassesAndCustomerHasCreditLimitUnderFiveHundredThenFalseIsReturned()
        {
            var mockRepository = new Mock<CompanyRepository>();
            mockRepository.Setup(x => x.GetById(It.IsAny<int>())).Returns(new Company
            {
                Id = 1,
                Name = "Important Client",
                Classification = Classification.Bronze,
            });

            var mockCreditServiceClient = new Mock<ICustomerCreditService>();
            mockCreditServiceClient.Setup(x => x.GetCreditLimit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>())).Returns(300);

            //how to get mock working? need Factory?
            var mockCreditServiceFactory = new Mock<ICustomerCreditServiceFactory>();
            mockCreditServiceFactory.Setup(x => x.GetCustomerCreditService()).Returns(mockCreditServiceClient.Object);

            var customerService = new CustomerService(mockRepository.Object, mockCreditServiceFactory.Object);

            var actual = customerService.AddCustomer("first", "last", "email@email.com", new DateTime(1970, 01, 01), 1);

            actual.Should().BeFalse();
        }

        //[TestMethod]
        //public void WhenAddCustomerIsCalledGivenValidationPassesAndCustomerIsHasCreditLimitOverFiveHundredThenTrueIsReturned()
        //{
        //    var customerService = new CustomerService(_mockRepository.Object, _mockCreditService.Object);

        //    var actual = customerService.AddCustomer("first", "last", "email@email.com", new DateTime(1970, 01, 01), 1);

        //    actual.Should().BeTrue();
        //}

        //[TestMethod]
        //public void WhenAddCustomerIsCalledGivenValidationPassesThenCustomerIsAdded() //may not need this test as this is a database call?
        //{
        //    var customerService = new CustomerService(_mockRepository.Object, _mockCreditService.Object);

        //    var actual = customerService.AddCustomer("first", "last", "email@email.com", new DateTime(1970, 01, 01), 1);

        //    actual.Should().BeTrue();
        //}
    }
}
