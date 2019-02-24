using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PartagesWeb.APIUnitTests
{
    public class AFaireUnitTest
    {
        [Fact]
        public void AFaireGestionPages()
        {
            Assert.True(1 == 0, "effacer Section quand TitreMenu (problème de relations)");

            Assert.True(1 == 0, "create TitreMenu (création UnitTest)");
            Assert.True(1 == 0, "edit TitreMenu");
            Assert.True(1 == 0, "effacer TitreMenu (création UnitTest)");
            Assert.True(1 == 0, "up TitreMenu");
            Assert.True(1 == 0, "down TitreMenu");

            Assert.True(1 == 0, "Register erreur + seed user ne fonctionne pas en cas de delete de la database");


        }
    }
}
