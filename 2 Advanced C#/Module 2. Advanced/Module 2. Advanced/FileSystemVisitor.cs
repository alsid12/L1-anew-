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
        /// Field that holds StopSearch flag value.
        /// </summary>
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

        /// <summary>
        /// Delegate for filtering files and folders.
        /// </summary>
        private readonly Func<FileSystemInfo, bool> _filter;
        #endregion

        #region Events

        ///<summary>
        /// Event for starting search.
        ///</summary>
        public event EventDelegate Start;

        ///<summary>
        /// Event for finishing search.
        ///</summary>
        public event EventDelegate Finish;

        ///<summary>
        /// Event for found file.
        /// </summary>
        public event ElementFoundDelegate FileFound;

        ///<summary>
        /// Event for found folder.
        /// </summary>
        public event ElementFoundDelegate DirectoryFound;

        ///<summary>
        /// Event for filtered found file.
        /// </summary>
        public event ElementFoundDelegate FilteredFileFound;

        ///<summary>
        /// Event for filtered found folder.
        /// </summary>
        public event ElementFoundDelegate FilteredFolderFound;
        #endregion

        #region Constructors

        ///<summary>
        /// Constructor for creating FileSystemVisitor object.
        /// </summary>
        public FileSystemVisitor(string path, Func<FileSystemInfo, bool> filter = null)
        {
            _files = new List<FileSystemInfo>();
            _path = path;
            _filter = filter;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Start for files and folders based on specified conditions.
        /// </summary>
        public void Execute()
        {
            Start?.Invoke();
            foreach (var file in GetDirectoryFiles(_path))
            {
                if (_stopSearch) break;
                if (_filter == null)
                {
                    if (file.GetType() == typeof(FileInfo)) FileFound?.Invoke(file.FullName);
                    else
                    {
                        DirectoryFound?.Invoke(file.FullName);
                    }

                    if (!DeleteElementFlag) _files.Add(file);
                    DeleteElementFlag = false;
                    _stopSearch = StopSearchFlag;
                }
                else
                {
                    if (file.GetType() == typeof(FileInfo)) FilteredFileFound?.Invoke(file.FullName);
                    else FilteredFolderFound?.Invoke(file.FullName);

                    if (!DeleteElementFlag && _filter(file)) _files.Add(file);
                    DeleteElementFlag = false;
                    _stopSearch = StopSearchFlag;
                }

            }
            Finish?.Invoke();
        }

        /// <summary>
        /// Recursive file and folder search.
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

        #region IEnumerable interface implementation.
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

            public bool MoveNext() => ++index < fileSystemVisitor._files.Count;

            public FileSystemInfo Current
            {
                get
                {
                    if (index <= -1 || index >=fileSystemVisitor._files.Count)
                        throw new InvalidOperationException();
                    return fileSystemVisitor._files[index];
                }
            }

            object IEnumerator.Current => Current;

            void IEnumerator.Reset()
            {
                index = -1;
            }
        }
        #endregion

    }
}
