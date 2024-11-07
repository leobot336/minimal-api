using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace minimal_api.dominio.ModelViews
{
    public class Home
    {
        public string Mensagem { get => "Bem vindo a API de veículos - Minimal API";}
        public string Documentacao {get => "/swagger";}
    }
}