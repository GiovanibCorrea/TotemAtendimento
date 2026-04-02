namespace TotemSantaCasa.Forms
{
    public class FormCPFNaoEncontrado : Form
    {
        private System.Windows.Forms.Timer _timerAutoFechar = new();

        public FormCPFNaoEncontrado()
        {
            InicializarComponentes();
            ConfigurarTimer();
        }

        private void InicializarComponentes()
        {
            Text = "CPF Não Localizado";
            WindowState = FormWindowState.Maximized;
            FormBorderStyle = FormBorderStyle.None;
            BackColor = UIHelper.CorFundo;
            StartPosition = FormStartPosition.CenterScreen;

            var panelCabecalho = UIHelper.CriarCabecalho("ATENÇÃO!", "");
            Controls.Add(panelCabecalho);
            Controls.Add(UIHelper.CriarRodape());

            var panelConteudo = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = UIHelper.CorFundo
            };

            var panelAlerta = new Panel
            {
                Width = 560,
                Height = 360,
                BackColor = Color.FromArgb(255, 245, 245),
                Anchor = AnchorStyles.None
            };

            var painelBorda = new Panel
            {
                BackColor = UIHelper.CorPrimaria,
                Bounds = new Rectangle(0, 0, 560, 8)
            };

            var lblIcone = new Label
            {
                Text = "⚠️",
                Font = new Font("Segoe UI Emoji", 52),
                BackColor = Color.Transparent,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Bounds = new Rectangle(0, 20, 560, 90)
            };

            var lblTitulo = UIHelper.CriarLabel(
                "Nenhum agendamento encontrado", 18, FontStyle.Bold,
                UIHelper.CorPrimaria, ContentAlignment.MiddleCenter);
            lblTitulo.SetBounds(30, 115, 500, 35);

            var lblMensagem = UIHelper.CriarLabel(
                "Não encontramos agendamento para este CPF.\nPor favor, dirija-se ao guichê de atendimento.",
                14, FontStyle.Regular, UIHelper.CorTexto, ContentAlignment.MiddleCenter);
            lblMensagem.SetBounds(30, 155, 500, 65);

            var lblInstrucao = UIHelper.CriarLabel(
                "Selecione a opção SENHAS na tela inicial\npara retirar uma senha de atendimento.",
                13, FontStyle.Regular, Color.Gray, ContentAlignment.MiddleCenter);
            lblInstrucao.SetBounds(30, 230, 500, 55);

            var lblContador = UIHelper.CriarLabel(
                "Retornando ao menu em 10 segundos...", 11, FontStyle.Regular,
                Color.Gray, ContentAlignment.MiddleCenter);
            lblContador.Name = "lblContador";
            lblContador.SetBounds(30, 300, 500, 28);

            panelAlerta.Controls.Add(painelBorda);
            panelAlerta.Controls.Add(lblIcone);
            panelAlerta.Controls.Add(lblTitulo);
            panelAlerta.Controls.Add(lblMensagem);
            panelAlerta.Controls.Add(lblInstrucao);
            panelAlerta.Controls.Add(lblContador);

            var btnOk = UIHelper.CriarBotaoGrande("ENTENDIDO", UIHelper.CorPrimaria, (s, e) => Close());
            btnOk.Width = 280;
            btnOk.Height = 65;
            btnOk.Anchor = AnchorStyles.None;

            panelConteudo.Controls.Add(panelAlerta);
            panelConteudo.Controls.Add(btnOk);
            panelConteudo.Resize += (s, e) =>
            {
                panelAlerta.Location = new Point(
                    (panelConteudo.ClientSize.Width - panelAlerta.Width) / 2,
                    (panelConteudo.ClientSize.Height - panelAlerta.Height) / 2 - 50);
                btnOk.Location = new Point(
                    (panelConteudo.ClientSize.Width - btnOk.Width) / 2,
                    panelAlerta.Bottom + 25);
            };

            Controls.Add(panelConteudo);
        }

        private void ConfigurarTimer()
        {
            int segundosRestantes = 10;
            _timerAutoFechar.Interval = 1000;
            _timerAutoFechar.Tick += (s, e) =>
            {
                segundosRestantes--;
                var lbl = Controls.Find("lblContador", true).FirstOrDefault() as Label;
                if (lbl != null)
                    lbl.Text = $"Retornando ao menu em {segundosRestantes} segundos...";

                if (segundosRestantes <= 0)
                {
                    _timerAutoFechar.Stop();
                    Close();
                }
            };
            _timerAutoFechar.Start();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _timerAutoFechar.Stop();
            base.OnFormClosing(e);
        }
    }
}
