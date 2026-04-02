using TotemSantaCasa.Models;

namespace TotemSantaCasa.Forms
{
    public class FormConfirmarTelefone : Form
    {
        private readonly ResultadoCheckin _resultado;
        private string _telefoneDigitado = string.Empty;
        private Label _lblDisplay = new();
        private System.Windows.Forms.Timer _timerInatividade = new();

        public bool Confirmado { get; private set; }

        public FormConfirmarTelefone(ResultadoCheckin resultado)
        {
            _resultado = resultado;
            InicializarComponentes();
            ConfigurarTimer();
        }

        private void InicializarComponentes()
        {
            Text = "Confirmar Telefone";
            WindowState = FormWindowState.Maximized;
            FormBorderStyle = FormBorderStyle.None;
            BackColor = UIHelper.CorFundo;
            StartPosition = FormStartPosition.CenterScreen;

            var panelCabecalho = UIHelper.CriarCabecalho("CONFIRME O NÚMERO DE TELEFONE", "");
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
                Height = 540,
                Anchor = AnchorStyles.None
            };

            string telefoneMascarado = MascararTelefone(_resultado.Paciente?.Telefone ?? "");

            var lblInstrucao = UIHelper.CriarLabel(
                "Confirme o número de telefone cadastrado:", 14,
                FontStyle.Regular, UIHelper.CorTexto, ContentAlignment.MiddleCenter);
            lblInstrucao.SetBounds(0, 0, 480, 40);

            var lblTelCadastrado = UIHelper.CriarLabel(
                telefoneMascarado, 26, FontStyle.Bold,
                UIHelper.CorPrimaria, ContentAlignment.MiddleCenter);
            lblTelCadastrado.SetBounds(0, 45, 480, 50);

            var lblDigite = UIHelper.CriarLabel(
                "Digite os 4 últimos dígitos:", 13,
                FontStyle.Regular, Color.Gray, ContentAlignment.MiddleCenter);
            lblDigite.SetBounds(0, 105, 480, 30);

            _lblDisplay = new Label
            {
                Font = new Font("Courier New", 28, FontStyle.Bold),
                ForeColor = UIHelper.CorTexto,
                BackColor = UIHelper.CorCinzaClaro,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                BorderStyle = BorderStyle.FixedSingle,
                Bounds = new Rectangle(140, 140, 200, 65),
                Text = "_ _ _ _"
            };

            var panelTeclado = CriarTeclado();
            panelTeclado.Location = new Point(40, 225);

            var btnVoltar = UIHelper.CriarBotaoPequeno("← VOLTAR", Color.Gray, (s, e) => Close());
            btnVoltar.SetBounds(40, 500, 180, 50);

            var btnConfirmar = UIHelper.CriarBotaoPequeno("CONFIRMAR →", UIHelper.CorBotaoVerde,
                (s, e) => ProcessarConfirmacao());
            btnConfirmar.SetBounds(260, 500, 180, 50);

            panelCentral.Controls.Add(lblInstrucao);
            panelCentral.Controls.Add(lblTelCadastrado);
            panelCentral.Controls.Add(lblDigite);
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
            var panel = new Panel { Width = 400, Height = 270 };
            string[,] teclas = {
                { "1", "2", "3" },
                { "4", "5", "6" },
                { "7", "8", "9" },
                { "Limpar", "0", "⌫" }
            };

            int larguraBotao = 120;
            int alturaBotao = 58;
            int espacamento = 8;

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
                            ? new Font("Segoe UI", 11, FontStyle.Bold)
                            : new Font("Segoe UI", 18, FontStyle.Bold),
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
            if (tecla == "Limpar") _telefoneDigitado = string.Empty;
            else if (tecla == "⌫" && _telefoneDigitado.Length > 0)
                _telefoneDigitado = _telefoneDigitado[..^1];
            else if (_telefoneDigitado.Length < 4 && tecla != "Limpar" && tecla != "⌫")
                _telefoneDigitado += tecla;

            AtualizarDisplay();
        }

        private void AtualizarDisplay()
        {
            string d = _telefoneDigitado.PadRight(4, '_');
            _lblDisplay.Text = $"{d[0]} {d[1]} {d[2]} {d[3]}";
        }

        private void ProcessarConfirmacao()
        {
            if (_telefoneDigitado.Length < 4)
            {
                MessageBox.Show("Digite os 4 últimos dígitos do telefone.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string telefoneReal = (_resultado.Paciente?.Telefone ?? "").Replace(" ", "")
                .Replace("-", "").Replace("(", "").Replace(")", "");

            if (telefoneReal.EndsWith(_telefoneDigitado))
            {
                Confirmado = true;
                Close();
            }
            else
            {
                MessageBox.Show("Os dígitos não conferem. Tente novamente ou dirija-se ao guichê.",
                    "Verificação Falhou", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _telefoneDigitado = string.Empty;
                AtualizarDisplay();
            }
        }

        private static string MascararTelefone(string telefone)
        {
            if (string.IsNullOrEmpty(telefone)) return "(__)  _____-____";
            if (telefone.Length >= 10)
            {
                int len = telefone.Length;
                return telefone[..(len - 4)] + "****";
            }
            return telefone;
        }

        private void ConfigurarTimer()
        {
            _timerInatividade.Interval = Config.Configuracoes.TimeoutTela * 1000;
            _timerInatividade.Tick += (s, e) => { _timerInatividade.Stop(); Close(); };
            _timerInatividade.Start();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _timerInatividade.Stop();
            base.OnFormClosing(e);
        }
    }
}
