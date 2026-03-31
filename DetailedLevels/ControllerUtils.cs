using System.Reflection;
using TMPro;
using UnityEngine;

namespace DetailedLevels
{
    public class ControllerUtils
    {
        public static void BindGamePad(Transform buttonGo, KeyCode gamepadKeyCode, InventoryGui inventoryGui = null)
        {
            UIGamePad uiGamePad = null;
            if (buttonGo.TryGetComponent(out uiGamePad))
            {
                string gamepadKey = KeyCodeToString(gamepadKeyCode);
                if (ZInput.instance != null)
                {
                    uiGamePad.m_hint.GetComponentInChildren<TextMeshProUGUI>(true).text = ZInput.instance.GetBoundKeyString(gamepadKey, true);
                }
                else
                {
                    ZInput.Initialize();
                    uiGamePad.m_hint.GetComponentInChildren<TextMeshProUGUI>(true).text = ZInput.instance?.GetBoundKeyString(gamepadKey, true);
                }
                uiGamePad.m_zinputKey = gamepadKey;
                uiGamePad.m_keyCode = gamepadKeyCode;
                if (inventoryGui != null && inventoryGui.m_crafting.TryGetComponent(out UIGroupHandler group))
                {
                    SetPrivateValue(uiGamePad, "m_group", group);
                }
            }
        }
        
        private static void SetPrivateValue(object obj, string name, object value, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.NonPublic)
        {
            obj.GetType().GetField(name, bindingAttr)?.SetValue(obj, value);
        }
        
        public static string KeyCodeToString(KeyCode keyCode)
        {
            return ((int)keyCode - 330) switch
            {
                1 => "JoyButtonB", 
                2 => "JoyButtonX", 
                3 => "JoyButtonY", 
                4 => "JoyLBumper", 
                5 => "JoyRBumper", 
                6 => "JoyBack", 
                7 => "JoyStart", 
                8 => "JoyLStick", 
                9 => "JoyRStick", 
                10 => "JoyDPadLeft", 
                11 => "JoyDPadRight", 
                12 => "JoyDPadUp", 
                13 => "JoyDPadDown", 
                14 => "JoyLTrigger", 
                15 => "JoyRTrigger", 
                16 => "JoyButtonA", 
                17 => "JoyButtonB", 
                18 => "JoyButtonX", 
                19 => "JoyButtonY", 
                _ => "JoyButtonA", 
            };
        }
    }
}