/********************************************************************************
# This is a practice project from Biao Yan (Bob Yan), and it's free to be 
# downloaded for study and test project.
*********************************************************************************/

using Microsoft.Playwright;

namespace PlaywrightTest.pages
{
    /// <summary>
    /// The class is used for Browse Page
    /// </summary>
    public class BrowsePage: PageBase
    {
        public BrowsePage(IPage page) : base(page) { }

        //the below variables are for the location definition in the Browse page
        private string cdxCardTag = null!;
        private string addTrolleyButton = null!;
        private string plusButton = null!;
        private string minusButton = null!;

        /// <summary>
        /// The method is used to initialize the variables of element location from user configuration file
        /// </summary>
        protected override void InitLocationDefinition()
        {
            cdxCardTag = userSettings.GetAppParameter("browse.product_section");
            addTrolleyButton = userSettings.GetAppParameter("browse.product_add_trolley_button_text");
            plusButton = userSettings.GetAppParameter("browse.product_plus_count_button");
            minusButton = userSettings.GetAppParameter("browse.product_minus_count_button");
        }

        /// <summary>
        /// The method is used to add one product item to trolley
        /// </summary>
        /// <param name="productName"></param>
        /// <returns></returns>
        public async Task AddToTrolley(string productName)
        {
            ILocator cdxCardElement = await LocateElementByChildText(productName, cdxCardTag);
            ILocator productFooterButton = cdxCardElement.GetByText(addTrolleyButton);
            await productFooterButton.ClickAsync();
        }

        /// <summary>
        /// The method is used to click the plus button for a product item.
        /// </summary>
        /// <param name="productName"></param>
        /// <returns></returns>
        public async Task AddCountForChosenItem(string productName)
        {
            ILocator cdxCardElement = await LocateElementByChildText(productName, cdxCardTag);
            ILocator productFooterButton = cdxCardElement.Locator(plusButton); ;
            await productFooterButton.ClickAsync();
        }

        /// <summary>
        /// The method is used to click the minus button for a product item
        /// </summary>
        /// <param name="productName"></param>
        /// <returns></returns>
        public async Task ReduceCountForChosenItem(string productName)
        {
            ILocator cdxCardElement = await LocateElementByChildText(productName, cdxCardTag);
            ILocator productFooterButton = cdxCardElement.Locator(minusButton); ;
            await productFooterButton.ClickAsync();
        }

        /// <summary>
        /// The method is used to select a category menu items
        /// </summary>
        /// <param name="productPath"></param>
        /// <returns></returns>
        public async Task SelectCategory(string productPath)
        {
           await ClickMenuItem(productPath);
        }

    }

}
