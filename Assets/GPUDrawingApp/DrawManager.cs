using UnityEngine;
using UnityEngine.UI;

public class DrawManager : MonoBehaviour
{
    [SerializeField] ComputeShader _drawComputeShader;
    [SerializeField] Color _backgroundColour;
    [SerializeField] Color _brushColour;
    [SerializeField] float _brushSize = 10f;
    
    [SerializeField] BrushSizeSlider _brushSizeSlider;
    RenderTexture _canvasRenderTexture;

    void Start()
    {
        _brushSizeSlider.slider.SetValueWithoutNotify(_brushSize);
        
        _canvasRenderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        _canvasRenderTexture.filterMode = FilterMode.Point;
        _canvasRenderTexture.enableRandomWrite = true;
        _canvasRenderTexture.Create();

        int initBackgroundKernel = _drawComputeShader.FindKernel("InitBackground");
        _drawComputeShader.SetVector("_BackgroundColour", _backgroundColour);
        _drawComputeShader.SetTexture(initBackgroundKernel, "_Result", _canvasRenderTexture);
        _drawComputeShader.Dispatch(initBackgroundKernel, _canvasRenderTexture.width / 8,
            _canvasRenderTexture.height / 8, 1);
    }

    void Update()
    {
        if (_brushSizeSlider.isInUse) return;
        
        int updateKernel = _drawComputeShader.FindKernel("Update");
        _drawComputeShader.SetVector("_MousePosition", Input.mousePosition);
        _drawComputeShader.SetBool("_MouseDown", Input.GetMouseButton(0));
        _drawComputeShader.SetFloat("_BrushSize", _brushSize);
        _drawComputeShader.SetVector("_BrushColour", _brushColour);
        _drawComputeShader.SetTexture(updateKernel, "_Result", _canvasRenderTexture);
        _drawComputeShader.Dispatch(updateKernel, _canvasRenderTexture.width / 8,
            _canvasRenderTexture.height / 8, 1);
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(_canvasRenderTexture, dest);
    }

    public void OnBrushSizeChanged(float newValue)
    {
        _brushSize = newValue;
    }
}