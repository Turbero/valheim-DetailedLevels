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
                if (itemDrop != null && (itemDrop.m_itemData.HavePrimaryAttack() || itemDrop.m_itemData.HaveSecondaryAttack()) && !isInToolList(itemDrop.name))
                {
                    Skills.SkillType skillType = itemDrop.m_itemData.m_shared.m_skillType;
                    if (skillType != Skills.SkillType.None)
                    {
                        //Add skillType to tooltip
                        string description = itemDrop.m_itemData.m_shared.m_description.Split('\n')[0]; // To remove profession from previous execution
                        itemDrop.m_itemData.m_shared.m_description = description + //do not use "+=" !
                                                                        $"\nSkill: $skill_{skillType.ToString().ToLower()}";
                    }
                }
            }
            updated = true;
        }

        private static bool isInToolList(string itemDropName)
        {
            return new List<string> { "Cultivator", "Hoe", "Tankard", "TankardAnniversary", "Tankard_dvergr" }.Contains(itemDropName);
        }
    }
}
