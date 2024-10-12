using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DetailedLevels.Features
{
    /*[HarmonyPatch(typeof(Character), nameof(Character.m_onDeath))]
    public class PlayerKillingBirds
    {
        static void Postfix(Bird __instance, DamageInfo damage)
        {
            if (!ConfigurationFile.birdsSkillIncrease.Value) return;

            // Verificar si la entidad eliminada es una gaviota
            if (__instance.name.Contains("Seagull"))
            {
                // Obtener al jugador que mató a la gaviota
                Player killer = GetKillerPlayer(damage);

                if (killer != null)
                {
                    // Determinar el arma utilizada para matar a la gaviota
                    ItemDrop.ItemData equippedItem = killer.GetCurrentWeapon();

                    // Si hay un arma equipada, aumentar la habilidad correspondiente
                    if (equippedItem != null)
                    {
                        Skills.SkillType skillToRaise = equippedItem.m_shared.m_itemType;
                        float skillIncreaseFactor = 1.0f; // Ajusta según sea necesario

                        killer.RaiseSkill(skillToRaise, skillIncreaseFactor);
                        Debug.Log($"El jugador ha matado una gaviota con {equippedItem.m_shared.m_name}. Habilidad {skillToRaise} incrementada.");
                    }
                    else
                    {
                        // Si el jugador está desarmado
                        Debug.Log("El jugador ha matado una gaviota sin arma equipada.");
                        killer.RaiseSkill(Skills.SkillType.Unarmed, 1.0f); // Incrementa la habilidad de combate desarmado
                    }
                }
            }
        }

        // Método auxiliar para obtener al jugador que causó la muerte
        private static Player GetKillerPlayer(DamageInfo damage)
        {
            if (damage != null && damage.GetAttacker() != null)
            {
                GameObject attackerObject = damage.GetAttacker().gameObject;
                Player attackerPlayer = attackerObject.GetComponent<Player>();

                // Devuelve el jugador que causó el daño si es un jugador
                return attackerPlayer;
            }
            return null;
        }

        private static Skills.SkillType GetSkillTypeFromWeapon(ItemDrop.ItemData weapon)
        {
            switch (weapon.m_shared.m_skillType)
            {
                case Skills.SkillType.Swords:
                    return Skills.SkillType.Swords;
                case Skills.SkillType.Axes:
                    return Skills.SkillType.Axes;
                case Skills.SkillType.Bows:
                    return Skills.SkillType.Bows;
                case Skills.SkillType.Knives:
                    return Skills.SkillType.Knives;
                case Skills.SkillType.Polearms:
                    return Skills.SkillType.Polearms;
                case Skills.SkillType.Spears:
                    return Skills.SkillType.Spears;
                case Skills.SkillType.Clubs:
                    return Skills.SkillType.Clubs;
                // Añade más tipos de armas según sea necesario
                default:
                    return Skills.SkillType.Unarmed; // Predeterminado si no se reconoce el arma
            }
        }
    }*/
}
