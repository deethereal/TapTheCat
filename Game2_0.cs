using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;



public class Game2_0 : MonoBehaviour
{
    [Header("Текст отвечающий за отоброжение очков")]
    public Text scoreText;
    [Header("Магазин")]
    public List<Item> shopItems = new List<Item>();
    public Text[] shopItemsText;
    [Header("Кнопки Товаров")]
    public Button[] shopBttns;
    [Header("Панелька магазина")]
    public GameObject shopPan;

    private int score;
    public int scoreIncrease; //боунс
    public int cost;
    private Save sv = new Save();
    private int totalBonusePS;
    //public float time;
    // public bool abonus;
    public int obonustime;



    private void Awake()
    {
        PlayerPrefs.DeleteKey("SV"); //очистка сохр.
        if (PlayerPrefs.HasKey("SV"))
        {

            sv = JsonUtility.FromJson<Save>(PlayerPrefs.GetString("SV"));
            score = sv.score;
            for (int i = 0; i < shopItems.Count; i++)
            {
                shopItems[i].levelOfItem = sv.levelOfItem[i];
                shopItems[i].bonusCounter = sv.bonusCounter[i];
                if (shopItems[i].needCostMultiplier)
                {
                    shopItems[i].cost *= (int)Mathf.Pow(shopItems[i].costMultiplier, shopItems[i].levelOfItem);
                }
                if (shopItems[i].bonusIncrease != 0)
                {
                    scoreIncrease += (int)Mathf.Pow(shopItems[i].bonusIncrease, shopItems[i].levelOfItem);
                }
                totalBonusePS += shopItems[i].bonusPerSec * shopItems[i].bonusCounter;

            }
            DateTime dt = new DateTime(sv.date[0], sv.date[1], sv.date[2], sv.date[3], sv.date[4], sv.date[5]);
            TimeSpan ts = DateTime.Now - dt;
            if (ts.TotalSeconds >= 0)
            {

                int offlinebonus = ((int)ts.TotalSeconds - sv.obonustime) * totalBonusePS + sv.obonustime * totalBonusePS * 2; //вторая часть
                score += offlinebonus;
                print(((int)ts.TotalSeconds - sv.obonustime) * totalBonusePS + "  " + sv.obonustime);
                print("Пока вас не было, вы получили " + offlinebonus + " МурМонет");

            }
            else
            {
                print("Читерить плохо, ты лишаешься всех МурМонет!");
                PlayerPrefs.DeleteKey("SV"); //- очистка сохр.
            }
        }
    }
    private void Start()
    {

        updateCosts(); //обновление текста
        StartCoroutine(BonusPerSec()); //тик в секунду


    }
    private void Update() //очки
    {
        scoreText.text = score + "\n МурМонет";
        
        print(cost);
        if (score >= shopItems[0].cost)
        {
            shopBttns[0].interactable = true;
        }
        else
        {
            shopBttns[0].interactable = false;
        }
        if (score >= shopItems[1].cost)
        {
            shopBttns[1].interactable = true;
        }
        else
        {
            shopBttns[1].interactable = false;
        }
        if (score >= cost && shopItems[1].bonusCounter != 0) //не работает:)
        {
            shopBttns[2].interactable = true;
        }
        else
        {
            shopBttns[2].interactable = false;
        }

    }
    public void BuyBttn(int index) // index-кнопка
    {
        cost = shopItems[index].cost * shopItems[shopItems[index].itemIndex].bonusCounter; //цена бонуса
        if (shopItems[index].itsBonus && score >= cost)
        {
            if (cost > 0)
            {
                score -= cost;
                StartCoroutine(BonusTimer(shopItems[index].timeOfBonus, index));
                //StartCoroutine(activeTime(shopItems[index].timeOfBonus)); //!
            }
            else print("Нечего улучшать"); // заменить на вибрацию

        }
        else if (score >= shopItems[index].cost)
        {
            if (shopItems[index].itsItemPerSec) // авто-клик
            {
                shopItems[index].bonusCounter++;
                cost = shopItems[2].cost * shopItems[1].bonusCounter;
            }

            else scoreIncrease += shopItems[index].bonusIncrease; //увеличение клика 

            score -= shopItems[index].cost;
            if (shopItems[index].needCostMultiplier) shopItems[index].cost *= shopItems[index].costMultiplier; //увеличение стоимости
            shopItems[index].levelOfItem++;

        }

        else print("Не хватает МурМонет");
        updateCosts();
    }
    private void updateCosts()
    {
        for (int i = 0; i < shopItems.Count; i++)
        {
            if (shopItems[i].itsBonus)
            {
                int cost = shopItems[i].cost * shopItems[shopItems[i].itemIndex].bonusCounter; //цена бонуса
                shopItemsText[i].text = shopItems[i].name + "\n" + cost + "МурМонет";
            }
            else shopItemsText[i].text = shopItems[i].name + "\n" + shopItems[i].cost + "МурМонет"; //цена ячейки в магазине
        }
    }

    IEnumerator BonusPerSec()
    {
        while (true)
        {
            for (int i = 0; i < shopItems.Count; i++)
            {
                score += (shopItems[i].bonusCounter * shopItems[i].bonusPerSec); // инкрис тика в секунду
                yield return new WaitForSeconds(0.33F); //задержка

            }
        }

    }

    IEnumerator BonusTimer(float time, int index)
    {
        shopBttns[index].interactable = false;
        shopItems[shopItems[index].itemIndex].bonusPerSec *= 2;
        scoreIncrease *= 2;
        yield return new WaitForSeconds(time);
        shopBttns[index].interactable = true;
        shopItems[shopItems[index].itemIndex].bonusPerSec /= 2;
        scoreIncrease /= 2;


    }
    /*IEnumerator activeTime(float time) //!
    {
        obonustime = 0;
        while (obonustime != (int)time)
        {
            obonustime++;
            yield return new WaitForSeconds(0.33F);
        }
        
    }
    */
    public void showShopPan()
    {
        shopPan.SetActive(!shopPan.activeSelf);
    }
    public void OnClick()
    {
        score += scoreIncrease; //клик
    }
#if UNITY_ANDROID && !UNITY_EDITOR
    private void OnApplicationPause(bool pause) // для Андроида    
    {
        if (pause)
        {
            sv.score = score;
            sv.levelOfItem = new int[shopItems.Count]; //~кол-во кнопок в магазине
            sv.bonusCounter = new int[shopItems.Count];
            for (int i = 0; i < shopItems.Count; i++)
            {
                sv.levelOfItem[i] = shopItems[i].levelOfItem;
                sv.bonusCounter[i] = shopItems[i].bonusCounter;
            }
            sv.date[0] = DateTime.Now.Year; sv.date[1] = DateTime.Now.Month; sv.date[2] = DateTime.Now.Day;
            sv.date[3] = DateTime.Now.Hour; sv.date[4] = DateTime.Now.Minute; sv.date[5] = DateTime.Now.Second;
            PlayerPrefs.SetString("SV", JsonUtility.ToJson(sv));
        }
    }
#else   
    private void OnApplicationQuit() //
    {
        sv.score = score;
        sv.levelOfItem = new int[shopItems.Count]; //~кол-во кнопок в магазине
        sv.bonusCounter = new int[shopItems.Count];
        // sv.obonustime = (int)time - obonustime; //!


        for (int i = 0; i < shopItems.Count; i++)
        {
            sv.levelOfItem[i] = shopItems[i].levelOfItem;
            sv.bonusCounter[i] = shopItems[i].bonusCounter;

        }
        sv.date[0] = DateTime.Now.Year; sv.date[1] = DateTime.Now.Month; sv.date[2] = DateTime.Now.Day;
        sv.date[3] = DateTime.Now.Hour; sv.date[4] = DateTime.Now.Minute; sv.date[5] = DateTime.Now.Second;
        PlayerPrefs.SetString("SV", JsonUtility.ToJson(sv));
    }
#endif
}

[Serializable]
public class Item
{
    [Tooltip("Название используемое на кпоке")]
    public string name;
    [Tooltip("Цена товара")]
    public int cost;
    [Tooltip("Текущий бонус к клику")]
    public int bonusIncrease;
    [HideInInspector]
    public int levelOfItem; //lvl товара
    [Space]
    [Tooltip("Нужен ли множитель для цены?")]
    public bool needCostMultiplier;
    [Tooltip("Множитель цены")]
    public int costMultiplier;
    [Space]
    [Tooltip("Товар дает бонус/сек")]
    public bool itsItemPerSec;
    [Tooltip("Бонус/сек")]
    public int bonusPerSec;
    [HideInInspector]
    public int bonusCounter; //количество авто-кликов
    [Space]
    [Tooltip("Это временный бонус?")]
    public bool itsBonus;
    [Tooltip("Множитель товара, который управляется бонусом(Умножается переменная bonusPerSec")]
    public int itemMultiplier;
    [Tooltip("Индекс товара, который будет управляться бонусом(Умножается переменная bonusPerSec этого товара")]
    public int itemIndex;
    [Tooltip("Длительность бонуса")]
    public float timeOfBonus;
}

[Serializable]
public class Save
{
    public int score;
    public int[] levelOfItem;
    public int[] bonusCounter;
    public int[] date = new int[6];
    //public bool abonus;
    public int obonustime;
}