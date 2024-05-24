using UnityEngine;

public class PerlinNoiseGenerator : MonoBehaviour
{
    [Range(0, 1920)] public int width = 256;
    [Range(0, 1920)] public int height = 256;
    [Range(0, 100)] public float scale = 20f;
    [Range(0, 10000)] public float offsetX, offsetY;

    public Renderer noiseDisplay;
    Texture2D noiseTexture;

    Color[] pixels;

    private void Start()
    {
        noiseDisplay.material.mainTexture = GenerateNoiseTexture();
    }

    public void DefineVariables()
    {
        noiseTexture = new(width, height);
        pixels = new Color[width * height];
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

        //noiseTexture.SetPixels(pixels);
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
        System.IO.File.WriteAllBytes(Application.dataPath + "/PerlinNoise.png", bytes);
    }


}
