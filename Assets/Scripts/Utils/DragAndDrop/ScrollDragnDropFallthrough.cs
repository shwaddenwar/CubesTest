using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Utils.DragAndDrop {
    public abstract class ScrollDragnDropFallthrough : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
        private IScrollDragnDropTarget target;
        [SerializeField] private ScrollRect targetScroll;

        protected bool DragTarget { get; private set; } = false;

        private void Awake() {
            target = GetComponent<IScrollDragnDropTarget>();
            if (target is null) {
                Debug.LogError($"Can't find target for ScrollDragnDropFallthrough on {gameObject.name}");
            }
        }

        public void SetScrollTarget(ScrollRect scrollView) {
            targetScroll = scrollView;
        }

        protected void StartDragTarget(PointerEventData eventData) {
            if (targetScroll) {
                ExecuteEvents.Execute(targetScroll.gameObject, eventData, ExecuteEvents.dropHandler);
                targetScroll.StopMovement();
            }

            DragTarget = true;
            OnBeginDrag(eventData);
        }

        public virtual void OnBeginDrag(PointerEventData eventData) {
            if (DragTarget) {
                target.OnBeginDrag(eventData);
                return;
            }

            if (targetScroll) {
                ExecuteEvents.Execute(targetScroll.gameObject, eventData, ExecuteEvents.beginDragHandler);
            }
        }

        public virtual void OnDrag(PointerEventData eventData) {
            if (DragTarget) {
                target.OnDrag(eventData);
                return;
            }

            if (targetScroll) {
                ExecuteEvents.Execute(targetScroll.gameObject, eventData, ExecuteEvents.dragHandler);
            }
        }

        public virtual void OnEndDrag(PointerEventData eventData) {
            if (DragTarget) {
                target.OnEndDrag(eventData);
                DragTarget = false;
                return;
            }

            if (targetScroll) {
                ExecuteEvents.Execute(targetScroll.gameObject, eventData, ExecuteEvents.endDragHandler);
            }
        }

        private void OnDestroy() {
            targetScroll = null;
            target = null;
        }
    }
}