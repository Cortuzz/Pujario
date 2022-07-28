using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Microsoft.Xna.Framework;

namespace Pujario.Utils
{
    public class UpdateOrderComparer : IComparer<IUpdateable>
    {
        private static readonly Comparer<int> Comparer = Comparer<int>.Default;

        public static Comparison<IUpdateable>
            Comparison = (x, y) => Comparer.Compare(x?.UpdateOrder ?? 0, y?.UpdateOrder ?? 0);

        [Pure]
        public int Compare(IUpdateable x, IUpdateable y) => Comparer.Compare(x?.UpdateOrder ?? 0, y?.UpdateOrder ?? 0);

        protected UpdateOrderComparer()
        {
        }

        public static UpdateOrderComparer Instance { get; } = new UpdateOrderComparer();
    }
}