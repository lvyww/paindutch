
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Windows.Threading;
using System.Collections.Generic;
using System.Drawing;


namespace TypeB
{

    [StructLayout(LayoutKind.Sequential)]
    public struct GUITHREADINFO
    {
        public int cbSize;
        public int flags;
        public IntPtr hwndActive;
        public IntPtr hwndFocus;
        public IntPtr hwndCapture;
        public IntPtr hwndMenuOwner;
        public IntPtr hwndMoveSize;
        public IntPtr hwndCaret;
        public RECT rectCaret;
    }
    public enum PROCESS_DPI_AWARENESS
    {
        PROCESS_DPI_UNAWARE = 0,
        PROCESS_SYSTEM_DPI_AWARE = 1,
        PROCESS_PER_MONITOR_DPI_AWARE = 2
    }
    /*
    public enum DPI_AWARENESS_CONTEXT
    {

        DPI_AWARENESS_CONTEXT_UNAWARE = -1,
        DPI_AWARENESS_CONTEXT_SYSTEM_AWARE = -2,
        DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE = -3,
        DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2 = -4,
        DPI_AWARENESS_CONTEXT_UNAWARE_GDISCALED = -5
    }
    */
    [StructLayout(LayoutKind.Sequential)]

    public struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct KeyboardHookStruct
    {
        public int vkCode;  //定一个虚拟键码。该代码必须有一个价值的范围1至254
        public int scanCode; // 指定的硬件扫描码的关键
        public int flags;  // 键标志
        public int time; // 指定的时间戳记的这个讯息
        public int dwExtraInfo; // 指定额外信息相关的信息
    }


    [StructLayout(LayoutKind.Sequential)]
    struct MOUSEINPUT
    {
        public int dx;
        public int dy;
        public uint mouseData;
        public uint dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct KEYBDINPUT
    {
        public ushort wVk;
        public ushort wScan;
        public uint dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct HARDWAREINPUT
    {
        public int uMsg;
        public short wParamL;
        public short wParamH;
    }

    [StructLayout(LayoutKind.Explicit)]
    struct MouseKeybdHardwareInputUnion
    {
        [FieldOffset(0)]
        public MOUSEINPUT mi;

        [FieldOffset(0)]
        public KEYBDINPUT ki;

        [FieldOffset(0)]
        public HARDWAREINPUT hi;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct INPUT
    {
        public uint type;
        public MouseKeybdHardwareInputUnion mkhi;
    }

    public static class DispatcherHelper
    {
        [SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public static void DoEvents()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(ExitFrames), frame);
            try { Dispatcher.PushFrame(frame); }
            catch (InvalidOperationException) { }
        }
        private static object ExitFrames(object frame)
        {
            ((DispatcherFrame)frame).Continue = false;
            return null;
        }
    }

    static internal class Win32
    {




        public const int WH_KEYBOARD_LL = 13;   //线程键盘钩子监听鼠标消息设为2，全局键盘监听鼠标消息设为13
        public const int WH_KEYBOARD = 20;   //线程键盘钩子监听鼠标消息设为2，全局键盘监听鼠标消息设为13








        public const int WM_KEYDOWN = 0x100;//KEYDOWN
        public const int WM_KEYUP = 0x101;//KEYUP
        public const int WM_SYSKEYDOWN = 0x104;//SYSKEYDOWN
        public const int WM_SYSKEYUP = 0x105;//SYSKEYUP




        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);
        [DllImport("user32.dll")]
        public static extern bool GetGUIThreadInfo(uint idThread, ref GUITHREADINFO lpgui);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);
        [DllImport("user32.dll")]
        public static extern void GetWindowText(IntPtr hWnd, StringBuilder lpString, Int32 nMaxCount);
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);


        [DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "GetClassName")]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out uint ProcessId);



        [DllImport("user32.dll")]
        public static extern int GetDpiForWindow(IntPtr hWnd);




        [DllImport("shcore.dll")]
        public static extern UInt32 GetDpiForMonitor(IntPtr hmonitor,
                                              int dpiType,
                                              out UInt32 dpiX,
                                              out UInt32 dpiY);

        [DllImport("user32.dll")]
        public static extern IntPtr MonitorFromWindow(IntPtr hwnd, UInt32 dwFlags);



        public static GUITHREADINFO? GetGuiThreadInfo(IntPtr hwnd)
        {
            if (hwnd != IntPtr.Zero)
            {
                uint threadId = GetWindowThreadProcessId(hwnd, IntPtr.Zero);
                GUITHREADINFO guiThreadInfo = new GUITHREADINFO();
                guiThreadInfo.cbSize = Marshal.SizeOf(guiThreadInfo);
                if (GetGUIThreadInfo(threadId, ref guiThreadInfo) == false)
                    return null;
                return guiThreadInfo;
            }
            return null;
        }

        public static string GetWindowTitle(IntPtr hwnd)
        {


            if (hwnd != IntPtr.Zero)
            {
                int length = Win32.GetWindowTextLength(hwnd);
                StringBuilder sb = new StringBuilder(length * 2 + 1);
                Win32.GetWindowText(hwnd, sb, sb.Capacity);
                return sb.ToString();
            }
            else
                return "";
        }

        const int GWL_EXSTYLE = -20;
        const long WS_EX_TOPMOST = 0x00000008L;
        public static bool IsTopMost(IntPtr hwnd)
        {
            if ((GetWindowLong(hwnd, GWL_EXSTYLE) & WS_EX_TOPMOST) != 0)
                return true;
            else
                return false;
        }
        public static string GetActiveProcessFileName(IntPtr hwnd)
        {

            uint pid;
            Win32.GetWindowThreadProcessId(hwnd, out pid);
            Process p = Process.GetProcessById((int)pid);
            return p.MainModule.ModuleName;
        }

        public static string GetWindowClassName(IntPtr hwnd)
        {


            if (hwnd != IntPtr.Zero)
            {
                var g = new StringBuilder(512);
                Win32.GetClassName(hwnd, g, 256);
                return g.ToString();
            }
            else
                return "";
        }




        #region 屏幕信息

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
        public class MONITORINFOEX
        {
            public int cbSize = Marshal.SizeOf(typeof(MONITORINFOEX));
            public RECT rcMonitor = new RECT();
            public RECT rcWork = new RECT();
            public int dwFlags = 0;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public char[] szDevice = new char[32];
        }




        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetMonitorInfo(IntPtr hmonitor, [In, Out] MONITORINFOEX info);
        #endregion



        public static Tuple<double, double> GetWorkingArea(IntPtr hwnd)
        {
            IntPtr monitor = Win32.MonitorFromWindow(hwnd, 2);

            MONITORINFOEX monitorInfo = new MONITORINFOEX();


            GetMonitorInfo(monitor, monitorInfo);

            double b = monitorInfo.rcWork.bottom;
            double r = monitorInfo.rcWork.right;

            uint dpiX, dpiY;
            Win32.GetDpiForMonitor(monitor, 0, out dpiX, out dpiY);

            r = r * 96.0 / dpiX;
            b = b * 96.0 / dpiY;

            return new Tuple<double, double>(r, b);

        }

        #region 键盘钩子

        public delegate int HookProc(int nCode, Int32 wParam, IntPtr lParam);


        //使用此功能，安装了一个钩子
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);


        //调用此函数卸载钩子
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(int idHook);


        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int CallNextHookEx(int idHook, int nCode, int wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern long GetWindowLong(IntPtr hWnd, int nIndex);


        //使用WINDOWS API函数代替获取当前实例的函数,防止钩子失效
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string name);


        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern short GetKeyState(int vKey);




        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);





        #endregion


















        #region COM


        [DllImport("ole32.dll")]
        static extern int CoInitialize(IntPtr pvReserved);
        [DllImport("ole32.dll")]
        static extern int CoUninitialize();

        [DllImport("oleacc.dll")]
        internal static extern int AccessibleObjectFromWindow(IntPtr hwnd, uint id, ref Guid iid, [In, Out, MarshalAs(UnmanagedType.IUnknown)] ref object ppvObject);

        public const uint CARET = 0xFFFFFFF8;




        #endregion














        #region 剪贴版


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
            if (!OpenClipboard(IntPtr.Zero)) { Win32SetText(text); return; }
            EmptyClipboard();
            SetClipboardData(13, Marshal.StringToHGlobalUni(text));
            CloseClipboard();
        }
        internal static void Win32SetTextAndImage(string text)
        {
            if (!OpenClipboard(IntPtr.Zero)) { Win32SetText(text); return; }
            EmptyClipboard();
            SetClipboardData(13, Marshal.StringToHGlobalUni(text));
            CloseClipboard();
            Bitmap bmp = null;
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

        #endregion




        #region sendtext



        [DllImport("user32")]
        public static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);
        #endregion



        //dpi




        /*
        [DllImport("user32.dll")]
        public static extern DPI_AWARENESS_CONTEXT GetWindowDpiAwarenessContext(IntPtr hwnd);

 */
   //     public static List <int> DelayCount = new List<int>();
        public static void Delay(int milliSecond)
        {
            int start = Environment.TickCount;
            while (Math.Abs(Environment.TickCount - start) < milliSecond)//毫秒
            {
                DispatcherHelper.DoEvents();
            }
     //       DelayCount.Add( milliSecond);
        }



        static public void SimulateInputKey(int key)
        {

            INPUT[] input = new INPUT[1];

            input[0].type = 1;//模拟键盘
            input[0].mkhi.ki.wVk = (ushort)key;
            input[0].mkhi.ki.dwFlags = 0;//按下
            SendInput(1u, input, Marshal.SizeOf((object)default(INPUT)));
            //          Sleep(50);

            input[0].type = 1;//模拟键盘
            input[0].mkhi.ki.wVk = (ushort)key;
            input[0].mkhi.ki.dwFlags = 2;//抬起
            SendInput(1u, input, Marshal.SizeOf((object)default(INPUT)));

        }

        static public void SimulateInputKeyDown(int key)
        {

            INPUT[] input = new INPUT[1];

            input[0].type = 1;//模拟键盘
            input[0].mkhi.ki.wVk = (ushort)key;
            input[0].mkhi.ki.dwFlags = 0;//按下
            SendInput(1u, input, Marshal.SizeOf((object)default(INPUT)));
            //          Sleep(50);


        }
        static public void SimulateInputKeyUp(int key)
        {

            INPUT[] input = new INPUT[1];



            input[0].type = 1;//模拟键盘
            input[0].mkhi.ki.wVk = (ushort)key;
            input[0].mkhi.ki.dwFlags = 2;//抬起
            SendInput(1u, input, Marshal.SizeOf((object)default(INPUT)));


        }





        public static void  CtrlTab()
        {


            SimulateInputKeyDown(VK_LCONTROL);
            Delay(KEY_DELAY);
            SimulateInputKeyDown(VK_TAB);
            Delay(KEY_DELAY);
            SimulateInputKeyUp(VK_TAB);
            Delay(KEY_DELAY);
            SimulateInputKeyUp(VK_LCONTROL);

        }

        static public void CtrlA()
        {

            SimulateInputKeyDown(VK_LCONTROL);
            Delay(KEY_DELAY);
            SimulateInputKeyDown(VK_A);
            Delay(KEY_DELAY);
            SimulateInputKeyUp(VK_A);
            Delay(KEY_DELAY);
            SimulateInputKeyUp(VK_LCONTROL);
        }

        static public void CtrlV()
        {

            SimulateInputKeyDown(VK_LCONTROL);
            Delay(KEY_DELAY);
            SimulateInputKeyDown(VK_V);
            Delay(KEY_DELAY);
            SimulateInputKeyUp(VK_V);
            Delay(KEY_DELAY);
            SimulateInputKeyUp(VK_LCONTROL);

        }


        static public void CtrlC()
        {
            SimulateInputKeyDown(VK_LCONTROL);
            Delay(KEY_DELAY);
            SimulateInputKeyDown(VK_C);
            Delay(KEY_DELAY);
            SimulateInputKeyUp(VK_C);
            Delay(KEY_DELAY);
            SimulateInputKeyUp(VK_LCONTROL);

        }


        static public void AltS()
        {


            SimulateInputKeyDown(VK_LMENU);
            Delay(KEY_DELAY);
            SimulateInputKeyDown(VK_S);
            Delay(KEY_DELAY);
            SimulateInputKeyUp(VK_S);
            Delay(KEY_DELAY);
            SimulateInputKeyUp(VK_LMENU);


        }
        static public void Tab()
        {
            SimulateInputKey(VK_TAB);


        }


        #region
        const int VK_LBUTTON = 0x01; //鼠标左键
        const int VK_RBUTTON = 0x02; //鼠标右键
        const int VK_CANCEL = 0x03; //控制中断处理
        const int VK_MBUTTON = 0x04; //中间鼠标按钮 (三键鼠标)
        const int VK_XBUTTON1 = 0x05; //X1 鼠标按钮
        const int VK_XBUTTON2 = 0x06; //X2 鼠标按钮

        public const int VK_BACK = 0x08; //BACKSPACE 密钥
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
        const int VK_A = 65;
        const int VK_B = 66;
        const int VK_C = 67;
        const int VK_D = 68;
        const int VK_E = 69;
        const int VK_F = 70;
        const int VK_G = 71;
        const int VK_H = 72;
        const int VK_I = 73;
        const int VK_P = 80;
        const int VK_Q = 81;
        const int VK_R = 82;
        const int VK_S = 83;
        const int VK_T = 84;
        const int VK_U = 85;
        const int VK_V = 86;

        const int KEY_DELAY = 25;
        #endregion

    }
}

