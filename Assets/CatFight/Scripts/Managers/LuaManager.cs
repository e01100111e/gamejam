using PixelCrushers.DialogueSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CatFight
{
    public class LuaManager : MonoBehaviour
    {
        public void Start()
        {
            Lua.RegisterFunction("StartBattle", this, SymbolExtensions.GetMethodInfo(() => StartBattle()));
            Lua.RegisterFunction("SetFightIndex", this, SymbolExtensions.GetMethodInfo(() => SetFightIndex(0)));
            Lua.RegisterFunction("OpenShopView", this, SymbolExtensions.GetMethodInfo(() => OpenShopView()));
            Lua.RegisterFunction("BackToMainMenu", this, SymbolExtensions.GetMethodInfo(() => BackToMainMenu()));


        }

        public void OpenShopView()
        {
           UIManager.Instance.OpenShopView();
        }

        private void OnDisable()
        {
            Lua.UnregisterFunction("StartBattle");
            Lua.UnregisterFunction("SetFightIndex");
            Lua.UnregisterFunction("OpenShopView");
            Lua.UnregisterFunction("BackToMainMenu");

        }

        public void BackToMainMenu()
        {
            GameManager.Instance.LoadBackToMainScene();
        }
        public void StartBattle()
        {
            BattleManager.Instance.LoadBattle();
        }

        public void SetFightIndex(double newIndex)
        {
            BattleManager.Instance.currentWaveIndex = (int)newIndex;
        }
    }
}
