using UnityEngine;
using UnityEngine.EventSystems;

public class HoverTilt : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float tiltAmount = 10f; // La cantidad máxima de rotación en grados
    public float smoothSpeed = 5f; // Velocidad de suavizado para la transición

    private RectTransform rectTransform;
    private Quaternion originalRotation;
    private bool isHovering = false;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalRotation = rectTransform.rotation;
    }

    void Update()
    {
        PerformTilt();
    }

    private void PerformTilt()
    {
        if (isHovering)
        {
            Vector2 mousePosition = Input.mousePosition;
            Vector2 rectCenter = RectTransformUtility.WorldToScreenPoint(Camera.main, rectTransform.position);
            Vector2 offset = mousePosition - rectCenter;

            float width = rectTransform.rect.width / 2f;
            float height = rectTransform.rect.height / 2f;

            // Normalizamos la posición relativa del ratón (-1 a 1)
            float normalizedX = (Mathf.Clamp01(offset.x / width) - 0.5f) * 2;
            float normalizedY = (Mathf.Clamp01(offset.y / height) - 0.5f) * 2;

            //Debug.Log(normalizedX);
            Debug.Log(normalizedY);

            // Calculamos la rotación en los ejes
            float tiltX = normalizedY * -tiltAmount; // Invertimos el eje Y para inclinar correctamente
            float tiltY = normalizedX * tiltAmount;

            // Aplicamos la rotación objetivo con suavizado
            Quaternion targetRotation = Quaternion.Euler(tiltX, tiltY, 0);
            rectTransform.rotation = Quaternion.Lerp(rectTransform.rotation, targetRotation, Time.deltaTime * smoothSpeed);
        }
        else
        {
            // Volver a la rotación original cuando no se hace hover
            rectTransform.rotation = Quaternion.Lerp(rectTransform.rotation, originalRotation, Time.deltaTime * smoothSpeed);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
    }
}
