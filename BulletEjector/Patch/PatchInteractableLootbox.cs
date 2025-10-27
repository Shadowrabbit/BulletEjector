// ******************************************************************
//       /\ /|       @file       PatchInteractableLootbox.cs
//       \ V/        @brief      死亡掉落子弹剥离补丁
//       | "")       @author     Catarina·RabbitNya, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2025-01-27 03:12:31
//    *(__\_\        @Copyright  Copyright (c) 2025, Shadowrabbit
// ******************************************************************

using BulletEjector.Extensions;
using HarmonyLib;
using ItemStatsSystem;
using JetBrains.Annotations;
using UnityEngine;

namespace BulletEjector.Patch;

/// <summary>
/// InteractableLootbox的Harmony补丁类
/// 用于在NPC死亡掉落时自动剥离枪械内的子弹
/// </summary>
[HarmonyPatch(typeof(InteractableLootbox), nameof(InteractableLootbox.CreateFromItem))]
[UsedImplicitly]
public static class PatchInteractableLootbox
{
    /// <summary>
    /// Prefix补丁：在CreateFromItem方法执行前触发
    /// 自动剥离角色Item中枪械的子弹
    /// </summary>
    /// <param name="item">要创建战利品箱的角色Item</param>
    [HarmonyPrefix]
    [UsedImplicitly]
    public static void Prefix(ref Item item)
    {
        if (item == null) return;
        Debug.Log($"[BulletEjector] Processing character Item: {item.DisplayName}");
        StripBulletsFromItem(item);
    }

    /// <summary>
    /// 从Item中剥离子弹
    /// </summary>
    private static void StripBulletsFromItem(Item item)
    {
        // 检查Item本身
        StripBulletsFromWeapon(item);
        // 只检查Inventory中的物品
        StripBulletsFromInventory(item.Inventory);
    }

    /// <summary>
    /// 从Inventory中剥离子弹
    /// </summary>
    private static void StripBulletsFromInventory(Inventory? inventory)
    {
        if (inventory?.Content == null) return;
        foreach (var item in inventory.Content)
        {
            StripBulletsFromWeapon(item);
        }
    }

    /// <summary>
    /// 从单个枪械中剥离子弹
    /// </summary>
    private static void StripBulletsFromWeapon(Item weapon)
    {
        var bullets = weapon.StripBullets();
        if (bullets.Count == 0) return;
        Debug.Log($"[BulletEjector] Stripped {bullets.Count} bullets from weapon {weapon.DisplayName}");
        // 将子弹添加回角色Inventory
        foreach (var bullet in bullets)
        {
            if (bullet == null)
            {
                continue;
            }

            var rootItem = weapon.GetRoot();
            var added = rootItem.Inventory?.AddAndMerge(bullet) ?? false;
            if (!added)
            {
                Debug.LogError($"[BulletEjector] Failed to add bullet {bullet.DisplayName} back to Inventory");
            }
        }
    }
}