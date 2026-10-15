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
//   toggle <n>                수정 목록 n번째의 체크를 바꾼다
//   remove <n>                수정 목록 n번째를 지운다
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
                        var item = list == null ? null : list.Cast<RowItem>().Where(x => x.Editable)
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
                    case "cancel": dialog.Close(); dialog = null; break;
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
                    case "shot":
                        await Dispatcher.InvokeAsync(() => { }, DispatcherPriority.ApplicationIdle);
                        if (dialog != null) dialog.UpdateLayout();
                        await Dispatcher.InvokeAsync(() => { }, DispatcherPriority.ApplicationIdle);
                        SaveScreenshot(dialog != null ? (System.Windows.Window)dialog : this, Path.Combine(outDir, arg + ".png"));
                        break;
                    default: log.Add("모르는 명령: " + line); break;
                }
            }
            File.WriteAllLines(Path.Combine(outDir, "scenario-log.txt"), log);
        }
    }
}
