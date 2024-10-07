using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using static Utils;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

namespace DetailedLevels.Features
{
    [HarmonyPatch(typeof(SkillsDialog), nameof(SkillsDialog.Setup))]
    class SkillsDialog_Patch
    {
        // Diccionario para rastrear si la habilidad tiene un buff activo
        private static Dictionary<string, bool> skillBuffs = new Dictionary<string, bool>();

        private static int hashCode;

        static void Postfix(ref Player player, ref List<GameObject> ___m_elements)
        {
            if (!ConfigurationFile.modEnabled.Value || InventoryGui.instance == null) return;

            // Crear una copia de la variable player para evitar problemas de captura en la lambda
            var currentPlayer = player;

            // Agregar listeners de clic a cada fila de habilidades
            for (int i = 0; i < ___m_elements.Count; i++)
            {
                GameObject skillRow = ___m_elements[i];

                // Capturar el índice de la habilidad
                int skillIndex = i;

                // Agregar listener para el clic
                skillRow.GetComponent<Button>().onClick.AddListener(() =>
                {
                    Skills.Skill skill = currentPlayer.GetSkills().GetSkillList()[skillIndex];
                    OnSkillClicked(currentPlayer, skill, skillRow);
                });
            }
        }

        private static void OnSkillClicked(Player player, Skills.Skill skill, GameObject skillRow)
        {
            // Obtener el nombre de la habilidad
            string skillName = "Buff for " + skill.m_info.m_skill.ToString();

            // Comprobar si el buff ya está activo
            bool hasBuff = skillBuffs.ContainsKey(skillName) && skillBuffs[skillName];

            if (hasBuff)
            {
                // Si el buff ya está activo, eliminarlo
                RemoveSkillBuff(player, skill);
                skillBuffs[skillName] = false;

                // Restablecer el fondo de la fila al color original
                skillRow.GetComponent<Image>().color = Color.white;
            }
            else
            {
                // Si el buff no está activo, agregar el buff
                Sprite skillIcon = GetSkillIcon(skillRow);  // Obtener el ícono de la habilidad
                AddSkillBuff(player, skill, skillIcon, skillRow);
                skillBuffs[skillName] = true;

                // TODO Cambiar el fondo de la fila a azul
                //skillRow.GetComponent<Image>().color = Color.blue;
            }
        }

        // Método para obtener el ícono de la habilidad desde la fila de habilidad
        private static Sprite GetSkillIcon(GameObject skillRow)
        {
            // Busca el componente de imagen (ícono) en la fila
            Image skillIcon = skillRow
                .transform.Find("icon_bkg").GetComponent<Image>()
                .transform.Find("icon").GetComponent<Image>();
            if (skillIcon != null)
            {
                return skillIcon.sprite;
            }
            return null;
        }

        private static void AddSkillBuff(Player player, Skills.Skill skill, Sprite skillIcon, GameObject skillRow)
        {
            // Usar reflection para obtener el campo m_seman
            var seManField = typeof(Player).GetField("m_seman", BindingFlags.NonPublic | BindingFlags.Instance);
            if (seManField != null)
            {
                SEMan seMan = (SEMan)seManField.GetValue(player);

                // Crear un nuevo efecto de estado personalizado (buff)
                SE_Stats customBuff = ScriptableObject.CreateInstance<SE_Stats>();

                // Personalizar los atributos del buff
                customBuff.m_name = "Buff for " + skill.m_info.m_skill;
                customBuff.m_tooltip = $"This is a custom buff based on {skill.m_info.m_skill}";
                customBuff.m_icon = skillIcon; // Usar el ícono de la habilidad


                //NIVEL ACTUAL DE LA SKILL SELECCIONADA
                /*int skillLevel = (int)skill.m_level; 
                float levelText = skillLevel;
                float levelPercentage = skill.GetLevelPercentage();

                // Number of decimals
                if (skill.m_accumulator > 0)
                {
                    levelText = (float)Math.Round(skillLevel + levelPercentage, Math.Min(15, Math.Max(0, numberOfDecimals.Value)));
                }
                customBuff.m_cooldown = levelText;*/

                //NIVEL ACTUAL SIN CALCULAR
                String value = Utils.FindChild(skillRow.transform, "leveltext", (IterativeSearchType)0).GetComponent<TMP_Text>().text;
                Logger.Log("*** Valor para el buff: " + value);
                customBuff.m_cooldown = float.Parse(value);
                //customBuff.m_addMaxHealth = skill.m_level; // Aumentar la vida según el nivel de la habilidad
                //customBuff.m_addMaxStamina = skill.m_level / 2f; // Aumentar la estamina

                // Aplicar el buff al jugador
                hashCode = customBuff.GetHashCode();
                seMan.AddStatusEffect(customBuff);

                Logger.Log($"Buff añadido: {skill.m_info.m_skill} con nivel {skill.m_level}");
            }
            else
            {
                Logger.LogError("No se pudo acceder al campo m_seman del jugador.");
            }
        }

        private static void RemoveSkillBuff(Player player, Skills.Skill skill)
        {
            // Usar reflection para obtener el campo m_seman
            var seManField = typeof(Player).GetField("m_seman", BindingFlags.NonPublic | BindingFlags.Instance);
            if (seManField != null)
            {
                SEMan seMan = (SEMan)seManField.GetValue(player);

                // Encontrar y eliminar el efecto de estado (buff) que corresponde a la habilidad
                StatusEffect existingBuff = seMan.GetStatusEffect(hashCode);
                if (existingBuff != null)
                {
                    seMan.RemoveStatusEffect(existingBuff);
                    Logger.Log($"Buff eliminado: {skill.m_info.m_skill}");
                }
            }
            else
            {
                Logger.LogError("No se pudo acceder al campo m_seman del jugador.");
            }
        }
    }
}
