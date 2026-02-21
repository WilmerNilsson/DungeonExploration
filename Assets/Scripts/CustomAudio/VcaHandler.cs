using System.Collections.Generic;
using CustomAudio;
using FMOD.Studio;
using UnityEngine;

public class VcaHandler
{
    public Dictionary<string, VCA> VcaLookup = new Dictionary<string, VCA>();

    public void Initialize()
    {
        VcaLookup.Clear();
        if (AudioSystem.instance.BankHandler.BankLookup.TryGetValue("Master", out var masterBankData))
        {
            masterBankData.Bank.getVCAList(out var vcaList);
            foreach (var vca in vcaList)
            {
                vca.getPath(out var path);
                var split = path.Split('/');
                VcaLookup.Add(split[^1], vca);
            }
            return;
        }
        AudioDebug.Print("Failed to get master bank", true);
    }

    public void SetVolume(string vcaName, float volume)
    {
        if (VcaLookup.TryGetValue(vcaName, out var vca))
        {
            vca.setVolume(volume);
        }
    }

    public bool TryGetVolume(string vcaName, out float volume)
    {
        if (VcaLookup.TryGetValue(vcaName, out var vca))
        {
            vca.getVolume(out volume);
            return true;
        }
        volume = 0;
        return false;
    }
}
