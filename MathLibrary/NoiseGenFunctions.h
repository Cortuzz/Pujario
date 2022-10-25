#pragma once
#define DLLEXPORT extern "C" __declspec(dllexport)

DLLEXPORT double OctavePerlin(double x, double y, double z, int octaves, double persistence);

DLLEXPORT double Perlin(double x, double y, double z);

DLLEXPORT double Lerp(double start, double end, double ratio);

DLLEXPORT double Fade(double t);

DLLEXPORT double Gradient(int hash, double x, double y, double z);