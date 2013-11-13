//
//  TestingController.h
//  DropboxSDKTestApp
//
//  Created by Zed Shaw on 4/16/10.
//  Copyright 2010 Dropbox. All rights reserved.
//

#import <Cocoa/Cocoa.h>


@interface TestingController : NSObject {
	IBOutlet NSTextField *passFail;
}

- (IBAction)runtests:(id)sender;	
@end
