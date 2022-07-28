using System;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Pujario.Core.Input
{
    public partial class InputManager
    {
        private readonly partial struct TypeOrderedHelper
        {
            public readonly bool[] IsUpdateable;
            private readonly Type[] _typeOrder;

            public TypeOrderedHelper(Type[] typeOrder)
            {
                _typeOrder = typeOrder;
                IsUpdateable = new bool[typeOrder.Length + 1];
                IsUpdateable[0] = false;
                for (int i = 1; i < _typeOrder.Length; i++)
                {
                    IsUpdateable[i] = typeOrder[i - 1].GetInterfaces()
                        .FirstOrDefault(type => type == typeof(IUpdateable)) != null;
                }
            }

            public int TypeIndexOf(Delegate handler)
            {
                var t = handler.Target?.GetType();
                for (int i = 0; i < _typeOrder.Length; ++i)
                {
                    if (_typeOrder[i] == t) return i;
                }

                if (t != null)
                    for (int i = 0; i < _typeOrder.Length; ++i)
                    {
                        if (t.IsSubclassOf(_typeOrder[i])) return i;
                    }

                return -1;
            }
        }
    }
}