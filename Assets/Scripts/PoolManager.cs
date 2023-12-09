using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public GameObject[] prefabs;

    List<GameObject>[] pools;

    public int[] MaxObjects;

    void Awake() {
        MaxObjects[0] = SettingsManager.GetAsInt(Settings.MaxObject);

        pools = new List<GameObject>[prefabs.Length];

        for (int i = 0; i < pools.Length; i++) {
            pools[i] = new List<GameObject>();

            for (int j = 0; j < MaxObjects[i]; j++) {
                GameObject gameObject = Instantiate(prefabs[i], new Vector3(0f, 20f, -10f), Quaternion.identity, transform);
                pools[i].Add(gameObject);
            }

            ReleaseAll(i);
        }
    }

    public GameObject Get(int index) {
        GameObject returnObj = null;

        foreach (GameObject obj in pools[index]) {
            if (!obj.activeSelf) {
                returnObj = obj;
                returnObj.transform.position = new Vector3(0f, 20f, -10f);
                returnObj.transform.localScale = new Vector3(2f, 0.7f);
                
                returnObj.SetActive(true);
                break;
            }
        }

        return returnObj;
    }

    public void Release(GameObject poolGo) {
        poolGo.SetActive(false);
    }

    public void ReleaseAll(int index) {
        foreach (GameObject obj in pools[index]) {
            if(!obj.IsDestroyed()) {
                obj.SetActive(false);
                NoteInstScript objScript = obj.GetComponent<NoteInstScript>();
                objScript.clear();
            }
        }
    }
}
