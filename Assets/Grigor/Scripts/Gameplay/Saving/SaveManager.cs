using System;
using CardboardCore.DI;
using CardboardCore.Utilities;
using UnityEngine;

namespace Grigor.Gameplay.Saving
{
    [Injectable]
    public class SaveManager : MonoBehaviour
    {
        public void Save<T>(string key, T value)
        {
            try
            {
                ES3.Save(key, value);
            }
            catch (Exception e)
            {
                Log.Write(e);
            }
        }

        public T Load<T>(string key)
        {
            if (!ES3.KeyExists(key))
            {
                Log.Write($"Key {key} does not exist in the save file!");
                return default;
            }

            return ES3.Load<T>(key);
        }

        public void LoadInto<T>(string key, T value) where T : class
        {
            if (!ES3.KeyExists(key))
            {
                Log.Write($"Key {key} does not exist in the save file!");
                return;
            }

            ES3.LoadInto(key, value);
        }
    }
}
