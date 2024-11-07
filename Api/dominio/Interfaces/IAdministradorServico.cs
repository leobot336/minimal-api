using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using minimal_api.dominio.dtos;
using minimal_api.dominio.entidades;

namespace minimal_api.infraestutura.Interfaces
{
    public interface IAdministradorServico
    {
        Administrador? Login(LoginDTO loginDTO);
        void Incluir(Administrador administrador);
        List<Administrador>Todos(int? pagina, string? email = null, string? perfil  = null);
        Administrador BuscarPorId(int id);
        void Atualizar(Administrador administrador);
        void Apagar(Administrador administrador);
    }
}