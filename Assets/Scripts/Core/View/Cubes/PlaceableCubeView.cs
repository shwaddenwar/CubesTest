using Core.Controllers;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Core.View.Cubes {
    public class PlaceableCubeView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler {
        [SerializeField] private CubeView cubeView;
        [SerializeField] private CanvasGroup interactionCanvas;

        public RectTransform CubeViewTransform { get; private set; }
        public RectTransform RectTransform { get; private set; }

        private CubesController _controller;

        [Inject]
        private void Construct(CubesController controller) {
            _controller = controller;
        }

        public string CubeId { get; private set; }

        private void Awake() {
            CubeViewTransform = cubeView.transform as RectTransform;
            RectTransform = transform as RectTransform;
        }

        public void SetData(string cubeId) {
            CubeId = cubeId;
            cubeView.SetData(CubeId);
        }

        public void ToggleInteractable(bool isInteractable) {
            interactionCanvas.interactable = isInteractable;
            interactionCanvas.blocksRaycasts = isInteractable;
        }

        public void OnBeginDrag(PointerEventData eventData) {
            _controller.StartDragFromTower(eventData, this);
        }

        public void OnEndDrag(PointerEventData eventData) { }

        public void OnDrop(PointerEventData eventData) {
            _controller.OnDrop(eventData, this);
        }

        public void OnDrag(PointerEventData eventData) { }

        public class Pool : MonoMemoryPool<string, PlaceableCubeView> {
            protected override void Reinitialize(string dataId, PlaceableCubeView item) {
                item.RectTransform.DOKill();
                item.SetData(dataId);
                item.RectTransform.localScale = Vector3.one;
                item.RectTransform.rotation = Quaternion.identity;
                var viewTransform = item.cubeView.transform;
                viewTransform.DOKill();
                viewTransform.localScale = Vector3.one;
                viewTransform.rotation = Quaternion.identity;
            }
        }
    }
}