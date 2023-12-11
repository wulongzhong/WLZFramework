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

    public Dictionary<string, ConfigPB.Sound> dicSounds;

    public DTSound(ConfigPB.Table table)
    {
        Instance = this;
        dicSounds = new Dictionary<string, ConfigPB.Sound>();
        foreach (var item in table.Sound)
        {
            item.InitCustom();
            dicSounds.Add(item.Id, item);
        }
    }

    public ConfigPB.Sound GetSoundById(string id)
    {
        return dicSounds[id];
    }
}
