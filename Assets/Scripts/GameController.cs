using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

/*
 就是Game Master
 
 */
public class GameController : MonoBehaviour
{
    #region 變數定義

    [Header("發射器設置")]
    public GameObject 球預製體;
    public GameObject 發射點;
    public LineRenderer 線渲染器;
    public float 發射速度 = 30f;
    public float 發射間隔 = 0.05f;
    public float 球體大小 = 0.25f;
    public float 球體存在時間 = 5f;

    [Header("遊戲狀態")]
    public bool 正在發射 = false;
    private bool 已下移 = false;
    public bool 已獲勝 = false;
    public bool 已失敗 = false;
    public bool 遊戲進行中 = false;
    public bool 關卡載入完成 = false;
    public bool 特殊關卡 = false;
    private bool 正在瞄準 = false;
    private bool 特殊關卡初次生成 = true;
    private bool 回合結束 = false;

    [Header("UI元素")]
    public Text 遊戲狀態文字;
    public Button 下一關按鈕;
    public Button 返回按鈕;
    public TextMeshPro 球數量文字;
    public TextMeshPro 回合文字;

    [Header("遊戲設置")]
    public int 當前關卡 = 1;
    public int 當前回合 = 0;
    public GameObject 發射器;

    // 私有變數
    private Ray 相機射線;
    private RaycastHit 射線碰撞;
    private Vector3 發射方向;
    private Quaternion 旋轉角度;
    private Vector3 起始位置;
    private float 特殊關卡計時器 = 0f;
    private int 剩餘磚塊數 = 0;
    private const float 磚塊生成間隔 = 2.5f;
    private int 持續發射上限 = 50;

    [SerializeField]
    private LayerMask 層級遮罩;

    #endregion

    #region Unity生命週期

    void Start()
    {
        初始化遊戲();
        起始位置 = 發射器.transform.position;
    }

    void Update()
    {
        處理返回鍵();
        if (!可以遊戲()) return;

        處理遊戲邏輯();
        處理特殊關卡邏輯();

        if ((遊戲進行中 && !已獲勝) || 特殊關卡)
        {
            處理發射邏輯();
        }
    }

    private void OnDestroy()
    {
        processCSV.OnBricksGenerated -= 處理磚塊生成完成;
    }

    #endregion

    #region 初始化

    void 初始化遊戲()
    {
        // 初始化發射器
        if (線渲染器 != null) 線渲染器.enabled = false;

        // 初始化遊戲狀態
        發射器.SetActive(false);
        遊戲進行中 = false;
        已獲勝 = false;
        已失敗 = false;
        特殊關卡 = false;
        特殊關卡計時器 = 0f;

        // 初始化UI
        初始化UI();

        // 訂閱事件
        processCSV.OnBricksGenerated += 處理磚塊生成完成;

        // 載入關卡
        載入遊戲();
    }

    void 初始化UI()
    {
        if (遊戲狀態文字 != null) 遊戲狀態文字.text = "";
        if (下一關按鈕 != null) 下一關按鈕.gameObject.SetActive(false);
        if (返回按鈕 != null) 返回按鈕.gameObject.SetActive(false);
    }

    #endregion

    #region 關卡載入

    void 載入遊戲()
    {
        當前關卡 = currentLevel._CurrentLevel;

        if (當前關卡 > 9)
        {
            當前關卡 = 1;
            currentLevel._CurrentLevel = 1;
        }

        if (currentLevel._CurrentLevel > 0)
        {
            載入一般關卡();
        }
        else if (currentLevel._CurrentLevel == 0)
        {
            載入隨機關卡();
        }
        else if (currentLevel._CurrentLevel == -1)
        {
            載入特殊關卡();
        }

        更新球數量文字();
    }

    void 載入一般關卡()
    {
        GameObject.Find("00GameMaster").GetComponent<processCSV>().getLevel(當前關卡);
        if (回合文字 != null) 回合文字.text = "LEVEL: " + 當前關卡.ToString();
    }

    void 載入隨機關卡()
    {
        if (currentLevel._CurrentLevel < 0) return;

        GetComponent<產生磚塊>().genBricks();
        if (發射器 != null) 發射器.SetActive(true);
        遊戲進行中 = true;
        if (回合文字 != null) 回合文字.text = "RANDOM";
        下移一格();
    }

    void 載入特殊關卡()
    {
        特殊關卡 = true;
        //if (回合文字 != null) 回合文字.text = "KEEPING";
        GameObject.Find("/文字-球數").GetComponent<TextMeshPro>().fontSize = 9;
        if (發射器 != null) 發射器.SetActive(true);
        遊戲進行中 = true;
        更新球數量文字();
    }

    #endregion

    #region 射擊系統

    void 處理發射邏輯()
    {
        if (已失敗 || 已獲勝) return;
        相機射線 = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButton(0))
        {
            if (特殊關卡 || (!正在發射 && !已下移))
            {
                正在瞄準 = true;
            }

            //正在瞄準 = true;
            if (線渲染器 != null)
            {
                線渲染器.enabled = true;
                處理瞄準();
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            
            if (線渲染器 != null) 線渲染器.enabled = false;
            if (!正在發射)
            {
                正在瞄準 = false;
                開始發射();
            }            
        }

        if (!特殊關卡)
        {
            if(!正在瞄準)
                處理球體回收();
        }
    }

    void 處理瞄準()
    {
        if (Physics.Raycast(相機射線, out 射線碰撞, 150f, 層級遮罩))
        {
            if (!正在發射 || 特殊關卡)
            {
                計算射擊軌跡();
            }
        }
    }

    void 計算射擊軌跡()
    {
        Vector3 碰撞點 = 射線碰撞.point;
        Vector3 發射器位置 = 發射點.transform.position;
        碰撞點.y = 0f;
        發射器位置.y = 0f;

        Ray 射擊射線 = new Ray(發射點.transform.position, (碰撞點 - 發射器位置).normalized);
        RaycastHit 牆體碰撞;

        if (Physics.SphereCast(射擊射線, 球體大小, out 牆體碰撞, 150f, 層級遮罩))
        {
            Vector3 第一碰撞點 = 牆體碰撞.point;
            第一碰撞點.y = 0f;

            Vector3 反射方向 = Vector3.Reflect((碰撞點 - 發射器位置).normalized, 牆體碰撞.normal).normalized;
            反射方向.y = 0f;

            Ray 反射射線 = new Ray(第一碰撞點 + 反射方向 * 球體大小, 反射方向);
            RaycastHit 第二次碰撞;
            Vector3 第二碰撞點 = 第一碰撞點 + 反射方向 * 20f;

            if (Physics.Raycast(反射射線, out 第二次碰撞, 20f, 層級遮罩))
            {
                第二碰撞點 = 第二次碰撞.point;
                第二碰撞點.y = 0f;
            }

            更新軌跡線(第一碰撞點, 第二碰撞點);
            更新發射器方向(碰撞點 - 發射器位置);
        }
    }

    void 更新軌跡線(Vector3 第一碰撞點, Vector3 第二碰撞點)
    {
        起始位置 = 發射器.transform.position;
        線渲染器.positionCount = 3;
        線渲染器.SetPosition(0, 起始位置);
        線渲染器.SetPosition(1, 第一碰撞點);
        線渲染器.SetPosition(2, 第二碰撞點);
    }

    void 更新發射器方向(Vector3 方向)
    {
        發射方向 = 方向.normalized;
        旋轉角度 = Quaternion.LookRotation(發射方向, Vector3.up);
        發射器.transform.rotation = 旋轉角度;
        發射器.transform.eulerAngles = new Vector3(0f, 發射器.transform.eulerAngles.y, 0f);
    }

    void 開始發射()
    {
        if (線渲染器 != null)
        {
            Vector3 線方向 = (線渲染器.GetPosition(1) - 線渲染器.GetPosition(0)).normalized;
            線方向.y = 0f;
            線渲染器.enabled = false;

            旋轉角度 = Quaternion.LookRotation(線方向, Vector3.up);
            發射器.transform.rotation = 旋轉角度;
            發射器.transform.eulerAngles = new Vector3(0f, 發射器.transform.eulerAngles.y, 0f);
        }

        正在發射 = true;
        已下移 = true;
        StartCoroutine(發射序列());
        當前回合++;
    }

    IEnumerator 發射序列()
    {
        if (特殊關卡)
        {
            while (!已獲勝 && 遊戲進行中)
            {
                if (已獲勝) break;
                建立球體();
                yield return new WaitForSeconds(發射間隔);
            }
        }
        else
        {
            for (int i = 0; i < currentLevel.balls && !已獲勝; i++)
            {
                建立球體();
                yield return new WaitForSeconds(發射間隔);
            }
        }

        正在發射 = false;
    }

    void 建立球體()
    {
        if (球預製體 == null || 發射點 == null) return;

        GameObject 球 = Instantiate(球預製體, 發射點.transform.position, Quaternion.identity);
        球.transform.localScale = new Vector3(球體大小, 球體大小, 球體大小);
        球.transform.forward = 發射方向;
        球.transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);

        if (球.TryGetComponent<Rigidbody>(out Rigidbody 剛體))
        {
            剛體.velocity = 發射方向 * 發射速度;
        }

        Destroy(球, 球體存在時間);
    }

    #endregion

    #region 遊戲邏輯

    void 處理遊戲邏輯()
    {
        if (遊戲進行中)
        {
            更新UI();
            檢查勝利條件();
        }
        處理遊戲結束狀態();
    }

    void 處理特殊關卡邏輯()
    {
        if (!特殊關卡) return;

        if (特殊關卡初次生成)
        {
            GameObject.Find("00GameMaster").GetComponent<產生磚塊>().genBricks();
            下移一格();
            特殊關卡初次生成 = false;
        }
        if (!遊戲進行中) return;
        特殊關卡計時器 += Time.deltaTime;
        if (特殊關卡計時器 >= 磚塊生成間隔)
        {
            if (當前回合 < 持續發射上限)
            {
                GameObject.Find("00GameMaster").GetComponent<產生磚塊>().genBricks();
                當前回合++;
            }

            下移一格();
            更新回合文字();
            特殊關卡計時器 = 0f;
        }

        if (當前回合 >= 持續發射上限)
        {
            檢查勝利條件();
        }
    }

    void 處理球體回收()
    {
        if (GameObject.FindGameObjectsWithTag("BALL").Length == 0)
        {
            正在發射 = false;
            var 下牆 = GameObject.Find("/牆/下").GetComponent<回收球>();

            if (已下移)
            {
                下牆.移動發射器();
                下移一格();
                已下移 = false;
            }
            else
            {
                下牆.移動發射器(); // 確保每次都會移動發射器
            }
        }
    }

    public void 下移一格()
    {
        StartCoroutine(下移一格協程());
    }

    IEnumerator 下移一格協程()
    {
        GameObject[] 磚塊們 = GameObject.FindGameObjectsWithTag("BRICKS");
        List<Coroutine> 移動協程列表 = new List<Coroutine>();

        foreach (GameObject 磚塊 in 磚塊們)
        {
            if (磚塊 != null)
                {
                    處理磚塊移動(磚塊, 移動協程列表);
                }
        }

        foreach (Coroutine 協程 in 移動協程列表)
        {
            yield return 協程;
        }

        if (currentLevel._CurrentLevel == 0 && 當前回合 < 31)
        {
            var 磚塊生成器 = GameObject.Find("/00GameMaster")?.GetComponent<產生磚塊>();
            磚塊生成器?.genBricks();
        }
    }

    void 處理磚塊移動(GameObject 磚塊, List<Coroutine> 移動協程列表)
    {
        Vector3 新位置 = 磚塊.transform.position;
        新位置.z -= 1;
        移動協程列表.Add(StartCoroutine(移動下降序列(磚塊, 新位置)));

        if (新位置.z < 0)
        {
            處理超出邊界磚塊(磚塊);
        }

        var 資源組件 = 磚塊.GetComponent<資源>();
        if (資源組件 != null && 資源組件.最後要刪除)
        {
            Destroy(磚塊);
        }
    }

    void 處理超出邊界磚塊(GameObject 磚塊)
    {
        if (磚塊.name == "AddBall(Clone)" ||
            磚塊.name == "Spread(Clone)" ||
            磚塊.name == "+(Clone)" ||
            磚塊.name == "-(Clone)")
        {
            Destroy(磚塊);
        }
        else
        {
            已失敗 = true;
        }
    }

    IEnumerator 移動下降序列(GameObject 物體, Vector3 目標位置)
    {
        if (物體 == null) yield break;

        float 持續時間 = 0.5f;
        float 經過時間 = 0f;
        Vector3 起始位置 = 物體.transform.position;

        while (經過時間 < 持續時間 && 物體 != null)
        {
            經過時間 += Time.deltaTime;
            float 進度 = 經過時間 / 持續時間;
            物體.transform.position = Vector3.Lerp(起始位置, 目標位置, 進度);
            yield return null;
        }

        if (物體 != null)
        {
            物體.transform.position = 目標位置;
        }
    }

    #endregion

    #region UI更新

    void 更新UI()
    {
        更新球數量文字();
        if ((currentLevel._CurrentLevel == 0 || 特殊關卡) && 回合文字 != null)
        {
            更新回合文字();
        }
    }

    void 更新球數量文字()
    {
        if (球數量文字 != null)
        {
            球數量文字.text = 特殊關卡 ? "KEEP SHOOTING" : $"BALL: {currentLevel.balls}";
        }
    }

    void 更新回合文字()
    {
        if (回合文字 != null)
        {
            回合文字.text = $"ROUND: {當前回合}";
        }
    }

    #endregion

    #region 遊戲狀態檢查

    bool 可以遊戲()
    {
        if (currentLevel._CurrentLevel > 0 && !關卡載入完成)
        {
            遊戲進行中 = false;
            return false;
        }
        return true;
    }

    void 檢查勝利條件()
    {
        if (特殊關卡 && 當前回合 < 持續發射上限)
        {
            return;
        }

        計算剩餘磚塊();

        if (剩餘磚塊數 == 0)
        {
            if (!特殊關卡 || (特殊關卡 && 當前回合 >= 持續發射上限))
            {
                已獲勝 = true;
            }
        }
    }

    void 計算剩餘磚塊()
    {
        GameObject[] 磚塊們 = GameObject.FindGameObjectsWithTag("BRICKS");
        剩餘磚塊數 = 磚塊們.Length;

        foreach (GameObject 磚塊 in 磚塊們)
        {
            if (磚塊.name == "AddBall(Clone)" ||
                磚塊.name == "Spread(Clone)" ||
                磚塊.name == "-(Clone)" ||
                磚塊.name == "+(Clone)")
            {
                剩餘磚塊數--;
            }
        }
    }

    void 處理遊戲結束狀態()
    {
        if (已獲勝)
        {
            處理勝利狀態();
        }
        else if (已失敗)
        {
            處理失敗狀態();
        }
    }

    void 處理勝利狀態()
    {
        if (發射器 != null) 發射器.SetActive(false);
        if (遊戲狀態文字 != null) 遊戲狀態文字.text = "WIN";
        if (下一關按鈕 != null) 下一關按鈕.gameObject.SetActive(true);
        if (返回按鈕 != null) 返回按鈕.gameObject.SetActive(true);

        遊戲進行中 = false;
        銷毀所有球體();
    }

    void 處理失敗狀態()
    {
        if (遊戲狀態文字 != null) 遊戲狀態文字.text = "LOSE";
        if (返回按鈕 != null) 返回按鈕.gameObject.SetActive(true);
        遊戲進行中 = false;
    }

    void 銷毀所有球體()
    {
        GameObject[] 球們 = GameObject.FindGameObjectsWithTag("BALL");
        foreach (GameObject 球 in 球們)
        {
            Destroy(球);
        }
    }

    #endregion

    #region 輸入處理

    void 處理返回鍵()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            SceneManager.LoadScene("01_選擇關卡");
        }
    }

    #endregion

    #region 事件處理

    private void 處理磚塊生成完成()
    {
        StartCoroutine(設置遊戲狀態());
    }

    private IEnumerator 設置遊戲狀態()
    {
        yield return new WaitForSeconds(1f);
        if (發射器 != null) 發射器.SetActive(true);
        遊戲進行中 = true;
        關卡載入完成 = true;
    }

    #endregion
}