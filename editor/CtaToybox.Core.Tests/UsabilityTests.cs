using System.IO;
using System.Linq;
using CtaToybox.Core;

// 화면을 읽을 수 있게 하는 코어 기능: 게임 현지화의 이름, 모드 이름, 종류 이름, 줄의 문맥, 매크로 호출 줄, 읽히는 키 표기
static class UsabilityTests
{
    const string Pot = "msgid \"\"\nmsgstr \"\"\n\"Language: en\n\"\n\n;// comment\nmsgctxt \"desc/entity/a\"\nmsgid \"<c(B29557)>Tank</c> A\"\nmsgstr \"\"\n\nmsgctxt \"desc/entity/long\"\nmsgid \"\"\n\"Two \"\n\"lines\"\nmsgstr \"\"\n";

    public static void Run()
    {
        // 현지화 파일(.pot) 읽기: 색 태그를 뗀다, 여러 줄 msgid를 잇는다
        var names = new DisplayNames();
        names.AddPot(Pot);
        T.Eq("Tank A", names.OfUnit("entity/-vehicle/x/a/a.def"), "color tags are removed");
        T.Eq("Two lines", names.OfUnit("entity/-vehicle/x/long/long.def"), "multi-line msgid is joined");
        names.AddPot("msgctxt \"desc/human/mp/usa/late/tankman\"\nmsgid \"Tankman [US]\"\nmsgstr \"\"\n\n" +
                     "msgctxt \"desc/weapon/m3_grease\"\nmsgid \"M3 Grease Gun\"\nmsgstr \"\"\n\n" +
                     "msgctxt \"desc/ammo/bulletrus_85 aphebc\"\nmsgid \"85-mm APHEBC\"\nmsgstr \"\"\n\n" +
                     "msgctxt \"desc/ammo/m3_grease_mag\"\nmsgid \"Grease Mag\"\nmsgstr \"\"\n");
        T.Eq("Tankman [US]", names.OfUnit("set/breed/mp/usa/late/tankman.set"), "a breed name is desc/human/<path>");
        T.Eq("M3 Grease Gun", names.OfUnit("set/stuff/smg/m3_grease"), "a weapon name is desc/weapon/<file>");
        T.Eq("85-mm APHEBC", names.OfUnit("set/stuff/shell/bulletrus_85.aphebc.ammo"), "an ammo name uses the file words without the extension");
        T.Eq("Grease Mag", names.OfUnit("set/stuff/smg/m3_grease_mag.ammo"), "a magazine");
        T.Eq("Tank A (a_xx)", names.OfUnit("entity/-vehicle/x/x/a_xx/a_xx.def"), "an _x/_xx file borrows the base name and shows its own file name");
        T.True(names.OfUnit("properties/tank.ext") == null, "other files have no name");

        // 게임과 모드의 현지화: 바닐라 pak, 모드 pak, 모드 폴더. 뒤에 로드되는 쪽이 이긴다
        string game = T.TempDir(), workshop = T.TempDir(), work = T.TempDir(), outDir = T.TempDir();
        T.Zip(Path.Combine(game, "resource", "entity", "-vehicle.pak"),
            ("-vehicle/x/a/a.def", "{game_entity\n {volume \"body1\" {thickness 45 {front 60}}}\n {volume \"turret\" {thickness 100}}\n (\"mob\" speed(40))\n {half (/ 10 4)}\n}"),
            ("-vehicle/x/b/b.def", "{game_entity {mass 1}}"),
            ("-vehicle/x/c/c.def", "{game_entity {mass 2}}"));
        T.Zip(Path.Combine(game, "resource", "gamelogic.pak"), ("entity/-vehicle/.presets", "(define \"mob\" {chassis {locomotion {maxspeed %speed}}})"));
        T.Zip(Path.Combine(game, "localizations", "default.pak"),
            ("default/interface/text/desc/v.pot", "msgctxt \"desc/entity/a\"\nmsgid \"Vanilla A\"\nmsgstr \"\"\n\nmsgctxt \"desc/entity/b\"\nmsgid \"Vanilla B\"\nmsgstr \"\"\n"));
        T.Put(Path.Combine(workshop, "111"), "mod.info", "{mod\n\t{name \"Test Mod\"}\n\t{desc \"x\"}\n}");
        T.Put(Path.Combine(workshop, "111", "resource"), "entity/-vehicle/x/b/b.def", "{game_entity {mass 5}}");
        T.Zip(Path.Combine(workshop, "111", "localizations", "default.pak"), ("default/interface/text/desc/m.pot", "msgctxt \"desc/entity/a\"\nmsgid \"Mod A\"\nmsgstr \"\"\n"));
        T.Put(Path.Combine(workshop, "222"), "mod.info", "{mod {name \"Folder Mod\"}}");
        T.Put(Path.Combine(workshop, "222"), "localizations/default/interface/text/desc/f.pot", "msgctxt \"desc/entity/c\"\nmsgid \"Folder C\"\nmsgstr \"\"\n");
        T.Put(Path.Combine(workshop, "222", "resource"), ".keep", "");
        string options = Path.Combine(work, "options.set");
        File.WriteAllText(options, "{options {mods \"mod_111:0\" \"mod_222:0\" \"" + Path.GetFileName(outDir) + ":0\"}}");
        string settings = Path.Combine(work, "settings.json");
        new EditorSettings { GameDir = game, WorkshopDir = workshop, OptionsSetPath = options, OutputModDir = outDir, OutputModName = Path.GetFileName(outDir) }.Save(settings);

        using var session = EditorSession.Open(settings, T.TempDir(), null);
        T.Eq("Mod A", session.Names.OfUnit("entity/-vehicle/x/a/a.def"), "a later mod's localization wins");
        T.Eq("Vanilla B", session.Names.OfUnit("entity/-vehicle/x/b/b.def"), "vanilla names are read from the game's localization pak");
        T.Eq("Folder C", session.Names.OfUnit("entity/-vehicle/x/c/c.def"), "a mod's localization folder is read too");

        // 모드 이름과 종류 이름
        T.Eq("바닐라", session.LayerLabel("vanilla"), "vanilla label");
        T.Eq("Test Mod", session.LayerLabel("mod_111"), "a workshop mod's name from mod.info");
        T.Eq("Folder Mod", session.LayerLabel("mod_222"), "another mod");
        T.Eq("mod_999", session.LayerLabel("mod_999"), "an unknown layer keeps its id");
        T.Eq("차량", UnitKind.Label("vehicle"), "vehicle label");
        T.Eq("보병", UnitKind.Label("breed"), "breed label");
        T.Eq("무기·탄약·장비", UnitKind.Label("stuff"), "stuff label");
        T.Eq("파일", UnitKind.Label(null), "a non-unit file");

        // 검색은 이름순: 이름 있는 유닛이 먼저, 이름이 같으면 경로순
        T.Eq(new[] { "entity/-vehicle/x/c/c.def", "entity/-vehicle/x/a/a.def", "entity/-vehicle/x/b/b.def" },
            session.Search(null, "vehicle", null).Select(u => u.Path).ToArray(), "search results are sorted by display name");

        // 줄의 문맥: 같은 키가 여러 번 나오면 어느 볼륨·부품의 것인지
        var a = session.OpenUnit("entity/-vehicle/x/a/a.def");
        var front = a.Rows.Single(r => r.Path == "/game_entity/volume/thickness/front");
        T.Eq("volume body1", front.Context, "a nested key gets the nearest named parent");
        var thick = a.Rows.Where(r => r.Path == "/game_entity/volume/thickness").Select(r => r.Context).ToArray();
        T.Eq(new[] { "volume body1", "volume turret" }, thick, "repeated keys get their own parent's name");
        T.True(a.Rows.Single(r => r.Path == "/game_entity/volume" && r.Occurrence == 0).Context == null, "a top-level named row has no context of its own");


        // 매크로 호출 줄: 계산에 쓰여 펼친 값으로는 못 고치는 인자도 여기서 고친다
        var call = a.Rows.Single(r => r.IsMacroCall);
        T.Eq("/game_entity/(\"mob\")", call.Path, "the macro call row of the unit file");
        T.Eq("40", call.Values[0].Trace.Text, "its argument");
        T.True(call.Values[0].Editable, "is editable");
        T.Eq("entity/-vehicle/x/a/a.def", call.Values[0].Trace.File, "in the unit file");

        // 수정 검사는 그 줄의 값 전체를 함께 준다(수정 목록에 어느 줄인지 보이려고)
        session.OpenProject(Path.Combine(work, "p.json"));
        var front60 = front.Values[0];
        var edit = session.AddEdit(a, front60, "70", "shared");
        T.Eq("60", session.CheckEdits().Single(c => c.Edit == edit).RowText, "the check carries the whole row's values");

        // 읽히는 키 표기
        T.Eq("volume › thickness › front", KeyLabel.Readable("/game_entity/volume/thickness/front"), "the root block is dropped and segments are joined");
        T.Eq("inventory › box › item", KeyLabel.Readable("/game_entity/extender[inventory]/box/item"), "extender[x] reads as x");
        T.Eq("fire rate (매크로)", KeyLabel.Readable("/from/(\"fire rate\")"), "a macro call reads as its name");
        T.Eq("perks › *", KeyLabel.Readable("/breed/perks/\"*\""), "quoted-name blocks read as *");
        T.Eq("mobility › speed()", KeyLabel.Readable("/a/(\"mobility\")/speed()").Replace(" (매크로)", ""), "call arguments keep their name");
    }
}
