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
        if(this.name == "◥(Clone)" || this.name == "◤(Clone)" || this.name == "◣(Clone)" || this.name == "◢(Clone)")
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
            // 計算生命值的比例（生命值高時接近 0，低時接近 1）
            lifeFactor = 1f - ((float)life / (currentLevel.balls + 100));
        }
        else
        {
            // 計算生命值的比例（生命值高時接近 0，低時接近 1）
            lifeFactor = 1f - ((float)life / (GameObject.Find("/00GameMaster/").GetComponent<GameController>().currentRound + 100));
        }
        

        // 紅色(0度)到綠色(120度)的映射
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

        // 計算背景色的感知亮度
        Color backgroundColor = _render.material.color;
        float brightness = backgroundColor.r * 0.299f + backgroundColor.g * 0.587f + backgroundColor.b * 0.114f;

        // 根據背景亮度選擇合適的文字顏色
        if (brightness > 0.6f)
        {
            // 背景偏亮（如紅色背景）時使用深色文字
            tx.color = new Color(0.1f, 0.1f, 0.1f, 1f);
        }
        else
        {
            // 背景偏暗時使用亮色文字
            tx.color = new Color(0.95f, 0.95f, 0.95f, 1f);
        }
    }
}
