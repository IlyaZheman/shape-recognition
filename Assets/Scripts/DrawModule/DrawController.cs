using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DrawModule
{
    public class DrawController : MonoBehaviour, IPointerDownHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private Image image;
        [SerializeField] private RectTransform rectTransform;

        [Range(2, 512)]
        [SerializeField] private int textureSize = 128;
        [SerializeField] private Color color;
        [SerializeField] private int brushSize = 8;

        [SerializeField] private Texture2D texture;

        private Vector2 _previousDragPosition;

        private void OnValidate()
        {
            if (!Application.isPlaying)
                return;

            Init();
        }

        private void Awake()
        {
            image = GetComponent<Image>();
            rectTransform = GetComponent<RectTransform>();

            Init();
        }

        private void Init()
        {
            if (texture == null)
            {
                texture = new Texture2D(textureSize, textureSize);
            }

            if (texture.width != textureSize)
            {
                texture.Reinitialize(textureSize, textureSize);
            }

            texture.wrapMode = TextureWrapMode.Repeat;
            texture.filterMode = FilterMode.Point;
            image.material.mainTexture = texture;
            texture.Apply();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!Draw(eventData, out var textureCoord))
                return;

            DrawCircle((int)textureCoord.x, (int)textureCoord.y);
            texture.Apply();

            _previousDragPosition = textureCoord;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!Draw(eventData, out var textureCoord))
                return;

            var distance = Vector2.Distance(_previousDragPosition, textureCoord);
            var lerpSteps = 1 / distance;

            for (float lerp = 0; lerp <= 1; lerp += lerpSteps)
            {
                var currentPosition = Vector2.Lerp(_previousDragPosition, textureCoord, lerp);
                DrawCircle((int)currentPosition.x, (int)currentPosition.y);
            }

            texture.Apply();

            _previousDragPosition = textureCoord;
        }

        private bool Draw(PointerEventData eventData, out Vector2 textureCoord)
        {
            var isInside = RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out var pointInRect);

            if (!isInside)
            {
                textureCoord = Vector2.zero;
                return false;
            }

            textureCoord = pointInRect - rectTransform.rect.min;
            textureCoord.x *= image.mainTexture.width / rectTransform.rect.width;
            textureCoord.y *= image.mainTexture.height / rectTransform.rect.height;
            return true;
        }

        private void DrawCircle(int rayX, int rayY)
        {
            for (var y = 0; y < brushSize; y++)
            {
                for (var x = 0; x < brushSize; x++)
                {
                    var x2 = Mathf.Pow(x - brushSize / 2, 2);
                    var y2 = Mathf.Pow(y - brushSize / 2, 2);
                    var r2 = Mathf.Pow(brushSize / 2 - 0.5f, 2);

                    if (x2 + y2 < r2)
                    {
                        var pixelX = rayX + x - brushSize / 2;
                        var pixelY = rayY + y - brushSize / 2;

                        if (pixelX >= 0 && pixelX < textureSize && pixelY >= 0 && pixelY < textureSize)
                        {
                            var oldColor = texture.GetPixel(pixelX, pixelY);
                            var resultColor = Color.Lerp(oldColor, color, color.a);
                            texture.SetPixel(pixelX, pixelY, resultColor);
                        }
                    }
                }
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _previousDragPosition = Vector2.zero;
        }
    }
}