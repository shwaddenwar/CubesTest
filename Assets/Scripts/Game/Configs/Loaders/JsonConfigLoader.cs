using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using Utils;

namespace Game.Configs.Loaders {
    public class JsonConfigLoader : ConfigsLoaderBase {
        protected override string ConfigPath => "JSONConfigs/{0}.json";

        private JsonSerializer _serializer;

        public JsonConfigLoader() {
            _serializer = new JsonSerializer() {
                TypeNameHandling = TypeNameHandling.Auto
            };
        }

        public override T LoadConfigObject<T>() {
            var file = LoadConfigFile<T>();
            using var textReader = new StringReader(file.text);
            using var jsonReader = new JsonTextReader(textReader);
            return _serializer.Deserialize<T>(jsonReader);
        }

        private TextAsset LoadConfigFile<T>() {
            return ResourceLoader.LoadResource<TextAsset>(GetConfigFilePath<T>());
        }
    }
}