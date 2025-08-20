using Game.Configs.ConfigObjects;
using Zenject;

namespace Game.Configs {
    public class Configs {
        private ConfigsLoaderBase _configsLoader;

        public GameConfig gameConfig { get; private set; }

        [Inject]
        public Configs(ConfigsLoaderBase configLoader) {
            _configsLoader = configLoader;
        }


        public void LoadConfig() {
            gameConfig = _configsLoader.LoadConfigObject<GameConfig>();
        }
    }
}