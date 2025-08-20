namespace Game.Configs.SO {
    public interface ISOConfigProvider<T> {
        public T GetConfigObject();
    }
}