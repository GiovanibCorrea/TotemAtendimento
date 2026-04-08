using TotemSantaCasa.Services;

namespace TotemSantaCasa.Forms
{
    public class FormBloqueioLicenca : Form
    {
        private readonly LicencaService _licencaService;
        private string _chaveDigitada = string.Empty;
        private Label _lblDisplay = new();
        private Label _lblMensagem = new();
        private Panel _panelTeclado = new();
        private Panel _panelAtivacao = new();
        private bool _modoAtivacao = false;

        public bool Ativado { get; private set; }

        public FormBloqueioLicenca(LicencaService licencaService, string mensagemErro = "")
        {
            _licencaService = licencaService;
            InicializarComponentes(mensagemErro);
        }

        private void InicializarComponentes(string mensagemErro)
        {
            Text = "Ativação Necessária";
            WindowState = FormWindowState.Maximized;
            FormBorderStyle = FormBorderStyle.None;
            BackColor = Color.FromArgb(20, 20, 20);
            StartPosition = FormStartPosition.CenterScreen;

            var panelCentral = new Panel
            {
                Width = 600,
                Height = 680,
                BackColor = Color.FromArgb(30, 30, 30),
                Anchor = AnchorStyles.None
            };

            var panelTopo = new Panel
            {
                BackColor = UITheme.Primaria,
                Bounds = new Rectangle(0, 0, 600, 8)
            };

            var lblIcone = new Label
            {
                Text = "🔒",
                Font = new Font("Segoe UI Emoji", 52),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Bounds = new Rectangle(0, 20, 600, 85)
            };

            var lblTitulo = new Label
            {
                Text = "Sistema não licenciado",
                Font = new Font(UITheme.FamiliaFonte, 20, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Bounds = new Rectangle(20, 110, 560, 40)
            };

            _lblMensagem = new Label
            {
                Text = string.IsNullOrEmpty(mensagemErro)
                    ? "Este totem não está ativado.\nContate a Digitaly Technology para obter sua licença."
                    : mensagemErro,
                Font = new Font(UITheme.FamiliaFonte, 12),
                ForeColor = Color.FromArgb(180, 180, 180),
                BackColor = Color.Transparent,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Bounds = new Rectangle(20, 155, 560, 60)
            };

            var lblInfoHardware = new Label
            {
                Text = $"ID do Totem: {HardwareService.ObterTotemId()}\nFingerprint: {HardwareService.ObterFingerprint()}",
                Font = new Font("Courier New", 10),
                ForeColor = Color.FromArgb(100, 180, 100),
                BackColor = Color.FromArgb(15, 15, 15),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Bounds = new Rectangle(20, 220, 560, 55),
                Padding = new Padding(10)
            };

            var btnAtivarManual = new Button
            {
                Text = "🔑  Inserir Chave de Ativação",
                Font = new Font(UITheme.FamiliaFonte, 13, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = UITheme.Primaria,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Bounds = new Rectangle(100, 285, 400, 55)
            };
            btnAtivarManual.FlatAppearance.BorderSize = 0;
            btnAtivarManual.Click += (s, e) => AlternarModoAtivacao(panelCentral);

            _panelAtivacao = CriarPainelAtivacao();
            _panelAtivacao.Location = new Point(20, 350);
            _panelAtivacao.Visible = false;

            var lblContato = new Label
            {
                Text = "contato@digitalytechnology.com.br  •  (41) 0000-0000",
                Font = new Font(UITheme.FamiliaFonte, 10),
                ForeColor = Color.FromArgb(100, 100, 100),
                BackColor = Color.Transparent,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Bounds = new Rectangle(20, 635, 560, 30)
            };

            panelCentral.Controls.Add(panelTopo);
            panelCentral.Controls.Add(lblIcone);
            panelCentral.Controls.Add(lblTitulo);
            panelCentral.Controls.Add(_lblMensagem);
            panelCentral.Controls.Add(lblInfoHardware);
            panelCentral.Controls.Add(btnAtivarManual);
            panelCentral.Controls.Add(_panelAtivacao);
            panelCentral.Controls.Add(lblContato);

            Controls.Add(panelCentral);
            Resize += (s, e) =>
            {
                panelCentral.Location = new Point(
                    (ClientSize.Width - panelCentral.Width) / 2,
                    (ClientSize.Height - panelCentral.Height) / 2);
            };
        }

        private Panel CriarPainelAtivacao()
        {
            var panel = new Panel { Width = 560, Height = 310, BackColor = Color.Transparent };

            var lblInstrucao = new Label
            {
                Text = "Digite a chave de ativação (formato: XXXXX-XXXXX-XXXXX-XXXXX):",
                Font = new Font(UITheme.FamiliaFonte, 11),
                ForeColor = Color.FromArgb(200, 200, 200),
                BackColor = Color.Transparent,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Bounds = new Rectangle(0, 0, 560, 35)
            };

            _lblDisplay = new Label
            {
                Font = new Font("Courier New", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 220, 100),
                BackColor = Color.FromArgb(15, 15, 15),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Bounds = new Rectangle(0, 40, 560, 55),
                Text = "_ _ _ _ _  -  _ _ _ _ _  -  _ _ _ _ _  -  _ _ _ _ _"
            };

            _panelTeclado = CriarTecladoHex();
            _panelTeclado.Location = new Point(0, 105);

            var btnConfirmar = new Button
            {
                Text = "✓  ATIVAR",
                Font = new Font(UITheme.FamiliaFonte, 14, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = UITheme.BotaoConfirmar,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Bounds = new Rectangle(310, 265, 250, 45)
            };
            btnConfirmar.FlatAppearance.BorderSize = 0;
            btnConfirmar.Click += (s, e) => ProcessarAtivacao();

            var btnLimpar = new Button
            {
                Text = "Limpar",
                Font = new Font(UITheme.FamiliaFonte, 12),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(80, 80, 80),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Bounds = new Rectangle(0, 265, 150, 45)
            };
            btnLimpar.FlatAppearance.BorderSize = 0;
            btnLimpar.Click += (s, e) => { _chaveDigitada = string.Empty; AtualizarDisplay(); };

            panel.Controls.Add(lblInstrucao);
            panel.Controls.Add(_lblDisplay);
            panel.Controls.Add(_panelTeclado);
            panel.Controls.Add(btnConfirmar);
            panel.Controls.Add(btnLimpar);

            return panel;
        }

        private Panel CriarTecladoHex()
        {
            var panel = new Panel { Width = 560, Height = 155 };

            string[] teclas = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F", "⌫", "-" };
            int col = 0, row = 0;
            int btnW = 60, btnH = 42, gap = 4;

            foreach (var tecla in teclas)
            {
                var btn = new Button
                {
                    Text = tecla,
                    Font = new Font("Courier New", 13, FontStyle.Bold),
                    ForeColor = Color.White,
                    BackColor = tecla == "⌫" ? Color.FromArgb(150, 80, 0) : Color.FromArgb(60, 60, 60),
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand,
                    Bounds = new Rectangle(col * (btnW + gap), row * (btnH + gap), btnW, btnH)
                };
                btn.FlatAppearance.BorderSize = 0;
                string t = tecla;
                btn.Click += (s, e) => ProcessarTecla(t);
                panel.Controls.Add(btn);

                col++;
                if (col >= 9) { col = 0; row++; }
            }

            return panel;
        }

        private void ProcessarTecla(string tecla)
        {
            string chaveSemMascara = _chaveDigitada.Replace("-", "");
            if (tecla == "⌫")
            {
                if (chaveSemMascara.Length > 0)
                    chaveSemMascara = chaveSemMascara[..^1];
            }
            else if (tecla == "-")
            {
            }
            else if (chaveSemMascara.Length < 20)
            {
                chaveSemMascara += tecla;
            }

            _chaveDigitada = chaveSemMascara;
            AtualizarDisplay();
        }

        private void AtualizarDisplay()
        {
            string s = _chaveDigitada.PadRight(20, '_');
            _lblDisplay.Text = $"{s[..5]}-{s[5..10]}-{s[10..15]}-{s[15..20]}";
        }

        private void ProcessarAtivacao()
        {
            if (_chaveDigitada.Length < 20)
            {
                _lblMensagem.ForeColor = Color.FromArgb(255, 100, 100);
                _lblMensagem.Text = "Chave incompleta. A chave deve ter 20 caracteres.";
                return;
            }

            string chaveFormatada = $"{_chaveDigitada[..5]}-{_chaveDigitada[5..10]}-{_chaveDigitada[10..15]}-{_chaveDigitada[15..20]}";
            var resultado = _licencaService.ValidarChaveManual(chaveFormatada);

            if (resultado.Valida)
            {
                _lblMensagem.ForeColor = Color.FromArgb(100, 220, 100);
                _lblMensagem.Text = "✓ " + resultado.Mensagem;
                Ativado = true;
                Task.Delay(1500).ContinueWith(_ => Invoke(Close));
            }
            else
            {
                _lblMensagem.ForeColor = Color.FromArgb(255, 100, 100);
                _lblMensagem.Text = "✗ " + resultado.Mensagem;
                _chaveDigitada = string.Empty;
                AtualizarDisplay();
            }
        }

        private void AlternarModoAtivacao(Panel panelCentral)
        {
            _modoAtivacao = !_modoAtivacao;
            _panelAtivacao.Visible = _modoAtivacao;
            panelCentral.Height = _modoAtivacao ? 700 : 680;
            Refresh();
        }
    }
}
