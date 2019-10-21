using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EFRawSQLSample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreatePets();
            QueryPetsByType(type: "Cat");
        }

        private static void CreatePets()
        {
            using var petContext = new PetContext();
            petContext.Database.EnsureCreated();
            petContext.Database.ExecuteSqlRaw("DELETE FROM Pets WHERE 1 = 1");
            petContext.Pets.Add(new Pet { Name = "Grumpy Cat", Type = "Cat" });
            petContext.Pets.Add(new Pet { Name = "Smelly Cat", Type = "Cat" });
            petContext.Pets.Add(new Pet { Name = "Lassie", Type = "Dog" });
            petContext.SaveChanges();
        }

        private static void QueryPetsByType(string type)
        {
            using var petContext = new PetContext();
            List<Pet> cats = petContext.Pets
                .FromSqlRaw("SELECT [PetId], [Name], [Type] FROM Pets WHERE [Type] = {0}", type)
                .ToList();

            foreach (var cat in cats)
            {
                Console.WriteLine($"Name: {cat.Name}");
            }
        }
    }

    public class PetContext : DbContext
    {
        public DbSet<Pet> Pets { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=pets.db");
    }

    public class Pet
    {
        public int PetId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }
}
