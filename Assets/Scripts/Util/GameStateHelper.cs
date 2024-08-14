using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GameStateHelper : MonoBehaviour
{
    public static GameStateHelper Instance { get; private set;}
    string fileName = "Card2CardGame.json";

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    private string GetFilePath()
    {
        return Path.Combine(Application.persistentDataPath, fileName);
    }
    private bool FileExits(string path) => File.Exists(path);
    public bool StateExits()
    {
        string path = GetFilePath();
        return File.Exists(path);
    }
    public bool LoadState(out GameStateStructure gameState)
    {
        string path = GetFilePath();
        gameState = null;
        if (!FileExits(path)) return false;

        string content = File.ReadAllText(path);
        gameState = JsonUtility.FromJson<GameStateStructure>(content);
        return true;
    }
    public void SaveState(GameStateStructure gameState)
    {
        string path = GetFilePath();

        string content = JsonUtility.ToJson(gameState);
        File.WriteAllText(path,content);
    }
    public bool DeleteState()
    {
        string path = GetFilePath();

        if (!FileExits(path)) return false;

        File.Delete(path);
        return true;
    }
}
