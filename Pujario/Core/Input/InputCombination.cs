using System;
using System.Collections.Immutable;
using Microsoft.Xna.Framework.Input;

namespace Pujario.Core.Input
{
    /// <summary>Presents any discrete input combination</summary>
    public readonly struct InputCombination : IEquatable<InputCombination>
    {
        private readonly int _hashCode;

        public readonly ImmutableArray<string> KeyMappingAliases;
        public readonly ImmutableArray<Keys> KeyboardInput;
        public readonly ImmutableArray<Buttons> GamepadInput;

        public InputCombination(
            string[] keyMappingAliases = null,
            Keys[] keys = null,
            Buttons[] buttons = null)
        {
            KeyMappingAliases = keyMappingAliases != null
                ? ImmutableArray.Create(keyMappingAliases)
                : ImmutableArray<string>.Empty;
            KeyboardInput = keys != null ? ImmutableArray.Create(keys) : ImmutableArray<Keys>.Empty;
            GamepadInput = buttons != null ? ImmutableArray.Create(buttons) : ImmutableArray<Buttons>.Empty;
            _hashCode = default;
            _hashCode = GetHashCode();
        }

        public bool Equals(InputCombination other) => _hashCode == other._hashCode;

        public override bool Equals(object obj) => obj is InputCombination other && Equals(other);

        public override int GetHashCode()
        {
            var code = new HashCode();
            if (KeyMappingAliases.IsEmpty) code.Add(int.MaxValue << 1);
            else
            {
                foreach (var alias in KeyMappingAliases) code.Add(alias.GetHashCode() << 1);
            }

            if (KeyboardInput.IsEmpty) code.Add(int.MaxValue << 2);
            else
            {
                foreach (var key in KeyboardInput) code.Add((int)key << 2);
            }

            if (GamepadInput.IsEmpty) code.Add(int.MaxValue << 3);
            else
            {
                foreach (var button in GamepadInput) code.Add((int)button << 3);
            }

            return code.ToHashCode();
        }
    }
}