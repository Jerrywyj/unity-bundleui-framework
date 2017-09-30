﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BundleUISystem;

public class Demo : MonoBehaviour
{
    string panel1 = "Panel1";
    Hashtable table = new Hashtable();
    private void Start()
    {
        table["tableItem1"] = "你好";
    }
    void OnGUI()
    {
        if (GUILayout.Button("打开panel1"))
        {
            
            UIGroup.Open(panel1, (x) => { Debug.Log("callBack panel1" + x.Data); }, table);
        }
        if (GUILayout.Button("打开panel1 1000 次"))
        {
            for (int i = 0; i < 1000; i++)
            {
                BundleUISystem.UIGroup.Open(panel1, (x) => { Debug.Log("onClose panel1" + x); }, table);
            }
        }
        if (GUILayout.Button("关闭panel1"))
        {
            BundleUISystem.UIGroup.Close(panel1);
        }
        if (GUILayout.Button("打开Poppanel【层级为10】"))
        {
            BundleUISystem.UIGroup.Open("PopPanel");
        }
        if (GUILayout.Button("关闭Poppanel"))
        {
            BundleUISystem.UIGroup.Close("PopPanel");
        }
        if (GUILayout.Button("打开Poppanel1【层级为6】"))
        {
            BundleUISystem.UIGroup.Open("PopPanel 1");
        }
        if (GUILayout.Button("关闭Poppanel1"))
        {
            BundleUISystem.UIGroup.Close("PopPanel 1");
        }
    }
}

class User
{
    public string Name { get; set; }
}