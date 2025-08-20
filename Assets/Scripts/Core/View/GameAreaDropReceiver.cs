using Core.Controllers;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Core.View {
    public class GameAreaDropReceiver : MonoBehaviour, IDropHandler {
        private CubesController _cubesController;

        [Inject]
        private void Construct(CubesController cubesController) {
            _cubesController = cubesController;
        }

        public void OnDrop(PointerEventData eventData) {
            _cubesController.OnDrop(eventData, this);
        }
    }
}