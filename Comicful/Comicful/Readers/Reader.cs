using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Comicful.Readers
{
    internal abstract class Reader
    {
        protected readonly StorageFile File;
        public int TotalPage;
        protected List<Stream> ImageStreams = new();
        protected List<ImageSource> LoadedImages = new();

        public enum Errors
        {
            UNSUPPORTED_FILE,
            READ_ERROR,
            OTHER_ERRORS
        }

        public Reader(StorageFile file)
        {
            this.File = file;
        }

        public virtual Task Read()
        {
            throw new NotImplementedException();
        }

        public virtual Task<ImageSource> GetPageImage(int pageIndex)
        {
            throw new NotImplementedException();
        }

        public virtual async void Dispose()
        {
            foreach (var stream in ImageStreams)
            {
                stream.Flush();
                await stream.DisposeAsync();
            }

            ImageStreams.Clear();
            LoadedImages.Clear();
        }

    }
}
