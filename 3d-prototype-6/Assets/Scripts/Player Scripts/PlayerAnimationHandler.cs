using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationHandler : MonoBehaviour
{
    public Player player;
    public void FinishEquip()
    {
        player.combat.canFire = true;
    }

    public void RefreshWeapon()
    {
        player.combat.canFire = true;
    }

    public void NonAutoRefreshWeapon()
    {
        player.combat.nonAutoRefresh = true;
    }

    public void SignalReload()
    {
        Debug.Log("Signal Reload Called");
        if (!player.combat.inHand.CanReload) return;
        Debug.Log("Signal Reload Passed");

        player.body.ADSFade(false, player.combat.adsSpeed);
        HUDManager.ToggleCursor(true);
        player.body.PlayWeapon("IsReloading", true);
        player.combat.canFire = false;
    }

    public void FinishReload()
    {
        player.body.PlayWeapon("IsReloading", false);
        player.combat.FullReload();
    }

    public void SingleReload()
    {

        if (!player.combat.inHand.CanReload)
        {
            player.body.PlayWeapon("No Bullets", false);
            return;
        }
        

        player.combat.inHand.SingleReload();
        HUDManager.UpdateAmmoText(player.combat.inHand.currentBulletCount, player.combat.inHand.currentRounds);


    }

    public void CheckAmmoCount()
    {
        if (!player.combat.inHand.CanReload) FinishReload();
    }

    public void SliderReset()
    {
        player.body.PlayWeapon("No Bullets", false);
    }

    public void ForceNoBullets()
    {
        player.body.PlayWeapon("No Bullets", true);
    }

    public void SwapOutWeapon()
    {
        player.combat.swappedInWeapon?.Invoke();
    }
}
