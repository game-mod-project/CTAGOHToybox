// 일괄 규칙(F1) 창. 대상 종류·경로 글자·계층·키·값 자리·바꾸는 방법·범위를 고르고 미리 본 뒤 수정 목록에 넣는다.
// 새 규칙이면 EditorSession.AddRule, 이미 있는 규칙을 고치면 EditRule을 부른다. 미리 보기는 프로젝트를 바꾸지 않는다.
// 칸을 바꾸면 미리 보기를 다시 해야 넣을 수 있다(본 것과 다른 것이 들어가지 않게)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CtaToybox.Core;

namespace CtaToybox.App
{
    public partial class RuleDialog : Window
    {
        const int MaxShown = 500;
        readonly EditorSession session;
        readonly string ruleId;   // 고치는 규칙의 id. 새 규칙이면 null
        bool filling;

        public sealed class Line { public string Title { get; set; } public string Sub { get; set; } }

        public RuleDialog(EditorSession session, BulkRule start, string ruleId, int valueCount, string warning)
        {
            InitializeComponent();
            this.session = session; this.ruleId = ruleId;
            Title = ruleId == null ? "일괄 규칙 만들기" : "일괄 규칙 고치기";
            OkButton.Content = ruleId == null ? "수정 목록에 넣기" : "규칙 바꾸기";
            KindBox.ItemsSource = UnitKind.Editor.Select(k => new Choice { Label = UnitKind.Label(k) + " (" + k + ")", Value = k }).ToList();
            LayerBox.ItemsSource = new[] { new Choice { Label = "전체", Value = null } }
                .Concat(session.Plan.Layers.Select(l => new Choice { Label = session.LayerLabel(l.Name), Value = l.Name })).ToList();
            var args = new List<Choice> { new Choice { Label = "수인 값 모두", Value = "-1" } };
            for (int i = 0; i < Math.Max(valueCount, Math.Max(1, start.Arg + 1)); i++) args.Add(new Choice { Label = (i + 1) + "번째 값", Value = i.ToString() });
            ArgBox.ItemsSource = args;
            OpBox.ItemsSource = new List<Choice> { new Choice { Label = "곱하기 (×)", Value = "mul" }, new Choice { Label = "더하기 (+, 빼기는 -)", Value = "add" }, new Choice { Label = "이 값으로 (=)", Value = "set" } };
            ScopeBox.ItemsSource = new List<Choice>
            {
                new Choice { Label = "자동: 파일을 쓰는 유닛이 모두 대상이면 '전부', 아니면 대상 유닛마다 '이 유닛만'", Value = "auto" },
                new Choice { Label = "전부: 같은 파일을 쓰는 대상 밖 유닛도 바뀐다", Value = "shared" },
                new Choice { Label = "이 유닛만: 대상 유닛마다 따로", Value = "unit" },
            };
            WarnText.Text = warning ?? "";
            Fill(start);
            foreach (var box in new Control[] { NameBox, PathsBox, KeyBox, ValueBox }) ((TextBox)box).TextChanged += (s, e) => Stale();
            foreach (var box in new[] { KindBox, LayerBox, ArgBox, OpBox, ScopeBox }) box.SelectionChanged += (s, e) => Stale();
        }

        void Fill(BulkRule r)
        {
            filling = true;
            NameBox.Text = r.Name ?? "";
            Select(KindBox, r.Kind);
            PathsBox.Text = r.Paths == null ? "" : string.Join(", ", r.Paths);
            Select(LayerBox, r.Layer);
            KeyBox.Text = r.Key ?? "";
            Select(ArgBox, r.Arg.ToString());
            Select(OpBox, r.Op ?? "mul");
            ValueBox.Text = r.Value ?? "";
            Select(ScopeBox, r.Scope ?? "auto");
            filling = false;
        }

        static void Select(ComboBox box, string value)
        {
            var list = ((IEnumerable<Choice>)box.ItemsSource).ToList();
            int at = list.FindIndex(c => c.Value == value);
            box.SelectedIndex = at >= 0 ? at : 0;
        }

        static string Picked(ComboBox box) { var c = box.SelectedItem as Choice; return c == null ? null : c.Value; }

        public BulkRule Rule()
        {
            var paths = PathsBox.Text.Split(',').Select(p => p.Trim()).Where(p => p.Length > 0).ToList();
            return new BulkRule
            {
                Name = NameBox.Text.Trim(),
                Kind = Picked(KindBox),
                Paths = paths.Count > 0 ? paths : null,
                Layer = Picked(LayerBox),
                Key = KeyBox.Text.Trim(),
                Arg = int.Parse(Picked(ArgBox) ?? "0"),
                Op = Picked(OpBox),
                Value = ValueBox.Text.Trim(),
                Scope = Picked(ScopeBox) ?? "auto",
            };
        }

        void Stale()
        {
            if (filling) return;
            OkButton.IsEnabled = false;
            if (ChangesList.Items.Count > 0 || SkippedList.Items.Count > 0) TotalText.Text = "칸이 바뀌었다. 미리 보기를 다시 한다.";
        }

        void Tail_Click(object sender, RoutedEventArgs e)
        {
            var parts = KeyBox.Text.Trim().TrimStart('*').Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 0) KeyBox.Text = "**/" + string.Join("/", parts.Skip(Math.Max(0, parts.Length - 2)));
        }

        void Preview_Click(object sender, RoutedEventArgs e) { PressPreview(); }

        // 시나리오도 부른다. 미리 본 규칙이 맞으면 true
        public bool PressPreview()
        {
            MessageText.Text = "";
            var rule = Rule();
            rule.Id = ruleId;
            string bad = BulkRules.Check(rule);
            if (bad != null) { MessageText.Text = bad; OkButton.IsEnabled = false; return false; }
            RulePreview p;
            try { p = session.PreviewRule(rule); }
            catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException || ex is FormatException)
            {
                MessageText.Text = "미리 보기에 실패했다: " + ex.Message; OkButton.IsEnabled = false; return false;
            }
            TotalText.Text = "대상 유닛 " + p.Targets + " · 바뀌는 유닛 " + p.Units + " · 바뀌는 값 " + p.Changes.Count + "건 · 수정 " + p.Edits.Count + "건" +
                             (p.OutsideUnits > 0 ? " · '전부' 수정으로 함께 바뀌는 대상 밖 유닛 " + p.OutsideUnits : "") + " · 건너뜀 " + p.Skipped.Count + "건";
            ChangesTitle.Text = "바뀌는 값" + (p.Changes.Count > MaxShown ? " (앞 " + MaxShown + "건만 보인다. 합계는 모두 센다)" : "");
            ChangesList.ItemsSource = p.Changes.Take(MaxShown).Select(c => new Line
            {
                Title = Text.UnitName(session, SourceStack.ToVirtual(c.Unit)) + " · " + Text.Value(c.Old) + " → " + Text.Value(c.New),
                Sub = (c.Scope == "unit" ? "이 유닛만" : "전부") + " · " + KeyLabel.Readable(c.Path) + " · " + c.File,
            }).ToList();
            SkippedTitle.Text = "건너뛰는 유닛" + (p.Skipped.Count > MaxShown ? " (앞 " + MaxShown + "건만 보인다)" : "");
            SkippedList.ItemsSource = p.Skipped.Take(MaxShown).Select(k => new Line
            {
                Title = Text.UnitName(session, SourceStack.ToVirtual(k.Unit)),
                Sub = k.Reason + " · " + SourceStack.ToVirtual(k.Unit),
            }).ToList();
            OkButton.IsEnabled = session.Project != null && p.Edits.Count > 0;
            if (session.Project == null) MessageText.Text = "프로젝트가 열려 있지 않다. 미리 보기만 할 수 있다.";
            else if (p.Edits.Count == 0) MessageText.Text = "바뀌는 값이 없다.";
            return true;
        }

        void Ok_Click(object sender, RoutedEventArgs e) { if (PressOk()) DialogResult = true; }

        // 시나리오도 부른다. 넣었으면 true(창은 부른 쪽이 닫는다)
        public bool PressOk()
        {
            if (!OkButton.IsEnabled) return false;
            try
            {
                if (ruleId == null) session.AddRule(Rule()); else session.EditRule(ruleId, Rule());
                return true;
            }
            catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException || ex is FormatException || ex is System.IO.IOException || ex is UnauthorizedAccessException)
            {
                MessageText.Text = "넣지 못했다: " + ex.Message;
                return false;
            }
        }
    }
}
