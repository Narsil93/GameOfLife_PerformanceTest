using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TextureGenerator : MonoBehaviour
{
    public int textureSize = 512;
    public Color color1 = Color.black;
    public Color color2 = Color.white;

    private Texture2D texture;

    public void SaveTextureToFile()
    {
        texture = new Texture2D(textureSize, textureSize);

        for (int x = 0; x < textureSize; x++)
        {
            for (int y = 0; y < textureSize; y++)
            {
                // random decision between 0 and 1
                float random = Random.Range(0f, 1f);
                Color color = random < 0.5f ? color1 : color2;

                texture.SetPixel(x, y, color);
            }
        }

        // create path to save file
        string filePath = Application.dataPath + "/RandomTexture/randomTexture.png";

        // encode texture to png format
        byte[] bytes = texture.EncodeToPNG();

        // write data to file
        File.WriteAllBytes(filePath, bytes);

        Debug.Log("Die Textur wurde unter " + filePath + " gespeichert.");
    }

    public void setTextureToPrototypes() {
        SaveTextureToFile();
        string texturePath = Application.dataPath + "/RandomTexture/randomTexture.png";
        Texture2D tex = new Texture2D(2,2);

        if(File.Exists(texturePath)) {
            byte[] textureData = File.ReadAllBytes(texturePath);
            
            tex.LoadImage(textureData);
        }
        Texture tex2 = Resources.Load(texturePath) as Texture;

        GameObject.Find("Plane_Texture_PT").GetComponent<MeshRenderer>().material.SetTexture("_MainTex", tex);
        //GameObject.Find("Plane_Texture_PT").GetComponent<MeshRenderer>().material.SetTexture("_MainTex", Resources.Load(texturePath, Texture));
    }

    private void Start()
    {

    }
}
