// ******************************************************************
//       /\ /|       @file       ItemExtensions.cs
//       \ V/        @brief      物品扩展方法
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2025-10-27 02:59:42
//    *(__\_\        @Copyright  Copyright (c) 2025, Shadowrabbit
// ******************************************************************

using System.Collections.Generic;
using HarmonyLib;
using ItemStatsSystem;

namespace BulletEjector.Extensions;

// 创建扩展方法
public static class ItemExtensions
{
    /// <summary>
    /// 剥离武器中的所有子弹
    /// </summary>
    /// <param name="weapon">武器物品</param>
    /// <returns>被剥离的子弹列表</returns>
    public static List<Item> StripBullets(this Item weapon)
    {
        var strippedBullets = new List<Item>();
        if (weapon == null)
        {
            return strippedBullets;
        }

        var gunSetting = weapon.GetComponent<ItemSetting_Gun>();
        if (gunSetting == null)
        {
            return strippedBullets;
        }

        // 手动移除武器内的所有子弹，不调用TakeOutAllBullets
        var bulletsToRemove = new List<Item>();
        foreach (var bullet in weapon.Inventory)
        {
            if (bullet != null && bullet.GetBool("IsBullet"))
            {
                bulletsToRemove.Add(bullet);
            }
        }

        // 从武器中移除子弹（使用公开API）
        foreach (var bullet in bulletsToRemove)
        {
            weapon.Inventory.RemoveItem(bullet);
            strippedBullets.Add(bullet);
        }

        // 使内部缓存失效，下一次读取时会根据Inventory重新计算
        var cacheField = AccessTools.Field(typeof(ItemSetting_Gun), "_bulletCountCache");
        cacheField?.SetValue(gunSetting, -1);
        return strippedBullets;
    }
}