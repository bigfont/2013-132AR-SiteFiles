using System;

using AppLimit.CloudComputing.SharpBox;
using AppLimit.CloudComputing.SharpBox.DropBox.Objects;

namespace AppLimit.CloudComputing.SharpBox.DropBox
{
  /// <summary>
  /// This class contains the needed access credentials for a specific dropbox
  /// application sandbox and a specific end user
  /// </summary>
  public class DropBoxCredentialsToken : DropBoxBaseCredentials, ICloudeStorageCredentials
  {
    public DropBoxCredentialsToken()
    {
    }

    public DropBoxCredentialsToken(string consumerKey, string consumerSecret, string tokenKey, string tokenSecret)
    {
      AccessToken = new DropBoxToken(tokenKey, tokenSecret);
      ConsumerKey = consumerKey;
      ConsumerSecret = consumerSecret;
    }

    /// <summary>
    /// The AccessToken contains previously created access information
    /// </summary>
    public ICloudStorageAccessToken AccessToken;
  }
}
