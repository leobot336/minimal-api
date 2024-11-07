namespace minimal_api.dominio.ModelViews
{
    public class ErrosDeValidacao
    {
        public ErrosDeValidacao()
        {
            Mensagens = new();
        }
        public List<string> Mensagens { get; set; }
    }
}