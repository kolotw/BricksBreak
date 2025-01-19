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
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.tag == "BALL")
        {
            life--;
            tx.text = life.ToString();
            changeColor();
            if(life <= 0) Destroy(this.gameObject); 
        }
    }

    void changeColor()
    {
        // �p��ͩR�Ȫ���ҡ]�V�C���ͩR�ȷ|�o��V���� 1 ���ȡ^
        float lifeFactor = 1f - ((float)life / (currentLevel.balls + 100));

        // �N lifeFactor �M�g�� 0-150 ���d��
        // lifeFactor ���� 1 �ɡ]�ͩR�C�^�Ah �|���� 150
        // lifeFactor ���� 0 �ɡ]�ͩR���^�Ah �|���� 0
        h = (lifeFactor * 150f) / 360f;

        // �O���̤j���M�שM���ץH��o�A�A���C��
        s = 1f;
        v = 1f;

        Color _color = Color.HSVToRGB(h, s, v);
        _render.material.color = _color;
        changeTextColor();
    }
    void changeTextColor()
    {
        // ���o�e�@���C�⪺ HSV ��
        Color.RGBToHSV(_render.material.color, out float prevH, out float prevS, out float prevV);

        // �p�⤬�ɦ⪺��ۡ]�[ 0.5 �۷��[ 180 �ס^
        float complementaryH = (prevH + 0.5f) % 1.0f;

        // �ϥΤ��ɦ�ۡA�O���쥻�����M�סA���ϥΩT�w������ 0.3
        Color _color = Color.HSVToRGB(complementaryH, prevS, 1f);

        tx.color = _color;
    }
}
