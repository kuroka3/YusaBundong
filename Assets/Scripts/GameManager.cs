using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour
{
    public PoolManager pool;
    public static int offset;
    public static float hispeed;

    public static int songCode = 0;
    public static List<int>[] priority = new List<int>[4]{new List<int>(), new List<int>(), new List<int>(), new List<int>()};
    public static GameManager instance;
    public static float[] judgement = new float[4]{0.08f, 0.12f, 0.16f, 0.18f};
    public static string appdata = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\YusaBundong";
    public static string tmpFolder = appdata + "\\tmp";
    public static KeyCode[] keys;

    public static string[][] charts;
    public static string[][] datas;
    public static AudioClip[] songs;
    public static Sprite[] jackets;

    public static double score = 0L;
    public static long combo = 0L;
    public static float acc = 0f;
    public static List<float> judgeHis = new List<float>();
    public static int[] judges = new int[5]{0, 0, 0, 0, 0};

    public static bool paused = false;

    void Awake() {
        instance = this;
    }

    public static bool loadCharts() {
        if(!Directory.Exists(appdata)) Directory.CreateDirectory(appdata);

        string songdir = appdata + "\\Songs";

        if(!Directory.Exists(songdir)) Directory.CreateDirectory(songdir);

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

            if(checkInfo(data) && checkChart(chart) && audio != null && jacket != null) {
                tmpcharts.Add(chart);
                tmpdatas.Add(data);
                tmpaudios.Add(audio);
                tmpjackets.Add(jacket);
            } else {
                Debug.LogError("Cannot load song: " + song);
                ErrorDisplayer.logError("Cannot load song: " + song);
            }
            ProgressBar.value = tmpcharts.ToArray().Length/dirs.Length;
        }

        charts = tmpcharts.ToArray();
        datas = tmpdatas.ToArray();
        songs = tmpaudios.ToArray();
        jackets = tmpjackets.ToArray();

        return charts.Length > 0;
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
        try {
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
        } catch(Exception e) {
            Debug.LogError(e);
            return null;
        }
    }

    public static void clear() {
        score = 0L;
        combo = 0L;
        acc = 0f;
        judgeHis.Clear();
        for (int i = 0; i<judges.Length; i++) {
            judges[i] = 0;
        }
        for (int i = 0; i<priority.Length; i++) {
            priority[i].Clear();
        }
        instance.pool.ReleaseAll(0);
    }

    private static bool checkInfo(string[] data) {
        try {
            string name = data[0];
            string artist = data[1];
            int bpm = int.Parse(data[2]);
            float endTime = float.Parse(data[3]);

            return true;
        } catch (Exception e) {
            Debug.LogError(e);
            return false;
        }
    }

    private static bool checkChart(string[] chart) {
        try {
            for (int i = 0; i<chart.Length; i++) {
                string element = chart[i];
                if(element.Equals("")) continue;
                string[] testData = element.Split(',');
                int[] testInts = new int[3]{int.Parse(testData[0]), int.Parse(testData[1]), int.Parse(testData[2])};
            }
            return true;
        } catch (Exception e) {
            Debug.LogError(e);
            return false;
        }
    }

    public static void loadSettings() {
        keys = new KeyCode[4];
        string settingsFile = appdata + "\\settings.txt";
        if (!File.Exists(settingsFile)) File.WriteAllLines(settingsFile, new string[]{((int) KeyCode.D).ToString(), ((int) KeyCode.F).ToString(), ((int) KeyCode.J).ToString(), ((int) KeyCode.K).ToString(), 0.ToString(), (31.32075f).ToString()});
        string[] datas = File.ReadAllLines(settingsFile);

        for (int i = 0; i<4; i++) {
            int j = int.Parse(datas[i]);
            keys[i] = (KeyCode) j;
        }
        
        offset = int.Parse(long.Parse(datas[4]).ToString());
        hispeed = float.Parse(double.Parse(datas[5]).ToString());
    }
}
