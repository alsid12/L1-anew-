using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task1
{
    public class FileSystemVisitor: IEnumerable
    {
        #region Fields and properties
        /// <summary>
        /// List of found files and folders
        /// </summary>
        private List<FileSystemInfo> _files;
        /// <summary>
        /// Path to directory from which start searching for files and folders.
        /// </summary>
        private readonly string _path;
        /// <summary>
        /// Delegate for filtering files and folders.
        /// </summary>
        private readonly Func<FileSystemInfo, bool> _filter;
        private bool _stopSearch;

        /// <summary>
        /// Flag to stop searching files and folders.
        /// </summary>
        public bool StopSearchFlag { get; set; }
        /// <summary>
        /// Flag to exclude file or folder.
        /// </summary>
        public bool DeleteElementFlag { get; set; }


        #endregion

        #region Delegates
        public delegate void EventDelegate();
        public delegate void ElementFoundDelegate(string elementFullName);
        #endregion

        #region Events
        ///<summary>
        /// Event for starting search.
        ///</summary>
        public event EventDelegate 
        #endregion


        #region Implementation of IEnumerable
        /// <summary>
        /// Search for files and folders in chosen folder.
        /// </summary>
        /// <param name="path">Folder where to search for files and folders</param>
        /// <result>FileInfo and DirectoryInfo objects found for current folder</result>

        private IEnumerable<FileSystemInfo> GetDirectoryFiles(string path)
        {
            DirectoryInfo directory = new DirectoryInfo(path);

            foreach (var file in directory.GetFiles())
            {
                yield return file;
            }

            foreach (DirectoryInfo dirInfo in directory.GetDirectories())
            {
                yield return dirInfo;

                foreach (var file in GetDirectoryFiles(dirInfo.FullName))
                {
                    yield return file;
                }
            }
        }

        #endregion

        /// <summary>
        /// Implementation of IEnumerable interface
        /// </summary>
        private FileSystemVisitorEnumerator GetEnumerator() => new FileSystemVisitorEnumerator(this);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private class FileSystemVisitorEnumerator : IEnumerator
        {
            private readonly FileSystemVisitor fileSystemVisitor;
            private int index;

            public FileSystemVisitorEnumerator(FileSystemVisitor visitor)
            {
                fileSystemVisitor = visitor;
                index = -1;
            }

            public bool MoveNext() => ++index < fileSystemVisitor._file
        }
    }
}
