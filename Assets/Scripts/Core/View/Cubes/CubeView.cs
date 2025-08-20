using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Core.View.Cubes {
    public class CubeView : MonoBehaviour {
        private static readonly string CubeViewPath = "Sprites/Cubes/{0}.png";

        [SerializeField] private Image cubeImage;

        public void SetData(string cubeSprite) {
            ResourceLoader.LoadResourceAsync<Sprite>(string.Format(CubeViewPath, cubeSprite), SetSprite);
        }

        private void SetSprite(Sprite s) {
            cubeImage.sprite = s;
        }
    }
}