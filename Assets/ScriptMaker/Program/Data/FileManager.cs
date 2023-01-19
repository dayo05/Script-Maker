using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ScriptMaker.Entry;
using ScriptMaker.Entry.Arrow;
using ScriptMaker.Entry.Arrow.Contexts;
using ScriptMaker.Entry.Block;
using ScriptMaker.Program.UI;
using ScriptMaker.Util;
using SFB;
using UnityEngine;

namespace ScriptMaker.Program.Data
{
    public static class FileManager
    {
        private static string currentFile = null;
        private static GameObject MainCamera;

        public static void Initialize()
        {
            MainCamera = GameObject.Find("Main Camera");
        }
        
        public static void LoadFile()
        {
            try
            {
                var path = StandaloneFileBrowser.OpenFilePanel("Open file", "",
                    new[] {new ExtensionFilter("Script file", "sc", "json")}, false);
                if (path.Length == 0) return;
                if (string.IsNullOrWhiteSpace(path[0])) return;
                currentFile = path[0];
                Log.Info($"Loading file: {currentFile}");

                using var sr = new StreamReader(currentFile);
                var options = new List<Option>();
                var prv = -1;
                var line = 0;
                foreach (var c in sr.ReadToEnd().Split('\n').Select(x => x.TrimStart()))
                {
                    line++;
                    if (c == string.Empty) continue;
                    if (c[0] == '#') continue;
                    var (d, cmd) = CountStartRArrow(c);
                    if(cmd == string.Empty) continue;
                    if (d > prv + 1) throw new InvalidDataException("Compile error on line " + line);

                    var o = options;
                    for (var i = 0; i < d; i++)
                        o = o.Last().subOptions;

                    var splitPos = cmd.IndexOf('=');
                    o.Add(new Option(cmd[..splitPos], cmd[(splitPos + 1)..]));
                    prv = d;
                }

                {
                    var cameraOptions = options.Last(x => x.Key == "Option" && x.Value == "Camera");
                    MainCamera.transform.position = new Vector3(cameraOptions["X"].Float(),
                        cameraOptions["Y"].Float(), MainCamera.transform.position.z);

                    ScriptSettings.CanMovePrevious =
                        options.LastOrDefault(x => x.Key == "CanMovePrevious")?.Bool ?? false;
                }

                {
                    EditorMain.currentNS = long.Parse(options.Last(x => x.Key == "NS").Value);
                    BlockHandler.Blocks.Select(x => x.Key).ToList().ForEach(BaseEntry.DeleteContent);
                    ArrowHandler.Arrows.Select(x => x.Key).ToList().ForEach(BaseEntry.DeleteContent);

                    foreach (var blockOption in options.Where(x => x.Key == "Block"))
                        BlockHandler.CreateEntryByOption(blockOption);
                    
                    foreach(var arrowOption in options.Where(x => x.Key == "Arrow"))
                        ArrowHandler.CreateArrowUnsafe(arrowOption["NS"].Long(), (ArrowContext)new ArrowContext().ReadOption(arrowOption["Context"].First()));
                }

                (int, string) CountStartRArrow(string s)
                {
                    for(var i = 0; i < s.Length; i++)
                        if (s[i] != '>')
                            return (i, s[i..]);
                    return (s.Length, string.Empty);
                }
            }
            catch (Exception e)
            {
                DialogGui.DisplayDialog(e.Message + "\nTrying to quit.\n" + e.StackTrace, DialogType.Ok, _ => Application.Quit());
            }
        }

        public static void SaveFile(Action callback = null)
        {
            try
            {
                if (currentFile is null)
                    while (true)
                    {
                        try
                        {
                            var path = StandaloneFileBrowser.SaveFilePanel("Save script", "", "",
                                new[]
                                {
                                    new ExtensionFilter("Script file", "sc"), new ExtensionFilter("Json file", "json")
                                });
                            if (path is null or "") return;
                            currentFile = path;
                            break;
                        }
                        catch (Exception)
                        {
                            //ignore
                        }
                    }

                var optList = new List<Option>
                {
                    Option("Version", 0.8),
                    Option("Option", "Camera")
                        .Append("X", MainCamera.transform.position.x)
                        .Append("Y", MainCamera.transform.position.y),
                    Option("NS", EditorMain.currentNS),
                    Option("CanMovePrevious", ScriptSettings.CanMovePrevious)
                };

                optList.AddRange(BlockHandler.Blocks.Values.Select(block => block.Serialize()));

                optList.AddRange(ArrowHandler.Arrows.Values.Select(arrow => arrow.Serialize()));
                
                //Save to currentFile
                using var sw = new StreamWriter(currentFile);
                foreach(var o in optList)
                    sw.WriteLine(o.Export());

                callback?.Invoke();

                Option Option(string key, object value)
                    => new(key, value);
            }
            catch (Exception e)
            {
                DialogGui.DisplayDialog(e.Message);
            }
        }
    }
}