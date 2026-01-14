using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Layer
{
    public static int Ground{ get { return 1 << 6; }}
    public static int Wall{ get { return 1 << 7; }}
    public static int Entity{ get { return 1 << 8; }}
    public static int Player{ get { return 1 << 9; }}
    public static int Enemy{ get { return 1 << 10; }}
    public static int Ally{ get { return 1 << 11; }}
    public static int Projectile{ get { return 1 << 12; }}
    public static int Debris{ get { return 1 << 13; }}
    public static int Ragdoll{ get { return 1 << 14; }}
    public static int Interactable{ get { return 1 << 15; }}
    public static int UIOverlay{ get { return 1 << 16; }}
    public static int Cover{ get { return 1 << 17; }}
}

/*
    6 Ground
    7 Wall
    8 Entity
    9 Player
    10 Enemy
    11 Ally
    12 Projectile
    13 Debris
    14 Ragdoll
    15 Interactable
*/
