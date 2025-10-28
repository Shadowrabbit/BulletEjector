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

        // 获取武器内的所有子弹Item（每个Item可能包含多个堆叠）
        var bulletsToRemove = new List<Item>();
        foreach (var item in weapon.Inventory)
        {
            if (item != null && item.GetBool("IsBullet"))
            {
                bulletsToRemove.Add(item);
            }
        }

        // 从武器中移除子弹Item（包含所有堆叠）
        foreach (var bulletItem in bulletsToRemove)
        {
            weapon.Inventory.RemoveItem(bulletItem);
            strippedBullets.Add(bulletItem);
        }

        // 将缓存设置为-1，强制UI重新计算子弹数量
        var cacheField = AccessTools.Field(typeof(ItemSetting_Gun), "_bulletCountCache");
        cacheField?.SetValue(gunSetting, -1);
        return strippedBullets;
    }
}