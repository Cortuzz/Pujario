using System.Collections.Generic;

namespace Pujario.Core.WorldPresentation
{
    /// <summary>
    /// An object that can select <see cref="WorldMapping.Chunk"/>s in <see cref="WorldMapping"/> to future updating 
    /// </summary>
    public interface ITickBeacon // : IEnumerable<WorldMapping.Chunk> ?
    {
        public bool ForDrawing { get; }
        public bool ForUpdating { get; }
        List<WorldMapping.Chunk> Select(WorldMapping world);
    }
}