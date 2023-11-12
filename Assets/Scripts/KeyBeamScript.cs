using UnityEngine;

public class KeyBeamScript : MonoBehaviour
{
    public GameObject[] keybeams;
    private SpriteRenderer[] renderers = new SpriteRenderer[4];
    public float dftA;
    private Color dft = new Color(0.678f, 0.867f, 1.0f);

    void Start() {        
        for (int i = 0; i<keybeams.Length; i++) {
            renderers[i] = keybeams[i].GetComponent<SpriteRenderer>();
        }

        for (int i = 0; i<keybeams.Length; i++) {
            renderers[i].color = dft.setA(0f);
        }
    }

    void FixedUpdate() {
        for (int i = 0; i<keybeams.Length; i++) {
            if(Input.GetKey(GameManager.instance.keys[i])) {
                renderers[i].color = dft.setA(dftA);
            } else if(renderers[i].color.a > 0) {
                renderers[i].color = renderers[i].color.setA(renderers[i].color.a - Time.fixedDeltaTime);
            } else {
                renderers[i].color = dft.setA(0f);
            }
        }
    }
}

public static class ColorExtension {
    public static Color setA(this Color color, float alpha) {
        color.a = alpha;
        return color;
    }
}