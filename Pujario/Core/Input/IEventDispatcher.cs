using System;

namespace Pujario.Core
{
    public interface IEventDispatcher<in TDelegate, in TKey> where TDelegate : Delegate where TKey : notnull
    {
        public void Subscribe(TKey on, TDelegate handler);
        public void Unsubscribe(TKey on, TDelegate handler);
    }
}