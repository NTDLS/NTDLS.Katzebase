﻿using ICSharpCode.AvalonEdit;
using NTDLS.Helpers;
using NTDLS.Katzebase.Client;
using NTDLS.Katzebase.Client.Exceptions;
using NTDLS.Katzebase.Client.Payloads;
using NTDLS.Katzebase.Management.Classes;
using System.Text;
using static NTDLS.Katzebase.Client.KbConstants;

namespace NTDLS.Katzebase.Management.Controls
{
    /// <summary>
    /// A code tab contains a code editor, output tab, splitter and a client connection.
    /// </summary>
    internal class CodeTabPage : TabPage, IDisposable
    {
        #region Properties.

        public FormStudio StudioForm { get; set; }
        public TabControl TabControlParent { get; private set; }
        public int ExecutionExceptionCount { get; private set; } = 0;
        public bool IsScriptExecuting { get; private set; } = false;
        public string ServerHost { get; set; }
        public KbClient? Client { get; private set; }
        public bool IsFileOpen { get; private set; } = false;

        private bool _isSaved = false;
        public bool IsSaved
        {
            get => _isSaved;

            set
            {
                _isSaved = value;
                if (_isSaved == true)
                {
                    Text = Text.TrimEnd('*');
                }
            }
        }

        public SplitContainer TabSplitContainer = new()
        {
            Orientation = Orientation.Horizontal,
            Dock = DockStyle.Fill,
            Panel2Collapsed = true,
            SplitterWidth = 10
        };

        public bool CollapseSplitter
        {
            get => TabSplitContainer.Panel2Collapsed;
            set
            {
                TabSplitContainer.Panel2Collapsed = value;
                if (value == false)
                {
                    TabSplitContainer.SplitterDistance = TabSplitContainer.Height / 2;
                }
            }
        }

        private string _filePath = string.Empty;
        public string FilePath
        {
            get => _filePath;
            set
            {
                Text = Path.GetFileName(value);
                _filePath = value;
            }
        }

        #endregion

        #region Controls.

        public TabPage OutputTab { get; private set; } = new("Output");
        public TabPage ResultsTab { get; private set; } = new("Results");
        public Panel ResultsPanel { get; private set; } = new()
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
        };

        public RichTextBox OutputTextbox { get; private set; } = new()
        {
            Dock = DockStyle.Fill,
            Font = new Font("Cascadia Mono", 10, FontStyle.Regular),
            WordWrap = false,
        };

        public TabControl BottomTabControl { get; private set; } = new() { Dock = DockStyle.Fill };
        public FullyFeaturedCodeEditor Editor { get; private set; }
        public FormFindText FindTextForm { get; private set; }
        public FormReplaceText ReplaceTextForm { get; private set; }

        #endregion

        #region IDisposable.

        private bool disposed = false;
        public new void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
            base.Dispose();
        }

        protected new virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                Client?.Dispose();
            }

            disposed = true;
            base.Dispose(disposing);
        }

        #endregion

        public CodeTabPage(FormStudio studioForm, TabControl tabControlParent, string serverHost, int serverPort, string username, string passwordHash, string tabText) :
             base(tabText)
        {
            StudioForm = studioForm;
            TabControlParent = tabControlParent;
            Editor = new FullyFeaturedCodeEditor(this);
            FindTextForm = new FormFindText(this);
            ReplaceTextForm = new FormReplaceText(this);
            ServerHost = serverHost;

            BottomTabControl.Resize += (object? sender, EventArgs e) =>
            {
                EvenlyDistributedListViews();
            };

            Controls.Add(TabSplitContainer);

            var editorContainer = new System.Windows.Forms.Integration.ElementHost
            {
                Dock = DockStyle.Fill,
                Child = Editor
            };

            Editor.AssociateContainer(editorContainer);

            TabSplitContainer.Panel1.Controls.Add(editorContainer);

            TabSplitContainer.Panel2.Controls.Add(BottomTabControl);
            BottomTabControl.Dock = DockStyle.Fill;
            BottomTabControl.TabPages.Add(OutputTab); //Add output tab to bottom.

            OutputTab.Controls.Add(OutputTextbox);
            OutputTextbox.Dock = DockStyle.Fill;

            BottomTabControl.TabPages.Add(ResultsTab); //Add results tab to bottom.
            ResultsTab.Controls.Add(ResultsPanel);
            ResultsPanel.Dock = DockStyle.Fill;


            if (string.IsNullOrEmpty(serverHost) == false)
            {
                Client = new KbClient(serverHost, serverPort, username, passwordHash, $"{KbConstants.FriendlyName}.UI.Query");
                Client.QueryTimeout = TimeSpan.FromSeconds(Program.Settings.UIQueryTimeOut);
            }


            Editor.KeyUp += TextEditor_KeyUp;
            Editor.TextChanged += TextEditor_TextChanged;
        }

        #region Text Editor.

        private void TextEditor_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.F5)
            {
                ExecuteCurrentScriptAsync(ExecuteType.Execute);
            }
            else if (e.Key == System.Windows.Input.Key.F6)
            {
                ExecuteCurrentScriptAsync(ExecuteType.ExplainPlan);
            }
            else if (e.Key == System.Windows.Input.Key.F7)
            {
                ExecuteCurrentScriptAsync(ExecuteType.ExplainOperations);
            }
        }

        private void TextEditor_TextChanged(object? sender, EventArgs e)
        {
            var editor = sender as TextEditor;
            if (editor != null)
            {
                var tab = EditorToTab(editor);
                if (tab != null)
                {
                    if (tab.Text.EndsWith('*') == false)
                    {
                        tab.Text = $"{tab.Text}*";
                    }
                    tab.IsSaved = false;
                }
            }
        }

        private CodeTabPage? EditorToTab(TextEditor editor)
        {
            foreach (CodeTabPage tab in TabControlParent.TabPages)
            {
                if (tab.Editor == editor)
                {
                    return tab;
                }
            }
            return null;
        }

        #endregion

        public void OpenFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                Editor.Document.FileName = filePath;
                Editor.Text = File.ReadAllText(Editor.Document.FileName);
                IsSaved = true;
            }

            FilePath = filePath;

            IsFileOpen = true;
        }

        public bool Save(string fileName)
        {
            File.WriteAllText(fileName, Editor.Text);
            IsSaved = true;
            OpenFile(fileName);
            return true;
        }

        public bool Save()
        {
            if (IsFileOpen)
            {
                File.WriteAllText(FilePath, Editor.Text);
                IsSaved = true;
                return true;
            }
            return false;
        }

        #region Execute.

        public void ExecuteStopCommand()
        {
            if (IsScriptExecuting == false)
            {
                return;
            }
            if (Client == null)
            {
                IsScriptExecuting = false;
                return;
            }


            Client.Transaction.Rollback();
        }

        public enum ExecuteType
        {
            Execute,
            ExplainPlan,
            ExplainOperations
        }

        /// <summary>
        /// This is for actually executing the script against a live database.
        /// </summary>
        public void ExecuteCurrentScriptAsync(ExecuteType executeType)
        {
            try
            {
                if (IsScriptExecuting)
                {
                    return;
                }
                IsScriptExecuting = true;

                if (Client == null || (Client?.ProcessId ?? 0) == 0)
                {
                    try
                    {
                        using var form = new FormConnect(Client?.Host ?? "", Client?.Port ?? 6858, string.IsNullOrWhiteSpace(Client?.Username) ? "admin" : Client.Username);
                        if (form.ShowDialog() != DialogResult.OK)
                        {
                            IsScriptExecuting = false;
                            return;
                        }

                        Client = new KbClient(form.ServerHost, form.ServerPort, form.Username, form.PasswordHash, $"{KbConstants.FriendlyName}.UI.Query");
                        Client.QueryTimeout = TimeSpan.FromSeconds(Program.Settings.UIQueryTimeOut);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, FriendlyName);
                        IsScriptExecuting = false;
                        return;
                    }
                }

                if (Client == null)
                {
                    return;
                }

                PreExecuteEvent(this);

                foreach (var dgv in ResultsPanel.Controls.OfType<DoubleBufferedListView>().ToList())
                {
                    dgv.Dispose();
                }
                ResultsPanel.Controls.Clear();

                string scriptText = Editor.Text + "\r\n";

                if (Editor.SelectionLength > 0)
                {
                    scriptText = Editor.SelectedText;
                }

                Task.Run(() =>
                {
                    ExecuteCurrentScriptSync(Client, scriptText, executeType);
                }).ContinueWith((t) =>
                {
                    PostExecuteEvent(this);
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", FriendlyName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PreExecuteEvent(CodeTabPage tabFilePage)
        {
            try
            {
                if (InvokeRequired)
                {
                    Invoke(new Action<CodeTabPage>(PreExecuteEvent), this);
                    return;
                }

                OutputTextbox.Text = "";
                ExecutionExceptionCount = 0;

                CollapseSplitter = false;

                tabFilePage.Text = $"{tabFilePage.Text} | (executing)";

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", FriendlyName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PostExecuteEvent(CodeTabPage tabFilePage)
        {
            try
            {
                if (InvokeRequired)
                {
                    Invoke(new Action<CodeTabPage>(PostExecuteEvent), tabFilePage);
                    return;
                }

                if (TabControlParent.SelectedTab == tabFilePage)
                {
                    if (ResultsPanel.Controls.OfType<DoubleBufferedListView>().Any())
                    {
                        BottomTabControl.SelectedTab = ResultsTab;
                    }
                    else
                    {
                        BottomTabControl.SelectedTab = OutputTab;
                    }

                    tabFilePage.Focus();
                    tabFilePage.Editor.Focus();
                }

                tabFilePage.Text = tabFilePage.Text.Replace(" | (executing)", "");

                IsScriptExecuting = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", FriendlyName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        class MetricsTextItem
        {
            public string Value { get; set; } = string.Empty;
            public string Average { get; set; } = string.Empty;
            public string Count { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
        }

        private void WriteWarnings(Dictionary<KbTransactionWarning, HashSet<string>> warnings)
        {
            foreach (var warning in warnings)
            {
                AppendToOutput($"Warning: {warning.Key}", Color.DarkOrange);
                foreach (var message in warning.Value)
                {
                    AppendToOutput($"    > {message}", Color.DarkOrange);
                }
            }
        }

        private void WriteMessages(List<KbQueryResultMessage> messages)
        {
            foreach (var message in messages)
            {
                if (message.MessageType == KbMessageType.Verbose)
                    AppendToOutput($"{message.Text}", Color.Black);
                else if (message.MessageType == KbMessageType.Warning)
                    AppendToOutput($"{message.Text}", Color.DarkOrange);
                else if (message.MessageType == KbMessageType.Deadlock)
                    AppendToOutput($"{message.Text}", Color.DarkBlue);
                else if (message.MessageType == KbMessageType.Error)
                    AppendToOutput($"{message.Text}", Color.DarkRed);
                else if (message.MessageType == KbMessageType.Explain)
                    AppendToOutput($"{message.Text}", Color.DarkGreen);
            }
        }
        private void WriteMetrics(KbMetricCollection? metrics)
        {
            if (metrics == null || metrics.Count == 0)
            {
                return;
            }

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Metrics {");

            var metricsTextItems = new List<MetricsTextItem>();

            foreach (var wt in metrics.Where(o => o.Value >= 0.5).OrderBy(o => o.Value))
            {
                if (wt.MetricType == KbMetricType.Cumulative)
                {
                    metricsTextItems.Add(new MetricsTextItem()
                    {
                        Name = wt.Name,
                        Value = $"Value: {wt.Value:n0}",
                        Average = $"Average: {wt.Value / wt.Count:n2}",
                        Count = $"Count: {wt.Count:n0}"
                    });
                }
                else
                {
                    metricsTextItems.Add(new MetricsTextItem() { Name = wt.Name, Value = $"Value: {wt.Value:n0}", });
                }
            }

            if (metricsTextItems.Count > 0)
            {
                int maxValueLength = metricsTextItems.Max(o => o.Value.Length);
                int maxAverageLength = metricsTextItems.Max(o => o.Average.Length);
                int maxCountLength = metricsTextItems.Max(o => o.Count.Length);

                foreach (var metricsTextItem in metricsTextItems)
                {
                    int diff = (maxValueLength - metricsTextItem.Value.Length) + 1;
                    string metricText = $"{metricsTextItem.Value}{new string(' ', diff)}";

                    diff = (maxCountLength - metricsTextItem.Count.Length) + 1;
                    metricText += $"{metricsTextItem.Count}{new string(' ', diff)}";

                    diff = (maxAverageLength - metricsTextItem.Average.Length) + 1;
                    metricText += $"{metricsTextItem.Average}{new string(' ', diff)}";

                    metricText += Helpers.Text.SeperateCamelCase(metricsTextItem.Name).Replace(":", " : ");

                    stringBuilder.AppendLine($"  {metricText}");
                }
            }

            stringBuilder.AppendLine($"}}");

            AppendToOutput(stringBuilder.ToString(), Color.DarkBlue);
        }

        private void ExecuteCurrentScriptSync(KbClient client, string scriptText, ExecuteType executeType)
        {
            var group = new WorkloadGroup();

            try
            {
                group.OnException += Group_OnException;
                group.OnStatus += Group_OnStatus;

                var scripts = KbTextUtility.SplitQueryBatchesOnGO(scriptText);

                var startTime = DateTime.UtcNow;

                if (executeType == ExecuteType.ExplainPlan)
                {
                    var results = client.Query.ExplainPlans(scripts, Program.Settings.UserQueryTimeOut >= 0 ? TimeSpan.FromSeconds(Program.Settings.UserQueryTimeOut) : Timeout.InfiniteTimeSpan);

                    int batchNumber = 1;
                    foreach (var result in results.Collection)
                    {
                        AppendToOutput($"Batch {batchNumber:n0} of {results.Collection.Count} completed in {result.Duration:N0}ms. ({result.RowCount} rows affected)", Color.Black);
                        batchNumber++;

                        WriteMetrics(result.Metrics);
                        WriteWarnings(result.Warnings);
                        WriteMessages(result.Messages);
                    }
                }
                else if (executeType == ExecuteType.ExplainOperations)
                {
                    var results = client.Query.ExplainOperations(scripts, Program.Settings.UserQueryTimeOut >= 0 ? TimeSpan.FromSeconds(Program.Settings.UserQueryTimeOut) : Timeout.InfiniteTimeSpan);

                    int batchNumber = 1;
                    foreach (var result in results.Collection)
                    {
                        AppendToOutput($"Batch {batchNumber:n0} of {results.Collection.Count} completed in {result.Duration:N0}ms. ({result.RowCount} rows affected)", Color.Black);
                        batchNumber++;

                        WriteMetrics(result.Metrics);
                        WriteWarnings(result.Warnings);
                        WriteMessages(result.Messages);
                    }
                }
                else if (executeType == ExecuteType.Execute)
                {
                    var results = client.Query.FetchMultiple(scripts, Program.Settings.UserQueryTimeOut >= 0 ? TimeSpan.FromSeconds(Program.Settings.UserQueryTimeOut) : Timeout.InfiniteTimeSpan);

                    int batchNumber = 1;
                    foreach (var result in results.Collection)
                    {
                        AppendToOutput($"Batch {batchNumber:n0} of {results.Collection.Count} completed in {result.Duration:N0}ms. ({result.RowCount} rows affected)", Color.Black);
                        batchNumber++;

                        WriteMetrics(result.Metrics);
                        WriteWarnings(result.Warnings);
                        WriteMessages(result.Messages);
                    }

                    PopulateResultsGrid(results);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            catch (Exception ex)
            {
                Group_OnException(group, new KbExceptionBase((ex.GetRoot() ?? ex).Message));
            }
        }

        internal void EvenlyDistributedListViews()
        {
            var grids = ResultsPanel.Controls.OfType<DoubleBufferedListView>().ToList();

            int spacing = 10;
            int totalSpacing = (grids.Count - 1) * spacing;
            int availableHeight = (BottomTabControl.Height - totalSpacing) - 20;
            int gridTop = 0;

            int gridHeight = availableHeight / 2;

            if (gridHeight < 100)
            {
                gridHeight = 100;
            }

            foreach (var grid in grids)
            {
                //grid.Width = BottomTabControl.Width;

                if (grids.Count > 1)
                {
                    grid.Dock = DockStyle.Top;
                }
                else
                {
                    grid.Dock = DockStyle.Fill;
                }

                grid.Top = gridTop;
                grid.Height = availableHeight / grids.Count;

                gridTop += grid.Height + spacing;
            }
        }

        private List<DoubleBufferedListView> AddEvenlyDistributedListViews(int numListViews)
        {
            var results = new List<DoubleBufferedListView>();

            foreach (var dgv in ResultsPanel.Controls.OfType<DoubleBufferedListView>().ToList())
            {
                dgv.Dispose();
            }
            ResultsPanel.Controls.Clear();

            for (int i = 0; i < numListViews; i++)
            {
                var ListView = new DoubleBufferedListView();
                results.Add(ListView);
                ResultsPanel.Controls.Add(ListView);
            }

            EvenlyDistributedListViews();

            results.Reverse();

            return results;
        }

        private void PopulateResultsGrid(KbQueryResultCollection resultCollection)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<KbQueryResultCollection>(PopulateResultsGrid), resultCollection);
                return;
            }

            var results = resultCollection.Collection.Where(o => o.Rows.Count != 0).ToList();

            var outputGrids = AddEvenlyDistributedListViews(results.Count);

            for (int i = 0; i < results.Count; i++)
            {
                var result = results[i];
                var outputGrid = outputGrids[i];

                try
                {
                    if (result == null || result.Rows.Count == 0)
                    {
                        continue;
                    }

                    outputGrid.SuspendLayout();
                    outputGrid.BeginUpdate();

                    foreach (var field in result.Fields)
                    {
                        outputGrid.Columns.Add(field.Name, field.Name);
                    }

                    int maxRowsToLoad = Program.Settings.QueryMaximumRows;
                    foreach (var row in result.Rows)
                    {
                        var rowValues = new List<string>();

                        for (int fieldIndex = 0; fieldIndex < result.Fields.Count; fieldIndex++)
                        {
                            var fieldValue = row.Values[fieldIndex];
                            rowValues.Add(fieldValue ?? string.Empty);
                        }

                        var item = new ListViewItem(rowValues.ToArray());

                        outputGrid.Items.Add(item);

                        maxRowsToLoad--;
                        if (maxRowsToLoad <= 0)
                        {
                            break;
                        }
                    }

                    ResizeListViewColumns(outputGrid);

                    outputGrid.ResumeLayout();
                    outputGrid.EndUpdate();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", FriendlyName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ResizeListViewColumns(ListView listView)
        {
            const int maxWidth = 500;

            foreach (ColumnHeader column in listView.Columns)
            {
                column.Width = -2; // Resize to fit content initially

                // Get the width of the column header
                int headerWidth = TextRenderer.MeasureText(column.Text, listView.Font).Width + 10; // Adding some padding

                // Get the maximum width of the column content
                int contentWidth = 0;
                foreach (ListViewItem item in listView.Items)
                {
                    int cellWidth = TextRenderer.MeasureText(item.SubItems[column.Index].Text, listView.Font).Width + 10; // Adding some padding
                    if (cellWidth > contentWidth)
                    {
                        contentWidth = cellWidth;
                    }
                }

                // Determine the final column width
                int finalWidth = Math.Max(headerWidth, contentWidth);
                finalWidth = Math.Min(finalWidth, maxWidth);

                // Set the column width
                column.Width = finalWidth;
            }
        }

        private void Group_OnStatus(WorkloadGroup sender, string text, Color color)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<WorkloadGroup, string, Color>(Group_OnStatus), sender, text, color);
                return;
            }

            AppendToOutput(text, color);
        }

        private void AppendToOutput(string text, Color color)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string, Color>(AppendToOutput), text, color);
                return;
            }

            OutputTextbox.SelectionStart = OutputTextbox.TextLength;
            OutputTextbox.SelectionLength = 0;

            OutputTextbox.SelectionColor = color;
            OutputTextbox.AppendText($"{text}\r\n");
            OutputTextbox.SelectionColor = OutputTextbox.ForeColor;

            OutputTextbox.SelectionStart = OutputTextbox.Text.Length;
            //OutputTextbox.ScrollToCaret();
        }

        private void Group_OnException(WorkloadGroup sender, KbExceptionBase ex)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<WorkloadGroup, KbExceptionBase>(Group_OnException), sender, ex);
                return;
            }

            ExecutionExceptionCount++;

            CollapseSplitter = false;

            AppendToOutput($"{ex.Message}\r\n", Color.DarkRed);
        }

        #endregion
    }
}
