using Core.Controllers;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils.DragAndDrop;
using Zenject;

namespace Core.View.Cubes {
    public class CubeScrollView : MonoBehaviour, IScrollDragnDropTarget {
        [SerializeField] private CubeView cubeView;
        private CubesController _cubesController;

        public string CubeId { get; private set; }

        [Inject]
        private void Construct(CubesController cubesController) {
            _cubesController = cubesController;
        }

        public void SetData(string cubeId) {
            CubeId = cubeId;
            cubeView.SetData(CubeId);
        }

        public void OnDrag(PointerEventData eventData) { }
        public void OnEndDrag(PointerEventData eventData) { }

        public void OnBeginDrag(PointerEventData eventData) {
            var viewTransform = cubeView.transform;
            viewTransform.DOKill();
            viewTransform.localScale = Vector3.zero;
            viewTransform.Rotate(new Vector3(0, 0, 60));
            _cubesController.StartDragFromScroll(eventData, this);
            viewTransform.DOScale(Vector3.one, 0.5f);
            viewTransform.DORotateQuaternion(Quaternion.identity, 0.5f).SetEase(Ease.OutCubic);
        }

        public class Factory : PlaceholderFactory<CubeScrollView> {
            public CubeScrollView CreateAndInit(ScrollRect scrollToFallthrough, string colorId) {
                var result = Create();
                var scrollFallthrough = result.GetComponent<ScrollDragnDropFallthrough>();
                scrollFallthrough.SetScrollTarget(scrollToFallthrough);
                result.SetData(colorId);
                return result;
            }
        }
    }
}