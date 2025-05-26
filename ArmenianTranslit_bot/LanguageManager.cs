public class LanguageManager
{
    private readonly Dictionary<long, string> _userLanguages = new();

    public void SetLanguage(long userId, string languageCode)
    {
        _userLanguages[userId] = languageCode;
    }

    public string? GetLanguage(long userId)
    {
        return _userLanguages.TryGetValue(userId, out var lang) ? lang : null;
    }
}
