#pragma once
// #define DLLEXPORT extern "C" __declspec(dllexport)

__declspec(dllexport) double OctavePerlin(double x, double y, double z, int octaves, double persistence);

__declspec(dllexport) double Perlin(double x, double y, double z);

__declspec(dllexport) double Lerp(double start, double end, double ratio);

__declspec(dllexport) double Fade(double t);

__declspec(dllexport) double Gradient(int hash, double x, double y, double z);