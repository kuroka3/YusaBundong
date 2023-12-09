using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneAnimation : MonoBehaviour
{
    public Image Background;
    public static SceneAnimation instance = null;
    public static bool isDoing = false;

    void Start() {
        if (instance != null) {
            DestroyImmediate(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        rect = gameObject.GetComponent<RectTransform>();
    }

    public void Scene(int Scene) {
        if (isDoing) return;

        IEnumerator LoadingScene(int index) {
            isDoing = true;
            yield return StartCoroutine(Bridge(true));
            yield return SceneManager.LoadSceneAsync(index);
            yield return StartCoroutine(Bridge(false));
            isDoing = false;
        }

        IEnumerator Bridge(bool isFirst) {
            float Duration = 1.0f;
            float Current = 0f;

            Vector2 Target;
            if (isFirst) Target = Convert2Screen(new Vector2(0, 0f));
            else Target = Convert2Screen(new Vector2(1920f, 0f));
            Vector2 Start;
            if (isFirst) Start = Convert2Screen(new Vector2(-1920f, 0f));
            else Start = Convert2Screen(new Vector2(0f, 0f));

            while(true) {
                Current += Time.deltaTime;

                float t = Current / Duration;
                if (isFirst) t = --t * t * t + 1;
                else t = t * t * t;

                instance.Background.transform.position = Vector2.Lerp(Start, Target, t);

                if (Vector2.Distance(instance.Background.transform.position, Target) < 0.1f) {
                    break;
                }

                yield return null;
            }
        }

        StartCoroutine(LoadingScene(Scene));
    }

    public static void LoadScene(int Scene) {
        instance.Scene(Scene);
    }

    private static RectTransform rect;

    private static Vector2 Convert2Screen(Vector2 vec) {
        return RectTransformUtility.WorldToScreenPoint(null, rect.TransformPoint(vec));
    }
}
