using System;
using System.Collections.Generic;
using System.IO;

public class OsuFile
{
    private Dictionary<string, Dictionary<string, string>> data = null;

    public OsuFile(string path) {
        data = ParseOsu(File.ReadAllText(path));
    }

    public Dictionary<string, string> this[string section] {
        get {
            if (data.ContainsKey(section)) {
                return data[section];
            }
            return null;
        }
    }

    private Dictionary<string, Dictionary<string, string>> ParseOsu(string OsuString) {
        Dictionary<string, Dictionary<string, string>> entries = new Dictionary<string, Dictionary<string, string>>();

        Dictionary<string, string> currentSection = new Dictionary<string, string>();

        string[] lines = OsuString.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

        string sectionName = "";
        int listInt = 0;
        
        for (int i = 0; i<lines.Length; i++) {
            string line = lines[i];

            if (!string.IsNullOrEmpty(line) && !line.StartsWith("//")) {

                if (line.StartsWith('\u005b') && line.EndsWith('\u005d')) {
                    sectionName = line.Substring(1, line.Length - 2);

                    currentSection = new Dictionary<string, string>();
                    listInt = 0;
                    entries.Add(sectionName, currentSection);
                } else {
                    switch (sectionName) {
                        case "General":
                        case "Editor": {
                            string[] keyValue = line.Split(": ");

                            if (keyValue.Length == 2) {
                                currentSection.Add(keyValue[0].Trim(), keyValue[1].Trim());
                            }
                            break;
                        }

                        case "Metadata":
                        case "Difficulty": {
                            string[] keyValue = line.Split(":");

                            if (keyValue.Length == 2) {
                                currentSection.Add(keyValue[0].Trim(), keyValue[1].Trim());
                            }
                            break;
                        }

                        case "Colours": {
                            string[] keyValue = line.Split(" : ");

                            if (keyValue.Length == 2) {
                                currentSection.Add(keyValue[0].Trim(), keyValue[1].Trim());
                            }
                            break;
                        }

                        case "Events":
                        case "TimingPoints":
                        case "HitObjects": {
                            string[] keyValue = new string[]{listInt.ToString(), line};

                            if (keyValue.Length == 2) {
                                currentSection.Add(keyValue[0].Trim(), keyValue[1].Trim());
                                listInt++;
                            }
                            break;
                        }
                    }
                }
            }
        }

        return entries;
    }
}
