﻿using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameSaving : MonoBehaviour
{
    [SerializeField] string m_fileName;
    public List<DataSaving> SaveData { get; set; } = new List<DataSaving>();
    public Action OnGameSaving { get; set; }

    public void Save()
    {
        OnGameSaving?.Invoke();

        string path = GetSaveFilePath();
        FileStream fileStream = new FileStream(path, FileMode.Create);
        using StreamWriter writer = new StreamWriter(fileStream);

        WriteSaveData(writer);
        writer.Close();
    }

    void WriteSaveData(StreamWriter writer)
    {
        foreach (var dataHandler in SaveData)
        {
            dataHandler.Save();
            writer.WriteLine(dataHandler.ToJson());
        }
    }

    public string GetSaveFilePath()
    {
        return Application.persistentDataPath + "/" + m_fileName;
    }
}
