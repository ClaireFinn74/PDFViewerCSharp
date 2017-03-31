# Claire Finn – Gesture Based UI Development 
# Hardware Used: Myo Armband 
# Software Used: Visual Studio 2015 
# Language Used: C# 
# Design Pattern: MVVM 
# Goal: ‘Book-Reader (.pdf Format)’  
 
__Purpose of the application:__

This application is a Book Reader. While the main purpose of the application is to create a book reader, I have also left the option open so that people can also load in other .PDF files, whether it be a set of presentation slides, a CV or any type of .pdf file. 
The Main Page includes all of the functionality of the app bar a few extras. 


The UI of the Main Page looks like this: 

![Main Page](https://github.com/ClaireFinn74/PDFViewerCSharp/blob/master/Images%20For%20Sharepoint%20Doc/ScreenShot1.PNG)

 
As you can see, the Main App Bar at the Top includes all of the buttons the page will need and expands so that when the user enters in text into the URL textbox, the bar, even though expanded, will not block the displayed buttons but rather, the buttons will move to an external menu via specifying them as “AppBarToggleButton”s.  

![ToggleButtons](https://github.com/ClaireFinn74/PDFViewerCSharp/blob/master/Images%20For%20Sharepoint%20Doc/ScreenShot2.PNG)

 
  
The user will enter any link into the textbox and as long as it has the extension ‘.pdf’. at the end the PDF should be loaded in handled via HTTP Client, opening a Remote stream to access the URL. The Large expanse on the side beside the App Bar is where the PDF will be loaded into. The loading in of the PDF is handled by a StackPanel to keep it in place.  
  
The User can also choose from a whole host of other options on the menu:

![ToggleButtons](https://github.com/ClaireFinn74/PDFViewerCSharp/blob/master/Images%20For%20Sharepoint%20Doc/ScreenShot3.PNG)
 
  
There’s the option to ‘Start Scribbling’, where, using ‘InkCanvas’, the user can write notes on the side of the PDF. This functionality is supposed to incorporate the idea of ‘Annotations’, but unfortunately I can only annotate the canvas on the side of the PDF as the PDF is a Bitmap Image.  

![InkCanvas](https://github.com/ClaireFinn74/PDFViewerCSharp/blob/master/Images%20For%20Sharepoint%20Doc/screenshot%206.PNG)
 
The user can pick the colour that they want to start drawing in from a Combo Box. I have only included a few basic colours: 
  
 ![ComboBox](https://github.com/ClaireFinn74/PDFViewerCSharp/blob/master/Images%20For%20Sharepoint%20Doc/screenshot5.PNG)
 
  
The User can press ‘Clear Note’ from the menu to enter erasing mode but must then press ‘Start Scribbling’ to be able to draw on the canvas again. 
The User can save this note to a file by pressing ‘Save Note’ from the menu. This is done via ‘File Picker’ and specifying ‘.GIF’ Format. 
It would also be useful to the user to be able to load their previous notes back into a previous PDF. This is also accounted for within the ‘Load Note’ Button which accesses the same folder. 

 ![LoadAndSaveNotes](https://github.com/ClaireFinn74/PDFViewerCSharp/blob/master/Images%20For%20Sharepoint%20Doc/screenshot7.PNG)

  
Another button on the Menu is a button called ‘Book Url’s that brings you to a page containing a list of URLs the user could use, leading to .pdf books on the internet. The user can pick one, copy and paste it, enter it into the textbox on the MainPage, and load in that book into the application. The Url and Images are read in via a JSON file. I originally had ‘Author, Description, and Name’ fields but due to the JSON file being a large one, the application kept crashing even when I made this process asynchronous. I had the ‘Name’ field bound to the List View so that when the user Clicks a ‘Book Url’, The Name of that book would appear. I had to replace this with the text ‘Book Url’ to try to make the JSON File shorter. I had to forego good functionality but in the end it’s the stability of the application that needs to be accounted for at all times. 

 ![LoadAndSaveNotes](https://github.com/ClaireFinn74/PDFViewerCSharp/blob/master/Images%20For%20Sharepoint%20Doc/Screenshot8.PNG) 
  
I have also included a local storage button where the user can open a PDF they have saved locally. The file will be loaded in with Adobe Acrobat locally in case of internet connection problems. 
  
The last Button on the Menu is a ‘Myo Connection’ button. This button opens up a Channel of Communication with the Myo Armband so that it can react to my application.  
 
__Gestures identified as appropriate for this application:  __

I’m using the Myo Armband to make gesture-based interactions with my application;  
   A ‘Wave-In/Wave-Out’ gesture to ‘Turn’ the pages via the scrollviewer. 
A  ‘Fingers Spread’ gesture to Start Writing a Note.  
A ‘Fist’ Gesture to ‘Erase’ a Note.  
A ‘Double Tap’ gesture to Save a Note made.  
All of these gestures work with my application in the following way: 
Wave In/Wave out: These gestures operate via moving the scrollbar.  I felt it was quite difficult to get this accurate and tailored to each PDF the user could load in. The way this works is explained in comments within the code: 
 
 
 ```
//You could always manually use the scrollViewer but I thought it'd 
          // be nice to adapt methods to do this using the Myo Armband.
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
                //Display the PDF within the stack Panel by transforming a textbox 
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
  ```
  
The user then activates this method by either pressing  the ‘Page Down’ button on the App Bar, or allowing the Myo to do it for them. The Myo clicks the button via the WaveIn gesture. The WaveOut gesture uses another method to scroll up the PDF. 
As you might gather, the fact that the Myo will be clicking this ‘ScrollDown’ button and the scrollviewer moves down in 1500 increments on the page, this isn’t too accurate for all PDFs in all sizes. Its not all bad though, allowing these gestures to be the ones to turn the pages of the PDF makes it quite effortless to scroll down the page and the two gestures are quite naturally the way a user would think about swiping through pages.  

__Fingers Spread:__ 

This gesture allows the user to ‘Start Scribbling’. It works rather accurately for other’s testing my app and for myself if you discount the added difficulty in the size of my arm. The User can benefit from not having to open up the menu every time they want to Start Scribbling again after Clearing A Note. The Fingers Spread gesture might simulate the way a user would open something up and the ‘Fist' gesture might simulate a ‘Close’ gesture so these were important considerations in the gestures I would use for Starting Scribbling and Clearing a Note.  
__Fist:__ 

This gesture is used to ‘Clear’ a note. I wanted the user to be able to access the ‘Start Scribbling’ and ‘Clear Note’ buttons easily without having to always expand the menu to do so. The fact that the user has to press ‘Start Scribbling’ again after Clearing a Note is quite cumbersome if they want to be fast. Incorporating the two buttons to work with gestures will allow the user to easily access these functionalities. As I stated above, The ‘Fist’ might simulate ‘Closing’ working with the Ink. Although I did consider swapping Fingers Spread and Fist as I felt the Fist Gesture might also simulate the way you might hold a paintbrush to ‘Start Scribbling’ eventually I settled with the described gestures above. 

__Double-Tap:__ 

I used this gesture to Save a Note by pressing the ‘Save Note’ button. Double-tap, in my opinion, looks naturally like a type of ‘save’ gesture.  
Originally I had some other ideas regarding the gestures I would use. Some considerations included a method containing “Application.Close” and the Myo would access this method from a button invisible to the user. This indeed closed the app when using the ‘Fist’ gesture with the Myo. However, the fact that I have a very small wrist which, even with the Myo clips, doesn’t fit well on my arm, resulted in the Myo Armband mistaking other gestures for the fist and the Application would close before I even got to do anything remotely interesting with it. 
I decided against this gesture for that reason. I concluded that the Myo can be useful in some other way as my application has plenty of things I could try out with it. 
Another gesture I tried out was the ‘double-tap’ gesture to click the button that loads in the URL the user entered. Unfortunately, due to the Myo seeming to need the information to be somewhat static to be able to work with it, that gesture did not work at all. 


__Hardware used in creating the application –__


   ![MyoArmBand](https://github.com/ClaireFinn74/PDFViewerCSharp/blob/master/Images%20For%20Sharepoint%20Doc/MARM.jpeg) 


I had originally considered working with the Kinect for XBox. After researching it earlier in the year I really liked the sound of what it can do for a user. After I had thought about what I wanted to actually achieve in the making of my application, I realised that if I wanted to make a PDF Reader, I needed to have a piece of hardware that was easy to use, portable and user-friendly. Portability was a huge consideration for my app. The user needed to be able to give a presentation with it or scroll through a book while sitting down for the evening. Heavy hardware with lots of wires like the Kinect did not seem to be a very good fit. I then thought about it some more and came to the conclusion that I needed to have something the user could wear, like a watch, almost hidden for maximum comfort. This is the main thing that brought me towards using the Myo Armband. I could see the usability in the fact that the armband is a snug, wrist-watch-type piece of hardware. It can allow people to give presentations easily by not having to stand over their computers. They could walk around the room (as long as they don't gesture unnecessarily!) and be open, gesturing when the page needs to be turned. It is also fit for purpose, as my main reason for making the PDF reader was to read books. The Myo can allow you to sit comfortably and barely even notice that you have it on your arm. When I then started working with the Myo, things weren’t as straightforward as that however. As I mentioned earlier, my wrist has a hard time fitting the armband. I used all of the clips and still had accuracy troubles compared to other students I had seen testing their applications. I created a Custom Profile several times but alas, the armband will only work so well with my arm. I figured a good ‘hack’ was to hold the armband down onto my arm as the electrical impulses in my arm had to be read through skin contact with the Myo. The Myo needs to establish a good connection with the electrical impulses in your arm. This sometimes worked as a fix for me. 
 
 
   ![MyoArmBand](https://github.com/ClaireFinn74/PDFViewerCSharp/blob/master/Images%20For%20Sharepoint%20Doc/MYOO.jpg) 
 
 
 
 
 
 
__Architecture for the solution__ –  
 
  ![ClassDiagram](https://github.com/ClaireFinn74/PDFViewerCSharp/blob/master/SimplePDF/Images/classDiagram.PNG) 
 
The architecture above is generated within Visual Studio 2015 by right-clicking on the solution and pressing 'view' and then 'View Class Diagram'. This is the full Class Diagram for my application. 
 
 
__Conclusions & Recommendations__ –  In conclusion, I enjoyed partaking in this module. I enjoyed working with the Myo Armband as throughout the course so far I haven't actually tried to integrate Hardware with Software. In doing so, I learned that you really need to have a knowledge of how the Hardware works in order to accurately work with it and understand what it can do for you. The Myo requires you to be able to understand the nature of how it communicates.  Using 'EMG sensors', the Myo armband measures electrical activity from your muscles to detect gestures. Using a '9-axis IMU', or rather, the 9 'sensors' you see on your armband, it also senses the motion of your forearm. The Myo armband transmits this information over a Bluetooth connection to communicate. Myo also streams this EMG and IMU data for developers like myself to use. EMG can translate signals into data that can be analysed and used. You cannot blindly start coding for the Myo without at least understanding this.

  ![MyoElecImpulse](https://github.com/ClaireFinn74/PDFViewerCSharp/blob/master/Images%20For%20Sharepoint%20Doc/MYO.jpg) 


If I were to undertake this project again I'd say I might actually try to think of an idea I could interpret for the Kinect to get a feel for how that also works. In terms of the Myo, I was very disappointed at the fact my arm couldn't properly fit causing my interactions with my own app to be inaccurate. It's quite frustrating for myself to use at times when I see other's with bigger arms use it effortlessly. I think a smaller armband might have been a consideration for Thalmic, as I am definitely not the only one my size. I also tried using Sync Fusion, a professional PDF .dll which allows you to add custom Sync Fusion Toolbar commands. I had extra functionality with that project such as "ctrl F" to find certain words and Zoom. I eventually decided against Syncfusion due to its exclusive coding style. A lot of stuff about it seemed vague and the way the code was implemented seemed as if one thing was doing lots of stuff at once without the user having knowledge of it. It was hard to get it to react properly with things I wanted to do within UWP, so it left me feeling as if it was too private/licensed/restrictive. I also thought I could use it free of charge but in the end I got lots of calls/emails about licensing, which obviously was going to be an issue for a student. I gleaned that you should always read up on licensing if you want to use a certain .dll or product. In the end I removed it from my project but it should be in previous versions of my github. Finally, I would recommend that next time I work with a piece of hardware I should try to buy it myself as I shared the Myo with two others. It was easy to share but it would have been better if I could have had more time with it myself for testing purposes. 



##References:

[Microsoft] https://msdn.microsoft.com/en-us
[Myo] https://www.myo.com/
[SyncFusion] https://www.syncfusion.com/
[StackOverFlow] https://stackoverflow.com/
[CodeProject] https://www.codeproject.com/
