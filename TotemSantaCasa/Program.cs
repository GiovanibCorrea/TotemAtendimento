using System.Runtime.InteropServices;
using TotemSantaCasa.Config;
using TotemSantaCasa.Forms;

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
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmDeviceName;
            public short dmSpecVersion, dmDriverVersion, dmSize, dmDriverExtra;
            public int dmFields;
            public int dmPositionX, dmPositionY;
            public int dmDisplayOrientation, dmDisplayFixedOutput;
            public short dmColor, dmDuplex, dmYResolution, dmTTOption, dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmFormName;
            public short dmLogPixels;
            public int dmBitsPerPel, dmPelsWidth, dmPelsHeight;
            public int dmDisplayFlags, dmDisplayFrequency;
            public int dmICMMethod, dmICMIntent, dmMediaType, dmDitherType;
            public int dmReserved1, dmReserved2, dmPanningWidth, dmPanningHeight;
        }

        private const int DMDO_270 = 3;
        private const int ENUM_CURRENT_SETTINGS = -1;
        private const int CDS_UPDATEREGISTRY = 0x01;

        private static void DefinirOrientacaoPortrait()
        {
            var dm = new DEVMODE();
            dm.dmSize = (short)Marshal.SizeOf(typeof(DEVMODE));

            if (!EnumDisplaySettings(null, ENUM_CURRENT_SETTINGS, ref dm))
                return;

            if (dm.dmDisplayOrientation == 0 || dm.dmDisplayOrientation == 2)
                (dm.dmPelsWidth, dm.dmPelsHeight) = (dm.dmPelsHeight, dm.dmPelsWidth);

            dm.dmDisplayOrientation = DMDO_270;
            dm.dmFields = 0x00080000 | 0x00100000 | 0x00400000;

            ChangeDisplaySettings(ref dm, CDS_UPDATEREGISTRY);
        }

        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
            DefinirOrientacaoPortrait();

            try
            {
                Configuracoes.Inicializar();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar configurações: {ex.Message}\n\nUsando valores padrão.",
                    "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.ThreadException += (sender, e) =>
            {
                MessageBox.Show($"Erro inesperado: {e.Exception.Message}\n\nO sistema será reiniciado.",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Restart();
            };

            while (true)
            {
                var formInicial = new FormInicial();
                Application.Run(formInicial);
            }
        }
    }
}
