using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class currentLevel : MonoBehaviour
{
    /*
    這裡只記錄目前關卡，放在首頁的currentLevel物件中。
    且確認它不會被刪除 DontDestroyOnLoad
    當遊戲又回到首頁時，有可能再產生一次這個物件
    所以要確認它不會再被產生
    這是 Singleton模式。
    Singleton 模式是一種設計模式，確保執行專案時只有一個實例，並提供全專案可調用參數。
     */
    public static int _CurrentLevel = 0;
    public static int balls=30;
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
}
