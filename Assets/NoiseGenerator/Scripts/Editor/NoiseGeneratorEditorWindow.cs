using UnityEngine;
using UnityEditor;
using Random = UnityEngine.Random;


public class NoiseGeneratorEditorWindow : EditorWindow
{
    PerlinGenerator noiseGenerator;
    bool isInitialized = false;


    [MenuItem("Window/Noise Generator")]
    public static void ShowWindow() => GetWindow<NoiseGeneratorEditorWindow>("Noise Generator");

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Noise Generator", EditorStyles.boldLabel);

        InitGenerator();

        DrawIntSlider(ref noiseGenerator.width, "Width", 0, 1920);
        DrawIntSlider(ref noiseGenerator.height, "Height", 0, 1080);
        DrawSlider(ref noiseGenerator.scale, "Scale", 0, 1000);

        if (GUILayout.Button("Generate Noise"))
            GenerateNoiseButton();

        if (GUILayout.Button("Save Texture"))
            noiseGenerator.SaveTexture();
    }
    void OnDestroy()
    {
        DestroyImmediate(noiseGenerator.noiseDisplay.gameObject);
    }

    void InitGenerator()
    {
        if (isInitialized) return;
        noiseGenerator = new PerlinGenerator();

        if (noiseGenerator == null)
        {
            EditorGUILayout.HelpBox("No PerlinGenerator found", MessageType.Warning);
            return;
        }

        isInitialized = true;
        noiseGenerator.DefineVariables();
    }

    void DrawSlider(ref float variable, string label, float min, float max)
    {
        variable = EditorGUILayout.Slider(label, variable, min, max);
    }

    void DrawIntSlider(ref int variable, string label, int min, int max)
    {
        variable = EditorGUILayout.IntSlider(label, variable, min, max);
    }
    void GenerateNoiseButton()
    {
        CheckNoiseDisplay();
        noiseGenerator.DefineVariables();
        noiseGenerator.GetNewNoise();
    }

    void CheckNoiseDisplay()
    {
        if (noiseGenerator.noiseDisplay != null) return;

        Renderer renderer;
        renderer = Resources.Load<Renderer>("Prefabs/NoiseDisplay");
        if (renderer == null)
        {
            EditorGUILayout.HelpBox("No noiseDisplay prefab found in resources", MessageType.Warning);
            return;
        }

        noiseGenerator.noiseDisplay = Instantiate(renderer);
    }
}


public class PerlinGenerator
{
    [Range(0, 1920)] public int width = 256;
    [Range(0, 1080)] public int height = 256;
    [Range(0, 100)] public float scale = 20f;
    [Range(0, 10000)] public float offsetX, offsetY;

    public Renderer noiseDisplay;
    Texture2D noiseTexture;



    public void DefineVariables()
    {
        noiseTexture = new Texture2D(width, height);
        offsetX = Random.Range(0, 10000);
        offsetY = Random.Range(0, 10000);
    }

    public void GetNewNoise() => UpdateRenderer();
    public void UpdateRenderer() => noiseDisplay.material.mainTexture = GenerateNoiseTexture();

    Texture2D GenerateNoiseTexture()
    {
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                Color color = GetColor(x, y);
                noiseTexture.SetPixel(x, y, color);
            }

        noiseTexture.Apply();
        return noiseTexture;
    }

    Color GetColor(int x, int y)
    {
        float perlinXCoord = (float)x / width * scale + offsetX;
        float perlinYCoord = (float)y / height * scale + offsetY;

        float sample = Mathf.PerlinNoise(perlinXCoord, perlinYCoord);

        return new Color(sample, sample, sample);
    }

    public void SaveTexture()
    {
        byte[] bytes = noiseTexture.EncodeToPNG();
        System.IO.File.WriteAllBytes(Application.dataPath + "/NoiseTexture.png", bytes);
    }
}
