using MyoSharp.Communication;
using MyoSharp.Device;
using MyoSharp.Exceptions;
using MyoSharp.Poses;
using SimplePdfViewer;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using Windows.Data.Pdf;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace SimplePdfViewer
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

        #region Methods



        public void CloseApp()
        {
            Application.Current.Exit();
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
                        CloseApp();
                        break;
                    case Pose.WaveIn:
                        scrollPDFDown();
                        break;
                    case Pose.WaveOut:
                        scrollPDFUp();
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

        #region Navigation
        private void GoToPDF_Click(object sender, RoutedEventArgs e)
        {
            //Frame.Navigate(typeof(PDF));
        }
        #endregion


        #region  Remote PDF
        public async void OpenRemote()
        {
            //Begin a new HTTPClient for access to urls
            HttpClient client = new HttpClient();
            //create a stream and wait for the client to get the url
            var stream = await
                client.GetStreamAsync("http://www.kkoworld.com/kitablar/suzanna-kollinz-acliq-oyunlari-1-hisse-eng.pdf");
            var memStream = new MemoryStream();
            await stream.CopyToAsync(memStream);
            memStream.Position = 0;
            Windows.Data.Pdf.PdfDocument doc = await Windows.Data.Pdf.PdfDocument.LoadFromStreamAsync(memStream.AsRandomAccessStream());
            //Write the number of pages in the PDF to a string
            txtPageCount.Text = doc.PageCount.ToString();
            //loads in the pdf
            Load(doc);
        }

        async void Load(Windows.Data.Pdf.PdfDocument pdfDoc)
        {
            PdfPages.Clear();

            //set up the pdf by saying i is less than total no. pages
            for (uint i = 0; i < pdfDoc.PageCount; i++)
            {
                //Set up a new image to render the pdf to the app
                BitmapImage image = new BitmapImage();

                //get i-the pdf doc
                var page = pdfDoc.GetPage(i);

                //Ref:docs.Microsoft.com
                //Provides random access of data in input and output streams
                //that are stored in memory instead of on disk.
                using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
                {
                    await page.RenderToStreamAsync(stream);
                    await image.SetSourceAsync(stream);
                }

                PdfPages.Add(image);
            }
        }

        //create an observable collection for the image and get/set
        public ObservableCollection<BitmapImage> PdfPages
        {
            get;
            set;
        } = new ObservableCollection<BitmapImage>();
        #endregion

        #region Local PDF

        #endregion

        #region Scroll Through The PDF
        public static int buttonClicked = 0;
 
        /* Declaring this as a method outside of the btn so I can use it 
        more than once, I'm going to have to call this method again
        within the myoPoses switch statement */

        public void scrollPDFDown()
        {
            buttonClicked++;
            if (buttonClicked > 0)
            {
                var transform = txtPageCount.TransformToVisual(stkpnlScrollView);
                Point absolutePosition = transform.TransformPoint(new Point(0, (1500 * buttonClicked)));
                scrollViewerPDF.ChangeView(null, absolutePosition.Y, null, false);
            }
            /*if (buttonClicked == 0)
            {
                var transform = txtPageCount.TransformToVisual(stkpnlScrollView);
                Point absolutePosition = transform.TransformPoint(new Point(0, 1500));
                scrollViewerPDF.ChangeView(null, absolutePosition.Y, null, false);
            }
            else if (buttonClicked == 1)
            {
                var transform = txtPageCount.TransformToVisual(stkpnlScrollView);
                Point absolutePosition = transform.TransformPoint(new Point(1500, 3000));
                scrollViewerPDF.ChangeView(null, absolutePosition.Y, null, false);
            }
            else if (buttonClicked == 2)
            {
                var transform = txtPageCount.TransformToVisual(stkpnlScrollView);
                Point absolutePosition = transform.TransformPoint(new Point(3000, 4500));
                scrollViewerPDF.ChangeView(null, absolutePosition.Y, null, false);
            }
            else if (buttonClicked == 3)
            {
                var transform = txtPageCount.TransformToVisual(stkpnlScrollView);
                Point absolutePosition = transform.TransformPoint(new Point(4500, 6000));
                scrollViewerPDF.ChangeView(null, absolutePosition.Y, null, false);
            }
            else if (buttonClicked == 4)
            {
                var transform = txtPageCount.TransformToVisual(stkpnlScrollView);
                Point absolutePosition = transform.TransformPoint(new Point(6000, 7500));
                scrollViewerPDF.ChangeView(null, absolutePosition.Y, null, false);
            }
            else if (buttonClicked == 5)
            {
                var transform = txtPageCount.TransformToVisual(stkpnlScrollView);
                Point absolutePosition = transform.TransformPoint(new Point(7500, 9000));
                scrollViewerPDF.ChangeView(null, absolutePosition.Y, null, false);
            }
            else if (buttonClicked == 6)
            {
                var transform = txtPageCount.TransformToVisual(stkpnlScrollView);
                Point absolutePosition = transform.TransformPoint(new Point(9000, 10500));
                scrollViewerPDF.ChangeView(null, absolutePosition.Y, null, false);
            }
            else if (buttonClicked == 7)
            {
                var transform = txtPageCount.TransformToVisual(stkpnlScrollView);
                Point absolutePosition = transform.TransformPoint(new Point(10500, 12000));
                scrollViewerPDF.ChangeView(null, absolutePosition.Y, null, false);
            }
            else if (buttonClicked == 8)
            {
                var transform = txtPageCount.TransformToVisual(stkpnlScrollView);
                Point absolutePosition = transform.TransformPoint(new Point(12000, 13500));
                scrollViewerPDF.ChangeView(null, absolutePosition.Y, null, false);
            }
            else if (buttonClicked == 9)
            {
                var transform = txtPageCount.TransformToVisual(stkpnlScrollView);
                Point absolutePosition = transform.TransformPoint(new Point(13500, 15000));
                scrollViewerPDF.ChangeView(null, absolutePosition.Y, null, false);
            }
            else if (buttonClicked == 10)
            {
                var transform = txtPageCount.TransformToVisual(stkpnlScrollView);
                Point absolutePosition = transform.TransformPoint(new Point(15000, 16500));
                scrollViewerPDF.ChangeView(null, absolutePosition.Y, null, false);
            } */

        }
        public void scrollPDFUp()
        {
            buttonClicked--;
            if (buttonClicked >= 0)
            {
                var transform = txtPageCount.TransformToVisual(stkpnlScrollView);
                Point absolutePosition = transform.TransformPoint(new Point(0, (1500 * buttonClicked)));
                scrollViewerPDF.ChangeView(null, absolutePosition.Y, null, false);
            }
        }
        public void btnPageDown_Click(object sender, RoutedEventArgs e)
        {
            scrollPDFDown();
            /*I started off thinking that it wouldn't be too difficult to
                get a scrollviewer to scroll programatically.
                Then I had a few difficulties crop up;
                I could use: 
            PdfPages.Move(i, j); where i is the start index and j is the
            index I want to move to.
            That resulted in the previous pages being cut out if I moved
            to page 5 for example so I couldn't scroll back. */

            /* I then tried to do a mechanism I thought, logically, would work;
              Making the start index i, the end index j global variables &
              incrementing that value. 
              But, unfortunately, that didn't seem to go down well. */


            /*I am now using the following code which transforms the page
                to a new position through the same starting and ending index
                idea. Still, I found that I would then have to do countless
                if/else statements with different start and end indexes to
                traverse through the pages. */

            /*As a result, I'm going to only do the navigation for the 1st
            10 pages, as it's quite a long document and I only need it to
            demonstrate functionality with a Myo, its not the main focus.  */

            //EDIT: Found I could just multiply the index by the amount of
            //times the button was clicked within the same statement:
//            Point absolutePosition = transform.TransformPoint(new Point(0, (1500 * buttonClicked)));
              //This nakes things much more optimal!

            /*   for (int i = 0; i < PdfPages.Count; i++)
            {

               //PdfPages.Move(0, 1);
           }*/
        }
        #endregion

        private void btnPageUp_Click(object sender, RoutedEventArgs e)
        {
            scrollPDFUp();
        }
    }
}
