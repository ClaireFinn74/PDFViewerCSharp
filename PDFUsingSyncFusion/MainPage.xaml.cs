using MyoSharp.Communication;
using MyoSharp.Device;
using MyoSharp.Exceptions;
using MyoSharp.Poses;
using Syncfusion.Pdf.Parsing;
using Syncfusion.Windows.PdfViewer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace PDFUsingSyncFusion
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        MyoSharp.Communication.IChannel _myoChannel;
        MyoSharp.Communication.IChannel _myoChannel1;
        IHub _myoHub;
        IHub _myoHub1;
        Pose _currentPose;
        double _currentRoll;

        public MainPage()
        {
            this.InitializeComponent();
        }

        #region Myo Setup
        public void btnCheckConnection_Click(object sender, RoutedEventArgs e)
        {
            // communication, device, exceptions, poses
            // create the channel 
            _myoChannel = Channel.Create(ChannelDriver.Create(ChannelBridge.Create(),
                                    MyoErrorHandlerDriver.Create(MyoErrorHandlerBridge.Create())));

            // create the hub with the channel
            _myoHub = MyoSharp.Device.Hub.Create(_myoChannel);
            // create the event handlers for connect and disconnect
            _myoHub.MyoConnected += _myoHub_MyoConnected;
            _myoHub.MyoDisconnected += _myoHub_MyoDisconnected;

            // start listening 
            _myoChannel.StartListening();


            // create the channel 
            _myoChannel1 = Channel.Create(ChannelDriver.Create(ChannelBridge.Create(),
                                    MyoErrorHandlerDriver.Create(MyoErrorHandlerBridge.Create())));

            // create the hub with the channel
            _myoHub1 = MyoSharp.Device.Hub.Create(_myoChannel1);
            // create the event handlers for connect and disconnect
            _myoHub1.MyoConnected += _myoHub_MyoConnected;
            _myoHub1.MyoDisconnected += _myoHub_MyoDisconnected;

            // start listening 
            _myoChannel1.StartListening();
        }

        internal void OnSourceChanged()
        {
            throw new NotImplementedException();
        }

        internal void OnIsZoomEnabledChanged()
        {
            throw new NotImplementedException();
        }

        public async void _myoHub_MyoDisconnected(object sender, MyoEventArgs e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                tblUpdates.Text = tblUpdates.Text + System.Environment.NewLine +
                                    "Myo disconnected";
            });
            _myoHub.MyoConnected -= _myoHub_MyoConnected;
            _myoHub.MyoDisconnected -= _myoHub_MyoDisconnected;
        }

        public async void _myoHub_MyoConnected(object sender, MyoEventArgs e)
        {
            e.Myo.Vibrate(VibrationType.Long);
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                tblUpdates.Text = "Myo Connected: " + e.Myo.Handle;
            });
            // add the pose changed event here
            e.Myo.PoseChanged += Myo_PoseChanged;

            // unlock the Myo so that it doesn't keep locking between our poses
            e.Myo.Unlock(UnlockType.Hold);

            try
            {
                var sequence = PoseSequence.Create(e.Myo, Pose.FingersSpread, Pose.WaveIn);
                sequence.PoseSequenceCompleted += Sequence_PoseSequenceCompleted;

            }
            catch (Exception myoErr)
            {
                string strMsg = myoErr.Message;
            }

        }


        public async void Sequence_PoseSequenceCompleted(object sender, PoseSequenceEventArgs e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                tblUpdates.Text = "Pose Sequence completed";
            });
        }

        public async void Pose_Triggered(object sender, PoseEventArgs e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                tblUpdates.Text = "Pose Held: " + e.Pose.ToString();
            });

        }
        #endregion

        #region MyoPoses

        public async void Myo_PoseChanged(object sender, PoseEventArgs e)
        {
            Pose curr = e.Pose;
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                tblUpdates.Text = curr.ToString();
                switch (curr)
                {
                    case Pose.Rest:
                        break;
                    case Pose.Fist:
                       
                        break;
                    case Pose.WaveIn:
                        break;
                    case Pose.WaveOut:
                        break;
                    case Pose.FingersSpread:
                        break;
                    case Pose.DoubleTap:
                        //chboxGithub.IsChecked = true;
                        break;
                    case Pose.Unknown:
                        var dialog = new MessageDialog("I'm sorry, that pose seems to be unknown.");
                        dialog.Title = "Pose Unknown Help";
                        dialog.Commands.Add(new UICommand { Label = "How to Open\\close the App with a Pose.", Id = 0 });
                        dialog.Commands.Add(new UICommand { Label = "How to turn the pages with a Pose.", Id = 1 });
                        var res = await dialog.ShowAsync();

                        if ((int)res.Id == 0)
                        {
                            var dialog0 = new MessageDialog("Try a Fist gesture to close the app." + "\n"
                            + "Try a Fingers-Spread gesture to Open the App. :)");
                            await dialog0.ShowAsync();

                        }
                        if ((int)res.Id == 1)
                        {
                            var dialog1 = new MessageDialog("Try a Wave-in gesture to turn the page."
                            + "\n" + "Try a Wave-Out gesture to go back a page.");
                            await dialog1.ShowAsync();
                        };
                        break;
                    default:
                        break;
                }
            });
        }
        #endregion


        #region PDFStuffSyncFusion
        private async void BtnOpen_Click(object sender, RoutedEventArgs e)
        {
            //Opens a file picker.
            var picker = new FileOpenPicker();
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.ViewMode = PickerViewMode.List;
            //Filters PDF files in the documents library.
            picker.FileTypeFilter.Add(".pdf");
            var file = await picker.PickSingleFileAsync();
            if (file == null) return;
            //Reads the stream of the loaded PDF document.
            var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
            Stream fileStream = stream.AsStreamForRead();
            byte[] buffer = new byte[fileStream.Length];
            fileStream.Read(buffer, 0, buffer.Length);
            //Loads the PDF document.
            PdfLoadedDocument ldoc = new PdfLoadedDocument(buffer);
            pdfViewer.LoadDocument(ldoc);
            // Sets the PDF Page View Mode to Fit Width.
            pdfViewer.ViewMode = PageViewMode.FitWidth;
            //Getting the amount of pages in the PDF Document
            int pageCount = pdfViewer.PageCount;
            
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            pdfViewer.SearchText((searchTxtBx.Text));
            searchTxtBx.Foreground = new SolidColorBrush(Colors.HotPink);
        }
        #endregion
    }
}