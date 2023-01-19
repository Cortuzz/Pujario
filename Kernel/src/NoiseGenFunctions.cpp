#include "NoiseGenFunctions.hpp"

#include <cstdint>

const uint8_t permutation[] = {
    151, 160, 137, 91, 90, 15,
    131, 13, 201, 95, 96, 53, 194, 233, 7, 225, 140, 36, 103, 30, 69, 142, 8, 99, 37, 240, 21, 10, 23,
    190, 6, 148, 247, 120, 234, 75, 0, 26, 197, 62, 94, 252, 219, 203, 117, 35, 11, 32, 57, 177, 33,
    88, 237, 149, 56, 87, 174, 20, 125, 136, 171, 168, 68, 175, 74, 165, 71, 134, 139, 48, 27, 166,
    77, 146, 158, 231, 83, 111, 229, 122, 60, 211, 133, 230, 220, 105, 92, 41, 55, 46, 245, 40, 244,
    102, 143, 54, 65, 25, 63, 161, 1, 216, 80, 73, 209, 76, 132, 187, 208, 89, 18, 169, 200, 196,
    135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173, 186, 3, 64, 52, 217, 226, 250, 124, 123,
    5, 202, 38, 147, 118, 126, 255, 82, 85, 212, 207, 206, 59, 227, 47, 16, 58, 17, 182, 189, 28, 42,
    223, 183, 170, 213, 119, 248, 152, 2, 44, 154, 163, 70, 221, 153, 101, 155, 167, 43, 172, 9,
    129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224, 232, 178, 185, 112, 104, 218, 246, 97, 228,
    251, 34, 242, 193, 238, 210, 144, 12, 191, 179, 162, 241, 81, 51, 145, 235, 249, 14, 239, 107,
    49, 192, 214, 31, 181, 199, 106, 157, 184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254,
    138, 236, 205, 93, 222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180,

    151, 160, 137, 91, 90, 15,
    131, 13, 201, 95, 96, 53, 194, 233, 7, 225, 140, 36, 103, 30, 69, 142, 8, 99, 37, 240, 21, 10, 23,
    190, 6, 148, 247, 120, 234, 75, 0, 26, 197, 62, 94, 252, 219, 203, 117, 35, 11, 32, 57, 177, 33,
    88, 237, 149, 56, 87, 174, 20, 125, 136, 171, 168, 68, 175, 74, 165, 71, 134, 139, 48, 27, 166,
    77, 146, 158, 231, 83, 111, 229, 122, 60, 211, 133, 230, 220, 105, 92, 41, 55, 46, 245, 40, 244,
    102, 143, 54, 65, 25, 63, 161, 1, 216, 80, 73, 209, 76, 132, 187, 208, 89, 18, 169, 200, 196,
    135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173, 186, 3, 64, 52, 217, 226, 250, 124, 123,
    5, 202, 38, 147, 118, 126, 255, 82, 85, 212, 207, 206, 59, 227, 47, 16, 58, 17, 182, 189, 28, 42,
    223, 183, 170, 213, 119, 248, 152, 2, 44, 154, 163, 70, 221, 153, 101, 155, 167, 43, 172, 9,
    129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224, 232, 178, 185, 112, 104, 218, 246, 97, 228,
    251, 34, 242, 193, 238, 210, 144, 12, 191, 179, 162, 241, 81, 51, 145, 235, 249, 14, 239, 107,
    49, 192, 214, 31, 181, 199, 106, 157, 184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254,
    138, 236, 205, 93, 222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180
};


/// <summary>Combined value of Perlin noises at different frequencies (octaves) in 3-dimensional space</summary>
/// <param name="x">X Perlin noise coordinate</param>
/// <param name="y">Y Perlin noise coordinate (pass 0 for 1D noise)</param>
/// <param name="z">Z Perlin noise coordinate (pass 0 for 2D noise)</param>
/// <param name="octaves">The number of Perlin noises of different frequencies (octaves)</param>
/// <param name="persistence">Indicating an increase in the influence of higher frequency octaves</param>
/// <returns>Combined value of Perlin noises in an unlimited range</returns>
double OctavePerlin(double x, double y, double z, int octaves, double persistence)
{
    auto total = 0.0;
    auto frequency = 1.0;
    auto amplitude = 1.0;

    for (auto i = 0; i < octaves; ++i)
    {
        total += Perlin(x * frequency, y * frequency, z * frequency) * amplitude;

        amplitude *= persistence;
        frequency *= 2;
    }

    return total;
}

/// <summary>Perlin noise in 3-dimensional space</summary>
/// <param name="x">X Perlin noise coordinate</param>
/// <param name="y">Y Perlin noise coordinate (pass 0 for 1D noise)</param>
/// <param name="z">Z Perlin noise coordinate (pass 0 for 2D noise)</param>
/// <returns>Perlin noise value in range between 0 and 1</returns>
double Perlin(double x, double y, double z)
{
    const int xi = static_cast<int>(x) & 255; // Calculate the "unit cube" that the point asked will be located in
    const int yi = static_cast<int>(y) & 255; // The left bound is ( |_x_|,|_y_|,|_z_| ) and the right bound is that
    const int zi = static_cast<int>(z) & 255; // plus 1.  Next we calculate the location (from 0.0 to 1.0) in that cube.

	double xf = x - static_cast<int>(x);								// We also fade the location to smooth the result.
	const double yf = y - static_cast<int>(y);
	const double zf = z - static_cast<int>(z);

	const double u = Fade(xf);
	const double v = Fade(yf);
	const double w = Fade(zf);
	
	int a  = permutation[xi] + yi;							// This here is Perlin's hash function.  We take our x value (remember,
	int aa = permutation[a] + zi;								// between 0 and 255) and get a random value (from our permutation[] array above) between
	int ab = permutation[a + 1] + zi;							// 0 and 255.  We then add y to it and plug that into permutation[], and add z to that.
	int b  = permutation[xi + 1] + yi;						// Then, we get another random value by adding 1 to that and putting it into permutation[]
	int ba = permutation[b] + zi;								// and add z to it.  We do the whole thing over again starting with x+1.  Later
	int bb = permutation[b + 1] + zi;							// we plug aa, ab, ba, and bb back into permutation[] along with their +1's to get another set.
														// in the end we have 8 values between 0 and 255 - one for each vertex on the unit cube.
														// These are all interpolated together using u, v, and w below.
															
	double x1 = Lerp(Gradient(permutation[aa], xf, yf, zf), Gradient(permutation[ba], xf - 1, yf, zf), u);
	double x2 = Lerp(Gradient(permutation[ab], xf, yf - 1, zf), Gradient (permutation[bb], xf - 1, yf-1, zf), u);
	const double y1 = Lerp(x1, x2, v);

    x1 = Lerp(Gradient(permutation[aa + 1], xf, yf, zf - 1), Gradient(permutation[ba + 1], xf - 1, yf, zf - 1), u);
    x2 = Lerp(Gradient(permutation[ab + 1], xf, yf - 1, zf - 1), Gradient(permutation[bb + 1], xf - 1, yf - 1, zf - 1),
              u);
    const double y2 = Lerp(x1, x2, v);

    return (Lerp(y1, y2, w) + 1) / 2; // For convenience we bound it to 0 - 1 (theoretical min/max before is -1 - 1)
}

/// <summary>Linear interpolation</summary>
/// <param name="start">First point</param>
/// <param name="end">Second point</param>
/// <param name="ratio">Interpolation multiplier between 0 and 1</param>
/// <returns>The interpolated result between the two float values.</returns>
double Lerp(double start, double end, double ratio)
{
    // TODO : Clamp ratio to range [0 - 1]
    return start + ratio * (end - start);
}

/// <summary>Fade function for Perlin noise</summary>
/// <param name="t">Value for fade function</param>
/// <returns>Fade value in t</returns>
double Fade(double t)
{
    return t * t * t * (t * (t * 6 - 15) + 10); // 6t^5 - 15t^4 + 10t^3
}

double Gradient(int hash, double x, double y, double z)
{
    const int h = hash & 15; // Take the hashed value and take the first 4 bits of it (15 == 0b1111)
    const double u = h < 8 /* 0b1000 */ ? x : y;

    double v;

    if (h < 4 /* 0b0100 */)
        v = y; // If the first and second significant bits are 0 set v = y
    else if (h == 12 /* 0b1100 */ || h == 14 /* 0b1110*/)
        v = x; // If the first and second significant bits are 1 set v = x
    else
        v = z; // If the first and second significant bits are not equal (0/1, 1/0) set v = z

    return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
    // Use the last 2 bits to decide if u and v are positive or negative.  Then return their addition.
}