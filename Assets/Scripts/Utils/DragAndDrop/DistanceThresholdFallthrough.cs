using UnityEngine;
using UnityEngine.EventSystems;

namespace Utils.DragAndDrop {
    public class DistanceThresholdFallthrough : ScrollDragnDropFallthrough {
        [SerializeField] private float verticalThreshold;
        private Vector2 _dragStartOffset;

        public override void OnBeginDrag(PointerEventData eventData) {
            _dragStartOffset = (Vector2)transform.position - eventData.position;
            base.OnBeginDrag(eventData);
        }

        public override void OnDrag(PointerEventData eventData) {
            base.OnDrag(eventData);
            if (!DragTarget) {
                var dragAwayMag = ((Vector2)transform.position - eventData.position - _dragStartOffset).sqrMagnitude;
                if (dragAwayMag > verticalThreshold * verticalThreshold) {
                    StartDragTarget(eventData);
                }
            }
        }
    }
}