using Newtonsoft.Json;
using ServerCore;
using Shared;
using System.Text;

namespace CsvToJson;

public static class CsvToJsonConverter
{
    static CsvToJsonConverter()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    // CSV 데이터를 List로 로드하는 메서드
    public static List<T> LoadCsv<T>(string filePath) where T : ICsvConvertible, new()
    {
        var encoding = Encoding.GetEncoding("euc-kr");
        var lines = File.ReadAllLines(filePath, encoding)
                        .Where(line => !string.IsNullOrWhiteSpace(line))
                        .ToArray();

        if(lines.Length == 0)
        {
            throw new Exception("CSV 파일이 비어있습니다.");
        }

        var headerLine = lines[0];
        var headers = headerLine.Split(',');
        ValidateHeader<T>(headers);

        return lines.Skip(1).Select(line => {
            var values = line.Split(',');
            T item = new T();
            item.FromCsv(values);
            return item;
        }).ToList();
    }

    // 헤더와 클래스 속성 일치 여부를 검사하는 메서드
    private static void ValidateHeader<T>(string[] headers)
    {
        var properties = typeof(T).GetProperties();
        foreach (var header in headers)
        {
            var headerParts = header.Split(':');
            if (headerParts.Length != 2)
                throw new Exception($"헤더 형식이 잘못되었습니다: {header}");

            var headerName = headerParts[0].Trim();
            var headerType = headerParts[1].Trim().ToLower();

            // T 타입의 속성 중 헤더 이름과 일치하는 속성을 찾음
            var property = properties.FirstOrDefault(p => p.Name.Equals(headerName, StringComparison.OrdinalIgnoreCase));
            if (property == null)
                throw new Exception($"'{headerName}'에 해당하는 속성이 클래스 '{typeof(T).Name}'에 존재하지 않습니다.");

            // 속성 타입과 헤더 타입을 비교하여 일치 여부 검사
            var propertyType = GetTypeString(property.PropertyType);
            if (propertyType != headerType)
                throw new Exception($"속성 '{headerName}'의 타입 '{propertyType}'가 CSV 헤더 타입 '{headerType}'와 일치하지 않습니다.");
        }
    }

    // C# 타입을 문자열로 반환하는 유틸리티 메서드
    private static string GetTypeString(Type type)
    {
        if (type == typeof(byte)) return "byte";
        if (type == typeof(short)) return "short";
        if (type == typeof(int)) return "int";
        if (type == typeof(long)) return "long";
        if (type == typeof(bool)) return "bool";
        if (type == typeof(float)) return "float";
        if (type == typeof(double)) return "double";
        if (type == typeof(string)) return "string";
        // 필요한 다른 타입도 추가 가능
        return type.Name.ToLower();
    }

    // 여러 CSV 파일을 JSON으로 변환하는 메인 메서드
    public static void ConvertCsvToJson(string[] csvPaths, string saveFolderPath)
    {
        var convertibleTypes = FindConvertibleTypes();

        var groupedFiles = csvPaths.GroupBy(path =>
        {
            var fileName = Path.GetFileNameWithoutExtension(path).ToLower();
            var underscoreIndex = fileName.IndexOf('_');
            return underscoreIndex > 0 ? fileName.Substring(0, underscoreIndex).ToLower() : null;
        });

        foreach(var group in groupedFiles)
        {
            if (group.Key == null)
            {
                foreach(var csvPath in group)
                {
                    ConvertSingleCsv(csvPath, saveFolderPath, convertibleTypes);
                }
            }
            else
            {
                ConvertGroupedFilesToJson(group.Key, group.ToList(), saveFolderPath, convertibleTypes);
            }
        }
    }

    // ICsvConvertible을 구현한 타입 찾기
    private static List<Type> FindConvertibleTypes()
    {
        return AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .Where(t => typeof(ICsvConvertible).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                    .ToList();
    }

    // 단일 CSV를 JSON으로 변환하는 메서드
    private static void ConvertSingleCsv(string csvPath, string saveFolderPath, List<Type> convertibleTypes)
    {
        var fileName = Path.GetFileNameWithoutExtension(csvPath).ToLower();
        var adjustedFileName = $"{fileName}data";
        var targetType = convertibleTypes.FirstOrDefault(t => t.Name.Equals(adjustedFileName, StringComparison.OrdinalIgnoreCase));

        if (targetType == null)
        {
            Console.WriteLine($"Unsupported file type: {fileName}");
            return;
        }

        var data = InvokeLoadCsv(targetType, csvPath);
        if (data == null)
        {
            Log.Error($"Failed to load {csvPath}");
            return;
        }

        // JSON 형식으로 감싸서 저장
        var wrappedData = new Dictionary<string, object> { { $"{fileName}s", data } };
        SaveToJsonFile(wrappedData, saveFolderPath, targetType.Name);
    }
    private static void ConvertGroupedFilesToJson(string prefix, List<string> files, string saveFolderPath, List<Type> convertibleTypes)
    {
        var groupedData = new Dictionary<string, List<object>>();

        foreach (var file in files)
        {
            var fileName = Path.GetFileNameWithoutExtension(file);
            var suffix = fileName.Split('_').Last();

            var adjustedFileName = $"{suffix}data";
            var targetType = convertibleTypes.FirstOrDefault(t => t.Name.Equals(adjustedFileName, StringComparison.OrdinalIgnoreCase));

            if (targetType == null)
            {
                Console.WriteLine($"Unsupported file type: {fileName}");
                continue;
            }

            var data = InvokeLoadCsv(targetType, file) as IEnumerable<object>;
            if (data == null)
            {
                Log.Error($"Failed to load {file}");
                continue;
            }

            var key = $"{suffix}s";
            groupedData[key] = data.ToList();
        }

        // JSON 파일로 저장
        SaveToJsonFile(new Dictionary<string, object> { { prefix, groupedData } }, saveFolderPath, $"{prefix}data");
    }

    // LoadCsv 메서드를 동적으로 호출
    private static object? InvokeLoadCsv(Type targetType, string csvPath)
    {
        var method = typeof(CsvToJsonConverter).GetMethod("LoadCsv")?.MakeGenericMethod(targetType);
        return method?.Invoke(null, new object[] { csvPath });
    }

    // 데이터를 JSON 파일로 저장
    private static void SaveToJsonFile(Dictionary<string, object> data, string saveFolderPath, string fileName)
    {
        var savePath = Path.Combine(saveFolderPath, $"{fileName}.json");
        var jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(savePath, jsonData);
    }
}
