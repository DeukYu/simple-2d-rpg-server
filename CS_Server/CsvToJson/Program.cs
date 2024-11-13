using CsvToJson;
using Newtonsoft.Json;
using System.Xml;

namespace Program;

public class Program
{
    public static void Main(string[] args)
    {
        var folderPath = Path.Combine("../../../", "GameData");
        string[] files = Directory.GetFiles(folderPath, "*.csv");
        var saveForlderPath = Path.Combine("../../../../Common/", "GameData");

        CsvToJsonConverter.ConvertCsvToJson(files, saveForlderPath);

        Console.WriteLine(files);
    }
}