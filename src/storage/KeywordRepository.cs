using System.Text.Json;

namespace ewsense.storage;

public class KeywordRepository
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };
    private readonly string _keywordsFilePath;
    private List<string> _keywords;

    public KeywordRepository()
    {
        _keywordsFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ewsense_keywords.json"
        );
        _keywords = [];
        LoadKeywords();
    }

    public IReadOnlyList<string> Keywords => _keywords.AsReadOnly();

    public event Action? KeywordsChanged;

    public void AddKeyword(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return;

        var trimmedKeyword = keyword.Trim();
        if (_keywords.Contains(trimmedKeyword))
            return;

        _keywords.Add(trimmedKeyword);
        SaveKeywords();
        KeywordsChanged?.Invoke();
    }

    public void RemoveKeyword(string keyword)
    {
        if (!_keywords.Remove(keyword))
            return;

        SaveKeywords();
        KeywordsChanged?.Invoke();
    }

    public bool HasKeywordEndingWith(string text)
    {
        return _keywords.Any(text.EndsWith);
    }

    private void LoadKeywords()
    {
        try
        {
            if (!File.Exists(_keywordsFilePath))
                return;

            var json = File.ReadAllText(_keywordsFilePath);
            var loadedKeywords = JsonSerializer.Deserialize<List<string>>(json);
            if (loadedKeywords is { Count: > 0 })
            {
                _keywords = loadedKeywords;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Error loading keywords: {ex.Message}",
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );
        }
    }

    private void SaveKeywords()
    {
        try
        {
            var json = JsonSerializer.Serialize(_keywords, JsonOptions);
            File.WriteAllText(_keywordsFilePath, json);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Error saving keywords: {ex.Message}",
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );
        }
    }
}
