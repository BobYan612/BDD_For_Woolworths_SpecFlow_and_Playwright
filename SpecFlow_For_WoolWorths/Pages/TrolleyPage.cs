/********************************************************************************
# This is a practice project from Biao Yan (Bob Yan), and it's free to be 
# downloaded for study and test project.
*********************************************************************************/

using Microsoft.Playwright;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace PlaywrightTest.pages
{
    /// <summary>
    /// The class is used for Trolley Page
    /// </summary>
    public class TrolleyPage : PageBase
    {
        //It's used to define the default waiting time for listing products in Trolley
        private const long defaultTimeoutInSeconds = 30;
        public TrolleyPage(IPage page) : base(page) { }

        //The below are the location definition for all the elements in trolley page
        private string trolleyProductSectionTag = null!;
        private string trolleyProductPlusButton = null!;
        private string trolleyProductMinusButton = null!;
        private string trolleyProductRemoveButton = null!;
        private string trolleyProductClearButton = null!;
        private string trolleyProductClearConfirmButton = null!;
        private string trolleyProductNameLoc = null!;
        private string trolleyProductQuantityInput = null!;
        private string trolleyProductSavingLoc = null!;
        private string trolleyProductSubTotalPriceText = null!;
        private string trolleyProductSubTotalPriceTag = null!;
        private string trolleyProductSingleUnitPriceLoc = null!;
        private string trolleyProductPriceLoc = null!;
        private string trolleyProductTotalPriceLoc = null!;
        private string trolleyProductTotalPriceClassname = null!;
        private string trolleyProductChooseCheckBox = null!;
        private string trolleyProductQuantityConfirmButton = null!;

        /// <summary>
        /// The method is used to read the definition from user configuration file.
        /// </summary>
        protected override void InitLocationDefinition()
        {
            trolleyProductSectionTag = userSettings.GetAppParameter("trolleypage.product_section");
            trolleyProductPlusButton = userSettings.GetAppParameter("trolleypage.product_plus_count_button");
            trolleyProductMinusButton = userSettings.GetAppParameter("trolleypage.product_minus_count_button");
            trolleyProductRemoveButton = userSettings.GetAppParameter("trolleypage.product_remove_button");
            trolleyProductClearButton = userSettings.GetAppParameter("trolleypage.product_clear_button_text");
            trolleyProductClearConfirmButton = userSettings.GetAppParameter("trolleypage.product_clear_confirm_button_text");
            trolleyProductNameLoc = userSettings.GetAppParameter("trolleypage.product_name_location");
            trolleyProductQuantityInput = userSettings.GetAppParameter("trolleypage.product_quantity_input");
            trolleyProductSavingLoc = userSettings.GetAppParameter("trolleypage.product_saving_classname");
            trolleyProductSubTotalPriceText = userSettings.GetAppParameter("trolleypage.product_single_total_price_text");
            trolleyProductSubTotalPriceTag = userSettings.GetAppParameter("trolleypage.product_single_total_price_tagname");
            trolleyProductSingleUnitPriceLoc = userSettings.GetAppParameter("trolleypage.product_single_unit_average_location");
            trolleyProductPriceLoc = userSettings.GetAppParameter("trolleypage.product_price_location");
            trolleyProductTotalPriceLoc = userSettings.GetAppParameter("trolleypage.product_total_price");
            trolleyProductTotalPriceClassname = userSettings.GetAppParameter("trolleypage.product_total_price_classname");
            trolleyProductChooseCheckBox = userSettings.GetAppParameter("trolleypage.product_choose_checkbox");
            trolleyProductQuantityConfirmButton = userSettings.GetAppParameter("trolleypage.product_add_confirm_button_text");
        }

        /// <summary>
        /// The method is used to check if a product has been shown in the trolley list
        /// </summary>
        /// <param name="productName">The product name</param>
        /// <returns>Task</returns>
        public async Task HasProductItem(string productName)
        {
            ILocator cdxCardElement = await LocateElementByChildText(productName, trolleyProductSectionTag);
            Assert.IsNotNull(cdxCardElement);
        }

        /// <summary>
        /// The method is used to click the plus button to add the count of one product item in the trolley.
        /// </summary>
        /// <param name="productName">Product name</param>
        /// <returns>Task</returns>
        public async Task AddCountForChosenItem(string productName)
        {
            ILocator cdxCardElement = await LocateElementByChildText(productName, trolleyProductSectionTag);
            ILocator productFooterButton = cdxCardElement.Locator(trolleyProductPlusButton); ;
            await productFooterButton.ClickAsync();
        }

        /// <summary>
        /// The method is used to click the minus button to reduce the count of one product item in the trolley.
        /// </summary>
        /// <param name="productName">Product name</param>
        /// <returns>Task</returns>
        public async Task ReduceCountForChosenItem(string productName)
        {
            ILocator cdxCardElement = await LocateElementByChildText(productName, trolleyProductSectionTag);
            ILocator productFooterButton = cdxCardElement.Locator(trolleyProductMinusButton); ;
            await productFooterButton.ClickAsync();
        }

        /// <summary>
        /// The method is used to click the plus button to remove the product item.
        /// </summary>
        /// <param name="productName">product name</param>
        /// <returns>Task</returns>
        public async Task RemoveChosenItem(string productName)
        {
            ILocator cdxCardElement = await LocateElementByChildText(productName, trolleyProductSectionTag);
            ILocator productFooterButton = await LocateElementFromParent(cdxCardElement, trolleyProductRemoveButton, SearchType.Text);
            await productFooterButton.ClickAsync();
        }

        /// <summary>
        /// The method is sued to clear the trolley items
        /// </summary>
        /// <returns>Task</returns>
        public async Task ClearTrolley()
        {
            ILocator clearButton = await LocateElement(trolleyProductClearButton, SearchType.Text);;
            await clearButton.ClickAsync();
            Thread.Sleep(1000);
            ILocator confirmButton = await LocateElement(trolleyProductClearConfirmButton, SearchType.Text);
            await confirmButton.ClickAsync();
        }

        /// <summary>
        /// The method is used to fetch all items in the trolley and return a dictionary object.
        /// </summary>
        /// <returns>Dictionay of the item information
        /// For Example:
        ///{
        ///  "product_name" : { "each_price" : 1,
        ///                     "quantity" :1,
        ///                     "savings" : 0,
        ///                     "total_price" : 1,
        ///                     "is_chosen" :1    // 1 means checked in, 0 means checked out
        ///                   }  
        /// }
        /// </returns>
        public async Task<Dictionary<string, Dictionary<string, float>>> GetTrolleyItems() 
        {
            var allItems = await GetAllProductItems(TimeSpan.FromSeconds(defaultTimeoutInSeconds));
            Dictionary<string, Dictionary<string, float>> itemInfo = new Dictionary<string, Dictionary<string, float>> { };
            foreach (var item in allItems)
            {
                ILocator labelLocator = await LocateElementFromParent(item, trolleyProductNameLoc);
                string itemName = await GetTextFromElement(labelLocator);
                ILocator priceLocator = await GetEachPriceLocator(item);
                string itemEachPrice = await GetTextFromElement(priceLocator);
                ILocator quantityLocator = await LocateElementFromParent(item, trolleyProductQuantityInput);
                string itemQantity = await quantityLocator.InputValueAsync();
                ILocator itemSavingLocator = (await LocateElementFromParent(item, trolleyProductSavingLoc)).Locator("//p");
                string itemSavingPrice = await GetTextFromElement(itemSavingLocator);
                
                //If there is no value for savings, then set it as $0 instead of empty string
                if (!itemSavingPrice.Contains("$"))
                {
                    itemSavingPrice = "$0";
                } 
                ILocator subTotalPriceLabelLocator = await LocateElementFromParent(item, trolleyProductSubTotalPriceText, SearchType.Text);
                ILocator subTotalPriceLabelParent = subTotalPriceLabelLocator.Locator("..");
                ILocator subTotalPriceLocator = subTotalPriceLabelParent.Locator(trolleyProductSubTotalPriceTag);
                string subTotalPrice = await GetTextFromElement(subTotalPriceLocator);
                ILocator chooseCheckBox = item.Locator(trolleyProductChooseCheckBox).Last;
                string checkedValue = await GetAttributeFromElement(chooseCheckBox, "value");
                float checkedToFloat = checkedValue.ToLower().Equals("true") ? 1 : 0;
                
                //Store the information in one dictionary
                var singleItemInfo = new Dictionary<string, float>
                {
                    { "each_price" , GetPriceNumberFromString(itemEachPrice) },
                    { "quantity", float.Parse(itemQantity) },
                    { "savings", GetPriceNumberFromString(itemSavingPrice) },
                    { "total_price", GetPriceNumberFromString(subTotalPrice) },
                    { "is_chosen",  checkedToFloat}
                };
                logger.LogInformation(JsonConvert.SerializeObject(singleItemInfo));
                itemInfo.Add(itemName, singleItemInfo);
            }
            string itemInfoStr = JsonConvert.SerializeObject(itemInfo);
            logger.LogInformation(itemInfoStr);
            return itemInfo;
        }
        
        /// <summary>
        /// Fetch all product ILocators in the trolley, it needs to wait for all items displayed in the list 
        /// Because there is some delay for showing the list.
        /// </summary>
        /// <param name="timeout">The timeout to wait produc items showing</param>
        /// <returns>A List for ILocators of product items</returns>
        private async Task<IReadOnlyList<ILocator>> GetAllProductItems(TimeSpan timeout)
        {
            bool success = false;
            double elapsed = 0;
            DateTime time = DateTime.Now;
            IReadOnlyList<ILocator> trolleyProductsList;
            int previousListCount = 0;
            int retryCount = 0;
            int maxRetryCount = 3;  //determine if the showing list is stable when detecting it for 3 times
            int defaultInterval = 500; //detection interval 

            trolleyProductsList = await _page.Locator(trolleyProductSectionTag).Locator("visible=true").AllAsync();
            //Try to wait the item list showing and unchanged for 3 times detection.
            while ((!success) && (elapsed < timeout.TotalSeconds))
            {                
                int courrentListCount = trolleyProductsList.Count;
                retryCount = (courrentListCount > 0 && courrentListCount == previousListCount) ? retryCount + 1 : 0;
                if(retryCount >= maxRetryCount)
                {
                    success = true;
                    continue;
                }
                Thread.Sleep(defaultInterval);
                elapsed = DateTime.Now.Subtract(time).Seconds;
                previousListCount = courrentListCount;
                trolleyProductsList = await _page.Locator(trolleyProductSectionTag).Locator("visible=true").AllAsync();
            }
            return trolleyProductsList;
        }

        /// <summary>
        /// The method is used to assert if the total price and subtotal price is correct
        /// </summary>
        /// <returns>Task</returns>
        public async Task AssertPriceAccuraracy()
        {
            Dictionary<string, Dictionary<string, float>> productListInfo = await GetTrolleyItems();
            float showingTotalPrice = await GetTotalPrice();
            float actualTotalPrice = 0;

            foreach(string productName in productListInfo.Keys)
            {
                Dictionary<string, float> productInfo = productListInfo[productName];
                float eachPrice = productInfo["each_price"];
                float productCount = productInfo["quantity"];
                float savingPrice = productInfo["savings"];
                float subTotalPrice = productInfo["total_price"];
                float isChecked = productInfo["is_chosen"];
                float expectedSubTotalPrice = eachPrice * productCount;
                logger.LogInformation($"{productName}: expecting price is:{expectedSubTotalPrice}, subtotal price is:{subTotalPrice}");
                Assert.IsTrue(Math.Abs(expectedSubTotalPrice - subTotalPrice) < 0.01);
                actualTotalPrice += isChecked == 1 ? subTotalPrice : 0;
            }
            logger.LogInformation($"The expecting total price is:{actualTotalPrice}, the showing total price is:{showingTotalPrice}");
            Assert.IsTrue(Math.Abs(actualTotalPrice - showingTotalPrice) < 0.01);

        }

        /// <summary>
        /// The method is used to fetch the subtotal price for a single product 
        /// </summary>
        /// <param name="parentLocator">The product root ILocator</param>
        /// <returns>The subtotal price Ilocator of one product item</returns>
        public async Task<ILocator> GetEachPriceLocator(ILocator parentLocator)
        {
            ILocator priceLocator = parentLocator.Locator(trolleyProductSingleUnitPriceLoc)
                .Or(parentLocator.Locator(trolleyProductPriceLoc)).Nth(0);

            await priceLocator.IsVisibleAsync();
            return priceLocator;
        }

        /// <summary>
        /// The method is used to fetch the total price of all product items
        /// </summary>
        /// <returns>the float price</returns>
        public async Task<float> GetTotalPrice()
        {
            float totalPrice;
            ILocator productPriceTotalParentLocator = await LocateElement(trolleyProductTotalPriceLoc);
            ILocator productPriceTotalLocator = productPriceTotalParentLocator.Locator(trolleyProductTotalPriceClassname);
            string productPriceValue = await GetTextFromElement(productPriceTotalLocator);
            totalPrice = GetPriceNumberFromString(productPriceValue);
            return totalPrice;
        }

        /// <summary>
        /// The method is used to parse the price text and convert the number to float format
        /// </summary>
        /// <param name="priceValue">The price value such as "$12.5 each"</param>
        /// <returns>The float value</returns>
        /// <exception cref="InvalidDataException">Throw a Invalid Exception if the price text is not a valid price</exception>
        public float GetPriceNumberFromString(string priceValue)
        {
            string pattern = @"\$(\d+[.]?\d*)";
            Regex r = new Regex(pattern);
            Match m = r.Match(priceValue);
            if (m.Success)
            {
                string priceNumber = m.Groups[1].Value;
                return float.Parse(priceNumber);
            } else
            {
                throw new InvalidDataException($"Failed to parse price value:{priceValue}");
            }
        }

        /// <summary>
        /// The method is used to input the quantity number in the input field
        /// </summary>
        /// <param name="productName">string of product numberr</param>
        /// <param name="quantityNumber">string of quantity number</param>
        /// <returns></returns>
        public async Task SetQuantityNumber(string productName, string quantityNumber)
        {
            ILocator cdxCardElement = await LocateElementByChildText(productName, trolleyProductSectionTag);
            ILocator quantityLocator = await LocateElementFromParent(cdxCardElement, trolleyProductQuantityInput);
            //Click the input firstly, then it may becomes editable status.
            await quantityLocator.ClickAsync();
            await quantityLocator.FillAsync(quantityNumber);
            ILocator confirmButton = await LocateElementFromParent(cdxCardElement, trolleyProductQuantityConfirmButton, SearchType.Text);
            await confirmButton.ClickAsync();
        }
    }
}
