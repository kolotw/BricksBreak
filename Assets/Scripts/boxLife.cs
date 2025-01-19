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
        // 計算生命值的比例（越低的生命值會得到越接近 1 的值）
        float lifeFactor = 1f - ((float)life / (currentLevel.balls + 100));

        // 將 lifeFactor 映射到 0-150 的範圍
        // lifeFactor 接近 1 時（生命低），h 會接近 150
        // lifeFactor 接近 0 時（生命高），h 會接近 0
        h = (lifeFactor * 150f) / 360f;

        // 保持最大飽和度和明度以獲得鮮艷的顏色
        s = 1f;
        v = 1f;

        Color _color = Color.HSVToRGB(h, s, v);
        _render.material.color = _color;
        changeTextColor();
    }
    void changeTextColor()
    {
        // 取得前一個顏色的 HSV 值
        Color.RGBToHSV(_render.material.color, out float prevH, out float prevS, out float prevV);

        // 計算互補色的色相（加 0.5 相當於加 180 度）
        float complementaryH = (prevH + 0.5f) % 1.0f;

        // 使用互補色相，保持原本的飽和度，但使用固定的明度 0.3
        Color _color = Color.HSVToRGB(complementaryH, prevS, 1f);

        tx.color = _color;
    }
}
