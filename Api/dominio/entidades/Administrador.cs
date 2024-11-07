using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using minimal_api.dominio.Enuns;

namespace minimal_api.dominio.entidades
{
    public class Administrador
    {
        public Administrador()
        {
            
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }        
        public string Email { get; set; }        
        public string Senha { get; set; }        
        public string Perfil { get; set; }
       
    }
}