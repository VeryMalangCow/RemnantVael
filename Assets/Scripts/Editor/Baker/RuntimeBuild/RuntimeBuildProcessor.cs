using UnityEngine;

/*
프리팹 내부의 모든 DepthController를 탐색
DepthController의 thisSr를 초기화(Sprite, Material → None)
InteractableBuildController인 경우 stateAnim.sr도 함께 초기화
처리 결과를 BakeResult에 기록
저장, 로그 출력, 경로 처리 등은 수행하지 않고 Build 프리팹 변환만 담당
*/

/// <summary> Build 프리팹을 Runtime용으로 변환 </summary>
public static class RuntimeBuildProcessor
{
    public static void Process(GameObject root, BakeResult result)
    {
        if (root == null || result == null)
            return;

        DepthController[] controllers =
            root.GetComponentsInChildren<DepthController>(true);

        result.TotalCount = controllers.Length;

        foreach (DepthController controller in controllers)
        {
            ProcessDepth(controller, result);
        }
    }

    private static void ProcessDepth(DepthController controller, BakeResult result)
    {
        if (controller == null)
            return;

        if (!ProcessRenderer(controller.thisSr, $"{controller.name} : Main SpriteRenderer", result))
            return;

        if (controller is InteractableBuildController interactable)
        {
            if (!ProcessInteractable(interactable, result))
                return;
        }

        result.SuccessCount++;
    }

    private static bool ProcessInteractable(InteractableBuildController controller, BakeResult result)
    {
        if (controller.StateAnim == null)
        {
            result.AddWarning($"{controller.name} : Missing StateAnimController.");
            return false;
        }

        if (!ProcessRenderer(controller.StateAnim.sr, $"{controller.name} : StateAnim SpriteRenderer", result))
            return false;

        if (controller is VaultController vault)
            if (!ProcessVault(vault, result)) return false;

        if (controller is OperatorController oper)
            if (!ProcessOperator(oper, result)) return false;

        if (controller is PrisonController prison)
            if (!ProcessPrison(prison, result)) return false;

        return true;
    }

    private static bool ProcessVault(VaultController controller, BakeResult result)
    {
        return ProcessRenderer(controller.PanelStateAnim.sr, $"{controller.name} : Vault StateAnim SpriteRenderer", result);
    }

    private static bool ProcessOperator(OperatorController controller, BakeResult result)
    {
        controller.SetPayTxt();

        return ProcessRenderer(controller.IconStateAnim.sr, $"{controller.name} : Operator StateAnim SpriteRenderer", result) &&
            ProcessRenderer(controller.AnnoIconSr, $"{controller.name} : Operator AnnoIcon SpriteRenderer", result);
    }

    private static bool ProcessPrison(PrisonController controller, BakeResult result)
    {
        controller.SetBakeTxt();

        var allySprites = controller.allySrs;
        for (int i = 0; i < allySprites.Count; i++)
        {
            if (!ProcessRenderer(allySprites[i], $"{controller.name} : Prison Ally SpriteRenderer", result))
                return false;
        }

        return ProcessRenderer(controller.UpsideSr, $"{controller.name} : Prison Upside SpriteRenderer", result) &&
            ProcessRenderer(controller.DangerIconSr, $"{controller.name} : Prison DangerIcon SpriteRenderer", result) &&
            ProcessRenderer(controller.TypeIconSr, $"{controller.name} : Prison TypeIcon SpriteRenderer", result) &&
            ProcessRenderer(controller.MiddleLineSr, $"{controller.name} : Prison MiddleLine SpriteRenderer", result) &&
            ProcessRenderer(controller.FloorSr, $"{controller.name} : Prison Floor SpriteRenderer", result);
    }

    private static bool ProcessRenderer(SpriteRenderer sr, string ownerName, BakeResult result)
    {
        if (sr == null)
        {
            result.AddWarning($"{ownerName} : Missing SpriteRenderer.");
            return false;
        }

        sr.sprite = null;
        sr.sharedMaterial = null;

        return true;
    }
}
