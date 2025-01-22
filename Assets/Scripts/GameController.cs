using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameController : MonoBehaviour
{
    // �o�g�������ܼ�
    private Ray cameraRay;
    private RaycastHit hit;
    [SerializeField] private LayerMask layerMask;
    private Vector3 shootDirection;
    private Quaternion rotation;
    [Header("�o�g���]�m")]
    public GameObject ballPrefab;
    public GameObject shootPoint;
    public LineRenderer lineRenderer;
    public float shootSpeed = 30f;
    public float shootInterval = 0.05f;
    public float ballSize = 0.25f;
    public float timeOut = 5f;

    [Header("�C�����A")]
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

    [Header("UI����")]
    public Text gameStatusText;
    public Button nextButton;
    public Button backButton;
    public TextMeshPro ballCountText;
    public TextMeshPro roundText;

    [Header("�C���]�m")]
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
        // ��l�Ƶo�g���]�m
        if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
        }
        // ��l�ƹC�����A
        shooter.SetActive(false);
        isPlaying = false;
        isWon = false;
        isLost = false;
        isSpecialLevel = false;
        specialLevelTimer = 0f;

        // ��l��UI
        if (gameStatusText != null) gameStatusText.text = "";
        if (nextButton != null) nextButton.gameObject.SetActive(false);
        if (backButton != null) backButton.gameObject.SetActive(false);

        // �q�\�ƥ�
        processCSV.OnBricksGenerated += HandleBricksGenerated;

        // �}�l���J���d
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
            GetComponent<���Ϳj��>().genBricks();
            if (shooter != null) shooter.SetActive(true);
            isPlaying = true;
            if (roundText != null) roundText.text = "LEVEL: Random";
            downOne();
        }
        else if (currentLevel._CurrentLevel == -1)  // �S�����d�P�_
        {
            isSpecialLevel = true;
            if (roundText != null) roundText.text = "LEVEL: Special";
            if (shooter != null) shooter.SetActive(true);
            isPlaying = true;
            GameObject.Find("00GameMaster").GetComponent<���Ϳj��>().genBricks();
            //currentLevel.balls = 999; // �]�m���j���ƭȪ�ܵL��
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

        // �S�����d����s�޿�
        if (isSpecialLevel && isPlaying)
        {
            specialLevelTimer += Time.deltaTime;
            if (specialLevelTimer >= BRICK_SPAWN_INTERVAL)
            {
                if (currentRound < keepShootingLimit) // �u�b100�^�X�e�ͦ��s�j��
                {
                    GameObject.Find("00GameMaster").GetComponent<���Ϳj��>().genBricks();
                    currentRound++;
                }

                // �L�׬O�_�F��100�^�X������ downOne
                downOne();

                // ��sUI
                if (roundText != null)
                {
                    roundText.text = "Round: " + currentRound.ToString();
                }

                specialLevelTimer = 0f;
            }

            // �b�^�X�ƹF��100��A�����ˬd��ӱ���
            if (currentRound >= keepShootingLimit)
            {
                CheckWinCondition(); // �����ˬd�O�_���
            }
        }

        // �u���b�C���i�椤�B���O��Ӫ��A�ɤ~�B�z�g���޿�
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
        // ����ṳ���g�u
        cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        // �B�z�˷��޿�
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

        // �p�G���O�S�����d�A�h�ݭn�ˬd�y�O�_�b���W
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
            GameObject.Find("/��/�U").GetComponent<�^���y>().���ʵo�g��();

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
                    // �Ĥ@���I���I
                    Vector3 collisionPoint = wallHit.point;
                    collisionPoint.y = 0f;

                    // �p��Ϯg��V
                    Vector3 reflectDirection = Vector3.Reflect((hitPoint - shooterPosition).normalized, wallHit.normal).normalized;
                    reflectDirection.y = 0f;

                    // �p��ĤG���I��
                    Ray reflectedRay = new Ray(collisionPoint + reflectDirection * ballSize, reflectDirection);
                    RaycastHit secondHit;
                    Vector3 secondHitPoint = collisionPoint + reflectDirection * 20f;

                    if (Physics.Raycast(reflectedRay, out secondHit, 20f, layerMask))
                    {
                        secondHitPoint = secondHit.point;
                        secondHitPoint.y = 0f;
                    }

                    // �]�m lineRenderer
                    startPos = shooter.transform.position;
                    lineRenderer.positionCount = 3;
                    lineRenderer.SetPosition(0, startPos);
                    lineRenderer.SetPosition(1, collisionPoint);
                    lineRenderer.SetPosition(2, secondHitPoint);

                    // �ϥά��u����V (��l�g����V)
                    Vector3 redLineDirection = (hitPoint - shooterPosition).normalized;
                    shootDirection = redLineDirection;

                    rotation = Quaternion.LookRotation(shootDirection, Vector3.up);
                    shooter.transform.rotation = rotation;
                    shooter.transform.eulerAngles = new Vector3(0f, shooter.transform.eulerAngles.y, 0f);

                    // Debug�g�u
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

        // �e�X�g����V�]����^�M shooter ���e��V�]�Ŧ�^
        Debug.DrawRay(shooter.transform.position, shootDirection * 10f, Color.red);
        Debug.DrawRay(shooter.transform.position, shooter.transform.forward * 10f, Color.blue);
    }

    void StartShooting()
    {
        if (lineRenderer != null)
        {
            // �b���� lineRenderer ���e�����o������V
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
        // �Ыزy��
        GameObject ball = Instantiate(ballPrefab, shootPoint.transform.position, Quaternion.identity);
        ball.transform.localScale = new Vector3(ballSize, ballSize, ballSize);
        // �]�m�y�骺��V�M�t��
        ball.transform.forward = shootDirection;
        ball.transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = shootDirection * shootSpeed;
        }
        // �]�m�P���ɶ�
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
                var resource = brick.GetComponent<�귽>();
                if (resource != null && resource.�̫�n�R��)
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
            var brickGenerator = GameObject.Find("/00GameMaster")?.GetComponent<���Ϳj��>();
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
        // �p�G�O�S�����d�B�^�X�ƥ��F��100�A������^
        if (isSpecialLevel && currentRound < keepShootingLimit)
        {
            return;
        }

        // �p���ڪ��j���ƶq�]�ư��S��j���^
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

        // �P�w�ӧQ����
        if (totalBricks == 0)
        {
            // �@�����d�������
            // �S�����d�ݭn�^�X�� >= 100 �B�M���Ҧ��j��
            if (!isSpecialLevel || (isSpecialLevel && currentRound >= keepShootingLimit))
            {
                isWon = true;
                if (isSpecialLevel)
                {
                    // �S�����d��Ӯɥi�H�b�o�̲K�[�S��ĪG�μ��y
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
            SceneManager.LoadScene("01_������d");
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