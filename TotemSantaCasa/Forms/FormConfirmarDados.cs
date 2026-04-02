using TotemSantaCasa.Models;

namespace TotemSantaCasa.Forms
{
    public class FormConfirmarDados : Form
    {
        private readonly ResultadoCheckin _resultado;
        private System.Windows.Forms.Timer _timerInatividade = new();

        public FormConfirmarDados(ResultadoCheckin resultado)
        {
            _resultado = resultado;
            InicializarComponentes();
            ConfigurarTimer();
        }

        private void InicializarComponentes()
        {
            Text = "Confirmar Dados";
            WindowState = FormWindowState.Maximized;
            FormBorderStyle = FormBorderStyle.None;
            BackColor = UIHelper.CorFundo;
            StartPosition = FormStartPosition.CenterScreen;

            string tituloAtend = _resultado.Agendamento?.TipoAgendamento?.ToUpper() ?? "ATENDIMENTO";
            var panelCabecalho = UIHelper.CriarCabecalho(tituloAtend, "Confirme seus dados cadastrais");
            Controls.Add(panelCabecalho);
            Controls.Add(UIHelper.CriarRodape());

            var panelConteudo = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = UIHelper.CorFundo
            };

            var panelCard = new Panel
            {
                Width = 620,
                Height = 520,
                BackColor = Color.White,
                Anchor = AnchorStyles.None
            };
            panelCard.Paint += (s, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, panelCard.ClientRectangle,
                    UIHelper.CorCinzaBorda, ButtonBorderStyle.Solid);
            };

            var panelTituloPaciente = new Panel
            {
                BackColor = UIHelper.CorPrimaria,
                Bounds = new Rectangle(0, 0, 620, 44)
            };
            var lblTituloPaciente = UIHelper.CriarLabel("DADOS DO PACIENTE", 13, FontStyle.Bold,
                Color.White, ContentAlignment.MiddleCenter);
            lblTituloPaciente.Dock = DockStyle.Fill;
            panelTituloPaciente.Controls.Add(lblTituloPaciente);

            int y = 52;
            int margemEsq = 20;
            int larguraCampo = 580;
            int alturaLinha = 30;
            int espacoEntreGrupos = 8;

            var paciente = _resultado.Paciente!;
            var agendamento = _resultado.Agendamento!;

            AdicionarCampo(panelCard, "Nome:", paciente.NomeCompleto.ToUpper(), margemEsq, ref y, larguraCampo, alturaLinha);
            AdicionarCampo(panelCard, "CPF:", FormatarCpf(paciente.CPF), margemEsq, ref y, larguraCampo, alturaLinha);
            AdicionarCampo(panelCard, "Nasc.:", paciente.DataNascimento.ToString("dd/MM/yyyy"), margemEsq, ref y, larguraCampo, alturaLinha);
            AdicionarCampo(panelCard, "Sexo:", paciente.Sexo == "F" ? "Feminino" : "Masculino", margemEsq, ref y, larguraCampo, alturaLinha);
            AdicionarCampo(panelCard, "Mãe:", paciente.NomeMae.ToUpper(), margemEsq, ref y, larguraCampo, alturaLinha);

            y += espacoEntreGrupos;
            var separador = new Panel
            {
                BackColor = UIHelper.CorCinzaBorda,
                Bounds = new Rectangle(margemEsq, y, larguraCampo, 1)
            };
            panelCard.Controls.Add(separador);
            y += espacoEntreGrupos + 4;

            AdicionarCampo(panelCard, "Convênio:", paciente.Convenio, margemEsq, ref y, larguraCampo, alturaLinha);
            AdicionarCampo(panelCard, "Especialidade:", agendamento.Especialidade, margemEsq, ref y, larguraCampo, alturaLinha);
            AdicionarCampo(panelCard, "Médico:", paciente.MedicoResponsavel, margemEsq, ref y, larguraCampo, alturaLinha);
            AdicionarCampo(panelCard, "Setor:", agendamento.Setor, margemEsq, ref y, larguraCampo, alturaLinha);
            AdicionarCampo(panelCard, "Atend.:", agendamento.NumeroSequencia, margemEsq, ref y, larguraCampo, alturaLinha);
            AdicionarCampo(panelCard, "Horário:", agendamento.DataHora.ToString("dd/MM/yyyy HH:mm"), margemEsq, ref y, larguraCampo, alturaLinha);
            AdicionarCampo(panelCard, "Status:", agendamento.Status, margemEsq, ref y, larguraCampo, alturaLinha);

            panelCard.Controls.Add(panelTituloPaciente);

            var btnDadosIncorretos = UIHelper.CriarBotaoPequeno("DADOS INCORRETOS", UIHelper.CorPrimaria,
                (s, e) => ProcessarDadosIncorretos());
            btnDadosIncorretos.SetBounds(0, 460, 280, 55);
            btnDadosIncorretos.Width = 290;

            var btnConfirmar = UIHelper.CriarBotaoPequeno("CONFIRMAR ✓", UIHelper.CorBotaoVerde,
                (s, e) => ProcessarConfirmacao());
            btnConfirmar.SetBounds(310, 460, 280, 55);
            btnConfirmar.Width = 310;
            btnConfirmar.Height = 55;

            panelCard.Controls.Add(btnDadosIncorretos);
            panelCard.Controls.Add(btnConfirmar);

            panelConteudo.Controls.Add(panelCard);
            panelConteudo.Resize += (s, e) =>
            {
                panelCard.Location = new Point(
                    (panelConteudo.ClientSize.Width - panelCard.Width) / 2,
                    (panelConteudo.ClientSize.Height - panelCard.Height) / 2);
            };

            Controls.Add(panelConteudo);
        }

        private static void AdicionarCampo(Panel parent, string label, string valor,
            int x, ref int y, int largura, int altura)
        {
            var lblLabel = new Label
            {
                Text = label,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.Gray,
                BackColor = Color.Transparent,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Bounds = new Rectangle(x, y, 130, altura)
            };

            var lblValor = new Label
            {
                Text = valor,
                Font = new Font("Segoe UI", 11),
                ForeColor = UIHelper.CorTexto,
                BackColor = Color.Transparent,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Bounds = new Rectangle(x + 135, y, largura - 140, altura)
            };

            parent.Controls.Add(lblLabel);
            parent.Controls.Add(lblValor);
            y += altura + 2;
        }

        private void ProcessarDadosIncorretos()
        {
            MessageBox.Show(
                "Por favor, dirija-se ao guichê de atendimento para atualizar seus dados.\n\nUm atendente irá realizar a correção no sistema Tasy.",
                "Dados Incorretos", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Close();
        }

        private async void ProcessarConfirmacao()
        {
            var loading = new FormLoading("Confirmando atendimento no sistema Tasy...");
            loading.Show(this);

            var apiService = new Services.ApiService();
            await apiService.ConfirmarAtendimentoAsync(_resultado.NumeroAtendimento);

            loading.Close();

            var impressao = new Printing.ImpressaoService();

            bool pulseiraOk = impressao.ImprimirPulseiraZebra(_resultado.Paciente!);
            bool comprovanteOk = impressao.ImprimirComprovanteTermico(_resultado.Paciente!);

            if (!pulseiraOk || !comprovanteOk)
            {
                MessageBox.Show(
                    "Atenção: houve um problema ao imprimir.\nDirija-se ao guichê para obter a pulseira.",
                    "Aviso de Impressão", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            var formConclusao = new FormConclusao(_resultado.Paciente!);
            formConclusao.ShowDialog(this);
            Close();
        }

        private void ConfigurarTimer()
        {
            _timerInatividade.Interval = Config.Configuracoes.TimeoutTela * 1000;
            _timerInatividade.Tick += (s, e) => { _timerInatividade.Stop(); Close(); };
            _timerInatividade.Start();
        }

        private static string FormatarCpf(string cpf)
        {
            if (cpf.Length == 11)
                return $"{cpf[..3]}.{cpf[3..6]}.{cpf[6..9]}-{cpf[9..]}";
            return cpf;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _timerInatividade.Stop();
            base.OnFormClosing(e);
        }
    }
}
