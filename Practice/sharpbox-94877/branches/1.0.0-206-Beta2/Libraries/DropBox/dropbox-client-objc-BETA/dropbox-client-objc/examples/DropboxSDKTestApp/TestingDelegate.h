//
//  TestingDelegate.h
//  DropboxSDKTestApp
//
//  Created by Zed Shaw on 4/19/10.
//  Copyright 2010 Dropbox. All rights reserved.
//

#import <Cocoa/Cocoa.h>
#import "DBRestClient.h"
#import "DBSession.h"

@interface TestingDelegate : NSObject <DBRestClientDelegate>
{
	@public
        DBSession *session;
}
@end


