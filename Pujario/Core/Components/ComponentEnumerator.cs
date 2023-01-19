using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Pujario.Core.Components
{
    /// <summary>
    /// An DFS enumeration threw component tree
    /// </summary>
    public class ComponentEnumerator : IEnumerator<IComponent>
    {
        private IComponent _root;
        private IComponent _currant;
        private Stack<IComponent> _stack;

        public IComponent Current => _currant;
        object IEnumerator.Current => _currant;

        public ComponentEnumerator([NotNull] IComponent root)
        {
            _root = root;
            _stack = new Stack<IComponent>();
            _stack.Push(root);
        }

        ComponentEnumerator() => Dispose();

        public bool MoveNext()
        {
            if (!_stack.TryPop(out _currant)) return false;
            if (_currant != null) 
                foreach (var item in _currant.ChildComponents) 
                        _stack.Push(item);
            return true;
        }

        public void Reset(IComponent newRoot)
        {
            _root = newRoot;
            Reset();
        }

        public void Reset()
        {
            _stack.Clear();
            _stack.Push(_root);
            _currant = null;
        }

        public void Dispose()
        {
            _currant = null;
            _stack.Clear();
            _stack = null;
            GC.SuppressFinalize(this);
        }
    }
}
