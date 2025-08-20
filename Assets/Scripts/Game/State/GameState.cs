using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Game.State {
    public class GameState {
        public float StartX;
        public float StartY;
        public List<CubeState> PlacedCubes = new();

        [JsonIgnore]
        public Vector2 StartPosition {
            get => new(StartX, StartY);
            set {
                StartX = value.x;
                StartY = value.y;
            }
        }
    }
}