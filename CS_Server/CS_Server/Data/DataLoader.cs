using Newtonsoft.Json;
using Shared;

public static class DataLoader
{
    private static string GetFilePath(string fileName)
    {
        var basePath = ConfigManager.Instance.PathConfig.GameDataPath;

        if (string.IsNullOrWhiteSpace(basePath) || string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("Invalid base path or file name");

        return Path.Combine(basePath, $"{fileName}.json");
    }

    public static Dictionary<Key, Value> Load<Loader, Key, Value>(string fileName) where Loader : ILoader<Key, Value>
    {
        try
        {
            var filePath = GetFilePath(fileName);
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File not found: {filePath}");

            var text = File.ReadAllText(filePath);

            var rootObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(text);
            object? dataToDeserialize;

            var keysOfInterest = new[] { "item" };
            dataToDeserialize = keysOfInterest
                .Select(key => rootObject?.ContainsKey(key) == true ? rootObject[key] : null)
                .FirstOrDefault(value => value != null);

            // 적합한 데이터가 없으면 전체 JSON을 사용
            dataToDeserialize ??= rootObject;

            // 데이터 직렬화 후 Loader 생성
            var specificJson = JsonConvert.SerializeObject(dataToDeserialize);

            var loader = JsonConvert.DeserializeObject<Loader>(specificJson);
            if (loader == null)
                throw new InvalidOperationException($"Failed to deserialize {fileName}");

            return loader.MakeDict();
        }
        catch (Exception ex)
        {
            throw new Exception($"[DataLoader] Error loading file '{fileName}': {ex.Message}");
        }
    }
}
