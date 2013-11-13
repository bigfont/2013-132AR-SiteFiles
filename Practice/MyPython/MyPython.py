# Include the Dropbox SDK
import dropbox

client = dropbox.client.DropboxClient('CHNG2PALarsAAAAAAAAAAUBLRT91LRvZsA2FlTiDaAsD6iXCR_OX7l9oZ1LvVN1k')
print('linked account: ', client.account_info())