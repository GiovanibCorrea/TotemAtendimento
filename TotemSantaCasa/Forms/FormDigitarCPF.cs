using TotemSantaCasa.Models;

namespace TotemSantaCasa.Forms
{
    public class FormDigitarCPF : Form
    {
        private readonly TipoAtendimento _tipoAtendimento;
        private string _cpfDigitado = string.Empty;
        private Label _lblDisplay = new();
        private System.Windows.Forms.Timer _timerInatividade = new();

        public FormDigitarCPF(TipoAtendimento tipo)
        {
            _tipoAtendimento = tipo;
            InicializarComponentes();
            ConfigurarTimer();
        }

        private void InicializarComponentes()
        {
            Text = "Identificação do Paciente";
            WindowState = FormWindowState.Maximized;
            FormBorderStyle = FormBorderStyle.None;
            BackColor = UIHelper.CorFundo;
            StartPosition = FormStartPosition.CenterScreen;

            string tituloPagina = _tipoAtendimento == TipoAtendimento.Consulta ? "CONSULTAS" : "EXAMES";
            var panelCabecalho = UIHelper.CriarCabecalho(tituloPagina, "Identificação do Paciente");
            Controls.Add(panelCabecalho);
            Controls.Add(UIHelper.CriarRodape());

            var panelConteudo = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = UIHelper.CorFundo
            };

            var panelCentral = new Panel
            {
                Width = 480,
                Height = 580,
                BackColor = UIHelper.CorFundo,
                Anchor = AnchorStyles.None
            };

            var lblInstrucao = UIHelper.CriarLabel("Digite o seu CPF:", 16, FontStyle.Regular,
                UIHelper.CorTexto, ContentAlignment.MiddleCenter);
            lblInstrucao.SetBounds(0, 0, 480, 40);

            var lblPaciente = UIHelper.CriarLabel("PACIENTE", 11, FontStyle.Regular,
                Color.Gray, ContentAlignment.MiddleCenter);
            lblPaciente.SetBounds(0, 50, 480, 25);

            _lblDisplay = new Label
            {
                Font = new Font("Courier New", 24, FontStyle.Bold),
                ForeColor = UIHelper.CorTexto,
                BackColor = UIHelper.CorCinzaClaro,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                BorderStyle = BorderStyle.FixedSingle,
                Bounds = new Rectangle(40, 78, 400, 60),
                Text = "___.___.___-__"
            };

            var panelTeclado = CriarTeclado();
            panelTeclado.Location = new Point(40, 155);

            var btnVoltar = UIHelper.CriarBotaoPequeno("← VOLTAR", Color.Gray, (s, e) => Close());
            btnVoltar.SetBounds(40, 530, 180, 50);

            var btnConfirmar = UIHelper.CriarBotaoPequeno("CONFIRMAR →", UIHelper.CorBotaoVerde,
                (s, e) => ProcessarCPF());
            btnConfirmar.SetBounds(260, 530, 180, 50);

            panelCentral.Controls.Add(lblInstrucao);
            panelCentral.Controls.Add(lblPaciente);
            panelCentral.Controls.Add(_lblDisplay);
            panelCentral.Controls.Add(panelTeclado);
            panelCentral.Controls.Add(btnVoltar);
            panelCentral.Controls.Add(btnConfirmar);

            panelConteudo.Controls.Add(panelCentral);
            panelConteudo.Resize += (s, e) =>
            {
                panelCentral.Location = new Point(
                    (panelConteudo.ClientSize.Width - panelCentral.Width) / 2,
                    (panelConteudo.ClientSize.Height - panelCentral.Height) / 2);
            };

            Controls.Add(panelConteudo);
        }

        private Panel CriarTeclado()
        {
            var panel = new Panel { Width = 400, Height = 370 };
            string[,] teclas = {
                { "7", "8", "9" },
                { "4", "5", "6" },
                { "1", "2", "3" },
                { "Limpar", "0", "⌫" }
            };

            int larguraBotao = 120;
            int alturaBotao = 80;
            int espacamento = 10;

            for (int linha = 0; linha < 4; linha++)
            {
                for (int col = 0; col < 3; col++)
                {
                    string valor = teclas[linha, col];
                    Color cor = valor == "Limpar"
                        ? Color.FromArgb(100, 100, 100)
                        : valor == "⌫"
                            ? Color.FromArgb(180, 100, 0)
                            : UIHelper.CorPrimaria;

                    var btn = new Button
                    {
                        Text = valor,
                        Font = valor == "Limpar"
                            ? new Font("Segoe UI", 12, FontStyle.Bold)
                            : new Font("Segoe UI", 20, FontStyle.Bold),
                        ForeColor = Color.White,
                        BackColor = cor,
                        FlatStyle = FlatStyle.Flat,
                        Cursor = Cursors.Hand,
                        Bounds = new Rectangle(
                            col * (larguraBotao + espacamento),
                            linha * (alturaBotao + espacamento),
                            larguraBotao, alturaBotao)
                    };
                    btn.FlatAppearance.BorderSize = 0;

                    string tecla = valor;
                    btn.Click += (s, e) => ProcessarTecla(tecla);

                    panel.Controls.Add(btn);
                }
            }

            return panel;
        }

        private void ProcessarTecla(string tecla)
        {
            _timerInatividade.Stop();
            _timerInatividade.Start();

            if (tecla == "Limpar")
            {
                _cpfDigitado = string.Empty;
            }
            else if (tecla == "⌫")
            {
                if (_cpfDigitado.Length > 0)
                    _cpfDigitado = _cpfDigitado[..^1];
            }
            else if (_cpfDigitado.Length < 11)
            {
                _cpfDigitado += tecla;
            }

            AtualizarDisplay();
        }

        private void AtualizarDisplay()
        {
            string display = _cpfDigitado.PadRight(11, '_');
            _lblDisplay.Text = $"{display[..3]}.{display[3..6]}.{display[6..9]}-{display[9..11]}";
        }

        private async void ProcessarCPF()
        {
            if (_cpfDigitado.Length < 11)
            {
                MessageBox.Show("Por favor, digite o CPF completo (11 dígitos).",
                    "CPF Incompleto", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var loading = new FormLoading("Consultando seu cadastro...");
            loading.Show(this);

            var apiService = new Services.ApiService();
            var resultado = await apiService.RealizarCheckinAsync(_cpfDigitado, _tipoAtendimento);

            loading.Close();

            if (!resultado.Sucesso || resultado.Paciente == null)
            {
                var formNaoEncontrado = new FormCPFNaoEncontrado();
                formNaoEncontrado.ShowDialog(this);
                Close();
                return;
            }

            var formConfirmacao = new FormConfirmarDados(resultado);
            formConfirmacao.ShowDialog(this);
            Close();
        }

        private void ConfigurarTimer()
        {
            _timerInatividade.Interval = Config.Configuracoes.TimeoutTela * 1000;
            _timerInatividade.Tick += (s, e) =>
            {
                _timerInatividade.Stop();
                Close();
            };
            _timerInatividade.Start();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _timerInatividade.Stop();
            base.OnFormClosing(e);
        }
    }
}
