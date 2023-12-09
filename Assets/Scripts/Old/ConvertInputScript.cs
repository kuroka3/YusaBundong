// using System;
// using System.IO;
// using System.Windows.Forms;
// using Ookii.Dialogs;
// using TMPro;
// using UnityEngine;
// using UnityEngine.SceneManagement;

// public class ConvertInputScript : MonoBehaviour
// {
//     public TMP_InputField title;
//     public TMP_InputField artist;
//     public TMP_InputField bpm;

//     public GameObject[] checks;

//     private string[] Files = new string[3];

//     private VistaOpenFileDialog[] dialogs = new VistaOpenFileDialog[3];
//     private Stream[] openStream = new Stream[3];

//     void Start() {
//         Reset();
//         dialogs[0] = new VistaOpenFileDialog
//         {
//             InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\osu!\\Songs\\",
//             Filter = "osu chart files (*.osu)|*.osu|txt files (*.txt)|*.txt",
//             FilterIndex = 0,
//             Title = "Select Chart file"
//         };

//         dialogs[1] = new VistaOpenFileDialog
//         {
//             InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\osu!\\Songs\\",
//             Filter = "MP3 file (*.mp3)|*.mp3",
//             FilterIndex = 0,
//             Title = "Select Audio file"
//         };

//         dialogs[2] = new VistaOpenFileDialog
//         {
//             InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\osu!\\Songs\\",
//             Filter = "PNG file (*.png)|*.png",
//             FilterIndex = 0,
//             Title = "Select Jacket file"
//         };
//     }

//     public string OpenFile(int code) {
//         VistaOpenFileDialog dialog = dialogs[code];

//         if(dialog.ShowDialog() == DialogResult.OK) {
//             if((openStream[code] = dialog.OpenFile()) != null) {
//                 openStream[code].Close();
//                 return dialog.FileName;
//             }
//         }
//         return null;
//     }

//     public void FindFile(int i) {
//         string result = OpenFile(i);

//         if (string.IsNullOrEmpty(result)) return;

//         checks[i].SetActive(true);
//         Files[i] = result;
//     }

//     public void confirm() {
//         if(string.IsNullOrEmpty(title.text) || string.IsNullOrEmpty(artist.text) || string.IsNullOrEmpty(bpm.text) || string.IsNullOrEmpty(Files[0]) || string.IsNullOrEmpty(Files[1]) || string.IsNullOrEmpty(Files[2])) return;

//         string outputFolder = GameManager.appdata + "\\Songs\\" + title.text;
//         int tryCount = 0;

//         while (Directory.Exists(outputFolder + tryCount)) {
//             tryCount++;
//         }

//         outputFolder += tryCount;

//         Directory.CreateDirectory(outputFolder);

//         ConvertScript.makeSongChartfromOSU(Files[0], title.text, artist.text, int.Parse(bpm.text), Files[2], Files[1], outputFolder);

//         SceneManager.LoadScene(1);
//     }

//     public void cancel() {
//         SceneManager.LoadScene(1);
//     }

//     public void Reset() {
//         title.text = "";
//         artist.text = "";
//         bpm.text = "";
//         Files[0] = null;
//         Files[1] = null;
//         Files[2] = null;
//         checks[0].SetActive(false);
//         checks[1].SetActive(false);
//         checks[2].SetActive(false);
//     }
// }
