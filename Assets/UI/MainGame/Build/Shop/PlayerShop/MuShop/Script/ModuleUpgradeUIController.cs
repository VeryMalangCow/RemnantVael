using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModuleUpgradeUIController : PlayerShopUIController
{
    [Space(20)]
    [Header("<><><><><> Module Upgrade Shop")]

    [Space(10)]
    [Header("=== Item")]
    [SerializeField] public TMP_Text bcTxt;
    [SerializeField] public TMP_Text ecTxt;
    [SerializeField] public TMP_Text msTxt;

    [Space(10)]
    [SerializeField] private List<TMP_Text> synergyLvTxtList;

    [Space(10)]
    [Header("=== Desc")]
    [SerializeField] private DescMUEUIController descPanelPrefab;
    private DescMUEUIController descPanel;

    [Space(10)]
    [Header("=== Drag")]
    [SerializeField] private InventoryItemEUIController dragItemEui;


    [Space(20)]
    [Header("=== Module")]

    [Space(10)]
    [Header("-- In Equip")]
    [SerializeField] public InventoryEUIController inventory_InEquip;
    [SerializeField] private OwnBtnEUIController toggleBtn_InEquip;

    [Space(5)]
    [Header("* Equip")]
    [SerializeField] private GameObject equippedPanelGo;
    [SerializeField] private Transform equippedSlotsParentTf;
    [SerializeField] private Transform equippedInnerParentTf;

    [Space(5)]
    [Header("* Synergy")]
    [SerializeField] private GameObject synergyPanelGo;
    [SerializeField] private CoupleData<GameObject> synergyPanelIsExistGo;
    [SerializeField] private SynergySlotEUIController synergyPrefab;
    [SerializeField] private Transform synergySlotsParentTf;
    [SerializeField] private Vector2 synergyInterval;

    [SerializeField] private Transform synergyDescsParentTf;
    [SerializeField] public List<Sprite> synergyTierFrames;

    [Space(5)]
    [Header("* Synergy Desc")]
    [SerializeField] private Image selectViewImg;
    [SerializeField] private TMP_Text selectViewName;
    [SerializeField] private TMP_Text selectViewAmalgamation;
    [SerializeField] private Transform synergyDescLinerParentTf;
    [SerializeField] private Transform synergyDescTextParentTf;
    [SerializeField] private List<TMP_Text> amalgamationTxtList;
    [SerializeField] public List<TMP_Text> amalgamationDescTxtList;

    [Space(10)]
    [Header("-- In Forge")]
    [SerializeField] public InventoryEUIController inventory_InForge;

    [Space(5)]
    [SerializeField] private TMP_Text noticeTxt;
    [SerializeField] private TMP_Text warningTxt;


    [Space(10)]
    [Header("-- In Forge -- Element View")]
    [SerializeField] private Transform forgeElementViewParentTf;

    [SerializeField] private DecompositionView decompositionViewPrefab;
    [SerializeField] private OwnBtnEUIController decompositionPanelBtn;
    private DecompositionView decompositionView;

    [SerializeField] private FusionView fusionViewPrefab;
    [SerializeField] private OwnBtnEUIController fusionPanelBtn;
    private FusionView fusionView;

    [SerializeField] private MakeView makeViewPrefab;
    [SerializeField] private OwnBtnEUIController makePanelBtn;
    private MakeView makeView;

    private List<ForgeElementView> forgeElementViews;
    private ForgeElementView currentForgeElementView;

    [Space(5)]
    [Header("=== Visual")]
    [SerializeField] Image[] subImgs;

    // string
    public static string amalgamationName;
    public static string notice_Equiped;
    public static string warning_NotSameRank;
    public static string warning_NotEnoughItem;
    public static string warning_AlreadyMaxLv;
    public static string warning_InvenFull;

    // Current
    [HideInInspector] public InventoryItemEUIController currentDraggingItemBtn = null;

    // Panel

    // Equiped
    private List<InventorySlotEUIController> equipedSlots;
    private List<Image> equipPanelInnerList;
    private List<TMP_Text> equipDescStateTxtList;

    // Synergy
    private List<SynergySlotEUIController> synergySlots;
    private SynergySlotEUIController selectedSynergySlot;

    // Forge Element
    public static readonly float forgeElementBtnOnAlpha = 1f;
    public static readonly float forgeElementBtnOffAlpha = 0.5f;

    // Forge Anno
    private CanvasGroup noticeCg;
    private CanvasGroup warningCg;

    // Drag
    private RectTransform dragItemRt;
    private bool isDragging;

    // Init
    public override IEnumerator InitAsync(Color mainClr, Color subClr)
    {
        yield return base.InitAsync(mainClr, subClr);

#if UNITY_EDITOR
        Stopwatch sw = Stopwatch.StartNew();
#endif
        // Desc
        descPanel = Instantiate(descPanelPrefab, transform);
        descPanel.Offset();
#if UNITY_EDITOR
        sw.Stop();
        UnityEngine.Debug.Log($"<color=yellow>Desc</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
#endif

#if UNITY_EDITOR
        sw.Restart();
#endif
        // Drag
        dragItemEui.Offset();
        dragItemRt = DevTool.Get_ComponentTType(dragItemEui.gameObject, out RectTransform rt) ? rt : null;

        dragItemEui.gameObject.SetActive(false);

        // Sync Lv Txt
        for (int i = 0; i < synergyLvTxtList.Count; i++)
            synergyLvTxtList[i].text = (ModuleItemManager.synchoronyMaxLv * (i + 1)).ToString();

#if UNITY_EDITOR
        sw.Stop();
        UnityEngine.Debug.Log($"<color=yellow>Basic</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
#endif
        yield return null;


        inventory_InEquip.Offset();
        yield return inventory_InEquip.Gen_AllSlotAndItemAsync(this, 8f);

#if UNITY_EDITOR
        sw.Restart();
#endif
        toggleBtn_InEquip.Offset();
        toggleBtn_InEquip.ownerUIController = this;

        // 슬롯
        equipedSlots = DevTool.Get_ChildList<InventorySlotEUIController>(equippedSlotsParentTf);
        for (int i = 0; i < equipedSlots.Count; i++)
        {
            equipedSlots[i].ownerUIController = this;
            equipedSlots[i].Offset();
            equipedSlots[i].item.Offset();
            equipedSlots[i].item.ownerUIController = this;

            equipedSlots[i].Set_EquipedTxt(true, i);
        }

        // 이너 라인, 설명
        equipPanelInnerList = DevTool.Get_ChildList<Image>(equippedInnerParentTf);
        equipDescStateTxtList = DevTool.Get_ChildList<TMP_Text>(equippedInnerParentTf);

#if UNITY_EDITOR
        sw.Stop();
        UnityEngine.Debug.Log($"<color=yellow>Extra (Equip)</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
#endif
        yield return null;


#if UNITY_EDITOR
        sw.Restart();
#endif
        // Synergy
        int x = 9, y = 2;
        int synergySlotAmount = x * y;
        synergySlots = new List<SynergySlotEUIController>(synergySlotAmount);
        for (int i = 0; i < y; i++)
        {
            for (int j = 0; j < x; j++)
            {
                var slot = Instantiate(synergyPrefab, synergySlotsParentTf);
                synergySlots.Add(slot);
                slot.Offset();
                slot.ownerUIController = this;
                slot.Init(new Vector2(synergyInterval.x * j, synergyInterval.y * i));
            }
        }

        SetOnOff_SynergySlot(false);

        Reset_EquipPanel();

#if UNITY_EDITOR
        sw.Stop();
        UnityEngine.Debug.Log($"<color=yellow>Synergy Slot (Equip)</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
#endif
        yield return null;

        inventory_InForge.Offset();
        yield return inventory_InForge.Gen_AllSlotAndItemAsync(this, 8f);


        var words = StaticResourceManager.instance.staticWords;
        var descs = StaticResourceManager.instance.staticDescs;

#if UNITY_EDITOR
        sw.Restart();
#endif
        // decomposition
        decompositionView = Instantiate(decompositionViewPrefab, forgeElementViewParentTf);
        decompositionView.Offset(this, decompositionPanelBtn);

#if UNITY_EDITOR
        sw.Stop();
        UnityEngine.Debug.Log($"<color=yellow>Decomposition (Forge)</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
#endif
        yield return null;



#if UNITY_EDITOR
        sw.Restart();
#endif
        // fusion
        fusionView = Instantiate(fusionViewPrefab, forgeElementViewParentTf);
        fusionView.Offset(this, fusionPanelBtn);
#if UNITY_EDITOR
        sw.Stop();
        UnityEngine.Debug.Log($"<color=yellow>Fusion (Forge)</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
#endif
        yield return null;



#if UNITY_EDITOR
        sw.Restart();
#endif
        // make
        makeView = Instantiate(makeViewPrefab, forgeElementViewParentTf);
        makeView.Offset(this, makePanelBtn);
#if UNITY_EDITOR
        sw.Stop();
        UnityEngine.Debug.Log($"<color=yellow>Make (Forge)</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
#endif
        yield return null;



#if UNITY_EDITOR
        sw.Restart();
#endif
        forgeElementViews = new List<ForgeElementView>() { decompositionView, fusionView, makeView };
        // visual
        noticeCg =
            DevTool.Get_ComponentTType(noticeTxt.gameObject.transform.parent.gameObject,
                out CanvasGroup nCg) ? nCg : null;
        warningCg =
            DevTool.Get_ComponentTType(warningTxt.gameObject.transform.parent.gameObject,
                out CanvasGroup wCg) ? wCg : null;

        noticeCg.gameObject.SetActive(false);
        warningCg.gameObject.SetActive(false);

        Reset_ForgePanel();

#if UNITY_EDITOR
        sw.Stop();
        UnityEngine.Debug.Log($"<color=yellow>Extra (Forge)</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
#endif
        yield return null;


#if UNITY_EDITOR
        sw.Restart();
#endif
        SetColor(mainClr, subClr);

#if UNITY_EDITOR
        sw.Stop();
        UnityEngine.Debug.Log($"<color=yellow>Visual</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
#endif
        yield return null;
    }

    public void SetColor(Color mainClr, Color subClr)
    {
        DevTool.SetColorImgs(subClr, subImgs);

        mainColorCompList = new List<Component>();
        subColorCompList = new List<Component>();

        // Equiped
        subColorCompList.AddRange(equipPanelInnerList);
        for (int i = 0; i < equipDescStateTxtList.Count; i++)
        {
            mainColorCompList.Add(equipDescStateTxtList[i]);
        }

        // Synergy
        mainColorCompList.AddRange(amalgamationDescTxtList);
        subColorCompList.Add(selectViewAmalgamation);
        for (int i = 0; i < synergySlots.Count; i++)
        {
            mainColorCompList.Add(synergySlots[i].tierImg);
            subColorCompList.Add(synergySlots[i].txt);
        }
        mainColorCompList.AddRange(DevTool.Get_ChildList<Image>(synergyDescLinerParentTf));
        for (int i = 0; i < synergyDescTextParentTf.childCount; i++)
        {
            if (synergyDescTextParentTf.GetChild(i).TryGetComponent(out TMP_Text Txt) &&
                !amalgamationDescTxtList.Contains(Txt))
            {
                subColorCompList.Add(Txt);
            }
        }

        // Equip Toggle Btn
        mainColorCompList.Add(toggleBtn_InEquip.transform.GetChild(0).GetComponent<TMP_Text>());

        // Forge Interact Panel Inner
        for (int i = 0; i < forgeElementViews.Count; i++)
        {
            mainColorCompList.Add(forgeElementViews[i].panelBtnTxt);

            mainColorCompList.Add(forgeElementViews[i].roleBtnTxt);
            mainColorCompList.Add(forgeElementViews[i].roleDescTxt);

            subColorCompList.AddRange(forgeElementViews[i].innerImgs);
        }

        // Desc
        mainColorCompList.AddRange(descPanel.Get_MainColorList());
        subColorCompList.AddRange(descPanel.Get_SubColorList());

        // Item
        decompositionView.SetColor(mainClr);
        fusionView.SetColor(mainClr);
        makeView.SetColor(mainClr);

        DevTool.Set_Color(mainClr, mainColorCompList);
        mainColorCompList.Clear();
        mainColorCompList = null;

        DevTool.Set_Color(subClr, subColorCompList);
        subColorCompList.Clear();
        subColorCompList = null;
    }

    public void SetModuleShard(int value)
    {
        msTxt.text = value.ToString();
    }

    public void SetEmptyBetteryUI(int value)
    {
        bcTxt.text = value.ToString();
    }

    public void SetChargedBetteryUI(int value)
    {
        ecTxt.text = value.ToString();
    }

    #region Reset

    // 장착 패널 리셋
    private void Reset_EquipPanel()
    {
        equippedPanelGo.gameObject.SetActive(true);
        synergyPanelGo.gameObject.SetActive(false);
    }


    private void Reset_ForgePanel()
    {
        currentForgeElementView = null;
        if (forgeElementViews == null)
            return;

        for (int i = 0; i < forgeElementViews.Count; i++)
        {
            forgeElementViews[i].panelRT.gameObject.SetActive(false);

            if (DevTool.Get_ComponentTType(forgeElementViews[i].panelBtn.gameObject, out CanvasGroup btnCg))
            {
                btnCg.alpha = forgeElementBtnOffAlpha;
            }
        }

        Reset_ForgeElementPanel();
    }

    // 강화 패널 리셋
    public void Reset_ForgeElementPanel()
    {
        ModuleItemManager.instance.Set_UnDecompositionSlot();
        ModuleItemManager.instance.Set_UnFusionSlotAll();

        inventory_InForge.SetOff_AllInventoryForgeSelectedUI();

        decompositionView.ResetPanel();
        fusionView.ResetPanel();
        makeView.ResetPanel();

        SetOff_Anno();
    }

    #endregion

    #region Mono

    protected override void OnEnable()
    {
        base.OnEnable();

        Reset_EquipPanel();
        Reset_ForgePanel();
    }

    private void LateUpdate()
    {
        Caculate_Drag();
    }

    #endregion

    #region Set (Panel)

    public override void SetOnThisPanel()
    {
        Tween_Enable();

        base.SetOnThisPanel();

        // Dur
        durEui.Set_Dur(ModuleUpgradeController.usingShop.currentDur);
    }

    public override void SetOff_ThisPanel()
    {
        if (Is_Interact_Msg()) return;

        base.SetOff_ThisPanel();

        Tween_Disable();
        SetOff_Desc();

        if (isDragging)
        {
            dragItemRt.gameObject.SetActive(false);
            isDragging = false;
            currentDraggingItemBtn = null;
        }

        ModuleUpgradeController.usingShop = null;
    }

    public override void Change_ThisPanel(int indexWindow)
    {
        // 인벤토리의 Scroll 벨류를 그대로 가져감
        float scrollValue = currentThisPanelTab.tabScrollbar.value;

        base.Change_ThisPanel(indexWindow);

        currentThisPanelTab.tabScrollbar.value = scrollValue;
    }

    #endregion

    #region Tween

    private void Tween_Enable()
    {
        for (int i = 0; i < forgeElementViews.Count; i++)
        {
            if (forgeElementViews[i].roleBtnTxtRT == null) break;

            forgeElementViews[i].rtTween.Play();
        }
    }

    private void Tween_Disable()
    {
        for (int i = 0; i < forgeElementViews.Count; i++)
        {
            if (forgeElementViews[i].roleBtnTxtRT == null) break;

            forgeElementViews[i].rtTween.Pause();
        }
    }

    #endregion

    #region Synergy

    public void Set_SynergySlots(Dictionary<int, int> dict)
    {
        if (dict.Count <= 0)
        {
            // 패널 키기/끄기
            SetOnOff_SynergySlot(false);
        }
        else
        {
            // 패널 키기/끄기
            SetOnOff_SynergySlot(true);

            // 모두 끄기
            for (int i = 0; i < synergySlots.Count; i++)
                synergySlots[i].SetOff_SynergySlot();

            // 가지고 있는 시너지 부분을 추가
            int currentSynergies = 0;
            foreach (KeyValuePair<int, int> keyValuePair in dict)
            {
                MainChipData MDC = ModuleItemManager.instance.Get_CorrectMainChip(keyValuePair.Key);
                synergySlots[currentSynergies].SetOn_SynergySlot(keyValuePair.Key, MDC.thisIcon, keyValuePair.Value);

                currentSynergies++;
            }
        }
    }

    private void SetOnOff_SynergySlot(bool exist)
    {
        // 패널 키기/끄기
        synergyPanelIsExistGo.typeBase.SetActive(!exist);
        synergyPanelIsExistGo.typeSpecial.SetActive(exist);
    }

    #endregion

    #region Set

    #region Item UI

    // 인벤토리 UI 셋
    public void SetInventoryUI(ModuleState[][] allModuleState)
    {
        inventory_InEquip.Set_InventoryUI(allModuleState);
        inventory_InForge.Set_InventoryUI(allModuleState);
    }

    // 장착 슬롯 UI 셋
    public void SetEquipedUI(ModuleState[][] allModuleState, CoupleData<int>[] equipedData)
    {
        for (int i = 0; i < equipedData.Length; i++)
        {
            int targetCol = equipedData[i].typeBase;
            int targetRow = equipedData[i].typeSpecial;

            if (targetCol == -1 || targetRow == -1)
            {
                equipedSlots[i].item.gameObject.SetActive(false);

                MainGameUIManager.instance.hud.TabModuleView.moduleSlots[i].item.gameObject.SetActive(false);

                Set_EquipedDesc(i);
            }
            else
            {
                ItemData data = allModuleState[targetCol][targetRow].thisItemData;

                equipedSlots[i].item.gameObject.SetActive(true);
                equipedSlots[i].item.Set_Data(new ItemData_UIVisual(data));

                MainGameUIManager.instance.hud.TabModuleView.moduleSlots[i].item.gameObject.SetActive(true);
                MainGameUIManager.instance.hud.TabModuleView.moduleSlots[i].item.Set_Data(new ItemData_UIVisual(data));

                Set_EquipedDesc(i, data);
            }
        }

        inventory_InEquip.Set_InventoryEquipedUI(equipedData);
        inventory_InForge.Set_InventoryEquipedUI(equipedData);
    }

    // 장착된 모듈들의 설명 키기/끄기
    private void Set_EquipedDesc(int index, ItemData itemData = null)
    {
        equipDescStateTxtList[index].text = itemData != null ? itemData.equipDesc : "-";
    }

    // 분해 슬롯 UI 셋
    public void Set_DecompositionUI(InventoryItemEUIController itemEui, CoupleData<int> applyIndex)
    {
        int col = itemEui.slot.col;
        int row = itemEui.slot.row;

        bool setActive;
        string gainBC;
        string gainMS;

        if (col != -1 && row != -1) // 만약 슬롯에 등록하는 것이라면
        {
            ModuleState moduleState = ModuleItemManager.instance.Get_ModuleState(itemEui);

            setActive = true;
            gainBC = ModuleItemManager.Get_BC_ByDescomposition(moduleState).ToString();
            gainMS = ModuleItemManager.Get_MS_ByDecomposition(moduleState).ToString();

            decompositionView.SetItemData(itemEui);
        }
        else // 슬롯에서 빼는 것이라면
        {
            setActive = false;
            gainBC = "-";
            gainMS = "-";
        }

        decompositionView.Set_DecompositionUI(setActive, gainBC, gainMS);

        inventory_InForge.Set_InventoryForgeSelectedUI(applyIndex, setActive);

        Check_DecompositionAnno();
    }

    // 합성 슬롯 UI 셋
    public void Set_FusionUI(ModuleState[][] allModuleState, CoupleData<int>[] slottedData)
    {
        inventory_InForge.SetOff_AllInventoryForgeSelectedUI();

        string needMS = "-";

        for (int i = 0; i < slottedData.Length; i++)
        {
            int targetCol = slottedData[i].typeBase;
            int targetRow = slottedData[i].typeSpecial;

            if (targetCol == -1 || targetRow == -1)
            {
                fusionView.fusionSlotList[i].item.gameObject.SetActive(false);
            }
            else
            {
                ItemData data = allModuleState[targetCol][targetRow].thisItemData;

                fusionView.fusionSlotList[i].item.gameObject.SetActive(true);
                fusionView.fusionSlotList[i].item.Set_Data(new ItemData_UIVisual(data));

                inventory_InForge.Set_InventoryForgeSelectedUI(new CoupleData<int>(targetCol, targetRow), true, i);
            }
        }

        if (!ModuleItemManager.instance.Is_EmptyFusionSlot() &&
            ModuleItemManager.instance.Is_SameRankFusionSlots())
        {
            ModuleState ms = ModuleItemManager.instance.Get_ModuleState(slottedData[0].typeBase, slottedData[0].typeSpecial);
            if (ms.thisItemData.rank < PlayerController.maxRank)
                needMS = ModuleItemManager.Get_MS_ForFusion(ms).ToString();
        }

        fusionView.preview_NeedMs_ForFusion.text = needMS;

        Check_FusionAnno();
    }

    #endregion

    #region Forge Anno

    // 분해 경고
    private void Check_DecompositionAnno()
    {
        CoupleData<int> index = ModuleItemManager.instance.Get_DecompositionIndex();
        if (index.typeBase == -1 || index.typeSpecial == -1)
        {
            SetOff_Anno();
            return;
        }
        else if (ModuleItemManager.instance.Is_IncludeOnlyEquipped(index)) // 장비하고 있나?
        {
            Set_Notice(true, notice_Equiped);
            return;
        }

        SetOff_Anno();
    }

    // 조립 경고
    private void Check_MakeAnno()
    {
        CoupleData<int> index = ModuleItemManager.instance.Get_EmptyModuleState();
        if (index.typeBase == -1 || index.typeSpecial == -1)
        {
            Set_Warning(true, warning_InvenFull);
            return;
        }
        else if (PlayerManager.instance.playerController.moduleShard < ModuleItemManager.Get_MS_ForMake() ||
            !PlayerManager.instance.playerController.IsEnoughChargedBettery(ModuleItemManager.Get_CB_ForMake()))
        {
            Set_Warning(true, warning_NotEnoughItem);
            return;
        }

        SetOff_Anno();
        return;
    }

    // 합성 경고
    private void Check_FusionAnno()
    {
        List<CoupleData<int>> index = ModuleItemManager.instance.Get_FusionIndex();

        for (int i = 0; i < index.Count; i++)
        {
            if (index[i].typeBase == -1 || index[i].typeSpecial == -1)
            {
                SetOff_Anno();
                return;
            }
        }
        if (!ModuleItemManager.instance.Is_SameRankFusionSlots()) // 랭크가 다른가?
        {
            Set_Warning(true, warning_NotSameRank);
            return;
        }

        for (int i = 0; i < index.Count; i++) // 이미 최대치인가?
        {
            if (ModuleItemManager.instance.Get_ModuleState(index[i]).thisItemData.rank >= PlayerController.maxRank)
            {
                Set_Warning(true, warning_AlreadyMaxLv);
                return;
            }
        }

        if (ModuleItemManager.Get_MS_ForFusion(
            ModuleItemManager.instance.Get_ModuleState(index[0]))
                > PlayerManager.instance.playerController.moduleShard) // MS가 부족한가?
        {
            Set_Warning(true, warning_NotEnoughItem);
            return;
        }

        for (int i = 0; i < index.Count; i++)
        {
            if (ModuleItemManager.instance.Is_IncludeOnlyEquipped(index[i]))
            {
                Set_Notice(true, notice_Equiped);
                return;
            }
        }

        SetOff_Anno();
    }

    // Notice
    private void Set_Notice(bool isOn, string anno = "")
    {
        if (noticeCg == null) return;

        noticeCg.gameObject.SetActive(isOn);
        if (isOn) noticeTxt.text = anno;
    }

    // Warning
    private void Set_Warning(bool isOn, string anno = "")
    {
        if (warningCg == null) return;

        warningCg.gameObject.SetActive(isOn);
        if (isOn) warningTxt.text = anno;
    }

    private void SetOff_Anno()
    {
        Set_Notice(false);
        Set_Warning(false);
    }

    #endregion

    #endregion

    #region Tab

    // 탭
    private bool Is_Interact_TabPanel()
    {
        for (int i = 0; i < panelTabList.Count; i++)
        {
            if (panelTabList[i].tabBtn == currentBtn)
            {
                Change_ThisPanel(i);
                return true;
            }
        }
        return false;
    }

    // 강화 탭
    private bool Is_Interact_ForgeTabPanel()
    {
        List<OwnBtnEUIController> btns = new List<OwnBtnEUIController>();
        for (int i = 0; i < forgeElementViews.Count; i++)
            btns.Add(forgeElementViews[i].panelBtn);

        if (btns.Contains(currentBtn))
        {
            for (int i = 0; i < forgeElementViews.Count; i++)
            {
                if (btns[i] == currentBtn)
                {
                    // 다른 탭일 경우 초기화
                    if (forgeElementViews[i] != currentForgeElementView)
                        Reset_ForgeElementPanel();

                    // 생성이라면
                    if (i == 2)
                    {
                        Check_MakeAnno();
                    }

                    forgeElementViews[i].panelRT.gameObject.SetActive(true);
                    forgeElementViews[i].PanelBtnCG.alpha = 1f;

                    currentForgeElementView = forgeElementViews[i];
                }
                else
                {
                    forgeElementViews[i].panelRT.gameObject.SetActive(false);
                    forgeElementViews[i].PanelBtnCG.alpha = 0.5f;
                }
            }

            SoundManager.instance.PlayUiSfx("Click01");
            return true;
        }
        return false;
    }

    // 시너지 변경 탭
    private bool Is_Interact_SynergyTabPanel()
    {
        if (currentBtn == toggleBtn_InEquip)
        {
            if (equippedPanelGo.activeSelf)
            {
                equippedPanelGo.SetActive(false);
                synergyPanelGo.SetActive(true);
                synergyDescsParentTf.gameObject.SetActive(false);
            }
            else
            {
                equippedPanelGo.SetActive(true);
                synergyPanelGo.SetActive(false);
            }

            SoundManager.instance.PlayUiSfx("Click01");

            return true;
        }
        return false;
    }

    // 시너지 아이템
    private bool Is_Interact_SynergyItem()
    {
        if (currentBtn is SynergySlotEUIController synergySlot && synergySlots.Contains(synergySlot))
        {
            SoundManager.instance.PlayUiSfx("Click01");

            synergyDescsParentTf.gameObject.SetActive(true);
            selectedSynergySlot = synergySlot;

            MainChipData MCD = ModuleItemManager.instance.Get_CorrectMainChip(selectedSynergySlot.id);

            // 기본 정보
            selectViewImg.sprite = selectedSynergySlot.img.sprite;
            selectViewAmalgamation.text = selectedSynergySlot.txt.text;
            selectViewName.text = MCD.name.ToString();

            // 적용 중인 시너지 싱크로니 레벨 Txt
            int synchoronyAmount = ModuleItemManager.instance.Get_SynchronyAmount(synergySlot.id);
            int synchoronyLvLimit = 0;

            for (int i = ModuleItemManager.synchoronyMaxLv; i > 0; i--)
            {
                if ((ModuleItemManager.synchoronyOneTierRange * i) <= synchoronyAmount)
                {
                    synchoronyLvLimit = i;
                    break;
                }
            }

            for (int i = 0; i < amalgamationDescTxtList.Count; i++)
            {
                amalgamationDescTxtList[i].text = MCD.amalgamationDescArr[i];

                if (i < synchoronyLvLimit)
                    DevTool.Set_AlphaColor(amalgamationDescTxtList[i], 1f);
                else
                    DevTool.Set_AlphaColor(amalgamationDescTxtList[i], 0.3f);
            }
            return true;
        }
        return false;
    }

    #endregion

    #region Input

    #region Try Interact

    public void Try_Interact()
    {
        if (isDragging) return;

        InputManager.instance.Play_MousePointerClick();

        if (Is_Interact_Msg()) return;

        if (currentBtn == null || ModuleUpgradeController.usingShop == null) return;

        if (Is_Interact_TabPanel()) return;
        if (Is_Interact_ForgeTabPanel()) return;
        if (Is_Interact_SynergyTabPanel()) return;
        if (Is_Interact_SynergyItem()) return;
        if (Is_Interact_RoleBtn()) return;
        if (Is_Interact_CloseBtn()) return;
    }

    public void Try_InteractSub()
    {
        if (isDragging) return;

        InputManager.instance.Play_MousePointerClick();

        if (msgEui.gameObject.activeSelf || ModuleUpgradeController.usingShop == null) return;

        // 아이템이면?
        if (currentItemBtn != null)
        {
            Interact_Item(currentItemBtn);
            return;
        }
    }

    public void Try_InteractDragOn()
    {
        if (Is_Interact_Msg()) return;

        if (msgEui.gameObject.activeSelf || ModuleUpgradeController.usingShop == null) return;

        if (currentItemBtn != null)
        {
            currentDraggingItemBtn = currentItemBtn;

            dragItemEui.gameObject.SetActive(true);
            dragItemEui.Set_Data(currentItemBtn);

            SoundManager.instance.PlayUiSfx("Click01");

            isDragging = true;
        }
    }

    public void Try_InteractDragOff()
    {
        if (!dragItemEui.gameObject.activeSelf) return;

        dragItemEui.gameObject.SetActive(false);

        SoundManager.instance.PlayUiSfx("Click01");

        isDragging = false;

        Set_DragInSlot();

        currentDraggingItemBtn = null;
    }

    #endregion

    #region Item

    // 모듈 아이템
    private void Interact_Item(InventoryItemEUIController itemEui)
    {
        int panelIndex = panelTabList.IndexOf(currentThisPanelTab);

        if (panelIndex == 0) // Equiped 창
        {
            if (itemEui.slot.col != -1 && itemEui.slot.row != -1) // 장착 시도
                Interact_Equipped(itemEui);
            else // 장착 해제
                Interact_UnEquipped(itemEui);
        }
        else if (panelIndex == 1) // Forge 창
        {
            int panelIndexOfForge = forgeElementViews.IndexOf(currentForgeElementView);

            if (panelIndexOfForge == 0) // 분해
            {
                if (itemEui.slot.col != -1 && itemEui.slot.row != -1) // 슬롯 장착
                    Interact_DecompositionInit(itemEui);
                else // 슬롯 해제
                    Interact_UnDecompositionInit(itemEui);
            }
            else if (panelIndexOfForge == 1)
            {
                if (itemEui.slot.col != -1 && itemEui.slot.row != -1) // 슬롯 장착
                    Interact_FusionInit(itemEui);
                else // 슬롯 해제
                    Interact_UnFusionInit(itemEui);
            }
        }

    }

    #region Equip

    // 장착
    private void Interact_Equipped(InventoryItemEUIController itemEui, int equipSlotIndex = -1)
    {
        if (equipSlotIndex == -1) // 빈 공간을 찾아 장착
        {
            equipSlotIndex = ModuleItemManager.instance.Get_EmptyEquippedIndex(new CoupleData<int>(itemEui.slot.col, itemEui.slot.row));
            if (equipSlotIndex == -1) return;
        }
        else if (ModuleItemManager.instance.Is_IncludeEquipped(new CoupleData<int>(itemEui.slot.col, itemEui.slot.row), out int _ListIndex)) // 특정 위치에 이미 있다면, 제거
        {
            ModuleItemManager.instance.Set_UnEquip(_ListIndex);
        }
        // 삽입
        ModuleItemManager.instance.Set_Equip(equipSlotIndex, new CoupleData<int>(itemEui.slot.col, itemEui.slot.row));

        // 사운드
        SoundManager.instance.PlayUiSfx("Equip");
    }

    // 장착 해제
    private void Interact_UnEquipped(InventoryItemEUIController itemEui)
    {
        int index = equipedSlots.IndexOf(itemEui.slot);

        ModuleItemManager.instance.Set_UnEquip(index);

        // 사운드
        SoundManager.instance.PlayUiSfx("Unequip");

        currentItemBtn = null;
        currentBtn = null;

        SetOff_Desc();
    }

    // 장착 스위칭
    private void Interact_EquippedSwitch(InventoryItemEUIController itemEui0, InventoryItemEUIController itemEui1)
    {
        int index0 = equipedSlots.IndexOf(itemEui0.slot);
        int index1 = equipedSlots.IndexOf(itemEui1.slot);
        ModuleItemManager.instance.Set_SwitchEquipment(index0, index1);
    }

    #endregion

    #region Descomposition

    // 분해 장착
    private void Interact_DecompositionInit(InventoryItemEUIController itemEui)
    {        
        // 이미 존재한다면
        if (!ModuleItemManager.instance.Is_EmptyDecompositionSlot())
            Interact_UnDecompositionInit(decompositionView.thisSlot.item);

        ModuleItemManager.instance.Set_DecompositionSlot(new CoupleData<int>(itemEui.slot.col, itemEui.slot.row));

        Set_DecompositionUI(itemEui, new CoupleData<int>(itemEui.slot.col, itemEui.slot.row));
    }

    // 분해 해제
    private void Interact_UnDecompositionInit(InventoryItemEUIController itemEui)
    {
        if (!ModuleItemManager.instance.Is_EmptyDecompositionSlot())
        {
            // 미리 값을 저장해야함 => 삭제할 것임
            CoupleData<int> applyIndex = ModuleItemManager.instance.Get_DecompositionIndex();

            ModuleItemManager.instance.Set_UnDecompositionSlot();

            Set_DecompositionUI(itemEui, applyIndex);

            currentItemBtn = null;
        }
    }

    #endregion

    #region Fusion

    // 합성 장착
    private void Interact_FusionInit(InventoryItemEUIController itemEui, int slotIndex = -1)
    {
        if (slotIndex == -1)
        {
            if (ModuleItemManager.instance.Is_EmptyFusionSlot(out int index) &&
                !ModuleItemManager.instance.Is_IncludeFusionSlots(new CoupleData<int>(itemEui.slot.col, itemEui.slot.row)))
            {
                ModuleItemManager.instance.Set_FusionSlot(index, new CoupleData<int>(itemEui.slot.col, itemEui.slot.row));
            }
        }
        else
        {
            // 이미 슬롯에 있다면, 제거
            if (ModuleItemManager.instance.Is_IncludeFusionSlots(new CoupleData<int>(itemEui.slot.col, itemEui.slot.row), out int listIndex))
                Interact_UnFusionInit(fusionView.fusionSlotList[listIndex].item);

            // 해당 인덱스 슬롯에 비어있지 않다면, 제거
            if (!ModuleItemManager.instance.Is_EmptyFusionSlot(slotIndex))
                Interact_UnFusionInit(fusionView.fusionSlotList[slotIndex].item);

            ModuleItemManager.instance.Set_FusionSlot(slotIndex, new CoupleData<int>(itemEui.slot.col, itemEui.slot.row));
        }
    }

    // 합성 해제
    private void Interact_UnFusionInit(InventoryItemEUIController itemEui)
    {
        if (!ModuleItemManager.instance.Is_AllEmptyFusionSlot())
        {
            List<InventoryItemEUIController> itemEUIList = new List<InventoryItemEUIController>();
            for (int i = 0; i < fusionView.fusionSlotList.Count; i++)
                itemEUIList.Add(fusionView.fusionSlotList[i].item);

            int index = itemEUIList.IndexOf(itemEui);

            CoupleData<int> applyIndex = ModuleItemManager.instance.Get_FusionIndex()[index];

            ModuleItemManager.instance.Set_UnFusionSlot(index);

            currentItemBtn = null;
        }
    }

    // 합성 스위칭
    private void Interact_FusionSwitch(InventoryItemEUIController itemEui0, InventoryItemEUIController itemEui1)
    {
        int index0 = fusionView.fusionSlotList.IndexOf(itemEui0.slot);
        int index1 = fusionView.fusionSlotList.IndexOf(itemEui1.slot);
        ModuleItemManager.instance.Set_SwitchFusion(index0, index1);
    }

    #endregion

    #endregion

    #region Role

    private bool Is_Interact_RoleBtn()
    {
        for (int i = 0; i < forgeElementViews.Count; i++)
        {
            if (forgeElementViews[i].roleBtn == currentBtn)
            {
                if (i == 0) // 분해
                    Role_Decomposition();
                else if (i == 1) // 합성
                    Role_Fusion();
                else if (i == 2) // 업글
                    Role_Make();
            }
        }
        return false;
    }

    private void Role_Decomposition()
    {
        if (ModuleItemManager.instance.Is_EmptyDecompositionSlot() ||
            warningCg.gameObject.activeSelf ||
            ModuleUpgradeController.usingShop.currentDur <= 0) return;

        CoupleData<int> index = ModuleItemManager.instance.Get_DecompositionIndex();

        // 보상 획득
        PlayerManager.instance.playerController.GainEmptyBettery(
            ModuleItemManager.Get_BC_ByDescomposition(ModuleItemManager.instance.Get_ModuleState(index)));

        PlayerManager.instance.playerController.GainModuleShard(
            ModuleItemManager.Get_MS_ByDecomposition(ModuleItemManager.instance.Get_ModuleState(index)));

        ModuleUpgradeController.usingShop.Take_Damage(spawnItem: false, soundOn: false);


        // 모듈 아이템 제거
        ModuleItemManager.instance.Remove_ModuleState(index);

        // 사운드
        SoundManager.instance.PlayUiSfx("Decomposition");

        // 기타 UI와 정보 초기화
        Reset_ForgeElementPanel();
    }

    private void Role_Fusion()
    {
        if (ModuleItemManager.instance.Is_EmptyFusionSlot() ||
            warningCg.gameObject.activeSelf ||
            ModuleUpgradeController.usingShop.currentDur <= 0) return;

        List<CoupleData<int>> indexList = ModuleItemManager.instance.Get_FusionIndex();

        // 소모 재화
        PlayerManager.instance.playerController.UseModuleShard(
            ModuleItemManager.Get_MS_ForFusion(ModuleItemManager.instance.Get_ModuleState(indexList[0])));

        ModuleUpgradeController.usingShop.Take_Damage(spawnItem: false, soundOn: false);

        // 보상 획득
        ModuleItemManager.instance.Set_UpRank(indexList[0]);

        // 모듈 아이템 제거
        indexList.RemoveAt(0);
        ModuleItemManager.instance.Remove_ModuleState(indexList);

        // 사운드
        SoundManager.instance.PlayUiSfx("Fusion");

        // 기타 UI와 정보 초기화
        Reset_ForgeElementPanel();
    }

    private void Role_Make()
    {
        if (warningCg.gameObject.activeSelf ||
            ModuleUpgradeController.usingShop.currentDur <= 0) return;

        // 소모 재화
        PlayerManager.instance.playerController.UseModuleShard(ModuleItemManager.Get_MS_ForMake());
        PlayerManager.instance.playerController.UseChargedBettery(ModuleItemManager.Get_CB_ForMake());
        ModuleUpgradeController.usingShop.Take_Damage(spawnItem: false, soundOn: false);

        // 보상 획득
        ModuleItemManager.instance.Gain_ModuleState();

        // 사운드
        SoundManager.instance.PlayUiSfx("Make");

        Check_MakeAnno();
    }

    #endregion

    #endregion

    #region Desc

    public void SetOn_Desc(InventoryItemEUIController itemEui)
    {
        int col = itemEui.slot.col;
        int row = itemEui.slot.row;
        ModuleState moduleState = null;
        if (col == -1 || row == -1) // 기타
        {
            if (equipedSlots.Contains(itemEui.slot)) // 장비 창
            {
                moduleState = ModuleItemManager.instance.Get_EquippedModuleState(
                    equipedSlots.IndexOf(itemEui.slot));
            }
        }
        else // 인벤토리
        {
            moduleState = ModuleItemManager.instance.Get_ModuleState(itemEui);
        }

        descPanel.SetOn_Desc(moduleState);
    }

    public void SetOff_Desc()
    {
        descPanel.SetOff_Desc();
    }

    #endregion

    #region Drag

    protected Vector2 Get_PanelLocalPoint()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            DevTool.Get_ComponentTType<RectTransform>(gameObject), // 변환할 UI(RectTransform)
            InputManager.instance.mousePos, // 현재 마우스 좌표 (Screen Space)
            null, // Canvas의 카메라 (Render Mode 따라 null 가능)
            out Vector2 localPoint); // 변환된 Local 좌표

        return localPoint;
    }

    private void Caculate_Drag()
    {
        if (!dragItemEui.gameObject.activeSelf) return;

        dragItemRt.anchoredPosition = Get_PanelLocalPoint();
    }

    private void Set_DragInSlot()
    {
        if (currentSlotBtn != null)
        {
            CoupleData<int> originalIndex = new CoupleData<int>(currentDraggingItemBtn.slot.col, currentDraggingItemBtn.slot.row);
            CoupleData<int> targetIndex = new CoupleData<int>(currentSlotBtn.col, currentSlotBtn.row);

            // 인벤토리에서 이동 시키기
            if (originalIndex.typeBase != -1 && originalIndex.typeSpecial != -1)
                Set_DragFromInventorySlot(originalIndex, targetIndex);
            else
                Set_DragFromInteractSlot(originalIndex, targetIndex);
        }
    }

    // 인벤토리에서 =>
    private void Set_DragFromInventorySlot(CoupleData<int> originalIndex, CoupleData<int> targetIndex)
    {
        // => 인벤토리로
        if (targetIndex.typeBase != -1 && targetIndex.typeSpecial != -1)
        {
            ModuleItemManager.instance.Set_ChangeInventorySlot(originalIndex, targetIndex); // 인벤토리 내 아이템 위치 변경
        }
        else // => 상호작용 슬롯으로
        {
            // 장착 패널
            int panelIndex = panelTabList.IndexOf(currentThisPanelTab);

            if (panelIndex == 0) // Equiped 창
            {
                Interact_Equipped(currentDraggingItemBtn, equipedSlots.IndexOf(currentSlotBtn)); // 특정 위치에 장착
            }
            else if (panelIndex == 1) // Forge 창
            {
                int panelIndexOfForge = forgeElementViews.IndexOf(currentForgeElementView);

                if (panelIndexOfForge == 0) // 분해
                {
                    Interact_DecompositionInit(currentDraggingItemBtn); // 분해 슬롯에 장착
                }
                else if (panelIndexOfForge == 1) // 합성
                {
                    Interact_FusionInit(currentDraggingItemBtn, fusionView.fusionSlotList.IndexOf(currentSlotBtn)); // 퓨전 슬롯에 장착
                }
            }

        }
    }

    // 상호작용 슬롯에서 =>
    private void Set_DragFromInteractSlot(CoupleData<int> originalIndex, CoupleData<int> targetIndex)
    {
        int panelIndex = panelTabList.IndexOf(currentThisPanelTab);

        // => 인벤토리로
        if (targetIndex.typeBase != -1 && targetIndex.typeSpecial != -1)
        {
            if (panelIndex == 0) // Equiped 창
            {
                Interact_UnEquipped(currentDraggingItemBtn); // 아이템 해제
            }
            else if (panelIndex == 1) // Forge 창
            {
                int panelIndexOfForge = forgeElementViews.IndexOf(currentForgeElementView);

                if (panelIndexOfForge == 0) // 분해
                {
                    Interact_UnDecompositionInit(currentDraggingItemBtn);
                }
                else if (panelIndexOfForge == 1) // 합성
                {
                    Interact_UnFusionInit(currentDraggingItemBtn);
                }
            }
            // => 상호작용으로
            else
            {
                if (panelIndex == 0) // Equiped 창
                {
                    Interact_EquippedSwitch(currentDraggingItemBtn, currentSlotBtn.item); // 아이템 장착 위치 바꾸기
                }
                else if (panelIndex == 1) // Forge 창
                {
                    if (forgeElementViews.IndexOf(currentForgeElementView) == 1) // 합성
                    {
                        Interact_FusionSwitch(currentDraggingItemBtn, currentSlotBtn.item);
                    }
                }
            }

        }
    }

    #endregion

    #region Set (Language)

    public override void SetLanguageTxt()
    {
        var words = StaticResourceManager.instance.staticWords;
        var descs = StaticResourceManager.instance.staticDescs;

        // Label
        labelName = $"{words.GetLanguage(27)} {words.GetLanguage(2)}";
        labelTxt.text = labelName;

        // Tab
        tabBtnTxtList = new List<string>
        {
            words.GetLanguage(32),
            words.GetLanguage(33),
        };

        amalgamationName = words.GetLanguage(50);
        notice_Equiped = descs.GetLanguage(20);
        warning_NotSameRank = descs.GetLanguage(21);
        warning_NotEnoughItem = descs.GetLanguage(22);
        warning_AlreadyMaxLv = descs.GetLanguage(23);
        warning_InvenFull = descs.GetLanguage(27);

        // Desc
        descPanel.Set_LanguageTxt();

        // Forge
        forgeElementViews[0].Set_LanguageTxt(words.GetLanguage(51), descs.GetLanguage(24));
        forgeElementViews[1].Set_LanguageTxt(words.GetLanguage(52), descs.GetLanguage(25));
        forgeElementViews[2].Set_LanguageTxt(words.GetLanguage(53), descs.GetLanguage(26));

        // Amalgamation
        DevTool.Set_TxtList(amalgamationTxtList, amalgamationName);

        base.SetLanguageTxt();
    }

    #endregion
}