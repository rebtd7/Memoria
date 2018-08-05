using System;
using System.Collections.Generic;
using FF9;
using Memoria;
using Memoria.Data;
using Memoria.Database;
using Memoria.Prime;
using Memoria.Scenes;
using Memoria.Test;
using UnityEngine;

// ReSharper disable InconsistentNaming
// ReSharper disable NotAccessedField.Global

public partial class BattleHUD : UIScene {

    public UIRoot uiRoot;
    public UIRect uiRect;
    public Vector4 attVec;
    public Vector4 sk1Vec;
    public Vector4 sk2Vec;
    public Vector4 itmVec;

    public void SetScaleOfUI () {
         attVec = new Vector4 (-175f, 112.5f, 345f, 112.5f);
         sk1Vec = new Vector4 (-175f, 0f, 345f, 112.5f);
         sk2Vec = new Vector4 (-175f, -112.5f, 345f, 112.5f);
         itmVec = new Vector4 (-175f, -225f, 345f, 112.5f);
         uiRoot =  _abilityScrollList.gameObject.transform.root.GetComponent<UIRoot> ();
         uiRect =  uiRoot.GetComponent<UIRect> ();
         uiRoot.scalingStyle = UIRoot.Scaling.Flexible;
         uiRoot.minimumHeight = Mathf.RoundToInt ((float) Screen.currentResolution.height * 1.55f);
         uiRect.SetRect (-UIManager.UIActualScreenSize.x / 1.55f, -UIManager.UIActualScreenSize.y / 1.55f, UIManager.UIActualScreenSize.x * 3f, UIManager.UIActualScreenSize.y * 3f);
    }

    public void SetCommandPSX () {
        UIWidget[] componentsInChildren =  CommandPanel.GetComponentsInChildren<UIWidget> ();
        if (UIManager.UIActualScreenSize.x / UIManager.UIActualScreenSize.y > 1.6f) {
            componentsInChildren[0].SetRect (-UIManager.UIActualScreenSize.x * 0.5f + CommandPanel.GetComponent<UIWidget>().width, -UIManager.UIActualScreenSize.y * 0.5f + CommandPanel.GetComponent<UIWidget>().height * 0.125f, 350f, 450f);
        }
        if (UIManager.UIActualScreenSize.x / UIManager.UIActualScreenSize.y < 1.6f) {
            componentsInChildren[0].SetRect (-UIManager.UIActualScreenSize.x * 0.5f + CommandPanel.GetComponent<UIWidget>().width * 0.25f, -UIManager.UIActualScreenSize.y * 0.5f + CommandPanel.GetComponent<UIWidget>().height * 0.25f, 350f, 450f);
        }
        if ((componentsInChildren[0].isVisible && UIManager.Input.GetKey(Control.Left)) || UIManager.Input.GetKey(Control.Right)) {
            if (componentsInChildren[0].isVisible && UIManager.Input.GetKey(Control.Left)) {
                 _commandPanel.Defend.Button.gameObject.SetActive (true);
            }
            if (componentsInChildren[0].isVisible && UIManager.Input.GetKey(Control.Right)) {
                 _commandPanel.Change.Button.gameObject.SetActive (true);
            }
             _commandPanel.Attack.Button.gameObject.SetActive (false);
             _commandPanel.Defend.Button.GetComponent<UIWidget> ().SetRect ( attVec.x,  attVec.y,  attVec.z,  attVec.w);
             _commandPanel.Change.Button.GetComponent<UIWidget> ().SetRect ( attVec.x,  attVec.y,  attVec.z,  attVec.w);
            if (componentsInChildren[0].isVisible && UIManager.Input.GetKey(Control.Left)) {
                ButtonGroupState.ActiveButton =  _commandPanel.GetCommandButton (BattleHUD.CommandMenu.Defend);
            }
            if (componentsInChildren[0].isVisible && UIManager.Input.GetKey(Control.Right)) {
                ButtonGroupState.ActiveButton =  _commandPanel.GetCommandButton (BattleHUD.CommandMenu.Change);
            }
            ButtonGroupState.SetPointerOffsetToGroup (new Vector2 (10f, 0f), "Battle.Command");
        } else {
             _commandPanel.Attack.Button.gameObject.SetActive (true);
            ButtonGroupState.SetPointerOffsetToGroup (new Vector2 (10f, 0f), "Battle.Command");
             _commandPanel.Change.Button.GetComponent<UIButton> ().gameObject.SetActive (false);
             _commandPanel.Defend.Button.GetComponent<UIButton> ().gameObject.SetActive (false);
             _commandPanel.Attack.Button.GetComponent<UIWidget> ().SetRect ( attVec.x,  attVec.y,  attVec.z,  attVec.w);
             _commandPanel.Skill1.Button.GetComponent<UIWidget> ().SetRect ( sk1Vec.x,  sk1Vec.y,  sk1Vec.z,  sk1Vec.w);
             _commandPanel.Skill2.Button.GetComponent<UIWidget> ().SetRect ( sk2Vec.x,  sk2Vec.y,  sk2Vec.z,  sk2Vec.w);
            _commandPanel.Item.Button.GetComponent<UIWidget> ().SetRect (itmVec.x, itmVec.y,  itmVec.z,  itmVec.w);
            _commandPanel.Attack.Button.GetComponent<UIWidget> ().ResetAndUpdateAnchors ();
            _commandPanel.Defend.Button.GetComponent<UIWidget> ().ResetAndUpdateAnchors ();
            _commandPanel.Skill1.Button.GetComponent<UIWidget> ().ResetAndUpdateAnchors ();
            _commandPanel.Skill2.Button.GetComponent<UIWidget> ().ResetAndUpdateAnchors ();
            _commandPanel.Change.Button.GetComponent<UIWidget> ().ResetAndUpdateAnchors ();
            _commandPanel.Item.Button.GetComponent<UIWidget> ().ResetAndUpdateAnchors ();
        }
    }

    private void MoveUIObjectsForScale () {
        /*if (UIManager.UIActualScreenSize.x / UIManager.UIActualScreenSize.y < 1.6f) {
             AbilityPanel.GetChild (0).GetComponent<UIWidget> ().SetRect (-UIManager.UIActualScreenSize.x * 0.5f + (float)  AbilityPanel.GetComponent<UIWidget> ().width * 0.52f, -UIManager.UIActualScreenSize.y * 0.5f + (float)  AbilityPanel.GetComponent<UIWidget> ().height * 0.68f, 650f, 500f);
             AbilityPanel.GetComponent<UIWidget> ().ResetAndUpdateAnchors ();
             ItemPanel.GetChild (0).GetComponent<UIWidget> ().SetRect (-UIManager.UIActualScreenSize.x * 0.5f + (float)  AbilityPanel.GetComponent<UIWidget> ().width * 0.52f, -UIManager.UIActualScreenSize.y * 0.5f + (float)  AbilityPanel.GetComponent<UIWidget> ().height * 0.68f, 650f, 500f);
             ItemPanel.GetComponent<UIWidget> ().ResetAndUpdateAnchors ();
             TargetPanel.transform.localPosition = new Vector3 (-UIManager.UIActualScreenSize.x * 0.5f + (float)  _targetPanel.Widget.width * 0.52f, -UIManager.UIActualScreenSize.y * 0.5f + (float)  _targetPanel.Widget.height * 0.78f);
             _statusPanel.HP.Transform.localPosition = new Vector3 (UIManager.UIActualScreenSize.x * 0.5f - (float)  _statusPanel.HP.Widget.width * 0.55f, -UIManager.UIActualScreenSize.y * 0.5f + (float)  _statusPanel.HP.Widget.height * 0.78f);
             _statusPanel.MP.Transform.localPosition = new Vector3 (UIManager.UIActualScreenSize.x * 0.5f - (float)  _statusPanel.MP.Widget.width * 0.55f, -UIManager.UIActualScreenSize.y * 0.5f + (float)  _statusPanel.MP.Widget.height * 0.78f);
             _statusPanel.GoodStatus.Transform.localPosition = new Vector3 (UIManager.UIActualScreenSize.x * 0.5f - (float)  _statusPanel.GoodStatus.Widget.width * 0.55f, -UIManager.UIActualScreenSize.y * 0.5f + (float)  _statusPanel.GoodStatus.Widget.height * 0.78f);
             _statusPanel.BadStatus.Transform.localPosition = new Vector3 (UIManager.UIActualScreenSize.x * 0.5f - (float)  _statusPanel.BadStatus.Widget.width * 0.55f, -UIManager.UIActualScreenSize.y * 0.5f + (float)  _statusPanel.BadStatus.Widget.height * 0.78f);
             _partyDetail.Transform.localPosition = new Vector3 (UIManager.UIActualScreenSize.x * 0.5f - (float)  _partyDetail.Widget.width * 0.52f, -UIManager.UIActualScreenSize.y * 0.5f + (float)  PartyDetailPanel.GetComponent<UIWidget> ().height * 0.78f);
             _partyDetail.Widget.panel.RebuildAllDrawCalls ();
             BattleDialogGameObject.gameObject.transform.localPosition = new Vector3 (0f, UIManager.UIActualScreenSize.y * 0.5f - 100f);
             BattleDialogGameObject.gameObject.transform.localScale = new Vector3 (1.1f, 1.1f);
            ButtonGroupState.SetPointerOffsetToGroup (new Vector2 (34f, 0f), "Battle.Ability");
            ButtonGroupState.SetPointerOffsetToGroup (new Vector2 (34f, 0f), "Battle.Item");
            ButtonGroupState.SetPointerOffsetToGroup (new Vector2 (16f, 0f), "Battle.Target");
            ButtonGroupState.SetPointerOffsetToGroup (new Vector2 (10f, 0f), "Battle.Command");
            ButtonGroupState.SetPointerLimitRectToGroup ( AbilityPanel.GetComponent<UIWidget> (),  _abilityScrollList.cellHeight, "Battle.Ability");
            ButtonGroupState.SetPointerLimitRectToGroup ( ItemPanel.GetComponent<UIWidget> (),  _itemScrollList.cellHeight, "Battle.Item");
            return;
        }*/
        AbilityPanel.gameObject.transform.AddX(-500f);
        AbilityPanel.GetComponent<UIWidget> ().ResetAndUpdateAnchors ();
        ItemPanel.gameObject.transform.AddX(-500f);
        ItemPanel.GetComponent<UIWidget> ().ResetAndUpdateAnchors ();
        TargetPanel.transform.localPosition = new Vector3 (-UIManager.UIActualScreenSize.x * 0.5f + _targetPanel.Widget.width * 0.8f, -UIManager.UIActualScreenSize.y * 0.5f + _targetPanel.Widget.height / 1.75f);
        _statusPanel.HP.Transform.localPosition = new Vector3 (UIManager.UIActualScreenSize.x * 0.5f - _statusPanel.HP.Widget.width * 1f, -UIManager.UIActualScreenSize.y * 0.5f + _statusPanel.HP.Widget.height / 1.75f);
        _statusPanel.MP.Transform.localPosition = new Vector3 (UIManager.UIActualScreenSize.x * 0.5f - _statusPanel.MP.Widget.width * 1f, -UIManager.UIActualScreenSize.y * 0.5f + _statusPanel.MP.Widget.height / 1.75f);
        _statusPanel.GoodStatus.Transform.localPosition = new Vector3 (UIManager.UIActualScreenSize.x * 0.5f - _statusPanel.GoodStatus.Widget.width * 1f, -UIManager.UIActualScreenSize.y * 0.5f + _statusPanel.GoodStatus.Widget.height / 1.75f);
        _statusPanel.BadStatus.Transform.localPosition = new Vector3 (UIManager.UIActualScreenSize.x * 0.5f - _statusPanel.BadStatus.Widget.width * 1f, -UIManager.UIActualScreenSize.y * 0.5f + _statusPanel.BadStatus.Widget.height / 1.75f);
        _partyDetail.Transform.localPosition = new Vector3 (UIManager.UIActualScreenSize.x * 0.5f - _partyDetail.Widget.width * 0.85f, -UIManager.UIActualScreenSize.y * 0.5f + _partyDetail.Widget.height / 1.75f);
        _partyDetail.Widget.panel.RebuildAllDrawCalls ();
        BattleDialogGameObject.gameObject.transform.localPosition = new Vector3 (0f, UIManager.UIActualScreenSize.y * 0.5f - 200f);
        BattleDialogGameObject.gameObject.transform.localScale = new Vector3 (1.1f, 1.1f);
        ButtonGroupState.SetPointerOffsetToGroup (new Vector2 (34f, 0f), "Battle.Ability");
        ButtonGroupState.SetPointerOffsetToGroup (new Vector2 (34f, 0f), "Battle.Item");
        ButtonGroupState.SetPointerOffsetToGroup (new Vector2 (16f, 0f), "Battle.Target");
        ButtonGroupState.SetPointerOffsetToGroup (new Vector2 (10f, 0f), "Battle.Command");
        ButtonGroupState.SetPointerLimitRectToGroup (AbilityPanel.GetComponent<UIWidget> (), _abilityScrollList.cellHeight, "Battle.Ability");
        ButtonGroupState.SetPointerLimitRectToGroup (ItemPanel.GetComponent<UIWidget> (), _itemScrollList.cellHeight, "Battle.Item");
    }

}