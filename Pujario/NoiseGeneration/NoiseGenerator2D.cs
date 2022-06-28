using Pujario.Exceptions.NoiseGenerator;
using Pujario.NoiseGeneration.Interfaces;
using Pujario.Utils;

namespace Pujario.NoiseGeneration
{
    public class NoiseGenerator2D : INoiseGenerator, INoiseGeneratorHorizontal, INoiseGeneratorVertical
    {
        public sbyte Dimensions => 2;
        public int Seed { get; }
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
        
        /// <summary>Generating <see cref="Dimensions">2 dimensional</see> noise</summary>
        /// <exception cref="NoiseGeneratorException">throwing when properties
        /// <see cref="Persistence">Persistence</see>, <see cref="Octaves">Octaves</see>,
        /// <see cref="HorizontalFrequency">Horizontal Frequency</see> or
        /// <see cref="VerticalFrequency">Vertical Frequency</see> was not set.
        /// </exception>
        /// <returns>One dimensional array of float noise values between 0 and 1
        /// with size <see cref="Width">Width</see> * <see cref="Height">Height</see>.</returns>
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