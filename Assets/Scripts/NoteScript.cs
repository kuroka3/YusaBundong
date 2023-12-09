using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NoteScript : MonoBehaviour
{
    public GameObject prefab;
    public float AfterTime;

    public AudioSource audioS;
    private Coroutine syncTimeCour;

    public int LimitError;

    private float LimitErrorFloat;

    public static NoteScript instance;
    public static float time = -1f;

    void Start()
    {
        instance = this;
        LimitErrorFloat = LimitError.ToFloat()*0.001f;
        startSong(GameManager.songCode);
    }

    void Update() {
        if (GameManager.paused) return;
        time += Time.deltaTime;
    }

    private IEnumerator syncTime() {
        while(true) {
            if (!GameManager.paused) {
                if(Math.Abs(audioS.time - time) >= LimitErrorFloat) {
                    time = audioS.time;
                }
            }
            
            yield return new WaitForSeconds(1f);
        }
    }

    private void startSong(int songid) {
        time = -AfterTime;

        audioS = gameObject.AddComponent<AudioSource>();
        audioS.clip = GameManager.Beatmaps[songid].audioClip;
        audioS.volume = SettingsManager.GetAsInt(Settings.Volume).ToFloat()*0.01f;

        string[] chart = GameManager.Beatmaps[songid].yusaBundongFiles[GameManager.DifficultyCode].chart;

        IEnumerator startAudio() {
            yield return new WaitForSeconds(AfterTime);

            audioS.Play();
            syncTimeCour = StartCoroutine(syncTime());

            int endTime = 0;

            for (int i = chart.Length-1; i>=0; i--) {
                if (!string.IsNullOrEmpty(chart[i])) {
                    string[] lastLineData = chart[i].Split(",");

                    if(lastLineData[2].Equals("-1")) endTime = int.Parse(lastLineData[1]);
                    else endTime = int.Parse(lastLineData[2]); 
                    break;
                }
            }

            float WaitTime = endTime.ToFloat()*0.001f+3f;

            while (time < WaitTime) {
                yield return new WaitForSeconds(0.1f);
            }

            SceneAnimation.LoadScene(4);
            StopCoroutine(syncTimeCour);
        }

        IEnumerator NoteSummon() {
            yield return new AsyncOperation();

            int id = 0;

            for (int i = 0; i<chart.Length; i++) {
                GameObject clone;

                while (true) {
                    clone = GameManager.instance.pool.Get(0);

                    if (clone == null) {
                        yield return new WaitForSeconds(0.1f);
                    } else {
                        break;
                    }
                }

                string Element = chart[i];
                string[] Data;
                int[] Data2Int;
                try {
                    Data = Element.Split(',');
                    Data2Int = new int[4]{Data[0].ToInt(true), Data[1].ToInt(true), id, Data[2].ToInt(true)};
                } catch(Exception e) {
                    Debug.LogError(e);
                    continue;
                }

                NoteInstScript cloneScript = clone.GetComponent<NoteInstScript>();
                cloneScript.sendData(Data2Int);

                id++;
            }
        }


        StartCoroutine(NoteSummon());
        StartCoroutine(startAudio());
    }

    public static void pauseSong(bool boo) {
        if (boo) instance.audioS.Pause();
        else instance.audioS.UnPause();
    }

    public static bool isPlayingSong() {
        return instance.audioS.isPlaying;
    }

    public void end() {
        StopCoroutine(syncTime());
    }
}
