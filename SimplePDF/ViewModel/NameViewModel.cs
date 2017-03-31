using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplePdfViewer.ViewModel
{
    public class NameViewModel : NotificationBase
    {
        public List<Data.myBooks> _allBooks =
            new List<Data.myBooks>();
        Model.Books Names;

        public NameViewModel()
        {
            LoadData();
        } 

        //Handling loading in large file
        public async void LoadData()
        {

            _allBooks = await Model.Books.LoadData();
            foreach (var book in _allBooks)
            {
                var np = new BookViewModel(book);
                _book.Add(np);
            }
             
        }

        ObservableCollection<BookViewModel> _book
               = new ObservableCollection<BookViewModel>();

        public ObservableCollection<BookViewModel> book
        {
            get { return _book; }
            set { SetProperty(ref _book, value); }
        }

        int _SelectedIndex;
        public int SelectedIndex
        {
            get { return _SelectedIndex; }
            set
            {
                if (SetProperty(ref _SelectedIndex, value))
                { RaisePropertyChanged(nameof(SelectedBook)); }
            }
        }

        public BookViewModel SelectedBook
        {
            get { return (_SelectedIndex >= 0) ? _book[_SelectedIndex] : null; }
        }
    }
}
