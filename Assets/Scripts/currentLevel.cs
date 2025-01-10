using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class currentLevel : MonoBehaviour
{
    public static int _CurrentLevel = 0;
    public static currentLevel Instance { get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
        {
            Instance = this; // 設置實例
            DontDestroyOnLoad(gameObject); // 防止對象在場景切換時被銷毀
        }
        else
        {
            Destroy(gameObject); // 如果已存在實例，銷毀當前對象
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
