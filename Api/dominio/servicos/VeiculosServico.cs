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
    public class VeiculosServico : IVeiculoServico
    {
        private readonly DbContexto _contexto;
        public VeiculosServico(DbContexto contexto)
        {
            _contexto = contexto;
        }

        public void Apagar(Veiculo veiculo)
        {
            _contexto.Veiculos.Remove(veiculo);
            _contexto.SaveChanges();
        }

        public void Atualizar(Veiculo veiculo)
        {
            _contexto.Veiculos.Update(veiculo);
            _contexto.SaveChanges();
        }

        public Veiculo? BuscaPorId(int id)
        {
            return _contexto.Veiculos.FirstOrDefault(v => v.Id == id );
        }

        public void Incluir(Veiculo veiculo)
        {
            _contexto.Veiculos.Add(veiculo);
            _contexto.SaveChanges();
        }

        public List<Veiculo> Todos(int? pagina, string? nome = null, string? marca = null)
        {
            var query = _contexto.Veiculos.AsQueryable();
            if (!string.IsNullOrEmpty(nome))
            {
                var veiculos = query.Where(v => v.Nome.ToLower().Contains(nome)).ToList();
                return veiculos;
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