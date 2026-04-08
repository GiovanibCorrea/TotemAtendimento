using System.Drawing;
using Newtonsoft.Json;
using TotemSantaCasa.Models;

namespace TotemSantaCasa.Services
{
    public class ConfiguracaoService
    {
        private const string ArquivoConfigLocal = "totem_config.json";
        private static readonly string DiretorioApp =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Digitaly", "Totem");

        private readonly HttpClient _httpClient;
        private TotemConfig _configAtual = new();

        public event Action<TotemConfig>? ConfiguracaoAtualizada;
        public TotemConfig ConfigAtual => _configAtual;

        public ConfiguracaoService()
        {
            _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(20) };
            Directory.CreateDirectory(DiretorioApp);
        }

        public async Task<TotemConfig> InicializarAsync(string endpointRemoto = "")
        {
            if (!string.IsNullOrEmpty(endpointRemoto))
            {
                var configRemota = await BuscarConfigRemotaAsync(endpointRemoto);
                if (configRemota != null)
                {
                    configRemota.UltimaAtualizacao = DateTime.Now;
                    SalvarConfigLocal(configRemota);
                    _configAtual = configRemota;
                    AplicarConfigNaUI(_configAtual);
                    return _configAtual;
                }
            }

            var configLocal = CarregarConfigLocal();
            if (configLocal != null)
            {
                _configAtual = configLocal;
                AplicarConfigNaUI(_configAtual);
                return _configAtual;
            }

            _configAtual = GerarConfigPadrao();
            SalvarConfigLocal(_configAtual);
            AplicarConfigNaUI(_configAtual);
            return _configAtual;
        }

        public TotemConfig AplicarConfigLocalManual(TotemConfig config)
        {
            config.UltimaAtualizacao = DateTime.Now;
            SalvarConfigLocal(config);
            _configAtual = config;
            AplicarConfigNaUI(_configAtual);
            ConfiguracaoAtualizada?.Invoke(_configAtual);
            return _configAtual;
        }

        public TotemConfig? CarregarConfigDeArquivo(string caminhoArquivo)
        {
            try
            {
                var json = File.ReadAllText(caminhoArquivo);
                return JsonConvert.DeserializeObject<TotemConfig>(json);
            }
            catch { return null; }
        }

        public void SalvarConfigParaArquivo(string caminhoArquivo, TotemConfig config)
        {
            File.WriteAllText(caminhoArquivo, JsonConvert.SerializeObject(config, Formatting.Indented));
        }

        private async Task<TotemConfig?> BuscarConfigRemotaAsync(string endpoint)
        {
            try
            {
                string totemId = HardwareService.ObterTotemId();
                string url = endpoint.Contains('?')
                    ? $"{endpoint}&totemId={totemId}"
                    : $"{endpoint}?totemId={totemId}";

                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode) return null;

                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TotemConfig>(json);
            }
            catch { return null; }
        }

        private TotemConfig? CarregarConfigLocal()
        {
            try
            {
                string caminho = Path.Combine(DiretorioApp, ArquivoConfigLocal);
                if (!File.Exists(caminho)) return null;
                var json = File.ReadAllText(caminho);
                return JsonConvert.DeserializeObject<TotemConfig>(json);
            }
            catch { return null; }
        }

        private void SalvarConfigLocal(TotemConfig config)
        {
            try
            {
                string caminho = Path.Combine(DiretorioApp, ArquivoConfigLocal);
                File.WriteAllText(caminho, JsonConvert.SerializeObject(config, Formatting.Indented));
            }
            catch { }
        }

        private static void AplicarConfigNaUI(TotemConfig config)
        {
            try
            {
                var p = config.Paleta;
                UITheme.Primaria      = ParseColor(p.Primaria,       UITheme.Primaria);
                UITheme.Secundaria    = ParseColor(p.Secundaria,     UITheme.Secundaria);
                UITheme.Fundo         = ParseColor(p.Fundo,          UITheme.Fundo);
                UITheme.Texto         = ParseColor(p.Texto,          UITheme.Texto);
                UITheme.BotaoConfirmar= ParseColor(p.BotaoConfirmar, UITheme.BotaoConfirmar);
                UITheme.Cabecalho     = ParseColor(p.Cabecalho,      UITheme.Cabecalho);
                UITheme.CabecalhoTexto= ParseColor(p.CabecalhoTexto, UITheme.CabecalhoTexto);

                var t = config.Tipografia;
                UITheme.FamiliaFonte     = t.FamiliaFonte;
                UITheme.TamanhoBase      = t.TamanhoBase;
                UITheme.TamanhoBotao     = t.TamanhoBotao;
                UITheme.TamanhoCabecalho = t.TamanhoCabecalho;
                UITheme.TamanhoTitulo    = t.TamanhoTitulo;
            }
            catch { }
        }

        private static Color ParseColor(string hex, Color fallback)
        {
            try { return ColorTranslator.FromHtml(hex); }
            catch { return fallback; }
        }

        private static TotemConfig GerarConfigPadrao() => new()
        {
            TotemId            = HardwareService.ObterTotemId(),
            NomeInstituicao    = "Santa Casa de Curitiba",
            Ambulatorio        = "Dom Eurico",
            TimeoutTela        = 60,
            Paleta             = new PaletaCores(),
            Tipografia         = new Tipografia(),
            LoadScreen         = new LoadScreenConfig(),
            Versao             = "1.0"
        };
    }

    public static class UITheme
    {
        public static Color Primaria       { get; set; } = Color.FromArgb(180, 30, 30);
        public static Color Secundaria     { get; set; } = Color.FromArgb(220, 60, 60);
        public static Color Fundo          { get; set; } = Color.White;
        public static Color Texto          { get; set; } = Color.FromArgb(30, 30, 30);
        public static Color BotaoConfirmar { get; set; } = Color.FromArgb(34, 139, 34);
        public static Color Cabecalho      { get; set; } = Color.FromArgb(180, 30, 30);
        public static Color CabecalhoTexto { get; set; } = Color.White;
        public static Color CinzaClaro     { get; set; } = Color.FromArgb(245, 245, 245);
        public static Color CinzaBorda     { get; set; } = Color.FromArgb(200, 200, 200);

        public static string FamiliaFonte      { get; set; } = "Segoe UI";
        public static int    TamanhoBase       { get; set; } = 14;
        public static int    TamanhoBotao      { get; set; } = 18;
        public static int    TamanhoCabecalho  { get; set; } = 22;
        public static int    TamanhoTitulo     { get; set; } = 28;

        public static Font FonteBase(FontStyle e = FontStyle.Regular)     => new(FamiliaFonte, TamanhoBase, e);
        public static Font FonteBotao(FontStyle e = FontStyle.Bold)        => new(FamiliaFonte, TamanhoBotao, e);
        public static Font FonteCabecalho(FontStyle e = FontStyle.Bold)    => new(FamiliaFonte, TamanhoCabecalho, e);
        public static Font FonteTitulo(FontStyle e = FontStyle.Bold)       => new(FamiliaFonte, TamanhoTitulo, e);
        public static Font FonteCustom(int tam, FontStyle e = FontStyle.Regular) => new(FamiliaFonte, tam, e);
    }
}
