using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class ButtonJuice : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler
{
    [Header("Scale Feedback")]
    [SerializeField] private float hoverScale = 1.08f;   // grows on hover
    [SerializeField] private float pressedScale = 0.92f; // squashes on press
    [SerializeField] private float animSpeed = 12f;      // higher = snappier settle

    [Header("Sound")]
    [SerializeField] private bool playClickSound = true;

    private RectTransform rt;
    private Button button;
    private Vector3 baseScale;
    private float targetFactor = 1f;
    private float currentFactor = 1f;
    private bool isHovering;
    private bool isPressed;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        button = GetComponent<Button>();
        baseScale = rt.localScale;
    }

    // Reset cleanly if the button is hidden/reshown (e.g. reopening the pause menu)
    void OnDisable()
    {
        currentFactor = 1f;
        targetFactor = 1f;
        isHovering = false;
        isPressed = false;
        if (rt != null) rt.localScale = baseScale;
    }

    void Update()
    {
        // Frame-rate independent smoothing; unscaled so it animates even when paused (timeScale 0)
        float t = 1f - Mathf.Exp(-animSpeed * Time.unscaledDeltaTime);
        currentFactor = Mathf.Lerp(currentFactor, targetFactor, t);
        rt.localScale = baseScale * currentFactor;
    }

    private bool Interactable => button == null || button.interactable;

    public void OnPointerEnter(PointerEventData e)
    {
        if (!Interactable) return;
        isHovering = true;
        UpdateTarget();
    }

    public void OnPointerExit(PointerEventData e)
    {
        isHovering = false;
        UpdateTarget();
    }

    public void OnPointerDown(PointerEventData e)
    {
        if (!Interactable) return;
        isPressed = true;
        UpdateTarget();
        if (playClickSound && AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();
    }

    public void OnPointerUp(PointerEventData e)
    {
        isPressed = false;
        UpdateTarget();
    }

    private void UpdateTarget()
    {
        if (isPressed)      targetFactor = pressedScale;
        else if (isHovering) targetFactor = hoverScale;
        else                 targetFactor = 1f;
    }
}