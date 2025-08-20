using System;
using Game.Configs.SO;
using UnityEngine;
using Utils;

namespace Game.Configs.Loaders {
    public class SOConfigLoader : ConfigsLoaderBase {
        protected override string ConfigPath => "SOConfigs/{0}.asset";

        public override T LoadConfigObject<T>() {
            var file = LoadConfigFile<T>();
            var soConfigFile = file as ISOConfigProvider<T>;
            if (soConfigFile is null) {
                throw new Exception($"Can't load config from file: {file}");
            }

            return soConfigFile.GetConfigObject();
        }

        private ScriptableObject LoadConfigFile<T>() {
            var result = ResourceLoader.LoadResource<ScriptableObject>(GetConfigFilePath<T>());
            return result;
        }
    }
}