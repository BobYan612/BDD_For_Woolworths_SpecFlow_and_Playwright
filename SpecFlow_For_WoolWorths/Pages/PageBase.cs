/********************************************************************************
# This is a practice project from Biao Yan (Bob Yan), and it's free to be 
# downloaded for study and test project.
*********************************************************************************/

using Microsoft.Playwright;
using System.Text.RegularExpressions;
using SpecFlow_For_WoolWorths.Support;
using Microsoft.Extensions.Logging;


namespace PlaywrightTest.pages
{
    /// <summary>
    /// The class is used to define the constant variables for search type
    /// </summary>
    public static class SearchType
    {
        public const string XPath = "xpath";
        public const string Text = "text";
        public const string ID = "id";
        public const string CSS = "css";
    }

    /// <summary>
    /// The class is the base object for all page objects and it creates some
    /// common methods and variables for its children.
    /// </summary>
    public abstract class PageBase
    {
        //The IPage object used to access the GUI components.
        protected readonly IPage _page;

        //The logger is used to manage and format the log ouput.
        protected ILogger logger;

        //the user settings from outside configuration file
        protected UserConfiguration userSettings = UserConfiguration.Instance;

        /// <summary>
        /// The unified constructure for all page objects, it defines some common variables
        /// and invokes the initial functions defined in child object. In the initial function,
        /// the child page objecct may initialize the location variabls of every elements
        /// </summary>
        /// <param name="page">the instantialized IPage object</param>
        public PageBase(IPage page)
        {
            _page = page;            
            using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddSimpleConsole(
                   options =>
                   {
                       options.IncludeScopes = true;
                       options.SingleLine = true;
                       options.TimestampFormat = "yyyy HH:mm:ss ";
                   }
                ));
            logger = factory.CreateLogger(this.GetType().Name);
            InitLocationDefinition();
        }

        /// <summary>
        /// The abstranct initialized method which should be implemented by child object.
        /// </summary>
        protected abstract void InitLocationDefinition();

        /// <summary>
        /// The method is used to search one element on the page. It append "visible=true" to forcely find 
        /// the visible element and await it's visible or it will throw out one timeout issue.
        /// </summary>
        /// <param name="locationDefine">The location definition for xpath/text/css/id</param>
        /// <param name="searchType">Only supports: xpath/css/id/text</param>
        /// <returns>ILocator object</returns>
        /// <exception cref="NotImplementedException">Exception for undefined type</exception>
        public async Task<ILocator> LocateElement(string locationDefine, string searchType = SearchType.XPath)
        {
            ILocator locator;        
            
            switch(searchType)
            {
                case SearchType.XPath:
                    locator = _page.Locator("xpath=" + locationDefine).Locator("visible=true").Last;
                    break;
                case SearchType.Text:
                    string matchPattern = @"^\s*" + locationDefine + @"\s*$";
                    Regex matchRegex = new Regex(matchPattern, RegexOptions.IgnoreCase);
                    locator = _page.GetByText(matchRegex).Locator("visible=true").Last;
                    break;
                case SearchType.ID:
                    locator = _page.GetByTestId(locationDefine).Locator("visible=true").Last;
                    break;
                case SearchType.CSS:
                    locator = _page.Locator("css=" + locationDefine).Locator("visible=true").Last;
                    break;
                default:
                    throw new NotImplementedException("Can't support ");
            }
            await locator.IsVisibleAsync();
            return locator;
        }

        /// <summary>
        /// The method is used to search one element on the page based on another ILocator object.
        /// </summary>
        /// <param name="parentLocator">The source ILocator to find its child element</param>
        /// <param name="locationDefine">The location definition for xpath/text/css/id </param>
        /// <param name="searchType">Only supports: xpath/css/id/text</param>
        /// <returns>ILocator object</returns>
        /// <exception cref="NotImplementedException">Exception for undefined type</exception>
        public async Task<ILocator> LocateElementFromParent(ILocator parentLocator, string locationDefine, string searchType = SearchType.XPath)
        {
            ILocator locator;

            switch (searchType)
            {
                case SearchType.XPath:
                    locator = parentLocator.Locator(locationDefine).Locator("visible=true").Last;
                    break;
                case SearchType.Text:
                    string matchPattern = @"^\s*" + locationDefine + @"\s*$";
                    Regex matchRegex = new Regex(matchPattern, RegexOptions.IgnoreCase);
                    locator = parentLocator.GetByText(matchRegex).Locator("visible=true").Last;
                    break;
                case SearchType.ID:
                    locator = parentLocator.GetByTestId(locationDefine).Locator("visible=true").Last;
                    break;
                case SearchType.CSS:
                    locator = parentLocator.Locator(locationDefine).Locator("visible=true").Last;
                    break;
                default:
                    throw new NotImplementedException("Can't support ");
            }
            await locator.IsVisibleAsync();
            return locator;
        }

        /// <summary>
        /// The method is used to select multiple menu items 
        /// For Example: Browse/Fruit & Veg/Fruit/Shop all Fruit, 
        /// it will click Browse, 
        /// then click Fruit on the popup menu
        /// then click "Shop all Fruit" on the next popup menu.
        /// </summary>
        /// <param name="pathLocation">The multiple menu path, separated by "/"</param>
        /// <returns>Task object</returns>
        public async Task ClickMenuItem(string pathLocation)
        {
            String[] pathList = pathLocation.Split('/');
            logger.LogDebug($"Selecting menu:{pathLocation}");
            foreach (var item in pathList)
            {
                string newValue = item;
                ILocator subMenuItem = _page.GetByText(newValue, new() { Exact = true }).Locator("visible=true").Last;
                ILocator clickItem = _page.Locator("//button or //a").Filter(new() { Has = subMenuItem });
                await subMenuItem.ClickAsync();
            }
        }

        /// <summary>
        /// Locate one parent according its child text defintion.
        /// For example: find a cdx-card whose child or grandchild has a text of "fresh fruit bananas yellow"
        /// </summary>
        /// <param name="childText">The unique text in the child element</param>
        /// <param name="parentTag">parant tag such as div, cdx-card</param>
        /// <param name="source">ILocator object which is the root element for the filter</param>
        /// <returns>ILocator object</returns>
        public async Task<ILocator> LocateElementByChildText(string childText, string parentTag, ILocator? source=null)
        {
            ILocator parentItem;
            string matchPattern = @"^\s*" + childText + @"\s*$";
            Regex matchRegex = new Regex(matchPattern, RegexOptions.IgnoreCase);
            if (source == null)
            {                
                ILocator childItem = _page.GetByText(matchRegex).Locator("visible=true").Last;
                parentItem = _page.Locator($"{parentTag}").Filter(new() { Has = childItem });
                await parentItem.IsVisibleAsync();
            } else
            {
                ILocator childItem = source.GetByText(matchRegex).Locator("visible=true").Last;
                parentItem = source.Locator($"{parentTag}").Filter(new() { Has = childItem });
                await parentItem.IsVisibleAsync();
            }
            return parentItem;
        }

        /// <summary>
        /// Locate one input according its following label.
        /// Sometimes the input is not a same level as label 
        /// instead it's under another element which is close to the label
        /// </summary>
        /// <param name="labelText">The label text content</param>
        /// <returns>ILocator for input object</returns>
        public async Task<ILocator> LocateInputByLabel(string labelText)
        {
            ILocator inputItem = await LocateElement($"//label[contains(.,'{labelText}')]/following-sibling::input[1] | //label[contains(.,'{labelText}')]/following-sibling::*[1]//input");
            await inputItem.IsVisibleAsync();
            return inputItem;
        }

        /// <summary>
        /// Fetch the text content for one element
        /// </summary>
        /// <param name="elemLocator">ILocator object which has the text</param>
        /// <returns>string text</returns>
        /// <exception cref="TextNotFound">Throw one exception if the text is null</exception>
        public async Task<string> GetTextFromElement(ILocator elemLocator)
        {
            string? content = await elemLocator.TextContentAsync();
            if (content == null)
            {
                throw new TextNotFound("Failed to find the text content");
            }
            return content;
        }

        /// <summary>
        /// Fetch one attribute value from one ILocator object.
        /// </summary>
        /// <param name="elemLocator">ILocator object</param>
        /// <param name="attributeName">the attribute name</param>
        /// <returns>string value of the attribute</returns>
        /// <exception cref="TextNotFound">Throw TextNotFound Exception if the value is null</exception>
        public async Task<string> GetAttributeFromElement(ILocator elemLocator, string attributeName)
        {
            string? value = await elemLocator.GetAttributeAsync(attributeName);
            if (value == null)
            {
                throw new TextNotFound("Failed to find the text content");
            }
            return value;
        }
    }
    /// <summary>
    /// The ElementNotFound for page object
    /// </summary>
    public class ElementNotFound : Exception
    {
        public ElementNotFound(string message) : base(message) { }
    }

    /// <summary>
    /// TextNotFound Exception for page object.
    /// </summary>
    public class TextNotFound : Exception
    {
        public TextNotFound(string message) : base(message) { }
    }


}
