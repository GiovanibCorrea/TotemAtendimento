using System.Drawing.Printing;
using System.Runtime.InteropServices;
using TotemSantaCasa.Models;

namespace TotemSantaCasa.Printing
{
    public class ImpressaoService
    {
        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool OpenPrinter(string pPrinterName, out IntPtr phPrinter, IntPtr pDefault);

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool ClosePrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool StartDocPrinter(IntPtr hPrinter, int Level, ref DOCINFO pDocInfo);

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool EndDocPrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool StartPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool EndPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, int dwCount, out int dwWritten);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct DOCINFO
        {
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pDocName;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pOutputFile;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pDataType;
        }

        public bool ImprimirPulseiraZebra(Paciente paciente)
        {
            string zpl = GerarZPL(paciente);
            return EnviarParaImpressora(Config.Configuracoes.ImpressoraZebra, zpl);
        }

        public bool ImprimirComprovanteTermico(Paciente paciente)
        {
            bool resultado = false;
            var pd = new PrintDocument();
            pd.PrinterSettings.PrinterName = Config.Configuracoes.ImpressoraTermica;

            pd.PrintPage += (sender, e) =>
            {
                if (e.Graphics == null) return;
                DesenharComprovante(e.Graphics, paciente, e.MarginBounds);
            };

            try
            {
                pd.Print();
                resultado = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao imprimir comprovante: {ex.Message}", "Erro de Impressão",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return resultado;
        }

        public bool ImprimirSenha(string tipoSenha, string numeroSenha)
        {
            bool resultado = false;
            var pd = new PrintDocument();
            pd.PrinterSettings.PrinterName = Config.Configuracoes.ImpressoraTermica;

            pd.PrintPage += (sender, e) =>
            {
                if (e.Graphics == null) return;
                DesenharSenha(e.Graphics, tipoSenha, numeroSenha, e.MarginBounds);
            };

            try
            {
                pd.Print();
                resultado = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao imprimir senha: {ex.Message}", "Erro de Impressão",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return resultado;
        }

        private static string GerarZPL(Paciente paciente)
        {
            string dataNasc = paciente.DataNascimento.ToString("dd/MM/yy");
            string nome = paciente.NomeCompleto.Length > 30
                ? paciente.NomeCompleto[..30].ToUpper()
                : paciente.NomeCompleto.ToUpper();

            return $@"^XA
^CI28
^PW812
^LL203
^FO10,10^A0N,28,28^FH\^FD{nome}^FS
^FO10,42^A0N,22,22^FH\^FDNasc: {dataNasc}  Sexo: {paciente.Sexo}^FS
^FO10,68^A0N,22,22^FH\^FDMae: {(paciente.NomeMae.Length > 28 ? paciente.NomeMae[..28].ToUpper() : paciente.NomeMae.ToUpper())}^FS
^FO10,94^A0N,22,22^FH\^FDAtendimento: {paciente.NumeroAtendimento}  {paciente.Setor}^FS
^FO10,120^A0N,22,22^FH\^FDMedico: {(paciente.MedicoResponsavel.Length > 26 ? paciente.MedicoResponsavel[..26].ToUpper() : paciente.MedicoResponsavel.ToUpper())}^FS
^FO10,150^BY2^BCN,40,Y,N,N^FD{paciente.CodigoBarras}^FS
^XZ";
        }

        private static void DesenharComprovante(Graphics g, Paciente paciente, Rectangle area)
        {
            float y = area.Top;
            float largura = area.Width;

            using var fonteTitulo = new Font("Courier New", 12, FontStyle.Bold);
            using var fonteNormal = new Font("Courier New", 9);
            using var fontePequena = new Font("Courier New", 8);
            using var br = new SolidBrush(Color.Black);

            string linha = new string('-', 42);

            g.DrawString("SANTA CASA DE CURITIBA", fonteTitulo, br,
                new RectangleF(area.Left, y, largura, 20), CentralizadoFormat());
            y += 22;
            g.DrawString($"Ambulatório {Config.Configuracoes.Ambulatorio}", fonteNormal, br,
                new RectangleF(area.Left, y, largura, 16), CentralizadoFormat());
            y += 20;
            g.DrawString(linha, fontePequena, br, area.Left, y);
            y += 16;

            g.DrawString("COMPROVANTE DE ATENDIMENTO", fonteNormal, br,
                new RectangleF(area.Left, y, largura, 16), CentralizadoFormat());
            y += 20;
            g.DrawString(linha, fontePequena, br, area.Left, y);
            y += 16;

            g.DrawString($"Paciente : {paciente.NomeCompleto}", fonteNormal, br, area.Left, y); y += 16;
            g.DrawString($"CPF      : {FormatarCpf(paciente.CPF)}", fonteNormal, br, area.Left, y); y += 16;
            g.DrawString($"Nasc     : {paciente.DataNascimento:dd/MM/yyyy}", fonteNormal, br, area.Left, y); y += 16;
            g.DrawString($"Convenio : {paciente.Convenio}", fontePequena, br, area.Left, y); y += 16;
            g.DrawString(linha, fontePequena, br, area.Left, y);
            y += 16;

            g.DrawString($"Atend    : {paciente.NumeroAtendimento}", fonteNormal, br, area.Left, y); y += 16;
            g.DrawString($"Setor    : {paciente.Setor}", fonteNormal, br, area.Left, y); y += 16;
            g.DrawString($"Medico   : {paciente.MedicoResponsavel}", fontePequena, br, area.Left, y); y += 16;
            g.DrawString($"Hora     : {paciente.HoraAgendamento:dd/MM/yyyy HH:mm}", fonteNormal, br, area.Left, y); y += 16;
            g.DrawString(linha, fontePequena, br, area.Left, y);
            y += 16;

            g.DrawString("Aguarde o chamado para atendimento.", fontePequena, br,
                new RectangleF(area.Left, y, largura, 16), CentralizadoFormat());
            y += 16;
            g.DrawString(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), fontePequena, br,
                new RectangleF(area.Left, y, largura, 16), CentralizadoFormat());
        }

        private static void DesenharSenha(Graphics g, string tipoSenha, string numero, Rectangle area)
        {
            float y = area.Top;
            float largura = area.Width;

            using var fonteTitulo = new Font("Courier New", 12, FontStyle.Bold);
            using var fonteNumero = new Font("Courier New", 48, FontStyle.Bold);
            using var fonteNormal = new Font("Courier New", 9);
            using var br = new SolidBrush(Color.Black);

            g.DrawString("SANTA CASA DE CURITIBA", fonteTitulo, br,
                new RectangleF(area.Left, y, largura, 20), CentralizadoFormat());
            y += 24;
            g.DrawString($"CHECK-IN TÉRREO", fonteNormal, br,
                new RectangleF(area.Left, y, largura, 16), CentralizadoFormat());
            y += 24;
            g.DrawString(tipoSenha.ToUpper(), fonteTitulo, br,
                new RectangleF(area.Left, y, largura, 20), CentralizadoFormat());
            y += 30;
            g.DrawString(numero, fonteNumero, br,
                new RectangleF(area.Left, y, largura, 70), CentralizadoFormat());
            y += 75;
            g.DrawString(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), fonteNormal, br,
                new RectangleF(area.Left, y, largura, 16), CentralizadoFormat());
        }

        private bool EnviarParaImpressora(string nomePrinter, string dados)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(dados);
            IntPtr hPrinter = IntPtr.Zero;
            IntPtr pBytes = IntPtr.Zero;

            try
            {
                if (!OpenPrinter(nomePrinter, out hPrinter, IntPtr.Zero))
                    return false;

                var docInfo = new DOCINFO
                {
                    pDocName = "Pulseira",
                    pDataType = "RAW"
                };

                if (!StartDocPrinter(hPrinter, 1, ref docInfo))
                    return false;

                StartPagePrinter(hPrinter);
                pBytes = Marshal.AllocCoTaskMem(bytes.Length);
                Marshal.Copy(bytes, 0, pBytes, bytes.Length);
                WritePrinter(hPrinter, pBytes, bytes.Length, out _);
                EndPagePrinter(hPrinter);
                EndDocPrinter(hPrinter);
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                if (pBytes != IntPtr.Zero) Marshal.FreeCoTaskMem(pBytes);
                if (hPrinter != IntPtr.Zero) ClosePrinter(hPrinter);
            }
        }

        private static StringFormat CentralizadoFormat()
        {
            return new StringFormat { Alignment = StringAlignment.Center };
        }

        private static string FormatarCpf(string cpf)
        {
            if (cpf.Length == 11)
                return $"{cpf[..3]}.{cpf[3..6]}.{cpf[6..9]}-{cpf[9..]}";
            return cpf;
        }
    }
}
