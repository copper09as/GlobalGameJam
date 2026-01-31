using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ButtonEffect : MonoBehaviour
{
    public Button button;
    /// <summary>
    /// 粒子特效
    /// </summary>
    public ParticleSystem particleEffect;
    /// <summary>
    /// 高亮颜色
    /// </summary>
    public Color highlightColor = Color.yellow;
    /// <summary>
    /// 原始颜色
    /// </summary>
    private Color originalColor;
    /// <summary>
    /// 缩放比例
    /// </summary>
    public float scaleFactor = 1.2f;
    /// <summary>
    /// 缩放持续时间
    /// </summary>
    public float duration = 0.1f;
    private Image buttonImage;
    // Start is called before the first frame update
    void Start()
    {
        // 获取按钮的原始颜色
       originalColor = button.GetComponent<Image>().color;
       //获取按钮的Image组件
       buttonImage = button.GetComponent<Image>();
       //监听按钮点击事件
       button.onClick.AddListener(OnButtonClick);    
    }
    void OnButtonClick()
    {
        //播放粒子效果
    particleEffect.Play();
    //启动协程播放按钮特效
    StartCoroutine(PlayButtonEffect());
    }
    IEnumerator PlayButtonEffect()
    {
        // 按钮缩放动画
    float elapsedTime = 0f;
    while (elapsedTime < duration)
    {
       button.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * scaleFactor, (elapsedTime / duration));
       elapsedTime += Time.deltaTime;
       yield return null;  
    }
    button.transform.localScale = Vector3.one * scaleFactor;
    // 按钮恢复原始大小动画
    elapsedTime = 0f;
    while (elapsedTime < duration)
    {
       button.transform.localScale = Vector3.Lerp(Vector3.one * scaleFactor, Vector3.one, (elapsedTime / duration));
       elapsedTime += Time.deltaTime;
       yield return null;  
    }
    button.transform.localScale = Vector3.one;
    // 按钮颜色高亮动画
    buttonImage.color = highlightColor;
    //等待一段时间后恢复原始颜色
    yield return new WaitForSeconds(0.2f);
    buttonImage.color = originalColor;
    }
}
    
     
       

  