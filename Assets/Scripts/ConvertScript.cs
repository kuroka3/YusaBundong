using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class ConvertScript : MonoBehaviour
{
    public static bool makeSongChartfromOSU(string target, string title, string artist, int bpm, string jacketPath, string audioPath, string endPath) {
        try {
            if(!Directory.Exists(GameManager.tmpFolder)) Directory.CreateDirectory(GameManager.tmpFolder);
            string chartFileName = GameManager.tmpFolder + "\\" + Guid.NewGuid().ToString() + ".tmp";
            string[] chart = convertChart(target);
            File.WriteAllLines(chartFileName, chart);

            int endTime = 0;

            int fromlength = 1;
            while(string.IsNullOrEmpty(chart[chart.Length-fromlength])) fromlength++;

            string[] lastLineData = chart[chart.Length-fromlength].Split(",");

            if(lastLineData[2].Equals("-1")) endTime = int.Parse(lastLineData[1]);
            else endTime = int.Parse(lastLineData[2]); 

            return makeSongChart(chartFileName, title, artist, bpm, endTime, jacketPath, audioPath, endPath);
        } catch (Exception e) {
            Debug.LogError(e);
            return false;
        }
    }

    public static bool makeSongChart(string target, string title, string artist, int bpm, int endTime, string jacketPath, string audioPath, string endPath, bool DeleteOnEnd = false) {
        try {
            if (!Directory.Exists(endPath)) Directory.CreateDirectory(endPath);

            // chart.txt
            File.Copy(target, endPath + "\\chart.txt");

            // info.txt
            File.WriteAllText(endPath + "\\info.txt", title + "\n" + artist + "\n" + bpm + "\n" + endTime);

            // jacket.png
            File.Copy(jacketPath, endPath + "\\jacket.png");

            // audio.mp3
            File.Copy(audioPath, endPath + "\\audio.mp3");

            if(DeleteOnEnd) File.Delete(target);

            return true;
        } catch (Exception e) {
            Debug.LogError(e);
            return false;
        }
    }

    private static string[] convertChart(string target) {
        string[] text = File.ReadAllLines(target);
        int line = Array.IndexOf(text, "[HitObjects]");
        
        if (line == -1 || line >= text.Length) return null;

        List<string> returnList = new();

        for (int i = line+1; i<text.Length; i++) {
            string[] data = text[i].Split(",");

            int endLine = -1;

            if (data[3] == "128") {
                string[] data2 = data[5].Split(":").Where(x => !string.IsNullOrEmpty(x)).ToArray();
                endLine = int.Parse(data2[0]);
            }

            string key = (int.Parse(data[0]) * 4 / 512).ToString();

            string returnValue = key + "," + data[2] + "," + endLine;
            returnList.Add(returnValue);
        }

        return returnList.ToArray();
    }
}
