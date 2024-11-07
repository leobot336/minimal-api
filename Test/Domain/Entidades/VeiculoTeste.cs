using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using minimal_api.dominio.entidades;


namespace Test.Domain.Entidades
{
    [TestClass]
    public class VeiculoTeste
    {
        [TestMethod]        
        public void testarGetSetPropriedades()
        {
            //arrange
            var veiculo = new Veiculo();

            //act
            veiculo.Id = 1;
            veiculo.Marca = "teste";
            veiculo.Nome = "teste1";
            veiculo.Ano = 1999;
            //assert
            Assert.AreEqual(1, veiculo.Id);
            Assert.AreEqual("teste@mail.com", veiculo.Marca);
            Assert.AreEqual("teste1", veiculo.Nome);
            Assert.Equals(1999, veiculo.Ano);

        }
    }
}