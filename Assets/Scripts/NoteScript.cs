using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NoteScript : MonoBehaviour
{
    public GameObject prefab;

    private AudioSource audioS;
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
            if(Math.Abs(audioS.time - time) >= LimitErrorFloat) {
                time = audioS.time;
            }
            yield return new WaitForSeconds(1f);
        }
    }

    private void startSong(int songid) {
        time = -1f;

        audioS = gameObject.AddComponent<AudioSource>();
        audioS.clip = GameManager.songs[songid];
        audioS.volume = SettingsManager.volume.ToFloat()*0.01f;

        IEnumerator startAudio() {
            yield return new WaitForSeconds(1);

            audioS.Play();
            syncTimeCour = StartCoroutine(syncTime());

            yield return new WaitForSeconds(float.Parse(GameManager.datas[songid][3])*0.001f + 3f);

            SceneManager.LoadScene(4);
            StopCoroutine(syncTimeCour);
        }

        IEnumerator NoteSummon() {
            yield return new AsyncOperation();

            int id = 0;
            // int stack = 0;

            for (int i = 0; i<GameManager.charts[songid].Length; i++) {
                GameObject clone;

                while (true) {
                    clone = GameManager.instance.pool.Get(0);

                    if (clone == null) {
                        yield return new WaitForSeconds(0.1f);
                    } else {
                        break;
                    }
                }

                string Element = GameManager.charts[songid][i];
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

                // print(Data2Int[0] + ", " + Data2Int[1] + ", " + id + ", " + Data2Int[2] + ", ");

                id++;
                // stack++;

                // if(stack == 50) {
                //     stack = 0;
                //     yield return new WaitForSeconds(((float.Parse(data[1])*0.001f)-2)-audioS.time);
                // }
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
