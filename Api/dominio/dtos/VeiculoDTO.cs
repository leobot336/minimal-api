using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace minimal_api.dominio.dtos
{
    public record VeiculoDTO
    {               
        public required string Nome { get; set; }        
        public required string Marca { get; set; }        
        public int Ano { get; set; }
    }
}