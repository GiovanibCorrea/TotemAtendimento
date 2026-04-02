using TotemSantaCasa.Models;

namespace TotemSantaCasa.Services
{
    public interface IApiService
    {
        Task<Paciente?> BuscarPacientePorCpfAsync(string cpf);
        Task<ResultadoCheckin> RealizarCheckinAsync(string cpf, TipoAtendimento tipo);
        Task<bool> ConfirmarAtendimentoAsync(string numeroAtendimento);
    }

    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public ApiService()
        {
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(Config.Configuracoes.ApiTimeout)
            };
            _baseUrl = Config.Configuracoes.ApiBaseUrl;
        }

        public async Task<Paciente?> BuscarPacientePorCpfAsync(string cpf)
        {
            await Task.Delay(500);

            if (string.IsNullOrEmpty(_baseUrl))
                return ObterPacienteMock(cpf);

            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/pacientes/{cpf}");
                if (!response.IsSuccessStatusCode)
                    return null;

                var json = await response.Content.ReadAsStringAsync();
                return Newtonsoft.Json.JsonConvert.DeserializeObject<Paciente>(json);
            }
            catch
            {
                return null;
            }
        }

        public async Task<ResultadoCheckin> RealizarCheckinAsync(string cpf, TipoAtendimento tipo)
        {
            await Task.Delay(800);

            if (string.IsNullOrEmpty(_baseUrl))
                return ObterCheckinMock(cpf, tipo);

            try
            {
                var body = new { cpf, tipo = tipo.ToString() };
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(body);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_baseUrl}/checkin", content);

                if (!response.IsSuccessStatusCode)
                    return new ResultadoCheckin { Sucesso = false, Mensagem = "Erro na integração com o sistema." };

                var responseJson = await response.Content.ReadAsStringAsync();
                return Newtonsoft.Json.JsonConvert.DeserializeObject<ResultadoCheckin>(responseJson)
                    ?? new ResultadoCheckin { Sucesso = false, Mensagem = "Erro ao processar resposta." };
            }
            catch (Exception ex)
            {
                return new ResultadoCheckin { Sucesso = false, Mensagem = $"Erro de comunicação: {ex.Message}" };
            }
        }

        public async Task<bool> ConfirmarAtendimentoAsync(string numeroAtendimento)
        {
            await Task.Delay(300);

            if (string.IsNullOrEmpty(_baseUrl))
                return true;

            try
            {
                var response = await _httpClient.PostAsync($"{_baseUrl}/atendimentos/{numeroAtendimento}/confirmar", null);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        private static Paciente? ObterPacienteMock(string cpf)
        {
            if (cpf == "00000000000")
                return null;

            return new Paciente
            {
                NomeCompleto = "MAYARA IZABELA COLAÇO DOS SANTOS",
                CPF = cpf,
                DataNascimento = new DateTime(1988, 12, 7),
                Sexo = "F",
                NomeMae = "SHEILLA CRISTINA FREITAS DOS SANTOS",
                Telefone = "(47) 3 997-5400",
                NumeroAtendimento = "1734995",
                Setor = "Setor 3",
                MedicoResponsavel = "MARISA PIZZICHINI",
                Especialidade = "Anestesiologia",
                Convenio = "SUS - Sistema Único de Saúde",
                StatusAgendamento = "Normal",
                HoraAgendamento = DateTime.Now,
                CodigoBarras = "1734995"
            };
        }

        private static ResultadoCheckin ObterCheckinMock(string cpf, TipoAtendimento tipo)
        {
            var paciente = ObterPacienteMock(cpf);
            if (paciente == null)
                return new ResultadoCheckin
                {
                    Sucesso = false,
                    Mensagem = "CPF não localizado no sistema."
                };

            return new ResultadoCheckin
            {
                Sucesso = true,
                NumeroAtendimento = paciente.NumeroAtendimento,
                Mensagem = "Check-in realizado com sucesso.",
                Paciente = paciente,
                Agendamento = new Agendamento
                {
                    NumeroSequencia = "36281333",
                    DataHora = DateTime.Now,
                    Especialidade = paciente.Especialidade,
                    MedicoResponsavel = paciente.MedicoResponsavel,
                    TipoAgendamento = tipo == TipoAtendimento.Consulta ? "Consulta" : "Exame",
                    Status = "Normal",
                    Convenio = paciente.Convenio,
                    Setor = paciente.Setor
                }
            };
        }
    }
}
