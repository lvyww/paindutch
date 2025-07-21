using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Documents;
using System.Windows.Markup;


using Diff;

using System.Reflection;
using Interop.UIAutomationClient;

using Net;
using LibB;



namespace TypeB
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 






    public partial class MainWindow : Window
    {


        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left; //最左坐标
            public int Top; //最上坐标
            public int Right; //最右坐标
            public int Bottom; //最下坐标
        }
        enum MouseEventFlag : uint
        {
            Move = 0x0001,
            LeftDown = 0x0002,
            LeftUp = 0x0004,
            RightDown = 0x0008,
            RightUp = 0x0010,
            MiddleDown = 0x0020,
            MiddleUp = 0x0040,
            XDown = 0x0080,
            XUp = 0x0100,
            Wheel = 0x0800,
            VirtualDesk = 0x4000,
            Absolute = 0x8000
        }

        [DllImport("User32")]
        public extern static bool GetCursorPos(ref System.Drawing.Point cPoint);
        [DllImport("user32.dll", EntryPoint = "GetForegroundWindow")]
        public static extern IntPtr fGetForegroundWindow();
        [DllImport("user32.dll")]
        static extern void mouse_event(MouseEventFlag flags, int dx, int dy, uint data, IntPtr extraInfo);

        [DllImport("User32")]
        public extern static void SetCursorPos(int x, int y);
        #region 热键

        [DllImport("user32.dll")]

        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);




        #region dll
        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", EntryPoint = "GetWindowText")]
        public static extern int GetWindowText(int hwnd, StringBuilder lpString, int cch);



        [DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "GetClassName")]
        public static extern int GetClassName(int hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll", EntryPoint = "SwitchToThisWindow")]
        private static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);



        [DllImport("user32.dll", EntryPoint = "SendMessageA")]
        private static extern int SendMessage(System.IntPtr ptr, int wMsg, int wParam, int lParam);
        //输入法
        [DllImport("imm32.dll")]
        public static extern IntPtr ImmGetContext(IntPtr hWnd);

        [DllImport("imm32.dll")]
        public static extern bool ImmGetConversionStatus(IntPtr hIMC,
        ref int conversion, ref int sentence);

        [DllImport("imm32.dll")]
        public static extern bool ImmSetConversionStatus(IntPtr hIMC, int conversion, int sentence);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern short GetKeyState(int vKey);
        //      public const int VK_BACK = 0x08; //BACKSPACE 密钥

        #endregion




        private const int HOTKEY_ID = 9000;

        //Modifiers:
        private const int MOD_NONE = 0x0000; //[NONE]
        private const int MOD_ALT = 0x0001; //ALT
        private const int MOD_CONTROL = 0x0002; //CTRL
        private const int MOD_SHIFT = 0x0004; //SHIFT
        private const int MOD_WIN = 0x0008; //WINDOWS





        //CAPS LOCK:

        private const int VK_A = 65;
        private const int VK_B = 66;
        private const int VK_C = 67;
        private const int VK_D = 68;
        private const int VK_E = 69;
        private const int VK_F = 70;
        private const int VK_G = 71;
        private const int VK_H = 72;
        private const int VK_I = 73;

        private const int VK_J = 74;
        private const int VK_K = 75;
        private const int VK_L = 76;
        private const int VK_M = 77;
        private const int VK_N = 78;
        private const int VK_O = 79;
        private const int VK_P = 80;
        private const int VK_Q = 81;
        private const int VK_R = 82;
        private const int VK_S = 83;
        private const int VK_T = 84;
        private const int VK_U = 85;
        private const int VK_V = 86;
        private const int VK_W = 87;
        private const int VK_X = 88;
        private const int VK_Y = 89;
        private const int VK_Z = 90;




        #region
        const int VK_LBUTTON = 0x01; //鼠标左键
        const int VK_RBUTTON = 0x02; //鼠标右键
        const int VK_CANCEL = 0x03; //控制中断处理
        const int VK_MBUTTON = 0x04; //中间鼠标按钮 (三键鼠标)
        const int VK_XBUTTON1 = 0x05; //X1 鼠标按钮
        const int VK_XBUTTON2 = 0x06; //X2 鼠标按钮

        const int VK_BACK = 0x08; //BACKSPACE 密钥
        const int VK_TAB = 0x09; //Tab 键

        const int VK_CLEAR = 0x0C; //CLEAR 键
        const int VK_RETURN = 0x0D; //Enter 键

        const int VK_SHIFT = 0x10; //SHIFT 键
        const int VK_CONTROL = 0x11; //Ctrl 键
        const int VK_MENU = 0x12; //Alt 键
        const int VK_PAUSE = 0x13; //PAUSE 键
        const int VK_CAPITAL = 0x14; //CAPS LOCK 键
        const int VK_KANA = 0x15; //IME Kana 模式
        const int VK_HANGUEL = 0x15; //IME 朝鲜文库埃尔模式 (保持兼容性;使用 VK_HANGUL)
        const int VK_HANGUL = 0x15; //IME Hanguel 模式
        const int VK_IME_ON = 0x16; //IME On
        const int VK_JUNJA = 0x17; //IME Junja 模式
        const int VK_FINAL = 0x18; //IME 最终模式
        const int VK_HANJA = 0x19; //IME Hanja 模式
        const int VK_KANJI = 0x19; //IME Kanji 模式
        const int VK_IME_OFF = 0x1A; //IME 关闭
        const int VK_ESCAPE = 0x1B; //ESC 键
        const int VK_CONVERT = 0x1C; //IME 转换
        const int VK_NONCONVERT = 0x1D; //IME 不转换
        const int VK_ACCEPT = 0x1E; //IME 接受
        const int VK_MODECHANGE = 0x1F; //IME 模式更改请求
        const int VK_SPACE = 0x20; //空格键
        const int VK_PRIOR = 0x21; //PAGE UP 键
        const int VK_NEXT = 0x22; //PAGE DOWN 键
        const int VK_END = 0x23; //END 键
        const int VK_HOME = 0x24; //HOME 键
        const int VK_LEFT = 0x25; //向左键
        const int VK_UP = 0x26; //向上键
        const int VK_RIGHT = 0x27; //向右键
        const int VK_DOWN = 0x28; //向下键
        const int VK_SELECT = 0x29; //SELECT 键
        const int VK_PRINT = 0x2A; //PRINT 键
        const int VK_EXECUTE = 0x2B; //EXECUTE 键
        const int VK_SNAPSHOT = 0x2C; //打印屏幕键
        const int VK_INSERT = 0x2D; //INS 密钥
        const int VK_DELETE = 0x2E; //DEL 键
        const int VK_HELP = 0x2F; //帮助密钥

        const int VK_LWIN = 0x5B; //左Windows键 (自然键盘)
        const int VK_RWIN = 0x5C; //右Windows键 (自然键盘)
        const int VK_APPS = 0x5D; //应用程序键 (自然键盘)

        const int VK_SLEEP = 0x5F; //计算机休眠键
        const int VK_NUMPAD0 = 0x60; //数字键盘 0 键
        const int VK_NUMPAD1 = 0x61; //数字键盘 1 键
        const int VK_NUMPAD2 = 0x62; //数字键盘 2 键
        const int VK_NUMPAD3 = 0x63; //数字键盘 3 键
        const int VK_NUMPAD4 = 0x64; //数字键盘 4 键
        const int VK_NUMPAD5 = 0x65; //数字键盘 5 键
        const int VK_NUMPAD6 = 0x66; //数字键盘 6 键
        const int VK_NUMPAD7 = 0x67; //数字键盘 7 键
        const int VK_NUMPAD8 = 0x68; //数字键盘 8 键
        const int VK_NUMPAD9 = 0x69; //数字键盘 9 键
        const int VK_MULTIPLY = 0x6A; //乘键
        const int VK_ADD = 0x6B; //添加密钥
        const int VK_SEPARATOR = 0x6C; //分隔符键
        const int VK_SUBTRACT = 0x6D; //减去键
        const int VK_DECIMAL = 0x6E; //十进制键
        const int VK_DIVIDE = 0x6F; //除键
        const int VK_F1 = 0x70; //F1 键
        const int VK_F2 = 0x71; //F2 键
        const int VK_F3 = 0x72; //F3 键
        const int VK_F4 = 0x73; //F4 键
        const int VK_F5 = 0x74; //F5 键
        const int VK_F6 = 0x75; //F6 键
        const int VK_F7 = 0x76; //F7 键
        const int VK_F8 = 0x77; //F8 键
        const int VK_F9 = 0x78; //F9 键
        const int VK_F10 = 0x79; //F10 键
        const int VK_F11 = 0x7A; //F11 键
        const int VK_F12 = 0x7B; //F12 键
        const int VK_F13 = 0x7C; //F13 键
        const int VK_F14 = 0x7D; //F14 键
        const int VK_F15 = 0x7E; //F15 键
        const int VK_F16 = 0x7F; //F16 键
        const int VK_F17 = 0x80; //F17 键
        const int VK_F18 = 0x81; //F18 键
        const int VK_F19 = 0x82; //F19 键
        const int VK_F20 = 0x83; //F20 键
        const int VK_F21 = 0x84; //F21 键
        const int VK_F22 = 0x85; //F22 键
        const int VK_F23 = 0x86; //F23 键
        const int VK_F24 = 0x87; //F24 键

        const int VK_NUMLOCK = 0x90; //NUM LOCK 密钥
        const int VK_SCROLL = 0x91; //SCROLL LOCK 键

        const int VK_LSHIFT = 0xA0; //左 SHIFT 键
        const int VK_RSHIFT = 0xA1; //右 SHIFT 键
        const int VK_LCONTROL = 0xA2; //左 Ctrl 键
        const int VK_RCONTROL = 0xA3; //右 Ctrl 键
        const int VK_LMENU = 0xA4; //左 Alt 键
        const int VK_RMENU = 0xA5; //右 ALT 键
        const int VK_BROWSER_BACK = 0xA6; //浏览器后退键
        const int VK_BROWSER_FORWARD = 0xA7; //浏览器前进键
        const int VK_BROWSER_REFRESH = 0xA8; //浏览器刷新键
        const int VK_BROWSER_STOP = 0xA9; //浏览器停止键
        const int VK_BROWSER_SEARCH = 0xAA; //浏览器搜索键
        const int VK_BROWSER_FAVORITES = 0xAB; //浏览器收藏键
        const int VK_BROWSER_HOME = 0xAC; //浏览器“开始”和“主页”键
        const int VK_VOLUME_MUTE = 0xAD; //静音键
        const int VK_VOLUME_DOWN = 0xAE; //音量减小键
        const int VK_VOLUME_UP = 0xAF; //音量增加键
        const int VK_MEDIA_NEXT_TRACK = 0xB0; //下一曲目键
        const int VK_MEDIA_PREV_TRACK = 0xB1; //上一曲目键
        const int VK_MEDIA_STOP = 0xB2; //停止媒体键
        const int VK_MEDIA_PLAY_PAUSE = 0xB3; //播放/暂停媒体键
        const int VK_LAUNCH_MAIL = 0xB4; //启动邮件键
        const int VK_LAUNCH_MEDIA_SELECT = 0xB5; //选择媒体键
        const int VK_LAUNCH_APP1 = 0xB6; //启动应用程序 1 键
        const int VK_LAUNCH_APP2 = 0xB7; //启动应用程序 2 键

        const int VK_OEM_1 = 0xBA; //用于其他字符;它可能因键盘而异。 对于美国标准键盘，“;：”键
        const int VK_OEM_PLUS = 0xBB; //对于任何国家/地区，“+”键
        const int VK_OEM_COMMA = 0xBC; //对于任何国家/地区，“，键
        const int VK_OEM_MINUS = 0xBD; //对于任何国家/地区，“-”键
        const int VK_OEM_PERIOD = 0xBE; //对于任何国家/地区，“.”键
        const int VK_OEM_2 = 0xBF; //用于其他字符;它可能因键盘而异。 对于美国标准键盘，“/？” key
        const int VK_OEM_3 = 0xC0; //用于其他字符;它可能因键盘而异。 对于美国标准键盘，“~”键


        const int VK_OEM_4 = 0xDB; //用于其他字符;它可能因键盘而异。 对于美国标准键盘，“[{”键
        const int VK_OEM_5 = 0xDC; //用于其他字符;它可能因键盘而异。 对于美国标准键盘，“\|”键
        const int VK_OEM_6 = 0xDD; //用于其他字符;它可能因键盘而异。 对于美国标准键盘，“]}”键
        const int VK_OEM_7 = 0xDE; //用于其他字符;它可能因键盘而异。 对于美国标准键盘，“单引号/双引号”键
        const int VK_OEM_8 = 0xDF; //用于其他字符;它可能因键盘而异。


        const int VK_OEM_102 = 0xE2; //<>美国标准键盘上的键，或\\|非美国 102 键键盘上的键

        const int VK_PROCESSKEY = 0xE5; //IME PROCESS 密钥

        const int VK_PACKET = 0xE7; //用于将 Unicode 字符当作键击传递。 该 VK_PACKET 键是用于非键盘输入方法的 32 位虚拟键值的低字。 有关详细信息，请参阅“备注”，以及KEYBDINPUTSendInputWM_KEYDOWNWM_KEYUP


        const int VK_ATTN = 0xF6; //Attn 键
        const int VK_CRSEL = 0xF7; //CrSel 键
        const int VK_EXSEL = 0xF8; //ExSel 密钥
        const int VK_EREOF = 0xF9; //擦除 EOF 密钥
        const int VK_PLAY = 0xFA; //播放键
        const int VK_ZOOM = 0xFB; //缩放键
        const int VK_NONAME = 0xFC; //预留
        const int VK_PA1 = 0xFD; //PA1 键
        const int VK_OEM_CLEAR = 0xFE; //清除键

        #endregion


        private HwndSource source;

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            IntPtr handle = new WindowInteropHelper(this).Handle;
            source = HwndSource.FromHwnd(handle);
            source.AddHook(HwndHook);

  //          RegisterHotKey(handle, HOTKEY_ID, MOD_NONE, VK_F5); //
            RegisterHotKey(handle, HOTKEY_ID, MOD_NONE, VK_F4); //
            RegisterHotKey(handle, HOTKEY_ID, MOD_CONTROL, VK_E); //
            RegisterHotKey(handle, HOTKEY_ID, MOD_NONE, VK_MBUTTON); //中键
        }


        
        public static MainWindow Current
        {
            get
            {
                foreach (var s in App.Current.Windows)
                {
                    if (s is MainWindow)
                    {
                        return (MainWindow)s;

                    }

                }

                return null;
            }

        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;

            switch (msg)
            {
                case WM_HOTKEY:
                    switch (wParam.ToInt32())
                    {
                        case HOTKEY_ID:
                            int vkey = (((int)lParam >> 16) & 0xFFFF);
                            if (vkey == VK_E)
                            {
                                HotKeyCtrlE();
                            }
                            else if (vkey == VK_F4)
                            {
                                HotKeyF4();
                            }
 //                           else if (vkey == VK_F5)
 //                           {
 //                               HotKeyF5();
  //                          }
                            else if (vkey == VK_MBUTTON)
                            {
                                HotKeyMButton();
                            }
                            handled = true;
                            break;
                    }
                    break;
            }
            return IntPtr.Zero;
        }



        #endregion








        Stopwatch sw = new Stopwatch();

        private enum UpdateLevel
        {

            Progress = 1,
            PageArrange = 2
        };
        //       static Brush myBrush = new SolidColorBrush(Color.FromArgb(0xff, 0x95, 0xb0, 0xe3));


        static string[] NoneHeadPuncts =
        {
            "=",
            "，",
            "-",
            "。",
            "·",

            "、",
            "】",
            "、",
            "；",
            "）",
            "！",
            "@",
            "#",
            "￥",
            "%",
            "…",
            "&",
            "*",
            "”",
           "’",
           "'",
            "+",

            "—",
            "》",
            "~",

            "|",
            "",
            "？",
            "：",
            "=",
            ",",
            "-",
            ".",
            "`",

            "\\",
            "]",
            "/",
            ";" ,
            "\'",
            ")",
            "!",
            "@",
            "#",
            "$",
            "%",
            "^",
            "&",
            "*",
            "(",
            "+",

            "_",
            ">",
            "~",

            "|",
            "",
            "?",
            ":" ,
            "\"",
            " "
        };

        static string[] NoneTailPuncts =
        {
            "【",
            "（",
            "《",
            "{",
            "[",
            "<",
            "{",
            "“",
           "‘",
          
        };


        static List<string> AZ = new List<string> { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "'" };

        private void UpdateDisplay(UpdateLevel updateLevel)
        {

            if (IsLookingType && StateManager.LastType)
            {
                BlindDiff();
                return;
            }



            if (updateLevel >= UpdateLevel.PageArrange)
                PageReArrange();

            if (updateLevel >= UpdateLevel.Progress)
                PageProgressUpdate();


            void PageReArrange()
            {

                double y = (this.ActualHeight - 95.74 - 0.5 - 15) * 0.75 - (expd.IsExpanded ? 145 : 0);
                double x = (this.ActualWidth - 52);
                Paginator.ArrangePage(x, y, Config.GetDouble("字体大小"), TextInfo.Words.Count);

                TextInfo.PageNum = -1;
            }

            void PageProgressUpdate()
            {
                //计算页码
                // int nextToType = TextInfo.wordStates.IndexOf(WordStates.NO_TYPE);
                int nextToType = new StringInfo(TbxInput.Text).LengthInTextElements;
                if (nextToType >= TextInfo.Words.Count)
                    nextToType = TextInfo.Words.Count - 1;

                /*
                if (nextToType == -1)
                {
                    nextToType = TextInfo.Words.Count - 1;
                }
                */
                //         int newPageNum = Paginator.GetPageNum(nextToType);

                //   if (newPageNum == -1)
                //     return;

                //  if (newPageNum != TextInfo.PageNum)
                if (TextInfo.PageNum == -1 || nextToType > Paginator.Pages[TextInfo.PageNum].BodyEnd || nextToType < Paginator.Pages[TextInfo.PageNum].BodyStart)
                {

                    //清空显示
                    TbDispay.Children.Clear();
                    ScDisplay.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
                    TextInfo.Blocks.Clear();



                    int pn = Paginator.GetPageNum(nextToType);
                    if (pn < 0)
                        return;
                    if (Paginator.Pages.Count < pn)
                        return;
                    Page p = Paginator.Pages[pn];



                    var fm = GetCurrentFontFamily();// new FontFamily(Config.GetString("字体"));


                    double fs = Config.GetDouble("字体大小");
                    double height = fs * (1.0 + Config.GetDouble("行距"));
                    double MinWidth = Config.GetDouble("字体大小") * 0.9;

                    ScDisplay.FontFamily = fm;
                    ScDisplay.Foreground = Colors.DisplayForeground;
                    ScDisplay.FontSize = fs; TbAcc.FontSize = fs / 2.3;
                    TbxInput.FontFamily = fm;
                    if (p.HeadEnd >= 0)
                        TextInfo.PageStartIndex = p.HeadStart;
                    else
                        TextInfo.PageStartIndex = p.BodyStart;


                    //添加头
                    if (p.HeadEnd >= 0)
                    {

                        for (int i = p.HeadStart; i <= p.HeadEnd; i++)
                        {


                            TextBlock tb = new TextBlock();
                            tb.Text = TextInfo.Words[i];

                            tb.Height = height;
                            if (tb.Text == "“" || tb.Text == "‘")
                            {
                                tb.MinWidth = MinWidth;
                                tb.TextAlignment = TextAlignment.Right;
                            }
                            else if (tb.Text == "”" || tb.Text == "’")
                            {
                                tb.MinWidth = MinWidth;
                                tb.TextAlignment = TextAlignment.Left;
                            }

                            TextInfo.Blocks.Add(tb);

                        }
                    }


                    if (p.BodyEnd >= 0)
                    {
                        for (int i = p.BodyStart; i <= p.BodyEnd; i++)
                        {

                            TextBlock tb = new TextBlock();
                            tb.Text = TextInfo.Words[i];
                            tb.Height = height;
                            if (tb.Text == "“" || tb.Text == "‘")
                            {
                                tb.MinWidth = MinWidth;
                                tb.TextAlignment = TextAlignment.Right;
                            }
                            else if (tb.Text == "”" || tb.Text == "’")
                            {
                                tb.MinWidth = MinWidth;
                                tb.TextAlignment = TextAlignment.Left;
                            }
                            TextInfo.Blocks.Add(tb);

                        }
                    }

                    if (p.FootEnd >= 0)
                    {
                        for (int i = p.FootStart; i <= p.FootEnd; i++)
                        {
                            TextBlock tb = new TextBlock();
                            tb.Text = TextInfo.Words[i];

                            tb.Height = height;
                            if (tb.Text == "“" || tb.Text == "‘")
                            {
                                tb.MinWidth = MinWidth;
                                tb.TextAlignment = TextAlignment.Right;
                            }
                            else if (tb.Text == "”" || tb.Text == "’")
                            {
                                tb.MinWidth = MinWidth;
                                tb.TextAlignment = TextAlignment.Left;
                            }
                            tb.TextDecorations = TextDecorations.Underline;
                            TextInfo.Blocks.Add(tb);


                        }
                    }


                    if (TextInfo.Blocks.Count >= 3) //标点打包，不在行首显示
                    {
                        int total = TextInfo.Blocks.Count;

                        //字符序列是否不允许出现在首尾
                        bool[] nohead = new bool[TextInfo.Blocks.Count];
                        bool[] notail = new bool[TextInfo.Blocks.Count];
                        for (int i = 0; i < TextInfo.Blocks.Count; i++)
                        {
                            nohead[i] = NoneHeadPuncts.Contains(TextInfo.Blocks[i].Text);
                            notail[i] = NoneTailPuncts.Contains(TextInfo.Blocks[i].Text);
                        }

                        for (int i = 1; i < TextInfo.Blocks.Count - 1; i++)
                        {
                            string c2 = TextInfo.Blocks[i].Text;
                            string c1 = TextInfo.Blocks[i-1].Text;
                            string c3 = TextInfo.Blocks[i + 1].Text;

                            if (AZ.Contains(c2))
                            {
                                if (AZ.Contains(c1))
                                    nohead[i] = true;

                                if (AZ.Contains(c3))
                                    notail[i] = true;

                            }

                        }


                        bool[] inpack = new bool[TextInfo.Blocks.Count]; //是否打包


                        bool[] isPackHead = new bool[TextInfo.Blocks.Count]; //是否是包头
                        bool[] isPackTail = new bool[TextInfo.Blocks.Count]; // 是否是包尾,





                        inpack[0] = notail[0] || nohead[1];
                        isPackHead[0] = inpack[0];
                        isPackTail[0] = false;

                        inpack[total - 1] = nohead[total - 1] || notail[total - 2];
                        isPackHead[total - 1] = false;
                        isPackTail[total - 1] = inpack[total - 1];

                        for (int i = 1; i < TextInfo.Blocks.Count - 1; i++)
                        {
                            inpack[i] = nohead[i] || notail[i] || notail[i - 1] || nohead[i + 1];

                            isPackHead[i] = !nohead[i] && inpack[i] && !notail[i - 1];

                            isPackTail[i] = !notail[i] && inpack[i] && !nohead[i + 1];

                        }


                        StackPanel lstk = new StackPanel();
                        for (int i = 0; i < TextInfo.Blocks.Count; i++)
                        {
                            if (isPackHead[i])
                            {
                                lstk = new StackPanel();
                                lstk.Orientation = Orientation.Horizontal;
                                lstk.Width = double.NaN;
                                lstk.Height = double.NaN;



                                lstk.Children.Add(TextInfo.Blocks[i]);
                            }
                            else if (isPackTail[i])
                            {
                                lstk.Children.Add(TextInfo.Blocks[i]);
                                TbDispay.Children.Add(lstk);
                            }
                            else if (inpack[i])
                            {
                                lstk.Children.Add(TextInfo.Blocks[i]);
                            }
                            else
                            {
                                TbDispay.Children.Add(TextInfo.Blocks[i]);
                            }

                        }


                    }
                    else
                    {
                        foreach (var tb in TextInfo.Blocks)
                            TbDispay.Children.Add(tb);
                    }


                    TextInfo.PageNum = pn;

                    TextInfo.BlocksStates.Clear();
                    TextInfo.Blocks.ForEach(t => TextInfo.BlocksStates.Add(WordStates.NO_TYPE));
                }



                //设置背景色
                if (!IsLookingType || IsBlindType)
                    for (int i = 0; i < TextInfo.Blocks.Count; i++)
                    {
                        if (TextInfo.BlocksStates[i] != TextInfo.wordStates[TextInfo.PageStartIndex + i])
                        {
                            switch (TextInfo.wordStates[TextInfo.PageStartIndex + i])
                            {
                                case WordStates.WRONG:
                                    TextInfo.Blocks[i].Background = IsBlindType ? Colors.CorrectBackground : Colors.IncorrectBackground;

                                    break;
                                case WordStates.RIGHT:
                                    TextInfo.Blocks[i].Background = Colors.CorrectBackground;
                                    break;
                                case WordStates.NO_TYPE:
                                    TextInfo.Blocks[i].Background = null;
                                    break;
                                default:
                                    break;

                            }
                            TextInfo.BlocksStates[i] = TextInfo.wordStates[TextInfo.PageStartIndex + i];

                        }
                    }

                /*
                //更新实时tips
                if (Config.GetBool("增强速度提示"))
                {
                    if (double.IsNaN(Score.GetValidSpeed()))
                    {
                        SldZoom.Background = null;
                    }
                    else
                    {
                        int acc = (int)(Score.GetValidSpeed() * 100) - 94;

                        if (acc < 0) acc = 0;
                        if (acc >5) acc = 5;

                        acc = 5 - acc;

                        SldZoom.Background = Colors.Levels[acc];
                    }


                }

                */

                if (Config.GetBool("允许滚动") || nextToType == 0)
                {


                    double fs = Config.GetDouble("字体大小");
                    double fh = fs * (1.0 + Config.GetDouble("行距"));

                    bool ScrollCondition = false;
                    int NextBlockIndex = (nextToType - TextInfo.PageStartIndex);





                    double pos = TextInfo.Blocks[NextBlockIndex].TranslatePoint(new Point(0, 0), TextInfo.Blocks[0]).Y + TextInfo.Blocks[NextBlockIndex].ActualHeight / 2;



                    double center = ScDisplay.ViewportHeight * 0.48;
                    double offset = pos - center;

                    if (offset < 0)
                        offset = 0;
                    if (offset > ScDisplay.ScrollableHeight)
                        offset = ScDisplay.ScrollableHeight;

                    if (NextBlockIndex > 0 && TextInfo.Blocks[NextBlockIndex].TranslatePoint(new Point(0, 0), TextInfo.Blocks[NextBlockIndex - 1]).X <= 0)
                        ScrollCondition = true;



                    if (Math.Abs(ScDisplay.VerticalOffset - offset) > fh * 0.8)
                        ScrollCondition = true;

                    if (nextToType == 0)
                        ScrollCondition = true;
                    if (ScrollCondition)
                        ScDisplay.ScrollToVerticalOffset(offset);

                    //跟随显示提示
                    bool showAccuracy = Config.GetBool("增强键准提示") && !double.IsNaN(Score.GetAccuracy());
                    bool showSpeed = Config.GetBool("增强速度提示") && !double.IsNaN(Score.GetValidSpeed());
                    
                    if (!showAccuracy && !showSpeed)
                    {
                        if (TbAcc.Visibility == Visibility.Visible)
                            TbAcc.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        if (TbAcc.Visibility == Visibility.Hidden)
                            TbAcc.Visibility = Visibility.Visible;
                        
                        double AccLeft = TextInfo.Blocks[NextBlockIndex].TranslatePoint(new Point(0, 0), TextInfo.Blocks[0]).X + TextInfo.Blocks[NextBlockIndex].ActualWidth / 3;
                        double AccTop = TextInfo.Blocks[NextBlockIndex].TranslatePoint(new Point(0, 0), TextInfo.Blocks[0]).Y + TextInfo.Blocks[NextBlockIndex].ActualHeight - offset;
                        Canvas.SetTop(TbAcc, AccTop);
                        Canvas.SetLeft(TbAcc, AccLeft);
                        
                        // 根据启用的功能显示不同内容
                        if (showAccuracy && showSpeed)
                        {
                            // 两个功能都启用，显示组合信息
                            TbAcc.Text = $"{Score.GetValidSpeed():F0},{(100 * Score.GetAccuracy()):F1}%";
                            // 使用速度颜色作为主色调
                            TbAcc.Foreground = Colors.GetSpeedColor(Score.GetValidSpeed());
                        }
                        else if (showSpeed)
                        {
                            // 只显示速度
                            TbAcc.Text = Score.GetValidSpeed().ToString("F2");
                            TbAcc.Foreground = Colors.GetSpeedColor(Score.GetValidSpeed());
                        }
                        else if (showAccuracy)
                        {
                            // 只显示准确率
                            TbAcc.Text = (100 * Score.GetAccuracy()).ToString("F2");
                            TbAcc.Foreground = Colors.GetAccColor(Score.GetAccuracy());
                        }
                    }

                    

                }







            }




            void BlindDiff()
            {


                TbDispay.Children.Clear();
                ScDisplay.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                TextBlock tb = new TextBlock();
                tb.FontSize = Config.GetDouble("字体大小");
                tb.FontFamily = GetCurrentFontFamily();// new FontFamily(Config.GetString("字体"));
                tb.Background = BdDisplay.Background;
                tb.TextWrapping = TextWrapping.Wrap;
                tb.HorizontalAlignment = HorizontalAlignment.Stretch;
                tb.VerticalAlignment = VerticalAlignment.Top;
                tb.Foreground = Colors.DisplayForeground;


                string currentMatchText = string.Concat(TextInfo.Words);



                string t1 = currentMatchText.Replace('”', '\"').Replace('“', '\"').Replace('‘', '\'').Replace('’', '\'');
                string t2 = TbxInput.Text.Replace('”', '\"').Replace('“', '\"').Replace('‘', '\'').Replace('’', '\'');
                List<DiffRes> diffs = DiffTool.Diff(t1, t2);
                int counter = 0;
                foreach (var df in diffs)
                {
                    Run r = new Run();

                    switch (df.Type)
                    {
                        case DiffType.None:
                            r.Text = currentMatchText.Substring(df.OrigIndex, 1);
                            r.Background = Colors.CorrectBackground;
                            break;
                        case DiffType.Delete:

                            r.Text = currentMatchText.Substring(df.OrigIndex - 1, 1);
                            counter--;
                         //   r.Background = Colors.CorrectBackground;
                            break;
                        case DiffType.Add:

                            r.Text = TbxInput.Text.Substring(df.RevIndex + counter, 1);
                            counter++;
                            r.Background = Colors.IncorrectBackground;
                            break;

                    }

                    tb.Inlines.Add(r);
                }

                TbDispay.Children.Add(tb);

            }







        }

        Timer Tdisplay = null;

        private void DispatchUpdateDisplay(object obj)
        {
            UpdateLevel ul = (UpdateLevel)obj;
            Dispatcher.Invoke(new Action(() => { UpdateDisplay(ul); }));

            if (Tdisplay != null)
            {
                Tdisplay.Dispose();
                Tdisplay = null;
            }

        }

        private void DelayUpdateDisplay(int delay, UpdateLevel updateLevel)
        {
            if (delay == 0)
            {
                if (Tdisplay != null)
                {
                    Tdisplay.Dispose();
                    Tdisplay = null;
                }




            }
            else if (delay > 0)
            {
                if (Tdisplay == null)
                {
                    Tdisplay = new Timer(DispatchUpdateDisplay, updateLevel, delay, Timeout.Infinite);
                }
                else
                {
                    Tdisplay.Dispose();
                    Tdisplay = new Timer(DispatchUpdateDisplay, updateLevel, delay, Timeout.Infinite);


                }
            }
        }

        private void RetypeThisGroup()
        {



            UpdateTypingStat();

            if (StateManager.txtSource == TxtSource.changeSheng ||  StateManager.txtSource == TxtSource.jbs || Config.GetBool("禁止F3重打"))
                return;




 
            LoadText(TextInfo.MatchText, RetypeType.retype, TxtSource.unchange);
   //         TbkStatusTop.Text = "重打";
            return;
    





        }



        private void InternalHotkeyF3(object sender, ExecutedRoutedEventArgs e)
        {
            HotkeyF3();
        }


        private void HotkeyF3()
        {
            if (StateManager.txtSource == TxtSource.trainer && winTrainer != null)
                winTrainer.F3();
            else
                RetypeThisGroup();
        }

        public MainWindow()
        {
            this.WindowStartupLocation = WindowStartupLocation.Manual;
            InitCfg();

            double left = Config.GetDouble("窗口坐标X");
            double top = Config.GetDouble("窗口坐标Y");

            if (left > 0.001)
                this.Left = left;
            if (top > 0.001)
                this.Top = top;

            this.Height = Config.GetDouble("窗口高度");
            this.Width = Config.GetDouble("窗口宽度");

            InitializeComponent();

        }




        private void BtnF3_Click(object sender, RoutedEventArgs e)
        {
            HotkeyF3();
        }














        const double CollapseHeight = 141;
        private void expanded(object sender, RoutedEventArgs e)
        {
            if (StateManager.ConfigLoaded)
            {
                content.Height = new GridLength(content.ActualHeight);
                Application.Current.MainWindow.Height += CollapseHeight;

                Config.Set("成绩面板展开", true);
                Config.Set("窗口宽度", this.Width, 0);
                Config.Set("窗口高度", this.Height, 0);



                content.Height = new GridLength(1.0, GridUnitType.Star);
            }



        }

        private void expd_Collapsed(object sender, RoutedEventArgs e)
        {

            if (StateManager.ConfigLoaded)
            {
                content.Height = new GridLength(content.ActualHeight);
                Application.Current.MainWindow.Height -= CollapseHeight;
                Config.Set("成绩面板展开", false);
                Config.Set("窗口宽度", this.Width, 0);
                Config.Set("窗口高度", this.Height, 0);



                content.Height = new GridLength(1.0, GridUnitType.Star);
            }


        }

        private void ShowTodayStats()
        {

        }


        private void InitCfg()
        {

            Config.SetDefault
            (
                "窗口高度", "720",
                "窗口宽度", "1000",
                "字体大小", "30",
              "窗体背景色", "505050",
            "窗体字体色", "D3D3D3",
            "跟打区背景色", "C5B28F",
            "跟打区字体色", "000000",
            "打对色", "95b0e3",
            "打错色", "FF6347",
             "显示进度条", "是",
            "长流用户名", "",
            "长流密码", "",
             "极速用户名", "",
            "极速密码", "",
            "禁止F3重打", "否",
            "增强键准提示", "否",
            "增强速度提示", "否",
                "盲打模式", "否",
                "看打模式", "否",
                "字体", "#霞鹜文楷 GB 屏幕阅读版",
                "行距", "0.35",
                "允许滚动", "是",
                //          "回放功能", "否",
                "自动发送成绩", "是",

                "鼠标中键载文", "否",
                "错字重打", "是",
                "错字重复次数", "3",
                "QQ窗口切换模式(1-2)", "1",
                 //               "字集过滤与替换", "否",
                 "载文模式(1-4)", "1",
                "成绩面板展开", "是",
                "成绩签名", "Pain打器",
                "成绩单屏蔽模块(逗号分隔多个)", "无",
                "开启程序调试Log", "否",
                "获取更新", "QQ群775237860"
            );


            Config.ReadConfig();
            //   TxtWrong.ReadConfig();
            //   TxtBack.ReadConfig();
            //   TxtCorrection.ReadConfig();


        }

        private void InitFontFamilySelector()
        {

            DirectoryInfo dr = new DirectoryInfo("字体");
            if (!dr.Exists)
                dr.Create();

            CultureInfo cn = CultureInfo.GetCultureInfo("zh-CN");
            CultureInfo en = CultureInfo.GetCultureInfo("en-US");

            foreach (var f in dr.GetFiles("*.ttf"))
            {
                var fullname = f.FullName;

                GlyphTypeface gf = new GlyphTypeface(new Uri(fullname));
                var s = gf.FamilyNames;
                //       var b =  gf.FontUri.ToString();

                string fontname = "";


                if (s.ContainsKey(cn))
                    fontname = s[cn];
                else if (s.ContainsKey(en))
                    fontname = s[en];


                if (fontname != "")
                {




                    ComboBoxItem cbi = new ComboBoxItem();



                    string currentPath = System.AppDomain.CurrentDomain.BaseDirectory;
                    Uri uri = new Uri(currentPath + "字体\\");
                    FontFamily fm = new FontFamily(uri, "./#" + fontname);
                    cbi.FontFamily = fm;
                    cbi.FontSize = Config.GetDouble("字体大小");
                    cbi.Content = "#" + fontname;

                    CbFonts.Items.Add(cbi);
                }

            }





            foreach (FontFamily fontfamily in Fonts.SystemFontFamilies)
            {
                LanguageSpecificStringDictionary lsd = fontfamily.FamilyNames;
                if (lsd.ContainsKey(XmlLanguage.GetLanguage("zh-cn")))
                {
                    string fontname = null;
                    if (lsd.TryGetValue(XmlLanguage.GetLanguage("zh-cn"), out fontname))
                    {
                        ComboBoxItem cbi = new ComboBoxItem
                        {
                            FontFamily = new FontFamily(fontname),
                            FontSize = Config.GetDouble("字体大小"),
                            Content = fontname
                        };

                        CbFonts.Items.Add(cbi);
                    }
                }
                else
                {
                    string fontname = null;
                    if (lsd.TryGetValue(XmlLanguage.GetLanguage("en-us"), out fontname))
                    {
                        ComboBoxItem cbi = new ComboBoxItem();
                        cbi.FontFamily = new FontFamily(fontname);
                        cbi.FontSize = Config.GetDouble("字体大小");
                        cbi.Content = fontname;
                        CbFonts.Items.Add(cbi);

                        //CbFonts.Items.Add(fontname);
                    }
                }
            }
        }


        public FontFamily GetCurrentFontFamily()
        {
            string fontName = Config.GetString("字体");

            if (fontName == null || fontName.Length == 0)
                return null;

            if (fontName.Substring(0, 1) == "#")
            {

                string currentPath = System.AppDomain.CurrentDomain.BaseDirectory;
                Uri uri = new Uri(currentPath + "字体\\");
                FontFamily fm = new FontFamily(uri, "./" + fontName);


                return fm;
            }
            else
            {
                return new FontFamily(fontName);
            }
        }
        private void InitDisplay()
        {


            this.Background = Colors.FromString(Config.GetString("窗体背景色"));
            this.Foreground = Colors.FromString(Config.GetString("窗体字体色"));
            BdDisplay.Background = Colors.FromString(Config.GetString("跟打区背景色"));
            Colors.DisplayForeground = Colors.FromString(Config.GetString("跟打区字体色"));
            Colors.CorrectBackground = Colors.FromString(Config.GetString("打对色"));
            Colors.IncorrectBackground = Colors.FromString(Config.GetString("打错色"));
            PgbTypingProgress.Foreground = Colors.FromString(Config.GetString("打对色"));

            if (winTrainer != null)
            {
                WinTrainer.Current.DisplayGrid.Background = BdDisplay.Background;
                winTrainer.Background = this.Background;
            }

            if (Config.GetBool("显示进度条"))
                PgbTypingProgress.Visibility = Visibility.Visible;
            else
                PgbTypingProgress.Visibility = Visibility.Hidden;

            ReadBlindType();




            this.Height = Config.GetDouble("窗口高度");
            this.Width = Config.GetDouble("窗口宽度");
            expd.IsExpanded = Config.GetBool("成绩面板展开");




            TbxInput.FontFamily = GetCurrentFontFamily();// new FontFamily(Config.GetString("字体")); ;
            SldZoom.Value = Config.GetDouble("字体大小");

            InitFontFamilySelector();


            for (int i = 0; i < CbFonts.Items.Count; i++)
            {
                ComboBoxItem cbi = CbFonts.Items[i] as ComboBoxItem;
                if (Config.GetString("字体") == cbi.Content.ToString())
                {
                    CbFonts.SelectedIndex = i;
                }
            }

            UpdateButtonProgress();

            /*
                        if (Config.GetBool("新版QQ"))
                        {
                            BtnF5.Visibility = Visibility.Hidden;

                        }
                        else
                            BtnF5.Visibility = Visibility.Visible;
            */

            if (Config.GetBool("鼠标中键载文"))
                StartMouseHook();
            /*
                        if (Config.GetBool("回放功能"))
                            StartHook();
                        else
                            StopHook();

                        Tbk.IsEnabled = Config.GetBool("回放功能");
            */

            BtnF3.IsEnabled = !Config.GetBool("禁止F3重打");
            DebugLog.Enable = Config.GetBool("开启程序调试Log");

            IntStringDict.Load();

            StateManager.ConfigLoaded = true;
        }

        private void ReadBlindType ()
        {
            if (Config.GetBool("看打模式"))
                SldBlind.Value = 3;
            else if (Config.GetBool("盲打模式"))
                SldBlind.Value = 2;
            else
                SldBlind.Value = 1;


            SldBindLookUpdate();
            /*
            if (SldBlind.Value < 1.01 ) //盲打
            {
                Config.Set("盲打模式", true);
                Config.Set("看打模式", false);
            }
            else if (SldBlind.Value > 2.99) //看打
            {
                Config.Set("盲打模式", true);
                Config.Set("看打模式", true);
            }
            else
            {
                Config.Set("盲打模式", false);
                Config.Set("看打模式", false);
            }

            */

            //      ChkBlindType.IsChecked = Config.GetBool("盲打模式");

            //      ChkLookType.IsChecked = Config.GetBool("看打模式") && !Config.GetBool("盲打模式");
        }
        private void ReloadCfg()
        {

            StateManager.ConfigLoaded = false;

            this.Background = Colors.FromString(Config.GetString("窗体背景色"));
            this.Foreground = Colors.FromString(Config.GetString("窗体字体色"));
            BdDisplay.Background = Colors.FromString(Config.GetString("跟打区背景色"));
            Colors.DisplayForeground = Colors.FromString(Config.GetString("跟打区字体色"));
            Colors.CorrectBackground = Colors.FromString(Config.GetString("打对色"));
            Colors.IncorrectBackground = Colors.FromString(Config.GetString("打错色"));
            PgbTypingProgress.Foreground = Colors.FromString(Config.GetString("打对色"));
            if (winTrainer != null)
            {
                WinTrainer.Current.DisplayGrid.Background = BdDisplay.Background;
                winTrainer.Background = this.Background;
            }


            this.Height = Config.GetDouble("窗口高度");
            this.Width = Config.GetDouble("窗口宽度");
            expd.IsExpanded = Config.GetBool("成绩面板展开");

            TbxInput.FontFamily = GetCurrentFontFamily();// new FontFamily(Config.GetString("字体")); ;
            SldZoom.Value = Config.GetDouble("字体大小");

            if (Config.GetBool("显示进度条"))
                PgbTypingProgress.Visibility = Visibility.Visible;
            else
                PgbTypingProgress.Visibility = Visibility.Hidden;


            ReadBlindType();

            InitFontFamilySelector();


            for (int i = 0; i < CbFonts.Items.Count; i++)
            {
                ComboBoxItem cbi = CbFonts.Items[i] as ComboBoxItem;
                if (Config.GetString("字体") == cbi.Content.ToString())
                {
                    CbFonts.SelectedIndex = i;
                }
            }


            UpdateButtonProgress();
            /*
            if (Config.GetBool("新版QQ"))
            {
                BtnF5.Visibility = Visibility.Hidden;

            }
            else
                BtnF5.Visibility = Visibility.Visible;
            */
            if (Config.GetBool("鼠标中键载文"))
                StartMouseHook();
            /*
                        if (Config.GetBool("回放功能"))
                            StartHook();
                        else
                            StopHook();

                        Tbk.IsEnabled = Config.GetBool("回放功能");
            */
            BtnF3.IsEnabled = !Config.GetBool("禁止F3重打");
            DebugLog.Enable = Config.GetBool("开启程序调试Log");
            StateManager.ConfigLoaded = true;
            IntStringDict.Load();

            UpdateDisplay(UpdateLevel.PageArrange);






        }








        private void win_size_change(object sender, SizeChangedEventArgs e)
        {


            if (StateManager.ConfigLoaded)
            {
                Config.Set("窗口宽度", this.Width, 0);
                Config.Set("窗口高度", this.Height, 0);
                // UpdateDisplay(UpdateLevel.PageArrange);
                DelayUpdateDisplay(500, UpdateLevel.PageArrange);
            }
        }


        private void NextArticle()
        {
            ArticleManager.NextSection();

        }


        void DelayStop(object obj)
        {

            Dispatcher.Invoke(StopHelper);

        }


        private string TxtResult = "";

        public void UpdateTypingStat(string newReport = "")
        {

            CounterLog.Add("字数", CounterLog.Buffer[0]);
            CounterLog.Buffer[0] = 0;
            CounterLog.Add("击键数", CounterLog.Buffer[1]);
            CounterLog.Buffer[1] = 0;
            CounterLog.Write();

            StringBuilder sb = new StringBuilder();
            sb.Append("今日字数：");
            sb.Append(CounterLog.GetCurrent("字数") + CounterLog.Buffer[0]);
            sb.Append("   ");
            sb.Append("总字数：");
            sb.Append(CounterLog.GetSum("字数") + CounterLog.Buffer[0]);
           


            sb.AppendLine();

            if (newReport != "")
                TxtResult = newReport + "\n" + TxtResult;
            sb.Append(TxtResult);



            TbxResults.Text = sb.ToString();
        }


        public bool IsLookingType
        {
            get
            {
                return Config.GetBool("看打模式") && StateManager.retypeType != RetypeType.wrongRetype;
            }
        }


        public bool IsBlindType
        {
            get
            {
                return Config.GetBool("盲打模式") && StateManager.retypeType != RetypeType.wrongRetype;
            }
        }

        void StopHelper()
        {
            TbxInput.IsReadOnly = true;
            StateManager.typingState = TypingState.end;
            sw.Stop();
         

            Score.HitRate = Score.GetHit() / Score.Time.TotalSeconds;
            Score.KPW = Score.GetHit() / (double)Score.TotalWordCount;
            Score.Speed = (double)Score.TotalWordCount / Score.Time.TotalMinutes;




            Score.InputWordCount = new StringInfo(TbxInput.Text).LengthInTextElements;

            //计算错字

            if (IsLookingType)
            {
                string currentMatchText = string.Concat(TextInfo.Words);
                Score.Less = 0;
                Score.More = 0;

                string t1 = currentMatchText.Replace('”', '\"').Replace('“', '\"').Replace('‘', '\'').Replace('’', '\'');

                string t2 = TbxInput.Text.Replace('”', '\"').Replace('“', '\"').Replace('‘', '\'').Replace('’', '\'');
                List<DiffRes> diffs = DiffTool.Diff(t1, t2);


                int counter = 0;
                foreach (var df in diffs)
                {


                    switch (df.Type)
                    {
                        case DiffType.None:


                            break;
                        case DiffType.Delete:
                            Score.Less++;
                            string w = currentMatchText.Substring(df.OrigIndex - 1, 1);


                            LogWrong(df.OrigIndex - 1, w);


                            counter--;

                            break;
                        case DiffType.Add:


                            counter++;
                            Score.More++;
                            break;

                    }


                }



            }
            else
            {
                Score.Wrong = 0;


                for (int i = 0; i < TextInfo.wordStates.Count; i++)
                {
                    if (TextInfo.wordStates[i] == WordStates.WRONG)
                    {
                        Score.Wrong++;
                        string w = TextInfo.Words[i];
                        LogWrong(i, w);
                    }
                }
            }

            TbkStatusTop.Text = Score.Progress();
            if (StateManager.retypeType != RetypeType.wrongRetype)
                UpdateTypingStat(Score.Report());// + " " + ;
            else
                UpdateTypingStat();
            string result = Score.Report();// + " " + Config.GetString("成绩签名");






            //自动发送成绩&&加载下一段





            if (StateManager.txtSource == TxtSource.changeSheng) //长生
            {
                cs.SendScore((int)Score.GetBacks(), (int)Score.GetCorrection(), Score.GetAccuracy() * 100.0, Score.KPW, Score.LRRatio * 100.0, Score.HitRate, Score.Wrong, Score.TotalWordCount, Score.GetChoose(), Score.GetValidSpeed(), Score.Time.TotalSeconds, Score.GetCiRatio() * 100.0);

            }

            if (StateManager.txtSource == TxtSource.jbs) //锦标赛
            {
           //     cs.SendScore((int)Score.GetBacks(), (int)Score.GetCorrection(), Score.GetAccuracy() * 100.0, Score.KPW, Score.LRRatio * 100.0, Score.HitRate, Score.Wrong, Score.TotalWordCount, Score.GetChoose(), Score.GetValidSpeed(), Score.Time.TotalSeconds, Score.GetCiRatio() * 100.0);
             //   jbs.SendScore(Score.GetValidSpeed().ToString("F2"), Score.HitRate.ToString("F2"), Score.KPW.ToString("F2"), Score.Time.TotalSeconds.ToString("F2"), Score.GetCorrection().ToString("F0"), "0", Score.GetHit().ToString("F0"), Score.GetAccuracy().ToString("F2"), Score.GetCiRatio().ToString("F2"), Score.Wrong.ToString("F0"));
                jbs.SendScore(Score.GetValidSpeed(), Score.HitRate, Score.KPW, Score.Time, (int)Score.GetCorrection(), 0, (int)Score.GetHit(), Score.GetAccuracy(), Score.GetCiRatio(), Score.Wrong, Config.GetString("成绩签名"));


            }


            if (StateManager.txtSource == TxtSource.book) //书籍
            {

                if (!Config.GetBool("错字重打")) //没有错字，或没有错字重打
                {
                    NextAndSendArticle(result);
                }
                else //(Config.GetBool("错字重打"))
                {
                    if (StateManager.retypeType == RetypeType.wrongRetype ) //错字重打
                    {
                        if ( TextInfo.WrongRec.Count == 0) //错字重打后无错字
                        {
                            NextAndSendArticle();
                        }
                       else
                        { }
                    }
                    else// (StateManager.retypeType != RetypeType.wrongRetype)//非错字重打，正文或普通重打
                    {
                        if (TextInfo.WrongRec.Count == 0) //一次打对无错字
                        {
                            NextAndSendArticle(result);
                        }
                        else //有错字，只发成绩
                        {
                            QQHelper.SendQQMessage(QQGroupName, result, 250, this);
                        }
                    }


                   
                }
            }
            else if (StateManager.txtSource == TxtSource.trainer) //练单器
            {

                if (winTrainer != null)
                    winTrainer.GetNextRound(Score.GetAccuracy(), Score.HitRate, Score.Wrong, result);





            }
            else
            {
                if (StateManager.retypeType != RetypeType.wrongRetype)
                {

                    QQHelper.SendQQMessage(QQGroupName, result, 250, this);

                }
                else
                {
                   
                }
            }










            if (Config.GetBool("错字重打") && TextInfo.WrongRec.Count > 0 && StateManager.txtSource != TxtSource.trainer)
            {
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < Config.GetInt("错字重复次数"); i++)
                {
                    foreach (var s in TextInfo.WrongRec.Values)
                        sb.Append(s);

                }

 
                LoadText(sb.ToString(), RetypeType.wrongRetype, TxtSource.unchange, true,true);
            }


     

        }



        Timer tm1;

        void ProcInput()
        {


            if (TextInfo.Words.Count == 0)
                return;

            TextInfo.Check(TbxInput.Text);

            CalScore();



            if (IsInputEnd())
                Stop();


            UpdateDisplay(UpdateLevel.Progress);



            void Stop()
            {
                Trace.WriteLine("stop");
              
                StateManager.LastType = true;
                Score.TotalWordCount = TextInfo.Words.Count;
                Score.Time = sw.Elapsed;
                timerProgress.Dispose();
                tm1 = new Timer(DelayStop, null, 150, Timeout.Infinite);





            }




            bool IsInputEnd()
            {



                if (!IsLookingType || TextInfo.Words.Count <= 3)
                {
                    StringInfo sb = new StringInfo(TbxInput.Text);

                    int lenA = TextInfo.Words.Count;
                    int lenB = sb.LengthInTextElements;

                    return lenA <= lenB && lenA >= 1 && TextInfo.Words.Last() == sb.SubstringByTextElements(lenA - 1, 1);

                }
                else
                {

                    StringInfo sb = new StringInfo(TbxInput.Text);

                    int lenA = TextInfo.Words.Count;
                    int lenB = sb.LengthInTextElements;

                    int LengthError = lenA / 10 + 1;

                    string la = "";

                    for (int i = lenA - 3; i <= lenA - 1; i++)
                        la += TextInfo.Words[i];

                    bool LastMatch = lenB > 3 && sb.SubstringByTextElements(lenB - 3, 3).Replace("”", "“") == la.Replace("”", "“");

                    bool LengthMatch = Math.Abs(lenB - lenA) <= LengthError;

                    return LastMatch && LengthMatch;
                }




            }


            void CalScore()
            {


                Score.TotalWordCount = TextInfo.Words.Count;
                Score.InputWordCount = new StringInfo(TbxInput.Text).LengthInTextElements;

                Score.Wrong = 0;

                if (!IsLookingType)
                {



                    for (int i = 0; i < TextInfo.wordStates.Count; i++)
                    {
                        if (TextInfo.wordStates[i] == WordStates.WRONG)
                        {
                            Score.Wrong++;

                        }
                    }

                }






                PgbTypingProgress.Value = (Score.InputWordCount * 100.0 / Score.TotalWordCount);
       
            }



        }

        CUIAutomation root = new CUIAutomation();

        const int KEY_DELAY = 25;

        public void NextAndSendArticle( string lastResult)
        {
            NextArticle();


            if (winArticle != null)
            {
                winArticle.UpdateDisplay();
            }
            string content2 = ArticleManager.GetFormattedCurrentSection();
            LoadText(content2, RetypeType.first, TxtSource.book, false, true);

            if (QQGroupName != "")
                QQHelper.SendQQMessageD(QQGroupName, lastResult, content2, 150, this);
            else
            {
                Win32.Win32SetText(lastResult);
                FocusInput();
            }


        //    LoadText(content2, RetypeType.first, TxtSource.book);


        }

        public void NextAndSendArticle()
        {
            NextArticle();


            if (winArticle != null)
            {
                winArticle.UpdateDisplay();
            }
            string content2 = ArticleManager.GetFormattedCurrentSection();
            LoadText(content2, RetypeType.first, TxtSource.book, false, true);

            if (QQGroupName != "")
                QQHelper.SendQQMessage(QQGroupName, content2, 150, this);
            else
                FocusInput();
            //    LoadText(content2, RetypeType.first, TxtSource.book);


        }


        public void SendArticle()
        {
            string content = ArticleManager.GetFormattedCurrentSection();

            if (winArticle != null)
            {
                winArticle.UpdateDisplay();
            }

            if (content == null || content.Length == 0)
                return;

            LoadText(content, RetypeType.first, TxtSource.book, false);

            if (QQGroupName !="")       
                QQHelper.SendQQMessage(QQGroupName, content, 250, this);
            else
            {
                Win32SetText(content);
                FocusInput();
            }






        }




        private void InputBox_TextInput(object sender, TextCompositionEventArgs e)
        {

            if (e.Text == "")
            {

                LogBack();


                return;
            }

            if (e.Text != "" && e.Text != "\r")
            {

                //分析打词率
                Score.AddInputStack(e.Text);

                //记录选重提交时间、字符
                StringInfo si = new StringInfo(e.Text);
                string last = si.SubstringByTextElements(si.LengthInTextElements - 1, 1);


                Score.CommitTime.Add(sw.ElapsedMilliseconds);
                Score.CommitStr.Add(last);


                if (si.LengthInTextElements >= 2)
                {
                    if (last == "…" || last == "—")
                    {
                        if (si.LengthInTextElements >= 3 && si.SubstringByTextElements(si.LengthInTextElements - 2, 1) == last)
                        {
                            Score.BiaoDingCommitTime.Add(sw.ElapsedMilliseconds);
                            Score.BiaoDingCommitStr.Add(last);
                        }
                    }
                    else
                    {
                        Score.BiaoDingCommitTime.Add(sw.ElapsedMilliseconds);
                        Score.BiaoDingCommitStr.Add(last);
                    }


                }



                StateManager.TextInput = true;

            }


        }

        private void DisplayProgress()
        {


            Score.Time = sw.Elapsed;
            Score.HitRate = Score.GetHit() / sw.Elapsed.TotalSeconds;

            Score.KPW = Score.GetHit() / (double)Score.InputWordCount;
            Score.Speed = (double)Score.InputWordCount / Score.Time.TotalMinutes;



            TbkStatusTop.Text = Score.Progress();

        }

        Timer timerProgress;
        private void timerUpdateProgress(object obj)
        {
            Dispatcher.Invoke(DisplayProgress);
        }

        private void InputBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {

            if (IsLookingType && StateManager.LastType && cacheLoadInfo != null && TbxInput.IsReadOnly && !detectKeyup)
            {
    

                detectKeyup = true;

                return;
            }


            if (TbxInput.IsReadOnly)
                return;
            //过滤热键

            if (e.Key == Key.F3 || e.Key == Key.F4 || e.Key == Key.F5)
                return;
            if (StateManager.typingState == TypingState.ready)
            {
                if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
                    return;


            }


            //回车暂停
            if (e.Key == Key.Enter && StateManager.txtSource != TxtSource.changeSheng && StateManager.txtSource != TxtSource.jbs)
            {
                if (StateManager.typingState == TypingState.typing)
                {
                    StateManager.typingState = TypingState.pause;
                    TbkStatusTop.Text = "暂停\t" + TbkStatusTop.Text;
                    sw.Stop();
                    //              Recorder.Stop();
                    if (timerProgress != null)
                        timerProgress.Dispose();
                }


                return;
            }

            //统计键法
            if (e.Key == Key.ImeProcessed)
                Score.SetJianFa(e.ImeProcessedKey);
            else
                Score.SetJianFa(e.Key);

            //打字击键总数记录计数
            //       if ( Recorder.State != Recorder.RecorderState.Playing)
            CounterLog.Buffer[1]++;








            //启动
            if (StateManager.typingState == TypingState.pause || StateManager.typingState == TypingState.ready)
            {
                var oldstate = StateManager.typingState;
                if (StateManager.typingState == TypingState.ready && StateManager.retypeType != RetypeType.wrongRetype)// && Recorder.State != Recorder.RecorderState.Playing)
                    RetypeCounter.Add(TextInfo.TextMD5, 1);

                sw.Start();

                StateManager.typingState = TypingState.typing;
                timerProgress = new Timer(timerUpdateProgress, null, 0, 250);

            }


            //退格
            Score.Hit++;




            switch (e.Key)
            {
                case Key.Space:
                    StateManager.TextInput = true;
                    Score.AddInputStack(" ");
                    break;
                case Key.Back:
                    LogCorrection();


                    Score.Correction++;
                    if (TbxInput.Text.Length > 0 && Score.ZiciStack.Count > 0)
                    {
                        Score.ZiciStack.Pop();
                        StateManager.TextInput = true;
                    }
                    break;

                // bime hit
                case Key.F14:
                    Score.BimeHit++;
                    break;
                case Key.F15:
                    Score.BimeCorrection++;
                    LogCorrection();
                    break;
                case Key.F16:
                    Score.BimeBacks++;
                    LogBack();
                    break;


                case Key.ImeProcessed:

                    {//统计选重
                        int vkey = KeyInterop.VirtualKeyFromKey(e.ImeProcessedKey);
                        if (IntStringDict.Selection.ContainsKey(vkey))
                        {
                            Score.ImeKeyTime.Add(sw.ElapsedMilliseconds);
                            Score.ImeKeyValue.Add(vkey);
                        }

                        if (IntStringDict.BiaoDing.ContainsKey(vkey))
                        {
                            Score.BiaoDingImeKeyTime.Add(sw.ElapsedMilliseconds);
                            Score.BiaoDingImeKeyValue.Add(vkey);
                        }

                    }

                    switch (e.ImeProcessedKey)
                    {
                        case Key.Back:

                            LogBack();
                            Score.Backs++;
                            break;
                        default:
                            if (GetKeyState(VK_BACK) < 0)
                            {
                                LogBack();
                                Score.Backs++;
                            }
                            break;
                    }
                    break;
                default:
                    break;
            }


        }


        private void HotKeyCtrlE()
        {
            LoadTextFromClipBoard();

        }
        private void HotKeyF4()
        {

            GetQQText();
        }


        public static bool Delay1(int delayTime) //延时函数
        {
            /*
            DateTime now = DateTime.Now;
            int s;
            do
            {
                TimeSpan spand = DateTime.Now - now;
                s = (int)spand.TotalMilliseconds;
                //Application.DoEvents();
            }
            while (s < delayTime);
            */
            System.Threading.Thread.Sleep(delayTime);

            return true;

        }
        [DllImport("User32")]
        internal static extern bool OpenClipboard(IntPtr hWndNewOwner);

        [DllImport("User32")]
        internal static extern bool CloseClipboard();

        [DllImport("User32")]
        internal static extern bool EmptyClipboard();

        [DllImport("User32")]
        internal static extern bool IsClipboardFormatAvailable(int format);

        [DllImport("User32")]
        internal static extern IntPtr GetClipboardData(int uFormat);

        [DllImport("User32", CharSet = CharSet.Unicode)]
        internal static extern IntPtr SetClipboardData(int uFormat, IntPtr hMem);

        internal static void Win32SetText(string text)
        {

            try
            {
                if (!OpenClipboard(IntPtr.Zero)) { Win32SetText(text); return; }
                EmptyClipboard();
                SetClipboardData(13, Marshal.StringToHGlobalUni(text));
                CloseClipboard();
            }
            catch (Exception)
            {

    
            }

        }

        internal static string Win32GetText(int format)
        {
            string value = string.Empty;
            //         OpenClipboard(IntPtr.Zero);
            if (OpenClipboard(IntPtr.Zero))
            {
                if (IsClipboardFormatAvailable(format))
                {
                    IntPtr ptr = GetClipboardData(format);
                    if (ptr != IntPtr.Zero)
                    {
                        value = Marshal.PtrToStringUni(ptr);
                    }
                    else
                    {
                        value = string.Empty;
                    }
                }
                CloseClipboard();
            }
            else
            {
                value = string.Empty;
            }
            return value;
        }
        private void GetQQText()
        {
         //   Win32.DelayCount.Clear();
            string groupName = QQGroupName;
            if (groupName == "")
            {
                DebugLog.AppendLine("无群");
                return;
            }

            DebugLog.AppendLine("GetQQText");


            if (QQHelper.IsNewQQ)
            {
                string MainTitle = "QQ";
                string allTxt = "";
                var q = root.GetRootElement().FindFirst(TreeScope.TreeScope_Children, root.CreatePropertyCondition(UIA_PropertyIds.UIA_NamePropertyId, MainTitle));
                if (q == null || q.CurrentClassName == "TXGuiFoundation")
                    return;

                if (null == q.FindFirst(TreeScope.TreeScope_Children, root.CreatePropertyCondition(UIA_PropertyIds.UIA_ControlTypePropertyId, UIA_ControlTypeIds.UIA_DocumentControlTypeId)))
                {
                    var wp = q.GetCurrentPattern(UIA_PatternIds.UIA_WindowPatternId) as IUIAutomationWindowPattern;
                    wp.SetWindowVisualState(WindowVisualState.WindowVisualState_Normal);
                    q.SetFocus();

                    Win32.Delay(50);
                }
       //         var msglist = q.FindFirst(TreeScope.TreeScope_Descendants, root.CreatePropertyCondition(UIA_PropertyIds.UIA_AutomationIdPropertyId, "ml-root"));
                var msglist = q.FindFirst(TreeScope.TreeScope_Descendants,  root.CreateOrCondition( root.CreatePropertyCondition(UIA_PropertyIds.UIA_NamePropertyId, "消息列表"), root.CreatePropertyCondition(UIA_PropertyIds.UIA_LocalizedControlTypePropertyId, "主要")));   //找不到消息区，aeMessage值为空
                var grouplist = q.FindFirst(TreeScope.TreeScope_Descendants, root.CreatePropertyCondition(UIA_PropertyIds.UIA_NamePropertyId, "会话列表"));   //找不到消息区，aeMessage值为空
                        /*                                                                                                                                  //        var grouplist = qq.FindFirst(TreeScope.TreeScope_Descendants, root.CreatePropertyCondition(UIA_PropertyIds.UIA_NamePropertyId, "会话列表"));   //找不到消息区，aeMessage值为空
                if (msglist == null)
                {
                    msglist = q.FindFirst(TreeScope.TreeScope_Descendants,  root.CreatePropertyCondition(UIA_PropertyIds.UIA_LocalizedControlTypePropertyId, "主要"));
                }
                */
                if (msglist == null || grouplist == null)
                {
                    return;
                }

                var edits = q.FindFirst(TreeScope.TreeScope_Descendants, root.CreateAndCondition(root.CreatePropertyCondition(UIA_PropertyIds.UIA_ControlTypePropertyId, UIA_ControlTypeIds.UIA_EditControlTypeId), root.CreatePropertyCondition(UIA_PropertyIds.UIA_NamePropertyId, groupName)));


                //点击群名
                if (edits == null)
                {
                    var groups = grouplist.FindAll(TreeScope.TreeScope_Children, root.CreateOrCondition(root.CreatePropertyCondition(UIA_PropertyIds.UIA_ControlTypePropertyId, UIA_ControlTypeIds.UIA_WindowControlTypeId), root.CreatePropertyCondition(UIA_PropertyIds.UIA_ControlTypePropertyId, UIA_ControlTypeIds.UIA_GroupControlTypeId)));
                    if (groups.Length > 0)
                    {
                        for (int i = 0; i < groups.Length; i++)
                        {
                            var group = groups.GetElement(i);
                            string s = group.CurrentName;

                            if ((s + "").IndexOf(groupName + " ") != 0)
                                continue;


                            var sp = group.GetCurrentPattern(UIA_PatternIds.UIA_InvokePatternId) as IUIAutomationInvokePattern;
                            if (sp != null)
                            {
                                sp.Invoke();

                                Win32.Delay(801);
                                edits = q.FindFirst(TreeScope.TreeScope_Descendants, root.CreateAndCondition(root.CreatePropertyCondition(UIA_PropertyIds.UIA_ControlTypePropertyId, UIA_ControlTypeIds.UIA_EditControlTypeId), root.CreatePropertyCondition(UIA_PropertyIds.UIA_NamePropertyId, groupName)));

                                break;
                            }

                        }
                    }
                }

                /*
                //遍历群
                int counter = 0;
                while (edits == null)
                {
                    counter++;
                    if (counter > 100)
                        break;
                    CtrlDown();
                    edits = q.FindFirst(TreeScope.TreeScope_Descendants, root.CreateAndCondition(root.CreatePropertyCondition(UIA_PropertyIds.UIA_ControlTypePropertyId, UIA_ControlTypeIds.UIA_EditControlTypeId), root.CreatePropertyCondition(UIA_PropertyIds.UIA_NamePropertyId, title)));

                }
                */

                if (edits != null)
                {
                    var msgs = msglist.FindAll(TreeScope.TreeScope_Descendants, root.CreatePropertyCondition(UIA_PropertyIds.UIA_ControlTypePropertyId, UIA_ControlTypeIds.UIA_TextControlTypeId));


                    if (msgs != null && msgs.Length > 0)
                    {
                        for (int i = msgs.Length - 1; i >= 0; i--)
                        {
                            allTxt += msgs.GetElement(i).CurrentName + "\n";
                        }
                    }


 

                    LoadText(allTxt, RetypeType.first, TxtSource.qq);
                }



                return;
            }


            else
            {




                {

                    if (groupName != "所在群")
                    {

                        System.Drawing.Point mpos = new System.Drawing.Point();

                        IntPtr win = FindWindow(null, groupName);
                        if (win.ToString() != "0")
                        {

                            SwitchToThisWindow(FindWindow(null, groupName), true); //激活窗口




                            Win32.Delay(150);
                            if (Config.GetString("QQ窗口切换模式(1-2)") == "2") //获取方式
                            {
                                Win32.Tab();

                            }
                            else
                            {
                                DebugLog.AppendLine("采用鼠标点击方法");
                                GetCursorPos(ref mpos);
                                RECT rect = new RECT();
                                IntPtr get = FindWindow("TXGuiFoundation", groupName);
                                GetWindowRect(get, ref rect);
                                int width = rect.Right - rect.Left;
                                int height = rect.Bottom - rect.Top;
                                SetCursorPos(rect.Left + width / 2, rect.Top + height / 2);
                                Win32.Delay(50);
                                mouse_event(MouseEventFlag.LeftDown, 0, 0, 0, win);
                                mouse_event(MouseEventFlag.LeftUp, 0, 0, 0, win);
                                //Application.DoEvents();
                            }
                            string allTxt = "";
                            Win32.Delay(50);

                            Win32.CtrlA();
                            DebugLog.a("ctrl A");

                            Win32.Delay(100);
                            Win32.CtrlC();
                            DebugLog.a("ctrl C");



                            switch (Config.GetString("载文模式(1-4)"))
                            {
                                case "1":
                                        if (allTxt == "")
                                    {
                                        Win32.Delay(150);
                                        TbClip.Paste();
                                        DebugLog.a("Paste");

                                        DebugLog.a("Tbclip内容：***********************\n" + TbClip.Text + "\n****************************");


                                        if (Config.GetString("QQ窗口切换模式(1-2)") == "1")
                                        {
                                            SetCursorPos(mpos.X, mpos.Y); //放回鼠标
                                            DebugLog.a("放回鼠标");
                                        }
                                    }
                                    return;
                                case "2":
                                    if (allTxt == "")
                                    {
                                        for (int i = 0; i < 20; i++)
                                        {
                                            Win32.Delay(80); //必须给定的延迟
                                            allTxt = Win32GetText(13);
                                            if (allTxt != "")
                                                break;
                                        }
                                    }
                                    break;
                                case "3":
                                    if (allTxt == "")
                                    {
                                        for (int i = 0; i < 30; i++)
                                        {
                                            try
                                            {

                                                var clipDataObject = Clipboard.GetDataObject();
                                                if (clipDataObject != null)
                                                {
                                                    allTxt = clipDataObject.GetData(DataFormats.StringFormat).ToString();
                                                }
                                                break;
                                            }
                                            catch
                                            {
                                                //   System.Threading.Thread.Sleep(10);//这句加不加都没关系
                                            }
                                        }

                                    }
                                    break;

                                case "4":
                                    if (allTxt == "")
                                    {
                                        for (int i = 0; i < 30; i++)
                                        {
                                            allTxt = Clipboard.GetText();
                                            if (allTxt != "")
                                                break;
                                            Win32.Delay(10);
                                        }
                                    }
                                    break;
                                default:
                                    break;



                            }





                            if (Config.GetString("QQ窗口切换模式(1-2)") == "1")
                                SetCursorPos(mpos.X, mpos.Y); //放回鼠标

                            FocusInput();

                            //    string MatchText = ExtractMatchText(get_);

                            //  if (MatchText != null)
                            //    LoadText(MatchText);
                            LoadText(allTxt, RetypeType.first, TxtSource.qq);
                            return;// get_;
                        }
                        else
                        {
                            if (Config.GetString("QQ窗口切换模式(1-2)") == "1")
                                SetCursorPos(mpos.X, mpos.Y); //放回鼠标
                            return;// "";
                        }
                    }
                    else
                    {
                        return;// "";
                    }

                }


            }






        }

        public class CacheLoadInfo
        {
            public  string rawTxt;
           public  RetypeType retypeType;
           public TxtSource source;
            public bool switchBack;
            public bool isAuto;
            public CacheLoadInfo(string rawTxt, RetypeType retypeType, TxtSource source, bool switchBack = true, bool isAuto = false)
            {
                this .rawTxt = rawTxt;
                this .retypeType = retypeType;
                this .source = source;
                this .switchBack = switchBack;
                this .isAuto = isAuto;
            }

        }

        CacheLoadInfo cacheLoadInfo = null;


        public void UpdateTopStatusText (string text)
        {
            TbkStatusTop.Text = text;
        }

        public void LoadText (CacheLoadInfo cli)
        {
            LoadText (cli.rawTxt, cli.retypeType, cli.source, cli.switchBack, cli.isAuto);
        }
        public void LoadText(string rawTxt, RetypeType retypeType, TxtSource source, bool switchBack = true, bool isAuto = false) //原文、来源、重打类型
        {

            if (Config.GetBool("禁止F3重打") && (retypeType == RetypeType.shuffle || retypeType == RetypeType.retype))
                return;

            var rt = ExtractRawTxt(rawTxt);

            if (rt.Item1 == "")
                return;

            if (isAuto && IsLookingType && StateManager.LastType) //看打模式的话，先缓存起来
            {
                cacheLoadInfo = new CacheLoadInfo(rawTxt, retypeType, source, switchBack, isAuto);
                return;
            }
        


            //设置states公共变量
            if (source != TxtSource.unchange)
                StateManager.txtSource = source;

            StateManager.retypeType = retypeType;

            //设置段号
            if (retypeType == RetypeType.wrongRetype)
            {

                Score.Paragraph = 0;
            }
            else if (retypeType == RetypeType.shuffle || retypeType == RetypeType.retype)
            {
                Score.Paragraph = TextInfo.Paragraph;
            }
            else //(retypeType == RetypeType.first)
            {
                Score.Paragraph = rt.Item2;
                TextInfo.Paragraph = rt.Item2;

            }


            //设置md5
            if (retypeType == RetypeType.first)
            {

                TextInfo.TextMD5 = TextInfo.CalMD5(rt.Item1);
            }

            //设置赛文
            if (retypeType == RetypeType.first)
            {

                TextInfo.MatchText = rt.Item1;
            }

            //设置textinfo

            TextInfo.Words.Clear();
            StringInfo si = new StringInfo(rt.Item1);

            for (int i = 0; i < si.LengthInTextElements; i++)
            {
                string s = si.SubstringByTextElements(i, 1);



                TextInfo.Words.Add(s);


            }

            TextInfo.wordStates.Clear();
            TextInfo.WrongRec.Clear();


            TextInfo.Words.ForEach(o => TextInfo.wordStates.Add(WordStates.NO_TYPE));


            StateManager.TextInput = false;





            //界面
            if (TextInfo.Words.Count > 0)
            {
                StateManager.typingState = TypingState.ready;
                StateManager.LastType = false;

                switch (retypeType)
                {
                    case RetypeType.shuffle:
                        TbkStatusTop.Text = "乱序";
                        break;
                    case RetypeType.retype:
                        TbkStatusTop.Text = "重打";
                        break;
                    case RetypeType.wrongRetype:
                        TbkStatusTop.Text = "错字重打";
                        break;
                    default:
                        TbkStatusTop.Text = "准备";
                        break;
                }
       //         if (retypeType != RetypeType.first)
        //            TbkStatusTop.Text = "准备";

                sw.Reset();
                if (timerProgress != null)
                    timerProgress.Dispose();
                Score.Reset();



                if (!(IsLookingType && StateManager.LastType))
                    TbxInput.IsReadOnly = false;
                TbxInput.Clear();
       //         TbxInput.Focus();
                PgbTypingProgress.Value = 0;
                UpdateDisplay(UpdateLevel.PageArrange);


                if (switchBack)
                {
                    FocusInput();
                }



            }



        }

        private Tuple<string, int> ExtractRawTxt(string rawTxt)
        {



            string[] lines = rawTxt.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            Regex r = new Regex("-----第[0-9]+段");

            int paragraph = 0;
            string head = "";
            string content = "";
            string tail = "";

            if (rawTxt == "")
                return new Tuple<string, int>(content, paragraph);


            //开始检测
            int index = -1;
            for (int i = 0; i< lines.Length; i++)
           // for (int i = lines.Length - 1; i > 0; i--)
            {
                if (r.Match(lines[i]).Success)
                {
                    index = i;
                    break;
                }
            }

            if (index >= 2) //赛文格式
            {

                head = lines[index - 2];
                content = lines[index - 1];
                tail = lines[index];

                var m = Regex.Match(tail, "第[0-9]+段");

                paragraph = Convert.ToInt32(m.Value.Substring(1, m.Value.Length - 2));

                if (head.Length >= 3 && head.Substring(0, 3) == "皇叔 ")
                    content = UnicodeBias(content);


            }
            else //非赛文格式
            {
                content = rawTxt.Replace("\n", "").Replace("\r", "").Replace("\t", "");
            }

            return new Tuple<string, int>(content, paragraph);


        }

        private void HotKeyMButton()
        {
            mouse_event(MouseEventFlag.LeftDown, 0, 0, 0, IntPtr.Zero);
            mouse_event(MouseEventFlag.LeftUp, 0, 0, 0, IntPtr.Zero);
            Win32.Delay(10);
//            Win32.CtrlA();
//            Win32.Delay(10);
            Win32.CtrlC();
            Win32.Delay(10);
            LoadTextFromClipBoard();

        }

        /*
        private void HotKeyF5()
        {


            ChangQu();

        }
        */
        /*
        private void ChangQu()
        {
            string QuName = QQHelper.GetQuName();

            if (QuName.Length > 0)
            {
                BtnF5.Content = "换群F5-" + QuName;
            }
            else
            {
                BtnF5.Content = "换群F5";
            }

            this.Activate();
            this.Topmost = true;  // important
            this.Topmost = false; // important
            this.Focus();         // important

            TbxInput.Focus();
        }
        */
        private void LoadTextFromClipBoard()
        {
            string cTxt = Clipboard.GetText();
            LoadText(cTxt, RetypeType.first, TxtSource.clipboard);
        }

        private string GetContentFromMatchText(string cTxt)
        {
            if (cTxt == "")
                return "";



            char[] sp = { '\n', '\r' };
            string[] SubTxt = cTxt.Split(sp, StringSplitOptions.RemoveEmptyEntries);




            string raw_txt = "";
            if (SubTxt.Length >= 3 && SubTxt[SubTxt.Length - 1].Contains("-----"))
            {

                for (int i = 1; i < SubTxt.Length - 1; i++)
                {
                    raw_txt += SubTxt[i];
                }

          //      var m = Regex.Match(SubTxt[SubTxt.Length - 1], "第[0-9]+段");


            }
            else if (SubTxt.Length > 0)
            {
                raw_txt = cTxt.Replace("\n", "").Replace("\r", "").Replace("\t", "");
            }
            else
            {
                return "";
            }

            if (cTxt.Length >= 3 && cTxt.Substring(0, 3) == "皇叔 ")
                raw_txt = UnicodeBias(raw_txt);

            return raw_txt;
        }

        private string UnicodeBias(string input)
        {
            StringBuilder sb = new StringBuilder();
            StringInfo si = new StringInfo(input);

            for (int i = 0; i < si.LengthInTextElements; i++)
            {
                string s = si.SubstringByTextElements(i, 1);


                string sout;
                int unicode;
                if (s.Length == 1)
                    unicode = (int)s[0];
                else
                    unicode = char.ConvertToUtf32(s[0], s[1]);


                unicode--;
                sout = char.ConvertFromUtf32(unicode);

                sb.Append(sout);
            }


            return sb.ToString();
        }





        private void BtnCtrlE_Click(object sender, RoutedEventArgs e)
        {

            HotKeyCtrlE();



        }

        private void auto()
        {

        }

        private void TbxInput_TextChanged(object sender, TextChangedEventArgs e)
        {





            if (StateManager.TextInput || Score.BimeHit > 0)
            {



                if (e.Changes.Count > 0)// && Recorder.State != Recorder.RecorderState.Playing)
                    CounterLog.Buffer[0] += e.Changes.First().AddedLength;



                StateManager.TextInput = false;
                ProcInput();
            }

        }

        //     AutomationElement aeInput;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitDisplay();


            IntStringDict.Load();


            //      AutomationElement dis = AutomationElement.FromHandle(new WindowInteropHelper(this).Handle);
            //     aeInput = dis.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.AutomationIdProperty, "input"));   //找到XXXX交流群的聊天窗口
            //     if (aeInput != null)
            //      {
            //          return;
            //      }
        }

        private void SldZoom_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (StateManager.ConfigLoaded)
            {

                Config.Set("字体大小", e.NewValue, 1);

                // UpdateDisplay(UpdateLevel.PageArrange);

                DelayUpdateDisplay(100, UpdateLevel.PageArrange);
            }
        }

        private void MenuQQ_Click(object sender, RoutedEventArgs e)
        {

            var mi = (MenuItem)sender;
            foreach (var mb in MenuQQGroup.Items)
            {
                var mbt = (MenuItem)mb;
                if (mbt.Header.ToString() != mi.Header.ToString())
                    mbt.IsChecked = false;
                else
                    mbt.IsChecked = true;
            }


            SelectQQGroup(mi.Header.ToString());



        }

        private void SelectQQGroup(string groupName)
        {

            if (groupName == "-潜水-")
            {
                BtnF5.Content = "选群▸";
            }
            else if (groupName != null && groupName.Length > 0)
            {
                BtnF5.Content = "当前-" + groupName;
            }
            else
            {
                BtnF5.Content = "选群▸";
            }

            FocusInput();
        }

        private void BtnF5_Click(object sender, RoutedEventArgs e)
        {
            var groupList = QQHelper.GetQunList();


            MenuQQGroup.Items.Clear();

            FocusInput();

            {
                MenuItem mi = new MenuItem();
                mi.Header = "-潜水-";
                mi.Click += MenuQQ_Click;
                mi.IsCheckable = true;


                if (BtnF5.Content.ToString().Length < 6)
                    mi.IsChecked = true;

                MenuQQGroup.Items.Add(mi);
            }
            foreach (string groupName in groupList)
            {
                MenuItem mi = new MenuItem();

                mi.Header = groupName;
                mi.Click += MenuQQ_Click;
                mi.IsCheckable = true;

                if (BtnF5.Content.ToString().Length > 3 && BtnF5.Content.ToString().Substring(3) == groupName)
                    mi.IsChecked = true;

                MenuQQGroup.Items.Add(mi);

            }

            MenuQQGroup.IsOpen = true;

            return;
        }

        private void BtnF4_Click(object sender, RoutedEventArgs e)
        {
            GetQQText();
        }




        /*
        public void CtrlTab()
        {

            System.Windows.Forms.SendKeys.SendWait("^{TAB}");


            //         Sleep(50);
        }

        public void CtrlA()
        {

            System.Windows.Forms.SendKeys.SendWait("^a");
        }

        public void CtrlV()
        {

            System.Windows.Forms.SendKeys.SendWait("^v");
        }


        public void CtrlC()
        {

            System.Windows.Forms.SendKeys.SendWait("^c");
        }


        public void AltS()
        {
            System.Windows.Forms.SendKeys.SendWait("%s");

        }
        public void Tab()
        {

            System.Windows.Forms.SendKeys.SendWait("{TAB}");


        }

        */
        private void CbFonts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StateManager.ConfigLoaded)
            {
                ComboBoxItem cbi = CbFonts.SelectedItem as ComboBoxItem;


                // string fontname = cbi.FontFamily.ToString();
                string fontname = cbi.Content.ToString();
                Config.Set("字体", fontname);
                UpdateDisplay(UpdateLevel.PageArrange);
            }

        }

        private void TbClip_TextChanged(object sender, TextChangedEventArgs e)
        {

            if (TbClip.Text != "")
            {

                LoadText(TbClip.Text, RetypeType.first, TxtSource.qq);
            }


            return;// get_;
        }


        private static Random rng = new Random();
        private void HotKeyCtrlL()
        {
            if (TextInfo.MatchText == "")
                return;
            List<string> ls = new List<string>();

            string sl = GetContentFromMatchText(TextInfo.MatchText);
            StringInfo si = new StringInfo(sl);

            for (int i = 0; i < si.LengthInTextElements; i++)
                ls.Add(si.SubstringByTextElements(i, 1));





            Sf(ls);

            string s = string.Join("", ls);

            LoadText(s, RetypeType.shuffle, TxtSource.unchange);

            void Sf<T>(IList<T> list)
            {
                int n = list.Count;
                while (n > 1)
                {
                    n--;
                    int k = rng.Next(n + 1);
                    T value = list[k];
                    list[k] = list[n];
                    list[n] = value;
                }
            }



            //    HotkeyF3();
   //         TbkStatusTop.Text = "乱序";
        }

        public static IEnumerable<T> Randomize<T>(IEnumerable<T> source)
        {
            Random rnd = new Random();
            return source.OrderBy((item) => rnd.Next());
        }



        private void InternalHotkeyF4(object sender, ExecutedRoutedEventArgs e)
        {
            HotKeyF4();
        }
        /*
        private void InternalHotkeyF5(object sender, ExecutedRoutedEventArgs e)
        {
            HotKeyF5();
        }
        */
        private void InternalHotkeyCtrlE(object sender, ExecutedRoutedEventArgs e)
        {
            HotKeyCtrlE();
        }

        private void InternalHotkeyCtrlL(object sender, ExecutedRoutedEventArgs e)
        {

            if (StateManager.txtSource == TxtSource.trainer && winTrainer != null)
                winTrainer.CtrlL();
            else
                HotKeyCtrlL();


        }



        /*
        private void Tbk_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {


            PlayRecord();
        }

        */


        /*
                private void Child2()
                {
                    Recorder.State = Recorder.RecorderState.Playing;
                    Stopwatch st = new Stopwatch();
                    INPUT[] input = new INPUT[1];
                    st.Start();

                    for (int i = 0; i < Recorder.RecItems.Count; i++)
                    {
                        var rec = Recorder.RecItems[i];
                        while (true)
                        {
                            if (st.ElapsedTicks >= rec.time + 10000000)
                            {

                                input[0].type = 1;//模拟键盘
                                                  //         input[0].ki.wVk = (short)KeyInterop.VirtualKeyFromKey(rec.key);
                                input[0].ki.wVk = (short)rec.key;
                                input[0].ki.dwFlags = rec.keystate;

                                SendInput((uint)1, input, Marshal.SizeOf((object)default(INPUT)));

                                break;
                            }
                        }
                    }

                    Recorder.State = Recorder.RecorderState.Stopped;


                }

        */


        /*
        private void PlayRecord()
        {
            if (Recorder.RecItems.Count == 0)
                return;
            StateManager.PlayRecord = true;
            Recorder.State = Recorder.RecorderState.Playing;
            HotkeyF3();
            TbxInput.Focus();
            Delay(500);

                 Thread th = new Thread(Child2);
                th.Start();


       


        }
        */
        
        bool detectKeyup = false;
        private void TbxInput_PreviewKeyUp(object sender, KeyEventArgs e)
        {

            if (IsLookingType && StateManager.LastType && cacheLoadInfo != null && TbxInput.IsReadOnly && detectKeyup)
            {
                cacheLoadInfo.isAuto = false;
                LoadText(cacheLoadInfo);
                cacheLoadInfo = null;

                detectKeyup = false;
                TbxInput.IsReadOnly = false;
                return;
            }


        }
        
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CounterLog.Add("字数", CounterLog.Buffer[0]);
            CounterLog.Buffer[0] = 0;
            CounterLog.Add("击键数", CounterLog.Buffer[1]);
            CounterLog.Buffer[1] = 0;
            CounterLog.Write();

            Config.Set("窗口坐标X", this.Left, 0);
            Config.Set("窗口坐标Y", this.Top, 0);
            Config.WriteConfig(0);

            //         StopHook();
            StopMouseHook();
            TextInfo.Exit = true;
            foreach (Window a in Application.Current.Windows)
            {
                //if (a.Title != "" && a.Title != this.Title)
                if (!( a is MainWindow))
                    a.Close();
            }

            //   Window[] childArray = Application.Current.Windows.

        }
        WinConfig winConfig;
        private void Tbk_PreviewMouseUp_1(object sender, MouseButtonEventArgs e)
        {
            foreach (Window item in Application.Current.Windows)
            {
                if (item is WinConfig)
                {
                    item.Focus();
                    item.Activate();
                    return;
                }

            }

            winConfig = new WinConfig();
            winConfig.ConfigSaved += new WinConfig.DelegateConfigSaved(ReloadCfg);
            winConfig.Show();
        }



        #region win32
        public delegate int HookProc(int nCode, Int32 wParam, IntPtr lParam);
        //    static int hKeyboardHook = 0; //声明键盘钩子处理的初始值
        public const int WH_KEYBOARD_LL = 13;   //线程键盘钩子监听鼠标消息设为2，全局键盘监听鼠标消息设为13
        public const int WH_KEYBOARD = 20;   //线程键盘钩子监听鼠标消息设为2，全局键盘监听鼠标消息设为13
        public const int WH_MOUSE_LL = 14;   //线程键盘钩子监听鼠标消息设为2，全局键盘监听鼠标消息设为13
                                             //      HookProc KeyboardHookProcedure; //声明KeyboardHookProcedure作为HookProc类型
        HookProc MouseHookProcedure; //声明KeyboardHookProcedure作为HookProc类型



        //键盘结构
        [StructLayout(LayoutKind.Sequential)]
        public class KeyboardHookStruct
        {
            public int vkCode;  //定一个虚拟键码。该代码必须有一个价值的范围1至254
            public int scanCode; // 指定的硬件扫描码的关键
            public int flags;  // 键标志
            public int time; // 指定的时间戳记的这个讯息
            public int dwExtraInfo; // 指定额外信息相关的信息
        }


        //使用此功能，安装了一个钩子
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);


        //调用此函数卸载钩子
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(int idHook);




        /*
        public void StartHook()
        {
            // 安装键盘钩子
            if (hKeyboardHook == 0)
            {
                KeyboardHookProcedure = new HookProc(KeyboardHookProc);

                hKeyboardHook = SetWindowsHookEx(WH_KEYBOARD_LL, KeyboardHookProcedure, GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName), 0);

                //如果SetWindowsHookEx失败
                if (hKeyboardHook == 0)
                {
                    StopHook();
                    throw new Exception("安装键盘钩子失败");
                }
            }
        }
        public void StopHook()
        {
            bool retKeyboard = true;


            if (hKeyboardHook != 0)
            {
                retKeyboard = UnhookWindowsHookEx(hKeyboardHook);
                hKeyboardHook = 0;
            }

            if (!(retKeyboard))
                throw new Exception("卸载钩子失败！");
        }
        */

        //使用WINDOWS API函数代替获取当前实例的函数,防止钩子失效
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string name);
        private const int WM_KEYDOWN = 0x100;//KEYDOWN
        private const int WM_KEYUP = 0x101;//KEYUP
        private const int WM_SYSKEYDOWN = 0x104;//SYSKEYDOWN
        private const int WM_SYSKEYUP = 0x105;//SYSKEYUP

        /*
        private int KeyboardHookProc(int nCode, Int32 wParam, IntPtr lParam)
        {

            if (!Config.GetBool("回放功能"))
                return 0;

            int rt = 0;

            //key down事件处理

            if (nCode < 0)
                return 0;

            KeyboardHookStruct InputKey = (KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyboardHookStruct));


            if (Recorder.State == Recorder.RecorderState.Recording)
            {

                Recorder.RecItem r = new Recorder.RecItem();



                r.time = sw.ElapsedTicks;
                r.key = InputKey.vkCode;

                uint testbit = 1 << 7;
                uint testinject = 1 << 4;

                if ((InputKey.flags & testbit) == 0)
                    r.keystate = 0;
                else if ((InputKey.flags & testbit) != 0)
                    r.keystate = 2;
                r.modifier = 0;

                //被注入不记录
                if ((InputKey.flags ^ testinject) != 0)
                    Recorder.RecItems.Add(r);


            }





            return rt;
        }

*/
        #endregion


        static private int MouseHooked = 0;
        public void StartMouseHook()
        {

            IntPtr pInstance = Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().ManifestModule);
            //pInstance = (IntPtr)4194304;
            // IntPtr pInstanc2 = Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly());
            // Assembly.GetExecutingAssembly().GetModules()[0]
            // 假如没有安装鼠标钩子
            if (MouseHooked == 0)
            {
                MouseHookProcedure = new HookProc(MouseHookProc);
                MouseHooked = SetWindowsHookEx(WH_MOUSE_LL, MouseHookProcedure, pInstance, 0);
                if (MouseHooked == 0)
                {
                    StopMouseHook();
                    throw new Exception("安装鼠标钩子失败");
                }
            }

        }


        public void StopMouseHook()
        {
            bool retKeyboard = true;


            if (MouseHooked != 0)
            {
                retKeyboard = UnhookWindowsHookEx(MouseHooked);
                MouseHooked = 0;
            }

            if (!(retKeyboard))
                throw new Exception("卸载钩子失败！");
        }


        private int MouseHookProc(int nCode, Int32 wParam, IntPtr lParam)
        {

            if (wParam == 0x207)
            {
                HotKeyMButton();
                return 1;
            }

            return 0;
        }

        ChangSheng cs = null;
        private void BtnChangSheng_Click(object sender, RoutedEventArgs e)
        {
            cs = new ChangSheng(Config.GetString("长流用户名"), Config.GetString("长流密码"));
            string article = cs.GetArticle();
            if (article != null && article.Length > 0)
            {


  
                SldBlind.Value = 1;
                SldBindLookUpdate();
                
                LoadText(article, RetypeType.first, TxtSource.changeSheng);
            }


        }

        JBS jbs = null;
        private void BtnJbs_Click(object sender, RoutedEventArgs e) //锦标赛
        {
            jbs = new JBS(Config.GetString("极速用户名"), Config.GetString("极速密码"));
            string article = jbs.GetArticle();
            if (article != null && article.Length > 0)
            {


     
                SldBlind.Value = 1;
                SldBindLookUpdate();

                LoadText(article, RetypeType.first, TxtSource.jbs);
            }
        }

        public WinArticle winArticle;// = new WinArticle();
        private void BtnArticle_Click(object sender, RoutedEventArgs e)
        {
            foreach (Window item in Application.Current.Windows)
            {
                if (item is WinArticle)
                {
                    item.Focus();
                    item.Activate();
                    return;
                }

            }

            winArticle = new WinArticle();
 
            winArticle.Show();
        }

        private void BtnSendArticle_Click(object sender, RoutedEventArgs e)
        {
            SendArticle();
        }



        private void MainWin_Deactivated(object sender, EventArgs e)
        {
            if (StateManager.txtSource != TxtSource.changeSheng && StateManager.txtSource != TxtSource.jbs)
            {
                if (StateManager.typingState == TypingState.typing)
                {
                    StateManager.typingState = TypingState.pause;
                    TbkStatusTop.Text = "暂停\t" + TbkStatusTop.Text;
                    sw.Stop();
                    //              Recorder.Stop();
                    if (timerProgress != null)
                        timerProgress.Dispose();
                }

            }
        }


        /*
        private void ChkLookType_Click(object sender, RoutedEventArgs e)
        {
            if (StateManager.ConfigLoaded)
            {
                if (ChkLookType.IsChecked == true)
                    ChkBlindType.IsChecked = false;

                Config.Set("盲打模式", ChkBlindType.IsChecked == true ? "是" : "否");
                Config.Set("看打模式", ChkLookType.IsChecked == true || ChkBlindType.IsChecked == true ? "是" : "否");
                UpdateDisplay(UpdateLevel.PageArrange);
                TbxInput.Focus();
            }
        }


        private void ChkBlindType_Click(object sender, RoutedEventArgs e)
        {
            if (StateManager.ConfigLoaded)
            {
                if (ChkBlindType.IsChecked == true)
                    ChkLookType.IsChecked = false;

                Config.Set("盲打模式", ChkBlindType.IsChecked == true ? "是" : "否");
                Config.Set("看打模式", ChkLookType.IsChecked == true || ChkBlindType.IsChecked == true ? "是" : "否");
                UpdateDisplay(UpdateLevel.PageArrange);
                TbxInput.Focus();
            }
        }
        */

        private void BtnPrev_Click(object sender, RoutedEventArgs e)
        {
            ArticleManager.PrevSection();
            LoadText(ArticleManager.GetFormattedCurrentSection(), RetypeType.first, TxtSource.book, false);
            TbxInput.Focus();

        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {

            ArticleManager.NextSection();



            LoadText(ArticleManager.GetFormattedCurrentSection(), RetypeType.first, TxtSource.book, false);
            TbxInput.Focus();

        }



        public void UpdateButtonProgress()
        {
            if (ArticleManager.Title == "")
            {
                BtnSendArticle.IsEnabled = false;
                BtnNext.IsEnabled = false;
                BtnPrev.IsEnabled = false;
                BtnSendArticle.Content = "发文F2";

            }

            else
            {
                BtnSendArticle.IsEnabled = true;
                BtnNext.IsEnabled = true;
                BtnPrev.IsEnabled = true;
                BtnSendArticle.Content = "发文-" + ArticleManager.Title.Replace(".txt", "").Replace(".TXT", "").Replace(".Txt", "") + "-" + ArticleManager.Progress + "/" + ArticleManager.TotalSize;
            }
        }



        private int GetLookTyping() //获取正在盲打的字
        {

            string currentMatchText = string.Join("", TextInfo.Words);


            string t1 = currentMatchText.Replace('”', '\"').Replace('“', '\"').Replace('‘', '\'').Replace('’', '\'');
            string t2 = TbxInput.Text.Replace('”', '\"').Replace('“', '\"').Replace('‘', '\'').Replace('’', '\'');
            List<DiffRes> diffs = DiffTool.Diff(t1, t2);

            int pos = 0;
            int counter = 0;
            foreach (var df in diffs)
            {
                Run r = new Run();

                switch (df.Type)
                {
                    case DiffType.None:
                        r.Text = currentMatchText.Substring(df.OrigIndex, 1);
                        pos = df.OrigIndex + 1;
                        break;
                    case DiffType.Delete:

                        r.Text = currentMatchText.Substring(df.OrigIndex - 1, 1);
                        counter--;
                        r.Background = Colors.CorrectBackground;
                        break;
                    case DiffType.Add:

                        r.Text = TbxInput.Text.Substring(df.RevIndex + counter, 1);
                        counter++;
                        r.Background = Colors.IncorrectBackground;
                        break;

                }




            }

            if (pos >= currentMatchText.Length)
                pos = currentMatchText.Length - 1;


            return pos;
        }

        private void LogBack() //记录回改的字
        {
            string currentMatchText = string.Concat(TextInfo.Words);
            if (!Config.GetBool("错字重打"))
                return;

            int pos;
            string w;
            if (!IsLookingType)
            {
                pos = TextInfo.wordStates.IndexOf(WordStates.NO_TYPE);
                if (pos == -1)
                    pos = TextInfo.Words.Count - 1;
                w = TextInfo.Words[pos];
            }
            else
            {
                pos = GetLookTyping();
                w = currentMatchText.Substring(pos, 1);
            }

            if (pos >= 0)
            {

                //             if (!TextInfo.BackCounter.ContainsKey(pos))
                //           {
                //        TxtBack.Set(w, TxtBack.GetInt(w) + 1);
                //             TextInfo.BackCounter[pos] = w;
                //        }

                if (!TextInfo.WrongExclude.Contains(w))
                    TextInfo.WrongRec[pos] = w;
            }
        }

        private void LogCorrection()
        {
            string currentMatchText = string.Concat(TextInfo.Words);
            if (!Config.GetBool("错字重打"))
                return;
            int pos;
            string w;
            if (IsLookingType)
            {
                pos = GetLookTyping();
                if (pos < 0)
                    return;

                w = currentMatchText.Substring(pos, 1);
            }
            else
            {
                pos = TextInfo.wordStates.IndexOf(WordStates.NO_TYPE);
                if (pos == -1)
                    pos = TextInfo.Words.Count - 1;

                pos -= 1;
                if (pos < 0)
                    return;

                w = TextInfo.Words[pos];
            }


            /*
            if (!TextInfo.CorrectionCounter.ContainsKey(pos))
            {
                TxtCorrection.Set(w, TxtCorrection.GetInt(w) + 1);
                TextInfo.CorrectionCounter[pos] = w;
            }
            */
            if (!TextInfo.WrongExclude.Contains(w))
                TextInfo.WrongRec[pos] = w;

        }


        private void LogWrong(int pos, string w)
        {
            if (!Config.GetBool("错字重打"))
                return;

            /*
            if (!TextInfo.WrongCounter.ContainsKey(pos))
            {
                TxtWrong.Set(w, TxtWrong.GetInt(w) + 1);
                TextInfo.WrongCounter[pos] = w;
            }
            */
            if (!TextInfo.WrongExclude.Contains(w))
                TextInfo.WrongRec[pos] = w;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

            BtnF5.Content = "选群▸";


            FocusInput();
        }

        private void TbxResults_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateTypingStat();
        }

        private void BtnTrainer_Click(object sender, RoutedEventArgs e)
        {
            ShowWinTrainer();
        }



        WinTrainer winTrainer;
        private void ShowWinTrainer()
        {

            if (WinTrainer.Current != null)
           // if (winTrainer != null)
            {
                winTrainer.Show();
                winTrainer.Focus();
                winTrainer.Activate();
                //         winTrainer.InitText();
            }
            else
            {
                winTrainer = new WinTrainer();
                winTrainer.Show();
                winTrainer.Activate();
                //    winTrainer.InitText();
            }

        }


        public string QQGroupName
        {
            get
            {
                if (BtnF5.Content.ToString().Length > 3)
                {
                    return BtnF5.Content.ToString().Substring(3);
                }
                else
                    return "";
            }

        }


        public void FocusInput()
        {
            this.Activate();
            this.Topmost = true;  // important
            this.Topmost = false; // important
            this.Focus();
            TbxInput.Focus();
        }





        private void SldBindLookUpdate()
        {

                Config.Set("盲打模式", SldBlind.Value > 1.99 && SldBlind.Value < 2.01 ? true : false);
                Config.Set("看打模式", SldBlind.Value > 2.99 && SldBlind.Value < 3.01 ? true : false);


            if (Config.GetBool("盲打模式"))
            {
                LbBlindType.Content = "盲打";
          //      LbBlindType.Foreground = Brushes.Purple;
           //     SldBlind.Background = Brushes.Purple;

            }
            else if (Config.GetBool("看打模式"))
            {
                LbBlindType.Content = "看打";
           //     LbBlindType.Foreground = Brushes.Red;
        //        SldBlind.Background = Brushes.Red;
            }
            else
            {
                LbBlindType.Content = "跟打";
                LbBlindType.Foreground = Colors.FromString(Config.GetString("窗体字体色"));
            }
        }

        private void SldBlind_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if (StateManager.ConfigLoaded)
            {
                SldBindLookUpdate();
                UpdateDisplay(UpdateLevel.PageArrange);
                TbxInput.Focus();
            }
        }


    }
}
