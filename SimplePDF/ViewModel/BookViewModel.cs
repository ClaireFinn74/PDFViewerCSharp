using System;

namespace SimplePdfViewer.ViewModel
{
    public class BookViewModel : NotificationBase<Data.myBooks>
    {
        public BookViewModel(Data.myBooks book = null) : base(book) { }

        public string Url
        {
            get { return This.Url; }
            set { SetProperty(This.Url, value, () => This.Url = value); }
        }

        public string Image
        {
            get { return This.Image; }
            set { SetProperty(This.Image, value, () => This.Image = value); }
        }
    }
}
