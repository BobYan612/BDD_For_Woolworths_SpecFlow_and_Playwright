/********************************************************************************
# This is a practice project from Biao Yan (Bob Yan), and it's free to be 
# downloaded for study and test project.
*********************************************************************************/

using Microsoft.Playwright;
using SpecFlow_For_WoolWorths.Support;

namespace SpecFlow_For_WoolWorths.Hooks
{
    /// <summary>
    /// The method is used to define the hooks for executing some progress before every senario
    /// </summary>
    [Binding]
    public class Hooks
    {
        //The property is used for page objects
        public IPage User { get; private set; } = null!;

        private readonly string Chromium = "Chromium";
        private readonly string Firefox = "Firefox";
        private readonly string Webkit = "Webkit";

        /// <summary>
        /// The method is used to create the IPage object and define the launch options
        /// </summary>
        /// <returns>Task</returns>
        [BeforeScenario] 
        public async Task InitializeIPageObject()
        {
            var userSetting = UserConfiguration.Instance;

            //Use this option to be able to see your test running
            string useHeadless = userSetting.GetAppParameter("test_with_headless");

            //Define the browse type
            string browseType = userSetting.GetAppParameter("browse_type");
            IBrowser browser;

            var playwright = await Playwright.CreateAsync();
            //Initialise a browser - 'Chromium' can be changed to 'Firefox' or 'Webkit'

            if (browseType == Chromium)
            {
                browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                {
                    Headless = bool.Parse(useHeadless) // -> Use this option to be able to see your test running
                });
            }
            else if (browseType == Firefox)
            {
                browser = await playwright.Firefox.LaunchAsync(new BrowserTypeLaunchOptions
                {
                    Headless = bool.Parse(useHeadless) // -> Use this option to be able to see your test running
                });
            }
            else if (browseType == Webkit)
            {
                browser = await playwright.Webkit.LaunchAsync(new BrowserTypeLaunchOptions
                {
                    Headless = bool.Parse(useHeadless) // -> Use this option to be able to see your test running
                });
            } else
            {
                throw new NotSupportedException($"Can't support the browse type:{browseType}");
            }

            //Setup a browser context
            var context1 = await browser.NewContextAsync();
            var timeoutDefine = UserConfiguration.Instance.GetAppParameter("searching_timeout");
            if (timeoutDefine.Trim().Length > 0)
            {
                int timeout = int.Parse(timeoutDefine);
                context1.SetDefaultTimeout(timeout);
            }

            //Initialise a page on the browser context.
            User = await context1.NewPageAsync();
        }

        /// <summary>
        /// clean some resource after scenario
        /// </summary>
        /// <returns></returns>
        [AfterScenario]
        public async Task DestroyResource()
        {
            if (User != null)
            {
                await User.CloseAsync();
            }
        }

    }
}
