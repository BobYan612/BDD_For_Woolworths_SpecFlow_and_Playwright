## BDD Test for Woolworths

**The Project is one example to use (Specflow + Playwright) to create a BDD test framework**

**Features**  
There are the below considerations during implementation:
 - Decouple the location definition from code to one configuration file ([elementlocations.json](SpecFlow_For_WoolWorths/elementlocations.json))
 - Specify the test options in one configuration file ([usersettings.json](SpecFlow_For_WoolWorths/usersettings.json)) which may easly change the Chromium/Firefox/Webkit and credential parameters.
 - Implement one singleton class([UserConfiguration](SpecFlow_For_WoolWorths/Support/UserConfiguration.cs)) to read the user configuration file
 - Design homepage as one factory pattern which may create other page objects, so that the outside invokers may easily obtain the other page object.
 - Use ILogger to control the output of the test log

**Dependency**  
SpecFlow  
SpecFlow.Plus.LivingDocPlugin  
SpecFlow.NUnit  
Microsoft.Playwright  
Microsoft.Playwright.NUnit  
Microsoft.Extensions.Logging  
Microsoft.Extensions.Configuration 

>[!NOTE]  
>The executable binary such as chrome.exe will be installed automatically when installing Playwright, or we need to download the related version from playwright official webserver. 


**Test**  
The project has implememnted and verified 5 test scenarios beased on the Trolley feature (see [trolley.feature](SpecFlow_For_WoolWorths/Features/trolley.feature) in Features folder)
 - Scenario1: _Choose a product from browse to trolley_
 - Scenario2: _Choose a product from speical & offers to trolley parameters._
 - Scenario3: _Choose a product from low price to trolley_
 - Scenario4: _Update the quantity in trolley_
 - Scenario5: _Remove one item in the trolley_  

> [!TIP]  
> Any suggestions or questions please contact biao.yan612@gmail.com



