//
//  TestingController.m
//  DropboxSDKTestApp
//
//  Created by Zed Shaw on 4/16/10.
//  Copyright 2010 Dropbox. All rights reserved.
//

#import "TestingController.h"
#import "TestingDelegate.h"
#import "DBRequest.h"
#import "DBRestClient.h"
#import "DBSession.h"

#include "TestingConfig.h"


@interface TestClientLogin : TestingDelegate
@end

@implementation TestClientLogin
- (void)restClientDidLogin:(DBRestClient*)client
{
    NSLog(@"PASSED: client login worked.");
}


- (void)restClient:(DBRestClient*)client loginFailedWithError:(NSError*)error
{
    NSLog(@"FAILED: client login failed %d %@", error.code, error.userInfo);
}
@end


@interface TestLoadMetadata : TestingDelegate
@end

@implementation TestLoadMetadata
- (void)restClientDidLogin:(DBRestClient*)client
{
    [client loadMetadata:@"/" withHash: nil];
}

- (void)restClient:(DBRestClient*)client loadedMetadata:(NSDictionary*)metadata 
{
    NSLog(@"PASSED: loaded metadata");
}
@end


@interface TestCreateFolder : TestingDelegate
@end
@implementation TestCreateFolder
- (void)restClientDidLogin:(DBRestClient*)client
{
    [client createFolder:@"/objc/testfolder"];
}

- (void)restClient:(DBRestClient*)client createdFolder:(NSDictionary*)folder 
{
    NSLog(@"PASSED: created the folder: %@", folder);
}
@end



@interface TestLoadFile : TestingDelegate
@end

@implementation TestLoadFile
- (void)restClientDidLogin:(DBRestClient*)client
{
    [client loadFile:@"/getting_started.rtf" intoPath:@"/tmp/getting_started.rtf"];
}

- (void)restClient:(DBRestClient*)client loadedFile:(NSString*)destPath 
{
    NSLog(@"PASSED: file loaded");
}
@end


@interface TestUploadFile : TestingDelegate
@end

@implementation TestUploadFile
- (void)restClientDidLogin:(DBRestClient*)client
{
    [client uploadFile:@"getting_started.rtf" toPath:@"/objc" fromPath:@"/tmp/getting_started.rtf"];
}


- (void)restClient:(DBRestClient*)client uploadedFile:(NSString*)sourcePath 
{
    NSLog(@"PASSED: file uploaded: %@", sourcePath);
}
@end



@interface TestLoadAccountInfo : TestingDelegate
@end

@implementation TestLoadAccountInfo
- (void)restClientDidLogin:(DBRestClient*)client
{
    [client loadAccountInfo];
}

- (void)restClient:(DBRestClient*)client loadedAccountInfo:(NSDictionary*)account 
{
    NSLog(@"PASSED: account info loaded");
}
@end


@interface TestLoadThumbnail : TestingDelegate
@end

@implementation TestLoadThumbnail
- (void)restClientDidLogin:(DBRestClient*)client
{
    [client loadThumbnail:@"/boston.jpg" ofSize:@"medium" intoPath:@"/tmp/boston_medium.jpg"];
    [client loadThumbnail:@"/boston.jpg" ofSize:@"small" intoPath:@"/tmp/boston_small.jpg"];
    [client loadThumbnail:@"/boston.jpg" ofSize:@"large" intoPath:@"/tmp/boston_large.jpg"];
}

- (void)restClient:(DBRestClient*)client loadedThumbnail:(NSString *)destPath
{
    NSLog(@"PASSED: thumbnails loaded: %@", destPath);
}
@end



@interface TestDeletePath : TestingDelegate
@end

@implementation TestDeletePath
- (void)restClientDidLogin:(DBRestClient*)client
{
    [client createFolder:@"/objc/testdelete"];
}

- (void)restClient:(DBRestClient*)client createdFolder:(NSString*)destPath 
{
    [client deletePath:@"/objc/testdelete"];
}

- (void)restClient:(DBRestClient*)client deletedPath:(NSString *)path
{
    NSLog(@"PASSED: path was deleted: %@", path);
}
@end


@interface TestCopyPath : TestingDelegate
@end

@implementation TestCopyPath
- (void)restClientDidLogin:(DBRestClient*)client
{
    [client uploadFile:@"boston_to_copy.jpg" toPath:@"/objc" fromPath:@"/tmp/boston.jpg"];
}

- (void)restClient:(DBRestClient*)client uploadedFile:(NSString*)sourcePath 
{
    [client deletePath:@"/objc/boston_copy.jpg"];
}

- (void)restClient:(DBRestClient*)client deletedPath:(NSString *)path
{
    [client copyFrom:@"/objc/boston_to_copy.jpg" toPath:@"/objc/boston_copy.jpg"];
}

- (void)restClient:(DBRestClient*)client deletePathFailedWithError:(NSError *)error
{
    // we ignore a failed delete since it's only done to clear things out
    [client copyFrom:@"/objc/boston_to_copy.jpg" toPath:@"/objc/boston_copy.jpg"];
}

- (void)restClient:(DBRestClient*)client copiedPath:(NSString *)from_path toPath:(NSString *)to_path
{
    NSLog(@"PASSED: copied file %@ to %@", from_path, to_path);
}
@end


@interface TestMovePath : TestingDelegate
@end

@implementation TestMovePath
- (void)restClientDidLogin:(DBRestClient*)client
{
    [client uploadFile:@"boston_to_move.jpg" toPath:@"/objc" fromPath:@"/tmp/boston.jpg"];
}

- (void)restClient:(DBRestClient*)client uploadedFile:(NSString*)sourcePath 
{
    [client deletePath:@"/objc/boston_moved.jpg"];
}

- (void)restClient:(DBRestClient*)client deletedPath:(NSString *)path
{
    [client moveFrom:@"/objc/boston_to_move.jpg" toPath:@"/objc/boston_moved.jpg"];
}

- (void)restClient:(DBRestClient*)client deletePathFailedWithError:(NSError *)error
{
    // we ignore a failed delete since it's only done to clear things out
    [client moveFrom:@"/objc/boston_to_move.jpg" toPath:@"/objc/boston_moved.jpg"];
}

- (void)restClient:(DBRestClient*)client movedPath:(NSString *)from_path toPath:(NSString *)to_path
{
    NSLog(@"PASSED: moved path %@ to %@", from_path, to_path);
}
@end


@interface TestCreateAccount : TestingDelegate
@end

@implementation TestCreateAccount

- (void)restClientDidLogin:(DBRestClient*)client
{
	// THIS IS COMMENTED OUT ON PURPOSE!  We use this to test, but you shouldn't bother.
	// Otherwise you'll fill our system with junk, and we track what people are creating and will block
	// you if you abuse this call.
	//	[client createAccount: @"testuser123456@asdfasfasdfasdfasdf.com" password: @"testing" firstName:@"Zed" lastName:@"Shaw"];
}

- (void)restClientCreatedAccount:(DBRestClient*)client
{
	NSLog(@"PASSED: account created");
}

@end

@implementation TestingController

- (void)runTest:(TestingDelegate *)delegate
{
	DBSession* session = [[DBSession alloc] initWithConsumerKey:CONSUMERKEY consumerSecret:CONSUMERSECRET];
	
	DBRestClient* client = [[DBRestClient alloc] initWithSession:session];
	client.delegate = delegate;

	delegate->session = session;

	[client loginWithEmail:USERNAME password:PASSWORD];
}



- (IBAction)runtests:(id)sender 
{
    NSLog(@"Running tests.");

    DBSession* session = [[DBSession alloc] initWithConsumerKey:CONSUMERKEY consumerSecret:CONSUMERSECRET];
    [session unlink];

    [self runTest: [TestClientLogin alloc]];
    [self runTest: [TestLoadMetadata alloc]];
    [self runTest: [TestCreateFolder alloc]];
    [self runTest: [TestLoadFile alloc]];
    [self runTest: [TestUploadFile alloc]];
	[self runTest: [TestLoadAccountInfo alloc]];
    [self runTest: [TestLoadThumbnail alloc]];
    [self runTest: [TestDeletePath alloc]];
    [self runTest: [TestCopyPath alloc]];
    [self runTest: [TestMovePath alloc]];
	
	// COMMENTED OUT ON PURPOSE
	//[self runTest: [TestCreateAccount alloc]];
}

@end
