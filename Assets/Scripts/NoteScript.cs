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

    public static NoteScript instance;
    public static float time;

    void Start()
    {
        instance = this;
        startSong(GameManager.songCode);
        syncTimeCour = StartCoroutine(syncTime());
    }

    void Update() {
        if (!audioS.isPlaying) return;
        time += Time.deltaTime;
    }

    private IEnumerator syncTime() {
        while(true) {
            if(audioS.time - time >= 0.04f || time - audioS.time >= 0.04f) {
                time = audioS.time;
            }
            yield return new WaitForSeconds(1f);
        }
    }

    private void startSong(int songid) {
        audioS = gameObject.AddComponent<AudioSource>();
        audioS.clip = GameManager.songs[songid];
        audioS.volume = SettingsManager.volume.ToFloat()*0.01f;

        IEnumerator startAudio() {
            yield return new WaitForSeconds(1);

            audioS.Play();

            yield return new WaitForSeconds(float.Parse(GameManager.datas[songid][3])*0.001f + 3f);

            SceneManager.LoadScene(4);
            StopCoroutine(syncTimeCour);
        }

        IEnumerator NoteSummon() {
            int id = 0;
            int stack = 0;

            for (int i = 0; i<GameManager.charts[songid].Length; i++) {
                string element = GameManager.charts[songid][i];
                try {
                    string[] testData = element.Split(',');
                    int[] testInts = new int[4]{int.Parse(testData[0]), int.Parse(testData[1]), id, int.Parse(testData[2])};
                } catch(Exception e) {
                    Debug.LogError(e);
                    continue;
                }

                GameObject clone = GameManager.instance.pool.Get(0);
                NoteInstScript cloneScript = clone.GetComponent<NoteInstScript>();

                string[] data = element.Split(',');

                cloneScript.sendData(new int[4]{int.Parse(data[0]), int.Parse(data[1]), id, int.Parse(data[2])});

                id++;
                stack++;

                if(stack == 50) {
                    stack = 0;
                    yield return new WaitForSeconds(((float.Parse(data[1])*0.001f)-2)-audioS.time);
                }
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
