using deVoid.Utils;
using TMPro;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public TextMeshProUGUI UI;

    public GameObject mob;

    private Enemy enemy;
    // Start is called before the first frame update
    void Start()
    { 
        UI = GetComponent<TextMeshProUGUI>();
        enemy = mob.GetComponent<Enemy>();
        Signals.Get<Enemy.EnemyDamage>().AddListener(updateHealthUI);
    }

    public void updateHealthUI() {
        UI.text = enemy.CurrentHealth.ToString() + "/" + enemy.MaxHealth.ToString();
    }


    // Shoutout to the observer pattern
    void Update()
    {
        updateHealthUI();
    }

}
