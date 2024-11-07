using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using minimal_api.dominio.dtos;
using minimal_api.dominio.entidades;
using minimal_api.infraestutura.DB;
using minimal_api.infraestutura.Interfaces;

namespace minimal_api.dominio.servicos
{
    public class AdministradoServico : IAdministradorServico
    {
        private readonly DbContexto _contexto;
        public AdministradoServico(DbContexto contexto)
        {
            _contexto = contexto;
        }

        public void Apagar(Administrador administrador)
        {
            _contexto.Administradores.Remove(administrador);
            _contexto.SaveChanges();
        }

        public void Atualizar(Administrador administrador)
        {
            _contexto.Administradores.Update(administrador);
            _contexto.SaveChanges();
        }

        public Administrador BuscarPorId(int id)
        {
            var adm =_contexto.Administradores.FirstOrDefault(a => a.Id == id);
            return adm;
            
        }

        public void Incluir(Administrador administrador)
        {
            _contexto.Add(administrador);
            _contexto.SaveChanges();
        }

        public Administrador? Login(LoginDTO loginDTO)
        {
           var adm = _contexto.Administradores.Where(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha).FirstOrDefault();
           return adm;
        }

        public List<Administrador> Todos(int? pagina, string? email = null, string? perfil  = null)
        {
            var query = _contexto.Administradores.AsQueryable();
            if (!string.IsNullOrEmpty(email))
            {
                var administradores = query.Where(a => a.Email.ToLower().Contains(email)).ToList();
                return administradores;
            }
            int itensPorPagina = 10;
            if (pagina != null)
            {
                query = query.Skip(((int)pagina - 1) * itensPorPagina).Take(itensPorPagina);
            }            
            return query.ToList();
        }       
    }
}