using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Interop.UIAutomationClient;
using static TypeB.MainWindow;

namespace TypeB
{
    class MsgRequest
    {
        public string groupName = "";

        public  Window caller = null;

        public MsgRequest(string groupName, Window caller)
        {
            this.groupName = groupName;
            this.caller = caller;
        }
    }
    internal static class QQHelper
    {

        #region dll

        [DllImport("user32.dll", EntryPoint = "GetWindowText")]
        public static extern int GetWindowText(int hwnd, StringBuilder lpString, int cch);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "EnumWindows")]
        public static extern int EnumWindows(CallBack x, int y);

        [DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "GetClassName")]
        public static extern int GetClassName(int hWnd, StringBuilder lpClassName, int nMaxCount);

       
        #endregion


        public delegate bool CallBack(int hwnd, int lParam);

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "SwitchToThisWindow")]
        private static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);



     //   public static Queue<string> titles = new Queue<string>();

       public static List<string> AvailTitle = new List<string>();




        public static bool IsNewQQ = false;




        static CUIAutomation root = new CUIAutomation();

        static List<string> QunList = new List<string>();
     
        static public List<string> GetQunList()
        {
            //先用旧方法，如果为空，则用新方法
            QunList.Clear();
            //旧方法
            EnumWindows(CheckQunTitleLegacy, 0);
            

            //旧方法是否有效
            if (QunList.Count > 0)
            {
                IsNewQQ = false;
                return QunList; 
            }
            else  //新方法
            {
                string MainTitle = "QQ";
                var q = root.GetRootElement().FindFirst(TreeScope.TreeScope_Children, root.CreatePropertyCondition(UIA_PropertyIds.UIA_NamePropertyId, MainTitle));
                if (q == null || q.CurrentClassName == "TXGuiFoundation")
                    return QunList;

                if (null == q.FindFirst(TreeScope.TreeScope_Children, root.CreatePropertyCondition(UIA_PropertyIds.UIA_ControlTypePropertyId, UIA_ControlTypeIds.UIA_DocumentControlTypeId)))
                {
                    var wp = q.GetCurrentPattern(UIA_PatternIds.UIA_WindowPatternId) as IUIAutomationWindowPattern;
                    wp.SetWindowVisualState(WindowVisualState.WindowVisualState_Normal);
                    q.SetFocus();

                    Win32.Delay(50);
                }


                var grouplist = q.FindFirst(TreeScope.TreeScope_Descendants, root.CreatePropertyCondition(UIA_PropertyIds.UIA_NamePropertyId, "会话列表"));   //找不到消息区，aeMessage值为空



                if (grouplist == null)
                    return QunList;


                var groups = grouplist.FindAll(TreeScope.TreeScope_Children, root.CreateOrCondition(root.CreatePropertyCondition(UIA_PropertyIds.UIA_ControlTypePropertyId, UIA_ControlTypeIds.UIA_WindowControlTypeId), root.CreatePropertyCondition(UIA_PropertyIds.UIA_ControlTypePropertyId, UIA_ControlTypeIds.UIA_GroupControlTypeId)));


                if (groups.Length > 0)
                {
                    for (int i = 0; i < groups.Length; i++)
                    {

                        string s = groups.GetElement(i).CurrentName;
                        string title = s.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0];
                        QunList.Add(title);
                    }
                }

                if (QunList.Count > 0)
                {
                    IsNewQQ = true;
                    return QunList;
                }
            }




            return QunList;




        }



        public static bool CheckQunTitleLegacy(int hwnd, int lParam)
        {


            var r = new Regex(@"QQ20\d{2}");
            var ex = new[]
                {
                        "QQ",
                        "QQ音乐",
                        "TXMenuWindow",
                        "FaceSelector",
                        "TXFloatingWnd",
                        "腾讯",
                        "消息盒子",
                        "来自",
                        "分类推荐",
                        "更换房间头像",
                        "网络设置",
                         "验证消息",
                        "图片查看",
                        "消息管理器",
                        "QQ数据线",
                        "播放队列",
                        
                    };

            var s = new StringBuilder(512);
            GetWindowText(hwnd, s, s.Capacity);
            var title = s.ToString();
            if (!r.IsMatch(title) && !ex.Contains(title) && !string.IsNullOrEmpty(title) && !title.Contains(" - "))
            {
                var g = new StringBuilder(512);
                GetClassName(hwnd, g, 256);
                if (g.ToString() == "TXGuiFoundation")
                {

                    QunList.Add(title);
                    

                }
            }




            return true;
        }

        static Timer tmSend;
        public static void SendQQMessage (string groupName, string msgContent, int delayTime, Window caller)
        {


            Win32SetText(msgContent);

            if (msgContent == "" || groupName == "")
                return;

            try
            {
      //          Win32SetText(msgContent);
                MsgRequest m = new MsgRequest(groupName, caller);

                tmSend = new Timer(SendQQMessageHelper, m, delayTime, Timeout.Infinite);
            }
            catch (Exception)
            {

             
            }


        }

        private static void SendQQMessageHelper(object obj)
        {

            try
            {
                MsgRequest m = (MsgRequest)obj;
                string groupName = m.groupName;
                //   string msgContent = m.msgContent;
                Window caller = m.caller;






                if (IsNewQQ)
                {
                    string MainTitle = "QQ";
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


                    //获取消息列表，群列表
           //         var msglist = q.FindFirst(TreeScope.TreeScope_Descendants, root.CreatePropertyCondition(UIA_PropertyIds.UIA_NamePropertyId, "消息列表"));   //找不到消息区，aeMessage值为空
                    var grouplist = q.FindFirst(TreeScope.TreeScope_Descendants, root.CreatePropertyCondition(UIA_PropertyIds.UIA_NamePropertyId, "会话列表"));   //找不到消息区，aeMessage值为空

            //        if (msglist == null || grouplist == null)
                     if (grouplist == null)
                    {
                        return;
                    }

                    //查找输入框名
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
                        edits.SetFocus();
                        Win32.CtrlV();

                        //      Send3Reverse(text);
                        Win32.Delay(60);
                        if (Config.GetBool("自动发送成绩"))
                        {
                            var send = q.FindFirst(TreeScope.TreeScope_Descendants, root.CreatePropertyCondition(UIA_PropertyIds.UIA_NamePropertyId, "发送"));   //找不到消息区，aeMessage值为空
                            if (send != null)
                            {
                                var sp = send.GetCurrentPattern(UIA_PatternIds.UIA_InvokePatternId) as IUIAutomationInvokePattern;
                                if (sp != null)
                                {
                                    sp.Invoke();
                                    Win32.Delay(20);
                                }
                            }
                        }

                        caller.Dispatcher.Invoke(() => {

 //                           caller.Dispatcher.Invoke(() => {
                            MainWindow.Current.FocusInput();
                           
                        });

                    }
                    //查找按钮名字

                }
                else
                {


                    SwitchToThisWindow(FindWindow(null, groupName), true); //激活窗口
                    Win32.Delay(80);


                    Win32.CtrlA();
                    Win32.CtrlV();
                    // Send3(text);
                    Win32.Delay(60);
                    if (Config.GetBool("自动发送成绩"))
                    {
                        Win32.AltS();
                        Win32.Delay(20);
                    }

                    caller.Dispatcher.Invoke(() => {
                        MainWindow.Current.FocusInput();

                    });
                }
            }
            catch (Exception)
            {

     
            }


        }


        public static void SendQQMessageD(string groupName, string msgContent1, string msgContent2, int delayTime, Window caller)
        {

            Win32SetText(msgContent1);
            if (msgContent1 == "" || msgContent1 == ""  || groupName == "")
                return;


            try
            {


                Win32.Delay(20);


                if (IsNewQQ)
                {
                    string MainTitle = "QQ";
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


                    //获取消息列表，群列表
     //               var msglist = q.FindFirst(TreeScope.TreeScope_Descendants, root.CreatePropertyCondition(UIA_PropertyIds.UIA_NamePropertyId, "消息列表"));   //找不到消息区，aeMessage值为空
                    var grouplist = q.FindFirst(TreeScope.TreeScope_Descendants, root.CreatePropertyCondition(UIA_PropertyIds.UIA_NamePropertyId, "会话列表"));   //找不到消息区，aeMessage值为空

                  //  if (msglist == null || grouplist == null)
                        if ( grouplist == null)
                        {
                        return;
                    }

                    //查找输入框名
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


                    if (edits != null)
                    {
                        edits.SetFocus();
                        Win32.CtrlV();

                        //      Send3Reverse(text);
                        Win32.Delay(30);
                        Win32.Win32SetText(msgContent2);
                        IUIAutomationInvokePattern sp_catch = null;
                        if (Config.GetBool("自动发送成绩"))
                        {
                            var send = q.FindFirst(TreeScope.TreeScope_Descendants, root.CreatePropertyCondition(UIA_PropertyIds.UIA_NamePropertyId, "发送"));   //找不到消息区，aeMessage值为空
                            if (send != null)
                            {
                                var sp = send.GetCurrentPattern(UIA_PatternIds.UIA_InvokePatternId) as IUIAutomationInvokePattern;
                                if (sp != null)
                                {
                                    sp_catch = sp;
                                    sp.Invoke();
                                    Win32.Delay(20);
                                }
                            }
                        }

                    //    Win32.Win32SetText(msgContent2);

                        Win32.Delay(20);

                        Win32.CtrlV();

                        //      Send3Reverse(text);
                        Win32.Delay(30);
                        if (Config.GetBool("自动发送成绩"))
                        {

                            /*
                            var send = q.FindFirst(TreeScope.TreeScope_Descendants, root.CreatePropertyCondition(UIA_PropertyIds.UIA_NamePropertyId, "发送"));   //找不到消息区，aeMessage值为空
                            if (send != null)
                            {
                                var sp = send.GetCurrentPattern(UIA_PatternIds.UIA_InvokePatternId) as IUIAutomationInvokePattern;
                                if (sp != null)
                                {
                                    sp.Invoke();
                                    Win32.Delay(20);
                                }
                            }
                            */
                            if (sp_catch != null)
                            {
                                sp_catch.Invoke();
                                Win32.Delay(20);
                            }
                        }



                        caller.Dispatcher.Invoke(() => {
                            MainWindow.Current.FocusInput();

                        });

                    }
                    //查找按钮名字

                }
                else
                {


                    SwitchToThisWindow(FindWindow(null, groupName), true); //激活窗口
                    Win32.Delay(50);


                    Win32.CtrlA();
                    Win32.CtrlV();
                    // Send3(text);
                    Win32.Delay(30);
                    Win32.Win32SetText(msgContent2);
                    if (Config.GetBool("自动发送成绩"))
                    {
                        Win32.AltS();
                        Win32.Delay(20);
                    }



                    Win32.Delay(10);


                    Win32.CtrlA();
                    Win32.CtrlV();
                    // Send3(text);
                    Win32.Delay(30);
                    if (Config.GetBool("自动发送成绩"))
                    {
                        Win32.AltS();
                        Win32.Delay(20);
                    }



                    caller.Dispatcher.Invoke(() => {
                        MainWindow.Current.FocusInput();

                    });
                }
            }
            catch (Exception)
            {

           
            }










        }

    }
}
