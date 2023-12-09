using System.IO;

public class YusaBundongFile
{

    public readonly string version;
    public readonly string[] chart;

    public YusaBundongFile(OsuFile osuFile) {
        version = osuFile["Metadata"]["Version"];
        chart = ConvertScript.convertChart(osuFile["HitObjects"]);
    }

    public YusaBundongFile(string path) {
        version = path.Split("\\")[^1].Replace(".ysb", "");
        chart = File.ReadAllLines(path);
    }
}
