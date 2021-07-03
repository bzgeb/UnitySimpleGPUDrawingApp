using UnityEngine;

public class DrawManager : MonoBehaviour
{
    [SerializeField] ComputeShader _drawComputeShader;
    [SerializeField] Color _backgroundColour;
    [SerializeField] Color _brushColour;
    [SerializeField] float _brushSize = 10f;

    [SerializeField] BrushSizeSlider _brushSizeSlider;
    [SerializeField, Range(0.01f, 1)] float _strokeSmoothingInterval = 0.1f;
    RenderTexture _canvasRenderTexture;

    Vector4 _previousMousePosition;

    void Start()
    {
        _brushSizeSlider.slider.SetValueWithoutNotify(_brushSize);

        _canvasRenderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        _canvasRenderTexture.filterMode = FilterMode.Point;
        _canvasRenderTexture.enableRandomWrite = true;
        _canvasRenderTexture.Create();

        int initBackgroundKernel = _drawComputeShader.FindKernel("InitBackground");
        _drawComputeShader.SetVector("_BackgroundColour", _backgroundColour);
        _drawComputeShader.SetTexture(initBackgroundKernel, "_Canvas", _canvasRenderTexture);
        _drawComputeShader.Dispatch(initBackgroundKernel, _canvasRenderTexture.width / 8,
            _canvasRenderTexture.height / 8, 1);

        _previousMousePosition = Input.mousePosition;
    }

    void Update()
    {
        if (!_brushSizeSlider.isInUse && Input.GetMouseButton(0))
        {
            int updateKernel = _drawComputeShader.FindKernel("Update");
            _drawComputeShader.SetVector("_PreviousMousePosition", _previousMousePosition);
            _drawComputeShader.SetVector("_MousePosition", Input.mousePosition);
            _drawComputeShader.SetBool("_MouseDown", Input.GetMouseButton(0));
            _drawComputeShader.SetFloat("_BrushSize", _brushSize);
            _drawComputeShader.SetVector("_BrushColour", _brushColour);
            _drawComputeShader.SetFloat("_StrokeSmoothingInterval", _strokeSmoothingInterval);
            _drawComputeShader.SetTexture(updateKernel, "_Canvas", _canvasRenderTexture);
            _drawComputeShader.Dispatch(updateKernel, _canvasRenderTexture.width / 8,
                _canvasRenderTexture.height / 8, 1);
        }
        
        _previousMousePosition = Input.mousePosition;
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