using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum MakeType
{
    MakeConcentrate, MakeNote, MakePerfume
}
public class KSD_RecipeManager : MonoBehaviour
{
    public static KSD_RecipeManager Instance { get; private set; }

    public KSD_ConcentrateRecipe[] ConcentrateRecipes;
    public KSD_NoteRecipe[] NoteRecipes;
    public KSD_PerfumeRecipe[] PerfumeRecipes;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool IsValidConcentrateRecipe(List<KSD_PerfumeMaterialInfo> materials)
    {
        // 1. 원액 제작: 비율 상관없이 서로 다른 재료 두개
        bool isExistRecipe = false;
        // 가공 상태가 아닌 재료를 넣었을 경우에는 기본적으로 실패
        foreach (var mat in materials)
        {
            if (mat.State != PerfumeMaterialState.Process)
            {
                return isExistRecipe;
            }
        }
        foreach (var recipe in ConcentrateRecipes)
        {
            // 예외 처리: 필요 재료가 2개가 아닌 경우
            if (recipe.NeedMaterials == null || recipe.NeedMaterials.Count != 2)
            {
                Debug.LogError($"비정상적인 레시피는 건너뜀: {recipe} ");
                continue;
            }

            // 1-1. 넣은 materials과 개수가 같지 않으면 continue
            if (recipe.NeedMaterials.Count != materials.Count) continue;


            // 1-2. materials의 종류가 다르면 continue
            // 서로 배열 및 리스트를 이름순으로 정렬 후, 서로의 종류 비교
            materials.OrderBy(x => x.Name);
            var recipeList = recipe.NeedMaterials.OrderBy(x => x.Name).ToList();

            var isCheck = true;
            // 이미 위에서 두 배열및 리스트의 크기가 같음을 확인
            for (int i = 0; i < materials.Count; i++)
            {
                // 만약 하나라도 다르면 종류가 일치하지 않음
                if (materials[i].Name != recipeList[i].Name)
                {
                    isCheck = false;
                    break;
                }
            }
            isExistRecipe = isCheck;

            if (isExistRecipe)
            {
                Debug.Log($"일치하는 레시피 발견: {recipe} ");
                return isExistRecipe;
            }
        }

        // 리턴값이 없는 것은 레시피를 발견하지 못한 경우
        return isExistRecipe;
    }  

    public bool IsValidNoteRecipe(List<KSD_PerfumeNoteInfo> notes)
    {
        //2. 노트 제작: 원액 + 알코올, 비율도 고려
        bool isExistRecipe = false;
        // 원액 상태가 아닌 노트를 넣었을 경우에는 기본적으로 실패
        foreach (var note in notes)
        {
            if (note.State != PerfumeNoteState.Concentrate)
            {
                return isExistRecipe;
            }
        }

        foreach (var recipe in NoteRecipes)
        {
            // 예외 처리: 필요 재료가 2개가 아닌 경우
            if (recipe.NeedConcentrates == null || recipe.NeedConcentrates.Count != 2)
            {
                Debug.LogError($"비정상적인 레시피는 건너뜀: {recipe} ");
                continue;
            }

            // 1-1. 넣은 materials과 개수가 같지 않으면 continue
            if (recipe.NeedConcentrates.Count != notes.Count) continue;

            // 1-2. materials의 종류가 같으면서 재료 비율이 일치하는 경우에 레시피 일치
            // 서로 배열 및 리스트를 이름순으로 정렬 후, 서로의 종류 비교
            notes.OrderBy(x => x.Name);
            var recipeList = recipe.NeedConcentrates.OrderBy(x => x.Name).ToList();

            var recipeLen = recipe.NeedConcentrates.Count;
            var noteLen = notes.Count;

            // 레시피에 들어가는 재료가 2개로 고정이므로, 한번 비교만으로 레시피 일치여부 확인 가능
            // 1-2-1. 서로 이름이 같으면서
            if (recipeList[0].Name == notes[0].Name)
            {
                var otherRecipe = recipeList[1];
                var otherNote = notes[1];
                // 1-2-2. 나머지 재료도 일치하는 지 확인
                if (otherRecipe.Name == otherNote.Name)
                {
                    //  레시피의 재료 비율 확인
                    float recipeRatio = (float)otherRecipe.NoteCount /
                                        recipeList[0].NoteCount;
                    float matRatio = (float)otherNote.NoteCount /
                                        notes[0].NoteCount;

                    // 1-2-3. 비율도 대략 같으면 레시피 일치
                    if (Mathf.Approximately(recipeRatio, matRatio))
                    {
                        isExistRecipe = true;
                    }
                }
            }

            if (isExistRecipe)
            {
                Debug.Log($"일치하는 레시피 발견: {recipe} ");
                return true;
            }
        }

        // 리턴값이 없는 것은 레시피를 발견하지 못한 경우
        return false;
    }

    public bool IsValidPerfumeRecipe(List<KSD_PerfumeNoteInfo> notes)
    {
        // 3. 향수 제작: 여러개의 노트 + 제각기 다른 노트 투입 횟수 => 비율이 아님
        bool isExistRecipe = true;

        // 노트 상태가 아닌 경우에는 기본적으로 실패
        foreach (var note in notes)
        {
            if (note.State != PerfumeNoteState.Note)
            {
                return false;
            }
        }

        foreach (var recipe in PerfumeRecipes)
        {
            // 예외 처리: 필요재료 리스트가 없는 경우
            if (recipe.NeedNotes == null)
            {
                Debug.LogError($"비정상적인 레시피는 건너뜀: {recipe} ");
                continue;
            }
            // 1-1. materials의 가짓수가 다른 경우는 Continue
            var recipeLen = recipe.NeedNotes.Count;
            var noteLen = notes.Count;
            if (recipeLen != noteLen) continue;

            // 1-2. 재료 종류가 같으면서, 재료 투입 횟수가 같은 경우에는 레시피 일치
            // 서로 배열 및 리스트를 이름순으로 정렬 후, 서로의 종류 비교
            notes.OrderBy(x => x.Name);
            var recipteList = recipe.NeedNotes.OrderBy(x => x.Name).ToList();

            // 이미 서로의 Count가 같음을 확인
            for (var i = 0; i < noteLen; i++)
            {
                print(i);
                print(recipteList[i].Name);
                print(notes[i].Name);
                // 1-2-1. 서로의 이름이 다르거나
                if (recipteList[i].Name != notes[i].Name)
                {
                    isExistRecipe = false;
                    break;
                }
                // 1-2-2. 서로의 재료 투입 획수가 다른 경우에는 레시피 불일치
                else if (recipteList[i].NoteCount != notes[i].NoteCount)
                {
                    isExistRecipe = false;
                    break;
                }
            }

            if (isExistRecipe)
            {
                Debug.LogError($"일치하는 레시피 발견: {recipe} ");
                return true;
            }
        }

        // 리턴값이 없는 것은 레시피를 발견하지 못한 경우
        return false;
    }
}
