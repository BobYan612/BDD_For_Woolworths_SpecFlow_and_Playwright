/********************************************************************************
# This is a practice project from Biao Yan (Bob Yan), and it's free to be 
# downloaded for study and test project.
*********************************************************************************/

using Microsoft.Playwright;
using PlaywrightTest.pages;
using SpecFlow_For_WoolWorths.Pages;


namespace SpecFlow_For_WoolWorths.StepDefinitions
{
    [Binding]
    public class TrolleyItemListStepDefinitions
    {
        public IPage iPage { get; private set; } = null!;
        public HomePage homepage;
        public TrolleyItemListStepDefinitions(Hooks.Hooks hook)
        {
            iPage = hook.User;
            homepage = new HomePage(iPage);
        }

        [Given(@"the user has login in")]
        public async Task GivenTheUserHasLoginIn()
        {
            await homepage.InitLogin();
        }

        [Given(@"Trolley item list is empty")]
        public async Task GivenTrolleyItemListIsEmpty()
        {
            var trolleyPage = await homepage.GotoTrolley();
            await trolleyPage.ClearTrolley();
        }

        [When(@"add ""([^""]*)"" to trolley under ""([^""]*)"" in ""([^""]*)""")]
        public async Task WhenAddToTrolleyUnderIn(string productName, string menuPath, string pageName)
        {
            BrowsePage pageObject;
            switch (pageName) 
            {
                case HomePage.BrowsePage:
                    pageObject = await homepage.GotoBrowse();
                    break;
                case HomePage.SpecialPage:
                    pageObject = await homepage.GotoSpecialPage();
                    break;
                case HomePage.LowpPicePage:
                    pageObject = await homepage.GotoLowPricePage();
                    break;
                default:
                    throw new NotSupportedException($"Can't support the page:{pageName}");
            }
            if (menuPath.Trim().Length > 0)
            {
               await pageObject.SelectCategory(menuPath);
            }
            await pageObject.AddToTrolley(productName);
        }

        [Then(@"The ""([^""]*)"" in the trolley can be seen")]
        public async Task ThenTheInTheTrolleyCanBeSeen(string productName)
        {
            TrolleyPage trolleyPage = await homepage.GotoTrolley();
            await trolleyPage.HasProductItem(productName);

        }

        [Then(@"The total price and subtotal price have been shown correctly")]
        public async Task ThenTheTotalPriceAndSubtotalPriceHaveBeenShownCorrectly()
        {
            TrolleyPage trolleyPage = await homepage.GotoTrolley();
            await trolleyPage.AssertPriceAccuraracy();
        }

        [Given(@"add ""([^""]*)"" to trolley under ""([^""]*)"" in ""([^""]*)""")]
        public async Task GivenAddToTrolleyUnderIn(string productName, string menuPath, string pageName)
        {
            await WhenAddToTrolleyUnderIn(productName, menuPath, pageName);
        }

        [When(@"click ""([^""]*)"" for ""([^""]*)"" in Trolley Page")]
        public async Task WhenClickForInTrolleyPage(string buttonType, string productName)
        {
            TrolleyPage trolleyPage = await homepage.GotoTrolley();
            if(buttonType == "plus")
            {
                await trolleyPage.AddCountForChosenItem(productName);
            }
            else if(buttonType == "minus")
            {
                await trolleyPage.ReduceCountForChosenItem(productName);
            }
            else if(buttonType == "remove")
            {
                await trolleyPage.RemoveChosenItem(productName);
            }
            else
            {
                throw new NotImplementedException($"The {buttonType} has not been implemented!");
            }
        }

        [When(@"set ""([^""]*)"" to quantity for ""([^""]*)"" in Trolley Page")]
        public async Task WhenSetToQuantityForInTrolleyPage(string quantityNumber, string productName)
        {
            TrolleyPage trolleyPage = await homepage.GotoTrolley();
            await trolleyPage.SetQuantityNumber(productName, quantityNumber);
        }



    }
}
