using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Game : MonoBehaviour
{
    public Text scoreText;
    private int bonus = 1;
    private long score;
    private int autoclickCount, autoclickBonus = 1;
    [Header("Магазин")]
    public float[] bonusTime;
    public GameObject SHOP;
    public int[] shopBonuses;
    public int[] shopCosts;
    public Button[] ShopButtons;
    public long cost;
    public Text[] shopButtnsText;
    private void Start()
    {
        StartCoroutine(BonusPerSec());
    }

    private void Update()
    {
        scoreText.text = score + " МурМонет";
        if (score >= shopCosts[0])
        {
            ShopButtons[0].interactable = true;
        }
        else
        {
            ShopButtons[0].interactable = false;
        }
        if (score >= shopCosts[1])
        {
            ShopButtons[1].interactable = true;
        }
        else
        {
            ShopButtons[1].interactable = false;
        }
        if (score >= cost && autoclickCount>0)
        {
            ShopButtons[2].interactable = true;
        }
        else
        {
            ShopButtons[2].interactable = false;
        }
    }
    public void ShopPan_ShowAndHide()
    {
        SHOP.SetActive(!SHOP.activeSelf);


    }

    public void ShopBttn_addbonus(int index = 0) //первая кнопка
    {

        if (score >= shopCosts[index])
        {
            ShopButtons[index].interactable = true;
            bonus += shopBonuses[index];
            score -= shopCosts[index];
            shopCosts[index] *= 2;
            shopButtnsText[index].text = "Увеличить количество МурМонет за щелчок: \n" + shopCosts[index] + "  МурМонет";
        }
        else
        {
            Debug.Log("Не хвтает МурМонет!");
        }
    }

    public void PerpetualMotion(int index)   //вторая кнопка
    {
        if (score >= shopCosts[index])
        {
            ShopButtons[index].interactable = true;
            autoclickCount++;
            score -= shopCosts[index];
            shopCosts[index] *= 2;
            cost = 2 * autoclickCount * 150;
            shopButtnsText[index].text = "Добавить АВТО-ТАП: \n" + shopCosts[index] + "  МурМонет";
            if (autoclickCount > 1)
            {
                shopButtnsText[2].text = "Получить БОНУС : \n" + cost + "  МурМонет";
            }
            else
            { shopButtnsText[2].text = "Получить БОНУС : \n  300 МурМонет"; }
        }
        
    }
    public void startBonusTimer(int index) //третья кнопка

    {
        
      //  shopButtnsText[2].text = "Поулчить БОНУС : \n" + cost + "  МурМонет";
            if (score >= cost)
            {
               
                StartCoroutine(BonusTimer(bonusTime[index], index));
                score -= cost;
            }
            
        
    }
    IEnumerator BonusPerSec()
    {
        while (true)
        {
            score += (autoclickCount * autoclickBonus);
            yield return new WaitForSeconds(1);

        }
    }
    IEnumerator BonusTimer(float time, int index)
    {
        
        ShopButtons[index].interactable = false;
        
        if (index == 2 && autoclickCount>0)
        {
            autoclickBonus *= 2;
            bonus *= 2;
            yield return new WaitForSeconds(time);
            autoclickBonus /= 2;
            bonus /= 2;
            
        }
        ShopButtons[index].interactable = true;
    }
    

    public void OnClick()
    {
        score+=bonus;
       


    }
}
