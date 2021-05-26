using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    public int price;
    public string consume;
    public List<string> receive = new List<string>();
    public List<string> passives = new List<string>();

    private void Awake()
    {
        GetComponentInChildren<Text>().text = price.ToString() + " G";
    }
}
