using System.Management;
using System.Security.Cryptography;
using System.Text;

namespace TotemSantaCasa.Services
{
    public static class HardwareService
    {
        private static string? _fingerprintCache;

        public static string ObterFingerprint()
        {
            if (_fingerprintCache != null)
                return _fingerprintCache;

            var sb = new StringBuilder();

            try { sb.Append(ObterValorWmi("Win32_Processor", "ProcessorId")); } catch { }
            try { sb.Append(ObterValorWmi("Win32_BaseBoard", "SerialNumber")); } catch { }
            try { sb.Append(ObterValorWmi("Win32_BIOS", "SerialNumber")); } catch { }
            try { sb.Append(ObterValorWmi("Win32_DiskDrive", "SerialNumber")); } catch { }

            if (sb.Length == 0)
                sb.Append(Environment.MachineName);

            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString()));
            _fingerprintCache = Convert.ToHexString(hash)[..32];

            return _fingerprintCache;
        }

        public static string ObterTotemId()
        {
            string fp = ObterFingerprint();
            return $"TOTEM-{fp[..8].ToUpper()}";
        }

        private static string ObterValorWmi(string classe, string propriedade)
        {
            using var searcher = new ManagementObjectSearcher($"SELECT {propriedade} FROM {classe}");
            foreach (ManagementObject obj in searcher.Get())
            {
                var val = obj[propriedade]?.ToString()?.Trim();
                if (!string.IsNullOrEmpty(val) && val != "To Be Filled By O.E.M.")
                    return val;
            }
            return string.Empty;
        }
    }
}
