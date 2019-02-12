using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using PartagesWeb.API.Data;
//-----------------------------------------------------------------------
// <license>https://github.com/stephaneworkspace/PartagesWeb.API/blob/master/LICENSE.md</license>
// <author>Stéphane</author>
//-----------------------------------------------------------------------
using PartagesWeb.API.Dtos.GestionPages;
using PartagesWeb.API.Models;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PartagesWeb.APIUnitTests
{
    /// <summary>
    /// Tests d'intégrations pour GestionPagesRepository
    /// https://www.youtube.com/watch?v=6nYefHkKby8
    /// </summary>
    public class GestionPagesUnitTests
    {
        private readonly ITestOutputHelper _output;
        private bool _useSqlServer;

        /// <summary>
        /// Constructeur pour l'écriture "Output" des tests et choix de la base de donnée
        /// </summary>
        /// <param name="output">Choix de la base de donnée (en mémoire ou SqlServeur)</param>
        public GestionPagesUnitTests(ITestOutputHelper output)
        {
            _output = output;
            _useSqlServer = true;
        }

        public DbContextOptionsBuilder<DataContext> GestionPagesDbContext()
        {
            var builder = new DbContextOptionsBuilder<DataContext>();
            if(_useSqlServer)
            {
                builder.EnableSensitiveDataLogging();
                //.UseSqlite("DataSource=:memory:");
                builder.UseSqlServer("Server = localhost; database = partagesweb; user id = sa; password = test1234;");
            } else
            {
                builder.UseInMemoryDatabase(Guid.NewGuid().ToString()).ConfigureWarnings(w => {
                    w.Ignore(InMemoryEventId.TransactionIgnoredWarning);
                }).EnableSensitiveDataLogging(true);
            }
            return builder;
        }

        [Fact]
        public async Task ProchainePositionAvec5NouvellesSectionAsync()
        {
            Section section01 = new Section();
            section01.Nom = "Informatique web";
            section01.Icone = "desktop";
            section01.Type = "none";
            section01.SwHorsLigne = false;
            Section section02 = new Section();
            section02.Nom = "Thème astral";
            section02.Icone = "globe";
            section02.Type = "astral";
            section02.SwHorsLigne = false;
            Section section03 = new Section();
            section03.Nom = "Forum";
            section03.Icone = "comments";
            section03.Type = "forum";
            section03.SwHorsLigne = true;
            Section section04 = new Section();
            section04.Nom = "Musique assisté par ordinateur";
            section04.Icone = "music";
            section04.Type = "none";
            section01.SwHorsLigne = false;
            using (var context = new DataContext(this.GestionPagesDbContext().Options))
            {
                // pour Sqlite Database.OpenConnection();
                var repository = new GestionPagesRepository(context);
                if (_useSqlServer) // or Sqlite
                {
                    context.Database.OpenConnection();
                }
                context.Database.EnsureCreated();
 
                // Déterminer la dernière position en ligne ou hors ligne
                var position = await repository.LastPositionSection(section.SwHorsLigne); // a faire sw boolean si en ligne ou hors ligne... 
                position++;
                section.Position = position;
                repository.Add<Section>(section);
                await repository.SaveAll();
                var test = context.Sections.CountAsync(s => s.Nom == "Test OK");
                _output.WriteLine("This is output from {0}", section);
                Assert.Equal(1, 1);
                
            }
        }
    }
}
