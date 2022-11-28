using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UI : MonoBehaviour
{
    private VisualElement buildDrawer;
    private VisualElement actionDrawer;

    void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        buildDrawer = root.Q<VisualElement>("BuildDrawer");
        actionDrawer = root.Q<VisualElement>("ActionDrawer");

        InitActionDrawer();
        InitBuildDrawer();

        buildDrawer.style.display = DisplayStyle.None;
    }

    private void InitActionDrawer()
    {
        Button buildMenuButton = actionDrawer.Q<Button>("OpenBuildDrawer");
        buildMenuButton.clicked += OpenBuildMode;
    }

    private void InitBuildDrawer()
    {
        buildDrawer.Q<Button>("Build_0").clicked += () => Managers.InputManager.BuildMode.SelectBuildingType(BuildingType.Forrester);
        buildDrawer.Q<Button>("Build_1").clicked += () => Managers.InputManager.BuildMode.SelectBuildingType(BuildingType.LumberMill);
        buildDrawer.Q<Button>("Build_2").clicked += () => Managers.InputManager.BuildMode.SelectBuildingType(BuildingType.Conveyor);
    }

    private void OpenBuildMode()
    {
        Managers.InputManager.OpenBuildMode();
        buildDrawer.style.display = DisplayStyle.Flex;
        actionDrawer.style.display = DisplayStyle.None;
    }
}
