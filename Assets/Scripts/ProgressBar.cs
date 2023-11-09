using UnityEngine;

public class ProgressBar : MonoBehaviour
{
    public static float value = 0f;

    void FixedUpdate() {
        transform.localScale = new Vector3(value*10, transform.localScale.y);
    }
}
