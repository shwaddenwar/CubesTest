using Game.Configs;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Core.View.Cubes {
    public class CubePickerView : MonoBehaviour {
        [SerializeField] private RectTransform cubeContainer;
        [SerializeField] private ScrollRect containerScroll;
        private CubeScrollView.Factory _scrollCubeFactory;
        private Configs _configs;

        [Inject]
        public void Construct(CubeScrollView.Factory scrollCubeFactory, Configs configs) {
            _configs = configs;
            _scrollCubeFactory = scrollCubeFactory;
        }

        private void Awake() {
            LoadCubes();
        }

        private void LoadCubes() {
            var availableCubes = _configs.gameConfig.CubeColors;
            foreach (var cubeId in availableCubes) {
                CreateCube(cubeId);
            }
        }

        private void CreateCube(string fromId) {
            var newCube = _scrollCubeFactory.CreateAndInit(containerScroll, fromId);
            newCube.transform.SetParent(cubeContainer, false);
        }
    }
}