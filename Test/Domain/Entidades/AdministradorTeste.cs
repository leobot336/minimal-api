
using minimal_api.dominio.entidades;

namespace Test.Domain.Entidades
{
    [TestClass]
    public class AdministradorTeste
    {
        [TestMethod]
        public void testarGetSetPropriedades()
        {
            //arrange
            var adm = new Administrador();

            //act
            adm.Id = 1;
            adm.Email = "teste@mail.com";
            adm.Senha = "teste";
            adm.Perfil = "Adm";
            //assert
            Assert.AreEqual(1, adm.Id);
            Assert.AreEqual("teste@mail.com", adm.Email);
            Assert.AreEqual("teste", adm.Senha);
            Assert.AreEqual("Adm", adm.Perfil);

        }

    }
}