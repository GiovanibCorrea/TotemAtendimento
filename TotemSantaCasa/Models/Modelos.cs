namespace TotemSantaCasa.Models
{
    public class Paciente
    {
        public string NomeCompleto { get; set; } = string.Empty;
        public string CPF { get; set; } = string.Empty;
        public DateTime DataNascimento { get; set; }
        public string Sexo { get; set; } = string.Empty;
        public string NomeMae { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public string NumeroAtendimento { get; set; } = string.Empty;
        public string Setor { get; set; } = string.Empty;
        public string MedicoResponsavel { get; set; } = string.Empty;
        public string Especialidade { get; set; } = string.Empty;
        public string Convenio { get; set; } = string.Empty;
        public string StatusAgendamento { get; set; } = string.Empty;
        public DateTime HoraAgendamento { get; set; }
        public string CodigoBarras { get; set; } = string.Empty;
    }

    public class Agendamento
    {
        public string NumeroSequencia { get; set; } = string.Empty;
        public DateTime DataHora { get; set; }
        public string Especialidade { get; set; } = string.Empty;
        public string MedicoResponsavel { get; set; } = string.Empty;
        public string TipoAgendamento { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Convenio { get; set; } = string.Empty;
        public string Setor { get; set; } = string.Empty;
        public string Observacao { get; set; } = string.Empty;
        public string TipoProcedimento { get; set; } = string.Empty;
    }

    public class ResultadoCheckin
    {
        public bool Sucesso { get; set; }
        public string NumeroAtendimento { get; set; } = string.Empty;
        public string Mensagem { get; set; } = string.Empty;
        public Paciente? Paciente { get; set; }
        public Agendamento? Agendamento { get; set; }
    }

    public enum TipoAtendimento
    {
        Consulta,
        Exame,
        Senha
    }
}
