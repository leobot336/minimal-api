using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using minimal_api.dominio.Enuns;

namespace minimal_api.dominio.dtos
{
    public class AdministradorDTO
    {        
        public required string Email { get; set; }        
        public required string Senha { get; set; }        
        public required Perfil? Perfil { get; set; }
    }
}