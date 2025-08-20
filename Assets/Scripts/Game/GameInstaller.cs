using Core.Controllers;
using Core.Model;
using Core.View.Cubes;
using Game.Configs;
using Game.Configs.Loaders;
using Game.State;
using Game.State.StateSaveLoading;
using UnityEngine;
using UnityEngine.Serialization;
using Utils.Localization;
using Zenject;

namespace Game {
    public class GameInstaller : MonoInstaller {
        [SerializeField] private GameObject placeableCubePrefab;
        [SerializeField] private GameObject scrollCubePrefab;
        
        [SerializeField]
        private CubesController cubesController;

        public override void InstallBindings() {
            InstallConfig();
            InstallState();
            InstallCore();
            InstallUtils();
        }

        private void InstallCore() {
            Container.BindInterfacesAndSelfTo<CubesModel>().AsSingle();
            Container.BindInstance(cubesController);
            Container.BindMemoryPool<PlaceableCubeView, PlaceableCubeView.Pool>()
                .FromComponentInNewPrefab(placeableCubePrefab).UnderTransformGroup("CubePool");
            Container.BindFactory<CubeScrollView, CubeScrollView.Factory>()
                .FromComponentInNewPrefab(scrollCubePrefab);
        }

        private void InstallState() {
            Container.BindInterfacesAndSelfTo<GameStateSystem>().AsSingle();
            Container.Bind<BaseSaveLoader>().To<JsonSaveLoader>().AsSingle();
        }

        private void InstallConfig() {
            Container.BindInterfacesAndSelfTo<Configs.Configs>().AsSingle();
            Container.Bind<ConfigsLoaderBase>().To<SOConfigLoader>().AsSingle();
            //Container.Bind<ConfigsLoaderBase>().To<JsonConfigLoader>().AsSingle();
        }

        private void InstallUtils() {
            Container.Bind<ILocalizationProvider>().To<StubLocalizationProvider>().AsSingle();
            Container.BindInterfacesAndSelfTo<LogTextDispatcher>().AsSingle();
        }
    }
}