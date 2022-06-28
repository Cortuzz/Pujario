using System;

namespace Pujario.Exceptions.NoiseGenerator
{
    public class NoiseGeneratorException : Exception
    {
        public NoiseGeneratorException(string message = "") : base(message)
        {
            
        }
    }
}