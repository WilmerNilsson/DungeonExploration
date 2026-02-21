using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;

public static class VcaHandler
{
    public static Dictionary<string, VCA> VcaLookup = new Dictionary<string, VCA>();

    public static void Initialize()
    {
        VcaLookup.Clear();
        if (!BankHandler.BankLookup.TryGetValue("Master", out var masterBankData))
        {
            AudioDebug.Print("Failed to get master bank", true);
            return;
        }
        masterBankData.Bank.getVCAList(out var vcaList);
        foreach (var vca in vcaList)
        {
            vca.getPath(out var path);
            var split = path.Split('/');
            VcaLookup.Add(split[^1], vca);
        }
    }

    public static void SetVolume(string vcaName, float volume)
    {
        if (VcaLookup.TryGetValue(vcaName, out var vca))
        {
            vca.setVolume(volume);
        }
    }

    public static bool TryGetVolume(string vcaName, out float volume)
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
