using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class BuildDrawer : Drawer
{
    private List<Button> BuildingButtons;
    private const string BUTTON_SELECTED_CLASS = "grid-button-selected";

    public BuildDrawer(VisualElement root) : base(root)
    {
        BuildingButtons = new List<Button>();
        int i = 0;
        Button currentButton = root.Q<Button>($"Build_{i}");
        do
        {
            BuildingButtons.Add(currentButton);
            i += 1;
            currentButton = root.Q<Button>($"Build_{i}");
        }
        while (currentButton != null);

        BuildingButtons[0].clicked += () => SelectBuilding(BuildingButtons[0], BuildingType.LumberCamp);
        BuildingButtons[1].clicked += () => SelectBuilding(BuildingButtons[1], BuildingType.LumberMill);
        BuildingButtons[2].clicked += () => SelectBuilding(BuildingButtons[2], BuildingType.Conveyor);
        BuildingButtons[3].clicked += () => SelectBuilding(BuildingButtons[3], BuildingType.Miner);
        BuildingButtons[4].clicked += () => SelectBuilding(BuildingButtons[4], BuildingType.StoneCarver);

        Button backButton = this.Root.Q<Button>("Back");
        backButton.clicked += GoBack;
    }
    
    private void SelectBuilding(Button button, BuildingType buildingType)
    {
        Debug.Log($"Selecting button {button.name} with type {buildingType}");

        Managers.InputManager.BuildMode.SelectBuildingType(buildingType);

        foreach (Button iterButton in BuildingButtons)
        {
            iterButton.RemoveFromClassList(BUTTON_SELECTED_CLASS);
        }
        button.AddToClassList(BUTTON_SELECTED_CLASS);
    }

    private void GoBack()
    {
        Managers.UI.Back();
        Managers.InputManager.SetGameInputMode();
    }
}