using System.Xml.Serialization;

namespace CalculoNotaAPI.Models
{
    public record CalculoRequest(string Xml, int PeriodoDias);

    [XmlRoot("NotaFiscal")]
    public class NotaFiscal
    {
        [XmlElement("Totais")]
        public Totais Totais { get; set; }
    }

    public class Totais
    {
        public decimal ValorTotalNota { get; set; }
    }
}
