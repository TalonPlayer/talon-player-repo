using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EntityPool : MonoBehaviour
{
    [Header("Weapon")]
    [HideInInspector] public List<WeaponModel> weapons;
    [HideInInspector] public List<WeaponModel> exclusiveWeapons;
    public List<string> lastNameInitials = new List<string>(){
        "A.", "B.", "C.", "D.", "E.", "F.", "G.", "H.", "I.", "J.", "K.", "L.",
        "M.", "N.", "O.", "P.", "Q.", "R.", "S.", "T.", "U.", "V.", "W.", "X.",
        "Y.", "Z."
    };
    public List<string> firstNames = new List<string>()
    {
        "Benold","Johnny","Michael","William","Cheryl","Bartholomew","Marcus","Ted","Joaquin","Ken","George","Jeff","Iggy","Vienna","Brent",
        "Chad","Steve","Roger","Tony","Bruce","Clark","Nicholas","Benjamin","Jaiden","Audrey","Maria","Konkey Dong","Bob","Helen","Bruce",
        "Matt","Riley","Delilah","Kevin","Jared","Jack","Felix","Ian","Ryan","Danny","Devon","James","Adam","Natasha","Katie",
        "Bailey","Cris","Rainer","Adrian","Antwan","Alex","Juliet","Bella","Marisa","Mia","Melody","Maggie","Lisa","Sharon","Karen",
        "Eddy","Tom","Andy","Andrew","Amy","Toby","Patrick","Parker","Peter","John","Vinh","Arista","Kami","Shizumaki","Miyamoto",
        "Walter","Hank","Skyler","Jimmy","Gus","Mike","Jane","Shanel","Zach","Sean","Areeb","Samantha","Jose","Sophia","Jill",
        "Harvey","Will","Elle","Ellie","Ella","Thomas","Thompson","Jorge","Naomi","Fred","Frederick","Jana","Akko","Richard","Rachel",
        "Chuck","Howard","Troy","Carly","Wendall","Timmy","Lil Timmy","Carl","Emma","Jesse","Bo","Daniel","Victor","Mack","Chandler",
        "Shane","Bonnie","Jamie","Vicor","Mitch","Michael","Mikey","Ralph","Donatello","Leo","Jax","Leonardo","Isaac","Ted","Jason",
        "Todd", "Ezekial","Chao","Peng","Shen","Chan","Eloise","Mathieu","Camille","Siobhan","Ciaran","Aoife","Sigrid","Henrik","Anja",
        "Lukas","Haruki","Yuto","Sora","Takumi","Ren","Aiko","Yui","Sakura","Kenji","Wei","Lina","Jiahao","Yuxuan","Yiwei","Ming","Lihua",
        "Zhen","Haoran","Meiling","Chen","Min-Jun","Seo-yeon","Ji-hoon","Ha-yeon","Jae-won","Soo-bin","Yu-na","Dae-hyun","Hye-jin","Seung-min",
        "Jose","Maria","Andres","Liza","Carlo","Ana","Miguel","Lea", "Paolo","Kristine","Somchai","Chaiwat","Kittisak","Ananda","Narin","Nicha",
        "Kanya","Pranee","Suda","Supaporn","Ivan","Dmitri","Alexei","Sergei","Nikolai","Mikhail","Anastasia","Ekaterina","Olga","Tatiana","Mehmet",
        "Ahmet","Emre","Can","Eren","Deniz","Ayşe","Elif","Fatima","Selin","Arjun","Rahul","Rohan","Aarav","Vihaan","Ananya","Priya",
        "Kavya","Isha","Neha","Oleksandr","Andriy","Bohdan","Mykola","Yuriy","Taras","Kateryna","Oksana","Olena","Vitalii","Kamau",
        "Wanjiru","Wambui","Njeri","Njoroge","Mwangi","Otieno","Onyango","Chebet","Kipchoge","Friedrich","Klaus"
    };
    public List<Texture> shirtTextures;
    public List<Color> primaryColors;
    public List<Color> secondaryColors;
    void Awake()
    {
        shirtTextures = Resources.LoadAll<Texture>("Sprites/Shirt Textures").ToList();
        weapons = Resources.LoadAll<WeaponModel>("Weapons").ToList();
        weapons = weapons.FindAll(w => !w.isExclusive);
        exclusiveWeapons = weapons.FindAll(w=>w.isExclusive);
    }

}
