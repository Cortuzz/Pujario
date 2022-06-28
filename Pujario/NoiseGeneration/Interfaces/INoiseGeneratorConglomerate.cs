namespace Pujario.NoiseGeneration.Interfaces
{
    public interface INoiseGeneratorConglomerate
    {
        public int Depth { get; }
        public float ConglomerateFrequency { get; set; }
    }
}