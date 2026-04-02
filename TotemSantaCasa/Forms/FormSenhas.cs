namespace TotemSantaCasa.Forms
{
    public class FormSenhas : Form
    {
        private System.Windows.Forms.Timer _timerInatividade = new();

        public FormSenhas()
        {
            InicializarComponentes();
            ConfigurarTimer();
        }

        private void InicializarComponentes()
        {
            Text = "Senhas de Atendimento";
            WindowState = FormWindowState.Maximized;
            FormBorderStyle = FormBorderStyle.None;
            BackColor = UIHelper.CorFundo;
            StartPosition = FormStartPosition.CenterScreen;

            var panelCabecalho = UIHelper.CriarCabecalho("SENHAS", "Selecione o tipo de atendimento");
            Controls.Add(panelCabecalho);
            Controls.Add(UIHelper.CriarRodape());

            var panelConteudo = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = UIHelper.CorFundo
            };

            var tiposSenha = new[]
            {
                ("RETORNO\nCANCEROLOGIA", "Retorno\nCancerologia"),
                ("PRIORITÁRIO\nADE", "Prioritário\nADE"),
                ("PRIORITÁRIO\n60+ ADE", "Prioritário\n60+ ADE"),
                ("EXAME/\nPROCEDIMENTO", "Exame/\nProcedimento"),
                ("UROLOGIA", "Urologia"),
                ("CHECK-IN\nTÉRREO", "Check-in\nTérreo"),
                ("OTORRINO", "Otorrino"),
                ("OFTALMOLOGIA", "Oftalmologia")
            };

            var tabela = new TableLayoutPanel
            {
                RowCount = 3,
                ColumnCount = 3,
                Anchor = AnchorStyles.None,
                AutoSize = true
            };

            for (int i = 0; i < 3; i++)
            {
                tabela.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                tabela.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            }

            for (int i = 0; i < tiposSenha.Length; i++)
            {
                string tipo = tiposSenha[i].Item2;
                var btn = CriarBotaoSenha(tiposSenha[i].Item1, (s, e) => EmitirSenha(tipo));
                tabela.Controls.Add(btn, i % 3, i / 3);
            }

            var btnVoltar = UIHelper.CriarBotaoPequeno("← VOLTAR", Color.Gray, (s, e) => Close());
            btnVoltar.Anchor = AnchorStyles.None;

            panelConteudo.Controls.Add(tabela);
            panelConteudo.Controls.Add(btnVoltar);
            panelConteudo.Resize += (s, e) =>
            {
                tabela.Location = new Point(
                    (panelConteudo.ClientSize.Width - tabela.Width) / 2,
                    (panelConteudo.ClientSize.Height - tabela.Height) / 2 - 40);
                btnVoltar.Location = new Point(
                    (panelConteudo.ClientSize.Width - btnVoltar.Width) / 2,
                    panelConteudo.ClientSize.Height - 80);
            };

            Controls.Add(panelConteudo);
        }

        private static Button CriarBotaoSenha(string texto, EventHandler onClick)
        {
            var btn = new Button
            {
                Text = texto,
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = UIHelper.CorPrimaria,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Width = 185,
                Height = 100,
                Margin = new Padding(6),
                TextAlign = ContentAlignment.MiddleCenter
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = UIHelper.CorSecundaria;
            btn.Click += onClick;
            return btn;
        }

        private void EmitirSenha(string tipo)
        {
            string numero = GerarNumeroSenha(tipo);

            var impressao = new Printing.ImpressaoService();
            bool ok = impressao.ImprimirSenha(tipo.Replace("\n", " "), numero);

            if (!ok)
            {
                MessageBox.Show("Erro ao emitir senha. Dirija-se ao guichê.",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var formConclusaoSenha = new FormConclusaoSenha(tipo.Replace("\n", " "), numero);
            formConclusaoSenha.ShowDialog(this);
            Close();
        }

        private static readonly Dictionary<string, int> _contadoresSenha = new();

        private static string GerarNumeroSenha(string tipo)
        {
            if (!_contadoresSenha.ContainsKey(tipo))
                _contadoresSenha[tipo] = 0;

            _contadoresSenha[tipo]++;

            string prefixo = tipo switch
            {
                var t when t.Contains("Retorno") => "RC",
                var t when t.Contains("Prioritário") && t.Contains("ADE") => "PA",
                var t when t.Contains("60+") => "P6",
                var t when t.Contains("Exame") => "EP",
                var t when t.Contains("Urologia") => "UR",
                var t when t.Contains("Check") => "CT",
                var t when t.Contains("Otorrino") => "OT",
                var t when t.Contains("Oftalmo") => "OF",
                _ => "SN"
            };

            return $"{prefixo}{_contadoresSenha[tipo]:D3}";
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
