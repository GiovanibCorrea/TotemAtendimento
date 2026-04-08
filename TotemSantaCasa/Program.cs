using System.Runtime.InteropServices;
using TotemSantaCasa.Config;
using TotemSantaCasa.Forms;
using TotemSantaCasa.Services;

namespace TotemSantaCasa
{
    internal static class Program
    {
        [DllImport("user32.dll")]
        private static extern bool EnumDisplaySettings(string? deviceName, int modeNum, ref DEVMODE devMode);

        [DllImport("user32.dll")]
        private static extern int ChangeDisplaySettings(ref DEVMODE devMode, int flags);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        private struct DEVMODE
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] public string dmDeviceName;
            public short dmSpecVersion, dmDriverVersion, dmSize, dmDriverExtra;
            public int dmFields, dmPositionX, dmPositionY;
            public int dmDisplayOrientation, dmDisplayFixedOutput;
            public short dmColor, dmDuplex, dmYResolution, dmTTOption, dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] public string dmFormName;
            public short dmLogPixels;
            public int dmBitsPerPel, dmPelsWidth, dmPelsHeight;
            public int dmDisplayFlags, dmDisplayFrequency;
            public int dmICMMethod, dmICMIntent, dmMediaType, dmDitherType;
            public int dmReserved1, dmReserved2, dmPanningWidth, dmPanningHeight;
        }

        private const int DMDO_270             = 3;
        private const int ENUM_CURRENT_SETTINGS = -1;
        private const int CDS_UPDATEREGISTRY   = 0x01;

        public static ConfiguracaoService ConfigService  { get; } = new();
        public static LicencaService      LicencaService { get; private set; } = new();

        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
            DefinirOrientacaoPortrait();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.ThreadException += (_, e) =>
            {
                MessageBox.Show($"Erro inesperado: {e.Exception.Message}\n\nO sistema será reiniciado.",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Restart();
            };

            while (true)
            {
                if (!ExecutarInicializacao()) break;
                Application.Run(new FormInicial());
            }
        }

        private static bool ExecutarInicializacao()
        {
            Models.TotemConfig? config = null;
            bool licencaOk = false;
            string mensagemLicenca = string.Empty;

            var loadScreen = new FormLoadScreen();
            loadScreen.Show();
            loadScreen.Refresh();

            var tarefa = Task.Run(async () =>
            {
                loadScreen.AtualizarStatus("Carregando configurações...", 10);
                await Task.Delay(300);

                try { Configuracoes.Inicializar(); } catch { }

                string endpointRemoto = Configuracoes.ApiBaseUrl;
                LicencaService = new LicencaService(endpointRemoto);

                loadScreen.AtualizarStatus("Conectando ao servidor...", 22);
                config = await ConfigService.InicializarAsync(endpointRemoto);

                if (config != null)
                {
                    loadScreen.DefinirNome(config.NomeInstituicao);

                    if (!string.IsNullOrEmpty(config.LoadScreen.UrlLogoInstituicao))
                        loadScreen.CarregarLogoInstituicao(config.LoadScreen.UrlLogoInstituicao);

                    if (!string.IsNullOrEmpty(config.LoadScreen.UrlLogoDigitaly))
                        loadScreen.CarregarLogoDigitaly(config.LoadScreen.UrlLogoDigitaly);
                }

                loadScreen.AtualizarStatus("Verificando licença...", 50);
                await Task.Delay(300);

                var resultadoLicenca = await LicencaService.ValidarAsync();
                licencaOk = resultadoLicenca.Valida;
                mensagemLicenca = resultadoLicenca.Mensagem;

                if (!licencaOk)
                {
                    loadScreen.AtualizarStatus("Licença pendente de ativação.", 70);
                    await Task.Delay(500);
                    loadScreen.Concluir();
                    return;
                }

                loadScreen.AtualizarStatus("Inicializando impressoras...", 75);
                await Task.Delay(300);

                loadScreen.AtualizarStatus("Conectando ao sistema Tasy...", 90);
                await Task.Delay(300);

                loadScreen.AtualizarStatus("Sistema pronto.", 100);
                await Task.Delay(100);
                loadScreen.Concluir();
            });

            while (!tarefa.IsCompleted)
            {
                Application.DoEvents();
                Thread.Sleep(16);
            }

            while (!loadScreen.IsDisposed && loadScreen.Visible)
            {
                Application.DoEvents();
                Thread.Sleep(16);
            }

            if (!licencaOk)
            {
                var formBloqueio = new FormBloqueioLicenca(LicencaService, mensagemLicenca);
                Application.Run(formBloqueio);

                if (!formBloqueio.Ativado)
                    return false;
            }

            return true;
        }

        private static void DefinirOrientacaoPortrait()
        {
            var dm = new DEVMODE();
            dm.dmSize = (short)Marshal.SizeOf(typeof(DEVMODE));
            if (!EnumDisplaySettings(null, ENUM_CURRENT_SETTINGS, ref dm)) return;
            if (dm.dmDisplayOrientation == 0 || dm.dmDisplayOrientation == 2)
                (dm.dmPelsWidth, dm.dmPelsHeight) = (dm.dmPelsHeight, dm.dmPelsWidth);
            dm.dmDisplayOrientation = DMDO_270;
            dm.dmFields = 0x00080000 | 0x00100000 | 0x00400000;
            ChangeDisplaySettings(ref dm, CDS_UPDATEREGISTRY);
        }
    }
}
