using UnityEngine;

public class Paint : MonoBehaviour
{
    private enum Method
    {
        Circle = 0,
        Quad = 1
    }

    [Range(2, 512)] [SerializeField] private int _textureSize = 128;
    [SerializeField] private TextureWrapMode _textureWrapMode;
    [SerializeField] private FilterMode _filterMode;
    [SerializeField] private Material _material;

    [SerializeField] private Camera _camera;
    [SerializeField] private Collider _collider;
    [SerializeField] private Color _color;
    [SerializeField] private int _brushSize = 8;
    [SerializeField] private Method _brushMethod;

    [SerializeField] private Texture2D _texture;

    private int _oldRayX, _oldRayY;

    private void OnValidate()
    {
        if (!Application.isPlaying)
            return;

        Init();
    }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        if (_texture == null)
        {
            _texture = new Texture2D(_textureSize, _textureSize);
        }

        if (_texture.width != _textureSize)
        {
            _texture.Reinitialize(_textureSize, _textureSize);
        }

        _texture.wrapMode = _textureWrapMode;
        _texture.filterMode = _filterMode;
        _material.mainTexture = _texture;
        _texture.Apply();
    }

    private void Update()
    {
        _brushSize += (int)Input.mouseScrollDelta.y;

        if (Input.GetMouseButton(0))
        {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (_collider.Raycast(ray, out var hit, 100f))
            {
                var rayX = (int)(hit.textureCoord.x * _textureSize);
                var rayY = (int)(hit.textureCoord.y * _textureSize);

                if (_oldRayX != rayX || _oldRayY != rayY)
                {
                    switch (_brushMethod)
                    {
                        case Method.Circle:
                            DrawCircle(rayX, rayY);
                            break;
                        case Method.Quad:
                            DrawQuad(rayX, rayY);
                            break;
                    }

                    _oldRayX = rayX;
                    _oldRayY = rayY;
                }

                _texture.Apply();
            }
        }
    }

    private void DrawQuad(int rayX, int rayY)
    {
        for (var y = 0; y < _brushSize; y++)
        {
            for (var x = 0; x < _brushSize; x++)
            {
                _texture.SetPixel(rayX + x - _brushSize / 2, rayY + y - _brushSize / 2, _color);
            }
        }
    }

    private void DrawCircle(int rayX, int rayY)
    {
        for (var y = 0; y < _brushSize; y++)
        {
            for (var x = 0; x < _brushSize; x++)
            {
                var x2 = Mathf.Pow(x - _brushSize / 2, 2);
                var y2 = Mathf.Pow(y - _brushSize / 2, 2);
                var r2 = Mathf.Pow(_brushSize / 2 - 0.5f, 2);

                if (x2 + y2 < r2)
                {
                    var pixelX = rayX + x - _brushSize / 2;
                    var pixelY = rayY + y - _brushSize / 2;

                    if (pixelX >= 0 && pixelX < _textureSize && pixelY >= 0 && pixelY < _textureSize)
                    {
                        var oldColor = _texture.GetPixel(pixelX, pixelY);
                        var resultColor = Color.Lerp(oldColor, _color, _color.a);
                        _texture.SetPixel(pixelX, pixelY, resultColor);
                    }
                }
            }
        }
    }
}