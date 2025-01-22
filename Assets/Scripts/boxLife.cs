using TMPro;
using UnityEngine;

public class boxLife : MonoBehaviour
{
    public int life = 10;
    public TextMeshPro tx;
    float h, s, v;
    Renderer _render;
    // Start is called before the first frame update
    void Start()
    {
        _render = GetComponent<Renderer>();
        s = 1f;
        v = 1f;
        changeColor();
        tx.text = life.ToString();
        setFontSize(); 
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.tag == "BALL")
        {
            life--;
            tx.text = life.ToString();
            changeColor();           
            if(life <= 0) Destroy(this.gameObject);
            setFontSize();
        }
    }
    void setFontSize()
    {
        if(this.name == "��(Clone)" || this.name == "��(Clone)" || this.name == "��(Clone)" || this.name == "��(Clone)")
        {
            if (life > 99)
            {
                tx.fontSize = 3.5f;
            }
            else
            {
                tx.fontSize = 5f;
            }
        }
        
    }
    void changeColor()
    {
        float lifeFactor = 0;
        if (currentLevel._CurrentLevel >= 0)
        {
            // �p��ͩR�Ȫ���ҡ]�ͩR�Ȱ��ɱ��� 0�A�C�ɱ��� 1�^
            lifeFactor = 1f - ((float)life / (currentLevel.balls + 100));
        }
        else
        {
            // �p��ͩR�Ȫ���ҡ]�ͩR�Ȱ��ɱ��� 0�A�C�ɱ��� 1�^
            lifeFactor = 1f - ((float)life / (GameObject.Find("/00GameMaster/").GetComponent<GameController>().currentRound + 100));
        }
        

        // ����(0��)����(120��)���M�g
        h = (lifeFactor * 120f) / 360f;
        s = 1f;
        v = 1f;

        Color _color = Color.HSVToRGB(h, s, v);
        _render.material.color = _color;
        changeTextColor();
    }

    void changeTextColor()
    {
        if (life <= 3)
        {
            tx.color = Color.HSVToRGB(0.1f, 0.1f, 0.1f);
            return;
        }

        // �p��I���⪺�P���G��
        Color backgroundColor = _render.material.color;
        float brightness = backgroundColor.r * 0.299f + backgroundColor.g * 0.587f + backgroundColor.b * 0.114f;

        // �ھڭI���G�׿�ܦX�A����r�C��
        if (brightness > 0.6f)
        {
            // �I�����G�]�p����I���^�ɨϥβ`���r
            tx.color = new Color(0.1f, 0.1f, 0.1f, 1f);
        }
        else
        {
            // �I�����t�ɨϥΫG���r
            tx.color = new Color(0.95f, 0.95f, 0.95f, 1f);
        }
    }
}
