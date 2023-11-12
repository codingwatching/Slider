using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JungleRecipeBookUI : MonoBehaviour
{
    public RecipeList recipeList;

    public Image displayImage;
    public TextMeshProUGUI displayNameText;
    public TextMeshProUGUI displayNumberText;
    public TextMeshProUGUI recipeCompletionText;
    public List<JungleRecipeWidget> recipeWidgets; // There are three of these
    public List<BumpChildren> bumps; // Unit can't serialize interfaces :(
    public List<JungleRecipeBookStatNumber> statNumbers; // Unit can't serialize interfaces :(

    private int currentShapeIndex; // Horizontal
    private int currentRecipePageIndex; // Vertical
    private Shape currentShape;

    private const float BUMP_DELAY = 1f / 16f;

    public void SetCurrentShape(int index, bool withSound = true)
    {
        StartCoroutine(_SetCurrentShape(index, withSound));
    }

    private IEnumerator _SetCurrentShape(int index, bool withSound = true)
    {
        currentShapeIndex = index;
        currentShape = recipeList.list[index].result;
        
        displayImage.sprite = currentShape.GetDisplaySprite();
        displayNameText.text = currentShape.GetDisplayName();
        displayNumberText.text = (index + 1).ToString().PadLeft(2, '0');

        for (int i = 0; i < 3; i++)
        {
            bumps[i].DoBump(withSound: withSound && index == currentShapeIndex);
            yield return new WaitForSeconds(BUMP_DELAY / 2);
        }

        bumps[3].DoBump(withSound: withSound && index == currentShapeIndex);

        int totalRecipes = recipeList.list[index].combinations.Count;
        int completedRecipes = 0;
        foreach (Recipe.Shapes s in recipeList.list[index].combinations)
        {
            if (s.numberOfTimesCreated > 0)
            {
                completedRecipes += 1;
            }
        }
        recipeCompletionText.text = $"{completedRecipes}/{totalRecipes}";

        yield return new WaitForSeconds(BUMP_DELAY);

        SetCurrentRecipeDisplay(index, 0, withSound && index == currentShapeIndex);
    }

    public void SetCurrentRecipeDisplay(int currentShapeIndex, int recipeIndex, bool withSound = true)
    {
        StartCoroutine(_SetCurrentRecipeDisplay(currentShapeIndex, recipeIndex, withSound));
    }

    private IEnumerator _SetCurrentRecipeDisplay(int currentShapeIndex, int recipeIndex, bool withSound = true)
    {
        currentRecipePageIndex = recipeIndex;

        for (int i = 0; i < recipeWidgets.Count; i++)
        {
            Recipe.Shapes shapes = null;
            if (recipeIndex + i < recipeList.list[currentShapeIndex].combinations.Count)
            {
                shapes = recipeList.list[currentShapeIndex].combinations[recipeIndex + i];
            }
            recipeWidgets[i].SetIngredientsOrNull(shapes, recipeList.list[currentShapeIndex].result);

            bumps[i + 4].DoBump(withSound: withSound && shapes != null && currentRecipePageIndex == recipeIndex);

            yield return new WaitForSeconds(BUMP_DELAY);
        }

        // Putting it here bc lazy
        UpdateStats(withSound && currentRecipePageIndex == recipeIndex);
    }

    private void UpdateStats(bool withSound = true)
    {
        // Stats bump
        bumps[7].DoBump(withSound: withSound);

        foreach (JungleRecipeBookStatNumber n in statNumbers)
        {
            n.RefreshNumber();
        }
    }

    public void IncrementCurrentShape()
    {
        int index = (currentShapeIndex + 1) % recipeList.list.Count;
        SetCurrentShape(index);
    }

    public void DecrementCurrentShape()
    {
        int index = (currentShapeIndex + recipeList.list.Count - 1) % recipeList.list.Count;
        SetCurrentShape(index);
    }

    public void IncrementRecipeDisplay()
    {
        int count = recipeList.list[currentShapeIndex].combinations.Count / 3;
        if (count <= 1)
            return;

        int index = (currentRecipePageIndex + 1) % count;
        SetCurrentRecipeDisplay(currentShapeIndex, index);
    }

    public void DecrementRecipeDisplay()
    {
        int count = recipeList.list[currentShapeIndex].combinations.Count / 3;
        if (count <= 1)
            return;
            
        int index = (currentRecipePageIndex + count - 1) % count;
        SetCurrentRecipeDisplay(currentShapeIndex, index);
    }
}