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
    [Header("Текст на кнопках товара")]
    public Text[] shopItemsText;
    [Header("Кнопки Товаров")]
    public Button[] shopBttns;
    [Header("Панелька магазина")]
    public GameObject shopPan;

    private int score;
    public int scoreIncrease ; //боунс
    public int cost;

    private void Start()
    {
        updateCosts(); //обновление текста
        StartCoroutine(BonusPerSec()); //тик в секунду


    }
    private void Update() //очки
    {
        scoreText.text = score + "\n МурМонет";
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
        if (score >= cost && shopItems[2].bonusCounter > 0)
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
            }
            else print("Нечего улучшать"); // заменить на вибрацию

        }
        else if (score >= shopItems[index].cost)
        {
            if (shopItems[index].itsItemPerSec)
            {
                shopItems[index].bonusCounter++; // авто-клик
                score -= shopItems[index].cost;
            }
            else
            { 
                scoreIncrease += shopItems[index].bonusIncrease; //увеличение клика 
                score -= shopItems[index].cost;
            }
        }
        else print("Не хватает МурМонет");
        updateCosts();
    }
    private void updateCosts()
    {
        for (int i = 0; i < shopItems.Count; i++ )
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
            for (int i =0; i<shopItems.Count; i++)
            {
                score += (shopItems[i].bonusCounter * shopItems[i].bonusPerSec); // инкрис тика в секунду
                yield return new WaitForSeconds(1); //задержка

            }
        }

    }

   IEnumerator BonusTimer(float time, int index )
    {
    shopBttns[index].interactable = false;
    shopItems[shopItems[index].itemIndex].bonusPerSec *= 2;
    scoreIncrease *= 2;
    yield return new WaitForSeconds(time);
    shopBttns[index].interactable = true;
    shopItems[shopItems[index].itemIndex].bonusPerSec /= 2;
    scoreIncrease /= 2;

    }
    public void showShopPan()
    {
        shopPan.SetActive(!shopPan.activeSelf);
    }
    public void OnClick()
    {
        score += scoreIncrease; //клик
    }
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
    public int levelOfItems; //lvl товара
    [Space]
    [Tooltip("Нужен ли множитель для цены?")]
    public bool needCostMultiplier;
    [Tooltip("Множитель цены")]
    public float costMultiplier;
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