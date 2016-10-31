﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public partial class ManagerContainer : MonoBehaviour
{
    static Dictionary<Scene, ManagerContainer> s_managerContainers = new Dictionary<Scene, ManagerContainer>();
    static ManagerContainer s_globalContainer;

    Manager LookupManager(System.Type managerType)
    {
        Manager result = null;
        m_managerLookup.TryGetValue(managerType, out result);
        return result;
    }

    public T GetManager<T>() where T:Manager 
    {
        var manager = LookupManager(typeof(T)) as T;

        if(manager==null && !isGlobalContainer && s_globalContainer!=null)
        {
            manager = s_globalContainer.LookupManager(typeof(T)) as T;
        }

        if(manager==null)
        {
            manager = AutoconstructManager(typeof(T)) as T;
        }

        return manager;
    }

    public static T GetManager<T>(Scene scene) where T:Manager 
    {
        T manager = null;
        ManagerContainer sceneContainer = null;
        if(!s_managerContainers.TryGetValue(scene, out sceneContainer)) 
        {
            var gameObject = new GameObject(scene.name+" ManagerContainer (autogenerated)");
            sceneContainer = gameObject.AddComponent<ManagerContainer>();
        }
        manager = sceneContainer.LookupManager(typeof(T)) as T;

        if(manager==null && s_globalContainer!=null) 
        {
            manager = s_globalContainer.LookupManager(typeof(T)) as T; 
        }

        if(manager==null && sceneContainer!=null)
        {
            manager = sceneContainer.AutoconstructManager(typeof(T)) as T;
        }

        return manager;
    }

    public static T GetGlobalManager<T>() where T:Manager 
    {
        T manager = null;
        if(s_globalContainer==null) 
        {
            var gameObject = new GameObject("Global ManagerContainer (autogenerated)");
            DontDestroyOnLoad(gameObject);
            s_globalContainer = gameObject.AddComponent<ManagerContainer>();
        }

        manager = s_globalContainer.GetManager<T>();
        return manager;
    }
}