using UnityEngine;

public class TooltipSystem : MonoBehaviour {
    private static TooltipSystem current;
    [SerializeField] private Tooltip tooltip;

    private void Awake() {
        current = this;
    }

    public static void Show(string header, string content) {
        current.tooltip.SetText(header, content);
        current.tooltip.gameObject.SetActive(true);
    }

    public static void Hide() {
        current.tooltip.gameObject.SetActive(false);
    }
}
