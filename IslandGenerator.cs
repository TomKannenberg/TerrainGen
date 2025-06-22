// FILE: IslandGenerator.cs
using UnityEngine;
using System.Linq;

public class IslandGenerator {
	private readonly GeneratorSettings settings;
	private readonly int seed;
	public int GetSeed() => seed;

	public IslandGenerator(GeneratorSettings settings) {
		this.settings = settings;
		this.seed = (settings.seed == 0) ? Random.Range(-10000, 10000) : settings.seed;
	}

	// --- FIX ---
	// The method now accepts the resolution as a parameter, ensuring it matches the terrain's actual resolution.
	public void GenerateMaps(int resolution, out float[,] heightMap, out float[,,] splatMap) {
		// -----------
		heightMap = new float[resolution, resolution];
		Vector2[] octaveOffsets = GenerateOctaveOffsets(settings.noiseSettings.octaves);

		// Pass 1: Generate HeightMap
		for (int y = 0; y < resolution; y++) {
			for (int x = 0; x < resolution; x++) {
				Vector2 coord = new Vector2((float)x / resolution, (float)y / resolution);
				float noiseValue = Fbm(coord, octaveOffsets);
				float distFromCenter = Mathf.Clamp01(Vector2.Distance(coord, Vector2.one * 0.5f) * 2);
				float islandShape = settings.islandShapeCurve.Evaluate(distFromCenter);
				float finalHeight = noiseValue * islandShape * settings.heightMultiplier;
				heightMap[y, x] = Mathf.Clamp01(finalHeight);
			}
		}

		// Pass 2: Generate SplatMap
		int numBiomes = settings.biomes.Count;
		// The splatmap also uses the passed-in resolution, which is correct as the manager sets both resolutions from the same source.
		splatMap = new float[resolution, resolution, numBiomes];
		if (numBiomes == 0) return;

		for (int y = 0; y < resolution; y++) {
			for (int x = 0; x < resolution; x++) {
				float height = heightMap[y, x];
				float slope = GetSlope(heightMap, x, y);
				float[] biomeStrengths = new float[numBiomes];
				float totalStrength = 0;

				for (int i = 0; i < numBiomes; i++) {
					BiomeSettings biome = settings.biomes[i];
					float strength = CalculateBiomeStrength(height, slope, biome);
					biomeStrengths[i] = strength;
					totalStrength += strength;
				}

				for (int i = 0; i < numBiomes; i++) {
					splatMap[y, x, i] = (totalStrength > 0) ? biomeStrengths[i] / totalStrength : (i == 0 ? 1 : 0);
				}
			}
		}
	}
	// ... (The rest of the IslandGenerator.cs file remains unchanged)
	private float CalculateBiomeStrength(float height, float slope, BiomeSettings biome) {
		float heightT = 1 - Mathf.Abs(Mathf.InverseLerp(biome.heightRange.x, biome.heightRange.y, height) * 2 - 1);
		float slopeT = 1 - Mathf.Abs(Mathf.InverseLerp(biome.slopeRange.x, biome.slopeRange.y, slope) * 2 - 1);
		return heightT * slopeT;
	}

	private float GetSlope(float[,] heightMap, int x, int y) {
		int resolution = heightMap.GetLength(0);
		float terrainHeight = settings.terrainSize.y;
		float h_x0 = heightMap[y, Mathf.Max(0, x - 1)] * terrainHeight;
		float h_x1 = heightMap[y, Mathf.Min(resolution - 1, x + 1)] * terrainHeight;
		float h_y0 = heightMap[Mathf.Max(0, y - 1), x] * terrainHeight;
		float h_y1 = heightMap[Mathf.Min(resolution - 1, y + 1), x] * terrainHeight;
		float dx = h_x1 - h_x0;
		float dy = h_y1 - h_y0;
		return Mathf.Atan(Mathf.Sqrt(dx * dx + dy * dy)) * Mathf.Rad2Deg;
	}

	private float Fbm(Vector2 point, Vector2[] octaveOffsets) {
		float total = 0, frequency = settings.noiseSettings.scale, amplitude = 1, maxValue = 0;
		for (int i = 0; i < settings.noiseSettings.octaves; i++) {
			float sampleX = (point.x * frequency) + octaveOffsets[i].x;
			float sampleY = (point.y * frequency) + octaveOffsets[i].y;
			total += Mathf.PerlinNoise(sampleX, sampleY) * amplitude;
			maxValue += amplitude;
			amplitude *= settings.noiseSettings.persistence;
			frequency *= settings.noiseSettings.lacunarity;
		}
		return total / maxValue;
	}//

	private Vector2[] GenerateOctaveOffsets(int octaves) {
		System.Random prng = new System.Random(seed);
		Vector2[] octaveOffsets = new Vector2[octaves];
		for (int i = 0; i < octaves; i++) {
			octaveOffsets[i] = new Vector2(prng.Next(-100000, 100000), prng.Next(-100000, 100000));
		}
		return octaveOffsets;
	}
}