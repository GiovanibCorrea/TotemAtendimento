using Microsoft.Extensions.Configuration;

namespace TotemSantaCasa.Config
{
    public static class Configuracoes
    {
        private static IConfiguration? _config;

        public static void Inicializar()
        {
            _config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
        }

        public static string ImpressoraTermica =>
            _config?["Impressoras:ImpressoraTermica"] ?? "IMPRESSORA_TERMICA";

        public static string ImpressoraZebra =>
            _config?["Impressoras:ImpressoraZebra"] ?? "ZEBRA_ZD220";

        public static string ApiBaseUrl =>
            _config?["API:BaseUrl"] ?? string.Empty;

        public static int ApiTimeout =>
            int.TryParse(_config?["API:Timeout"], out int t) ? t : 30;

        public static string NomeInstituicao =>
            _config?["Totem:NomeInstituicao"] ?? "Santa Casa de Curitiba";

        public static string Ambulatorio =>
            _config?["Totem:Ambulatorio"] ?? "Dom Eurico";

        public static int TimeoutTela =>
            int.TryParse(_config?["Totem:TimeoutTela"], out int t) ? t : 60;
    }
}
