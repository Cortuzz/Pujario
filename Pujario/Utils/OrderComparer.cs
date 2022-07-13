using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Pujario.Utils
{
    public class UpdateOrderComparer : IComparer<IUpdateable>
    {
        public int Compare(IUpdateable x, IUpdateable y)
        {
            if (x == null)
                if (y == null) return 0;
                else return -1;

            if (y == null) return 1;
            if (x.UpdateOrder == y.UpdateOrder) return 0;

            return x.UpdateOrder > y.UpdateOrder ? 1 : -1;

            // or that
            /*return y == null
                ? (x == null ? 0 : 1)
                : x == null
                    ? -1
                    : (x.UpdateOrder < y.UpdateOrder ? -1 : 1);*/
        }

        protected UpdateOrderComparer()
        {
        }

        public static UpdateOrderComparer Instance { get; } = new UpdateOrderComparer();
    }
}