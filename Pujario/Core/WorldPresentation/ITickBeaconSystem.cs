using System.Collections.Generic;

namespace Pujario.Core.WorldPresentation
{
    public enum TickBeaconMode : byte
    {
        Draw = 0b00000001,
        Update = 0b00000010,
        Any = 0b00000011
    }
    
    /// <summary>
    /// An object that can select <see cref="WorldMapping.Chunk"/>s in <see cref="WorldMapping"/> 
    /// </summary>
    public interface ITickBeacon
    {
        TickBeaconMode Mode { get; }
        IEnumerator<WorldMapping.Chunk> GetSelector(WorldMapping world);
    }

    /// <summary>
    /// An cascade of <see cref="ITickBeacon"/>, which presents active area for updating in <see cref="WorldMapping"/>
    /// Can provide order/ control 
    /// </summary>
    public interface ITickBeaconSystem
    {
        public void UseBeacon(ITickBeacon beacon);
        public void DisuseBeacon(ITickBeacon beacon);

        public IEnumerator<IActor> GetDrawEnumerator(WorldMapping world);
        public IEnumerator<IActor> GetUpdateEnumerator(WorldMapping world);
    }
}