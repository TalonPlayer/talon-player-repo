using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CostumeManager : MonoBehaviour
{
    public static CostumeManager Instance;
    private MyEntity currentEntity;
    void Awake()
    {
        Instance = this;
    }

    [Header("Outfits")]
    public Outfit bandana; // elbow
    public List<Outfit> faceList, hatList, gloveList, shoeList, beltList;
    public List<Outfit> zombieHatList;
    public void Dress(MyEntity entity, EntityObj info)
    {
        currentEntity = entity;
        List<Costume> order = new List<Costume>()
        {
            Costume.Face,
            Costume.Hat,
            Costume.GloveR,
            Costume.GloveL,
            Costume.ShoeR,
            Costume.ShoeL,
            Costume.Belt,
        };

        if (entity.brain.isHuman)
            order.Add(Costume.Bandana);


        for (int i = 0; i < order.Count; i++)
        {
            Transform bodyPart = null;
            string id = null;
            switch (order[i])
            {
                case Costume.Face:
                    bodyPart = currentEntity.body.face;
                    id = info.faceID;
                    break;
                case Costume.Hat:
                    bodyPart = currentEntity.body.hat;

                    if (!currentEntity.brain.isHuman)
                        id = "Z" + info.hatID;
                    else
                        id = info.hatID;
                    break;
                case Costume.GloveR:
                    bodyPart = currentEntity.body.gloveR;
                    id = info.glovesID;
                    break;
                case Costume.GloveL:
                    bodyPart = currentEntity.body.gloveL;
                    id = info.glovesID;
                    break;
                case Costume.ShoeL:
                    bodyPart = currentEntity.body.shoeL;
                    id = info.shoesID;
                    break;
                case Costume.ShoeR:
                    bodyPart = currentEntity.body.shoeR;
                    id = info.shoesID;
                    break;
                case Costume.Belt:
                    bodyPart = currentEntity.body.belt;
                    id = info.beltID;
                    break;
                case Costume.Bandana:
                    bodyPart = currentEntity.body.bandana;
                    ChangeOutfit(bodyPart, id, order[i]);
                    return;
            }

            if (string.IsNullOrEmpty(id) || bodyPart == null) continue;
            ChangeOutfit(bodyPart, id, order[i]);
        }
    }

    public void ChangeOutfit(Transform bodyPart, string id, Costume type)
    {
        // Clear children of transform, this gets rid of any current outfits
        if (bodyPart.childCount != 0)
            foreach (Transform child in Helper.GetChildren(bodyPart))
                Destroy(child.gameObject);

        List<Outfit> outfits = new List<Outfit>();

        // Get the outfit type
        switch (type)
        {
            case Costume.Face:
                outfits = faceList;
                break;
            case Costume.Hat:
                if (id[0] == 'Z')
                    outfits = zombieHatList;
                else
                    outfits = hatList;

                // Find outfit by ID
                Outfit hat = outfits.Find(o => o.ID == id);
                // Validation
                if (hat == null) { Debug.Log("Invalid outfit id: " + id); return; }

                // Create the outfit
                GameObject hatObj = Instantiate(hat.outfit, bodyPart);
                currentEntity.body.heldItems.Add(hatObj);
                
                return;
            case Costume.GloveR:
                outfits = gloveList;
                break;
            case Costume.GloveL:
                outfits = gloveList;
                break;
            case Costume.ShoeR:
                outfits = shoeList;
                break;
            case Costume.ShoeL:
                outfits = shoeList;
                break;
            case Costume.Belt:
                outfits = beltList;
                break;
            case Costume.Bandana:
                GameObject obj = Instantiate(bandana.outfit, bodyPart);
                currentEntity.body.heldItems.Add(obj);
                return;
        }

        // Find outfit by ID
        Outfit item = outfits.Find(o => o.ID == id);
        // Validation
        if (item == null) { Debug.Log("Invalid outfit id: " + id); return; }

        // Create the outfit
        Instantiate(item.outfit, bodyPart);
    }


}

public enum Costume
{
    Face,
    Hat,
    GloveR,
    GloveL,
    ShoeR,
    ShoeL,
    Belt,
    Bandana
}

