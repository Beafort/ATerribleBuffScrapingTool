using Microsoft.Win32;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools.V120.Debugger;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Media;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.DevTools.V120.Media;
using NAudio;
using NAudio.Wave;
namespace Automated_Item_Buyer
{
    internal class Program
    {

        static void Main(string[] args)
        {
            //Initialize the first driver
            string currentUser = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            ChromeOptions options = new ChromeOptions();
            options.AddArguments(@"--user-data-dir=C:\Users\Beafo\AppData\Local\Google\Chrome\User Data");
            Console.WriteLine("Enter the id of the item: ");
            String itemID = Console.ReadLine();
            String itemUrl = "https://buff.163.com/goods/" + itemID + "?from=market#tab=selling&page_num=1&sort_by=created.desc";
            IWebDriver driver = new ChromeDriver(options);
            driver.Navigate().GoToUrl(itemUrl);
            Thread.Sleep(2000);
            Console.Clear();

            


            //Buy logics
            bool buying = true;
            Console.WriteLine("What is the maximum price for this item?: ");
            string inputPrice = Console.ReadLine();
            float price = Convert.ToSingle(inputPrice);
            Console.WriteLine("How many of the items do you want to buy?: ");
            int buyAmount = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("What is the maximum float of the item ? (if the item doenst have a float then enter 0)");
            string inputWear = Console.ReadLine();
            float wear = Convert.ToSingle(inputWear);



           
            while (buying)
            {
                buying = BuyItem(price, wear, buyAmount, driver, itemUrl);
            }
           




            



        }

        /// <summary>
        /// Buying item
        /// </summary>
        /// <param name="itemPrice">The highest acceptable price</param>
        /// <param name="buyAmount">The amount of the items</param>
        /// <param name="driver">The driver initialized</param>
        /// <param name="wear">The maximum float</param>
        static bool BuyItem(float desiredPrice, float desiredWear, int buyAmount, IWebDriver driver, string itemUrl)
        {

            bool found = false;
            // For loop for 10 items (the header row counts as 1 already)
            for (int i = 2; i <= 11; i++)
            {
                IWebElement sellOrderList;
                //Get the sell order list 
                while (true)
                {
                    try
                    {
                        sellOrderList = driver.FindElement(By.XPath("//*[@id=\"market-selling-list\"]/tbody/tr[" + i + "]"));
                        break;
                    }
                    catch
                    {
                        //Extend sleep duration if the the site has not been loaded fully yet
                        Console.WriteLine("Something Happened !");
                        using (var audioFile = new AudioFileReader(@"""C:\Users\Beafo\source\repos\BUfffffffff\BUfffffffff\windows-error-sound-effect-35894.mp3"""))
                        using (var outputDevice = new WaveOutEvent())
                        {
                            outputDevice.Init(audioFile);
                            outputDevice.Play();
                            while (outputDevice.PlaybackState == PlaybackState.Playing)
                            {
                                Thread.Sleep(100);
                            }
                        }
                        Thread.Sleep(1000);

                    }
                }
                
                


                //get price from site as string and make price conversion into float
                float price = toPrice(sellOrderList.FindElement(By.XPath("/ html / body / div[6] / div / div[7] / table / tbody / tr[" + i + "] / td[5] / div[1] / strong")).Text);



                float wear;
                //get wear from site as string and check if the item has been inspected
                try
                {
                    wear = toWear(sellOrderList.FindElement(By.XPath("/ html / body / div[6] / div / div[7] / table / tbody / tr[" + i + "] / td[3] / div / div[1] / div[1]")).Text);
                }
                catch (Exception e)
                {
                    Console.WriteLine("The item at place numbered " + (i - 1) + " has not been inspected");
                    continue;
                }


                //Check if the item meet the conditions 
                if (checkCondition(desiredPrice, desiredWear, price, wear) == true)
                {
                    Console.WriteLine("Found an item that fit the criteria at place numbered: " + i +   " " + price + " " + wear);
                    found = true;
                }
            }
            //Check found and not found
            if (!found)
            {
                Console.WriteLine("None found, refreshing");

            }
            else
            {
                using (var audioFile = new AudioFileReader(@"C:\Users\Beafo\source\repos\BUfffffffff\BUfffffffff\stop-13692.mp3"))
                using (var outputDevice = new WaveOutEvent())
                {
                    outputDevice.Init(audioFile);
                    outputDevice.Play();
                    while (outputDevice.PlaybackState == PlaybackState.Playing)
                    {
                        Thread.Sleep(100);
                    }
                }
                Console.WriteLine("Continue ? [Y/N]");
                if (Console.ReadLine().ToUpper() == "N")
                {
                    return false;
                }
            }
            driver.Navigate().Refresh();
            Thread.Sleep(2000);
            
            return true;
            

        }








        /// <summary>
        /// Check if price and wear is lower than the desired ones
        /// </summary>
        /// <param name="desiredPrice"></param>
        /// <param name="desiredWear"></param>
        /// <param name="Price"></param>
        /// <param name="Wear"></param>
        /// <returns></returns>

        public static bool checkCondition(float desiredPrice, float desiredWear, float price, float  wear)
        {
            //check if the price and wear of the input is lower then the desired one 
            if (price <= desiredPrice && wear <= desiredWear)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static float toWear(string wear)
        {
            float returnWear = Convert.ToSingle(wear.Trim().Remove(0, 7));
            return returnWear;
        }
        public static float toPrice(string  price)
        {
            float returnPrice = Convert.ToSingle(price.Trim().Remove(0, 2));
            return returnPrice;
        }
        public static int Clamp(int value, int min, int max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }
    }
}
