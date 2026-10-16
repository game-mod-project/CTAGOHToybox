// 화면 확인 시나리오(--scenario <파일>). 사람이 하는 조작을 차례로 하고 그 사이 화면을 찍는다.
// 반드시 --project로 임시 프로젝트를 준다(사용자의 수정 목록을 건드리지 않게).
// 한 줄에 명령 하나. #으로 시작하면 주석이다.
//   unit <가상 경로>          유닛을 연다
//   tab summary|full|roster|items
//   filter <글자>             전체 탭 거르기
//   open <글자>               값 고치기 창을 연다(지금 탭의 줄 가운데 이름·키·값에 글자가 든 첫 줄)
//   type <n> <글자>           창의 n번째 값 칸에 쓴다(1부터)
//   piece <n> <조각 이름> <글자>  창의 n번째 값이 문자열 속 설정이면 그 조각 칸에 쓴다
//   scope unit|shared         창의 적용 범위
//   ok                        "수정 목록에 넣기"를 누른다(꺼져 있으면 기록만 남긴다)
//   cancel                    창을 닫는다
//   openblock <이름>          전체 탭에서 그 이름의 따옴표 이름 블록 줄(복제할 수 있는 줄)을 연다(E18)
//   copyblock <새 이름>       열린 창(따옴표 이름 블록 줄)에서 그 블록을 새 이름으로 복제한다(E18)
//   toggle <n>                수정 목록 n번째의 체크를 바꾼다
//   remove <n>                수정 목록 n번째를 지운다
//   group <목록> <글자> open|close  summary|full|roster 목록에서 이름에 글자가 든 첫 묶음을 펼치거나 접는다
//   scroll <목록>             그 목록을 맨 아래로 내린다
//   rule <종류>|<경로 글자>|<키>|<값 자리>|<연산>|<값>  일괄 규칙 창을 채워 연다(F1). 경로 글자는 쉼표로 나누고 비울 수 있다. 값 자리 -1은 수인 값 모두
//   rulerow <글자>            지금 탭에서 이름·키·값에 글자가 든 첫 줄로 '이 키로 일괄 규칙...'을 연다(오른쪽 단추 메뉴와 같다)
//   rulepreview               규칙 창의 '미리 보기'를 누른다
//   ruleok                    규칙 창의 '수정 목록에 넣기'를 누른다
//   shot <이름>               창이 열려 있으면 창을, 아니면 주 창을 찍는다(<이름>.png)
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

namespace CtaToybox.App
{
    public partial class MainWindow
    {
        async Task RunScenario(string file, string outDir)
        {
            EditDialog dialog = null;
            RuleDialog ruleDialog = null;
            var log = new List<string>();
            foreach (string raw in File.ReadAllLines(file))
            {
                string line = raw.Trim();
                if (line.Length == 0 || line.StartsWith("#")) continue;
                int sp = line.IndexOf(' ');
                string cmd = sp < 0 ? line : line.Substring(0, sp), arg = sp < 0 ? "" : line.Substring(sp + 1);
                await Dispatcher.InvokeAsync(() => { }, DispatcherPriority.ApplicationIdle);
                switch (cmd)
                {
                    case "unit": ShowUnit(arg); break;
                    case "tab": Tabs.SelectedItem = arg == "full" ? FullTab : arg == "items" ? ItemsTab : arg == "roster" ? RosterTab : SummaryTab; break;
                    case "filter": FullFilter.Text = arg; break;
                    case "open":
                    {
                        var list = (Tabs.SelectedItem == SummaryTab ? SummaryList : Tabs.SelectedItem == RosterTab ? RosterList : FullList).ItemsSource as System.Collections.IEnumerable;
                        var item = list == null ? null : list.Cast<RowItem>().Where(x => x.Editable || Text.IsCopyableBlock(x.Row))
                            .FirstOrDefault(x => (x.Name ?? "").Contains(arg) || (x.Key ?? "").Contains(arg) || (x.Values ?? "").Contains(arg));
                        if (item == null) { log.Add("open: 줄을 찾지 못했다: " + arg); break; }
                        dialog = new EditDialog(session, unit, item.Row, item.Name, item.Row.Context) { Owner = this };
                        dialog.Show();
                        break;
                    }
                    case "type": { int n = int.Parse(arg.Substring(0, arg.IndexOf(' '))); dialog.TypeValue(n - 1, arg.Substring(arg.IndexOf(' ') + 1)); break; }
                    case "piece": { var parts = arg.Split(' ', 3); dialog.TypePiece(int.Parse(parts[0]) - 1, parts[1], parts[2]); break; }
                    case "scope": dialog.ChooseScope(arg == "unit"); break;
                    case "ok":
                        if (dialog.PressOk()) { dialog = null; RefreshUnitViews(); RefreshEdits(); log.Add("ok: 넣었다"); }
                        else log.Add("ok: 단추가 꺼져 있거나 저장에 실패했다");
                        break;
                    case "openblock":
                    {
                        // 따옴표 이름 블록의 이름 줄(값이 그 이름인 복제할 수 있는 줄)을 연다(E18)
                        var list = FullList.ItemsSource as System.Collections.IEnumerable;
                        var item = list == null ? null : list.Cast<RowItem>().FirstOrDefault(x => Text.IsCopyableBlock(x.Row) && Text.Value(x.Row.Values[0].Trace.Text) == arg);
                        if (item == null) { log.Add("openblock: 줄을 찾지 못했다: " + arg); break; }
                        dialog = new EditDialog(session, unit, item.Row, item.Name, item.Row.Context) { Owner = this };
                        dialog.Show();
                        break;
                    }
                    case "copyblock":
                        if (dialog.PressCopy(arg)) { dialog.Close(); dialog = null; RefreshUnitViews(); RefreshEdits(); log.Add("copyblock: 넣었다"); }
                        else log.Add("copyblock: 넣지 못했다");
                        break;
                    case "rule":
                    {
                        var p = arg.Split('|');
                        var start = new CtaToybox.Core.BulkRule
                        {
                            Kind = p[0], Paths = p[1].Length == 0 ? null : p[1].Split(',').Select(x => x.Trim()).ToList(), Key = p[2],
                            Arg = int.Parse(p[3]), Op = p[4], Value = p[5], Scope = "auto",
                        };
                        ruleDialog = OpenRuleDialog(start, null, Math.Max(1, start.Arg + 1), null, false);
                        break;
                    }
                    case "rulerow":
                    {
                        var list = (Tabs.SelectedItem == SummaryTab ? SummaryList : FullList).ItemsSource as System.Collections.IEnumerable;
                        var item = list == null ? null : list.Cast<RowItem>().Where(x => x.Editable)
                            .FirstOrDefault(x => (x.Name ?? "").Contains(arg) || (x.Key ?? "").IndexOf(arg, StringComparison.OrdinalIgnoreCase) >= 0 || (x.Values ?? "").Contains(arg));
                        if (item == null) { log.Add("rulerow: 줄을 찾지 못했다: " + arg); break; }
                        ruleDialog = OpenRuleDialog(RuleForRow(item.Row), null, item.Row.Values.Count, RuleWarning(item.Row), false);
                        log.Add("rulerow: " + item.Row.Path);
                        break;
                    }
                    case "rulepreview": log.Add("rulepreview: " + (ruleDialog == null ? "창이 없다" : ruleDialog.PressPreview() ? "됐다" : "규칙이 맞지 않다")); break;
                    case "ruleok":
                        if (ruleDialog != null && ruleDialog.PressOk()) { ruleDialog.Close(); ruleDialog = null; AfterRuleChange(); log.Add("ruleok: 넣었다"); }
                        else log.Add("ruleok: 단추가 꺼져 있거나 넣지 못했다");
                        break;
                    case "cancel":
                        if (ruleDialog != null) { ruleDialog.Close(); ruleDialog = null; }
                        else if (dialog != null) { dialog.Close(); dialog = null; }
                        break;
                    case "toggle":
                    {
                        var e = ((IEnumerable<object>)EditList.ItemsSource.Cast<object>()).OfType<EditItem>().ElementAt(int.Parse(arg) - 1);
                        Try("수정 켜고 끄기", () => session.SetEnabled(e.Edit, !e.Edit.Enabled));
                        RefreshEdits(); RefreshUnitViews();
                        break;
                    }
                    case "remove":
                    {
                        var e = EditList.ItemsSource.Cast<object>().OfType<EditItem>().ElementAt(int.Parse(arg) - 1);
                        Try("수정 삭제", () => session.RemoveEdit(e.Edit));
                        RefreshEdits(); RefreshUnitViews();
                        break;
                    }
                    case "group":
                    {
                        // group summary|full|roster <글자> open|close: 묶음 이름에 글자가 든 첫 묶음을 펼치거나 접는다
                        var parts = arg.Split(' ');
                        var list = parts[0] == "full" ? FullList : parts[0] == "roster" ? RosterList : SummaryList;
                        string text = string.Join(" ", parts.Skip(1).Take(parts.Length - 2));
                        var ex = Expanders(list).FirstOrDefault(x => x.DataContext is System.Windows.Data.CollectionViewGroup g && (g.Name as string ?? "").Contains(text));
                        if (ex == null) log.Add("묶음 없음: " + line); else ex.IsExpanded = parts[parts.Length - 1] == "open";
                        break;
                    }
                    case "scroll": { var sv = ScrollOf(arg == "full" ? FullList : arg == "roster" ? RosterList : SummaryList); if (sv != null) sv.ScrollToEnd(); break; }
                    case "shot":
                        await Dispatcher.InvokeAsync(() => { }, DispatcherPriority.ApplicationIdle);
                        if (ruleDialog != null) ruleDialog.UpdateLayout(); else if (dialog != null) dialog.UpdateLayout();
                        await Dispatcher.InvokeAsync(() => { }, DispatcherPriority.ApplicationIdle);
                        SaveScreenshot(ruleDialog != null ? ruleDialog : dialog != null ? (System.Windows.Window)dialog : this, Path.Combine(outDir, arg + ".png"));
                        break;
                    default: log.Add("모르는 명령: " + line); break;
                }
            }
            File.WriteAllLines(Path.Combine(outDir, "scenario-log.txt"), log);
        }

        static IEnumerable<Expander> Expanders(System.Windows.DependencyObject d)
        {
            for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(d); i++)
            {
                var child = System.Windows.Media.VisualTreeHelper.GetChild(d, i);
                if (child is Expander ex) yield return ex;
                foreach (var inner in Expanders(child)) yield return inner;
            }
        }
    }
}
