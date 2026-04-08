namespace TotemSantaCasa.Models
{
    public class TotemConfig
    {
        public string TotemId { get; set; } = string.Empty;
        public string NomeInstituicao { get; set; } = "Santa Casa de Curitiba";
        public string Ambulatorio { get; set; } = "Dom Eurico";
        public PaletaCores Paleta { get; set; } = new();
        public Tipografia Tipografia { get; set; } = new();
        public LoadScreenConfig LoadScreen { get; set; } = new();
        public string EndpointApi { get; set; } = string.Empty;
        public int PollingIntervalSegundos { get; set; } = 300;
        public int TimeoutTela { get; set; } = 60;
        public DateTime UltimaAtualizacao { get; set; } = DateTime.MinValue;
        public string Versao { get; set; } = "1.0";
    }

    public class PaletaCores
    {
        public string Primaria { get; set; } = "#B41E1E";
        public string Secundaria { get; set; } = "#DC3C3C";
        public string Fundo { get; set; } = "#FFFFFF";
        public string Texto { get; set; } = "#1E1E1E";
        public string BotaoConfirmar { get; set; } = "#228B22";
        public string Cabecalho { get; set; } = "#B41E1E";
        public string CabecalhoTexto { get; set; } = "#FFFFFF";
    }

    public class Tipografia
    {
        public string FamiliaFonte { get; set; } = "Segoe UI";
        public int TamanhoBase { get; set; } = 14;
        public int TamanhoBotao { get; set; } = 18;
        public int TamanhoCabecalho { get; set; } = 22;
        public int TamanhoTitulo { get; set; } = 28;
    }

    public class LoadScreenConfig
    {
        public string UrlLogoInstituicao { get; set; } = string.Empty;
        public string UrlLogoDigitaly { get; set; } = string.Empty;
        public string MensagemBoasVindas { get; set; } = "Inicializando sistema...";
        public string CorFundo { get; set; } = "#B41E1E";
        public string CorTexto { get; set; } = "#FFFFFF";
        public string CorBarra { get; set; } = "#FFFFFF";
        public bool ExibirLogoDigitaly { get; set; } = true;
    }

    public class LicencaInfo
    {
        public string TotemId { get; set; } = string.Empty;
        public string HardwareFingerprint { get; set; } = string.Empty;
        public string ChaveAtivacao { get; set; } = string.Empty;
        public bool Ativa { get; set; }
        public DateTime ValidaAte { get; set; }
        public string NomeCliente { get; set; } = string.Empty;
        public string Plano { get; set; } = string.Empty;
        public string MensagemBloqueio { get; set; } = string.Empty;
    }

    public class ResultadoValidacaoLicenca
    {
        public bool Valida { get; set; }
        public string Mensagem { get; set; } = string.Empty;
        public LicencaInfo? Licenca { get; set; }
    }
}
