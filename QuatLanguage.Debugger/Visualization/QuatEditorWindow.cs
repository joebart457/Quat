using QuatLanguage.Debugger.Context;
using QuatLanguage.Debugger.Visualization.Extensions;
using QuatLanguage.Debugger.Visualization.Views;
using QuatLanguage.Interpreter.Engine.Words;
using QuatLanguage.Interpreter.Parser;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using Terminal.Gui;
using TokenizerCore.Interfaces;
using TokenizerCore.Models.Constants;

namespace QuatLanguage.Debugger.Visualization;

public class QuatEditorWindow : Window
{
    public static QuatEditorWindow? Instance { get; private set; }

    private List<CultureInfo> _cultureInfos;
    private string? _fileName;
    private bool _forceMinimumPosToZero = true;
    private bool _matchCase;
    private bool _matchWholeWord;
    private byte[] _originalText = Array.Empty<byte>();
    private bool _saved = true;
    private TabView? _tabView;
    private string _textToFind;
    private string? _textToReplace;
    private SyntaxHighlightingTextView _textView;
    private TextView _debugConsoleTextView;
    private FindReplaceWindow _findReplaceWindow;
    private TableView _debugNintTableView;
    private TableView _debugNFloatTableView;
    private TableView _debugAddressTableView;
    private TableView _debugCallStackTableView;
    private MenuBar _menuBar;
    private DebuggableContext? _debuggableContext;
    private QuatParser _quatParser;
    public QuatEditorWindow()
    {
        Instance = this;
        BorderStyle = LineStyle.None;
        _cultureInfos = Application.SupportedCultures ?? new();

        _quatParser = DebuggableQuatContextFactory.CreateNew()
            .UseDetachedMemoryModel()
            .EmitParser();

        _textView = CreateTextView();
        Add(_textView);

        _menuBar = CreateMenuBar();
        Add(_menuBar);

        var debugTableViews = CreateDebugTableViews();
        foreach(var tableView in debugTableViews) Add(tableView);

        var statusBar = CreateStatusBar();
        Add(statusBar);

        _debugConsoleTextView = CreateDebugConsoleTextView();
        Add(_debugConsoleTextView);

        Closed += (s, e) => Thread.CurrentThread.CurrentUICulture = new("en-US");

        _findReplaceWindow = CreateFindReplace();
        Add(_findReplaceWindow);
    }

    public void Load(string path)
    {
        _textView.Load(path);
        EnterDebugView(false);

    }

    public void ScrollTo(IToken token)
    {
        UpdateDebugTableViews();
        _textView.SetFocus();
        var size = _textView.GetContentSize();
        _textView.ScrollTo(token.Location.Line - (size.Height / 2));
        _textView.HighlightToken = token;
    }

    public bool DebugBreakOnNext => _debugBreakOnNext;

    private bool _debugBreakOnNext = false;
    private void SetBreakOnNext(bool breakOnNext = true)
    {
        _debugBreakOnNext = breakOnNext;
    }
    private bool _debuggingActive = false;
    private void RunDebug(bool step)
    {
        Save();
        if (_fileName == null) return;
        var context = DebuggableQuatContextFactory.CreateNew()
            .UseDetachedMemoryModel()
            .CreateContext(_fileName, out var errors);
        if (errors.Any())
        {
            ScrollTo(errors.First().Token);
            MessageBox.ErrorQuery("Parsing Error!", string.Join("\r\n", errors.Select(x => x.Message)), "Ok");
            return;
        }

        EnterDebugView(step);
        _debuggableContext = (DebuggableContext)context;
        try
        {
            _debuggableContext.LookupAndRun("Main");
            _debuggableContext.Dispose();
            _debuggableContext = null;
            ExitDebugView();
            if (Running == false) Application.Run(this);
        }
        catch (Exception ex)
        {
            if (Running == false) Application.Run(this);
            MessageBox.ErrorQuery("Error!", ex.Message, "Ok");
            ExitDebugView();
            return;
        }
    }

    public void WriteConsoleMessage(string message)
    {
        _debugConsoleTextView.Text += message;
        Application.Refresh();
    }

    #region WindowOperationsBoilerplate

    private void EnterDebugView(bool step)
    {
        SetBreakOnNext(step);
        ShowDebugConsoleTextView();
        _debugAddressTableView.Visible = true;
        _debugNintTableView.Visible = true;
        _debugNFloatTableView.Visible = true;
        _debugCallStackTableView.Visible = true;
        _debuggingActive = true;
        _menuBar.Menus = CreateDebugMenuBarItems();
        UpdateDebugTableViews();
        Application.Refresh();
    }

    private void ExitDebugView()
    {
        _debuggingActive = false;
        _debugAddressTableView.Visible = false;
        _debugNintTableView.Visible = false;
        _debugNFloatTableView.Visible = false;
        _debugCallStackTableView.Visible = false;
        _menuBar.Menus = CreateMenuBarItems();
        _textView.HighlightToken = null;
        HideDebugConsoleTextView();
        SetBreakOnNext(false);
        Application.Refresh();
    }

    private void ShowDebugConsoleTextView()
    {
        _textView.Width = Dim.Percent(50);
        _textView.Height = Dim.Percent(75);
        _debugConsoleTextView.Visible = true;
    }

    private void HideDebugConsoleTextView()
    {
        _textView.Width = Dim.Percent(100);
        _textView.Height = Dim.Fill(1);
        _debugConsoleTextView.Visible = false;
    }

    private void UpdateDebugTableViews()
    {

        _debugNintTableView.Table = new EnumerableTableSource<nint>(
            _debuggableContext?.ValueStack.Reverse().ToList() ?? new(),
            new Dictionary<string, Func<nint, object>>() {
                                { "Value",(p)=>p},
                                { "svalue",(p)=>p.GetStringValueOrNull(_debuggableContext, 24) ?? ""},
            });

        _debugNFloatTableView.Table = new EnumerableTableSource<NFloat>(
            _debuggableContext?.FloatStack.Reverse().ToList() ?? new(),
            new Dictionary<string, Func<NFloat, object>>() {
                                        { "FValue",(p)=> p },
            });

        _debugAddressTableView.Table = new EnumerableTableSource<(int, Grammar)>(
            _debuggableContext?.AddressStack.Reverse().ToList() ?? new(),
            new Dictionary<string, Func<(int, Grammar), object>>() {
                                { "Address",(p)=>p.Item1},
                                { "Rel",(p)=> p.Item2.Name},
            });

        _debugCallStackTableView.Table = new EnumerableTableSource<Grammar>(
            _debuggableContext?.CallStack.Reverse().ToList() ?? new(),
            new Dictionary<string, Func<Grammar, object>>() {
                                { "Call",(p)=>p.Name},
            });
    }


    public SyntaxHighlightingTextView CreateTextView()
    {
        return new(_quatParser)
        {
            X = 0,
            Y = 1,
            Width = Dim.Percent(100),
            Height = Dim.Fill(1),
        };
    }

    private MenuBar CreateMenuBar()
    {
        return new MenuBar
        {
            Menus = CreateMenuBarItems()
        };
    }

    private MenuBarItem[] CreateMenuBarItems()
    {
        return [
                new (
                     "_File",
                     new MenuItem []
                     {
                         new ("_New", "", () => New ()),
                         new ("_Open", "", () => Open ()),
                         new ("_Save", "", () => Save ()),
                         new ("_Save As", "", () => SaveAs ()),
                         new ("_Close", "", () => CloseFile ()),
                         null,
                         new ("_Quit", "", () => Quit ())
                     }
                    ),
                new (
                     "_Edit",
                     new MenuItem []
                     {
                         new (
                              "_Copy",
                              "",
                              () => Copy (),
                              null,
                              null,
                              KeyCode.CtrlMask | KeyCode.C
                             ),
                         new (
                              "C_ut",
                              "",
                              () => Cut (),
                              null,
                              null,
                              KeyCode.CtrlMask | KeyCode.W
                             ),
                         new (
                              "_Paste",
                              "",
                              () => Paste (),
                              null,
                              null,
                              KeyCode.CtrlMask | KeyCode.Y
                             ),
                         null,
                         new (
                              "_Find",
                              "",
                              () => Find (),
                              null,
                              null,
                              KeyCode.CtrlMask | KeyCode.S
                             ),
                         new (
                              "Find _Next",
                              "",
                              () => FindNext (),
                              null,
                              null,
                              KeyCode.CtrlMask
                              | KeyCode.ShiftMask
                              | KeyCode.S
                             ),
                         new (
                              "Find P_revious",
                              "",
                              () => FindPrevious (),
                              null,
                              null,
                              KeyCode.CtrlMask
                              | KeyCode.ShiftMask
                              | KeyCode.AltMask
                              | KeyCode.S
                             ),
                         new (
                              "_Replace",
                              "",
                              () => Replace (),
                              null,
                              null,
                              KeyCode.CtrlMask | KeyCode.R
                             ),
                         new (
                              "Replace Ne_xt",
                              "",
                              () => ReplaceNext (),
                              null,
                              null,
                              KeyCode.CtrlMask
                              | KeyCode.ShiftMask
                              | KeyCode.R
                             ),
                         new (
                              "Replace Pre_vious",
                              "",
                              () => ReplacePrevious (),
                              null,
                              null,
                              KeyCode.CtrlMask
                              | KeyCode.ShiftMask
                              | KeyCode.AltMask
                              | KeyCode.R
                             ),
                         new (
                              "Replace _All",
                              "",
                              () => ReplaceAll (),
                              null,
                              null,
                              KeyCode.CtrlMask
                              | KeyCode.ShiftMask
                              | KeyCode.AltMask
                              | KeyCode.A
                             ),
                         null,
                         new (
                              "_Select All",
                              "",
                              () => SelectAll (),
                              null,
                              null,
                              KeyCode.CtrlMask | KeyCode.T
                             )
                     }
                    ),
                new(
                    "_Debug",
                    new MenuItem[]
                    {
                        new("_Run", "", () => RunDebug(false)),
                        new("_Step", "", () => RunDebug(true)),
                    }
                    ),
            ];
    }

    private MenuBarItem[] CreateDebugMenuBarItems()
    {
        return [
                new (
                     "_File",
                     new MenuItem []
                     {
                         new ("_New", "", () => New ()),
                         new ("_Open", "", () => Open ()),
                         new ("_Save", "", () => Save ()),
                         new ("_Save As", "", () => SaveAs ()),
                         new ("_Close", "", () => CloseFile ()),
                         null,
                         new ("_Quit", "", () => Quit ())
                     }
                    ),
                new (
                     "_Edit",
                     new MenuItem []
                     {
                         new (
                              "_Copy",
                              "",
                              () => Copy (),
                              null,
                              null,
                              KeyCode.CtrlMask | KeyCode.C
                             ),
                         new (
                              "C_ut",
                              "",
                              () => Cut (),
                              null,
                              null,
                              KeyCode.CtrlMask | KeyCode.W
                             ),
                         new (
                              "_Paste",
                              "",
                              () => Paste (),
                              null,
                              null,
                              KeyCode.CtrlMask | KeyCode.Y
                             ),
                         null,
                         new (
                              "_Find",
                              "",
                              () => Find (),
                              null,
                              null,
                              KeyCode.CtrlMask | KeyCode.S
                             ),
                         new (
                              "Find _Next",
                              "",
                              () => FindNext (),
                              null,
                              null,
                              KeyCode.CtrlMask
                              | KeyCode.ShiftMask
                              | KeyCode.S
                             ),
                         new (
                              "Find P_revious",
                              "",
                              () => FindPrevious (),
                              null,
                              null,
                              KeyCode.CtrlMask
                              | KeyCode.ShiftMask
                              | KeyCode.AltMask
                              | KeyCode.S
                             ),
                         new (
                              "_Replace",
                              "",
                              () => Replace (),
                              null,
                              null,
                              KeyCode.CtrlMask | KeyCode.R
                             ),
                         new (
                              "Replace Ne_xt",
                              "",
                              () => ReplaceNext (),
                              null,
                              null,
                              KeyCode.CtrlMask
                              | KeyCode.ShiftMask
                              | KeyCode.R
                             ),
                         new (
                              "Replace Pre_vious",
                              "",
                              () => ReplacePrevious (),
                              null,
                              null,
                              KeyCode.CtrlMask
                              | KeyCode.ShiftMask
                              | KeyCode.AltMask
                              | KeyCode.R
                             ),
                         new (
                              "Replace _All",
                              "",
                              () => ReplaceAll (),
                              null,
                              null,
                              KeyCode.CtrlMask
                              | KeyCode.ShiftMask
                              | KeyCode.AltMask
                              | KeyCode.A
                             ),
                         null,
                         new (
                              "_Select All",
                              "",
                              () => SelectAll (),
                              null,
                              null,
                              KeyCode.CtrlMask | KeyCode.T
                             )
                     }
                    ),
                new(
                    "_Debug",
                    new MenuItem[]
                    {
                        new("_Run", "", () => RunDebug(false)),
                        new("_Step", "", () => RunDebug(true)),
                    }
                    ),
                new("Continue", "", () => {SetBreakOnNext(false); Quit(); }) { CanExecute = () => _debuggingActive },
                new("Step Over", "", () => {SetBreakOnNext(true); Quit(); }){ CanExecute = () => _debuggingActive },
            ];
    }



    public List<TableView> CreateDebugTableViews()
    {
        _debugNintTableView = new TableView()
        {
            X = Pos.Right(_textView),
            Y = 1,
            Width = Dim.Percent(20),
            Height = Dim.Fill(1),
            Visible = false,
            Style = new TableStyle()
            {
                ShowHeaders = true,
                ColumnStyles = new()
                {
                    {0, new(){ MinWidth = (IntPtr.Size / 2) * 5, MaxWidth = (IntPtr.Size / 2) * 5} },
                    {1, new(){ MinWidth = 5, MaxWidth = 24} }
                }
            }
        };

        _debugNFloatTableView = new TableView()
        {
            X = Pos.Right(_debugNintTableView),
            Y = 1,
            Width = Dim.Percent(10),
            Height = Dim.Fill(1),
            Visible = false,
        };

        _debugAddressTableView = new TableView()
        {
            X = Pos.Right(_debugNFloatTableView),
            Y = 1,
            Width = Dim.Percent(10),
            Height = Dim.Fill(1),
            Visible = false,
        };

        _debugCallStackTableView = new TableView()
        {
            X = Pos.Right(_debugAddressTableView),
            Y = 1,
            Width = Dim.Percent(10),
            Height = Dim.Fill(1),
            Visible = false,
        };


        _debugNintTableView.SelectedCellChanged += (sender, e) =>
        {
            if (e.NewCol != 1) return;

            if (!string.IsNullOrEmpty(e.Table[e.NewRow, 1].ToString()))
            {
                MessageBox.Query("String View", ((nint)e.Table[e.NewRow, 0]).GetStringValueOrNull(_debuggableContext, 0), "Ok");
                _debugNintTableView.SelectedColumn = 0; // allows user to click again immeidately to re-view the contents
            }

        };
        UpdateDebugTableViews();

        return [_debugNintTableView, _debugNFloatTableView, _debugAddressTableView, _debugCallStackTableView];
    }

    private TextView CreateDebugConsoleTextView()
    {
        TextView textView = new TextView()
        {
            X = 0,
            Y = Pos.Bottom(_textView),
            Width = Dim.Percent(50),
            Height = Dim.Percent(25),
            Visible = false,
            BorderStyle = LineStyle.HeavyDotted,
        };
        return textView;
    }

    private FindReplaceWindow CreateFindReplace()
    {
        _findReplaceWindow = new(_textView);
        _tabView = new()
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(0)
        };

        _tabView.AddTab(new() { DisplayText = "Find", View = CreateFindTab() }, true);
        _tabView.AddTab(new() { DisplayText = "Replace", View = CreateReplaceTab() }, false);
        _tabView.SelectedTabChanged += (s, e) => _tabView.SelectedTab.View.FocusDeepest(NavigationDirection.Forward, null);
        _findReplaceWindow.Add(_tabView);

        _findReplaceWindow.Visible = false;
        return _findReplaceWindow;
    }

    public StatusBar CreateStatusBar()
    {
        var siCursorPosition = new Shortcut(KeyCode.Null, "", null);
        _textView.UnwrappedCursorPosition += (s, e) =>
        {
            siCursorPosition.Title = $"Ln {e.Point.Y + 1}, Col {e.Point.X + 1}";
        };

        return new StatusBar(
                                       new[]
                                       {
                                           new (Application.QuitKey, $"Quit", Quit),
                                           new (Key.F2, "Open", Open),
                                           new (Key.F3, "Save", () => Save ()),
                                           new (Key.F4, "Save As", () => SaveAs ()),
                                           new (Key.Empty, $"OS Clipboard IsSupported : {Clipboard.IsSupported}", null),
                                           siCursorPosition,
                                       }
                                      )
        {
            AlignmentModes = AlignmentModes.StartToEnd | AlignmentModes.IgnoreFirstOrLast
        };
    }

    private bool CanCloseFile()
    {
        if (_textView.Text == Encoding.Unicode.GetString(_originalText))
        {
            return true;
        }

        int r = MessageBox.ErrorQuery(
                                       "Save File",
                                       $"Do you want save changes in {Title}?",
                                       "Yes",
                                       "No",
                                       "Cancel"
                                      );

        if (r == 0)
        {
            return Save();
        }

        if (r == 1)
        {
            return true;
        }

        return false;
    }

    private void CloseFile()
    {
        if (!CanCloseFile())
        {
            return;
        }

        try
        {
            _textView.CloseFile();
            New(false);
        }
        catch (Exception ex)
        {
            MessageBox.ErrorQuery("Error", ex.Message, "Ok");
        }
    }

    private void ContinueFind(bool next = true, bool replace = false)
    {
        if (!replace && string.IsNullOrEmpty(_textToFind))
        {
            Find();

            return;
        }

        if (replace
            && (string.IsNullOrEmpty(_textToFind)
                || (_findReplaceWindow == null && string.IsNullOrEmpty(_textToReplace))))
        {
            Replace();

            return;
        }

        bool found;
        bool gaveFullTurn;

        if (next)
        {
            if (!replace)
            {
                found = _textView.FindNextText(
                                                _textToFind,
                                                out gaveFullTurn,
                                                _matchCase,
                                                _matchWholeWord
                                               );
            }
            else
            {
                found = _textView.FindNextText(
                                                _textToFind,
                                                out gaveFullTurn,
                                                _matchCase,
                                                _matchWholeWord,
                                                _textToReplace,
                                                true
                                               );
            }
        }
        else
        {
            if (!replace)
            {
                found = _textView.FindPreviousText(
                                                    _textToFind,
                                                    out gaveFullTurn,
                                                    _matchCase,
                                                    _matchWholeWord
                                                   );
            }
            else
            {
                found = _textView.FindPreviousText(
                                                    _textToFind,
                                                    out gaveFullTurn,
                                                    _matchCase,
                                                    _matchWholeWord,
                                                    _textToReplace,
                                                    true
                                                   );
            }
        }

        if (!found)
        {
            MessageBox.Query("Find", $"The following specified text was not found: '{_textToFind}'", "Ok");
        }
        else if (gaveFullTurn)
        {
            MessageBox.Query(
                              "Find",
                              $"No more occurrences were found for the following specified text: '{_textToFind}'",
                              "Ok"
                             );
        }
    }

    private void Copy()
    {
        if (_textView != null)
        {
            _textView.Copy();
        }
    }

    private MenuItem CreateAllowsTabChecked()
    {
        var item = new MenuItem { Title = "Allows Tab" };
        item.CheckType |= MenuItemCheckStyle.Checked;
        item.Checked = _textView.AllowsTab;
        item.Action += () => { _textView.AllowsTab = (bool)(item.Checked = !item.Checked); };

        return item;
    }

    private MenuItem CreateCanFocusChecked()
    {
        var item = new MenuItem { Title = "CanFocus" };
        item.CheckType |= MenuItemCheckStyle.Checked;
        item.Checked = _textView.CanFocus;

        item.Action += () =>
        {
            _textView.CanFocus = (bool)(item.Checked = !item.Checked);

            if (_textView.CanFocus)
            {
                _textView.SetFocus();
            }
        };

        return item;
    }


    private void CreateDemoFile(string fileName)
    {
        var sb = new StringBuilder();

        sb.Append("Hello world.\n");
        sb.Append("This is a test of the Emergency Broadcast System.\n");

        for (var i = 0; i < 30; i++)
        {
            sb.Append(
                       $"{i} - This is a test with a very long line and many lines to test the ScrollViewBar against the TextView. - {i}\n"
                      );
        }

        StreamWriter sw = File.CreateText(fileName);
        sw.Write(sb.ToString());
        sw.Close();
    }


    private class FindReplaceWindow : Window
    {
        private TextView _textView;
        public FindReplaceWindow(TextView textView)
        {
            Title = "Find and Replace";

            _textView = textView;
            X = Pos.AnchorEnd() - 1;
            Y = 2;
            Width = 57;
            Height = 11;
            Arrangement = ViewArrangement.Movable;

            KeyBindings.Add(Key.Esc, Command.Cancel);
            AddCommand(Command.Cancel, () =>
            {
                Visible = false;

                return true;
            });
            VisibleChanged += FindReplaceWindow_VisibleChanged;
            Initialized += FindReplaceWindow_Initialized;
        }

        private void FindReplaceWindow_VisibleChanged(object? sender, EventArgs e)
        {
            if (Visible == false)
            {
                _textView.SetFocus();
            }
            else
            {
                FocusDeepest(NavigationDirection.Forward, null);
            }
        }

        private void FindReplaceWindow_Initialized(object? sender, EventArgs e)
        {
            Border.LineStyle = LineStyle.Dashed;
            Border.Thickness = new(0, 1, 0, 0);
        }
    }

    private void ShowFindReplace(bool isFind = true)
    {
        _findReplaceWindow.Visible = true;
        _findReplaceWindow.SuperView?.MoveSubviewToStart(_findReplaceWindow);
        _tabView.SetFocus();
        _tabView.SelectedTab = isFind ? _tabView.Tabs.ToArray()[0] : _tabView.Tabs.ToArray()[1];
        _tabView.SelectedTab.View.FocusDeepest(NavigationDirection.Forward, null);
    }

    private void Cut()
    {
        if (_textView != null)
        {
            _textView.Cut();
        }
    }

    private void Find() { ShowFindReplace(true); }
    private void FindNext() { ContinueFind(); }
    private void FindPrevious() { ContinueFind(false); }

    private View CreateFindTab()
    {
        var d = new View()
        {
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };

        int lblWidth = "Replace:".Length;

        var label = new Label
        {
            Width = lblWidth,
            TextAlignment = Alignment.End,

            Text = "Find:"
        };
        d.Add(label);

        SetFindText();

        var txtToFind = new TextField
        {
            X = Pos.Right(label) + 1,
            Y = Pos.Top(label),
            Width = Dim.Fill(1),
            Text = _textToFind
        };
        txtToFind.HasFocusChanging += (s, e) => txtToFind.Text = _textToFind;
        d.Add(txtToFind);

        var btnFindNext = new Button
        {
            X = Pos.Align(Alignment.Center),
            Y = Pos.AnchorEnd(),
            Enabled = !string.IsNullOrEmpty(txtToFind.Text),
            IsDefault = true,

            Text = "Find _Next"
        };
        btnFindNext.Accept += (s, e) => FindNext();
        d.Add(btnFindNext);

        var btnFindPrevious = new Button
        {
            X = Pos.Align(Alignment.Center),
            Y = Pos.AnchorEnd(),
            Enabled = !string.IsNullOrEmpty(txtToFind.Text),
            Text = "Find _Previous"
        };
        btnFindPrevious.Accept += (s, e) => FindPrevious();
        d.Add(btnFindPrevious);

        txtToFind.TextChanged += (s, e) =>
        {
            _textToFind = txtToFind.Text;
            _textView.FindTextChanged();
            btnFindNext.Enabled = !string.IsNullOrEmpty(txtToFind.Text);
            btnFindPrevious.Enabled = !string.IsNullOrEmpty(txtToFind.Text);
        };

        var ckbMatchCase = new CheckBox
        {
            X = 0,
            Y = Pos.Top(txtToFind) + 2,
            CheckedState = _matchCase ? CheckState.Checked : CheckState.UnChecked,
            Text = "Match c_ase"
        };
        ckbMatchCase.CheckedStateChanging += (s, e) => _matchCase = e.NewValue == CheckState.Checked;
        d.Add(ckbMatchCase);

        var ckbMatchWholeWord = new CheckBox
        {
            X = 0,
            Y = Pos.Top(ckbMatchCase) + 1,
            CheckedState = _matchWholeWord ? CheckState.Checked : CheckState.UnChecked,
            Text = "Match _whole word"
        };
        ckbMatchWholeWord.CheckedStateChanging += (s, e) => _matchWholeWord = e.NewValue == CheckState.Checked;
        d.Add(ckbMatchWholeWord);
        return d;
    }


    public void OpenFile(string? path)
    {
        _fileName = path;
        LoadFile();
    }

    private void LoadFile()
    {
        if (_fileName != null)
        {

            _textView.Load(_fileName);

            _originalText = Encoding.Unicode.GetBytes(_textView.Text);
            Title = _fileName;
            _saved = true;
        }
    }

    private void New(bool checkChanges = true)
    {
        if (checkChanges && !CanCloseFile())
        {
            return;
        }

        Title = "Untitled.txt";
        _fileName = null;
        _originalText = new MemoryStream().ToArray();
        _textView.Text = Encoding.Unicode.GetString(_originalText);
    }

    private void Open()
    {
        if (!CanCloseFile())
        {
            return;
        }

        List<IAllowedType> aTypes = new()
        {
            new AllowedType (
                             "Text",
                             ".txt;.bin;.xml;.json",
                             ".txt",
                             ".bin",
                             ".xml",
                             ".json"
                            ),
            new AllowedTypeAny ()
        };
        var d = new OpenDialog { Title = "Open", AllowedTypes = aTypes, AllowsMultipleSelection = false };
        Application.Run(d);

        if (!d.Canceled && d.FilePaths.Count > 0)
        {
            _fileName = d.FilePaths[0];
            LoadFile();
        }

        d.Dispose();
    }

    private void Paste()
    {
        if (_textView != null)
        {
            _textView.Paste();
        }
    }

    private void Quit()
    {
        if (!CanCloseFile())
        {
            return;
        }

        Application.RequestStop();
    }

    private void Replace() { ShowFindReplace(false); }

    private void ReplaceAll()
    {
        if (string.IsNullOrEmpty(_textToFind) || (string.IsNullOrEmpty(_textToReplace) && _findReplaceWindow == null))
        {
            Replace();

            return;
        }

        if (_textView.ReplaceAllText(_textToFind, _matchCase, _matchWholeWord, _textToReplace))
        {
            MessageBox.Query(
                              "Replace All",
                              $"All occurrences were replaced for the following specified text: '{_textToReplace}'",
                              "Ok"
                             );
        }
        else
        {
            MessageBox.Query(
                              "Replace All",
                              $"None of the following specified text was found: '{_textToFind}'",
                              "Ok"
                             );
        }
    }

    private void ReplaceNext() { ContinueFind(true, true); }
    private void ReplacePrevious() { ContinueFind(false, true); }

    private View CreateReplaceTab()
    {
        var d = new View()
        {
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };

        int lblWidth = "Replace:".Length;

        var label = new Label
        {
            Width = lblWidth,
            TextAlignment = Alignment.End,
            Text = "Find:"
        };
        d.Add(label);

        SetFindText();

        var txtToFind = new TextField
        {
            X = Pos.Right(label) + 1,
            Y = Pos.Top(label),
            Width = Dim.Fill(1),
            Text = _textToFind
        };
        txtToFind.HasFocusChanging += (s, e) => txtToFind.Text = _textToFind;
        d.Add(txtToFind);

        var btnFindNext = new Button
        {
            X = Pos.Align(Alignment.Center),
            Y = Pos.AnchorEnd(),
            Enabled = !string.IsNullOrEmpty(txtToFind.Text),
            IsDefault = true,
            Text = "Replace _Next"
        };
        btnFindNext.Accept += (s, e) => ReplaceNext();
        d.Add(btnFindNext);

        label = new()
        {
            X = Pos.Left(label),
            Y = Pos.Top(label) + 1,
            Text = "Replace:"
        };
        d.Add(label);

        SetFindText();

        var txtToReplace = new TextField
        {
            X = Pos.Right(label) + 1,
            Y = Pos.Top(label),
            Width = Dim.Fill(1),
            Text = _textToReplace
        };
        txtToReplace.TextChanged += (s, e) => _textToReplace = txtToReplace.Text;
        d.Add(txtToReplace);

        var btnFindPrevious = new Button
        {
            X = Pos.Align(Alignment.Center),
            Y = Pos.AnchorEnd(),
            Enabled = !string.IsNullOrEmpty(txtToFind.Text),
            Text = "Replace _Previous"
        };
        btnFindPrevious.Accept += (s, e) => ReplacePrevious();
        d.Add(btnFindPrevious);

        var btnReplaceAll = new Button
        {
            X = Pos.Align(Alignment.Center),
            Y = Pos.AnchorEnd(),
            Enabled = !string.IsNullOrEmpty(txtToFind.Text),
            Text = "Replace _All"
        };
        btnReplaceAll.Accept += (s, e) => ReplaceAll();
        d.Add(btnReplaceAll);

        txtToFind.TextChanged += (s, e) =>
        {
            _textToFind = txtToFind.Text;
            _textView.FindTextChanged();
            btnFindNext.Enabled = !string.IsNullOrEmpty(txtToFind.Text);
            btnFindPrevious.Enabled = !string.IsNullOrEmpty(txtToFind.Text);
            btnReplaceAll.Enabled = !string.IsNullOrEmpty(txtToFind.Text);
        };

        var ckbMatchCase = new CheckBox
        {
            X = 0,
            Y = Pos.Top(txtToFind) + 2,
            CheckedState = _matchCase ? CheckState.Checked : CheckState.UnChecked,
            Text = "Match c_ase"
        };
        ckbMatchCase.CheckedStateChanging += (s, e) => _matchCase = e.NewValue == CheckState.Checked;
        d.Add(ckbMatchCase);

        var ckbMatchWholeWord = new CheckBox
        {
            X = 0,
            Y = Pos.Top(ckbMatchCase) + 1,
            CheckedState = _matchWholeWord ? CheckState.Checked : CheckState.UnChecked,
            Text = "Match _whole word"
        };
        ckbMatchWholeWord.CheckedStateChanging += (s, e) => _matchWholeWord = e.NewValue == CheckState.Checked;
        d.Add(ckbMatchWholeWord);

        return d;
    }

    private bool Save()
    {
        if (_fileName != null)
        {
            return SaveFile(Title, _fileName);
        }

        return SaveAs();
    }

    private bool SaveAs()
    {
        List<IAllowedType> aTypes = new()
        {
            new AllowedType ("Text Files", ".txt", ".bin", ".xml"), new AllowedTypeAny ()
        };
        var sd = new SaveDialog { Title = "Save file", AllowedTypes = aTypes };

        sd.Path = Title;
        Application.Run(sd);
        bool canceled = sd.Canceled;
        string path = sd.Path;
        string fileName = sd.FileName;
        sd.Dispose();

        if (!canceled)
        {
            if (File.Exists(path))
            {
                if (MessageBox.Query(
                                      "Save File",
                                      "File already exists. Overwrite any way?",
                                      "No",
                                      "Ok"
                                     )
                    == 1)
                {
                    return SaveFile(fileName, path);
                }

                _saved = false;

                return _saved;
            }

            return SaveFile(fileName, path);
        }

        _saved = false;

        return _saved;
    }

    private bool SaveFile(string title, string file)
    {
        try
        {
            Title = title;
            _fileName = file;
            File.WriteAllText(_fileName, _textView.Text);
            _originalText = Encoding.Unicode.GetBytes(_textView.Text);
            _saved = true;
            _textView.ClearHistoryChanges();
            MessageBox.Query("Save File", "File was successfully saved.", "Ok");
        }
        catch (Exception ex)
        {
            MessageBox.ErrorQuery("Error", ex.Message, "Ok");

            return false;
        }

        return true;
    }

    private void SelectAll() { _textView.SelectAll(); }

    private void SetFindText()
    {
        _textToFind = !string.IsNullOrEmpty(_textView.SelectedText) ? _textView.SelectedText :
                      string.IsNullOrEmpty(_textToFind) ? "" : _textToFind;

        _textToReplace = string.IsNullOrEmpty(_textToReplace) ? "" : _textToReplace;
    }

    #endregion
}