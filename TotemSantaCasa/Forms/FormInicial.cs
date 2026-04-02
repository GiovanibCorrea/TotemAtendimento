using TotemSantaCasa.Models;

namespace TotemSantaCasa.Forms
{
    public class FormInicial : Form
    {
        private System.Windows.Forms.Timer _timerReload = new();
        private System.Windows.Forms.Timer _timerRelogio = new();
        private Label _lblHora = new();

        public FormInicial()
        {
            InicializarComponentes();
            ConfigurarTimers();
        }

        private void InicializarComponentes()
        {
            Text = "Totem - Santa Casa de Curitiba";
            WindowState = FormWindowState.Maximized;
            FormBorderStyle = FormBorderStyle.None;
            BackColor = UIHelper.CorFundo;
            StartPosition = FormStartPosition.CenterScreen;

            var panelCabecalho = new Panel
            {
                BackColor = UIHelper.CorPrimaria,
                Dock = DockStyle.Top,
                Height = 120
            };

            var lblBemVindo = new Label
            {
                Text = "BEM-VINDO",
                Font = new Font("Segoe UI", 14, FontStyle.Regular),
                ForeColor = Color.FromArgb(255, 200, 200),
                BackColor = Color.Transparent,
                AutoSize = false,
                TextAlign = ContentAlignment.BottomCenter,
                Dock = DockStyle.None,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            var lblInstituto = new Label
            {
                Text = "Santa Casa de Curitiba",
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.None,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            _lblHora = new Label
            {
                Text = DateTime.Now.ToString("HH:mm"),
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleRight,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Width = 120,
                Height = 50
            };

            panelCabecalho.Controls.Add(lblBemVindo);
            panelCabecalho.Controls.Add(lblInstituto);
            panelCabecalho.Controls.Add(_lblHora);

            panelCabecalho.Resize += (s, e) =>
            {
                lblBemVindo.SetBounds(0, 12, panelCabecalho.Width, 25);
                lblInstituto.SetBounds(0, 38, panelCabecalho.Width, 50);
                _lblHora.SetBounds(panelCabecalho.Width - 130, 10, 120, 40);
            };

            var panelConteudo = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = UIHelper.CorFundo,
                Padding = new Padding(40)
            };

            var lblInstrucao = UIHelper.CriarLabel(
                "Selecione uma opção para iniciar seu atendimento:",
                16, FontStyle.Regular, UIHelper.CorTexto, ContentAlignment.MiddleCenter);
            lblInstrucao.Dock = DockStyle.None;
            lblInstrucao.Height = 50;
            lblInstrucao.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            var panelBotoes = new TableLayoutPanel
            {
                RowCount = 1,
                ColumnCount = 3,
                Anchor = AnchorStyles.None,
                AutoSize = true
            };
            panelBotoes.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            panelBotoes.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            panelBotoes.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            var btnConsultas = CriarBotaoMenu("🩺", "CONSULTAS", UIHelper.CorPrimaria,
                (s, e) => AbrirTelaAtendimento(TipoAtendimento.Consulta));

            var btnExames = CriarBotaoMenu("🔬", "EXAMES", Color.FromArgb(180, 30, 30),
                (s, e) => AbrirTelaAtendimento(TipoAtendimento.Exame));

            var btnSenhas = CriarBotaoMenu("🎟️", "SENHAS", Color.FromArgb(150, 25, 25),
                (s, e) => AbrirTelaSenhas());

            panelBotoes.Controls.Add(btnConsultas, 0, 0);
            panelBotoes.Controls.Add(btnExames, 1, 0);
            panelBotoes.Controls.Add(btnSenhas, 2, 0);
            panelBotoes.SetColumnSpan(btnConsultas, 1);
            panelBotoes.Margin = new Padding(20);

            var lblAmbulatorio = UIHelper.CriarLabel(
                $"Ambulatório {Config.Configuracoes.Ambulatorio}",
                12, FontStyle.Regular, Color.Gray, ContentAlignment.MiddleCenter);
            lblAmbulatorio.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblAmbulatorio.Height = 30;

            panelConteudo.Controls.Add(lblInstrucao);
            panelConteudo.Controls.Add(panelBotoes);
            panelConteudo.Controls.Add(lblAmbulatorio);

            panelConteudo.Resize += (s, e) =>
            {
                lblInstrucao.SetBounds(0, 30, panelConteudo.ClientSize.Width, 50);
                panelBotoes.Location = new Point(
                    (panelConteudo.ClientSize.Width - panelBotoes.Width) / 2,
                    (panelConteudo.ClientSize.Height - panelBotoes.Height) / 2 - 20);
                lblAmbulatorio.SetBounds(0, panelConteudo.ClientSize.Height - 40,
                    panelConteudo.ClientSize.Width, 30);
            };

            Controls.Add(panelConteudo);
            Controls.Add(panelCabecalho);
        }

        private static Panel CriarBotaoMenu(string icone, string texto, Color cor, EventHandler onClick)
        {
            var panel = new Panel
            {
                Width = 280,
                Height = 260,
                BackColor = cor,
                Margin = new Padding(15),
                Cursor = Cursors.Hand
            };

            var lblIcone = new Label
            {
                Text = icone,
                Font = new Font("Segoe UI Emoji", 52),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Bounds = new Rectangle(0, 30, 280, 120)
            };

            var lblTexto = new Label
            {
                Text = texto,
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Bounds = new Rectangle(0, 155, 280, 60)
            };

            var lblSubTexto = new Label
            {
                Text = "Toque para selecionar",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(200, 255, 255, 255),
                BackColor = Color.Transparent,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Bounds = new Rectangle(0, 215, 280, 35)
            };

            panel.Controls.Add(lblIcone);
            panel.Controls.Add(lblTexto);
            panel.Controls.Add(lblSubTexto);

            panel.Click += onClick;
            lblIcone.Click += onClick;
            lblTexto.Click += onClick;
            lblSubTexto.Click += onClick;

            panel.MouseEnter += (s, e) => panel.BackColor = ControlPaint.Light(cor, 0.15f);
            panel.MouseLeave += (s, e) => panel.BackColor = cor;
            foreach (Control c in panel.Controls)
            {
                c.MouseEnter += (s, e) => panel.BackColor = ControlPaint.Light(cor, 0.15f);
                c.MouseLeave += (s, e) => panel.BackColor = cor;
            }

            return panel;
        }

        private void AbrirTelaAtendimento(TipoAtendimento tipo)
        {
            var form = new FormDigitarCPF(tipo);
            form.ShowDialog(this);
        }

        private void AbrirTelaSenhas()
        {
            var form = new FormSenhas();
            form.ShowDialog(this);
        }

        private void ConfigurarTimers()
        {
            _timerRelogio.Interval = 1000;
            _timerRelogio.Tick += (s, e) => _lblHora.Text = DateTime.Now.ToString("HH:mm");
            _timerRelogio.Start();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _timerReload.Stop();
            _timerRelogio.Stop();
            base.OnFormClosing(e);
        }
    }
}
