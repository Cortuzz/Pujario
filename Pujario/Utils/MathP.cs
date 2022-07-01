using System.Runtime.InteropServices;

namespace Pujario.Utils
{
	public static class MathP
    {
	    /// <summary>Combined value of Perlin noises at different frequencies (octaves) in 3-dimensional space</summary>
	    /// <param name="x">X Perlin noise coordinate</param>
	    /// <param name="y">Y Perlin noise coordinate (pass 0 for 1D noise)</param>
	    /// <param name="z">Z Perlin noise coordinate (pass 0 for 2D noise)</param>
	    /// <param name="octaves">The number of Perlin noises of different frequencies (octaves)</param>
	    /// <param name="persistence">Indicating an increase in the influence of higher frequency octaves</param>
	    /// <returns>Combined value of Perlin noises in an unlimited range</returns>
	    [DllImport("MathLibrary.dll")]
	    public static extern double OctavePerlin(double x, double y, double z, int octaves, double persistence);

	    /// <summary>Perlin noise in 3-dimensional space</summary>
	    /// <param name="x">X Perlin noise coordinate</param>
	    /// <param name="y">Y Perlin noise coordinate (pass 0 for 1D noise)</param>
	    /// <param name="z">Z Perlin noise coordinate (pass 0 for 2D noise)</param>
	    /// <returns>Perlin noise value in range between 0 and 1</returns>
	    [DllImport("MathLibrary.dll")]
	    public static extern double Perlin(double x, double y, double z);

	    /// <summary>Linear interpolation</summary>
	    /// <param name="start">First point</param>
	    /// <param name="end">Second point</param>
	    /// <param name="ratio">Interpolation multiplier between 0 and 1</param>
	    /// <returns>The interpolated result between the two float values.</returns>
	    [DllImport("MathLibrary.dll")]
	    public static extern double Lerp(double start, double end, double ratio);

	    /// <summary>Fade function for Perlin noise</summary>
	    /// <param name="t">Value for fade function</param>
	    /// <returns>Fade value in t</returns>
	    [DllImport("MathLibrary.dll")]
	    public static extern double Fade(double t);
    }
}