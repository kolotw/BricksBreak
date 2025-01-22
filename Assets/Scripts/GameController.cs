using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameController : MonoBehaviour
{
    // 發射器相關變數
    private Ray cameraRay;
    private RaycastHit hit;
    [SerializeField] private LayerMask layerMask;
    private Vector3 shootDirection;
    private Quaternion rotation;
    [Header("發射器設置")]
    public GameObject ballPrefab;
    public GameObject shootPoint;
    public LineRenderer lineRenderer;
    public float shootSpeed = 30f;
    public float shootInterval = 0.05f;
    public float ballSize = 0.25f;
    public float timeOut = 5f;

    [Header("遊戲狀態")]
    public bool isShooting = false;
    private bool isDown = false;
    public bool isWon = false;
    public bool isLost = false;
    public bool isPlaying = false;
    public bool levelHasLoadCompleted = false;
    public bool isSpecialLevel = false;
    private float specialLevelTimer = 0f;
    private const float BRICK_SPAWN_INTERVAL = 3f;
    private int keepShootingLimit = 50;

    [Header("UI元素")]
    public Text gameStatusText;
    public Button nextButton;
    public Button backButton;
    public TextMeshPro ballCountText;
    public TextMeshPro roundText;

    [Header("遊戲設置")]
    public int nowLevel = 1;
    public int currentRound = 0;
    private int totalBricks = 0;
    public GameObject shooter;
    private Vector3 startPos;

    void Start()
    {
        InitializeGame();
        startPos = shooter.transform.position;
    }

    void InitializeGame()
    {
        // 初始化發射器設置
        if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
        }
        // 初始化遊戲狀態
        shooter.SetActive(false);
        isPlaying = false;
        isWon = false;
        isLost = false;
        isSpecialLevel = false;
        specialLevelTimer = 0f;

        // 初始化UI
        if (gameStatusText != null) gameStatusText.text = "";
        if (nextButton != null) nextButton.gameObject.SetActive(false);
        if (backButton != null) backButton.gameObject.SetActive(false);

        // 訂閱事件
        processCSV.OnBricksGenerated += HandleBricksGenerated;

        // 開始載入關卡
        LoadGamePlay();
    }

    void LoadGamePlay()
    {
        nowLevel = currentLevel._CurrentLevel;
        if (nowLevel > 9)
        {
            nowLevel = 1;
            currentLevel._CurrentLevel = 1;
        }

        if (currentLevel._CurrentLevel > 0)
        {            
            GameObject.Find("00GameMaster").GetComponent<processCSV>().getLevel(nowLevel);
            if (roundText != null) roundText.text = "LEVEL: " + nowLevel.ToString();
        }
        else if(currentLevel._CurrentLevel == 0)
        {
            if(currentLevel._CurrentLevel < 0) return;
            GetComponent<產生磚塊>().genBricks();
            if (shooter != null) shooter.SetActive(true);
            isPlaying = true;
            if (roundText != null) roundText.text = "LEVEL: Random";
            downOne();
        }
        else if (currentLevel._CurrentLevel == -1)  // 特殊關卡判斷
        {
            isSpecialLevel = true;
            if (roundText != null) roundText.text = "LEVEL: Special";
            if (shooter != null) shooter.SetActive(true);
            isPlaying = true;
            GameObject.Find("00GameMaster").GetComponent<產生磚塊>().genBricks();
            //currentLevel.balls = 999; // 設置較大的數值表示無限
            UpdateBallCountText();
            return;
        }
        else
        {

        }

        UpdateBallCountText();
    }

    void Update()
    {
        HandleEscapeKey();
        if (!CanPlay()) return;
        HandleGameplayLogic();

        // 特殊關卡的更新邏輯
        if (isSpecialLevel && isPlaying)
        {
            specialLevelTimer += Time.deltaTime;
            if (specialLevelTimer >= BRICK_SPAWN_INTERVAL)
            {
                if (currentRound < keepShootingLimit) // 只在100回合前生成新磚塊
                {
                    GameObject.Find("00GameMaster").GetComponent<產生磚塊>().genBricks();
                    currentRound++;
                }

                // 無論是否達到100回合都執行 downOne
                downOne();

                // 更新UI
                if (roundText != null)
                {
                    roundText.text = "Round: " + currentRound.ToString();
                }

                specialLevelTimer = 0f;
            }

            // 在回合數達到100後，持續檢查獲勝條件
            if (currentRound >= keepShootingLimit)
            {
                CheckWinCondition(); // 持續檢查是否獲勝
            }
        }

        // 只有在遊戲進行中且不是獲勝狀態時才處理射擊邏輯
        if (isPlaying && !isWon)
        {
            HandleShootingLogic();
        }
        if (isSpecialLevel)
        {
            HandleShootingLogic();
        }
    }

    bool CanPlay()
    {
        if (currentLevel._CurrentLevel > 0 && !levelHasLoadCompleted)
        {
            isPlaying = false;
            return false;
        }
        return true;
    }

    void HandleGameplayLogic()
    {
        if (isPlaying)
        {
            UpdateUI();
            CheckWinCondition();
        }
        HandleGameEndStates();
    }

    void HandleShootingLogic()
    {
        // 獲取攝像機射線
        cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        // 處理瞄準邏輯
        if (Input.GetMouseButton(0))
        {
            if (lineRenderer != null)
            {
                lineRenderer.enabled = true;
                HandleAiming();               
            }
        }
        else if (Input.GetMouseButtonUp(0) && !isShooting)
        {
            if (lineRenderer != null)
            {
                lineRenderer.enabled = false;
            }
            StartShooting();
        }

        // 如果不是特殊關卡，則需要檢查球是否在場上
        if (!isSpecialLevel)
        {
            HandleBallRecycling();
        }
    }

    void HandleBallRecycling()
    {
        if (GameObject.FindGameObjectsWithTag("BALL").Length == 0)
        {
            isShooting = false;
            GameObject.Find("/牆/下").GetComponent<回收球>().移動發射器();

            if (isDown)
            {
                downOne();
                isDown = false;
            }
        }
    }

    void HandleAiming()
    {
        if (Physics.Raycast(cameraRay, out hit, 150f, layerMask))
        {
            Vector3 hitPoint = hit.point;
            Vector3 shooterPosition = shootPoint.transform.position;
            hitPoint.y = 0f;
            shooterPosition.y = 0f;

            if (!isShooting || isSpecialLevel)
            {
                Ray shootRay = new Ray(shootPoint.transform.position, (hitPoint - shooterPosition).normalized);
                RaycastHit wallHit;
                if (Physics.SphereCast(shootRay, ballSize, out wallHit, 150f, layerMask))
                {
                    // 第一次碰撞點
                    Vector3 collisionPoint = wallHit.point;
                    collisionPoint.y = 0f;

                    // 計算反射方向
                    Vector3 reflectDirection = Vector3.Reflect((hitPoint - shooterPosition).normalized, wallHit.normal).normalized;
                    reflectDirection.y = 0f;

                    // 計算第二次碰撞
                    Ray reflectedRay = new Ray(collisionPoint + reflectDirection * ballSize, reflectDirection);
                    RaycastHit secondHit;
                    Vector3 secondHitPoint = collisionPoint + reflectDirection * 20f;

                    if (Physics.Raycast(reflectedRay, out secondHit, 20f, layerMask))
                    {
                        secondHitPoint = secondHit.point;
                        secondHitPoint.y = 0f;
                    }

                    // 設置 lineRenderer
                    startPos = shooter.transform.position;
                    lineRenderer.positionCount = 3;
                    lineRenderer.SetPosition(0, startPos);
                    lineRenderer.SetPosition(1, collisionPoint);
                    lineRenderer.SetPosition(2, secondHitPoint);

                    // 使用紅線的方向 (原始射擊方向)
                    Vector3 redLineDirection = (hitPoint - shooterPosition).normalized;
                    shootDirection = redLineDirection;

                    rotation = Quaternion.LookRotation(shootDirection, Vector3.up);
                    shooter.transform.rotation = rotation;
                    shooter.transform.eulerAngles = new Vector3(0f, shooter.transform.eulerAngles.y, 0f);

                    // Debug射線
                    Debug.DrawRay(shooter.transform.position, shootDirection * 10f, Color.red);
                    Debug.DrawRay(shooter.transform.position, shooter.transform.forward * 10f, Color.blue);
                }
            }
        }
    }

    void UpdateShooterRotation()
    {
        rotation = Quaternion.LookRotation(shootDirection, Vector3.up);
        shooter.transform.rotation = rotation;
        shooter.transform.eulerAngles = new Vector3(0f, shooter.transform.eulerAngles.y, 0f);

        // 畫出射擊方向（紅色）和 shooter 的前方向（藍色）
        Debug.DrawRay(shooter.transform.position, shootDirection * 10f, Color.red);
        Debug.DrawRay(shooter.transform.position, shooter.transform.forward * 10f, Color.blue);
    }

    void StartShooting()
    {
        if (lineRenderer != null)
        {
            // 在關閉 lineRenderer 之前先取得它的方向
            Vector3 lineDirection = (lineRenderer.GetPosition(1) - lineRenderer.GetPosition(0)).normalized;
            lineDirection.y = 0f;

            lineRenderer.enabled = false;

            rotation = Quaternion.LookRotation(lineDirection, Vector3.up);
            shooter.transform.rotation = rotation;
            shooter.transform.eulerAngles = new Vector3(0f, shooter.transform.eulerAngles.y, 0f);
        }

        isShooting = true;
        isDown = true;
        StartCoroutine(ShootSequence());
        currentRound++;
    }

    IEnumerator ShootSequence()
    {
        if (isSpecialLevel)
        {
            while (!isWon && isPlaying)
            {
                if (isWon)
                {
                    isShooting = false;
                    yield break;
                }
                CreateBall();
                yield return new WaitForSeconds(shootInterval);
            }
        }
        else
        {
            for (int i = 0; i < currentLevel.balls; i++)
            {
                if (isWon)
                {
                    isShooting = false;
                    yield break;
                }
                CreateBall();
                yield return new WaitForSeconds(shootInterval);
            }
        }
        isShooting = false;
    }

    void CreateBall()
    {
        if (ballPrefab == null || shootPoint == null)
        {
            return;
        }
        // 創建球體
        GameObject ball = Instantiate(ballPrefab, shootPoint.transform.position, Quaternion.identity);
        ball.transform.localScale = new Vector3(ballSize, ballSize, ballSize);
        // 設置球體的方向和速度
        ball.transform.forward = shootDirection;
        ball.transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = shootDirection * shootSpeed;
        }
        // 設置銷毀時間
        Destroy(ball, timeOut);
    }

    public void downOne()
    {
        StartCoroutine(DownOneCoroutine());
    }

    IEnumerator DownOneCoroutine()
    {
        GameObject[] bricks = GameObject.FindGameObjectsWithTag("BRICKS");
        List<Coroutine> movementCoroutines = new List<Coroutine>();
        foreach (GameObject brick in bricks)
        {
            if (brick != null)
            {
                Vector3 newPosition = brick.transform.position;
                newPosition.z -= 1;
                movementCoroutines.Add(StartCoroutine(MoveDownSequence(brick, newPosition)));
                if (newPosition.z < 0)
                {
                    if (brick.name == "AddBall(Clone)" || brick.name == "Spread(Clone)" || brick.name == "+(Clone)" || brick.name == "-(Clone)")
                    {
                        Destroy(brick);
                    }
                    else
                    {
                        isLost = true;
                    }
                }
                var resource = brick.GetComponent<資源>();
                if (resource != null && resource.最後要刪除)
                {
                    Destroy(brick);
                }
            }
        }
        foreach (Coroutine coroutine in movementCoroutines)
        {
            yield return coroutine;
        }
        if (currentLevel._CurrentLevel == 0 && currentRound < 31)
        {
            var brickGenerator = GameObject.Find("/00GameMaster")?.GetComponent<產生磚塊>();
            if (brickGenerator != null)
            {
                brickGenerator.genBricks();
            }
        }
    }

    IEnumerator MoveDownSequence(GameObject obj, Vector3 targetPos)
    {
        if (obj == null) yield break;
        float duration = 0.5f;
        float elapsedTime = 0f;
        Vector3 startPosition = obj.transform.position;
        while (elapsedTime < duration && obj != null)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            obj.transform.position = Vector3.Lerp(startPosition, targetPos, t);
            yield return null;
        }
        if (obj != null)
        {
            obj.transform.position = targetPos;
        }
    }

    void UpdateUI()
    {
        UpdateBallCountText();
        if ((currentLevel._CurrentLevel == 0 || isSpecialLevel) && roundText != null)
        {
            roundText.text = "Round: " + currentRound.ToString();
        }
    }

    void UpdateBallCountText()
    {
        if (ballCountText != null)
        {
            if (isSpecialLevel)
            {
                ballCountText.text = "KeepShooting";
            }
            else
            {
                ballCountText.text = "Ball: " + currentLevel.balls;
            }
        }
    }

    void CheckWinCondition()
    {
        // 如果是特殊關卡且回合數未達到100，直接返回
        if (isSpecialLevel && currentRound < keepShootingLimit)
        {
            return;
        }

        // 計算實際的磚塊數量（排除特殊磚塊）
        GameObject[] bricks = GameObject.FindGameObjectsWithTag("BRICKS");
        totalBricks = bricks.Length;

        foreach (GameObject brick in bricks)
        {
            if (brick.name == "AddBall(Clone)" ||
                brick.name == "Spread(Clone)" ||
                brick.name == "-(Clone)" ||
                brick.name == "+(Clone)")
            {
                totalBricks--;
            }
        }

        // 判定勝利條件
        if (totalBricks == 0)
        {
            // 一般關卡直接獲勝
            // 特殊關卡需要回合數 >= 100 且清除所有磚塊
            if (!isSpecialLevel || (isSpecialLevel && currentRound >= keepShootingLimit))
            {
                isWon = true;
                if (isSpecialLevel)
                {
                    // 特殊關卡獲勝時可以在這裡添加特殊效果或獎勵
                }
            }
        }
    }

    void HandleGameEndStates()
    {
        if (isWon)
        {
            HandleWinState();
        }
        else if (isLost)
        {
            HandleLoseState();
        }
    }

    void HandleWinState()
    {
        if (shooter != null) shooter.SetActive(false);
        if (gameStatusText != null) gameStatusText.text = "WIN";
        if (nextButton != null) nextButton.gameObject.SetActive(true);
        if (backButton != null) backButton.gameObject.SetActive(true);
        isPlaying = false;
        DestroyAllBalls();
    }

    void HandleLoseState()
    {
        if (gameStatusText != null) gameStatusText.text = "LOSE";
        if (backButton != null) backButton.gameObject.SetActive(true);
        isPlaying = false;
    }

    void DestroyAllBalls()
    {
        GameObject[] balls = GameObject.FindGameObjectsWithTag("BALL");
        foreach (GameObject ball in balls)
        {
            Destroy(ball);
        }
    }

    void HandleEscapeKey()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            SceneManager.LoadScene("01_選擇關卡");
        }
    }

    private void HandleBricksGenerated()
    {
        StartCoroutine(setIsPlaying());
    }

    private IEnumerator setIsPlaying()
    {
        yield return new WaitForSeconds(1f);
        if (shooter != null) shooter.SetActive(true);
        isPlaying = true;
        levelHasLoadCompleted = true;
    }

    private void OnDestroy()
    {
        processCSV.OnBricksGenerated -= HandleBricksGenerated;
    }
}