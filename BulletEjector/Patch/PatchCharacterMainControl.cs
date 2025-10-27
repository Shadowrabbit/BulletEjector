// ******************************************************************
//       /\ /|       @file       PatchCharacterMainControl.cs
//       \ V/        @brief      死亡前剥离子弹（挂钩在CharacterMainControl.OnDead）
//       | "")       @author     Catarina·RabbitNya, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2025-10-27 03:12:31
//    *(__\_\        @Copyright  Copyright (c) 2025, Shadowrabbit
// ******************************************************************

using System.Collections.Generic;
using System.Linq;
using BulletEjector.Extensions;
using HarmonyLib;
using ItemStatsSystem;
using ItemStatsSystem.Items;
using JetBrains.Annotations;
using UnityEngine;

namespace BulletEjector.Patch;

/// <summary>
/// 在角色死亡前剥离其背包与已装备武器中的子弹
/// 正确挂点：CharacterMainControl.OnDead（在生成战利品箱之前）
/// </summary>
[HarmonyPatch(typeof(CharacterMainControl), "OnDead")]
[UsedImplicitly]
public static class PatchCharacterMainControl
{
    /// <summary>
    /// Prefix：在OnDead开始时执行，早于掉落箱生成
    /// 仅对非玩家角色生效
    /// </summary>
    [HarmonyPrefix]
    [UsedImplicitly]
    public static void Prefix(CharacterMainControl __instance)
    {
        if (__instance == null)
        {
            return;
        }

        // 仅对非主角处理，保持与原逻辑一致
        if (LevelManager.Instance != null && LevelManager.Instance.MainCharacter == __instance)
        {
            return;
        }

        var characterItem = __instance.CharacterItem;
        if (characterItem == null) return;
        Debug.Log($"[BulletEjector] Strip bullets before death for: {characterItem.DisplayName}");
        StripBulletsFromCharacterItem(characterItem);
    }

    /// <summary>
    /// 从角色根Item（背包与装备槽）中剥离子弹
    /// </summary>
    private static void StripBulletsFromCharacterItem(Item characterItem)
    {
        // 背包内
        StripBulletsFromInventory(characterItem.Inventory);
        // 已装备槽位
        if (characterItem.Slots == null) return;
        var slotSnapshot = new List<Slot>();
        foreach (var s in characterItem.Slots)
        {
            if (s != null) slotSnapshot.Add(s);
        }

        foreach (var slot in slotSnapshot)
        {
            var equipped = slot.Content;
            if (equipped == null) continue;
            StripBulletsFromWeapon(equipped);
        }
    }

    /// <summary>
    /// 从Inventory中剥离子弹
    /// </summary>
    private static void StripBulletsFromInventory(Inventory? inventory)
    {
        if (inventory?.Content == null) return;
        var snapshot = new List<Item>();
        var content = inventory.Content;
        for (var i = 0; i < content.Count; i++)
        {
            var it = content[i];
            if (it != null) snapshot.Add(it);
        }

        foreach (var item in snapshot)
        {
            if (item.GetComponent<ItemSetting_Gun>() != null)
            {
                StripBulletsFromWeapon(item);
            }
        }
    }

    /// <summary>
    /// 从单个枪械中剥离子弹
    /// </summary>
    private static void StripBulletsFromWeapon(Item weapon)
    {
        var bulletItems = weapon.StripBullets();
        if (bulletItems.Count == 0) return;
        // 计算总子弹数量（包括堆叠）
        var totalBulletCount = bulletItems.Sum(bullet => bullet?.StackCount ?? 0);
        Debug.Log($"[BulletEjector] Stripped {totalBulletCount} bullets from weapon {weapon.DisplayName}");
        // 将子弹Item添加回角色Inventory
        foreach (var bulletItem in bulletItems)
        {
            if (bulletItem == null) continue;
            var rootItem = weapon.GetRoot();
            var added = rootItem.Inventory?.AddAndMerge(bulletItem) ?? false;
            if (!added)
            {
                Debug.LogError($"[BulletEjector] Failed to add bullet {bulletItem.DisplayName} back to Inventory");
            }
        }
    }
}