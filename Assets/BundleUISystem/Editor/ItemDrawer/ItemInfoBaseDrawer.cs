﻿using UnityEngine;
using System.Collections;
using UnityEditor;
using BundleUISystem;
using System;
//bundle: //var bundleName = property.FindPropertyRelative("bundleName");

/// <summary>
/// 用于绘制三种加载资源的item类型
/// </summary>
public abstract class ItemInfoBaseDrawer : PropertyDrawer
{
    protected SerializedProperty assetNameProp;
    protected SerializedProperty typeProp;
    protected SerializedProperty parentLayerProp;
    protected SerializedProperty resetProp;
    protected SerializedProperty buttonProp;
    protected SerializedProperty toggleProp;
    protected SerializedProperty instanceIDProp;
    protected SerializedObject serializedObject;
    protected const float widthBt = 20;
    protected float singleHeight;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        singleHeight = EditorGUIUtility.singleLineHeight;
        this.serializedObject = property.serializedObject;
        InitPropertys(property);
        if (!property.isExpanded)
        {
            return EditorGUIUtility.singleLineHeight;
        }
        else
        {
            return GetInfoItemHeight();
        }
    }
    protected virtual void InitPropertys(SerializedProperty property)
    {
        assetNameProp = property.FindPropertyRelative("assetName");
        typeProp = property.FindPropertyRelative("type"); ;
        parentLayerProp = property.FindPropertyRelative("parentLayer");
        resetProp = property.FindPropertyRelative("reset");
        buttonProp = property.FindPropertyRelative("button");
        toggleProp = property.FindPropertyRelative("toggle");
        instanceIDProp = property.FindPropertyRelative("instanceID");
    }

    protected abstract float GetInfoItemHeight();

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Rect btnRect = new Rect(position.xMin, position.yMin, position.width * 0.9f, singleHeight);

        if (GUI.Button(btnRect, assetNameProp.stringValue, EditorStyles.toolbar))
        {
            ResetBuildInfoOnOpen();

            property.isExpanded = !property.isExpanded;

            if (property.isExpanded)
            {
                if (instanceIDProp.intValue == 0)
                {
                    var gopfb = GetPrefabItem();
                    if(gopfb != null)
                    {
                        InstantiatePrefab(gopfb);
                    }
                    else
                    {
                        Debug.Log("未找到预制体:" + assetNameProp.stringValue);
                    }
                }
            }
            else
            {
                if(instanceIDProp.intValue != 0)
                {
                    HideItemIfInstenced();
                }
                instanceIDProp.intValue = 0;
            }
        }

        WorningIfNotRight(btnRect);

        Rect acceptRect = new Rect(position.max.x - position.width * 0.1f, position.yMin, position.width * 0.1f, singleHeight);

        DragAndDrapAction(acceptRect);

        DrawObjectField(acceptRect);

        if (property.isExpanded)
        {
            Rect opendRect = new Rect(position.xMin, position.yMin + singleHeight, position.width, position.height - singleHeight);
            DrawExpanded(opendRect);
        }
    }

    protected abstract void DrawExpanded(Rect opendRect);

    protected abstract void DrawObjectField(Rect acceptRect);

    protected virtual void DragAndDrapAction(Rect acceptRect)
    {
        switch (Event.current.type)
        {
            case EventType.DragUpdated:
                if (acceptRect.Contains(Event.current.mousePosition))
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                }
                break;
            case EventType.DragPerform:
                if (acceptRect.Contains(Event.current.mousePosition))
                {
                    if (DragAndDrop.objectReferences.Length > 0)
                    {
                        var obj = DragAndDrop.objectReferences[0];
                        if (obj is GameObject)
                        {
                            OnDragPerformGameObject(obj as GameObject);
                        }
                    }
                    DragAndDrop.AcceptDrag();
                    Event.current.Use();
                }
                break;
        }
    }

    protected abstract void OnDragPerformGameObject(GameObject go);

    protected virtual void WorningIfNotRight(Rect rect)
    {
        switch ((ItemInfoBase.Type)typeProp.enumValueIndex)
        {
            case ItemInfoBase.Type.Name:
                break;
            case ItemInfoBase.Type.Button:
                if (buttonProp.objectReferenceValue == null)
                {
                    Worning(rect, "button lost!");
                }
                break;
            case ItemInfoBase.Type.Toggle:
                if (toggleProp.objectReferenceValue == null)
                {
                    Worning(rect, "toggle lost!");
                }
                break;
            case ItemInfoBase.Type.Enable:
                break;
            default:
                break;
        }
    }

    protected abstract void HideItemIfInstenced();

    protected virtual void ResetBuildInfoOnOpen()
    {
        //使用对象是UIGroupObj，将无法从button和Toggle加载
        if (serializedObject.targetObject is GroupObj)
        {
            if (typeProp.enumValueIndex == (int)ItemInfoBase.Type.Button || typeProp.enumValueIndex == (int)ItemInfoBase.Type.Toggle)
            {
                typeProp.enumValueIndex = (int)ItemInfoBase.Type.Name;
            }
        }
    }
    protected abstract GameObject GetPrefabItem();

    protected virtual void InstantiatePrefab(GameObject gopfb)
    {
        if (gopfb != null)
        {
            GameObject go = PrefabUtility.InstantiatePrefab(gopfb) as GameObject;

            var obj = serializedObject.targetObject;

            if (obj is UIGroup)
            {
                if (go.GetComponent<Transform>() is RectTransform)
                {
                    go.transform.SetParent((obj as UIGroup).transform, false);
                }
                else
                {
                    go.transform.SetParent((obj as UIGroup).transform, true);
                }
            }
            else if (obj is GroupObj)
            {
                if (go.GetComponent<Transform>() is RectTransform)
                {
                    var canvas = GameObject.FindObjectOfType<Canvas>();
                    go.transform.SetParent(canvas.transform, false);
                }
                else
                {
                    go.transform.SetParent(null);
                }
            }

            if (resetProp.boolValue)
            {
                go.transform.position = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
            }
            instanceIDProp.intValue = go.GetInstanceID();
        }
    }
    protected virtual void Worning(Rect rect, string info)
    {
        GUI.color = Color.red;
        EditorGUI.SelectableLabel(rect, info);
        GUI.color = Color.white;
    }
}