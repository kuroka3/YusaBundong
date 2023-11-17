using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScript : MonoBehaviour
{
    void Start() {
        Application.targetFrameRate = 144;
        // ConvertScript.makeSongChartfromOSU("C:\\Users\\kurokami\\AppData\\Local\\osu!\\Songs\\1466713 Hoshimachi Suisei - GHOST\\Hoshimachi Suisei - GHOST (Rivals_7) [Cosmos].osu", "GHOST", "Hoshimachi Suisei", 182, 272758, "C:\\Users\\kurokami\\Downloads\\w3iKoDXt4FjXGW12MDFwW1D7MDK4Gw_DkRpvJmC_NnY61yPnJ-sYCq7P12lpO3h4vFN_cQurHp3ORrckBvC-q2dL1WeBgPEYi0UQVO0YsGDF049UBUDw1skLktBZ2vblUD7fTTIJgJDt4O-OFhkaVg.png", "C:\\Users\\kurokami\\AppData\\Local\\osu!\\Songs\\1466713 Hoshimachi Suisei - GHOST\\audio.mp3", GameManager.appdata + "\\Songs\\GHOST");
    }

    void Update() {
        
        if(Input.GetMouseButtonDown(0)) {
            SceneManager.LoadScene(1);
        }
    }
}
