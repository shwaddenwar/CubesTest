using Game.Configs.ConfigObjects;
using UnityEngine;

namespace Game.Configs.SO {
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Configs/GameConfig")]
    public class SOGameConfig : ScriptableObject, ISOConfigProvider<GameConfig> {
        [SerializeField] private GameConfig _gameConfig;

        GameConfig ISOConfigProvider<GameConfig>.GetConfigObject() {
            return _gameConfig;
        }
    }
}