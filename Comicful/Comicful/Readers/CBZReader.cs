using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Comicful.Readers
{
    internal class CBZReader : Reader
    {
        public CBZReader(StorageFile file) : base(file)
        {
        }

        public override Task Read()
        {
            return Task.Run(async () =>
            {
                var content = ZipFile.OpenRead(File.Path);
                var imageEntries = content.Entries.Where(item =>
                    item.Name.EndsWith(".jpg", StringComparison.InvariantCultureIgnoreCase) ||
                    item.Name.EndsWith(".jpeg", StringComparison.InvariantCultureIgnoreCase) ||
                    item.Name.EndsWith(".png", StringComparison.InvariantCultureIgnoreCase)
                );
                TotalPage = imageEntries.Count();
                foreach (var imageEntry in imageEntries)
                {
                    var imageStream = imageEntry.Open();
                    var memStream = new MemoryStream();
                    await imageStream.CopyToAsync(memStream);
                    memStream.Position = 0;
                    ImageStreams.Add(memStream);
                    await imageStream.DisposeAsync();
                }
            });
        }

        public override async Task<ImageSource> GetPageImage(int pageIndex)
        {
            if (pageIndex > LoadedImages.Count - 1)
            {
                var image = new BitmapImage();
                var imageStream = ImageStreams[pageIndex];
                var stream = imageStream.AsRandomAccessStream();
                imageStream.Position = 0;
                await image.SetSourceAsync(stream);
                LoadedImages.Add(image);
                return image;
            }
            return LoadedImages[pageIndex];

        }
    }
}
