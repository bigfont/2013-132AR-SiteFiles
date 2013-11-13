//
//  TestingDelegate.m
//  DropboxSDKTestApp
//
//  Created by Zed Shaw on 4/19/10.
//  Copyright 2010 Dropbox. All rights reserved.
//

#import "TestingDelegate.h"
#include <assert.h>


@implementation TestingDelegate

- (void)restClientDidLogin:(DBRestClient*)client
{
    NSLog(@"ERROR testing delegate method called that shouldn't be: %s", __FUNCTION__);;
}

- (void)restClient:(DBRestClient*)client loginFailedWithError:(NSError*)error
{
    NSLog(@"ERROR testing delegate method called that shouldn't be: %s", __FUNCTION__);;
}

- (void)restClient:(DBRestClient*)client loadedMetadata:(NSDictionary*)metadata 
{
    NSLog(@"ERROR testing delegate method called that shouldn't be: %s", __FUNCTION__);
}

- (void)restClient:(DBRestClient*)client loadedAccountInfo:(NSDictionary*)account 
{
    NSLog(@"ERROR testing delegate method called that shouldn't be: %s", __FUNCTION__);
}

- (void)restClient:(DBRestClient*)client loadedFile:(NSString*)destPath 
{
    NSLog(@"ERROR testing delegate method called that shouldn't be: %s", __FUNCTION__);
}


- (void)restClient:(DBRestClient*)client uploadedFile:(NSString*)sourcePath 
{
    NSLog(@"ERROR testing delegate method called that shouldn't be: %s", __FUNCTION__);
}


- (void)restClient:(DBRestClient*)client createdFolder:(NSDictionary*)folder 
{
    NSLog(@"ERROR testing delegate method called that shouldn't be: %s", __FUNCTION__);
}

- (void)restClient:(DBRestClient*)client deletedPath:(NSString *)path
{
    NSLog(@"ERROR testing delegate method called that shouldn't be: %s", __FUNCTION__);
}

- (void)restClient:(DBRestClient*)client loadedThumbnail:(NSString *)destPath
{
    NSLog(@"ERROR testing delegate method called that shouldn't be: %s", __FUNCTION__);
}

- (void)restClient:(DBRestClient*)client copiedPath:(NSString *)from_path toPath:(NSString *)to_path
{
    NSLog(@"ERROR testing delegate method called that shouldn't be: %s", __FUNCTION__);
}

- (void)restClient:(DBRestClient*)client movedPath:(NSString *)from_path toPath:(NSString *)to_path
{
    NSLog(@"ERROR testing delegate method called that shouldn't be: %s", __FUNCTION__);
}

- (void)restClientCreatedAccount:(DBRestClient*)client;
{
    NSLog(@"ERROR testing delegate method called that shouldn't be: %s", __FUNCTION__);	
}

- (void)restClient:(DBRestClient*)client loadMetadataFailedWithError:(NSError*)error
{
    NSLog(@"ERROR unexpected error %s: %@", __FUNCTION__, error.userInfo);
}

- (void)restClient:(DBRestClient*)client loadAccountInfoFailedWithError:(NSError*)error
{
    NSLog(@"ERROR unexpected error %s: %@", __FUNCTION__, error.userInfo);
}

- (void)restClient:(DBRestClient*)client loadFileFailedWithError:(NSError*)error
{
    NSLog(@"ERROR unexpected error %s: %@", __FUNCTION__, error.userInfo);
}

- (void)restClient:(DBRestClient*)client loadThumbnailFailedWithError:(NSError*)error
{
    NSLog(@"ERROR unexpected error %s: %@", __FUNCTION__, error.userInfo);
}

- (void)restClient:(DBRestClient*)client uploadFileFailedWithError:(NSError*)error
{
    NSLog(@"ERROR unexpected error %s: %@", __FUNCTION__, error.userInfo);
}

- (void)restClient:(DBRestClient*)client createFolderFailedWithError:(NSError*)error
{
    NSLog(@"ERROR unexpected error %s: %@", __FUNCTION__, error.userInfo);

}

- (void)restClient:(DBRestClient*)client deletePathFailedWithError:(NSError*)error
{
    NSLog(@"ERROR unexpected error %s: %@", __FUNCTION__, error.userInfo);

}

- (void)restClient:(DBRestClient*)client copyPathFailedWithError:(NSError*)error
{
    NSLog(@"ERROR unexpected error %s: %@", __FUNCTION__, error.userInfo);

}

- (void)restClient:(DBRestClient*)client movePathFailedWithError:(NSError*)error
{
    NSLog(@"ERROR unexpected error %s: %@", __FUNCTION__, error.userInfo);
}

- (void)restClient:(DBRestClient*)client createAccountFailedWithError:(NSError*)error
{
    NSLog(@"ERROR unexpected error %s: %@", __FUNCTION__, error.userInfo);
}

@end



