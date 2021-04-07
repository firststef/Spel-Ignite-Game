using UnityEngine;
using UnityEngine.UI;

public class StatsAutoHealthBar : MonoBehaviour
{
    private StatsController stats;
    private Slider slider;

    void Start()
    {
        stats = transform.parent.parent.GetComponent<StatsController>();
        slider = GetComponent<Slider>();
        stats.hpChanged.AddListener(SetPercentage);
        SetPercentage();
    }

    private void Update()
    {
        slider.transform.position = Camera.main.WorldToScreenPoint(stats.transform.position);
    }

    private void SetPercentage()
    {
        slider.value = stats.GetHP() / stats.maxHP;
    }
}
