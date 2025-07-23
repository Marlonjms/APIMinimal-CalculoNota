namespace CalculoNotaAPI.Models
{
    public record LoginRequest(string Email, string Senha);
    public record XmlInput(string Xml);

    public class NotaFiscal
    {
        public decimal ValorRecebivel { get; set; }
        public decimal Taxa { get; set; }
        public int Periodos { get; set; }
    }
}
