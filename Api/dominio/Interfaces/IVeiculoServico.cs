using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using minimal_api.dominio.dtos;
using minimal_api.dominio.entidades;

namespace minimal_api.infraestutura.Interfaces
{
    public interface IVeiculoServico
    {
        List<Veiculo> Todos(int? pagina, string? nome= null, string? marca= null);
        Veiculo? BuscaPorId(int id);
        void Incluir(Veiculo veiculo);
        void Atualizar(Veiculo veiculo);
        void Apagar(Veiculo veiculo);
    }
}