using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace DetailedLevels.Tools
{
	internal class GameManager
	{
		public static object GetPrivateValue(object obj, string name, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.NonPublic)
		{
			return obj.GetType().GetField(name, bindingAttr)?.GetValue(obj);
		}
        
		public static object GetPrivateMethod(object obj, string name, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.NonPublic)
		{
			return obj.GetType().GetMethod(name, bindingAttr)?.Invoke(obj, null);
		}
		
		public static int GetCurrentDay()
		{
			return (int) GetPrivateMethod(EnvMan.instance, "GetCurrentDay");
		}
	}
	
	[HarmonyPatch(typeof(Player), "Message")]
	internal static class PlayerMessagePatch
	{
		[UsedImplicitly]
		private static bool Prefix(Player __instance, MessageHud.MessageType type, string msg, int amount = 0, Sprite icon = null)
		{
			ZNetView m_nview = (ZNetView)GameManager.GetPrivateValue(__instance, "m_nview");
			if (m_nview == null || !m_nview.IsValid())
			{
				return false;
			}
			if (Player.m_localPlayer == null)
			{
				return false;
			}
			if (msg == Localization.instance.Localize("$msg_newday", GameManager.GetCurrentDay().ToString()) && ConfigurationFile.dayMessageOff.Value)
			{
				return false;
			}
			return true;
		}
	}
	
	[HarmonyPatch(typeof(MessageHud), "ShowMessage")]
	internal static class MessageHudShowMessagePatch
	{
		[UsedImplicitly]
		private static bool Prefix(MessageHud __instance, MessageHud.MessageType type, string text, int amount = 0, Sprite icon = null)
		{
			if (Hud.IsUserHidden())
			{
				return false;
			}
			if (text == Localization.instance.Localize("$msg_newday", GameManager.GetCurrentDay().ToString()) && ConfigurationFile.dayMessageOff.Value)
			{
				return false;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(Minimap), "AddPin")]
	internal static class MinimapAddPinPatch
	{
		[UsedImplicitly]
		private static void Postfix(Minimap __instance, Vector3 pos, Minimap.PinType type, string name, bool save, bool isChecked, long ownerID = 0L)
		{
			if (ConfigurationFile.dayMessageOff.Value)
			{
				List<Minimap.PinData> m_pins = (List<Minimap.PinData>)GameManager.GetPrivateValue(__instance, "m_pins");
				foreach (Minimap.PinData pin in m_pins)
				{
					if (pin.m_name == $"$hud_mapday {EnvMan.instance.GetDay(ZNet.instance.GetTimeSeconds())}")
					{
						pin.m_name = "";
					}
					else if (pin.m_name.Contains("$hud_mapday"))
					{
						pin.m_name = "";
					}
				}
			}
		}
	}
}