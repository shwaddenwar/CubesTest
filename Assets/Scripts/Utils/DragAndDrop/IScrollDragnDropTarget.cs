using UnityEngine.EventSystems;

namespace Utils.DragAndDrop {
    public interface IScrollDragnDropTarget {
        public void OnDrag(PointerEventData eventData);
        public void OnBeginDrag(PointerEventData eventData);
        public void OnEndDrag(PointerEventData eventData);
    }
}