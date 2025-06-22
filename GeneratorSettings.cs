// FILE: GeneratorSettings.cs
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "New Generator Settings", menuName = "Island Generation/Generator Settings")]
public class GeneratorSettings : ScriptableObject {
	[Header("Core Properties")]
	public int seed = 0;
	[Header("Terrain Shape & Size")]
	public AnimationCurve islandShapeCurve;
	public int heightmapResolution = 513;
	public float heightMultiplier = 1f;
	public Vector3 terrainSize = new Vector3(1000, 250, 1000);
	[Header("Noise Properties")]
	public NoiseSettings noiseSettings;
	[Header("Biomes")]
	public List<BiomeSettings> biomes;

	public TerrainLayer[] GenerateTerrainLayers() {
		if (biomes == null) return new TerrainLayer[0];
		return biomes.Where(b => b.layer != null).Select(b => b.layer).ToArray();
	}
}

[System.Serializable]
public class NoiseSettings {
	[Range(1, 10)] public int octaves = 7;
	[Range(1.1f, 5f)] public float lacunarity = 2.1f;
	[Range(0f, 1f)] public float persistence = 0.45f;
	[Range(1f, 200f)] public float scale = 25f;
}

[System.Serializable]
public class BiomeSettings {
	public string name = "New Biome";
	public TerrainLayer layer;
	[Vector2Range(0f, 1f)] public Vector2 heightRange = new Vector2(0.2f, 0.5f);
	[Vector2Range(0f, 90f)] public Vector2 slopeRange = new Vector2(0f, 30f);
}

public class Vector2RangeAttribute : PropertyAttribute {
	public readonly float min;
	public readonly float max;
	public Vector2RangeAttribute(float min, float max) { this.min = min; this.max = max; }
}