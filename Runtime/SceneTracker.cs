using UnityEngine;
using UnityEngine.SceneManagement;
using RuntimeDebugger.Commands;

public static class SceneTracker
{
    static Scene _currentScene;
    static int _previousObjectCount = 0;
    static GameObject[] _currentGameObjects;

    public static void GetCurrentScene()
    {
        _currentScene = SceneManager.GetActiveScene();
        _previousObjectCount = _currentScene.rootCount;
    }

    public static void UpdateScene()
    {
        //Initialize
        if(_currentScene == null)
            return;
            
        //Check if the current transform count is equal to the last
        if(_currentScene.rootCount > _previousObjectCount)
        {
            _currentGameObjects = new GameObject[_currentScene.rootCount];
            _currentGameObjects = _currentScene.GetRootGameObjects();
            GameObject newGameObject = _currentGameObjects[_currentGameObjects.Length - 1];
            CommandManager.CopyCommand(newGameObject);
            //Adds one object to the count to keep updating
            ++_previousObjectCount;
        }
    }
}