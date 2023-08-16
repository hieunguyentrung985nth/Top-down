using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public class DataToSave
{
    public List<WeaponAmmoAmount> weaponAmount = new List<WeaponAmmoAmount>();

    public List<SupportItemAmount> supportAmount = new List<SupportItemAmount>();

    public List<PowerItemAmount> powerAmount = new List<PowerItemAmount>();
 
}
[Serializable]
public class WeaponAmmoAmount
{
    public int id;

    public WeaponItem item;

    public float amount;

    public bool status;
}
[Serializable]
public class PowerItemAmount
{
    public int id;

    public PowerItem item;

    public PowerType typePower;

    public float amount;
}
[Serializable]
public class SupportItemAmount
{
    public int id;

    public SupportItem item;

    public float amount;
}

public class SoundSettings
{
    public float bgmVolume;

    public float sfxVolume;
}
