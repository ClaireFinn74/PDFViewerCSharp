using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Data.Json;
using Windows.Storage;

namespace SimplePdfViewer.Model
{
    class Books
    {
        //Make a List of books
        public List<Data.myBooks> Names { get; set; }
        public static List<Data.myBooks> gBookList = new List<Data.myBooks>();
        public String BookName { get; set; }

        public Books()
        {
            //changing this to handle async/await fix
            //LoadData();
            //Names = gBookList;
        }

        //Make an async method to load the data
        public static async Task<List<Data.myBooks>> LoadData()
        {
            //await
            await LoadLocalData();
            return gBookList;
        }

        public static async Task LoadLocalData()
        {
            var file = await Package.Current.InstalledLocation.GetFileAsync("Data\\myBooks.txt");
            var result = await FileIO.ReadTextAsync(file);

            var ZBookList = JsonArray.Parse(result);
            CreateBooksList(ZBookList);
        }

        private static void CreateBooksList(JsonArray ZBookList)
        {
            foreach (var item in ZBookList)
            {
                var oneBook = item.GetObject();
                Data.myBooks nBook = new Data.myBooks();

                foreach (var key in oneBook.Keys)
                {
                    IJsonValue value;
                    if (!oneBook.TryGetValue(key, out value))
                        continue;

                    switch (key)
                    {
                        case "Url":
                            nBook.Url = value.GetString();
                            break;
                        case "Image":
                            nBook.Image = value.GetString();
                            break;
                    } // end switch
                } // end foreach(var key in oneBook.Keys )
                gBookList.Add(nBook);
            } // end foreach (var item in jBookList)
        }
    }
}
