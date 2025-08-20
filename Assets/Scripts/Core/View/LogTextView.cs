using DG.Tweening;
using Game;
using TMPro;
using UnityEngine;
using Zenject;

namespace Core.View {
    public class LogTextView : MonoBehaviour {
        [SerializeField] private CanvasGroup fadeCanvas;
        [SerializeField] private TextMeshProUGUI tmpText;

        [SerializeField] private float fadeTime;
        [SerializeField] private float showTime;

        private LogTextDispatcher _logTextDispatcher;
        private Sequence _fadeSequence;

        [Inject]
        private void Construct(LogTextDispatcher logTextDispatcher) {
            _logTextDispatcher = logTextDispatcher;
        }

        public void DisplayText(string text) {
            tmpText.text = text;
            ShowText();
        }

        private void ShowText() {
            _fadeSequence.Kill();
            _fadeSequence = DOTween.Sequence();
            _fadeSequence.Append(fadeCanvas.DOFade(1, fadeTime));
            _fadeSequence.AppendInterval(showTime);
            _fadeSequence.Append(fadeCanvas.DOFade(0, fadeTime));
            _fadeSequence.Play();
        }

        private void OnEnable() {
            _logTextDispatcher.OnShowText += DisplayText;
        }

        private void OnDisable() {
            _logTextDispatcher.OnShowText -= DisplayText;
        }
    }
}