using Bogus;
using Logistics.Domain.Entities;
using Logistics.Domain.Enums;

namespace Logistics.Tests.Helpers;

public static class FakeDataGenerator
{
    private static readonly Faker _faker = new Faker("pt_BR");

    public static Company GenerateCompany(bool isActive = true)
    {
        var name = _faker.Company.CompanyName();
        var document = _faker.Random.Replace("##############"); // 14 dígitos
        var company = new Company(name, document);
        
        if (!isActive)
            company.Deactivate();
        
        return company;
    }

    public static User GenerateUser(Guid? companyId = null, UserRole role = UserRole.CompanyUser)
    {
        var name = _faker.Person.FullName;
        var email = _faker.Internet.Email(name);
        var password = BCrypt.Net.BCrypt.HashPassword("Test@123");
        
        return new User(name, email, password, role, companyId);
    }

    public static User GenerateAdminUser()
    {
        return GenerateUser(null, UserRole.Admin);
    }

    public static Vehicle GenerateVehicle(Guid companyId)
    {
        var licensePlate = _faker.Random.Replace("???####"); // ABC1234
        var model = _faker.Vehicle.Model();
        var year = _faker.Random.Int(2015, DateTime.UtcNow.Year);
        
        return new Vehicle(companyId, licensePlate, model, year);
    }

    public static Driver GenerateDriver(Guid companyId)
    {
        var name = _faker.Person.FullName;
        var licenseNumber = _faker.Random.Replace("###########"); // 11 dígitos
        var phone = _faker.Phone.PhoneNumber("(##) #####-####");
        
        return new Driver(companyId, name, licenseNumber, phone);
    }

    public static string GenerateCNPJ()
    {
        return _faker.Random.Replace("##############");
    }

    public static string GenerateEmail()
    {
        return _faker.Internet.Email();
    }

    public static string GeneratePhone()
    {
        return _faker.Phone.PhoneNumber("(##) #####-####");
    }
}
