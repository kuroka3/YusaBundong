using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public GameObject[] prefabs;

    List<GameObject>[] pools;

    void Awake() {
        pools = new List<GameObject>[prefabs.Length];

        for (int i = 0; i < pools.Length; i++) {
            pools[i] = new List<GameObject>();
        }
    }

    public GameObject Get(int index) {
        GameObject returnObj = null;

        foreach (GameObject obj in pools[index]) {
            if (!obj.activeSelf) {
                returnObj = obj;
                returnObj.transform.position = new Vector3(0f, 20f, -10f);
                returnObj.transform.localScale = new Vector3(3.65f, 3);
                
                returnObj.SetActive(true);
                break;
            }
        }

        if (!returnObj) {
            returnObj = Instantiate(prefabs[index], new Vector3(0f, 20f, -10f), Quaternion.identity, transform);
            pools[index].Add(returnObj);
        }

        return returnObj;
    }

    public void Release(GameObject poolGo) {
        poolGo.SetActive(false);
    }
}
