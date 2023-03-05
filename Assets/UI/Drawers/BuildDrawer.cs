using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class BuildDrawer : Drawer
{
    private List<Button> BuildingButtons;
    private const string BUTTON_SELECTED_CLASS = "grid-button-selected";

    private List<BuildingType> Buildings = new List<BuildingType>
    {
        BuildingType.LumberCamp,
        BuildingType.SawMill,
        BuildingType.Conveyor,
        BuildingType.Mine,
        BuildingType.StoneCarver,
        BuildingType.Assembler,
        BuildingType.GuardTower,
    };

    public BuildDrawer()
    {
        this.BuildingButtons = new List<Button>();
        foreach (BuildingType building in Buildings)
        {
            Button button = new SquareButton(
                onClick: () => Managers.InputManager.BuildMode.SelectBuildingType(building),
                icon: Icons.GetBuildingIcon(building)
            );

            BuildingButtons.Add(button);
            this.Add(button);
        }

        Button backButton = new Button();
        backButton.AddToClassList("floating-circle-button");
        backButton.clicked += GoBack;
        this.Add(backButton);
    }

    private void GoBack()
    {
        Managers.InputManager.OpenSelectedCharacterMode(Managers.MainCharacter);
    }
}