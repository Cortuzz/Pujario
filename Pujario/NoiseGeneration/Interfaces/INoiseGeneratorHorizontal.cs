namespace Pujario.NoiseGeneration.Interfaces
{
    public interface INoiseGeneratorHorizontal
    {
        /// <summary>Noise texture width</summary>
        public int Width { get; }
        
        /// <summary>Noise horizontal frequency</summary>
        public float HorizontalFrequency { get; set; }
    }
}