using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public AudioClip[] audioS;
    public PoolManager pool;
    public TextAsset[] chartFiles;
    public int offset;
    public KeyCode[] keys;

    public static int songCode = 0;
    public static List<int>[] priority = new List<int>[4]{new List<int>(), new List<int>(), new List<int>(), new List<int>()};
    public static string[][] charts;
    public static GameManager instance;
    public static float[] judgement = new float[4]{0.0417f, 0.0833f, 0.1083f, 0.125f};

    void Start() {
        instance = this;
    }

    public static void loadCharts() {
        List<string[]> tmpcharts = new List<string[]>();

        foreach (TextAsset chartF in instance.chartFiles) {
            StringReader reader = new StringReader(chartF.text);
            List<string> tmplist = new List<string>();

            string line;
            line = reader.ReadLine();
            
            while (line != null) {
                tmplist.Add(line);
                line = reader.ReadLine();
            }
            reader.Close();

            tmpcharts.Add(tmplist.ToArray());
        }

        charts = tmpcharts.ToArray();
    }
}
