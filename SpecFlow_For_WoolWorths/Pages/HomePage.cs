/********************************************************************************
# This is a practice project from Biao Yan (Bob Yan), and it's free to be 
# downloaded for study and test project.
*********************************************************************************/

using Microsoft.Playwright;
using PlaywrightTest.pages;
using Microsoft.Extensions.Logging;


namespace SpecFlow_For_WoolWorths.Pages
{
    public class HomePage : PageBase
    {

        public HomePage(IPage page) : base(page) { }

        //The below varaibles are used for the elements in the home page
        private string homepageURL = null!;
        private string homepageLogoClassname = null!;
        private string homepageLoginMenuButton = null!;
        private string homepageLoginMenuPath = null!;
        private string homepageLoginHelloText = null!;
        private string homepageTrolleyButton = null!;
        private string homepageSpecialPriceMenuButton = null!;
        private string homepageLowPriceMenuButton = null!;

        //The below varaibles are used to define the page name in specflow
        public const string BrowsePage = "Browse Page";
        public const string SpecialPage = "Specials & offers Page";
        public const string LowpPicePage = "Low Price Page";

        /// <summary>
        /// The method is used to initialized the variables of element location
        /// </summary>
        protected override void InitLocationDefinition()
        {
            homepageURL = userSettings.GetAppParameter("homepage.url");
            homepageLogoClassname = userSettings.GetAppParameter("homepage.main_logo_location_classname");
            homepageLoginMenuButton = userSettings.GetAppParameter("homepage.login_popup_menu");
            homepageLoginMenuPath = userSettings.GetAppParameter("homepage.login_menu_path");
            homepageLoginHelloText = userSettings.GetAppParameter("homepage.login_hello_text");
            homepageTrolleyButton = userSettings.GetAppParameter("homepage.trolley_button_location");
            homepageSpecialPriceMenuButton = userSettings.GetAppParameter("homepage.special_price_menu_path");
            homepageLowPriceMenuButton = userSettings.GetAppParameter("homepage.low_price_menu_path");
        }

        /// <summary>
        /// The method is used to complete initialization process for accessing a home page
        /// </summary>
        /// <returns></returns>
        public async Task InitLogin()
        {
            await GotoHomePage();
            await Login();
            await _page.WaitForURLAsync(homepageURL);
        }

        /// <summary>
        /// The method is used to access the home url for initial access
        /// </summary>
        /// <returns>Task</returns>
        private async Task GotoHomePage()
        {
            logger.LogInformation("homepageUrl is :" + homepageURL);
            await _page.GotoAsync(homepageURL);
            await _page.WaitForURLAsync(homepageURL);
        }

        /// <summary>
        /// The method is used to complete the login process bu invoking the login page
        /// </summary>
        /// <returns></returns>
        public async Task Login()
        {
            bool isLoggined = await this.IsLogged();
            if (!isLoggined)
            {
                logger.LogInformation("Login in system");
                await ClickMenuItem(homepageLoginMenuPath);
                var loginPage = new Login(_page);
                await loginPage.LoginIn();

            }
            else
            {
                logger.LogInformation("The system has loggin in.");
            }

        }

        /// <summary>
        /// The method is used to check if the system is logined in.
        /// </summary>
        /// <returns>bool: true for login in, false for login out</returns>
        /// <exception cref="InvalidCastException"></exception>
        public async Task<bool> IsLogged()
        {
            string helloWord = homepageLoginHelloText;
            var loginPopupButton = await LocateElement(homepageLoginMenuButton);
            string? buttonValue = await loginPopupButton.TextContentAsync();
            if (buttonValue == null)
            {
                throw new InvalidCastException("Can't find the login button");
            }
            if (buttonValue.StartsWith(helloWord))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// The method is used to access the Trolley page
        /// </summary>
        /// <returns>Trolley Page object</returns>
        public async Task<TrolleyPage> GotoTrolley()
        {
            var trolleyItems = await LocateElement(homepageTrolleyButton);
            await trolleyItems.ClickAsync();
            return new TrolleyPage(_page);

        }

        /// <summary>
        /// The method is used to access Browse Page
        /// </summary>
        /// <returns>Browse Page</returns>
        public async Task<BrowsePage> GotoBrowse()
        {
            await GotoHome();
            return new BrowsePage(_page);
        }

        /// <summary>
        /// The method is used to access the Special&Offer Page
        /// </summary>
        /// <returns>Special Page</returns>
        public async Task<SpecialPricePage> GotoSpecialPage()
        {
            await ClickMenuItem(homepageSpecialPriceMenuButton);
            return new SpecialPricePage(_page);
        }

        /// <summary>
        /// The method is used to access the Low Price Page
        /// </summary>
        /// <returns></returns>
        public async Task<LowPricePage> GotoLowPricePage()
        {
            await ClickMenuItem("Low Price");
            return new LowPricePage(_page);
        }

        /// <summary>
        /// The method is used to return home page from other page
        /// </summary>
        /// <returns></returns>
        public async Task<HomePage> GotoHome()
        {
            var homeIcon = _page.Locator(homepageLogoClassname);
            await homeIcon.ClickAsync();
            return this;
        }


    }
}
