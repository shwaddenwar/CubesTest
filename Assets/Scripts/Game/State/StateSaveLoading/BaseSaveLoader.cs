namespace Game.State.StateSaveLoading {
    public abstract class BaseSaveLoader {
        public abstract void SaveState(GameState state);

        public abstract GameState LoadState();

        public abstract void ResetState();
    }
}