using UnityEngine;

public class DrawManager : MonoBehaviour
{
    [SerializeField] ComputeShader _drawComputeShader;
    [SerializeField] Color _backgroundColour;
    [SerializeField] Color _brushColour;
    RenderTexture _canvasRenderTexture;

    void Start()
    {
        _canvasRenderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        _canvasRenderTexture.filterMode = FilterMode.Point;
        _canvasRenderTexture.enableRandomWrite = true;
        _canvasRenderTexture.Create();

        int initBackgroundKernel = _drawComputeShader.FindKernel("InitBackground");
        _drawComputeShader.SetVector("_BackgroundColour", _backgroundColour);
        _drawComputeShader.SetTexture(initBackgroundKernel, "_Result", _canvasRenderTexture);
        _drawComputeShader.Dispatch(initBackgroundKernel, _canvasRenderTexture.width / 8,
            _canvasRenderTexture.height / 8, 1);
        /*
        int mainKernel = _drawComputeShader.FindKernel("CSMain");
        _drawComputeShader.SetTexture(mainKernel, "_Result", _canvasRenderTexture);
        _drawComputeShader.Dispatch(mainKernel, _canvasRenderTexture.width / 8,
            _canvasRenderTexture.height / 8, 1);
            */
    }

    void Update()
    {
        int updateKernel = _drawComputeShader.FindKernel("Update");
        _drawComputeShader.SetVector("_MousePosition", Input.mousePosition);
        _drawComputeShader.SetBool("_MouseDown", Input.GetMouseButton(0));
        _drawComputeShader.SetFloat("_BrushSize", 10f);
        _drawComputeShader.SetVector("_BrushColour", _brushColour);
        _drawComputeShader.SetTexture(updateKernel, "_Result", _canvasRenderTexture);
        _drawComputeShader.Dispatch(updateKernel, _canvasRenderTexture.width / 8,
            _canvasRenderTexture.height / 8, 1);
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(_canvasRenderTexture, dest);
    }
}