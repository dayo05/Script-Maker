# How to create new block in this program:
1. 프로젝트 생성 후 다음 의존성을 삽입합니다. 해당 파일은 모두 다운받은 프로그램 어딘가에서 찾을 수 있습니다.
 * Assembly-CSharp.dll
 * UnityEngine.CoreModule.dll
 * UnityEngine.UI.dll (optional)
2. Class library 형태의 프로젝트를 생성합니다.
3. csproj파일의 TargetFramework를 netstandard2.1로 변경합니다.
4. ModBase를 상속받는 클래스를 하나 만듭니다. 그 이후 그 클래스에 Mod Attribute를 삽입합니다.

예시
```cs
using ScriptMaker.Entry.Block;
using ScriptMaker.Util;
namespace ExampleMod {
    [Mod]
    public class MyMod: ModBase {
        public override string Name => "MyMod";
        public override void OnLoad() {
            Log.Info("Initialized!!");
        }
    }
}
```
나머지는 자동으로 해줍니다. 추가적인 예시는 CoreBlocks, VideoMod 등이 있습니다.

Minecraft Mod Side-implementation: 
1. Inheritance me.ddayo.customscript.client.script.blocks.BlockBase
2. Register your custom block on me.ddayo.customscript.client.script.blocks.BlockBase
Name of block will be name of your block class in Unity. And also, name of context will be name of your context class in Unity.
