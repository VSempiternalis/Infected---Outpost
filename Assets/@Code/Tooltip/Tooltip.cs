using UnityEngine;
using TMPro;

public class Tooltip : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI headerField;
    [SerializeField] private TextMeshProUGUI contentField;

    private void Awake() {
        // print("TOOLTIP awake");
        // rect = GetComponent<RectTransform>();
    }

    private void Update() {
        // print("TOOLTIP update");
    }

    private void SetPos() {
        Vector2 pos = Input.mousePosition;
        // print(pos);
        pos = new Vector2(pos.x + 150, pos.y - 75);

        // float pivotX = pos.x / Screen.width;
        // float pivotY = pos.y / Screen.height;

        // rect.pivot = new Vector2(pivotX, pivotY);
        transform.position = pos;
        // print(transform.position);
    }

    public void SetText(string header, string content) {
        // print("TOOLTIP set text");
        //Dont show if empty
        if(string.IsNullOrEmpty(header)) headerField.gameObject.SetActive(false);
        else {
            headerField.gameObject.SetActive(true);
            headerField.text = header;
        }

        contentField.text = content;
        SetPos();
    }

    // private void SetElement() {
    //     print("TOOLTIP set element");
    //     int headerLength = headerField.text.Length;
    //     int contentLength = contentField.text.Length;

    //     // layoutElement.enabled = (headerLength > characterWrapLimit || contentLength > characterWrapLimit)? true:false;
    // }
}
