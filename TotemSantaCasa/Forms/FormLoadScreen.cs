using TotemSantaCasa.Models;
using TotemSantaCasa.Services;

namespace TotemSantaCasa.Forms
{
    public class FormLoadScreen : Form
    {
        private readonly ProgressBar _progressBar   = new();
        private readonly Label       _lblStatus     = new();
        private readonly Label       _lblNome       = new();
        private readonly Label       _lblSubtitulo  = new();
        private readonly PictureBox  _picInstituicao = new();
        private readonly PictureBox  _picDigitaly   = new();
        private readonly Panel       _panelBarraFundo = new();
        private readonly Panel       _panelBarraValor = new();
        private readonly System.Windows.Forms.Timer _timer = new();

        private int _progressoAtual = 0;
        private int _progressoAlvo  = 0;
        private bool _concluido     = false;

        private readonly Color _corFundo;
        private readonly Color _corTexto;
        private readonly Color _corBarra;

        public FormLoadScreen(LoadScreenConfig? cfg = null)
        {
            cfg ??= new LoadScreenConfig();

            _corFundo  = ParseCor(cfg.CorFundo,  UITheme.Primaria);
            _corTexto  = ParseCor(cfg.CorTexto,  Color.White);
            _corBarra  = ParseCor(cfg.CorBarra,  Color.White);

            Text             = "Iniciando...";
            WindowState      = FormWindowState.Maximized;
            FormBorderStyle  = FormBorderStyle.None;
            StartPosition    = FormStartPosition.CenterScreen;
            BackColor        = _corFundo;
            DoubleBuffered   = true;

            ConstruirLayout(cfg);

            _timer.Interval = 16;
            _timer.Tick    += Tick;
            _timer.Start();
        }

        private void ConstruirLayout(LoadScreenConfig cfg)
        {
            _picInstituicao.SizeMode  = PictureBoxSizeMode.Zoom;
            _picInstituicao.BackColor = Color.Transparent;

            _picDigitaly.SizeMode  = PictureBoxSizeMode.Zoom;
            _picDigitaly.BackColor = Color.Transparent;
            _picDigitaly.Visible   = cfg.ExibirLogoDigitaly;

            _lblNome.Font      = new Font(UITheme.FamiliaFonte, UITheme.TamanhoTitulo, FontStyle.Bold);
            _lblNome.ForeColor = _corTexto;
            _lblNome.BackColor = Color.Transparent;
            _lblNome.AutoSize  = false;
            _lblNome.TextAlign = ContentAlignment.MiddleCenter;

            _lblSubtitulo.Text      = "Sistema de Autoatendimento";
            _lblSubtitulo.Font      = new Font(UITheme.FamiliaFonte, 14);
            _lblSubtitulo.ForeColor = Color.FromArgb(200, _corTexto);
            _lblSubtitulo.BackColor = Color.Transparent;
            _lblSubtitulo.AutoSize  = false;
            _lblSubtitulo.TextAlign = ContentAlignment.MiddleCenter;

            _panelBarraFundo.BackColor = Color.FromArgb(50, _corTexto);

            _panelBarraValor.BackColor = _corBarra;
            _panelBarraValor.Width     = 0;

            _panelBarraFundo.Controls.Add(_panelBarraValor);

            _lblStatus.Text      = cfg.MensagemBoasVindas;
            _lblStatus.Font      = new Font(UITheme.FamiliaFonte, 11);
            _lblStatus.ForeColor = Color.FromArgb(180, _corTexto);
            _lblStatus.BackColor = Color.Transparent;
            _lblStatus.AutoSize  = false;
            _lblStatus.TextAlign = ContentAlignment.MiddleCenter;

            var lblVersao = new Label
            {
                Name      = "lblVersao",
                Text      = $"Digitaly Technology  •  v1.0  •  {HardwareService.ObterTotemId()}",
                Font      = new Font(UITheme.FamiliaFonte, 9),
                ForeColor = Color.FromArgb(110, _corTexto),
                BackColor = Color.Transparent,
                AutoSize  = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Anchor    = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            Controls.AddRange(new Control[]
            {
                _picInstituicao, _picDigitaly,
                _lblNome, _lblSubtitulo,
                _panelBarraFundo,
                _lblStatus, lblVersao
            });

            Resize += (_, _) => Reposicionar(lblVersao);
            Load   += (_, _) => Reposicionar(lblVersao);
        }

        private void Reposicionar(Label lblVersao)
        {
            int w  = ClientSize.Width;
            int h  = ClientSize.Height;
            int cx = w / 2;
            int cy = h / 2;

            _picInstituicao.SetBounds(cx - 100, cy - 240, 200, 130);
            _picDigitaly.SetBounds(cx - 60, h - 110, 120, 55);

            _lblNome.SetBounds(40, cy - 100, w - 80, 55);
            _lblSubtitulo.SetBounds(40, cy - 44, w - 80, 34);

            _panelBarraFundo.SetBounds(60, cy + 25, w - 120, 6);
            _panelBarraValor.Height = 6;

            _lblStatus.SetBounds(40, cy + 44, w - 80, 30);
            lblVersao.SetBounds(0, h - 38, w, 28);
        }

        public void DefinirNome(string nome)
        {
            if (InvokeRequired) { Invoke(() => DefinirNome(nome)); return; }
            _lblNome.Text = nome;
        }

        public void AtualizarStatus(string mensagem, int progresso)
        {
            if (InvokeRequired) { Invoke(() => AtualizarStatus(mensagem, progresso)); return; }
            _lblStatus.Text = mensagem;
            _progressoAlvo  = Math.Clamp(progresso, 0, 100);
        }

        public void CarregarLogoInstituicao(string urlOuCaminho)
        {
            if (string.IsNullOrEmpty(urlOuCaminho)) return;
            Task.Run(async () =>
            {
                var img = await CarregarImagemAsync(urlOuCaminho);
                if (img == null) return;
                if (InvokeRequired) Invoke(() => _picInstituicao.Image = img);
                else _picInstituicao.Image = img;
            });
        }

        public void CarregarLogoDigitaly(string urlOuCaminho)
        {
            if (string.IsNullOrEmpty(urlOuCaminho)) return;
            Task.Run(async () =>
            {
                var img = await CarregarImagemAsync(urlOuCaminho);
                if (img == null) return;
                if (InvokeRequired) Invoke(() => _picDigitaly.Image = img);
                else _picDigitaly.Image = img;
            });
        }

        public void Concluir()
        {
            if (InvokeRequired) { Invoke(Concluir); return; }
            _progressoAlvo = 100;
            _lblStatus.Text = "Sistema pronto.";
            _concluido = true;
        }

        private void Tick(object? sender, EventArgs e)
        {
            if (_progressoAtual < _progressoAlvo)
            {
                _progressoAtual = Math.Min(_progressoAtual + 2, _progressoAlvo);
                int largura = (int)(_panelBarraFundo.Width * (_progressoAtual / 100.0));
                _panelBarraValor.SetBounds(0, 0, largura, 6);
            }

            if (_concluido && _progressoAtual >= 100)
            {
                _timer.Stop();
                Task.Delay(700).ContinueWith(_ =>
                {
                    if (IsDisposed) return;
                    if (InvokeRequired) Invoke(Close);
                    else Close();
                });
            }
        }

        private static async Task<Image?> CarregarImagemAsync(string urlOuCaminho)
        {
            try
            {
                if (urlOuCaminho.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    using var http  = new HttpClient();
                    var bytes       = await http.GetByteArrayAsync(urlOuCaminho);
                    using var ms    = new MemoryStream(bytes);
                    return Image.FromStream(new MemoryStream(bytes));
                }
                return Image.FromFile(urlOuCaminho);
            }
            catch { return null; }
        }

        private static Color ParseCor(string hex, Color fallback)
        {
            try { return ColorTranslator.FromHtml(hex); }
            catch { return fallback; }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _timer.Stop();
            base.OnFormClosing(e);
        }
    }
}
