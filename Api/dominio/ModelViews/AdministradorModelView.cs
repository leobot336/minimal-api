using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using minimal_api.dominio.Enuns;

namespace minimal_api.dominio.ModelViews
{
    public record AdministradorModelView
    {
        public string Email { get; set; }  
        public string Perfil { get; set; }
    }
}