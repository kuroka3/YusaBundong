using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class YusaBundongBeatmap
{
    public readonly YusaBundongFile[] yusaBundongFiles;
    public readonly IniObject metaData;
    public readonly AudioClip audioClip;
    public readonly Sprite Jacket = null;

    public YusaBundongBeatmap(string path) {
        if (!Directory.Exists(path) || !File.Exists(path + "\\info.ini")) throw new ArgumentException();

        string[] files = Directory.GetFiles(path, "*.ysb");

        if (files.Length == 0) throw new ArgumentNullException();

        yusaBundongFiles = Array.ConvertAll(files, item => new YusaBundongFile(item));
        metaData = new IniObject(File.ReadAllText(path + "\\info.ini"));

        if (!CheckInfo()) throw new ArgumentNullException();

        audioClip = loadSong(path + "\\" + metaData["Data", "AudioPath"]);
        Jacket = LoadingScene.LoadJacket(path + "\\" + metaData["Data", "JacketPath"]);
    }

    private static readonly string[] NeedKeys = new string[]{
        "BeatmapID",
        "Title",
        "Title_u",
        "Artist",
        "Artist_u",
        "AudioPath",
        "FromBPM",
        "ToBPM",
        "JacketPath"
    };

    private bool CheckInfo() {
        try {
            Dictionary<string, string> dic = metaData["Data"];
            foreach (string needKey in NeedKeys) {
                if (!dic.ContainsKey(needKey)) {
                    return false;
                } else {
                    continue;
                }
            }
            return true;
        } catch {
            return false;
        }
    }

    private static AudioClip loadSong(string path) {
        using UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip("file:///" + path, GetAudioTypeFromExtension(path));
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

    private static AudioType GetAudioTypeFromExtension(string filePath) {
        string extension = Path.GetExtension(filePath).ToLower();

        return extension switch
        {
            ".ogg" => AudioType.OGGVORBIS,
            ".mp3" => AudioType.MPEG,
            ".wav" => AudioType.WAV,
            ".aif" or ".aiff" => AudioType.AIFF,
            ".mod" => AudioType.MOD,
            ".it" => AudioType.IT,
            ".s3m" => AudioType.S3M,
            ".xm" => AudioType.XM,
            _ => AudioType.UNKNOWN,
        };
    }
}
