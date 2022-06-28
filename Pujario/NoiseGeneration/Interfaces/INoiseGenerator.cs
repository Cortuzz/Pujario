using Pujario.Exceptions.NoiseGenerator;

namespace Pujario.NoiseGeneration.Interfaces
{
    public interface INoiseGenerator
    {
        /// <summary>Number of noise dimensions</summary>
        public sbyte Dimensions { get; }
        
        /// <summary>Noise seed for random generation</summary>
        public int Seed { get; }
        
        /// <summary>Amplitude multiplier for each octave</summary>
        /// <exception cref="NoiseParameterException">throwing when property set to value less than or equals to 0</exception>
        public float Persistence { get; set; }
        
        /// <summary>Number of octaves of noise</summary>
        /// <exception cref="NoiseParameterException">throwing when property set to value less than 1</exception>
        public int Octaves { get; set; }

        /// <summary>Generating <see cref="Dimensions">N dimensional</see> noise</summary>
        /// <exception cref="NoiseGeneratorException">throwing when properties
        /// <see cref="Persistence">Persistence</see> or <see cref="Octaves">Octaves</see> was not set.
        /// </exception>
        /// <returns>One Dimensional array of float noise values between 0 and 1.</returns>
        public float[] Generate();
    }
}