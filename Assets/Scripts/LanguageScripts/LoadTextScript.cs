using TMPro;
using UnityEngine;

public class LoadTextScript : MonoBehaviour
{
    public string key;
    private TextMeshProUGUI textM;

    void Start() {
        textM = GetComponent<TextMeshProUGUI>();
        textM.text = LanguageScript.lang[key];
        Destroy(this);
    }
}
