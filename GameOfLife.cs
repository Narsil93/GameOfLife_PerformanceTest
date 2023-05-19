using System;
using UnityEngine;
using System.Diagnostics;
using TMPro;
using System.Threading.Tasks;

public class GameOfLife : MonoBehaviour
{
    public int textureSize = 512;
    public float updateInterval = 0.01f;
    private Texture2D texture;

    private Color[] colors;
    private bool[] cells;
    private float timer;
    private bool run = false;

    private void Start()
    {
        texture = GetComponent<MeshRenderer>().material.mainTexture as Texture2D;

        // initialize arrays for color and cells
        colors = new Color[textureSize * textureSize];
        cells = new bool[textureSize * textureSize];

        // read pixel from texture and assign cells
        Color[] pixels = texture.GetPixels();
        for (int i = 0; i < pixels.Length; i++)
        {
            cells[i] = pixels[i] == Color.black ? false : true;
        }
    }

    private void Update()
    {
        // update cells
        if (timer >= updateInterval && run)
        {
            timer = 0f;
            UpdateCellsParallel();
            UpdateTexture();
        }
        else
        {
            timer += Time.deltaTime;
        }
    }

    private void UpdateCellsParallel()
    {
        // copy new cells
        bool[] newCells = new bool[cells.Length];
        cells.CopyTo(newCells, 0);

        // apply Game of Life rules with multi-threading
        Parallel.For(0, textureSize, x =>
        {
            for (int y = 0; y < textureSize; y++)
            {
                int index = x * textureSize + y;
                int neighbors = CountNeighbors(x, y);

                if (cells[index])
                {
                    if (neighbors < 2 || neighbors > 3)
                    {
                        newCells[index] = false;
                    }
                }
                else
                {
                    if (neighbors == 3)
                    {
                        newCells[index] = true;
                    }
                }
            }
        });

        // update cells
        cells = newCells;
    }

    private int CountNeighbors(int x, int y)
    {
        int count = 0;
        for (int i = x - 1; i <= x + 1; i++)
        {
            for (int j = y - 1; j <= y + 1; j++)
            {
                if (i >= 0 && i < textureSize && j >= 0 && j < textureSize && !(i == x && j == y))
                {
                    int index = i * textureSize + j;
                    if (cells[index])
                    {
                        count++;
                    }
                }
            }
        }
        return count;
    }

    private void UpdateTexture()
    {
        // update color-array based on cells
        for (int x = 0; x < textureSize; x++)
        {
            for (int y = 0; y < textureSize; y++)
            {
                int index = x * textureSize + y;
                colors[index] = cells[index] ? Color.white : Color.black;
            }
        }

        // apply changes on texture
        texture.SetPixels(colors);
        texture.Apply();

        // update mainTexture
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material.mainTexture = texture;
    }

    public void calibCPU() {
        int numIterations = Int32.Parse(GameObject.Find("IterationsInput").GetComponent<TMP_InputField>().text);
        Stopwatch sw = new Stopwatch();
        TMP_Text textComponent = GameObject.Find("Text_Ausgabe").GetComponent<TMP_Text>();
        textComponent.text = "";

        sw.Start();
        for (int i = 0; i < numIterations; i++)
        {
            UpdateCellsParallel();
            UpdateTexture();
        }
        sw.Stop();

        // measure CPU time in ms
        long elapsedTime = sw.ElapsedMilliseconds;

        // log results
        UnityEngine.Debug.Log("PROTOTYPE 1: CPU calibration time for " + numIterations + " iterations: " + elapsedTime + " ms");
        
        // measure CPU time in ticks
        long elapsedTicks = sw.ElapsedTicks;
        //long elapsedCycles = elapsedTicks * System.Environment.ProcessorCount;

        // log results
        UnityEngine.Debug.Log("PROTOTYPE 1: CPU ticks for " + numIterations + " iterations: " + elapsedTicks);

        // log on textComponent
        textComponent.text = string.Format("PROTOTYPE 1: CPU calibration time for {0} iterations: {1} ms\nPROTOTYPE 1: CPU ticks for {0} iterations: {2}" , numIterations, elapsedTime, elapsedTicks);
    }

    public void runProtoype() {
        if(run == false)
            this.run = true;
        else
            this.run = false;
    }

}
