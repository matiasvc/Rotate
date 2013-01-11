#include "TestFlight.h"

extern "C" void TestFlight_OpenFeedbackView()
{
    [TestFlight openFeedbackView];
}

extern "C" void TestFlight_PassCheckpoint(char* checkpointName)
{
    NSString *stringFromChar = [NSString stringWithCString:checkpointName encoding:NSASCIIStringEncoding];
    [TestFlight passCheckpoint:stringFromChar];
}

extern "C" void TestFlight_SubmitFeedback(char* feedbackString)
{
    NSString *stringFromChar = [NSString stringWithCString:feedbackString encoding:NSASCIIStringEncoding];
    [TestFlight submitFeedback:stringFromChar];
}

extern "C" void TestFlight_AddCustomEnvironmentInformation(char* information, char* key)
{
    NSString *stringFromCharA = [NSString stringWithCString:information encoding:NSASCIIStringEncoding];
    NSString *stringFromCharB = [NSString stringWithCString:key encoding:NSASCIIStringEncoding];
    [TestFlight addCustomEnvironmentInformation:stringFromCharA forKey:stringFromCharB];
}