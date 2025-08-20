using Game.State.StateSaveLoading;
using Zenject;

namespace Game.State {
    public class GameStateSystem {
        public GameState State { get; private set; }
        private BaseSaveLoader _saveLoader;

        [Inject]
        public GameStateSystem(BaseSaveLoader saveLoader) {
            _saveLoader = saveLoader;
        }

        public void LoadState() {
            State = _saveLoader.LoadState();
        }

        public void SaveState() {
            _saveLoader.SaveState(State);
        }

        public void ResetState() {
            State.PlacedCubes.Clear();
            State.StartPosition = default;
            _saveLoader.ResetState();
        }
    }
}