using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace App
{
    public class CustomerService
    {
        private CompanyRepository _companyRepository;
        private ICustomerCreditServiceFactory _customerCreditServiceFactory;

        public CustomerService(CompanyRepository companyRepository, ICustomerCreditServiceFactory customerCreditServiceFactory)
        {
            _companyRepository = companyRepository;
            _customerCreditServiceFactory = customerCreditServiceFactory;
        }

        public bool AddCustomer(string firname, string surname, string email, DateTime dateOfBirth, int companyId)
        {
            if (this.Validate(firname, surname, email, dateOfBirth) == false)
            {
                return false;
            }

            var company = _companyRepository.GetById(companyId);

            var customer = new Customer
                               {
                                   Company = company,
                                   DateOfBirth = dateOfBirth,
                                   EmailAddress = email,
                                   Firstname = firname,
                                   Surname = surname
                               };

            if (company.Name == "VeryImportantClient")
            {
                // Skip credit check
                customer.HasCreditLimit = false;
            }
            else if (company.Name == "ImportantClient")
            {
                // Do credit check and double credit limit
                customer.HasCreditLimit = true;
                //Change this to use Factory? something in constructor so can be unit tested with moq
                using (var customerCreditService = new CustomerCreditServiceClient())
                {
                    var creditLimit = customerCreditService.GetCreditLimit(customer.Firstname, customer.Surname, customer.DateOfBirth);
                    creditLimit = creditLimit*2;
                    customer.CreditLimit = creditLimit;
                }
            }
            else
            {
                // Do credit check
                customer.HasCreditLimit = true;
                using (var customerCreditService = new CustomerCreditServiceClient())
                {
                    var creditLimit = customerCreditService.GetCreditLimit(customer.Firstname, customer.Surname, customer.DateOfBirth);
                    customer.CreditLimit = creditLimit;
                }
            }

            if (customer.HasCreditLimit && customer.CreditLimit < 500)
            {
                return false;
            }

            //Change to use wrapper
            CustomerDataAccess.AddCustomer(customer);

            return true;
        }

        public bool Validate(string firname, string surname, string email, DateTime dateOfBirth)
        {
            var isValid = true;
            //Validation
            if (string.IsNullOrEmpty(firname) || string.IsNullOrEmpty(surname))
            {
                return false;
            }

            //Validation
            if (!email.Contains("@") && !email.Contains("."))
            {
                return false;
            }

            //Validation
            var now = DateTime.Now;
            int age = now.Year - dateOfBirth.Year;
            if (now.Month < dateOfBirth.Month || (now.Month == dateOfBirth.Month && now.Day < dateOfBirth.Day)) age--;

            var minAge = 21;
            if (age < minAge)
            {
                return false;
            }

            return isValid;
        }
    }
}
