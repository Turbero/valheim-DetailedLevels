using BepInEx;
using HarmonyLib;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Utils;

namespace DetailedLevels
{
    [BepInPlugin("Turbero.DetailedLevels", "Detailed Levels", "1.0.0")]
    public class DetailedLevels : BaseUnityPlugin
    {
        private readonly Harmony harmony = new Harmony("Turbero.DetailedLevels");

        void Awake()
        {
            harmony.PatchAll();
        }

        [HarmonyPatch(typeof(SkillsDialog), nameof(SkillsDialog.Setup))]
        class SkillValue_Patch
        {
            static void Postfix(ref Player player, ref List<GameObject> ___m_elements)
            {
                List<Skills.Skill> skillList = player.GetSkills().GetSkillList();
                for (int j = 0; j < skillList.Count; j++)
                {
                    GameObject obj = ___m_elements[j];
                    Skills.Skill skill = skillList[j];
                    if (skill.m_accumulator > 0)
                    {
                        float levelPercentage = skill.GetLevelPercentage();
                        double skillLevel = Math.Round(skill.m_level);
                        double skillValue = Math.Round(skillLevel + levelPercentage, 2);

                        //Debug.Log($"Name/skillLevel/skillLevelRounded/levelPercentage/skillValue: {skill.m_info.m_skill.ToString()}/{skill.m_level}/{skillLevel}/{levelPercentage}/{skillValue}");

                        Utils.FindChild(obj.transform, "leveltext", (IterativeSearchType)0).GetComponent<TMP_Text>().text = skillValue.ToString();
                    }
                }
            }
        }
    }
}

