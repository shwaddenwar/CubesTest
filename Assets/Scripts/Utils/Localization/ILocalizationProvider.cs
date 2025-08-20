namespace Utils.Localization {
    public interface ILocalizationProvider {
        public string GetLocalizedString(string key);
        public void SetLanguage(string language);
    }
}