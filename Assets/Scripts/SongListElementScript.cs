using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SongListElementScript : MonoBehaviour, IPointerClickHandler
{
    private int myCode;
    public Image myImage;

    void Awake() {
        myImage = GetComponent<Image>();
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        
        SongListScript.instance.Select(myCode);
    }

    public void SendData(int yourCode) {
        myCode = yourCode;
    }
}
