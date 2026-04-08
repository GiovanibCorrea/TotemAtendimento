using TotemSantaCasa.Services;

namespace TotemSantaCasa.Forms
{
    public static class UIHelper
    {
        public static Color CorPrimaria => UITheme.Primaria;
        public static Color CorSecundaria => UITheme.Secundaria;
        public static Color CorBotaoVerde => UITheme.BotaoConfirmar;
        public static Color CorFundo => UITheme.Fundo;
        public static Color CorTexto => UITheme.Texto;
        public static Color CorCinzaClaro => UITheme.CinzaClaro;
        public static Color CorCinzaBorda => UITheme.CinzaBorda;

        public static Button CriarBotaoGrande(string texto, Color corFundo, EventHandler? onClick = null)
        {
            var btn = new Button
            {
                Text = texto,
                Font = UITheme.FonteBotao(),
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
            if (onClick != null) btn.Click += onClick;
            return btn;
        }

        public static Button CriarBotaoPequeno(string texto, Color corFundo, EventHandler? onClick = null)
        {
            var btn = new Button
            {
                Text = texto,
                Font = new Font(UITheme.FamiliaFonte, 13, FontStyle.Bold),
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
            if (onClick != null) btn.Click += onClick;
            return btn;
        }

        public static Label CriarLabel(string texto, int tamanhoFonte, FontStyle estilo = FontStyle.Regular,
            Color? cor = null, ContentAlignment alinhamento = ContentAlignment.MiddleLeft)
        {
            return new Label
            {
                Text = texto,
                Font = new Font(UITheme.FamiliaFonte, tamanhoFonte, estilo),
                ForeColor = cor ?? UITheme.Texto,
                BackColor = Color.Transparent,
                AutoSize = false,
                TextAlign = alinhamento
            };
        }

        public static Panel CriarCabecalho(string titulo, string subtitulo = "")
        {
            var panel = new Panel
            {
                BackColor = UITheme.Cabecalho,
                Height = 100,
                Dock = DockStyle.Top
            };

            var lblTitulo = new Label
            {
                Text = titulo,
                Font = UITheme.FonteCabecalho(),
                ForeColor = UITheme.CabecalhoTexto,
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
                lblTitulo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

                var lblSub = new Label
                {
                    Text = subtitulo,
                    Font = new Font(UITheme.FamiliaFonte, 13),
                    ForeColor = Color.FromArgb(220, UITheme.CabecalhoTexto.R,
                        UITheme.CabecalhoTexto.G, UITheme.CabecalhoTexto.B),
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
                Font = new Font(UITheme.FamiliaFonte, 9),
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
