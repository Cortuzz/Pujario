using Pujario.NoiseGeneration.Interfaces;
using Pujario.Utils;

namespace Pujario.NoiseGeneration
{
    public class NoiseGenerator1D : INoiseGenerator, INoiseGeneratorHorizontal
    {
        public int Seed { get; set; }
        public int Width { get; }
        public float HorizontalFrequency { get; set; }
        public float Persistence { get; set; }
        public int Octaves { get; set; }
        
        
        public NoiseGenerator1D(int seed, int width)
        {
            Seed = seed;
            Width = width;
        }
        
        public float[] Generate()
        {
            var noiseMap = new float[Width];
            for (var x = 0; x < Width; x++)
            {
                var xCoord = x * HorizontalFrequency + Seed;
                noiseMap[x] = (float)MathP.OctavePerlin(xCoord, 0, 0, Octaves, Persistence);
            }

            return noiseMap;
        }
    }
}