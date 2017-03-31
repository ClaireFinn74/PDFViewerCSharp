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
using Windows.UI.Input.Inking;
using System.Collections.Generic;
using System.Linq;
using Windows.Storage;
using Windows.Storage.Pickers;
using System.Net;
using System.Text;

namespace SimplePdfViewer
{
    public sealed partial class MainPage : Page
    {
        //Open a channel of Communication with the Myo Armband and it's Hubs.
        MyoSharp.Communication.IChannel _myoChannel;
        MyoSharp.Communication.IChannel _myoChannel1;
        IHub _myoHub;
        IHub _myoHub1;
        Pose _currentPose;
        double _currentRoll;


        public MainPage()
        {
            this.InitializeComponent();
            #region inkCanvas Setup
            // Set supported inking device types.
            inkCanvas.InkPresenter.InputDeviceTypes =
            Windows.UI.Core.CoreInputDeviceTypes.Mouse |
            Windows.UI.Core.CoreInputDeviceTypes.Pen;

            // Set initial ink stroke attributes.
            InkDrawingAttributes drawingAttributes = new InkDrawingAttributes();
            drawingAttributes.Color = Windows.UI.Colors.Black;
            inkCanvas.InkPresenter.UpdateDefaultDrawingAttributes(drawingAttributes);

            #endregion

            // Listen for button click to initiate saving a note.
            btnSave.Click += btnSave_Click;
            // Listen for button click to initiate loading a note.
            btnLoad.Click += btnLoad_Click;
        }

        #region inkCanvas (Annotations...Sort of.)
        // Update ink stroke color for new strokes.
        private void OnPenColorChanged(object sender, SelectionChangedEventArgs e)
        {
            if (inkCanvas != null)
            {
                InkDrawingAttributes drawingAttributes =
                    inkCanvas.InkPresenter.CopyDefaultDrawingAttributes();

                //Pick a colour to write a note/draw.
                string value = ((ComboBoxItem)PenColor.SelectedItem).Content.ToString();
                //Using a switch statement for these colors
                switch (value)
                {
                    case "Pink":
                        drawingAttributes.Color = Windows.UI.Colors.HotPink;
                        break;
                    case "Black":
                        drawingAttributes.Color = Windows.UI.Colors.Black;
                        break;
                    case "Red":
                        drawingAttributes.Color = Windows.UI.Colors.Red;
                        break;
                    case "Green":
                        drawingAttributes.Color = Windows.UI.Colors.Green;
                        break;
                    case "Yellow":
                        drawingAttributes.Color = Windows.UI.Colors.Yellow;
                        break;
                    case "Blue":
                        drawingAttributes.Color = Windows.UI.Colors.Blue;
                        break;
                    case "Orange":
                        drawingAttributes.Color = Windows.UI.Colors.Orange;
                        break;
                    case "Brown":
                        drawingAttributes.Color = Windows.UI.Colors.Brown;
                        break;
                    //Always default to a standard black colour
                    default:
                        drawingAttributes.Color = Windows.UI.Colors.Black;
                        break;
                };

                inkCanvas.InkPresenter.UpdateDefaultDrawingAttributes(drawingAttributes);
            }
        }
        #endregion

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

        #region Methods (saveNote, closeApp, openRemote, Load, openLocalPdf) 

        //A method to save a note made with inkCanvas using File Picker
        public async void saveNote()
        {
            // Get all strokes on the InkCanvas.
            IReadOnlyList<InkStroke> currentStrokes = inkCanvas.InkPresenter.StrokeContainer.GetStrokes();

            // Strokes present on ink canvas.
            if (currentStrokes.Count > 0)
            {
                // Let users choose their ink file using a file picker.
                // Initialize the picker.
                Windows.Storage.Pickers.FileSavePicker savePicker =
                    new Windows.Storage.Pickers.FileSavePicker();
                savePicker.SuggestedStartLocation =
                    Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
                savePicker.FileTypeChoices.Add(
                    "GIF with embedded ISF",
                    new List<string>() { ".gif" });
                savePicker.DefaultFileExtension = ".gif";
                savePicker.SuggestedFileName = "InkSample";

                // Show the file picker.
                Windows.Storage.StorageFile file =
                    await savePicker.PickSaveFileAsync();
                // When chosen, picker returns a reference to the selected file.
                if (file != null)
                {
                    // Prevent updates to the file until updates are 
                    // finalized with call to CompleteUpdatesAsync.
                    Windows.Storage.CachedFileManager.DeferUpdates(file);
                    // Open a file stream for writing.
                    IRandomAccessStream stream = await file.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite);
                    // Write the ink strokes to the output stream.
                    using (IOutputStream outputStream = stream.GetOutputStreamAt(0))
                    {
                        await inkCanvas.InkPresenter.StrokeContainer.SaveAsync(outputStream);
                        await outputStream.FlushAsync();
                    }
                    stream.Dispose();

                    // Finalize write so other apps can update file.
                    Windows.Storage.Provider.FileUpdateStatus status =
                        await Windows.Storage.CachedFileManager.CompleteUpdatesAsync(file);

                    if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
                    {
                        // File saved.
                    }
                    else
                    {
                        // File couldn't be saved.
                    }
                }
                // User selects Cancel and picker returns null.
                else
                {
                    // Operation cancelled.
                }
            }
        }

        /*A method to close the app using Appliation.Current.Exit for Fist
          gesture of the Myo Armband to use. */
        public void closeApp()
        {
            Application.Current.Exit();
        }
        
        /* A method to open a Remote PDF (The Hunger Games book) from a 
           stream using HttpClient and a URL. */
        public async void openRemote()
        {
            //Begin a new HTTPClient for access to urls
            HttpClient client = new HttpClient();
            //create a stream and wait for the client to get the url
            var stream = await
                  client.GetStreamAsync(txtUri.Text);
            //   client.GetStreamAsync("http://www.kkoworld.com/kitablar/suzanna-kollinz-acliq-oyunlari-1-hisse-eng.pdf");
            var memStream = new MemoryStream();
            await stream.CopyToAsync(memStream);
            memStream.Position = 0;
            Windows.Data.Pdf.PdfDocument doc = await Windows.Data.Pdf.PdfDocument.LoadFromStreamAsync(memStream.AsRandomAccessStream());
            //loads in the pdf
            Load(doc);
        }



        /* A method so the user can pick their own PDF file locally.
           The file will open in Adobe Acrobat unfortunately.
           Also using File Picker. */
        public async void openLocalPDF()
        {
            StorageFile file = null;
            FileOpenPicker filePicker = new FileOpenPicker();
            filePicker.FileTypeFilter.Add(".pdf");
            filePicker.ViewMode = PickerViewMode.Thumbnail;
            //Starting to look for the PDF in the Downloads Folder
            filePicker.SuggestedStartLocation = PickerLocationId.Downloads;
            filePicker.SettingsIdentifier = "picker1";
            filePicker.CommitButtonText = "Open Pdf File";
            //Opening only one file
            file = await filePicker.PickSingleFileAsync();
            //Opening it asynchronously for optimization.
            await Windows.System.Launcher.LaunchFileAsync(file);

        }

        //A method to Load in the Remote PDF file
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

        //A method to deal with starting drawing with the inkCanvas
        public void startScribbling()
        {
            inkCanvas.InkPresenter.InputProcessingConfiguration.Mode = InkInputProcessingMode.Inking;
        }

        //A method to deal with erasing scribbles
        public void eraseScribble()
        {
            inkCanvas.InkPresenter.InputProcessingConfiguration.Mode = InkInputProcessingMode.Erasing;
        }

        #endregion

        //This Region Contains Everything Myo Related.
        #region MyoPoses

        /* Nothing new here, methods are explained previously and just used
           within the switch statement for Myo poses.
           The methods are all pretty self-explanatory anyways. */
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
                        eraseScribble();
                        break;
                    case Pose.WaveIn:
                        scrollPDFDown();
                        break;
                    case Pose.WaveOut:
                        scrollPDFUp();
                        break;
                    case Pose.FingersSpread:
                        startScribbling();
                        break;
                    case Pose.DoubleTap:
                        saveNote();
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

        /* You could always manually use the scrollViewer but I thought it'd
           be nice to adapt methods to do this using the Myo Armband. */
        #region Scroll Through The PDF (Adapted for Myo Armband)

        /* The amount of times the user clicks button should always begin
        at 0 on launching the App. */ 
        public static int buttonClicked = 0;
        /* Declaring this as a method outside of the btn so I can use it 
        more than once, I'm going to have to call this method again
        within the myoPoses switch statement.
        A Method to simulate scrolling down the PDF using the Myo Armband. */
        public void scrollPDFDown()
        {
            /* Keep a count on the button clicks by Incrementing the Value
            previously instantiated outside of the method.
             */
            buttonClicked++;
     
            if (buttonClicked > 0)
            {
                //Display the PDF within the stack Panel
                var transform = txtScroll.TransformToVisual(stkpnlScrollView);
                /* Start at 0 (the start of the PDF), when the user clicks
                   once go to 1500 (Page 1).
                   Then, by multiplying this by the number of times the
                   button is clicked by the user, I can keep scrolling
                   down the pages in 1500 increments to simulate scrolling
                   with the Myo Armband */
                Point absolutePosition = transform.TransformPoint(new Point(0, (1500 * buttonClicked)));
                scrollViewerPDF.ChangeView(null, absolutePosition.Y, null, false);

            }
        }

        //A Method to simulate scrolling up the PDF with the Myo Armband.
        public void scrollPDFUp()
        {
            /* Decrement the value of the user's button clicks to scroll
               back up the PDF.
             */
            buttonClicked--;
            if (buttonClicked >= 0)
            {
                var transform = txtScroll.TransformToVisual(stkpnlScrollView);
                Point absolutePosition = transform.TransformPoint(new Point(0, (1500 * buttonClicked)));
                scrollViewerPDF.ChangeView(null, absolutePosition.Y, null, false);
            }
        }

        #endregion

        //This region contains all of my Buttons used in the App
        #region Buttons (btnGoToBooksList, btnPageUp, btnPageDown btnSave, btnLoad, btnStartInking, btnClear, btnOpenLocalPdf)

        //Navigate to a List of Sample book URLs
        private void btnGoToBooksList_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Books));
        }

        /* Scrolls up the Remote PDF, although there's already a
        scrollViewer, the button makes it easier for the Myo Armband
        to scroll using a method  */
        private void btnPageUp_Click(object sender, RoutedEventArgs e)
        {
            scrollPDFUp();
        }

        // Scrolls down the PDF
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

        //Includes the saveNote method for saving Note created using inkCanvas
        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            saveNote();
        }

        //Loading in a previously saved Note
        private async void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            // Let users choose their ink file using a file picker.
            // Initialize the picker.
            Windows.Storage.Pickers.FileOpenPicker openPicker =
                new Windows.Storage.Pickers.FileOpenPicker();
            openPicker.SuggestedStartLocation =
                Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            openPicker.FileTypeFilter.Add(".gif");
            // Show the file picker.
            Windows.Storage.StorageFile file = await openPicker.PickSingleFileAsync();
            // User selects a file and picker returns a reference to the selected file.
            if (file != null)
            {
                // Open a file stream for reading.
                IRandomAccessStream stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
                // Read from file.
                using (var inputStream = stream.GetInputStreamAt(0))
                {
                    await inkCanvas.InkPresenter.StrokeContainer.LoadAsync(inputStream);
                }
                stream.Dispose();
            }
            // User selects Cancel and picker returns null.
            else
            {
                // Operation cancelled.
            }
        }

        //Start writing a note using inkCanvas
        private void btnStartInking_Click(object sender, RoutedEventArgs e)
        {
            startScribbling();
        }

        //Erase Your Note
        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            eraseScribble();
        }

        /*A Button Containing a method openLocalPDF, which will open
        a file of the user's choosing as long as they have Adobe
        Acrobat installed! UWP doesn't have too many permissions to
        access PDF files on the file system unless you specify a URI.
        */
        private void btnOpenLocalPdf_Click(object sender, RoutedEventArgs e)
        {
            openLocalPDF();
        }

        #endregion
    }
    }
