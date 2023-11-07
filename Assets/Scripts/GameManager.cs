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

    public static int songCode = 0;
    public static List<int>[] priority = new List<int>[4]{new List<int>(), new List<int>(), new List<int>(), new List<int>()};
    public static GameManager instance;
    public static float[] judgement = new float[4]{0.0417f, 0.0833f, 0.1083f, 0.125f};
    public static string appdata = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\YusaBundong";

    public static string[][] charts;
    public static string[][] datas;
    public static AudioClip[] songs;
    public static Sprite[] jackets;

    void Start() {
        instance = this;
    }

    public static void loadCharts() {
        string songdir = appdata + "\\Songs";

        List<string[]> tmpdatas = new List<string[]>();
        List<string[]> tmpcharts = new List<string[]>();
        List<AudioClip> tmpaudios = new List<AudioClip>();
        List<Sprite> tmpjackets = new List<Sprite>();

        string[] dirs = Directory.GetDirectories(songdir);
        
        foreach (string song in dirs) {
            string[] data = readFileData(song + "\\info.txt");
            string[] chart = readFileData(song + "\\chart.txt");
            AudioClip audio = loadSong(song);
            Sprite jacket = loadJacket(song);

            tmpcharts.Add(chart);
            tmpdatas.Add(data);
            tmpaudios.Add(audio);
            tmpjackets.Add(jacket);
        }

        charts = tmpcharts.ToArray();
        datas = tmpdatas.ToArray();
        songs = tmpaudios.ToArray();
        jackets = tmpjackets.ToArray();

        // foreach (TextAsset chartF in instance.chartFiles) {
        //     StringReader reader = new StringReader(chartF.text);
        //     List<string> tmplist = new List<string>();

        //     string line;
        //     line = reader.ReadLine();
            
        //     while (line != null) {
        //         tmplist.Add(line);
        //         line = reader.ReadLine();
        //     }
        //     reader.Close();

        //     tmpcharts.Add(tmplist.ToArray());
        // }

        // charts = tmpcharts.ToArray();
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
