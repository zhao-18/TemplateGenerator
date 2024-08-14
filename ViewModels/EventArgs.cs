using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplateGenerator.ViewModels
{
    public class NavigateNextEventArgs : EventArgs
    {
        public int pageNumber { get; set; }

        public NavigateNextEventArgs(int pageNumber)
        {
            this.pageNumber = pageNumber;
        }
    }

    public class FolderPickedEventArgs : EventArgs
    {
        public IStorageFolder storageFolder { get; set; }

        public FolderPickedEventArgs (IStorageFolder storageFolder)
        {
            this.storageFolder = storageFolder;
        }
    }

    public class MultiFolderPickedEventArgs : EventArgs
    {
        public List<IStorageFolder> Entries { get; set; }

        public MultiFolderPickedEventArgs(List<IStorageFolder> entries)
        {
            Entries = entries;
        }
    }

    public class MultiFilePickedEventArgs : EventArgs
    {
        public List<IStorageFile> Entries { get; set; }

        public MultiFilePickedEventArgs(List<IStorageFile> entries)
        {
            Entries = entries;
        }
    }
}
