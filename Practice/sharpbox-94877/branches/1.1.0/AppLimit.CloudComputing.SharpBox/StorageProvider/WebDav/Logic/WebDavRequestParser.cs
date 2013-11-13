using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppLimit.CloudComputing.SharpBox.StorageProvider.BaseObjects;
using System.IO;
using System.Xml;
using System.Globalization;
using System.Diagnostics;
using AppLimit.CloudComputing.SharpBox.StorageProvider.API;
using AppLimit.CloudComputing.SharpBox.Common.IO;
using AppLimit.CloudComputing.SharpBox.Common.Net;

#if SILVERLIGHT || ANDROID
using System.Net;
#else
using System.Web;
using System.Net;
#endif

namespace AppLimit.CloudComputing.SharpBox.StorageProvider.WebDav.Logic
{
    internal class WebDavRequestParser
    {
        public static List<BaseFileEntry> CreateObjectsFromNetworkStream(Stream data, IStorageProviderService service, IStorageProviderSession session)
        {
            WebDavConfiguration config = session.ServiceConfiguration as WebDavConfiguration;

            List<BaseFileEntry> fileEntries = new List<BaseFileEntry>();

            try
            {
                String resourceName = String.Empty;
                long resourceLength = 0; 
                DateTime resourceModificationDate = DateTime.Now;
                DateTime resourceCreationDate = DateTime.Now;
                Boolean bIsHidden = false;
                Boolean bIsDirectory = false;
                String davNameSpaceTag = string.Empty;

                XmlTextReader reader = new XmlTextReader(data);    
                while (reader.Read())    
                {
                    switch(reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            {                                
                                // we are on an element and we have to handle this elements
                                String currentElement = reader.Name.ToLower();

                                // build the namespace tag
                                if (davNameSpaceTag == String.Empty && currentElement.Contains("multistatus"))
                                {
                                    String[] fields = currentElement.Split(':');
                                    if (fields.Length > 1)
                                        davNameSpaceTag = fields[0];
                                }

                                // we found a resource name
                                if (currentElement.Equals(davNameSpaceTag + ":href"))
                                {
                                    // go one more step
                                    reader.Read();
                                    
                                    // get the name
                                    String nameBase = reader.Value;

                                    // remove the base url
                                    if ( nameBase.StartsWith(config.ServiceLocator.ToString()))
                                        nameBase = nameBase.Remove(0, config.ServiceLocator.ToString().Length);

                                    // trim all trailing slashes
                                    nameBase = nameBase.TrimEnd('/');
                                    
                                    // get the last file or directory name
                                    PathHelper ph = new PathHelper(nameBase);
                                    resourceName = ph.GetFileName();

                                    // unquote name 
                                    resourceName = HttpUtility.UrlDecode(resourceName);                                    
                                } 
                                else if (currentElement.Equals(davNameSpaceTag + ":ishidden"))
                                {
                                    // go one more step
                                    reader.Read();

                                    // try to parse
                                    try
                                    {
                                        bIsHidden = Convert.ToBoolean(int.Parse(reader.Value));
                                    }
                                    catch (Exception)
                                    {
                                        bIsHidden = false;
                                    }
                                }
                                else if (currentElement.Equals(davNameSpaceTag + ":getcontentlength"))
                                {
                                    // go one more step
                                    reader.Read();

                                    // read value
                                    resourceLength = long.Parse(reader.Value);                                 
                                }
                                else if (currentElement.Equals(davNameSpaceTag + ":creationdate"))                             
                                {
                                    // go one more step
                                    reader.Read();

                                    // parse
                                    resourceCreationDate = DateTime.Parse(reader.Value, CultureInfo.CurrentCulture);                                 
                                }
                                else if (currentElement.Equals(davNameSpaceTag + ":getlastmodified"))
                                {
                                    // go one more step
                                    reader.Read();

                                    // parse
                                    resourceModificationDate = DateTime.Parse(reader.Value, CultureInfo.CurrentCulture);                                 
                                }
                                else if (currentElement.Equals(davNameSpaceTag + ":collection"))                                
                                {
                                    // set as directory
                                    bIsDirectory = true;                                 
                                }                                
                                
                                // go ahead
                                break;
                            }

                        case XmlNodeType.EndElement:
                            {
                                // handle the end of an response
                                if (!reader.Name.ToLower().Equals(davNameSpaceTag + ":response"))
                                    break;

                                // handle the end of an response, this means
                                // create entry
                                BaseFileEntry entry = null;

                                if (bIsDirectory)
                                    entry = new BaseDirectoryEntry(resourceName, resourceLength, resourceModificationDate, service, session);
                                else
                                    entry = new BaseFileEntry(resourceName, resourceLength, resourceModificationDate, service, session);

                                entry.SetPropertyValue("CreationDate", resourceCreationDate);
						
								if (!bIsHidden)
                                	fileEntries.Add(entry);

                                // reset all state properties
                                resourceName = String.Empty;
                                resourceLength = 0;
                                resourceModificationDate = DateTime.Now;
                                resourceCreationDate = DateTime.Now;
                                bIsHidden = false;
                                bIsDirectory = false;

                                // go ahead
                                break;
                            }
                        default:
                            {
                                
                                break;
                            }
                    }                    
                };                                   
            }
            catch (Exception)
            {                                
            }

            return fileEntries;
        }
    }
}
