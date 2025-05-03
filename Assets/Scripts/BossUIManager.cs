using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BossUIManager : MonoBehaviour
{
    public static BossUIManager Instance;

    public Slider bossHPSlider;
    private EnemyHealth currentBoss;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
        //bossHPSlider = GameObject.Find("BossEnemyHP")?.GetComponent<Slider>();
        //bossHPSlider.gameObject.SetActive(false);
    }
   
    public void ShowBossHPBar(EnemyHealth boss)
    {   
        currentBoss = boss;
        bossHPSlider.maxValue = boss.maxHP;
        bossHPSlider.value = boss.maxHP;
        bossHPSlider.gameObject.SetActive(true);
    }
    public void SetCurrentHealth(int currentHP)
    {
        bossHPSlider.value = currentHP;
    }
    public void SetSlider(Slider slider)
    {
        bossHPSlider = slider;
        bossHPSlider.gameObject.SetActive(false);
    }
    private void OnSceneLoaded(Scene scene,LoadSceneMode mode)
    {   
        bossHPSlider = GameObject.Find("BossEnemyHP")?.GetComponent<Slider>();
        if (bossHPSlider != null)
            bossHPSlider.gameObject.SetActive(false);
    }
    void Update()
    {
        if (currentBoss != null)
        {
            bossHPSlider.value = currentBoss.IsDead ? 0 : currentBoss.GetCurrentHP();

            if (currentBoss.IsDead)
            {
                bossHPSlider.gameObject.SetActive(false);
                currentBoss = null;
            }
        }
    }
}
