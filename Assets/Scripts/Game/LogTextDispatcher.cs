using System;
using Utils.Localization;
using Zenject;

namespace Game {
    public class LogTextDispatcher {
        private readonly ILocalizationProvider _localizationProvider;

        public event Action<string> OnShowText;

        [Inject]
        private LogTextDispatcher(ILocalizationProvider localizationProvider) {
            _localizationProvider = localizationProvider;
        }

        public void ShowText(string textKey) {
            var localizedText = _localizationProvider.GetLocalizedString(textKey);
            HandleShowText(localizedText);
        }

        protected virtual void HandleShowText(string obj) {
            OnShowText?.Invoke(obj);
        }
    }
}