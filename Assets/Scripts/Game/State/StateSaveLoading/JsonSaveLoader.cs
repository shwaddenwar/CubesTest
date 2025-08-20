using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace Game.State.StateSaveLoading {
    public class JsonSaveLoader : BaseSaveLoader {
        private const string SaveFileName = "save.json";
        private string SavePath => Application.persistentDataPath;
        private readonly FileInfo _saveFile;
        private readonly JsonSerializer _serializer;

        public JsonSaveLoader() {
            _saveFile = new FileInfo(Path.Combine(SavePath, SaveFileName));
            _serializer = new JsonSerializer() {
                TypeNameHandling = TypeNameHandling.Auto,
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore
            };
        }

        public override void SaveState(GameState state) {
            try {
                using var fileStream = TryGetSaveFile(FileAccess.Write);
                using var streamWriter = new StreamWriter(fileStream);
                using var jsonStreamWriter = new JsonTextWriter(streamWriter);
                _serializer.Serialize(jsonStreamWriter, state);
                jsonStreamWriter.Flush();
            }
            catch (Exception e) {
                Debug.LogWarning($"Couldn't create save: {e.Message}");
            }
        }

        private FileStream TryGetSaveFile(FileAccess accessType) {
            return _saveFile.Open(FileMode.OpenOrCreate, accessType);
        }

        public override GameState LoadState() {
            GameState state;
            try {
                using var fileStream = TryGetSaveFile(FileAccess.Read);
                using var streamReader = new StreamReader(fileStream);
                using var jsonTextReader = new JsonTextReader(streamReader);
                state = _serializer.Deserialize<GameState>(jsonTextReader);
            }
            catch (Exception e) {
                state = new GameState();
                Debug.LogWarning($"Couldn't load save, creating new state: {e.Message}");
            }

            if (state is null) {
                state = new GameState();
            }

            return state;
        }

        public override void ResetState() {
            try {
                _saveFile.Delete();
            }
            catch (Exception e) {
                Debug.LogWarning($"Couldn't delete save: {e.Message}");
            }
        }
    }
}