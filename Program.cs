// See https://aka.ms/new-console-template for more information
using jomiunsExtensions;
using System.Diagnostics;

var argsClone = args.ToArray();
for (int leIndex = 0; leIndex < argsClone.Length; leIndex++)
{
    var arg = argsClone[leIndex];
    argsClone[leIndex] = arg.replacess(new
    {
        jt = "juta",
        jeti = "juta",
        rb = "ribu",
        ribeng = "ribu",
    });
}
var aParser = jomiunsCli.factory.newCliParser(argsClone);
var h38745 = aParser.registerSatuan<string>("ambil nilai untuk satuan tertentu", "satuannya", "meter", "m", "tumbak");
if (aParser.startParsing() is false)
    return;

if (h38745.Results.Any())
{
    var theSatuan = h38745.Results.First().matchedKeyword;
    var valueDalamSatuan = h38745.Results.First().theResult.reverseTerbilang();
    var valueDalamMeter = valueDalamSatuan; //default meter

    if (theSatuan.inThisButIgnoreCase("tumbak"))
        valueDalamMeter *= 14;

    //tentukan nilai jualnya
    var allUnrecognizedCommand = string.Join(" ", aParser.unrecognizedCommands);
    var nilaiJual = allUnrecognizedCommand.reverseTerbilang();
    var hargaPerMeter = nilaiJual / valueDalamMeter;

    Console.WriteLine($@"Luas {valueDalamSatuan} {theSatuan}
Harga {allUnrecognizedCommand} ({nilaiJual:N0})
{(theSatuan.notInThisButIgnoreCase("meter") ? $@"Luas dalam meter {valueDalamMeter} meter
Harga per meter: {hargaPerMeter:N0}" : $"Harga per meter: {hargaPerMeter:N0}")}");
}

if (IsLaunchedFromCmd() is false)
{
    Task.Run(async () => {
        await Task.Delay(10000);
        Environment.Exit(0);
    });
    Console.WriteLine("Press any key to exit... (otomatis dalam 10 detik)");
    Console.ReadKey();
}

return;
static bool IsLaunchedFromCmd()
{
    try
    {
        using (var process = Process.GetCurrentProcess())
        {
            var parent = GetParentProcess(process.Id);
            if (parent == null) return false;

            string parentName = parent.ProcessName.ToLower();
            return parentName == "cmd" || parentName == "powershell" || parentName == "wt"; // Windows Terminal
        }
    }
    catch
    {
        return false;
    }
}

static Process GetParentProcess(int id)
{
    var query = $"SELECT ParentProcessId FROM Win32_Process WHERE ProcessId = {id}";
    using (var searcher = new System.Management.ManagementObjectSearcher(query))
    {
        var results = searcher.Get().Cast<System.Management.ManagementObject>();
        var parentId = results.FirstOrDefault()?["ParentProcessId"];
        if (parentId == null) return null;

        return Process.GetProcessById(Convert.ToInt32(parentId));
    }
}
