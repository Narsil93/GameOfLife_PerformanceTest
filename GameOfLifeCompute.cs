using System;
using UnityEngine;
using System.Diagnostics;
using TMPro;

public class GameOfLifeCompute : MonoBehaviour
{
    public ComputeShader computeShader;
    public int textureSize = 512;
    public float updateInterval = 0.5f;

    private RenderTexture texture;
    private bool run = false;
    private int kernelID;
    private int threadGroupSize;

    private float lastUpdateTime;

    private void Start()
    {
        // find the kernel ID for the compute shader
        kernelID = computeShader.FindKernel("GameOfLife");
        
        // read main texture and assign it to RenderTexture
        Texture mainMaterialTexture = GetComponent<MeshRenderer>().material.mainTexture;

        texture = new RenderTexture(textureSize, textureSize, 24);
        texture.wrapMode = TextureWrapMode.Repeat;
        texture.enableRandomWrite = true;
        texture.filterMode = FilterMode.Point;
        texture.useMipMap = false;
        texture.Create();

        // write data on new RenderTexture
        Graphics.Blit(mainMaterialTexture, texture);

        // set textureSize for ComputeShader
        computeShader.SetInt("TextureSize", textureSize);

        // calculate the thread group size based on the textureSize
        uint x, y, z;
        computeShader.GetKernelThreadGroupSizes(kernelID, out x, out y, out z);
        threadGroupSize = Mathf.CeilToInt((float)textureSize / x);

        lastUpdateTime = Time.time;
    }

    private void Update()
    {
        // check if the update interval has elapsed
        if (Time.time - lastUpdateTime >= updateInterval && run)
        {
            // set the texture for compute shader
            computeShader.SetTexture(kernelID, "Result", texture);

            // run the compute shader
            computeShader.Dispatch(kernelID, threadGroupSize, threadGroupSize, 1);

            // update mainTexture
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            meshRenderer.material.mainTexture = texture;

            // update the last update time
            lastUpdateTime = Time.time;
        }
    }

    public void calibCPU() {
        int numIterations = Int32.Parse(GameObject.Find("IterationsInput").GetComponent<TMP_InputField>().text);
        Stopwatch sw = new Stopwatch();
        TMP_Text textComponent = GameObject.Find("Text_Ausgabe").GetComponent<TMP_Text>();
        textComponent.text = "";

        sw.Start();
        for (int i = 0; i < numIterations; i++)
        {
            computeShader.Dispatch(0, textureSize / 8, textureSize / 8, 1);
        }
        sw.Stop();

        // measure CPU time in ms
        long elapsedTime = sw.ElapsedMilliseconds;

        // log results
        UnityEngine.Debug.Log("PROTOTYPE 2: CPU calibration time for " + numIterations + " iterations: " + elapsedTime + " ms");
        
        // measure CPU time in ticks
        long elapsedTicks = sw.ElapsedTicks;
        //long elapsedCycles = elapsedTicks * System.Environment.ProcessorCount;

        // log results
        UnityEngine.Debug.Log("PROTOTYPE 2: CPU ticks for " + numIterations + " iterations: " + elapsedTicks);

        // log on textComponent
        textComponent.text = string.Format("PROTOTYPE 2: CPU calibration time for {0} iterations: {1} ms\nPROTOTYPE 2: CPU ticks for {0} iterations: {2}" , numIterations, elapsedTime, elapsedTicks);
    }

    public void runProtoype() {
        if(run == false)
            this.run = true;
        else
            this.run = false;
    }
}

