using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using System;

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;
    void Start()
    {
        menuPanel.SetActive(true);
    }

    public void StartGame()
    {
        menuPanel.SetActive(false);
    }
}
