using System;
using System.Reflection;

namespace Game.Configs {
    public abstract class ConfigsLoaderBase {
        protected abstract string ConfigPath { get; }

        public abstract T LoadConfigObject<T>();

        protected string GetConfigFilePath<T>() {
            var configName = typeof(T).GetCustomAttribute<ConfigAttribute>()?.Name;
            if (string.IsNullOrEmpty(configName)) {
                throw new Exception($"Can't no config attribute or config name is empty on {typeof(T)}");
            }

            return string.Format(ConfigPath, configName);
        }
    }
}