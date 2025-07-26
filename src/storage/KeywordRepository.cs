using System.Text.Json;

namespace ewsense.storage;

public class KeywordRepository
{
    private readonly string _keywordsFilePath;
    private List<string> _keywords;

    public KeywordRepository()
    {
        _keywordsFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ewsense_keywords.json"
        );
        _keywords = new List<string>();
        LoadKeywords();
    }

    public IReadOnlyList<string> Keywords => _keywords.AsReadOnly();

    public event Action? KeywordsChanged;

    public void AddKeyword(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return;

        var trimmedKeyword = keyword.Trim();
        if (!_keywords.Contains(trimmedKeyword))
        {
            _keywords.Add(trimmedKeyword);
            SaveKeywords();
            KeywordsChanged?.Invoke();
        }
    }

    public void RemoveKeyword(string keyword)
    {
        if (_keywords.Remove(keyword))
        {
            SaveKeywords();
            KeywordsChanged?.Invoke();
        }
    }

    public void ClearKeywords()
    {
        if (_keywords.Count > 0)
        {
            _keywords.Clear();
            SaveKeywords();
            KeywordsChanged?.Invoke();
        }
    }

    public bool ContainsKeyword(string keyword)
    {
        return _keywords.Contains(keyword);
    }

    public bool HasKeywordEndingWith(string text)
    {
        return _keywords.Any(keyword => text.EndsWith(keyword));
    }

    public string? GetKeywordEndingWith(string text)
    {
        return _keywords.FirstOrDefault(keyword => text.EndsWith(keyword));
    }

    private void LoadKeywords()
    {
        try
        {
            if (File.Exists(_keywordsFilePath))
            {
                string json = File.ReadAllText(_keywordsFilePath);
                var loadedKeywords = JsonSerializer.Deserialize<List<string>>(json);
                if (loadedKeywords != null && loadedKeywords.Count > 0)
                {
                    _keywords = loadedKeywords;
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading keywords: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void SaveKeywords()
    {
        try
        {
            string json = JsonSerializer.Serialize(_keywords, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_keywordsFilePath, json);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving keywords: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
