using Pujario.NoiseGeneration.Interfaces;
using Pujario.Utils;

namespace Pujario.NoiseGeneration
{
    public class NoiseGenerator2D : INoiseGenerator, INoiseGeneratorHorizontal, INoiseGeneratorVertical
    {
        public int Seed { get; set; }
        public int Width { get; }
        public int Height { get; }
        public float HorizontalFrequency { get; set; }
        public float VerticalFrequency { get; set; }
        public float Persistence { get; set; }
        public int Octaves { get; set; }


        public NoiseGenerator2D(int seed, int width, int height)
        {
            Seed = seed;
            Width = width;
            Height = height;
        }
        
        public float[] Generate()
        {
            var noiseMap = new float[Width * Height];
            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    var xCoord = x * HorizontalFrequency + Seed;
                    var yCoord = y * VerticalFrequency + Seed;
                    
                    noiseMap[y * Width + x] = (float)MathP.OctavePerlin(xCoord, yCoord, 0, Octaves, Persistence);
                }
            }

            return noiseMap;
        }
    }
}