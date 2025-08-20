using System.Collections.Generic;
using Game.State;
using Game.State.StateSaveLoading;
using UnityEngine;
using Zenject;

namespace Game {
    public class Bootstrap : MonoBehaviour {
        private Configs.Configs _configs;
        private GameStateSystem _gameStateSystem;

        [Inject]
        private void Construct(Configs.Configs configs, GameStateSystem gameStateSystem, BaseSaveLoader stateLoader) {
            _gameStateSystem = gameStateSystem;
            _configs = configs;
        }

        private void Awake() {
            _configs.LoadConfig();
            _gameStateSystem.LoadState();
            ValidateState();
        }

        private void ValidateState() {
            var colorSet = new HashSet<string>(_configs.gameConfig.CubeColors.Count);
            foreach (var cubeData in _configs.gameConfig.CubeColors) {
                colorSet.Add(cubeData);
            }

            var stateBad = false;
            foreach (var placedCube in _gameStateSystem.State.PlacedCubes) {
                if (!colorSet.Contains(placedCube.CubeId)) {
                    stateBad = true;
                    break;
                }
            }

            if (stateBad) {
                Debug.LogWarning("State no longer valid, resetting");
                _gameStateSystem.ResetState();
            }
        }
    }
}