using UnityEngine;
using UnityEngine.UI;

public class StatsAutoHealthBar : MonoBehaviour
{
    private StatsController stats;
    private Slider slider;
    public float yOffset = 0;

    void Start()
    {
        stats = GetComponentInParent<StatsController>();
        slider = GetComponent<Slider>();
        stats.hpChanged.AddListener(SetPercentage);
        SetPercentage();
    }

    private void Update()
    {
        slider.transform.position = Camera.main.WorldToScreenPoint(new Vector3(stats.transform.position.x, stats.transform.position.y + yOffset, stats.transform.position.z));
    }

    private void SetPercentage()
    {
        slider.value = stats.GetHP() / stats.maxHP;
    }
}
