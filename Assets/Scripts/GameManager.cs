using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour
{
    public PoolManager pool;
    public int offset;
    public KeyCode[] keys;
    public float hispeed;

    public static int songCode = 0;
    public static List<int>[] priority = new List<int>[4]{new List<int>(), new List<int>(), new List<int>(), new List<int>()};
    public static GameManager instance;
    public static float[] judgement = new float[4]{0.08f, 0.12f, 0.16f, 0.18f};
    public static string appdata = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\YusaBundong";

    public static string[][] charts;
    public static string[][] datas;
    public static AudioClip[] songs;
    public static Sprite[] jackets;

    public static double score = 0L;
    public static long combo = 0L;
    public static float acc = 0f;
    public static List<float> judgeHis = new List<float>();

    void Awake() {
        instance = this;
    }

    public static void loadCharts() {
        string songdir = appdata + "\\Songs";

        List<string[]> tmpdatas = new List<string[]>();
        List<string[]> tmpcharts = new List<string[]>();
        List<AudioClip> tmpaudios = new List<AudioClip>();
        List<Sprite> tmpjackets = new List<Sprite>();

        string[] dirs = Directory.GetDirectories(songdir);
        
        for (int i = 0; i<dirs.Length; i++) {
            string song = dirs[i];
            string[] data = readFileData(song + "\\info.txt");
            string[] chart = readFileData(song + "\\chart.txt");
            AudioClip audio = loadSong(song);
            Sprite jacket = loadJacket(song);

            tmpcharts.Add(chart);
            tmpdatas.Add(data);
            tmpaudios.Add(audio);
            tmpjackets.Add(jacket);

            ProgressBar.value = tmpcharts.ToArray().Length/dirs.Length;
        }

        charts = tmpcharts.ToArray();
        datas = tmpdatas.ToArray();
        songs = tmpaudios.ToArray();
        jackets = tmpjackets.ToArray();
    }

    private static string[] readFileData(string path) {
        string[] infos = File.ReadAllText(path).Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        return infos;
    }

    private static AudioClip loadSong(string path) {
        using (UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip("file:///" + path + "\\audio.mp3", AudioType.MPEG))
        {
            request.SendWebRequest();

            while (!request.isDone)
            {
                continue;
            }

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
                return null;
            }

            return DownloadHandlerAudioClip.GetContent(request);
        }
    }

    private static Sprite loadJacket(string path) {
        string filePath = path + "\\jacket.png";

        Texture2D texture = null;
        byte[] fileData;

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            texture.LoadImage(fileData);
        }
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));;
    }
}
