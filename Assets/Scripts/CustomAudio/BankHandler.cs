using System;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEditor;
using UnityEngine;

public static class BankHandler
{
    private const string BankExtension = ".bank";
    private const string StringBankExtension = ".strings.bank";
    private const string BankPrefix = "bank:/";

    public struct BankData
    {
        public string Name;
        public Bank Bank;
        public Bus[] Buses;
    }
    
    public static Dictionary<string, BankData> BankLookup = new Dictionary<string, BankData>();
    
    public static void LoadBank(string bankName, bool loadSamples = false) //Laddar bank, om master laddas också string bank
    {
        RuntimeManager.LoadBank(bankName + BankExtension, loadSamples);
        
        RuntimeManager.StudioSystem.getBank(BankPrefix + bankName, out var bank);
        bank.getBusList(out var buses);
        BankLookup.TryAdd(bankName, new BankData { Name = bankName, Bank = bank, Buses = buses });
        
        if (bankName == "Master") RuntimeManager.LoadBank(bankName + StringBankExtension, loadSamples);
        AudioDebug.Print("Loading " + bankName + BankExtension);
    }

    public static void UnloadBank(string bankName) //Unloadar en bank
    {
        BankLookup.Remove(bankName);
        
        RuntimeManager.UnloadBank(bankName + BankExtension);
        
        if (bankName == "Master") RuntimeManager.UnloadBank(bankName + StringBankExtension);
        AudioDebug.Print("Unloading " + bankName + BankExtension);
    }
}
