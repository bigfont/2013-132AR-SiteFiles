
#import "TestController.h"
#import "DBRequest.h"
#import "DBRestClient.h"
#import "DBSession.h"

NSString *USERNAME = @"your@login.com";
NSString *PASSWORD = @"yourpassword";
NSString *CONSUMERKEY = @"yourconsumerkey";
NSString *CONSUMERSECRET = @"yoursecret";

@interface TestRunner : NSObject <DBRestClientDelegate>
{
	@public
}
@end

@implementation TestRunner
- (void)restClientDidLogin:(DBRestClient*)client
{
    [client uploadFile:@"boston_to_copy.jpg" toPath:@"/objc" fromPath:@"/tmp/boston.jpg"];
}

- (void)restClient:(DBRestClient*)client loginFailedWithError:(NSError*)error
{
    NSLog(@"FAILED: client login failed %d %@", error.code, error.userInfo);
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


@implementation TestController

- (IBAction)runtests:(id)sender 
{
	DBSession* session = [[DBSession alloc] initWithConsumerKey:CONSUMERKEY consumerSecret:CONSUMERSECRET];
	[session unlink];

	DBRestClient* client = [[DBRestClient alloc] initWithSession:session];
	TestRunner *delegate = [TestRunner alloc];
	client.delegate = delegate;
    
	[client loginWithEmail:USERNAME password:PASSWORD];	
}

@end
