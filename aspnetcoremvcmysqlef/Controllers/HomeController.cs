using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using MySQL.Data.EntityFrameworkCore.Extensions;

namespace aspnetcoremvcmysqlef.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        public IActionResult AddEmployee()
        {
            // Create an employee instance and save the entity to the database
            var entry = new Employee() { Name = "John", LastName = "Winston" };

            using (var context = EmployeesContextFactory.Create())
            {
                context.Add(entry);
                context.SaveChanges();
            }

            ViewData["Message"] = $"Employee was saved in the database with id: {entry.Id}";

            return View("Employee");
        }

        public IActionResult Employee()
        {
            ViewData["Message"] = "List or add employees";
            
            Employees model = new Employees();

            // get selection from the DB here
            using (var ctx = EmployeesContextFactory.Create())
            {
                model.List = ctx.Employees.Where(x => x.Id > 0).ToList();
            }
            
            return View(model);
        }

        [HttpPost]
        public IActionResult Employee(Employee model)
        {
            // Create an employee instance and save the entity to the database
            using (var ctx = EmployeesContextFactory.Create())
            {
                ctx.SaveChanges();
            }

            TempData["success"] = ($"Employee was saved in the database with id: {model.Id}");

            return View();
        }
    }
}

namespace aspnetcoremvcmysqlef {

    /// <summary>
    /// The entity framework context with a Employees DbSet
    /// </summary>
    public class EmployeesContext : DbContext
    {
        public EmployeesContext(DbContextOptions<EmployeesContext> options)
        : base(options)
        { }

        public DbSet<Employee> Employees { get; set; }
    }

    /// <summary>
    /// Factory class for EmployeesContext
    /// </summary>
    public static class EmployeesContextFactory
    {
        public static EmployeesContext Create()
        {
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            var configuration = builder.Build();
            
            return Create(configuration.GetConnectionString("SampleConnection"));
        }

        public static EmployeesContext Create(string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder<EmployeesContext>();
            optionsBuilder.UseMySQL(connectionString);

            //Ensure database creation
            var context = new EmployeesContext(optionsBuilder.Options);
            context.Database.EnsureCreated();

            return context;
        }
    }

    /// <summary>
    /// A basic class for an Employee
    /// </summary>
    public class Employee
    {
        public Employee()
        {
        }

        public int Id { get; set; }

        [MaxLength(30)]
        public string Name { get; set; }

        [MaxLength(50)]
        public string LastName { get; set; }
    }

    public class Employees
    {
        public List<Employee> List { get; set; }
    }
}
