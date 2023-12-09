using System;
using System.Collections.Generic;
using System.IO;

public class OsuBeatmap
{

    public readonly OsuFile[] osuFiles;

    public readonly string BeatmapId;
    public readonly string Title;
    public readonly string Title_u;
    public readonly string Artist;
    public readonly string Artist_u;
    public readonly string AudioPath;
    public readonly string FromBPM;
    public readonly string ToBPM;

    public OsuBeatmap(string path) {
        string[] files = Directory.GetFiles(path, "*.osu");
        
        List<OsuFile> tempOsuFile = new();
        foreach(string file in files) {
            OsuFile osuFile = new(file);
            if (osuFile["General"]["Mode"] == "3") tempOsuFile.Add(osuFile);
        }
        osuFiles = tempOsuFile.ToArray();

        if (osuFiles.Length == 0) throw new ArgumentNullException();

        BeatmapId = osuFiles[0]["Metadata"]["BeatmapSetID"];
        Title = osuFiles[0]["Metadata"]["Title"];
        Title_u = osuFiles[0]["Metadata"]["TitleUnicode"];
        Artist = osuFiles[0]["Metadata"]["Artist"];
        Artist_u = osuFiles[0]["Metadata"]["ArtistUnicode"];
        AudioPath = osuFiles[0]["General"]["AudioFilename"];
        
        List<float> BPMsList = new();

        foreach (string element in osuFiles[0]["TimingPoints"].Values) {
            string[] point = element.Split(',');

            if (point[1].ToFloat() > 0) BPMsList.Add(60/(point[1].ToFloat()*0.001f));
        }

        float[] BPMs = sortFloats(BPMsList);

        FromBPM = BPMs[0].ToString();
        ToBPM = BPMs[BPMs.Length-1].ToString();
    }

    public IniObject Data2Ini(string JacketPath) {
        return new IniObject() {
            ["Data", "BeatmapID"] = BeatmapId,
            ["Data", "Title"] = Title,
            ["Data", "Title_u"] = Title_u,
            ["Data", "Artist"] = Artist,
            ["Data", "Artist_u"] = Artist_u,
            ["Data", "AudioPath"] = AudioPath,
            ["Data", "FromBPM"] = FromBPM,
            ["Data", "ToBPM"] = ToBPM,
            ["Data", "JacketPath"] = JacketPath
        };
    }

    public void ConvertOsus() {
        string endPath = GameManager.appdata + "\\Songs\\" + BeatmapId;

        if (Directory.Exists(endPath)) {
            if (Directory.GetFiles(endPath).Length > 0) {
                Directory.Delete(endPath, true);
            }
        }

        if (!Directory.Exists(endPath)) {
            Directory.CreateDirectory(endPath);
        }

        foreach (OsuFile osuFile in osuFiles) {
            YusaBundongFile yusa = new(osuFile);
            File.WriteAllLines(string.Format("{0}\\{1}.ysb", endPath, yusa.version), yusa.chart);
        }
    }

    private float[] sortFloats(List<float> input) {
        float[] data = input.ToArray();
        int N = data.Length;

        for (int i = 0; i < N - 1; i++)
        {
            for (int j = i + 1; j < N; j++)
            {
                if (data[i] > data[j])
                {
                    (data[j], data[i]) = (data[i], data[j]);
                }
            }
        }

        return data;
    }
}
