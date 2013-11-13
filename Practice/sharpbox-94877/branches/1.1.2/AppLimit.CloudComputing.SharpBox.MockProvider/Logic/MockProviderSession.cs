using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppLimit.CloudComputing.SharpBox.StorageProvider.API;
using AppLimit.CloudComputing.SharpBox.MockProvider.Model;
using System.IO;

namespace AppLimit.CloudComputing.SharpBox.MockProvider.Logic
{
    /// <summary>
    /// This class implements the behaviour of an authenticted session 
    /// and allows the logic code to get access to the session token, 
    /// the associated service and the service configuration
    /// 
    /// The MockSessionProvider is reponsable to hold our virtual filesystem
    /// which based on the internal object model of SharpBox
    /// 
    /// - Virtual FileSystem is /
    ///     - Data
    ///         - TestFolder1
    ///         - TestFolder2
    ///     - Data2
    ///         - TestFolder3
    ///             - File.rnd
    /// </summary>
    internal class MockProviderSession : IStorageProviderSession
    {
        private ICloudStorageAccessToken _Token;
        private IStorageProviderService _Service;
        private ICloudStorageConfiguration _Config;

        private ICloudDirectoryEntry _Root;
        private Dictionary<ICloudFileSystemEntry, MemoryStream> _DataDictionary = new Dictionary<ICloudFileSystemEntry, MemoryStream>();

        /// <summary>
        /// The ctor gets all security information and generates the starting point 
        /// of out virtual filesystem.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="service"></param>
        /// <param name="config"></param>
        public MockProviderSession(MockProviderCredentials token, MockProviderService service, MockPoviderConfiguration config)
        {
            // get the parameter
            _Token = token;
            _Service = service;
            _Config = config;

            // build the virtual in memory filesystem 
            _Root = GenericStorageProviderFactory.CreateDirectoryEntry(this, "/", null);
            ICloudDirectoryEntry data = GenericStorageProviderFactory.CreateDirectoryEntry(this, "Data", _Root);
            GenericStorageProviderFactory.CreateDirectoryEntry(this, "TestFolder1", data);
            GenericStorageProviderFactory.CreateDirectoryEntry(this, "TestFolder2", data);
            ICloudDirectoryEntry data2 = GenericStorageProviderFactory.CreateDirectoryEntry(this, "Data2", _Root);
            ICloudDirectoryEntry TestFolder3 = GenericStorageProviderFactory.CreateDirectoryEntry(this, "TestFolder3", data2);
            GenericStorageProviderFactory.CreateFileSystemEntry(this, "File.rnd", TestFolder3);            

            // add a couple data for unittests as well
            ICloudDirectoryEntry nunit = GenericStorageProviderFactory.CreateDirectoryEntry(this, "NUnitIntegrationTests", _Root);
            ICloudDirectoryEntry srvdata = GenericStorageProviderFactory.CreateDirectoryEntry(this, "ServerData", nunit);
            GenericStorageProviderFactory.CreateFileSystemEntry(this, "te'st.txt", srvdata);            
            GenericStorageProviderFactory.CreateFileSystemEntry(this, "hhw,,.gif", srvdata);
            ICloudDirectoryEntry f1 = GenericStorageProviderFactory.CreateDirectoryEntry(this, "1", srvdata);
            ICloudDirectoryEntry f2 = GenericStorageProviderFactory.CreateDirectoryEntry(this, "2", f1);
            ICloudDirectoryEntry f3 = GenericStorageProviderFactory.CreateDirectoryEntry(this, "3", f2);
            ICloudDirectoryEntry f4 = GenericStorageProviderFactory.CreateDirectoryEntry(this, "4", f3);
            GenericStorageProviderFactory.CreateDirectoryEntry(this, "5", f4);
        }

        /// <summary>
        /// Our session hold all the information about the virtual filesystem. We have to 
        /// give our service implementation an access gateway to the virtual root of the 
        /// filesystem. 
        /// </summary>
        /// <returns></returns>
        public ICloudDirectoryEntry GetVirtualRoot()
        {
            return _Root;
        }

        /// <summary>
        /// The Mock Session holds all uploaded data also in the memory, so
        /// the access to the data streams are realized through this method
        /// </summary>
        /// <param name="fsEntry"></param>
        /// <returns></returns>
        public Stream GetDataStream(ICloudFileSystemEntry fsEntry)
        {
            // declare ret value
            MemoryStream dataStream = null;

            // check or create the data stream
            if (_DataDictionary.ContainsKey(fsEntry))
                dataStream = _DataDictionary[fsEntry];
            else
            {
                // create the memory stream
                dataStream = new MemoryStream();
                
                _DataDictionary.Add(fsEntry, dataStream);
            }

            // reset stream position
            dataStream.Position = 0;

            // go ahead
            return dataStream;              
        }

        #region IStorageProviderSession Members

        public ICloudStorageAccessToken SessionToken
        {
            get { return _Token; }
        }

        public IStorageProviderService Service
        {
            get { return _Service; }
        }

        public ICloudStorageConfiguration ServiceConfiguration
        {
            get { return _Config; }
        }

        #endregion
    }
}
