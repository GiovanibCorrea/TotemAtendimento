using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using TotemSantaCasa.Models;

namespace TotemSantaCasa.Services
{
    public class LicencaService
    {
        private const string ArquivoLicencaLocal = "licenca.json";
        private const string ArquivoChaveLocal = "chave_local.key";
        private static readonly string DiretorioApp =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Digitaly", "Totem");

        private readonly HttpClient _httpClient;
        private readonly string _servidorLicencaUrl;

        public LicencaService(string servidorUrl = "")
        {
            _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };
            _servidorLicencaUrl = servidorUrl;
            Directory.CreateDirectory(DiretorioApp);
        }

        public async Task<ResultadoValidacaoLicenca> ValidarAsync()
        {
            string fingerprint = HardwareService.ObterFingerprint();
            string totemId = HardwareService.ObterTotemId();

            var resultadoRemoto = await ValidarRemotoAsync(fingerprint, totemId);
            if (resultadoRemoto.Valida)
            {
                SalvarLicencaLocal(resultadoRemoto.Licenca!);
                return resultadoRemoto;
            }

            var resultadoCache = ValidarCacheLocal(fingerprint);
            if (resultadoCache.Valida)
                return resultadoCache;

            return new ResultadoValidacaoLicenca
            {
                Valida = false,
                Mensagem = "Licença não encontrada ou expirada. Contate a Digitaly Technology."
            };
        }

        public ResultadoValidacaoLicenca ValidarChaveManual(string chaveDigitada)
        {
            string fingerprint = HardwareService.ObterFingerprint();
            string totemId = HardwareService.ObterTotemId();

            string chaveEsperada = GerarChaveLocal(fingerprint, totemId);
            string chaveDigitadaNorm = chaveDigitada.Replace("-", "").Replace(" ", "").ToUpper();
            string chaveEsperadaNorm = chaveEsperada.Replace("-", "").ToUpper();

            if (chaveDigitadaNorm == chaveEsperadaNorm)
            {
                var licenca = new LicencaInfo
                {
                    TotemId = totemId,
                    HardwareFingerprint = fingerprint,
                    ChaveAtivacao = chaveDigitada,
                    Ativa = true,
                    ValidaAte = DateTime.Now.AddYears(1),
                    NomeCliente = "Ativação Local",
                    Plano = "Local"
                };
                SalvarLicencaLocal(licenca);
                SalvarChaveLocal(chaveDigitada);

                return new ResultadoValidacaoLicenca
                {
                    Valida = true,
                    Mensagem = "Chave válida. Sistema ativado com sucesso.",
                    Licenca = licenca
                };
            }

            return new ResultadoValidacaoLicenca
            {
                Valida = false,
                Mensagem = "Chave de ativação inválida para este hardware."
            };
        }

        private async Task<ResultadoValidacaoLicenca> ValidarRemotoAsync(string fingerprint, string totemId)
        {
            if (string.IsNullOrEmpty(_servidorLicencaUrl))
                return new ResultadoValidacaoLicenca { Valida = false, Mensagem = "Servidor não configurado." };

            try
            {
                var payload = new { totemId, fingerprint, versao = "1.0" };
                var json = JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_servidorLicencaUrl}/api/licenca/validar", content);

                if (!response.IsSuccessStatusCode)
                    return new ResultadoValidacaoLicenca { Valida = false, Mensagem = "Servidor de licença indisponível." };

                var responseJson = await response.Content.ReadAsStringAsync();
                var resultado = JsonConvert.DeserializeObject<ResultadoValidacaoLicenca>(responseJson);
                return resultado ?? new ResultadoValidacaoLicenca { Valida = false };
            }
            catch
            {
                return new ResultadoValidacaoLicenca { Valida = false, Mensagem = "Sem conexão com servidor de licença." };
            }
        }

        private ResultadoValidacaoLicenca ValidarCacheLocal(string fingerprint)
        {
            try
            {
                string caminho = Path.Combine(DiretorioApp, ArquivoLicencaLocal);
                if (!File.Exists(caminho))
                    return new ResultadoValidacaoLicenca { Valida = false };

                var json = File.ReadAllText(caminho);
                var licenca = JsonConvert.DeserializeObject<LicencaInfo>(json);

                if (licenca == null || !licenca.Ativa)
                    return new ResultadoValidacaoLicenca { Valida = false, Mensagem = "Licença local inativa." };

                if (licenca.HardwareFingerprint != fingerprint)
                    return new ResultadoValidacaoLicenca { Valida = false, Mensagem = "Licença não pertence a este hardware." };

                if (licenca.ValidaAte < DateTime.Now)
                    return new ResultadoValidacaoLicenca { Valida = false, Mensagem = "Licença expirada." };

                return new ResultadoValidacaoLicenca
                {
                    Valida = true,
                    Mensagem = "Licença local válida (modo offline).",
                    Licenca = licenca
                };
            }
            catch
            {
                return new ResultadoValidacaoLicenca { Valida = false };
            }
        }

        private void SalvarLicencaLocal(LicencaInfo licenca)
        {
            try
            {
                string caminho = Path.Combine(DiretorioApp, ArquivoLicencaLocal);
                File.WriteAllText(caminho, JsonConvert.SerializeObject(licenca, Formatting.Indented));
            }
            catch { }
        }

        private void SalvarChaveLocal(string chave)
        {
            try
            {
                string caminho = Path.Combine(DiretorioApp, ArquivoChaveLocal);
                File.WriteAllText(caminho, chave);
            }
            catch { }
        }

        public static string GerarChaveLocal(string fingerprint, string totemId)
        {
            string seed = $"DIGITALY-{fingerprint}-{totemId}-TOTEM";
            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(seed));
            string hex = Convert.ToHexString(hash)[..20].ToUpper();
            return $"{hex[..5]}-{hex[5..10]}-{hex[10..15]}-{hex[15..20]}";
        }

        public string ObterChaveEsperadaParaEsteHardware()
        {
            string fingerprint = HardwareService.ObterFingerprint();
            string totemId = HardwareService.ObterTotemId();
            return GerarChaveLocal(fingerprint, totemId);
        }
    }
}
