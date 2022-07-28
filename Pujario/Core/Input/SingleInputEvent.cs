using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Pujario.Core.Input
{
    public partial class InputManager
    {
        private partial class SingleInputEvent<TState>
        {
            private TState _prevSate;
            private readonly Func<TState, TState, bool> _equalityFunc;
            private readonly Action<TState>[] _handlers;
            private readonly TypeOrderedHelper _helper;

            public SingleInputEvent(TState initState, Func<TState, TState, bool> equalityFunc, Type[] typeOrder)
            {
                _prevSate = initState;
                _equalityFunc = equalityFunc;
                _handlers = new Action<TState>[typeOrder.Length + 2];
                _helper = new TypeOrderedHelper(typeOrder);
            }

            public void HandleInput(TState state)
            {
                if (_equalityFunc(state, _prevSate)) return;
                _prevSate = state;
                _handlers[0]?.Invoke(state);
                int lm1 = _handlers.Length - 1;
                for (int i = 1; i < lm1; ++i)
                {
                    if (_helper.IsUpdateable[i - 1] && !(_handlers[i].Target as IUpdateable)!.Enabled) continue;
                    _handlers[i]?.Invoke(state);
                }

                _handlers[lm1]?.Invoke(state);
            }

            public void Add(in Action<TState> handler)
            {
                if (handler.Target == null) _handlers[0] += handler;
                else
                {
                    int index = _helper.TypeIndexOf(handler);
                    index = index < 0 ? _handlers.Length - 1 : index + 1;
                    _handlers[index] += handler;
                }
            }

            public void Remove(in Action<TState> handler)
            {
                if (handler.Target == null) _handlers[0] -= handler;
                else
                {
                    int index = _helper.TypeIndexOf(handler);
                    index = index < 0 ? _handlers.Length - 1 : index + 1;
                    _handlers[index] -= handler;
                }
            }
        }
    }
}