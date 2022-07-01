#pragma once


extern "C" __declspec(dllexport) double OctavePerlin(double x, double y, double z, int octaves, double persistence);

extern "C" __declspec(dllexport)double Perlin(double x, double y, double z);

extern "C" __declspec(dllexport)double Lerp(double start, double end, double ratio);

extern "C" __declspec(dllexport)double Fade(double t);

extern "C" __declspec(dllexport)double Gradient(int hash, double x, double y, double z);