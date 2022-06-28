namespace Pujario.NoiseGeneration.Interfaces
{
    public interface INoiseGeneratorVertical
    {
        /// <summary>Noise texture height</summary>
        public int Height { get; }
        
        /// <summary>Noise vertical frequency</summary>
        public float VerticalFrequency { get; set; }
    }
}