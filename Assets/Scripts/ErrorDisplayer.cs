using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ErrorDisplayer : MonoBehaviour
{
    public TextMeshProUGUI logger;
    private static string log;

    void Start() {
        logger.text = log;
    }

    public static void logError(string message) {
        log = log + message + "\n";
    }

    public static void ClearLog() {
        log = "";
    }
}
