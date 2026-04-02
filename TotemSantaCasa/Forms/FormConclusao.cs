using TotemSantaCasa.Models;

namespace TotemSantaCasa.Forms
{
    public class FormLoading : Form
    {
        public FormLoading(string mensagem = "Processando...")
        {
            Text = "Aguarde";
            WindowState = FormWindowState.Maximized;
            FormBorderStyle = FormBorderStyle.None;
            BackColor = Color.FromArgb(240, 240, 240);
            StartPosition = FormStartPosition.CenterScreen;

            var panel = new Panel
            {
                Width = 420,
                Height = 180,
                BackColor = Color.White,
                Anchor = AnchorStyles.None
            };

            var lblIcone = new Label
            {
                Text = "⏳",
                Font = new Font("Segoe UI Emoji", 40),
                BackColor = Color.Transparent,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Bounds = new Rectangle(0, 15, 420, 70)
            };

            var lblMensagem = UIHelper.CriarLabel(mensagem, 14, FontStyle.Regular,
                UIHelper.CorTexto, ContentAlignment.MiddleCenter);
            lblMensagem.SetBounds(20, 90, 380, 50);
            lblMensagem.BackColor = Color.Transparent;

            panel.Controls.Add(lblIcone);
            panel.Controls.Add(lblMensagem);

            Controls.Add(panel);
            Resize += (s, e) =>
            {
                panel.Location = new Point(
                    (ClientSize.Width - panel.Width) / 2,
                    (ClientSize.Height - panel.Height) / 2);
            };
        }
    }

    public class FormConclusao : Form
    {
        private System.Windows.Forms.Timer _timerAutoFechar = new();

        public FormConclusao(Paciente paciente)
        {
            InicializarComponentes(paciente);
            ConfigurarTimer();
        }

        private void InicializarComponentes(Paciente paciente)
        {
            Text = "Atendimento Confirmado";
            WindowState = FormWindowState.Maximized;
            FormBorderStyle = FormBorderStyle.None;
            BackColor = UIHelper.CorFundo;
            StartPosition = FormStartPosition.CenterScreen;

            var panelCabecalho = UIHelper.CriarCabecalho("CHECK-IN REALIZADO!", "");
            Controls.Add(panelCabecalho);
            Controls.Add(UIHelper.CriarRodape());

            var panelConteudo = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = UIHelper.CorFundo
            };

            var panelCard = new Panel
            {
                Width = 560,
                Height = 380,
                BackColor = Color.White,
                Anchor = AnchorStyles.None
            };
            panelCard.Paint += (s, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, panelCard.ClientRectangle,
                    UIHelper.CorBotaoVerde, ButtonBorderStyle.Solid);
            };

            var lblIcone = new Label
            {
                Text = "✅",
                Font = new Font("Segoe UI Emoji", 60),
                BackColor = Color.Transparent,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Bounds = new Rectangle(0, 15, 560, 100)
            };

            var lblTitulo = UIHelper.CriarLabel(
                "Atendimento confirmado com sucesso!", 17, FontStyle.Bold,
                UIHelper.CorBotaoVerde, ContentAlignment.MiddleCenter);
            lblTitulo.SetBounds(20, 120, 520, 40);
            lblTitulo.BackColor = Color.Transparent;

            var lblNome = UIHelper.CriarLabel(
                paciente.NomeCompleto, 15, FontStyle.Bold,
                UIHelper.CorTexto, ContentAlignment.MiddleCenter);
            lblNome.SetBounds(20, 165, 520, 35);
            lblNome.BackColor = Color.Transparent;

            var lblAtendimento = UIHelper.CriarLabel(
                $"Atendimento Nº {paciente.NumeroAtendimento}  •  {paciente.Setor}", 13,
                FontStyle.Regular, Color.Gray, ContentAlignment.MiddleCenter);
            lblAtendimento.SetBounds(20, 202, 520, 30);
            lblAtendimento.BackColor = Color.Transparent;

            var lblPulseira = UIHelper.CriarLabel(
                "🖨️  Retire sua pulseira de identificação.", 13,
                FontStyle.Regular, UIHelper.CorTexto, ContentAlignment.MiddleCenter);
            lblPulseira.SetBounds(20, 245, 520, 30);
            lblPulseira.BackColor = Color.Transparent;

            var lblAguarde = UIHelper.CriarLabel(
                "Aguarde o chamado para o atendimento médico.", 13,
                FontStyle.Regular, UIHelper.CorTexto, ContentAlignment.MiddleCenter);
            lblAguarde.SetBounds(20, 278, 520, 30);
            lblAguarde.BackColor = Color.Transparent;

            var lblContador = UIHelper.CriarLabel(
                "Retornando ao menu em 15 segundos...", 11,
                FontStyle.Regular, Color.Gray, ContentAlignment.MiddleCenter);
            lblContador.Name = "lblContador";
            lblContador.SetBounds(20, 330, 520, 28);
            lblContador.BackColor = Color.Transparent;

            panelCard.Controls.Add(lblIcone);
            panelCard.Controls.Add(lblTitulo);
            panelCard.Controls.Add(lblNome);
            panelCard.Controls.Add(lblAtendimento);
            panelCard.Controls.Add(lblPulseira);
            panelCard.Controls.Add(lblAguarde);
            panelCard.Controls.Add(lblContador);

            panelConteudo.Controls.Add(panelCard);
            panelConteudo.Resize += (s, e) =>
            {
                panelCard.Location = new Point(
                    (panelConteudo.ClientSize.Width - panelCard.Width) / 2,
                    (panelConteudo.ClientSize.Height - panelCard.Height) / 2);
            };

            Controls.Add(panelConteudo);
        }

        private void ConfigurarTimer()
        {
            int segundos = 15;
            _timerAutoFechar.Interval = 1000;
            _timerAutoFechar.Tick += (s, e) =>
            {
                segundos--;
                var lbl = Controls.Find("lblContador", true).FirstOrDefault() as Label;
                if (lbl != null)
                    lbl.Text = $"Retornando ao menu em {segundos} segundos...";
                if (segundos <= 0) { _timerAutoFechar.Stop(); Close(); }
            };
            _timerAutoFechar.Start();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _timerAutoFechar.Stop();
            base.OnFormClosing(e);
        }
    }

    public class FormConclusaoSenha : Form
    {
        private System.Windows.Forms.Timer _timerAutoFechar = new();

        public FormConclusaoSenha(string tipo, string numero)
        {
            InicializarComponentes(tipo, numero);
            ConfigurarTimer();
        }

        private void InicializarComponentes(string tipo, string numero)
        {
            Text = "Senha Emitida";
            WindowState = FormWindowState.Maximized;
            FormBorderStyle = FormBorderStyle.None;
            BackColor = UIHelper.CorFundo;
            StartPosition = FormStartPosition.CenterScreen;

            var panelCabecalho = UIHelper.CriarCabecalho("SENHA EMITIDA!", "");
            Controls.Add(panelCabecalho);
            Controls.Add(UIHelper.CriarRodape());

            var panelConteudo = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = UIHelper.CorFundo
            };

            var panelCard = new Panel
            {
                Width = 460,
                Height = 340,
                BackColor = Color.White,
                Anchor = AnchorStyles.None
            };

            var panelTopo = new Panel
            {
                BackColor = UIHelper.CorPrimaria,
                Bounds = new Rectangle(0, 0, 460, 55)
            };
            var lblTipoTop = UIHelper.CriarLabel(tipo.ToUpper(), 14, FontStyle.Bold,
                Color.White, ContentAlignment.MiddleCenter);
            lblTipoTop.Dock = DockStyle.Fill;
            panelTopo.Controls.Add(lblTipoTop);

            var lblNumero = new Label
            {
                Text = numero,
                Font = new Font("Segoe UI", 72, FontStyle.Bold),
                ForeColor = UIHelper.CorPrimaria,
                BackColor = Color.Transparent,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Bounds = new Rectangle(0, 55, 460, 140)
            };

            var lblAguarde = UIHelper.CriarLabel(
                "Retire o comprovante e aguarde o chamado.", 13,
                FontStyle.Regular, UIHelper.CorTexto, ContentAlignment.MiddleCenter);
            lblAguarde.SetBounds(20, 200, 420, 30);
            lblAguarde.BackColor = Color.Transparent;

            var lblContador = UIHelper.CriarLabel("Retornando em 10 segundos...", 11,
                FontStyle.Regular, Color.Gray, ContentAlignment.MiddleCenter);
            lblContador.Name = "lblContador";
            lblContador.SetBounds(20, 295, 420, 30);
            lblContador.BackColor = Color.Transparent;

            panelCard.Controls.Add(panelTopo);
            panelCard.Controls.Add(lblNumero);
            panelCard.Controls.Add(lblAguarde);
            panelCard.Controls.Add(lblContador);

            panelConteudo.Controls.Add(panelCard);
            panelConteudo.Resize += (s, e) =>
            {
                panelCard.Location = new Point(
                    (panelConteudo.ClientSize.Width - panelCard.Width) / 2,
                    (panelConteudo.ClientSize.Height - panelCard.Height) / 2);
            };

            Controls.Add(panelConteudo);
        }

        private void ConfigurarTimer()
        {
            int segundos = 10;
            _timerAutoFechar.Interval = 1000;
            _timerAutoFechar.Tick += (s, e) =>
            {
                segundos--;
                var lbl = Controls.Find("lblContador", true).FirstOrDefault() as Label;
                if (lbl != null) lbl.Text = $"Retornando em {segundos} segundos...";
                if (segundos <= 0) { _timerAutoFechar.Stop(); Close(); }
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
