namespace TotemSantaCasa.Forms
{
    public static class UIHelper
    {
        public static readonly Color CorPrimaria = Color.FromArgb(180, 30, 30);
        public static readonly Color CorSecundaria = Color.FromArgb(220, 60, 60);
        public static readonly Color CorBotaoVerde = Color.FromArgb(34, 139, 34);
        public static readonly Color CorFundo = Color.White;
        public static readonly Color CorTexto = Color.FromArgb(30, 30, 30);
        public static readonly Color CorCinzaClaro = Color.FromArgb(245, 245, 245);
        public static readonly Color CorCinzaBorda = Color.FromArgb(200, 200, 200);

        public static Button CriarBotaoGrande(string texto, Color corFundo, EventHandler? onClick = null)
        {
            var btn = new Button
            {
                Text = texto,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = corFundo,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Height = 90,
                Width = 320,
                TextAlign = ContentAlignment.MiddleCenter
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = ControlPaint.Light(corFundo, 0.2f);
            btn.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(corFundo, 0.1f);

            if (onClick != null)
                btn.Click += onClick;

            return btn;
        }

        public static Button CriarBotaoPequeno(string texto, Color corFundo, EventHandler? onClick = null)
        {
            var btn = new Button
            {
                Text = texto,
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = corFundo,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Height = 55,
                Width = 200,
                TextAlign = ContentAlignment.MiddleCenter
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = ControlPaint.Light(corFundo, 0.2f);

            if (onClick != null)
                btn.Click += onClick;

            return btn;
        }

        public static Label CriarLabel(string texto, int tamanhoFonte, FontStyle estilo = FontStyle.Regular,
            Color? cor = null, ContentAlignment alinhamento = ContentAlignment.MiddleLeft)
        {
            return new Label
            {
                Text = texto,
                Font = new Font("Segoe UI", tamanhoFonte, estilo),
                ForeColor = cor ?? CorTexto,
                BackColor = Color.Transparent,
                AutoSize = false,
                TextAlign = alinhamento
            };
        }

        public static Panel CriarCabecalho(string titulo, string subtitulo = "")
        {
            var panel = new Panel
            {
                BackColor = CorPrimaria,
                Height = 100,
                Dock = DockStyle.Top
            };

            var lblTitulo = new Label
            {
                Text = titulo,
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };

            if (!string.IsNullOrEmpty(subtitulo))
            {
                lblTitulo.TextAlign = ContentAlignment.BottomCenter;
                lblTitulo.Dock = DockStyle.None;
                lblTitulo.Location = new Point(0, 15);
                lblTitulo.Size = new Size(panel.Width, 45);
                lblTitulo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

                var lblSub = new Label
                {
                    Text = subtitulo,
                    Font = new Font("Segoe UI", 13),
                    ForeColor = Color.FromArgb(255, 200, 200),
                    BackColor = Color.Transparent,
                    AutoSize = false,
                    TextAlign = ContentAlignment.TopCenter,
                    Location = new Point(0, 60),
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
                };
                panel.Controls.Add(lblSub);
            }

            panel.Controls.Add(lblTitulo);
            return panel;
        }

        public static Panel CriarRodape()
        {
            var panel = new Panel
            {
                BackColor = Color.FromArgb(240, 240, 240),
                Height = 50,
                Dock = DockStyle.Bottom
            };

            var lblRodape = new Label
            {
                Text = $"Santa Casa de Curitiba  •  {Config.Configuracoes.Ambulatorio}  •  {DateTime.Now:dd/MM/yyyy}",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Gray,
                BackColor = Color.Transparent,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };

            panel.Controls.Add(lblRodape);
            return panel;
        }
    }
}
