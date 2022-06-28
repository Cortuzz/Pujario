namespace Pujario.NoiseGeneration.Interfaces
{
    public interface INoiseGeneratorConglomerate
    {
        /// <summary>Noise texture depth</summary>
        public int Depth { get; }
        
        /// <summary>Noise conglomerate frequency</summary>
        public float ConglomerateFrequency { get; set; }
    }
}