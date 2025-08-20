using System.Collections.Generic;
using Core.Model;
using UnityEngine;
using Zenject;

namespace Core.View.Cubes {
    public class CubeTowerView : MonoBehaviour {
        [SerializeField] private RectTransform gameArea;
        [SerializeField] private float cubeGap;
        private Vector2 _cubeSize;
        private Vector2 _startPosition;
        private List<CubePlacement> _placedCubes = new();
        private PlaceableCubeView.Pool _placeableCubePool;
        private CubesModel _cubesModel;

        [Inject]
        private void Construct(PlaceableCubeView.Pool placeableCubePool, CubesModel cubesModel) {
            _cubesModel = cubesModel;
            _placeableCubePool = placeableCubePool;
        }

        public void PlaceNewCube(Vector2 atWorldPosition, PlaceableCubeView cube, float offset, bool animated = true) {
            var isFirstCube = _placedCubes.Count == 0;
            Vector2 placeAt;
            if (isFirstCube) {
                var cubeTransform = cube.RectTransform;
                _cubeSize = cubeTransform.sizeDelta;
                _startPosition = atWorldPosition + GetNewPlacementOffset(offset) * Vector2.right;
                placeAt = _startPosition;
            }
            else {
                var horizontalPosition = GetNewPlacementOffset(offset) +
                                         GetPlacementAt(_placedCubes.Count - 1).HorizontalPlace;
                var verticalPosition = GetPlacementHeight(_placedCubes.Count);
                placeAt = new Vector2(horizontalPosition, verticalPosition);
            }

            var cubePlacement = new CubePlacement(placeAt.x, cube);
            _placedCubes.Add(cubePlacement);
            if (isFirstCube) {
                cube.RectTransform.position = placeAt;
                return;
            }

            if (animated) {
                AnimationsSystem.JumpToTower(cube.RectTransform, placeAt, cube.CubeViewTransform);
            }
            else {
                cube.RectTransform.position = placeAt;
            }
        }

        public Vector2 WorldToGameArea(Vector2 worldPosition) {
            var screenPosition = RectTransformUtility.WorldToScreenPoint(Camera.current, worldPosition);
            return gameArea.InverseTransformPoint(screenPosition);
        }

        public Vector2 GameAreaToWorld(Vector2 gameAreaPos) {
            return gameArea.TransformPoint(gameAreaPos);
        }

        public DropData GetDropData(Vector2 atPosition, RectTransform forCube) {
            var forCubeSize = forCube.sizeDelta;

            int cubesLeft;
            var lowerOffsetBound = float.MinValue;
            var upperOffsetBound = float.MaxValue;

            var minCorner = gameArea.rect.min + forCubeSize / 2;
            var maxCorner = gameArea.rect.max - forCubeSize / 2;
            var minWorld = GameAreaToWorld(minCorner);
            var maxWorld = GameAreaToWorld(maxCorner);
            var resultPosition = atPosition;
            if (_placedCubes.Count == 0) {
                cubesLeft = 1;
                resultPosition = new Vector2(Mathf.Clamp(atPosition.x, minWorld.x, maxWorld.x),
                    Mathf.Clamp(atPosition.y, minWorld.y, maxWorld.y));
            }
            else {
                var nextHorizontalPosition = GetNewPlacementOffset(0) +
                                             GetPlacementAt(_placedCubes.Count - 1).HorizontalPlace;
                var nextVerticalPosition = GetPlacementHeight(_placedCubes.Count);
                cubesLeft = Mathf.CeilToInt((maxWorld.y - nextVerticalPosition) / _cubeSize.y);
                lowerOffsetBound = Mathf.Max(lowerOffsetBound,
                    (minWorld.x - nextHorizontalPosition) / forCubeSize.x);
                upperOffsetBound = Mathf.Min(upperOffsetBound, (maxWorld.x - nextHorizontalPosition) / forCubeSize.x);
            }

            var result = new DropData(cubesLeft, lowerOffsetBound, upperOffsetBound, resultPosition,
                WorldToGameArea(resultPosition));
            return result;
        }

        public void HandleCubeRemoved(CubeRemoveResult cubeRemoveResult, int atIndex) {
            _placedCubes.RemoveAt(atIndex);
            if (cubeRemoveResult == CubeRemoveResult.Ignore) {
                return;
            }

            for (var i = _placedCubes.Count - 1; i >= atIndex; i--) {
                switch (cubeRemoveResult) {
                    case CubeRemoveResult.Fall:
                        DropCube(i);
                        break;
                    case CubeRemoveResult.Delete:
                        RemoveCube(i);
                        break;
                }
            }
        }

        public bool IsPartOfTower(PlaceableCubeView cubeView) {
            return GetPlacementIndexOf(cubeView) >= 0;
        }

        public int GetPlacementIndexOf(PlaceableCubeView cubeView) {
            return _placedCubes.FindIndex(p => p.Cube == cubeView);
        }

        private float GetNewPlacementOffset(float offset) {
            return _cubeSize.x * offset;
        }

        private float GetPlacementHeight(int index) {
            return _startPosition.y + index * (_cubeSize.y + cubeGap);
        }

        public Vector2 GetCubePlacementPosition(int index) {
            var cubePlacement = GetPlacementAt(index);
            return new Vector2(cubePlacement.HorizontalPlace, GetPlacementHeight(index));
        }

        private CubePlacement GetPlacementAt(int index) {
            return _placedCubes[index];
        }

        private void RemoveCube(int atIndex) {
            var cubePlacement = GetPlacementAt(atIndex);
            _placedCubes.RemoveAt(atIndex);
            var cubeToDestroy = cubePlacement.Cube;
            AnimationsSystem.DestroyCube(cubeToDestroy.RectTransform, () => { ReturnCubeToPool(cubeToDestroy); });
        }

        private void DropCube(int atIndex) {
            var cubePlacement = _placedCubes[atIndex];
            AnimationsSystem.FloatTo(cubePlacement.Cube.RectTransform, GetCubePlacementPosition(atIndex));
        }

        private void ReturnCubeToPool(PlaceableCubeView cubeView) {
            if (cubeView == null) {
                return;
            }

            _placeableCubePool.Despawn(cubeView);
        }

        private class CubePlacement {
            public float HorizontalPlace { get; private set; }
            public PlaceableCubeView Cube { get; private set; }

            public CubePlacement(float horizontalPlace, PlaceableCubeView cube) {
                HorizontalPlace = horizontalPlace;
                Cube = cube;
            }
        }
    }
}