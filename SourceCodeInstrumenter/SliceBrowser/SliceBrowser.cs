using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using Tracer.Poco;
using System.Threading;

namespace SliceBrowser
{
    public partial class SliceBrowser : Form
    {
        private readonly Dictionary<int, string> executedFiles;
        private readonly List<Stmt> executedStmts;
        private readonly List<Stmt> slicedStatements;
        private int currentExecutedStmt;
        public Dictionary<int, List<Stmt>> stmtsInFile;
        private Dictionary<int, Dictionary<Stmt, List<TextMarker>>> markersInFile;
        private int CurrentFileId { get; set; }

        public static void Run(List<Stmt> execStmts, ISet<Stmt> slicedStmts, Dictionary<int, string> files)
        {
            var thread = new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new SliceBrowser(execStmts, slicedStmts, files));
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        public SliceBrowser(List<Stmt> execStmts, ISet<Stmt> slicedStmts, Dictionary<int, string> files)
        {
            markersInFile = new Dictionary<int, Dictionary<Stmt, List<TextMarker>>>();
            executedFiles = files;
            executedStmts = execStmts;
            slicedStatements = slicedStmts.ToList();
            stmtsInFile = slicedStmts.GroupBy(x => x.FileId).ToDictionary(g => g.Key, g => g.ToList());
            InitializeComponent();
        }

        private void sliceViewer_Load(object sender, EventArgs e)
        {
            // Load the syntax highlighting for C#.
            sliceViewer.SetHighlighting("C#");
            loaded_files.DataSource = new BindingSource(executedFiles, null);
            loaded_files.DisplayMember = "Value";
            loaded_files.ValueMember = "Key";

            // TODO SLICER 
            //currentExecutedStmt = executedStmts.Count - 1;
            //ShowStmt(executedStmts[currentExecutedStmt]);
            //MarkCurrentStmt(executedStmts[currentExecutedStmt]);

            currentExecutedStmt = slicedStatements.Count - 1;
            ShowStmt(slicedStatements[currentExecutedStmt]);
            MarkCurrentStmt(slicedStatements[currentExecutedStmt]);
        }

        private void ShowStmt(Stmt stmt)
        {
            if (CurrentFileId != stmt.FileId)
            {
                ChangeFile(stmt.FileId);
            }
            MoveCursorToStmt(stmt);
            sliceViewer.ActiveTextAreaControl.ScrollTo(sliceViewer.ActiveTextAreaControl.Caret.Line);
            TextAreaRefresh();
        }

        private void TextAreaRefresh()
        {
            TextAreaUpdate updatedArea = new TextAreaUpdate(TextAreaUpdateType.WholeTextArea);
            sliceViewer.Document.RequestUpdate(updatedArea);
            sliceViewer.Document.CommitUpdate();
        }

        private void MarkCurrentStmt(Stmt stmt)
        {
            RemoveSolidMarkerForStmt(stmt);
            AddCurrentMarkerToStmt(stmt);
        }

        private void UnMarkCurrentStmt(Stmt stmt)
        {
            RemoveSolidMarkerForStmt(stmt);
            AddExecutedMarkerToStmt(stmt);
        }

        private void RemoveSolidMarkerForStmt(Stmt stmt)
        {
            var markersForStmts = markersInFile[stmt.FileId];
            var stmtMarkers = markersForStmts[stmt];
            //var marker = stmtMarkers.Single(x => x.TextMarkerType == TextMarkerType.SolidBlock);
            var markers = stmtMarkers.Where(x => x.TextMarkerType == TextMarkerType.SolidBlock);
            
            //sliceViewer.Document.MarkerStrategy.RemoveAll(x => x.TextMarkerType == TextMarkerType.SolidBlock);
            foreach (var mark in markers)
            {
                sliceViewer.Document.MarkerStrategy.RemoveMarker(mark);
            }

            stmtMarkers.RemoveAll(x => x.TextMarkerType == TextMarkerType.SolidBlock);
            //foreach (var marker in markers) {
            //    sliceViewer.Document.MarkerStrategy.RemoveMarker(marker);
            //    stmtMarkers.Remove(marker);
            //}
        }

        private void ChangeFile(int fileId)
        {
            var file = executedFiles.Single(x => x.Key == fileId);
            ChangeFile(file);
        }

        private void ChangeFile(KeyValuePair<int, string> file)
        {
            using (var reader = new StreamReader(file.Value))
            {
                // Clean text area.
                sliceViewer.Text = "";

                // Load new file.
                sliceViewer.ActiveTextAreaControl.TextArea.InsertString(reader.ReadToEnd());

                // Mark all executed spans in file.
                var executedStmtsInFile = executedStmts.Where(x => x.FileId == file.Key).ToList();
                foreach (var stmt in executedStmtsInFile)
                {
                    AddExecutedMarkerToStmt(stmt);
                }

                // Mark sliced spans.
                if (stmtsInFile.ContainsKey(file.Key))
                {
                    foreach (var stmt in stmtsInFile[file.Key])
                    {
                        AddSlicedMarkerToStmt(stmt);
                    }
                }

                // Update current file id.
                CurrentFileId = file.Key;
            }
            loaded_files.SelectedIndex = loaded_files.FindStringExact(file.Value);
        }

        public void AddExecutedMarkerToStmt(Stmt stmt)
        {
            TextMarker marker = ExecutedStmtMarker(stmt);
            AddMarkerForStmt(stmt, marker);
            sliceViewer.Document.MarkerStrategy.AddMarker(marker);
        }

        public void AddCurrentMarkerToStmt(Stmt stmt)
        {
            TextMarker marker = CurrentStmtMarker(stmt);
            AddMarkerForStmt(stmt, marker);
            sliceViewer.Document.MarkerStrategy.AddMarker(marker);
        }

        public void AddSlicedMarkerToStmt(Stmt stmt)
        {
            TextMarker marker = SlicedStmtMarker(stmt);
            AddMarkerForStmt(stmt, marker);
            sliceViewer.Document.MarkerStrategy.AddMarker(marker);
        }

        public TextMarker ExecutedStmtMarker(Stmt stmt)
        {
            var spanStart = stmt.SpanStart;
            var spanEnd = stmt.SpanEnd;
            var length = spanEnd - spanStart;
            TextMarker marker = new TextMarker(spanStart, length, TextMarkerType.SolidBlock, Color.LightGoldenrodYellow);
            return marker;
        }

        public TextMarker SlicedStmtMarker(Stmt stmt)
        {
            int spanStart = stmt.SpanStart;
            int spanEnd = stmt.SpanEnd;
            int length = spanEnd - spanStart;
            TextMarker marker = new TextMarker(spanStart, length, TextMarkerType.Underlined, Color.Red);
            return marker;
        }
        
        public TextMarker CurrentStmtMarker(Stmt stmt)
        {
            int spanStart = stmt.SpanStart;
            int spanEnd = stmt.SpanEnd;
            int length = spanEnd - spanStart;
            TextMarker marker = new TextMarker(spanStart, length, TextMarkerType.SolidBlock, Color.Yellow);
            return marker;
        }

        public void AddMarkerForStmt(Stmt stmt, TextMarker marker)
        {
            if (markersInFile.ContainsKey(stmt.FileId))
            {
                var markersForStmts = markersInFile[stmt.FileId];
                if (markersForStmts.ContainsKey(stmt))
                {
                    markersForStmts[stmt].Add(marker);
                }
                else
                {
                    List<TextMarker> stmtMarkers = new List<TextMarker>();
                    stmtMarkers.Add(marker);
                    markersForStmts.Add(stmt, stmtMarkers);
                }
            }
            else
            {
                var markersForStmts = new Dictionary<Stmt, List<TextMarker>>();
                List<TextMarker> stmtMarkers = new List<TextMarker>();
                stmtMarkers.Add(marker);
                markersForStmts.Add(stmt, stmtMarkers);
                markersInFile.Add(stmt.FileId, markersForStmts);
            }
        }

        private void MoveCursorToStmt(Stmt stmt)
        {
            var startCol = stmt.SpanStart;
            int startLine = sliceViewer.Document.GetLineNumberForOffset(stmt.SpanStart);
            TextLocation loc = new TextLocation(startCol, startLine);
            sliceViewer.ActiveTextAreaControl.Caret.Position = loc;
        }

        private void loaded_files_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (loaded_files.SelectedItem != null)
            {
                var selectedFile = (KeyValuePair<int, string>) loaded_files.SelectedItem;
                if (selectedFile.Key != CurrentFileId)
                {
                    ChangeFile(selectedFile);
                }
            }
        }

        private void back_button_Click(object sender, EventArgs e)
        {
            // TODO SLICER 
            //UnMarkCurrentStmt(executedStmts[currentExecutedStmt]);
            UnMarkCurrentStmt(slicedStatements[currentExecutedStmt]);

            int tmp = currentExecutedStmt - 1;
            if (tmp < 0)
            {
                var confirmResult =
                    MessageBox.Show("Beginning of the execution trace reached. Do you want to go to the end?",
                        "Begining reached!",
                        MessageBoxButtons.YesNo);
                if (confirmResult == DialogResult.Yes)
                {
                    // TODO SLICER 
                    //currentExecutedStmt = executedStmts.Count - 1;
                    currentExecutedStmt = slicedStatements.Count - 1;
                }
                else
                {
                    return;
                }
            }
            else
            {
                currentExecutedStmt--;
            }
            // TODO SLICER 
            //ShowStmt(executedStmts[currentExecutedStmt]);
            //MarkCurrentStmt(executedStmts[currentExecutedStmt]);
            ShowStmt(slicedStatements[currentExecutedStmt]);
            MarkCurrentStmt(slicedStatements[currentExecutedStmt]);
        }

        private void next_button_Click(object sender, EventArgs e)
        {
            // TODO SLICER UnMarkCurrentStmt(executedStmts[currentExecutedStmt]);
            UnMarkCurrentStmt(slicedStatements[currentExecutedStmt]);

            int tmp = currentExecutedStmt + 1;
            // TODO SLICER if (tmp >= executedStmts.Count)
            if (tmp >= slicedStatements.Count)
            {
                var confirmResult =
                    MessageBox.Show("End of the execution trace reached. Do you want to go to the beginning?",
                        "End reached!",
                        MessageBoxButtons.YesNo);
                if (confirmResult == DialogResult.Yes)
                {
                    currentExecutedStmt = 0;
                }
                else
                {
                    return;
                }
            }
            else
            {
                currentExecutedStmt++;
            }

            // TODO SLICER
            //ShowStmt(executedStmts[currentExecutedStmt]);
            //MarkCurrentStmt(executedStmts[currentExecutedStmt]);
            ShowStmt(slicedStatements[currentExecutedStmt]);
            MarkCurrentStmt(slicedStatements[currentExecutedStmt]);
        }

        private void begin_button_Click(object sender, EventArgs e)
        {
            // TODO SLICER
            //UnMarkCurrentStmt(executedStmts[currentExecutedStmt]);
            //currentExecutedStmt = 0;
            //ShowStmt(executedStmts[currentExecutedStmt]);
            //MarkCurrentStmt(executedStmts[currentExecutedStmt]);

            UnMarkCurrentStmt(slicedStatements[currentExecutedStmt]);
            currentExecutedStmt = 0;
            ShowStmt(slicedStatements[currentExecutedStmt]);
            MarkCurrentStmt(slicedStatements[currentExecutedStmt]);
        }

        private void end_button_Click(object sender, EventArgs e)
        {
            // TODO SLICER
            //UnMarkCurrentStmt(executedStmts[currentExecutedStmt]);
            //currentExecutedStmt = executedStmts.Count - 1;
            //ShowStmt(executedStmts[currentExecutedStmt]);
            //MarkCurrentStmt(executedStmts[currentExecutedStmt]);

            UnMarkCurrentStmt(slicedStatements[currentExecutedStmt]);
            currentExecutedStmt = slicedStatements.Count - 1;
            ShowStmt(slicedStatements[currentExecutedStmt]);
            MarkCurrentStmt(slicedStatements[currentExecutedStmt]);
        }
    }
}