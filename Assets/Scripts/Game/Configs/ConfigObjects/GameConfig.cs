using System;
using System.Collections.Generic;

namespace Game.Configs.ConfigObjects {
    [Config("GameConfig")]
    [Serializable]
    public class GameConfig {
        public List<string> CubeColors = new();
    }
}