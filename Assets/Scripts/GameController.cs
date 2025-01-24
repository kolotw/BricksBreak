using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

/*
 �N�OGame Master
 
 */
public class GameController : MonoBehaviour
{
    #region �ܼƩw�q

    [Header("�o�g���]�m")]
    public GameObject �y�w�s��;
    public GameObject �o�g�I;
    public LineRenderer �u��V��;
    public float �o�g�t�� = 30f;
    public float �o�g���j = 0.05f;
    public float �y��j�p = 0.25f;
    public float �y��s�b�ɶ� = 5f;

    [Header("�C�����A")]
    public bool ���b�o�g = false;
    private bool �w�U�� = false;
    public bool �w��� = false;
    public bool �w���� = false;
    public bool �C���i�椤 = false;
    public bool ���d���J���� = false;
    public bool �S�����d = false;
    private bool ���b�˷� = false;
    private bool �S�����d�즸�ͦ� = true;
    private bool �^�X���� = false;

    [Header("UI����")]
    public Text �C�����A��r;
    public Button �U�@�����s;
    public Button ��^���s;
    public TextMeshPro �y�ƶq��r;
    public TextMeshPro �^�X��r;

    [Header("�C���]�m")]
    public int ��e���d = 1;
    public int ��e�^�X = 0;
    public GameObject �o�g��;

    // �p���ܼ�
    private Ray �۾��g�u;
    private RaycastHit �g�u�I��;
    private Vector3 �o�g��V;
    private Quaternion ���ਤ��;
    private Vector3 �_�l��m;
    private float �S�����d�p�ɾ� = 0f;
    private int �Ѿl�j���� = 0;
    private const float �j���ͦ����j = 2.5f;
    private int ����o�g�W�� = 50;

    [SerializeField]
    private LayerMask �h�žB�n;

    #endregion

    #region Unity�ͩR�g��

    void Start()
    {
        ��l�ƹC��();
        �_�l��m = �o�g��.transform.position;
    }

    void Update()
    {
        �B�z��^��();
        if (!�i�H�C��()) return;

        �B�z�C���޿�();
        �B�z�S�����d�޿�();

        if ((�C���i�椤 && !�w���) || �S�����d)
        {
            �B�z�o�g�޿�();
        }
    }

    private void OnDestroy()
    {
        processCSV.OnBricksGenerated -= �B�z�j���ͦ�����;
    }

    #endregion

    #region ��l��

    void ��l�ƹC��()
    {
        // ��l�Ƶo�g��
        if (�u��V�� != null) �u��V��.enabled = false;

        // ��l�ƹC�����A
        �o�g��.SetActive(false);
        �C���i�椤 = false;
        �w��� = false;
        �w���� = false;
        �S�����d = false;
        �S�����d�p�ɾ� = 0f;

        // ��l��UI
        ��l��UI();

        // �q�\�ƥ�
        processCSV.OnBricksGenerated += �B�z�j���ͦ�����;

        // ���J���d
        ���J�C��();
    }

    void ��l��UI()
    {
        if (�C�����A��r != null) �C�����A��r.text = "";
        if (�U�@�����s != null) �U�@�����s.gameObject.SetActive(false);
        if (��^���s != null) ��^���s.gameObject.SetActive(false);
    }

    #endregion

    #region ���d���J

    void ���J�C��()
    {
        ��e���d = currentLevel._CurrentLevel;

        if (��e���d > 9)
        {
            ��e���d = 1;
            currentLevel._CurrentLevel = 1;
        }

        if (currentLevel._CurrentLevel > 0)
        {
            ���J�@�����d();
        }
        else if (currentLevel._CurrentLevel == 0)
        {
            ���J�H�����d();
        }
        else if (currentLevel._CurrentLevel == -1)
        {
            ���J�S�����d();
        }

        ��s�y�ƶq��r();
    }

    void ���J�@�����d()
    {
        GameObject.Find("00GameMaster").GetComponent<processCSV>().getLevel(��e���d);
        if (�^�X��r != null) �^�X��r.text = "LEVEL: " + ��e���d.ToString();
    }

    void ���J�H�����d()
    {
        if (currentLevel._CurrentLevel < 0) return;

        GetComponent<���Ϳj��>().genBricks();
        if (�o�g�� != null) �o�g��.SetActive(true);
        �C���i�椤 = true;
        if (�^�X��r != null) �^�X��r.text = "RANDOM";
        �U���@��();
    }

    void ���J�S�����d()
    {
        �S�����d = true;
        //if (�^�X��r != null) �^�X��r.text = "KEEPING";
        GameObject.Find("/��r-�y��").GetComponent<TextMeshPro>().fontSize = 9;
        if (�o�g�� != null) �o�g��.SetActive(true);
        �C���i�椤 = true;
        ��s�y�ƶq��r();
    }

    #endregion

    #region �g���t��

    void �B�z�o�g�޿�()
    {
        if (�w���� || �w���) return;
        �۾��g�u = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButton(0))
        {
            if (�S�����d || (!���b�o�g && !�w�U��))
            {
                ���b�˷� = true;
            }

            //���b�˷� = true;
            if (�u��V�� != null)
            {
                �u��V��.enabled = true;
                �B�z�˷�();
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            
            if (�u��V�� != null) �u��V��.enabled = false;
            if (!���b�o�g)
            {
                ���b�˷� = false;
                �}�l�o�g();
            }            
        }

        if (!�S�����d)
        {
            if(!���b�˷�)
                �B�z�y��^��();
        }
    }

    void �B�z�˷�()
    {
        if (Physics.Raycast(�۾��g�u, out �g�u�I��, 150f, �h�žB�n))
        {
            if (!���b�o�g || �S�����d)
            {
                �p��g���y��();
            }
        }
    }

    void �p��g���y��()
    {
        Vector3 �I���I = �g�u�I��.point;
        Vector3 �o�g����m = �o�g�I.transform.position;
        �I���I.y = 0f;
        �o�g����m.y = 0f;

        Ray �g���g�u = new Ray(�o�g�I.transform.position, (�I���I - �o�g����m).normalized);
        RaycastHit ����I��;

        if (Physics.SphereCast(�g���g�u, �y��j�p, out ����I��, 150f, �h�žB�n))
        {
            Vector3 �Ĥ@�I���I = ����I��.point;
            �Ĥ@�I���I.y = 0f;

            Vector3 �Ϯg��V = Vector3.Reflect((�I���I - �o�g����m).normalized, ����I��.normal).normalized;
            �Ϯg��V.y = 0f;

            Ray �Ϯg�g�u = new Ray(�Ĥ@�I���I + �Ϯg��V * �y��j�p, �Ϯg��V);
            RaycastHit �ĤG���I��;
            Vector3 �ĤG�I���I = �Ĥ@�I���I + �Ϯg��V * 20f;

            if (Physics.Raycast(�Ϯg�g�u, out �ĤG���I��, 20f, �h�žB�n))
            {
                �ĤG�I���I = �ĤG���I��.point;
                �ĤG�I���I.y = 0f;
            }

            ��s�y��u(�Ĥ@�I���I, �ĤG�I���I);
            ��s�o�g����V(�I���I - �o�g����m);
        }
    }

    void ��s�y��u(Vector3 �Ĥ@�I���I, Vector3 �ĤG�I���I)
    {
        �_�l��m = �o�g��.transform.position;
        �u��V��.positionCount = 3;
        �u��V��.SetPosition(0, �_�l��m);
        �u��V��.SetPosition(1, �Ĥ@�I���I);
        �u��V��.SetPosition(2, �ĤG�I���I);
    }

    void ��s�o�g����V(Vector3 ��V)
    {
        �o�g��V = ��V.normalized;
        ���ਤ�� = Quaternion.LookRotation(�o�g��V, Vector3.up);
        �o�g��.transform.rotation = ���ਤ��;
        �o�g��.transform.eulerAngles = new Vector3(0f, �o�g��.transform.eulerAngles.y, 0f);
    }

    void �}�l�o�g()
    {
        if (�u��V�� != null)
        {
            Vector3 �u��V = (�u��V��.GetPosition(1) - �u��V��.GetPosition(0)).normalized;
            �u��V.y = 0f;
            �u��V��.enabled = false;

            ���ਤ�� = Quaternion.LookRotation(�u��V, Vector3.up);
            �o�g��.transform.rotation = ���ਤ��;
            �o�g��.transform.eulerAngles = new Vector3(0f, �o�g��.transform.eulerAngles.y, 0f);
        }

        ���b�o�g = true;
        �w�U�� = true;
        StartCoroutine(�o�g�ǦC());
        ��e�^�X++;
    }

    IEnumerator �o�g�ǦC()
    {
        if (�S�����d)
        {
            while (!�w��� && �C���i�椤)
            {
                if (�w���) break;
                �إ߲y��();
                yield return new WaitForSeconds(�o�g���j);
            }
        }
        else
        {
            for (int i = 0; i < currentLevel.balls && !�w���; i++)
            {
                �إ߲y��();
                yield return new WaitForSeconds(�o�g���j);
            }
        }

        ���b�o�g = false;
    }

    void �إ߲y��()
    {
        if (�y�w�s�� == null || �o�g�I == null) return;

        GameObject �y = Instantiate(�y�w�s��, �o�g�I.transform.position, Quaternion.identity);
        �y.transform.localScale = new Vector3(�y��j�p, �y��j�p, �y��j�p);
        �y.transform.forward = �o�g��V;
        �y.transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);

        if (�y.TryGetComponent<Rigidbody>(out Rigidbody ����))
        {
            ����.velocity = �o�g��V * �o�g�t��;
        }

        Destroy(�y, �y��s�b�ɶ�);
    }

    #endregion

    #region �C���޿�

    void �B�z�C���޿�()
    {
        if (�C���i�椤)
        {
            ��sUI();
            �ˬd�ӧQ����();
        }
        �B�z�C���������A();
    }

    void �B�z�S�����d�޿�()
    {
        if (!�S�����d) return;

        if (�S�����d�즸�ͦ�)
        {
            GameObject.Find("00GameMaster").GetComponent<���Ϳj��>().genBricks();
            �U���@��();
            �S�����d�즸�ͦ� = false;
        }
        if (!�C���i�椤) return;
        �S�����d�p�ɾ� += Time.deltaTime;
        if (�S�����d�p�ɾ� >= �j���ͦ����j)
        {
            if (��e�^�X < ����o�g�W��)
            {
                GameObject.Find("00GameMaster").GetComponent<���Ϳj��>().genBricks();
                ��e�^�X++;
            }

            �U���@��();
            ��s�^�X��r();
            �S�����d�p�ɾ� = 0f;
        }

        if (��e�^�X >= ����o�g�W��)
        {
            �ˬd�ӧQ����();
        }
    }

    void �B�z�y��^��()
    {
        if (GameObject.FindGameObjectsWithTag("BALL").Length == 0)
        {
            ���b�o�g = false;
            var �U�� = GameObject.Find("/��/�U").GetComponent<�^���y>();

            if (�w�U��)
            {
                �U��.���ʵo�g��();
                �U���@��();
                �w�U�� = false;
            }
            else
            {
                �U��.���ʵo�g��(); // �T�O�C�����|���ʵo�g��
            }
        }
    }

    public void �U���@��()
    {
        StartCoroutine(�U���@���{());
    }

    IEnumerator �U���@���{()
    {
        GameObject[] �j���� = GameObject.FindGameObjectsWithTag("BRICKS");
        List<Coroutine> ���ʨ�{�C�� = new List<Coroutine>();

        foreach (GameObject �j�� in �j����)
        {
            if (�j�� != null)
                {
                    �B�z�j������(�j��, ���ʨ�{�C��);
                }
        }

        foreach (Coroutine ��{ in ���ʨ�{�C��)
        {
            yield return ��{;
        }

        if (currentLevel._CurrentLevel == 0 && ��e�^�X < 31)
        {
            var �j���ͦ��� = GameObject.Find("/00GameMaster")?.GetComponent<���Ϳj��>();
            �j���ͦ���?.genBricks();
        }
    }

    void �B�z�j������(GameObject �j��, List<Coroutine> ���ʨ�{�C��)
    {
        Vector3 �s��m = �j��.transform.position;
        �s��m.z -= 1;
        ���ʨ�{�C��.Add(StartCoroutine(���ʤU���ǦC(�j��, �s��m)));

        if (�s��m.z < 0)
        {
            �B�z�W�X��ɿj��(�j��);
        }

        var �귽�ե� = �j��.GetComponent<�귽>();
        if (�귽�ե� != null && �귽�ե�.�̫�n�R��)
        {
            Destroy(�j��);
        }
    }

    void �B�z�W�X��ɿj��(GameObject �j��)
    {
        if (�j��.name == "AddBall(Clone)" ||
            �j��.name == "Spread(Clone)" ||
            �j��.name == "+(Clone)" ||
            �j��.name == "-(Clone)")
        {
            Destroy(�j��);
        }
        else
        {
            �w���� = true;
        }
    }

    IEnumerator ���ʤU���ǦC(GameObject ����, Vector3 �ؼЦ�m)
    {
        if (���� == null) yield break;

        float ����ɶ� = 0.5f;
        float �g�L�ɶ� = 0f;
        Vector3 �_�l��m = ����.transform.position;

        while (�g�L�ɶ� < ����ɶ� && ���� != null)
        {
            �g�L�ɶ� += Time.deltaTime;
            float �i�� = �g�L�ɶ� / ����ɶ�;
            ����.transform.position = Vector3.Lerp(�_�l��m, �ؼЦ�m, �i��);
            yield return null;
        }

        if (���� != null)
        {
            ����.transform.position = �ؼЦ�m;
        }
    }

    #endregion

    #region UI��s

    void ��sUI()
    {
        ��s�y�ƶq��r();
        if ((currentLevel._CurrentLevel == 0 || �S�����d) && �^�X��r != null)
        {
            ��s�^�X��r();
        }
    }

    void ��s�y�ƶq��r()
    {
        if (�y�ƶq��r != null)
        {
            �y�ƶq��r.text = �S�����d ? "KEEP SHOOTING" : $"BALL: {currentLevel.balls}";
        }
    }

    void ��s�^�X��r()
    {
        if (�^�X��r != null)
        {
            �^�X��r.text = $"ROUND: {��e�^�X}";
        }
    }

    #endregion

    #region �C�����A�ˬd

    bool �i�H�C��()
    {
        if (currentLevel._CurrentLevel > 0 && !���d���J����)
        {
            �C���i�椤 = false;
            return false;
        }
        return true;
    }

    void �ˬd�ӧQ����()
    {
        if (�S�����d && ��e�^�X < ����o�g�W��)
        {
            return;
        }

        �p��Ѿl�j��();

        if (�Ѿl�j���� == 0)
        {
            if (!�S�����d || (�S�����d && ��e�^�X >= ����o�g�W��))
            {
                �w��� = true;
            }
        }
    }

    void �p��Ѿl�j��()
    {
        GameObject[] �j���� = GameObject.FindGameObjectsWithTag("BRICKS");
        �Ѿl�j���� = �j����.Length;

        foreach (GameObject �j�� in �j����)
        {
            if (�j��.name == "AddBall(Clone)" ||
                �j��.name == "Spread(Clone)" ||
                �j��.name == "-(Clone)" ||
                �j��.name == "+(Clone)")
            {
                �Ѿl�j����--;
            }
        }
    }

    void �B�z�C���������A()
    {
        if (�w���)
        {
            �B�z�ӧQ���A();
        }
        else if (�w����)
        {
            �B�z���Ѫ��A();
        }
    }

    void �B�z�ӧQ���A()
    {
        if (�o�g�� != null) �o�g��.SetActive(false);
        if (�C�����A��r != null) �C�����A��r.text = "WIN";
        if (�U�@�����s != null) �U�@�����s.gameObject.SetActive(true);
        if (��^���s != null) ��^���s.gameObject.SetActive(true);

        �C���i�椤 = false;
        �P���Ҧ��y��();
    }

    void �B�z���Ѫ��A()
    {
        if (�C�����A��r != null) �C�����A��r.text = "LOSE";
        if (��^���s != null) ��^���s.gameObject.SetActive(true);
        �C���i�椤 = false;
    }

    void �P���Ҧ��y��()
    {
        GameObject[] �y�� = GameObject.FindGameObjectsWithTag("BALL");
        foreach (GameObject �y in �y��)
        {
            Destroy(�y);
        }
    }

    #endregion

    #region ��J�B�z

    void �B�z��^��()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            SceneManager.LoadScene("01_������d");
        }
    }

    #endregion

    #region �ƥ�B�z

    private void �B�z�j���ͦ�����()
    {
        StartCoroutine(�]�m�C�����A());
    }

    private IEnumerator �]�m�C�����A()
    {
        yield return new WaitForSeconds(1f);
        if (�o�g�� != null) �o�g��.SetActive(true);
        �C���i�椤 = true;
        ���d���J���� = true;
    }

    #endregion
}