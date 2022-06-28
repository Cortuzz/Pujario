using Pujario.Exceptions.NoiseGenerator;
using Pujario.NoiseGeneration.Interfaces;
using Pujario.Utils;

namespace Pujario.NoiseGeneration
{
    public class NoiseGenerator1D : INoiseGenerator, INoiseGeneratorHorizontal
    {
        public sbyte Dimensions => 1;
        public int Seed { get; }
        public int Width { get; }
        public float HorizontalFrequency { get; set; }
        public float Persistence { get; set; }
        public int Octaves { get; set; }

        public NoiseGenerator1D(int seed, int width)
        {
            Seed = seed;
            Width = width;
        }
        
        /// <summary>Generating <see cref="Dimensions">1 dimensional</see> noise</summary>
        /// <exception cref="NoiseGeneratorException">throwing when properties
        /// <see cref="Persistence">Persistence</see>, <see cref="Octaves">Octaves</see> or
        /// <see cref="HorizontalFrequency">Horizontal Frequency</see> was not set.
        /// </exception>
        /// <returns>One dimensional array of float noise values between 0 and 1
        /// with size <see cref="Width">Width</see>.</returns>
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