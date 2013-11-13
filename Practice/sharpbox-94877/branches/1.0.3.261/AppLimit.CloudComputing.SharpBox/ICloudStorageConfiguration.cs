namespace AppLimit.CloudComputing.SharpBox
{
    /// <summary>
    /// This interface has to be implemented from storage providers to support 
    /// access configuration information. Consumers of this library has to create 
    /// an instance of a provider specific implementation to build up a connection 
    /// to the CloudStorage
    /// </summary>
    public interface ICloudStorageConfiguration
    {        
    }
}
