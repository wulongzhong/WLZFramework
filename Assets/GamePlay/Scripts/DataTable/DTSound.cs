using System.Collections.Generic;
namespace ConfigPB
{
    public sealed partial class Sound
    {
        public void InitCustom()
        {
        }
    }
}

public class DTSound
{
    public static DTSound Instance;

    public Dictionary<int, ConfigPB.Sound> dicSounds;

    public DTSound(ConfigPB.Table table)
    {
        Instance = this;
        dicSounds = new Dictionary<int, ConfigPB.Sound>();
        foreach (var item in table.Sound)
        {
            item.InitCustom();
            dicSounds.Add(item.Id, item);
        }
    }

    public ConfigPB.Sound GetSoundById(int id)
    {
        return dicSounds[id];
    }
}
