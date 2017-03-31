﻿#pragma checksum "C:\Users\owner\Downloads\PDFViewerCSharp\SimplePdfViewer\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "4DE3F8BB535F3800445779F0CF1E54EE"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SimplePdfViewer
{
    partial class MainPage : 
        global::Windows.UI.Xaml.Controls.Page, 
        global::Windows.UI.Xaml.Markup.IComponentConnector,
        global::Windows.UI.Xaml.Markup.IComponentConnector2
    {
        internal class XamlBindingSetters
        {
        };

        private class MainPage_obj1_Bindings :
            global::Windows.UI.Xaml.Markup.IComponentConnector,
            IMainPage_Bindings
        {
            private global::SimplePdfViewer.MainPage dataRoot;
            private bool initialized = false;
            private const int NOT_PHASED = (1 << 31);
            private const int DATA_CHANGED = (1 << 30);

            // Fields for each control that has bindings.
            private global::Windows.UI.Xaml.Controls.AppBarToggleButton obj10;

            public MainPage_obj1_Bindings()
            {
            }

            // IComponentConnector

            public void Connect(int connectionId, global::System.Object target)
            {
                switch(connectionId)
                {
                    case 10:
                        this.obj10 = (global::Windows.UI.Xaml.Controls.AppBarToggleButton)target;
                        ((global::Windows.UI.Xaml.Controls.AppBarToggleButton)target).Tapped += (global::System.Object param0, global::Windows.UI.Xaml.Input.TappedRoutedEventArgs param1) =>
                        {
                        this.dataRoot.openRemote();
                        };
                        break;
                    default:
                        break;
                }
            }

            // IMainPage_Bindings

            public void Initialize()
            {
                if (!this.initialized)
                {
                    this.Update();
                }
            }
            
            public void Update()
            {
                this.initialized = true;
            }

            public void StopTracking()
            {
            }

            // MainPage_obj1_Bindings

            public void SetDataRoot(global::SimplePdfViewer.MainPage newDataRoot)
            {
                this.dataRoot = newDataRoot;
            }

            public void Loading(global::Windows.UI.Xaml.FrameworkElement src, object data)
            {
                this.Initialize();
            }

            // Update methods for each path node used in binding steps.
        }
        /// <summary>
        /// Connect()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 14.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                {
                    this.root = (global::Windows.UI.Xaml.Controls.Page)(target);
                }
                break;
            case 2:
                {
                    this.Grid = (global::Windows.UI.Xaml.Controls.Grid)(target);
                }
                break;
            case 3:
                {
                    this.inkCanvas = (global::Windows.UI.Xaml.Controls.InkCanvas)(target);
                }
                break;
            case 4:
                {
                    this.txtScroll = (global::Windows.UI.Xaml.Controls.TextBox)(target);
                }
                break;
            case 5:
                {
                    this.PenColor = (global::Windows.UI.Xaml.Controls.ComboBox)(target);
                    #line 40 "..\..\..\MainPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.ComboBox)this.PenColor).SelectionChanged += this.OnPenColorChanged;
                    #line default
                }
                break;
            case 6:
                {
                    this.scrollViewerPDF = (global::Windows.UI.Xaml.Controls.ScrollViewer)(target);
                }
                break;
            case 7:
                {
                    this.tblUpdates = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 8:
                {
                    this.stkpnlScrollView = (global::Windows.UI.Xaml.Controls.StackPanel)(target);
                }
                break;
            case 9:
                {
                    this.txtScrollViewHandler = (global::Windows.UI.Xaml.Controls.TextBox)(target);
                }
                break;
            case 10:
                {
                    this.RemotePDF = (global::Windows.UI.Xaml.Controls.AppBarToggleButton)(target);
                }
                break;
            case 11:
                {
                    this.btnCheckConnection = (global::Windows.UI.Xaml.Controls.AppBarToggleButton)(target);
                    #line 22 "..\..\..\MainPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.AppBarToggleButton)this.btnCheckConnection).Click += this.btnCheckConnection_Click;
                    #line default
                }
                break;
            case 12:
                {
                    this.btnPageUp = (global::Windows.UI.Xaml.Controls.AppBarButton)(target);
                    #line 23 "..\..\..\MainPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.AppBarButton)this.btnPageUp).Click += this.btnPageUp_Click;
                    #line default
                }
                break;
            case 13:
                {
                    this.btnPageDown = (global::Windows.UI.Xaml.Controls.AppBarButton)(target);
                    #line 24 "..\..\..\MainPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.AppBarButton)this.btnPageDown).Click += this.btnPageDown_Click;
                    #line default
                }
                break;
            case 14:
                {
                    this.btnOpenLocalPdf = (global::Windows.UI.Xaml.Controls.AppBarButton)(target);
                    #line 25 "..\..\..\MainPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.AppBarButton)this.btnOpenLocalPdf).Click += this.btnOpenLocalPdf_Click;
                    #line default
                }
                break;
            case 15:
                {
                    this.btnStartInking = (global::Windows.UI.Xaml.Controls.AppBarButton)(target);
                    #line 26 "..\..\..\MainPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.AppBarButton)this.btnStartInking).Click += this.btnStartInking_Click;
                    #line default
                }
                break;
            case 16:
                {
                    this.btnClear = (global::Windows.UI.Xaml.Controls.AppBarButton)(target);
                    #line 27 "..\..\..\MainPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.AppBarButton)this.btnClear).Click += this.btnClear_Click;
                    #line default
                }
                break;
            case 17:
                {
                    this.btnSave = (global::Windows.UI.Xaml.Controls.AppBarButton)(target);
                    #line 28 "..\..\..\MainPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.AppBarButton)this.btnSave).Click += this.btnSave_Click;
                    #line default
                }
                break;
            case 18:
                {
                    this.btnLoad = (global::Windows.UI.Xaml.Controls.AppBarButton)(target);
                    #line 29 "..\..\..\MainPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.AppBarButton)this.btnLoad).Click += this.btnLoad_Click;
                    #line default
                }
                break;
            case 19:
                {
                    this.txtUri = (global::Windows.UI.Xaml.Controls.TextBox)(target);
                }
                break;
            default:
                break;
            }
            this._contentLoaded = true;
        }

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 14.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::Windows.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target)
        {
            global::Windows.UI.Xaml.Markup.IComponentConnector returnValue = null;
            switch(connectionId)
            {
            case 1:
                {
                    global::Windows.UI.Xaml.Controls.Page element1 = (global::Windows.UI.Xaml.Controls.Page)target;
                    MainPage_obj1_Bindings bindings = new MainPage_obj1_Bindings();
                    returnValue = bindings;
                    bindings.SetDataRoot(this);
                    this.Bindings = bindings;
                    element1.Loading += bindings.Loading;
                }
                break;
            }
            return returnValue;
        }
    }
}
