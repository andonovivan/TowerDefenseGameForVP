using UnityEngine;

public class Singleton<T> : MonoBehaviour where T: MonoBehaviour{

    private static T instance;

    public static T Instance
    {
        get {
            // Check if instance already exists
            if (instance == null)
                // if not, set instance to of type T
                instance = FindObjectOfType<T>();
               else if (instance != FindObjectOfType<T>())
                // if instance already exists and it's not this:
                Destroy(FindObjectOfType<T>());

            // Sets this to not be destroyed when reloading scene
            DontDestroyOnLoad(FindObjectOfType<T>());
               return instance;
            }
    }
}
