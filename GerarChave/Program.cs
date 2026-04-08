using System.Management;
using System.Security.Cryptography;
using System.Text;

Console.WriteLine("=== Digitaly Technology - Gerador de Chave de Ativação ===\n");

string fingerprint = ObterFingerprint();
string totemId = $"TOTEM-{fingerprint[..8].ToUpper()}";
string chave = GerarChave(fingerprint, totemId);

Console.WriteLine($"TotemId     : {totemId}");
Console.WriteLine($"Fingerprint : {fingerprint}");
Console.WriteLine($"\nChave de Ativação:\n");
Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine($"  {chave}");
Console.ResetColor();
Console.WriteLine("\nInsira essa chave na tela de ativação do totem.");
Console.WriteLine("\nPressione qualquer tecla para sair...");
Console.ReadKey();

static string ObterFingerprint()
{
    var sb = new StringBuilder();
    try { sb.Append(Wmi("Win32_Processor", "ProcessorId")); } catch { }
    try { sb.Append(Wmi("Win32_BaseBoard", "SerialNumber")); } catch { }
    try { sb.Append(Wmi("Win32_BIOS", "SerialNumber")); } catch { }
    try { sb.Append(Wmi("Win32_DiskDrive", "SerialNumber")); } catch { }
    if (sb.Length == 0) sb.Append(Environment.MachineName);
    using var sha = SHA256.Create();
    var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString()));
    return Convert.ToHexString(hash)[..32];
}

static string Wmi(string classe, string prop)
{
    using var s = new ManagementObjectSearcher($"SELECT {prop} FROM {classe}");
    foreach (ManagementObject o in s.Get())
    {
        var v = o[prop]?.ToString()?.Trim();
        if (!string.IsNullOrEmpty(v) && v != "To Be Filled By O.E.M.") return v;
    }
    return string.Empty;
}

static string GerarChave(string fingerprint, string totemId)
{
    string seed = $"DIGITALY-{fingerprint}-{totemId}-TOTEM";
    using var sha = SHA256.Create();
    var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(seed));
    string hex = Convert.ToHexString(hash)[..20].ToUpper();
    return $"{hex[..5]}-{hex[5..10]}-{hex[10..15]}-{hex[15..20]}";
}
