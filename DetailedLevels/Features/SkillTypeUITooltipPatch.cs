using System.Collections.Generic;
using HarmonyLib;

namespace DetailedLevels.Features
{
    [HarmonyPatch(typeof(InventoryGui), "Show")]
    public static class SkillTypeCraftStationDescriptionsPatch
    {
        public static bool updated = false;

        public static void Prefix(Container container, int activeGroup)
        {
            if (updated) return;

            foreach (Recipe recipe in ObjectDB.instance.m_recipes)
            {
                ItemDrop itemDrop = recipe.m_item;
                if (itemDrop != null &&
                    (itemDrop.m_itemData.HavePrimaryAttack() || itemDrop.m_itemData.HaveSecondaryAttack()) &&
                    !isInToolList(itemDrop.name))
                {
                    Skills.SkillType skillType = itemDrop.m_itemData.m_shared.m_skillType;
                    if (skillType != Skills.SkillType.None)
                    {
                        //Add skillType to tooltip
                        string description =
                            itemDrop.m_itemData.m_shared.m_description
                                .Split('\n')[0]; // To remove profession from previous execution
                        itemDrop.m_itemData.m_shared.m_description = description + //do not use "+=" !
                                                                     $"\nSkill: $skill_{skillType.ToString().ToLower()}";
                    }
                }
            }

            updated = true;
        }

        private static bool isInToolList(string itemDropName)
        {
            return new List<string>
                { "Cultivator", "Hoe", "Tankard", "TankardAnniversary", "Tankard_dvergr" }.Contains(itemDropName);
        }
    }

    /*[HarmonyPatch(typeof(UITooltip), "UpdateTextElements")]
    public static class SkillTypeUITooltipPatch
    {
        public static void Postfix(UITooltip __instance)
        {
            GameObject m_tooltip = (GameObject)GameManager.GetPrivateValue(__instance, "m_tooltip", BindingFlags.Static | BindingFlags.NonPublic);
            if (m_tooltip != null)
            {
                Transform transform = Utils.FindChild(m_tooltip.transform, "Text");
                if (transform != null)
                {
                    Logger.Log("UpdateTextElements - before __instance.text: "+__instance.m_text);
                    Logger.Log("UpdateTextElements - transform.GetComponent<TMP_Text>().text: "+transform.GetComponent<TMP_Text>().text);

                    //Add skillType
                    var professionText = "REPLACE ME WITH SKILL TYPE";
                    __instance.m_text = __instance.m_text.Replace("_description", "_description " + professionText);
                    Logger.Log("UpdateTextElements - post __instance.text: "+__instance.m_text);

                    transform.GetComponent<TMP_Text>().text = Localization.instance.Localize(__instance.m_text); //Recalculate
                }

            }
        }
    }*/
}
