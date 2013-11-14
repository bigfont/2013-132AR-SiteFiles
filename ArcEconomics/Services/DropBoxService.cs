using ArcEconomics.Models;
using AspDropBox.Core;
using AspDropBox.Core.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace ArcEconomics.Services
{
    public class DropBoxService
    {
        private DropBoxCoreApi coreApi;
        public DropBoxService()
        {
            coreApi = GetCoreApi();
        }
        private DropBoxCoreApi GetCoreApi()
        {

            DropBoxCoreApi api;
            api = new DropBoxCoreApi(ConfigurationManager.AppSettings["DropBoxBearerCode"]);
            return api;

        }
        public DropBoxDirectory[] GetDirectoriesFromMetadata(Metadata metadata)
        {
            DropBoxDirectory[] dirs;

            dirs = metadata.Contents
                .Where<Metadata>(m => m.Is_Dir)
                .Select<Metadata, DropBoxDirectory>((m, d) => new DropBoxDirectory() { Path = m.Path, ChildDirectories = null })
                .ToArray<DropBoxDirectory>();

            return dirs;
        }
        private DropBoxFile[] GetFilesFromMetadata(Metadata metadata)
        {
            DropBoxFile[] files;

            files = metadata.Contents
                .Where<Metadata>(m => !m.Is_Dir)
                .Select<Metadata, DropBoxFile>((m, d) => new DropBoxFile() { Path = m.Path, PublicUrl = coreApi.GetShare(RootType.Sandbox, m.Path).Url })
                .ToArray<DropBoxFile>();

            return files;
        }
        public DropBoxDirectory GetRootDirectory()
        {
            return GetDirectoryByName(String.Empty);
        }
        public DropBoxDirectory GetDirectoryByName(string directoryName)
        {
            Metadata metadata;
            DropBoxDirectory dir;

            metadata = coreApi.GetMetadata(RootType.Sandbox, directoryName);

            // instantiate the current dir
            dir = new DropBoxDirectory();
            dir.Path = metadata.Path;
            dir.ChildDirectories = this.GetDirectoriesFromMetadata(metadata);
            dir.ChildFiles = this.GetFilesFromMetadata(metadata);

            return dir;
        }
    }
}