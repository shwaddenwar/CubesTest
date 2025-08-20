using Game.Configs;
using Game.State;
using UnityEngine;
using Zenject;

namespace Core.Model {
    public class CubesModel {
        private readonly GameStateSystem _gameStateSystem;
        private readonly Configs _configs;
        private GameState GameState => _gameStateSystem.State;
        public int PlacedCubes => GameState.PlacedCubes.Count;
        public Vector2 StartPosition => GameState.StartPosition;


        [Inject]
        private CubesModel(GameStateSystem gameStateSystem, Configs configs) {
            _configs = configs;
            _gameStateSystem = gameStateSystem;
        }

        public float PlaceCube(DropData dropData, string cubeId) {
            float offset = 0;
            if (PlacedCubes == 0) {
                SetStartPosition(dropData.FinalGameAreaPosition);
            }
            else {
                offset = GenerateOffset(dropData.LowerOffsetBound, dropData.UpperOffsetBound);
            }

            AddCubeToState(cubeId, offset);
            return offset;
        }

        public bool CanPlaceOnGameArea(DropData dropData, string cubeId) {
            return PlacedCubes < 1;
        }

        public bool CanPlaceCubeOnTower(DropData dropData, string cubeId) {
            return dropData.CubesLeft >= 0;
        }

        public CubeState GetCubeAt(int index) {
            return GameState.PlacedCubes[index];
        }

        private float GenerateOffset(float lowerBound = float.MinValue, float upperBound = float.MaxValue) {
            var randomValue = Random.value - 0.5f;
            return Mathf.Clamp(randomValue, lowerBound, upperBound);
        }

        private void AddCubeToState(string cubeId, float offset) {
            GameState.PlacedCubes.Add(new CubeState(cubeId, offset));
            _gameStateSystem.SaveState();
        }

        private void SetStartPosition(Vector2 position) {
            GameState.StartPosition = position;
        }

        public CubeRemoveResult RemoveCube(int fromIndex) {
            var result = RemoveCubeFromState(fromIndex);
            _gameStateSystem.SaveState();
            return result;
        }

        private CubeRemoveResult RemoveCubeFromState(int fromIndex) {
            var wasCubeOffset = GameState.PlacedCubes[fromIndex].Offset;
            GameState.PlacedCubes.RemoveAt(fromIndex);
            if (PlacedCubes <= fromIndex) {
                return CubeRemoveResult.Ignore;
            }

            var newOffset = wasCubeOffset + GameState.PlacedCubes[fromIndex].Offset;
            if (fromIndex != 0 && Mathf.Abs(newOffset) > 1) {
                DeleteCubesFrom(fromIndex);
                return CubeRemoveResult.Delete;
            }

            GameState.PlacedCubes[fromIndex].Offset = newOffset;
            return CubeRemoveResult.Fall;
        }

        private void DeleteCubesFrom(int fromIndex) {
            for (var i = GameState.PlacedCubes.Count - 1; i >= fromIndex; i--) {
                GameState.PlacedCubes.RemoveAt(i);
            }
        }
    }

    public enum CubeRemoveResult {
        Fall,
        Delete,
        Ignore
    }

    public class DropData {
        public int CubesLeft { get; private set; }
        public float LowerOffsetBound { get; private set; }
        public float UpperOffsetBound { get; private set; }
        public Vector2 FinalWorldPosition { get; private set; }
        public Vector2 FinalGameAreaPosition { get; private set; }

        public DropData(int cubesLeft, float lowerOffsetBound, float upperOffsetBound, Vector2 finalWorldPosition,
            Vector2 finalGameAreaPosition) {
            CubesLeft = cubesLeft;
            LowerOffsetBound = lowerOffsetBound;
            UpperOffsetBound = upperOffsetBound;
            FinalWorldPosition = finalWorldPosition;
            FinalGameAreaPosition = finalGameAreaPosition;
        }
    }
}