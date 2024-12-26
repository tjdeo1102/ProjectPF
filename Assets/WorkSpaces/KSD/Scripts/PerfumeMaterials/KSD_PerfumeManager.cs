using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum MakeType
{
    MakeConcentrate, MakeNote, MakePerfume
}
public class KSD_PerfumeManager : MonoBehaviour
{
    public static KSD_PerfumeManager Instance { get; private set; }

    [Header("레시피 등록")]
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

    public bool IsValidConcentrateRecipe(List<KSD_PerfumeMaterialInfo> materials, out KSD_PerfumeNoteInfo concentrateInfo)
    {
        // 1. 원액 제작: 비율 상관없이 서로 다른 재료 두개
        concentrateInfo = null;
        // 가공 상태가 아닌 재료를 넣었을 경우에는 기본적으로 실패
        foreach (var mat in materials)
        {
            if (mat.State != PerfumeMaterialState.Process)
            {
                return false;
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

            // 1-1. 넣은 재료의 가짓수가 같지 않으면 continue
            if (recipe.NeedMaterials.Count != materials.Count) continue;

            // 1-2. 재료의 종류가 다르면 continue
            // 서로 배열 및 리스트를 이름순으로 정렬 후, 서로의 종류 비교
            var matList = materials.OrderBy(x => x.Name).ToList();
            var recipeList = recipe.NeedMaterials.OrderBy(x => x.Name).ToList();

            var isCheck = true;
            // 이미 위에서 두 배열및 리스트의 크기가 같음을 확인
            for (int i = 0; i < matList.Count; i++)
            {
                // 만약 하나라도 다르면 종류가 일치하지 않음
                if (matList[i].Name != recipeList[i].Name)
                {
                    isCheck = false;
                    break;
                }
            }

            if (isCheck)
            {
                Debug.Log($"일치하는 레시피 발견: {recipe} ");
                concentrateInfo = recipe.ResultConcentrate;
                return true;
            }
        }
        // 리턴값이 없는 것은 레시피를 발견하지 못한 경우
        return false;
    }  

    public bool IsValidNoteRecipe(List<KSD_PerfumeNoteInfo> concentrates, out KSD_PerfumeNoteInfo noteInfo)
    {
        //2. 노트 제작: 원액 + 알코올, 비율도 고려
        noteInfo = null;
        // 원액 상태가 아닌 노트를 넣었을 경우에는 기본적으로 실패
        foreach (var con in concentrates)
        {
            if (con.State != PerfumeNoteState.Concentrate)
            {
                return false;
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

            // 1-1. 넣은 원액의 개수가 같지 않으면 continue
            if (recipe.NeedConcentrates.Count != concentrates.Count) continue;

            // 1-2. 원액의 종류가 같으면서 재료 투입 횟수가 일치하는 경우에 레시피 일치
            // 서로 배열 및 리스트를 이름순으로 정렬 후, 서로의 종류 비교
            var concentrateList = concentrates.OrderBy(x => x.Name).ToList();
            var recipeList = recipe.NeedConcentrates.OrderBy(x => x.Name).ToList();

            // 이미 서로의 원액 가짓수가 같음을 확인
            var isCheck = true;
            for (var i = 0; i < concentrateList.Count; i++)
            {
                // 1-2-1. 서로의 이름이 다르거나
                // 1-2-2. 서로의 재료 투입 횟수가 다른 경우에는 레시피 불일치
                if (recipeList[i].Name != concentrateList[i].Name
                    || recipeList[i].NoteCount != concentrateList[i].NoteCount)
                {
                    isCheck = false;
                    break;
                }
            }

            if (isCheck)
            {
                noteInfo = recipe.ResultNote;
                Debug.Log($"일치하는 레시피 발견: {recipe} ");
                return true;
            }
        }

        // 리턴값이 없는 것은 레시피를 발견하지 못한 경우
        return false;
    }

    public bool IsValidPerfumeRecipe(List<KSD_PerfumeNoteInfo> notes, out KSD_PerfumeInfo perfumeInfo)
    {
        // 3. 향수 제작: 여러개의 노트 + 제각기 다른 노트 투입 횟수 => 비율이 아님
        perfumeInfo = null;
        bool isExistRecipe = false;

        // 노트 상태가 아닌 경우에는 기본적으로 실패
        foreach (var note in notes)
        {
            if (note.State != PerfumeNoteState.Note)
            {
                return isExistRecipe;
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
            if (recipe.NeedNotes.Count != notes.Count) continue;

            // 1-2. 재료 종류가 같으면서, 재료 투입 횟수가 같은 경우에는 레시피 일치
            // 서로 배열 및 리스트를 이름순으로 정렬 후, 서로의 종류 비교
            var noteList = notes.OrderBy(x => x.Name).ToList();
            var recipeList = recipe.NeedNotes.OrderBy(x => x.Name).ToList();

            bool isCheck = true;
            // 이미 서로의 Count가 같음을 확인
            for (var i = 0; i < noteList.Count; i++)
            {
                // 1-2-1. 서로의 이름이 다르거나
                // 1-2-2. 서로의 재료 투입 횟수가 다른 경우에는 레시피 불일치
                if (recipeList[i].Name != noteList[i].Name
                    || recipeList[i].NoteCount != noteList[i].NoteCount)
                {
                    isCheck = false;
                    break;
                }
            }

            if (isCheck)
            {
                perfumeInfo = recipe.ResultPerfume;
                Debug.Log($"일치하는 레시피 발견: {recipe} ");
                return true;
            }
        }

        // 리턴값이 없는 것은 레시피를 발견하지 못한 경우
        return false;
    }
}
