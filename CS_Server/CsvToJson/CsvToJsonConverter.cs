using Newtonsoft.Json;
using Shared;
using System.Text;
using static Program.Program;

namespace CsvToJson;

public static class CsvToJsonConverter
{
    public static List<T> LoadCsv<T>(string filePath) where T : ICsvConvertible, new()
    {
        var result = new List<T>();

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        var encoding = Encoding.GetEncoding("euc-kr");


        var lines = File.ReadAllLines(filePath, encoding)
                    .Where(line => !string.IsNullOrWhiteSpace(line))
                    .ToArray();

        foreach (var line in lines.Skip(1)) // 첫번째 줄은 건너뛴다.
        {
            var values = line.Split(','); // 한줄씩 가져온다.
            T item = new T();
            item.FromCsv(values);
            result.Add(item);
        }
        return result;
    }

    public static void ConvertCsvToJson(string[] csvPaths, string saveFolderPath)
    {
        // 현재 어셈블리에서 모든 ICsvConvertible 구현 클래스를 찾는다.
        var convertibleTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(ICsvConvertible).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
            .ToList();

        foreach (var csvPath in csvPaths)
        {
            var fileName = Path.GetFileNameWithoutExtension(csvPath).ToLower();

            var targetType = convertibleTypes.FirstOrDefault(t => t.Name.ToLower() == fileName);
            if (targetType == null)
            {
                Console.WriteLine($"Unsupported file type: {fileName}");
                continue;
            }
            var method = typeof(CsvToJsonConverter)
                            .GetMethod("LoadCsv")
                            .MakeGenericMethod(targetType); // 해당 타입에 맞는 LoadCsv 호출

            var save = Path.Combine(saveFolderPath, $"{targetType.Name}.json");
            var data = method.Invoke(null, new object[] { csvPath });

            // Dictionary<string, List<T>>로 래핑
            var wrappedData = new Dictionary<string, object>
        {
            { fileName + "s", data } // fileName을 키로 사용하여 데이터를 감싼다.
        };

            var jsonData = JsonConvert.SerializeObject(wrappedData, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(save, jsonData);
        }
    }
}
