// ******************************************************************
//       /\ /|       @file       ItemExtensions.cs
//       \ V/        @brief      
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2025-10-27 02:59:42
//    *(__\_\        @Copyright  Copyright (c) 2025, Shadowrabbit
// ******************************************************************

using System.Collections.Generic;
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
            return strippedBullets;

        var gunSetting = weapon.GetComponent<ItemSetting_Gun>();
        if (gunSetting == null)
            return strippedBullets;

        // 获取武器内的所有子弹（在TakeOutAllBullets之前）
        var bulletsInGun = new List<Item>();
        foreach (var bullet in weapon.Inventory)
        {
            if (bullet != null && bullet.GetBool("IsBullet"))
            {
                bulletsInGun.Add(bullet);
            }
        }

        // 使用现有的TakeOutAllBullets方法
        gunSetting.TakeOutAllBullets();
        // 返回被剥离的子弹
        return bulletsInGun;
    }

    /// <summary>
    /// 检查物品是否是武器
    /// </summary>
    /// <param name="item">物品</param>
    /// <returns>是否是武器</returns>
    public static bool IsWeapon(this Item item)
    {
        return item != null && item.GetComponent<ItemSetting_Gun>() != null;
    }
}