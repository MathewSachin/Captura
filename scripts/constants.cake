readonly var sourceFolder = Directory("src");
readonly var tempFolder = Directory("temp");
readonly var distFolder = Directory("dist");
readonly var licensesFolder = Directory("licenses");
readonly var chocoFolder = Directory("choco");

readonly var slnPath = sourceFolder + File("Captura.sln");

readonly var PortablePath = tempFolder + File("Captura-Portable.zip");
readonly var SetupPath = tempFolder + File("Captura-Setup.exe");

const string Release = "Release";