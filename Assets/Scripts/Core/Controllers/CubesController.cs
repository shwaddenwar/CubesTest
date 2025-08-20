using Core.Model;
using Core.View;
using Core.View.Cubes;
using DG.Tweening;
using Game;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Core.Controllers {
    public class CubesController : MonoBehaviour, IDragHandler, IEndDragHandler {
        [SerializeField] private RectTransform gameArea;
        [SerializeField] private CubeTowerView cubeTowerView;

        [SerializeField] private Transform aboveHole;
        [SerializeField] private Transform holeBottom;
        [SerializeField] private Transform holeMask;

        private PlaceableCubeView _currentDrag;
        private PlaceableCubeView.Pool _placeableCubeViewPool;
        private CubesModel _cubesModel;
        private LogTextDispatcher _logTextDispatcher;
        private bool _dropSuccessful = false;
        private bool _shouldReturnInteractivity = true;

        [Inject]
        private void Construct(PlaceableCubeView.Pool placeableCubeViewPool, CubesModel cubesModel,
            LogTextDispatcher logTextDispatcher) {
            _logTextDispatcher = logTextDispatcher;
            _placeableCubeViewPool = placeableCubeViewPool;
            _cubesModel = cubesModel;
        }

        private void Awake() {
            LoadGameArea();
        }

        private void LoadGameArea() {
            var startPosition = cubeTowerView.GameAreaToWorld(_cubesModel.StartPosition);
            for (var i = 0; i < _cubesModel.PlacedCubes; i++) {
                var cubeToLoad = _cubesModel.GetCubeAt(i);
                var cubeView = GetCubeView(cubeToLoad.CubeId, startPosition);
                cubeTowerView.PlaceNewCube(startPosition, cubeView, cubeToLoad.Offset, false);
            }
        }

        public void OnDrag(PointerEventData eventData) {
            if (_currentDrag is null) {
                return;
            }

            _currentDrag.transform.position = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData) {
            if (_currentDrag) {
                _currentDrag.OnEndDrag(eventData);
                if (!_dropSuccessful) {
                    OnDropFail(_currentDrag);
                }

                if (_shouldReturnInteractivity) {
                    _currentDrag.ToggleInteractable(true);
                }
            }

            _currentDrag = null;
        }

        private PlaceableCubeView GetCubeView(string fromData, Vector2 atPosition) {
            var result = _placeableCubeViewPool.Spawn(fromData);
            result.transform.SetParent(gameArea, false);
            result.transform.position = atPosition;
            return result;
        }

        public void StartDragFromScroll(PointerEventData eventData, CubeScrollView cubeView) {
            ExecuteEvents.Execute(eventData.pointerDrag, eventData, ExecuteEvents.endDragHandler);
            var draggedCubeView = GetCubeView(cubeView.CubeId, cubeView.transform.position);
            InitializeDrag(eventData, draggedCubeView);
            ExecuteEvents.Execute(_currentDrag.gameObject, eventData, ExecuteEvents.beginDragHandler);
        }

        public void StartDragFromTower(PointerEventData eventData, PlaceableCubeView cubeView) {
            InitializeDrag(eventData, cubeView);
        }

        public void OnDrop(PointerEventData eventData, PlaceableCubeView cubeView) {
            if (_currentDrag is null) {
                return;
            }

            var dropData = cubeTowerView.GetDropData(eventData.position, _currentDrag.RectTransform);

            if (!cubeTowerView.IsPartOfTower(cubeView) || cubeTowerView.IsPartOfTower(_currentDrag) ||
                !_cubesModel.CanPlaceCubeOnTower(dropData, _currentDrag.CubeId)) {
                return;
            }

            HandleDropInTower(dropData);
            _logTextDispatcher.ShowText("cube_placed_tower");
            if (dropData.CubesLeft <= 0) {
                _logTextDispatcher.ShowText("tower_height_limit");
            }
        }

        public void OnDrop(PointerEventData eventData, GameAreaDropReceiver gameAreaDropReceiver) {
            if (_currentDrag is null) {
                return;
            }

            var dropData = cubeTowerView.GetDropData(eventData.position, _currentDrag.RectTransform);

            if (!_cubesModel.CanPlaceOnGameArea(dropData, _currentDrag.CubeId)) {
                return;
            }

            HandleDropInTower(dropData);
            _logTextDispatcher.ShowText("cube_placed_area");
        }

        public void OnDrop(PointerEventData eventData, HoleDropReceiver holeDropReceiver) {
            if (_currentDrag is null) {
                return;
            }

            var cubeIndexInTower = cubeTowerView.GetPlacementIndexOf(_currentDrag);
            var cubeToDrop = _currentDrag;
            _dropSuccessful = true;
            _shouldReturnInteractivity = false;
            AnimationsSystem.DropIntoHole(cubeToDrop.RectTransform, aboveHole, holeBottom, holeMask,
                () => { DespawnCube(cubeToDrop); });

            if (cubeIndexInTower < 0) {
                return;
            }

            var removeResult = _cubesModel.RemoveCube(cubeIndexInTower);
            cubeTowerView.HandleCubeRemoved(removeResult, cubeIndexInTower);
            _logTextDispatcher.ShowText("cube_drop_hole");
        }

        private void InitializeDrag(PointerEventData eventData, PlaceableCubeView cubeView) {
            _dropSuccessful = false;
            _currentDrag = cubeView;
            _currentDrag.transform.DOKill();
            _currentDrag.ToggleInteractable(false);
            _shouldReturnInteractivity = true;
            eventData.pointerDrag = gameObject;
        }

        private void HandleDropInTower(DropData dropData) {
            var offset = _cubesModel.PlaceCube(dropData, _currentDrag.CubeId);
            cubeTowerView.PlaceNewCube(dropData.FinalWorldPosition, _currentDrag, offset);
            _dropSuccessful = true;
        }

        private void OnDropFail(PlaceableCubeView cubeView) {
            var towerIndex = cubeTowerView.GetPlacementIndexOf(cubeView);
            if (towerIndex >= 0) {
                var positionToFloatTo = cubeTowerView.GetCubePlacementPosition(towerIndex);
                AnimationsSystem.FloatTo(cubeView.RectTransform, positionToFloatTo);
                return;
            }

            _shouldReturnInteractivity = false;
            AnimationsSystem.DestroyCube(cubeView.CubeViewTransform, () => { DespawnCube(cubeView); });
            _logTextDispatcher.ShowText("cube_drop_none");
        }

        private void DespawnCube(PlaceableCubeView cubeView) {
            if (cubeView == null) {
                return;
            }

            _placeableCubeViewPool.Despawn(cubeView);
        }
    }
}