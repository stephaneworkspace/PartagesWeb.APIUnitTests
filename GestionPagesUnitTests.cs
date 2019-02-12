//-----------------------------------------------------------------------
// <license>https://github.com/stephaneworkspace/PartagesWeb.API/blob/master/LICENSE.md</license>
// <author>St�phane</author>
//-----------------------------------------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using PartagesWeb.API.Data;
using PartagesWeb.API.Dtos.GestionPages;
using PartagesWeb.API.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PartagesWeb.APIUnitTests
{
    /// <summary>
    /// Tests d'int�grations pour GestionPagesRepository
    /// https://www.youtube.com/watch?v=6nYefHkKby8
    /// </summary>
    public class GestionPagesUnitTests
    {
        private readonly ITestOutputHelper _output;
        private bool _useSqlServer;
        private bool _detail;

        /// <summary>
        /// Constructeur pour l'�criture "Output" des tests et choix de la base de donn�e ainsi que si on veut afficher le detail des tests unitaires
        /// </summary>
        /// <param name="output">Choix de la base de donn�e (en m�moire ou SqlServeur)</param>
        public GestionPagesUnitTests(ITestOutputHelper output)
        {
            _output = output;
            _useSqlServer = false;
            _detail = false;
        }

        /// <summary>
        /// Cr�ation DataContext
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// Ajoute 7 sections et effectue quelques tests en rapport avec SectionsController
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ProchainePositionAvec5NouvellesSectionAsync()
        {
            Section section01 = new Section();
            section01.Nom = "Informatique web";
            section01.Icone = "desktop";
            section01.Type = "none";
            section01.SwHorsLigne = false;
            Section section02 = new Section();
            section02.Nom = "Th�me astral";
            section02.Icone = "globe";
            section02.Type = "astral";
            section02.SwHorsLigne = false;
            Section section03 = new Section();
            section03.Nom = "Forum";
            section03.Icone = "comments";
            section03.Type = "forum";
            section03.SwHorsLigne = true;
            Section section04 = new Section();
            section04.Nom = "Musique assist� par ordinateur";
            section04.Icone = "music";
            section04.Type = "none";
            section01.SwHorsLigne = false;
            Section section05 = new Section();
            section05.Nom = "Galerie de dessins";
            section05.Icone = "image";
            section05.Type = "galerie";
            section05.SwHorsLigne = false;
            Section section06 = new Section();
            section06.Nom = "Contenu hors ligne A";
            section06.Icone = "desktop";
            section06.Type = "none";
            section06.SwHorsLigne = true;
            Section section07 = new Section();
            section07.Nom = "Contenu hors ligne B";
            section07.Icone = "desktop";
            section07.Type = "none";
            section07.SwHorsLigne = true;
            using (var context = new DataContext(this.GestionPagesDbContext().Options))
            {
                // Connection au repository
                var repository = new GestionPagesRepository(context);
                if (_useSqlServer) // or Sqlite
                {
                    context.Database.OpenConnection();
                }
                context.Database.EnsureCreated();

                section01 = await CreateSection(section01, repository);
                section02 = await CreateSection(section02, repository);
                section03 = await CreateSection(section03, repository);
                section04 = await CreateSection(section04, repository);
                section05 = await CreateSection(section05, repository);
                section06 = await CreateSection(section06, repository);
                section07 = await CreateSection(section07, repository);

                // Test en ligne
                Assert.True(section02.Position > section01.Position, "section02.Position > section01.Position");
                Assert.True(section04.Position > section01.Position, "section04.Position > section01.Position");
                Assert.True(section05.Position > section01.Position, "section05.Position > section01.Position");
                Assert.True(section04.Position > section02.Position, "section04.Position > section02.Position");
                Assert.True(section05.Position > section02.Position, "section05.Position > section02.Position");
                Assert.True(section05.Position > section04.Position, "section05.Position > section04.Position");

                // Test hors ligne
                Assert.True(section06.Position > section03.Position, "section06.Position > section03.Position");
                Assert.True(section07.Position > section03.Position, "section07.Position > section03.Position");
                Assert.True(section07.Position > section06.Position, "section07.Position > section06.Position");

                List<Section> sections = await GetArbreCompletSections(repository);

                // Test en ligne

                // Monter section02 (et changer la valeur de section01)
                await MonterSection(section02.Id, repository);
                sections = await GetArbreCompletSections(repository);
                Assert.True(section01.Position > section02.Position, "sections01.Position > section02.Position");

                // Monter section05 (et changer la valeur de section04)
                await MonterSection(section05.Id, repository);
                sections = await GetArbreCompletSections(repository);
                Assert.True(section04.Position > section05.Position, "sections04.Position > section02.Position");

                // Descendre section02 (et changer la valeur de section01)
                await DescendreSection(section02.Id, repository);
                sections = await GetArbreCompletSections(repository);
                Assert.True(section02.Position > section01.Position, "sections02.Position > section01.Position");

                // Descendre section05 (et changer la valeur de section04)
                await DescendreSection(section05.Id, repository);
                sections = await GetArbreCompletSections(repository);
                Assert.True(section05.Position > section04.Position, "sections05.Position > section04.Position");

                // Test hors ligne

                // Monter sections06 (et changer la valeur de section03)
                await MonterSection(section06.Id, repository);
                sections = await GetArbreCompletSections(repository);
                Assert.True(section03.Position > section06.Position, "sections03.Position > section06.Position");

                // Monter sections07 (et changer la valeur de section03)
                await MonterSection(section07.Id, repository);
                sections = await GetArbreCompletSections(repository);
                Assert.True(section03.Position > section07.Position, "sections03.Position > section07.Position");

                // Descendre sections07 (et changer la valeur de section03)
                await DescendreSection(section07.Id, repository);
                sections = await GetArbreCompletSections(repository);
                Assert.True(section07.Position > section03.Position, "sections07.Position > section03.Position");

                // Descendre sections06 (et changer la valeur de section03)
                await DescendreSection(section06.Id, repository);
                sections = await GetArbreCompletSections(repository);
                Assert.True(section06.Position > section03.Position, "sections06.Position > section03.Position");

                // var test = context.Sections.CountAsync(s => s.Nom == "Informatique web");
                // Assert.Equal(1, 1);
            }
        }

        /// <summary>
        /// Equivalent dans SectionsController de public async Task<IActionResult> Create(SectionForCreateDto sectionForCreateDto)
        /// </summary>
        /// <param name="section">Model</param>
        /// <param name="repository">Repository</param>
        private async Task<Section> CreateSection(Section section, GestionPagesRepository repository)
        {
            // D�terminer la derni�re position en ligne ou hors ligne
            var position = await repository.LastPositionSection(section.SwHorsLigne);
            // Prochaine position
            position++;
            section.Position = position;
            repository.Add(section);
            await repository.SaveAll();

            
            _output.WriteLine("CreateSection(section, repository) - {0}", section.Id);
            if (_detail)
            {
                _output.WriteLine("");
                _output.WriteLine("Id {0}", section.Id);
                _output.WriteLine("Nom {0}", section.Nom);
                _output.WriteLine("Icone {0}", section.Icone);
                _output.WriteLine("Type {0}", section.Type);
                _output.WriteLine("Position {0}", section.Position);
                _output.WriteLine("SwHorsLigne {0}", section.SwHorsLigne);
                _output.WriteLine("");
            }

            return section;
        }
        /// <summary>
        /// Equivalent dans SectionsController de public async Task<IActionResult> GetSections()
        /// </summary>
        /// <param name="repository"></param>
        /// <returns>Repository</returns>
        private async Task<List<Section>> GetArbreCompletSections(GestionPagesRepository repository)
        {
            _output.WriteLine("GetArbreCompletSections(repository);");
            List<Section> sections = await repository.GetSections();
            foreach (var item in sections)
            {
                if (_detail)
                {
                    _output.WriteLine("");
                    _output.WriteLine("Id {0}", item.Id);
                    _output.WriteLine("Nom {0}", item.Nom);
                    _output.WriteLine("Icone {0}", item.Icone);
                    _output.WriteLine("Type {0}", item.Type);
                    _output.WriteLine("Position {0}", item.Position);
                    _output.WriteLine("SwHorsLigne {0}", item.SwHorsLigne);
                    _output.WriteLine("");
                }
            }
            return sections;
        }
        /// <summary>
        /// Equivalent dans SectionsController de public async Task<IActionResult> Monter(int id)
        /// </summary>
        /// <param name="id">Id de la section � monter</param>
        /// <param name="repository">Repository</param>
        /// <returns></returns>
        private async Task MonterSection(int id, GestionPagesRepository repository)
        {
            _output.WriteLine("MonterSection({0}, repository)", id);
            if (_detail)
            {
                _output.WriteLine("");
            }
            await repository.UpSection(id);
        }
        /// <summary>
        /// Equivalent dans SectionsController de public async Task<IActionResult> Descendre(int id)
        /// </summary>
        /// <param name="id">Id de la section � monter</param>
        /// <param name="repository">Repository</param>
        /// <returns></returns>
        private async Task DescendreSection(int id, GestionPagesRepository repository)
        {
            _output.WriteLine("DescendreSection({0}, repository)", id);
            if (_detail)
            {
                _output.WriteLine("");
            }
            await repository.DownSection(id);
        }
    }
}
