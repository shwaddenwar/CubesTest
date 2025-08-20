using System;

namespace Game.Configs {
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class ConfigAttribute : Attribute {
        public readonly string Name;

        public ConfigAttribute(string name) {
            Name = name;
        }
    }
}