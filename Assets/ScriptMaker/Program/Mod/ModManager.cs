using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ScriptMaker.Util;
using UnityEngine;

namespace ScriptMaker.Program.Mod
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ModAttribute : Attribute
    {
        public string Name { get; }

        public ModAttribute(string name)
        {
            Name = name;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class AddDependencyAttribute : Attribute
    {
        public string Dependency { get; }

        public AddDependencyAttribute(string name)
        {
            Dependency = name;
        }
    }

    public class ModLoadingException : Exception
    {
        public ModLoadingException(string message): base(message) {}
        public ModLoadingException(string message, Exception e): base(message, e) {}
    }
    
    public static class ModManager
    {
        internal static List<object> loadedMods = new();
        internal static void LoadMods()
        {
            var modDir = Path.Combine(Directory.GetCurrentDirectory(), "mods");
            if (!Directory.Exists(modDir))
                Directory.CreateDirectory(modDir);

            Log.Info("Loading assemblies");
            var mods = new Dictionary<string, (Type mod, ModAttribute attribute)>(); 
            foreach (var assembly in Directory.GetFiles(modDir)
                         .Where(x => x.EndsWith(".dll")).Select(Assembly.LoadFile))
            {
                Log.Info($"Loading assembly: {assembly.GetName().Name}");
                foreach(var m in assembly.GetTypes().Where(x =>
                        Attribute.GetCustomAttribute(x, typeof(ModAttribute)) is not null)
                    .Select(x => (mod: x, attribute: Attribute.GetCustomAttribute(x, typeof(ModAttribute)) as ModAttribute)))
                {
                    if (mods.ContainsKey(m.attribute.Name))
                        throw new ModLoadingException($"Mod name duplicated: {m.attribute.Name}");
                    mods[m.attribute.Name] = m;
                }
            }
            
            Log.Info("Resolving dependencies");
            var deps = new Dictionary<string, List<string>>();
            var leftDeps = new Dictionary<string, int>();

            foreach (var mod in mods)
            {
                var dd = mod.Value.mod.GetCustomAttributes(typeof(AddDependencyAttribute))
                    .Select(x => x as AddDependencyAttribute);
                leftDeps[mod.Key] = dd.Count();
                foreach (var d in dd)
                {
                    if (deps.ContainsKey(d.Dependency)) deps[d.Dependency].Add(mod.Key);
                    else deps[d.Dependency] = new List<string> { mod.Key };
                }
            }

            Log.Info("Constructing mods");
            while (leftDeps.Count != 0)
            {
                var k = leftDeps.First(x => x.Value == 0);
                var modName = k.Key;
                if(deps.ContainsKey(modName))
                    foreach (var x in deps[modName])
                        leftDeps[x]--;
                leftDeps.Remove(modName);
                
                Log.Info("Loading mod: " + modName);
                try
                {
                    loadedMods.Add(Activator.CreateInstance(mods[modName].mod));
                }
                catch (Exception e)
                {
                    Log.Error(new ModLoadingException($"Exception thrown on initializing mod: {modName}", e));
                }
            }
        }
    }
}