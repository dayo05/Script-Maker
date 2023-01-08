# Script Maker
* **Important: 아직 이 프로그램은 M1 Native를 지원하지 않습니다. M1 이용자분들은 로제타 사용해주세요.**
* **Minecraft 1.12.2버전 사용시 최하단의 정보를 반드시 확인해주세요.**
## How to use
* 이 프로그램은 Minecraft mod Custom Script를 위한 Editor입니다.
1. 파일을 다운로드 해주세요. [다운로드 링크](TODO)
 * Windows의 경우 ScriptMaker-vX.X-win.zip을, MacOS의 경우 ScriptMaker-vX.X-mac.zip을 받아주세요. Linux의 경우 호환은 되나, 직접 빌드해주세요.~~빌드하기 귀찮아요~~
 * 다운받은 파일은 압축을 풀어주세요.
2. 원하는 Mod를 다운로드 해주세요. Custom Script를 별도의 모드를 활용하여 확장할 경우, 에디터에도 해당 기능을 사용하기 위한 추가적인 모드를 추가할 것을 권장합니다.
 * 제가 관리하는 CoreBlocks 모드와 Video 모드는 각각의 Release page에서 다운 가능합니다. [CoreBlocks 다운로드](TODO) [Video 다운로드](TODO)
 * 다운받은 모드는, 다음 폴더로 옮겨주세요. 마인크래프트 모드 적용하는거랑 비슷하다고 생각하시면 됩니다!
 > Windows: 다운받은 exe가 있는 폴더에 mods라는 이름의 폴더 생성 후 그 폴더 안에 넣어주세요.
 > Mac OS: 다운받은 .app가 있는 폴더에 mods라는 이름의 폴더 생성 후 그 폴더 안에 넣어주세요.
3. 프로그램을 실행해주세요.
 * 다음 화면이 나오면 성공입니다.(해상도 차이로 인해 버튼들의 크기는 달라질 수 있습니다!)
<img width="1512" alt="image" src="https://user-images.githubusercontent.com/77111795/211200224-1ac74bc9-3df2-4c84-8b78-d4a67243bdcb.png">
 * 이 화면에는 후술할 기본 Begin Block이 생성되있습니다.
 
## Tutorial

Script Maker은 누구나 Minecraft 상에서 GUI를 쉽게 구현할 수 있게 하기 위해 제작되었습니다. 이를 위하여 블럭과 화살표를 이용한 프로그래밍과 포토샵의 레이어와 유사한 렌더링 시스템을 활용합니다.

### 블럭 만들기

* 블럭은 화면에 무언가를 그리거나, 특정 조건까지 기다리는 등 실제 동작을 수행합니다.
1. 화면 좌상단의 +버튼을 누릅니다.
2. +버튼 바로 오른쪽에 나온 두개의 아이콘중 좌측의 블럭을 누릅니다.
3. 그러면 나오는 창의 이름은 편집 창이라고 합니다. 이 편집 창의 좌상단의 x아이콘 바로 우측의 드롭다운 박스에서 블럭의 종류를 선택합니다.
4. 블럭마다 있는 옵션을 전부 고른 다음에 창 우하단의 Apply 버튼을 누르면 블럭이 생성됩니다.

### 화살표 만들기

* 화살표는 블럭간의 관계를 표현합니다. 대표적으로, 블럭의 실행 순서가 있습니다.
1. 화면 좌상단의 +버튼을 누릅니다.
2. +버튼 바로 오른쪽에 나온 두개의 아이콘중 우측의 화살표를 누릅니다.
3. 시작 블럭과 끝 블럭을 차례대로 누릅니다. 취소하고싶다면, esc를 누르면 됩니다.

### 기본 블럭 정보: Begin Block편

* Begin Block은 아무런 모드가 없더라도 존재하는 기본 블럭입니다.(기타 텍스트 표시 등은 CoreBlocks 등의 모드에 있습니다. 편의상, 그리고 모드 제작 예시로 보여드리기 위해 분리시켜놨어요)
* Begin Block은 스크립트의 시작점을 정의합니다.
* Begin Block의 편집 창에서 수정 가능한 항목은 하나입니다. 이 항목은 스크립트의 시작점에 대한 추가적인 정보를 제공합니다.
> 예시로는, default를 입력하는 경우 스크립트를 gui로 실행시 처음 시작점으로 사용되며, hud로 입력시 스크립트를 hud로 실행시 처음 시작점으로 사용됩니다.
* 이 값은 한 스크립트 내에서 중복될 수 없습니다.
<img width="1512" alt="image" src="https://user-images.githubusercontent.com/77111795/211201085-520cff64-7b81-4a20-968e-4487eac0f4c9.png">

### 불러오기 및 저장

* 저장버튼은 좌상단의 아이콘중 위에서 3번째, 불러오기 버튼은 좌상단의 아이콘중 2번째에 위치해 있습니다.
* 이 버튼을 클릭할 경우 각 OS별로 기본 파일 저장/불러오기 화면이 나옵니다.
* 4번째 버튼은 새로운 창을 띄울때 쓰는 버튼입니다.

## Technical Information(기술적인 기본 정보)

### [Script Maker Mod 제작 방법](https://github.com/dayo05/Script-Maker/blob/master/modding.md)
* [예시: CoreBlocks](https://github.com/dayo05/ScriptMakerCoreBlocks)
* [예시: VideoMod](https://github.com/dayo05/ScriptMakerVideoMod)
### Namespace별 간략 설명
* ScriptMaker => Base Namespace
* ScriptMaker.Entry => 블럭, 화살표 등 구성품
* ScriptMaker.Entry.Arrow => 화살표
* ScriptMaker.Entry.Arrow.Contexts => 화살표 데이터
* ScriptMaker.Entry.Block => 블럭
* ScriptMaker.Entry.Block.Contexts => 블럭 데이터
* ScriptMaker.Program => 메인 에디터
* ScriptMaker.Program.Data => IO관련
* ScriptMaker.Program.Mod => Mod loading and events
* ScriptMaker.Program.UI => UI
* ScriptMaker.Program.UI.RightClickMenu => 우클릭시 나오는 메뉴 관련
### M1 Native 미지원 이유
* 이 프로그램은 [StandaloneFileBrowser](https://github.com/gkngkc/UnityStandaloneFileBrowser)을 사용하는데, 이 API가 M1을 지원을 안해요... 대체품 찾아주시면 적용하겠습니다!
### Minecraft 1.12.2 사용시의 경고
* 이 프로그램은 가내수공업팀 내부에서 사용될 목적으로 최초 제작되었으며, 팀장님 ok받고서 배포하게 되었습니다.
* 이 프로그램의 0.1(최초)버전은 4일만에 제작되었으며, 코드는 엄청 엉망진창이였습니다. 결국 0.5 -> 0.6으로 이동하면서 코드를 전부 갈아엎었고, 그러면서 기존 0.5버전의 Script Maker을 기반으로 하는 Custom Script 1.12.2버전의 유지보수도 포기했습니다.
* 원하신다면, 0.6+ 버전을 1.12.2로 포팅하셔도 됩니다. 전 막지 않습니다. 원하신다면, 공식으로도 올려드릴 수 있습니다. 그렇지만, 제가 그걸 하기를 바라지 마세요.
* **0.5버전의 Custom Script는 중대한 보안 이슈가 있으니, 신뢰하지 못하는 사람이 접속할 수 있는 서버의 경우 절.대.로. 사용하지 마세요. MIT라이센스에 따라 저는 이러한 부분에서 면책입니다. 컨텐츠 테스트시에도 이 버전의 Custom Script를 사용한다면, 모든 서버 파일을 백업해 두시고, 신뢰되지 않는 코드가 실행되지 않도록 최선을 다해주세요...**
* 별 문제 없으면 1.16.5 혹은 1.18.2버전의 Custom Script를 사용해주세요.
* Script-Maker 0.5의 소스 코드는 Dayo#3334로 연락 주시면 압축해서 보내드릴수도 있습니다.(제 컴퓨터에 남아있을지 장담하지 못해요)
