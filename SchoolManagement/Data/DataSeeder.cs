namespace SchoolManagement.Data;

using Bogus;
using System.Linq;
using SchoolManagement.Models;

public static class DataSeeder
{
    public static void Seed(SchoolDbContext context)
    {
        // Prevent duplicate seeding
        if (context.Schools.Any()) return;

        var schoolFaker = new Faker<School>()
            .RuleFor(s => s.Name, f => f.Company.CompanyName())
            .RuleFor(s => s.Principal, f => f.Name.FullName())
            .RuleFor(s => s.Address, f => f.Address.FullAddress());

        var schools = schoolFaker.Generate(10);
        context.Schools.AddRange(schools);
        context.SaveChanges();

        var studentFaker = new Faker<Student>()
            .RuleFor(s => s.FullName, f => f.Name.FullName())
            .RuleFor(s => s.StudentCode, f => f.Random.AlphaNumeric(8))
            .RuleFor(s => s.Email, f => f.Internet.Email())
            .RuleFor(s => s.Phone, f => f.Phone.PhoneNumber("##########"))
            .RuleFor(s => s.SchoolId, f => f.PickRandom(schools).Id);

        context.Students.AddRange(studentFaker.Generate(20));
        context.SaveChanges();
    }
}
