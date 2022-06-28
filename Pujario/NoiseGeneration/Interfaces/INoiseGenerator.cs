namespace Pujario.NoiseGeneration.Interfaces
{
    public interface INoiseGenerator
    {
        public int Seed { get; set; }
        public float Persistence { get; set; }
        public int Octaves { get; set; }

        public float[] Generate();
    }
}