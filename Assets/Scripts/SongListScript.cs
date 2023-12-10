using System.Collections;
using System.Collections.Generic;
using Animation;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SongListScript : MonoBehaviour
{
    public TextMeshProUGUI[] values;
    public Image Jacket;
    public GameObject Prefab;
    public GameObject parent;
    public GameObject scrollView;
    public Image Block;

    private List<SongListElementScript> Elements = new();
    private ScrollRect scrollRect;
    private float[] Positions;
    private float Space;

    private Coroutine ListMovingCoroutine = null;

    public static SongListScript instance;

    void Start() {
        instance = this;
        scrollRect = scrollView.GetComponent<ScrollRect>();
        Positions = new float[GameManager.Beatmaps.Length];

        for (int i = 0; i < GameManager.Beatmaps.Length; i++) {
            YusaBundongBeatmap beatmap = GameManager.Beatmaps[i];
            GameObject obj = Instantiate(Prefab, parent.transform);
            SongListElementScript elementScript = obj.GetComponent<SongListElementScript>();
            elementScript.SendData(i);
            Elements.Add(elementScript);

            TextMeshProUGUI title = obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI artist = obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

            if (SettingsManager.Get<bool>(global::Settings.ShowAsUnicode)) {
                title.text = beatmap.metaData["Data", "Title_u"];
                artist.text = beatmap.metaData["Data", "Artist_u"];
            } else {
                title.text = beatmap.metaData["Data", "Title"];
                artist.text = beatmap.metaData["Data", "Artist"];
            }
        }

        Space = 1.0f/(Elements.ToArray().Length-1).ToFloat();
        movementThreshold = Space/100f;

        for (int i = 0; i < Positions.Length; i++) {
            Positions[i] = 1 - (Space*i.ToFloat());
        }

        scrollRect.verticalNormalizedPosition = Positions[GameManager.songCode];
        RefreshData();
    }

    private float lastScrollPos;
    private float movementThreshold;
    private bool isMoving = false;
    private bool isAnimationing = false;

    void FixedUpdate() {
        Select(FindClosestIndex(scrollRect.verticalNormalizedPosition, Positions), false);

        if (Mathf.Abs(scrollRect.verticalNormalizedPosition - lastScrollPos) > movementThreshold) {
            lastScrollPos = scrollRect.verticalNormalizedPosition;
            isMoving = true;
        } else {
            if (isMoving == true && !Input.GetKey(KeyCode.Mouse0) && !isAnimationing) {
                isMoving = false;
                Select(FindClosestIndex(scrollRect.verticalNormalizedPosition, Positions));
            }
        }
    }

    private int FindClosestIndex(float target, float[] values) {
        float minDifference = Mathf.Abs(values[0] - target);
        int closestIndex = 0;

        for(int i = 1; i < values.Length; i++) {
            float difference = Mathf.Abs(values[i] - target);
            if(difference < minDifference) {
                minDifference = difference;
                closestIndex = i;
            }
        }

        return closestIndex;
    }

    public void Select(int code, bool ScrollTo = true) {
        if (code == GameManager.songCode) return;

        if (ScrollTo) {
            if (isAnimationing) StopCoroutine(ListMovingCoroutine);
            ListMovingCoroutine = StartCoroutine(ToMoveList(code));
        } else {
            GameManager.songCode = code;
            GameManager.DifficultyCode = 0;
        }

        RefreshData();
    }

    private IEnumerator ToMoveList(int code) {
        isAnimationing = true;

        float Current = 0f;
        float Duration = 0.5f;

        float Start = scrollRect.verticalNormalizedPosition;
        float Target = Positions[code];

        while (true) {
            Current += Time.deltaTime;
            float t = EaseUtil.Ease(Current, Duration, Ease.OUT_CUBIC);
            scrollRect.verticalNormalizedPosition = Mathf.Lerp(Start, Target, t);

            if (Current >= Duration) break;

            yield return null;
        }
        
        isAnimationing = false;
    }

    private RectTransform rect;
    private Vector2 center = new(0f, 0f);
    private Vector2 bottom = new(0f, -1080f);
    private Vector2 fhd = new(1920f, 1080f);

    private readonly Vector2[] targets = new Vector2[]{new(0f, -225f), new(0f, -290f), new(0f, 195f)};

    void Awake() {
        rect = gameObject.GetComponent<RectTransform>();

        center = Convert2Screen(center);
        bottom = Convert2Screen(bottom);
        
        for (int i = 0; i<targets.Length; i++) {
            targets[i] = Convert2Screen(targets[i]);
        }

        DontDestroyOnLoad(gameObject);
    }

    private Vector2 Convert2Screen(Vector2 vec) {
        return RectTransformUtility.WorldToScreenPoint(null, rect.TransformPoint(vec));
    }

    private IEnumerator Animation1() {
        StartCoroutine(Animation2());

        float Duration = 1.5f;
        float Current = 0f;

        Vector2[] Targets = new Vector2[gameObject.transform.childCount];
        Vector2[] Starts = new Vector2[gameObject.transform.childCount];

        while (true) {
            Current += Time.deltaTime;

            float t = Current / Duration;
            t = --t * t * t + 1;

            for (int i = 0; i<gameObject.transform.childCount; i++) {
                Transform obj = gameObject.transform.GetChild(i);
                if (obj.name == Block.name) continue;

                if (Targets[i] == Vector2.zero) Targets[i] = new Vector2(obj.position.x, obj.position.y-Convert2Screen(new Vector2(0f, 1080f)).y);
                if (Starts[i] == Vector2.zero) Starts[i] = obj.position;

                obj.position = Vector2.Lerp(Starts[i], Targets[i], t);
            }

            if (Vector2.Distance(gameObject.transform.GetChild(0).position, Targets[0]) < 0.1f) {
                break;
            }

            yield return null;
        }

        StartCoroutine(Animation3());
    }

    private IEnumerator Animation2() {
        float Duration = 1.25f;
        float Current = 0f;

        Vector2 Target = center;
        Vector2 Start = Block.transform.position;

        while(true) {
            Current += Time.deltaTime;

            float t = Current / Duration;
            t = --t * t * t + 1;

            Block.transform.position = Vector2.Lerp(Start, Target, t);

            if (Vector2.Distance(Block.transform.position, center) < 0.1f) {
                break;
            }

            yield return null;
        }
    }

    private IEnumerator Animation3() {
        float Duration = 1.25f;
        float Current = 0f;

        Vector2 Target = fhd;
        Vector2 Start = Block.rectTransform.sizeDelta;

        while(true) {
            Current += Time.deltaTime;

            float t = Current / Duration;
            t = --t * t * t + 1;

            Block.rectTransform.sizeDelta = Vector2.Lerp(Start, Target, t);

            if (Vector2.Distance(Block.rectTransform.sizeDelta, Target) < 1f) {
                break;
            }

            yield return null;
        }

        StartCoroutine(Animation4());
    }

    private IEnumerator Animation4() {
        float Duration = 1.25f;
        float Current = 0f;

        Vector2[] Targets = targets;
        Vector2[] Starts = new Vector2[3];

        while(true) {
            Current += Time.deltaTime;

            float t = Current / Duration;
            t = --t * t * t + 1;

            for (int i = 0; i<3; i++) {
                TextMeshProUGUI obj = values[i+4];

                if (Starts[i] == Vector2.zero) Starts[i] = obj.transform.position;

                obj.transform.position = Vector2.Lerp(Starts[i], Targets[i], t);

                if (values[i+4].color != Color.black) {
                    values[i+4].color = values[i+4].color.setA(values[i+4].color.a + Time.deltaTime);
                }
            }

            if (Vector2.Distance(values[4].transform.position, Targets[0]) < 0.1f) {
                break;
            }

            yield return null;
        }

        yield return new WaitForSeconds(3f);
        ReallyStartGame();
        StartCoroutine(Animation5());
    }

    IEnumerator Animation5() {
        yield return new AsyncOperation();
        yield return new WaitForFixedUpdate();

        float Duration = 1.5f;
        float Current = 0f;

        Vector2 Target = bottom;
        Vector2 Start = Block.transform.position;

        while (true) {
            Current += Time.deltaTime;

            float t = Current / Duration;

            t = t * t * t;

            Block.transform.position = Vector2.Lerp(Start, Target, t);

            if (Vector2.Distance(Block.transform.position, Target) < 0.1f) {
                DestroyImmediate(gameObject);
                break;
            }

            yield return null;
        }
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.Space)) {
            StartGame();
        }
    }

    private IEnumerator DestorySelf() {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

    public void Reload() {
        SceneAnimation.LoadScene(1);
        
        StartCoroutine(DestorySelf());
    }

    private void StartGame() {
        StartCoroutine(Animation1());
    }

    private void ReallyStartGame() {
        SceneManager.LoadSceneAsync(3);
    }

    public void Settings() {
        SceneAnimation.LoadScene(5);
        StartCoroutine(DestorySelf());
    }

    public void Exit() {
        Application.Quit(0);
    }
    
    private void RefreshData() {
        YusaBundongBeatmap SelectedBeatmap = GameManager.Beatmaps[GameManager.songCode];

        if (SelectedBeatmap == null) return;

        Dictionary<string, string> Data = SelectedBeatmap.metaData["Data"];

        if (SettingsManager.Get<bool>(global::Settings.ShowAsUnicode)) {
            values[0].text = Data["Title_u"];
            values[1].text = Data["Artist_u"];
            values[4].text = Data["Title_u"];
            values[5].text = Data["Artist_u"];
        } else {
            values[0].text = Data["Title"];
            values[1].text = Data["Artist"];
            values[4].text = Data["Title"];
            values[5].text = Data["Artist"];
        }

        if (Data["FromBPM"] == Data["ToBPM"]) {
            values[2].text = Data["FromBPM"] + "BPM";
        } else {
            values[2].text = string.Format("{0}-{1}BPM", Data["FromBPM"], Data["ToBPM"]);
        }

        if (SelectedBeatmap.Jacket != null) {
            Jacket.sprite = SelectedBeatmap.Jacket;
        }

        Color color = new(0f, 0f, 0f, 0f);

        foreach (SongListElementScript song in Elements) {
            song.myImage.color = color;
        }

        Elements[GameManager.songCode].myImage.color = color.setA(0.7f);

        RefreshDifficulty();
    }

    public void RefreshDifficulty() {
        values[3].text = GameManager.Beatmaps[GameManager.songCode].yusaBundongFiles[GameManager.DifficultyCode].version;
        values[6].text = GameManager.Beatmaps[GameManager.songCode].yusaBundongFiles[GameManager.DifficultyCode].version;
    }

    public void ChangeDifficulty(bool isUp) {
        if (isUp) {
            if (GameManager.DifficultyCode == GameManager.Beatmaps[GameManager.songCode].yusaBundongFiles.Length-1) {
                GameManager.DifficultyCode = 0;
            } else {
                GameManager.DifficultyCode++;
            }
        } else {
            if (GameManager.DifficultyCode == 0) {
                GameManager.DifficultyCode = GameManager.Beatmaps[GameManager.songCode].yusaBundongFiles.Length-1;
            } else {
                GameManager.DifficultyCode--;
            }
        }

        RefreshDifficulty();
    }
}
