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
            QueryPetCountByType(type: "Cat");
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
            List<Pet> pets = petContext.Pets
                .FromSqlRaw("SELECT [PetId], [Name], [Type] FROM Pets WHERE [Type] = {0}", type)
                .ToList();

            foreach (var pet in pets)
            {
                Console.WriteLine($"Name: {pet.Name}");
            }
        }

        private static void QueryPetCountByType(string type)
        {
            using var petContext = new PetContext();
            PetCount petCount = petContext.PetCounts
                .FromSqlRaw("SELECT COUNT(*) as 'Count' FROM Pets WHERE [Type] = {0}", type)
                .Single();

            Console.WriteLine($"Count: {petCount.Count}");
        }
    }

    public class PetContext : DbContext
    {
        public DbSet<Pet> Pets { get; set; }
        public DbSet<PetCount> PetCounts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=pets.db");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<PetCount>(eb =>
                {
                    eb.HasNoKey();
                    eb.Property(Pet => Pet.Count).HasColumnName("Count");
                });
        }
    }

    public class Pet
    {
        public int PetId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }


    public class PetCount
    {
        public int Count { get; set; }
    }
}
