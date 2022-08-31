using System;
using System.Collections.Generic;
using Pujario.Core.Components;

namespace Pujario.Utils
{
    static public class ComponentHelper
    {
        public static bool TryGetTransformableParent(IComponent component, out ITransformable parent)
        {
            var cur = component;
            while (cur.ParentComponent.TryGetTarget(out cur))
            {
                if ((parent = component as ITransformable) == null) continue;
                return true;
            }
            if (component.Owner.TryGetTarget(out var componentProvider) 
                && (parent = component as ITransformable) != null) return true;
            else parent = null;
            return false;
        }
    }
}
