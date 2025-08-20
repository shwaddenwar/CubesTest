using System.Collections.Generic;

namespace Utils.Localization {
    public class StubLocalizationProvider : ILocalizationProvider {
        private static readonly Dictionary<string, string> RuLocales = new() {
            { "cube_placed_tower", "Кубик размещён в башне" },
            { "cube_placed_area", "Кубик размещён на поле" },
            { "cube_drop_none", "Кубик исчез" },
            { "cube_drop_hole", "Кубик выкинут в дыру" },
            { "tower_height_limit", "Башня достигла максимальной высоты" }
        };

        private static readonly Dictionary<string, string> EnLocales = new() {
            { "cube_placed_tower", "Cube has been placed in tower" },
            { "cube_placed_area", "Cube has been placed in game area" },
            { "cube_drop_none", "Cube has disappeared" },
            { "cube_drop_hole", "Cube was discarded into a hole" },
            { "tower_height_limit", "Tower has reached height limit" }
        };

        private readonly Dictionary<string, Dictionary<string, string>> _localizations = new() {
            { "ru", RuLocales },
            { "en", EnLocales }
        };

        private string _currentLanguage = "ru";

        public string GetLocalizedString(string key) {
            if (!_localizations[_currentLanguage].TryGetValue(key, out var locale)) {
                return $"NO LOCALE: {key}";
            }

            return locale;
        }

        public void SetLanguage(string language) {
            if (!_localizations.ContainsKey(language)) {
                return;
            }

            _currentLanguage = language;
        }
    }
}