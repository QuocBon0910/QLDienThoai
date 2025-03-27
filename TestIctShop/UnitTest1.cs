using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using ClosedXML.Excel;
using System.IO;
using System;

namespace TestIctShop
{
    [TestFixture]
    public class AdminProductTests
    {
        private string filePath = @"D:\HK2-Y3\BDCLPM-LT\Test Case\Quản lý sản phẩm - Quốc Đạt.xlsx"; // Đường dẫn file Excel
        private int currentRow = 2; // Bắt đầu từ dòng 2 (dòng 1 là tiêu đề)
        private class TestResult
        {
            public string TestName { get; }
            public string Status { get; }
            public DateTime StartTime { get; }
            public DateTime EndTime { get; }
            public string ErrorMessage { get; }

            public TestResult(string testName, string status, DateTime startTime, DateTime endTime, string errorMessage)
            {
                TestName = testName;
                Status = status;
                StartTime = startTime;
                EndTime = endTime;
                ErrorMessage = errorMessage;
            }
        }

        private IWebDriver driver;
        private WebDriverWait wait;
 

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1);
            driver.Manage().Window.Maximize();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(1));
        }

        private void Login(string email, string password)
        {
            driver.Navigate().GoToUrl("https://localhost:44322/User/DangNhap");
            driver.FindElement(By.Id("userMail")).SendKeys(email);
            driver.FindElement(By.Id("password")).SendKeys(password);
            driver.FindElement(By.XPath("//input[@value='Đăng nhập']")).Click();
            wait.Until(d => d.Url.Contains("Admin/Home"));
        }

        [Test, Order(2)]
        public void AddProductNoName()
        {
            Login("Admin@gmail.com", "12345678");

            IWebElement addButton = WaitForElement(By.XPath("//p/button[@class='btn-default']/a"), 10);
            addButton.Click();

            driver.FindElement(By.Id("Tensp")).SendKeys("");
            driver.FindElement(By.Id("Giatien")).SendKeys("5000000");
            driver.FindElement(By.Id("Soluong")).SendKeys("100");
            driver.FindElement(By.Id("Mota")).SendKeys("Sản phẩm hot");
            driver.FindElement(By.Id("Thesim")).SendKeys("2");
            driver.FindElement(By.Id("Bonhotrong")).SendKeys("128");
            driver.FindElement(By.Id("Ram")).SendKeys("8");

            new SelectElement(driver.FindElement(By.Id("Sanphammoi"))).SelectByText("False");

            driver.FindElement(By.Id("Anhbia")).SendKeys("/Images/files/ss3.jpg");

            new SelectElement(driver.FindElement(By.Id("Mahang"))).SelectByText("Sam Sung");
            new SelectElement(driver.FindElement(By.Id("Mahdh"))).SelectByText("Android");
            driver.FindElement(By.XPath("//input[@type='submit' and @value='Thêm mới sản phẩm']")).Click();

            Assert.IsFalse(driver.FindElements(By.XPath("//table[contains(@class,'table-bordered')]//tr")).Count > 0, "❌ Sản phẩm không hợp lệ nhưng vẫn được thêm!");
        }


        [Test, Order(1)]
        public void AddProduct()
        {
            Login("Admin@gmail.com", "12345678");

            IWebElement addButton = wait.Until(d => d.FindElement(By.XPath("//p/button[@class='btn-default']/a")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", addButton);

            string productName = "Xiaomi Redmi K20 Pro";
            driver.FindElement(By.Id("Tensp")).SendKeys(productName);
            driver.FindElement(By.Id("Giatien")).SendKeys("5000000");
            driver.FindElement(By.Id("Soluong")).SendKeys("100");
            driver.FindElement(By.Id("Mota")).SendKeys("Sản phẩm hot");
            driver.FindElement(By.Id("Thesim")).SendKeys("2");
            driver.FindElement(By.Id("Bonhotrong")).SendKeys("128");
            driver.FindElement(By.Id("Ram")).SendKeys("8");

            new SelectElement(driver.FindElement(By.Id("Sanphammoi"))).SelectByText("False");
            new SelectElement(driver.FindElement(By.Id("Mahang"))).SelectByText("Sam Sung");
            new SelectElement(driver.FindElement(By.Id("Mahdh"))).SelectByText("Android");

            driver.FindElement(By.Id("Anhbia")).SendKeys("/Images/files/ss3.jpg");

            IWebElement submitButton = driver.FindElement(By.XPath("//input[@type='submit' and @value='Thêm mới sản phẩm']"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", submitButton);

            WaitForUrl("Admin/Home", 10);

            IWebElement productCell = WaitForElement(By.XPath($"//table[contains(@class,'table-bordered')]//td[contains(text(), '{productName}')]"), 10);

            Assert.IsNotNull(productCell, "❌ Sản phẩm không xuất hiện trong bảng trên trang Admin/Home.");
        }


        private void WaitForUrl(string expectedUrlPart, int timeoutInSeconds)
        {
            for (int i = 0; i < timeoutInSeconds * 2; i++) // Kiểm tra mỗi 0.5 giây
            {
                if (driver.Url.Contains(expectedUrlPart))
                    return;
                Thread.Sleep(500);
            }
            throw new TimeoutException($"❌ Lỗi: Không chuyển hướng đến {expectedUrlPart} sau {timeoutInSeconds} giây.");
        }
        private IWebElement WaitForElement(By by, int timeoutInSeconds)
        {
            for (int i = 0; i < timeoutInSeconds * 2; i++)  
            {
                try
                {
                    var elements = driver.FindElements(by);
                    if (elements.Count > 0 && elements[0].Displayed)
                    {
                        return elements[0]; 
                    }
                }
                catch (Exception) { }

                Thread.Sleep(500); 
            }

            throw new TimeoutException($"❌ Lỗi: Không tìm thấy phần tử {by} sau {timeoutInSeconds} giây.");
        }
        [Test, Order(3)]
        public void AddProduct_CheckDuplicate()
        {
            Login("Admin@gmail.com", "12345678");

            string productName = "Xiaomi Redmi K20 Pro";

            // Kiểm tra số lượng sản phẩm trùng tên trước khi thêm
            int existingCount = driver.FindElements(By.XPath($"//table[contains(@class,'table-bordered')]//td[contains(text(), '{productName}')]")).Count;

            // Nếu đã có sản phẩm trùng tên, test sẽ FAIL ngay lập tức
            Assert.Fail($"❌ Test Failed: Đã có sản phẩm '{productName}' trong bảng, không thể thêm trùng!");

            // Code thêm sản phẩm (sẽ không chạy nếu test fail ở bước trên)
            IWebElement addButton = wait.Until(d => d.FindElement(By.XPath("//p/button[@class='btn-default']/a")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", addButton);

            driver.FindElement(By.Id("Tensp")).SendKeys(productName);
            driver.FindElement(By.Id("Giatien")).SendKeys("5000000");
            driver.FindElement(By.Id("Soluong")).SendKeys("100");
            driver.FindElement(By.Id("Mota")).SendKeys("Sản phẩm hot");
            driver.FindElement(By.Id("Thesim")).SendKeys("2");
            driver.FindElement(By.Id("Bonhotrong")).SendKeys("128");
            driver.FindElement(By.Id("Ram")).SendKeys("8");

            new SelectElement(driver.FindElement(By.Id("Sanphammoi"))).SelectByText("False");
            new SelectElement(driver.FindElement(By.Id("Mahang"))).SelectByText("Sam Sung");
            new SelectElement(driver.FindElement(By.Id("Mahdh"))).SelectByText("Android");

            driver.FindElement(By.Id("Anhbia")).SendKeys("/Images/files/ss3.jpg");

            IWebElement submitButton = driver.FindElement(By.XPath("//input[@type='submit' and @value='Thêm mới sản phẩm']"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", submitButton);

            // Kiểm tra số lượng sản phẩm sau khi thêm
            int finalCount = driver.FindElements(By.XPath($"//table[contains(@class,'table-bordered')]//td[contains(text(), '{productName}')]")).Count;

            // Nếu số lượng sản phẩm tăng lên, test fail
            Assert.AreEqual(existingCount, finalCount, $"❌ Test Failed: Hệ thống cho phép thêm sản phẩm '{productName}' trùng tên!");
        }


        [Test, Order(4)]
        public void AddProductVietnameseName()
        {
            Login("Admin@gmail.com", "12345678");

            IWebElement addButton = wait.Until(d => d.FindElement(By.XPath("//p/button[@class='btn-default']/a")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", addButton);

            string productName = "Điện thoại Xiaomi Redmi K20 Pro";
            driver.FindElement(By.Id("Tensp")).SendKeys(productName);
            driver.FindElement(By.Id("Giatien")).SendKeys("5000000");
            driver.FindElement(By.Id("Soluong")).SendKeys("100");
            driver.FindElement(By.Id("Mota")).SendKeys("Sản phẩm hot");
            driver.FindElement(By.Id("Thesim")).SendKeys("2");
            driver.FindElement(By.Id("Bonhotrong")).SendKeys("128");
            driver.FindElement(By.Id("Ram")).SendKeys("8");

            new SelectElement(driver.FindElement(By.Id("Sanphammoi"))).SelectByText("False");
            new SelectElement(driver.FindElement(By.Id("Mahang"))).SelectByText("Sam Sung");
            new SelectElement(driver.FindElement(By.Id("Mahdh"))).SelectByText("Android");

            driver.FindElement(By.Id("Anhbia")).SendKeys("/Images/files/ss3.jpg");

            IWebElement submitButton = driver.FindElement(By.XPath("//input[@type='submit' and @value='Thêm mới sản phẩm']"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", submitButton);

            WaitForUrl("Admin/Home", 10);

            IWebElement productCell = WaitForElement(By.XPath($"//table[contains(@class,'table-bordered')]//td[contains(text(), '{productName}')]"), 10);

            Assert.IsNotNull(productCell, "❌ Test Failed: Sản phẩm không xuất hiện trên trang Admin/Home.");
        }


        [Test, Order(5)]
        public void AddProductNoQuantity()
        {
            Login("Admin@gmail.com", "12345678");

            IWebElement addButton = wait.Until(d => d.FindElement(By.XPath("//p/button[@class='btn-default']/a")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", addButton);

            string productName = "Điện thoại Xiaomi Redmi K20 Pro";
            driver.FindElement(By.Id("Tensp")).SendKeys(productName);
            driver.FindElement(By.Id("Giatien")).SendKeys("5000000");
            driver.FindElement(By.Id("Mota")).SendKeys("Sản phẩm hot");
            driver.FindElement(By.Id("Thesim")).SendKeys("2");
            driver.FindElement(By.Id("Bonhotrong")).SendKeys("128");
            driver.FindElement(By.Id("Ram")).SendKeys("8");

            new SelectElement(driver.FindElement(By.Id("Sanphammoi"))).SelectByText("False");
            new SelectElement(driver.FindElement(By.Id("Mahang"))).SelectByText("Sam Sung");
            new SelectElement(driver.FindElement(By.Id("Mahdh"))).SelectByText("Android");

            driver.FindElement(By.Id("Anhbia")).SendKeys("/Images/files/ss3.jpg");

            IWebElement submitButton = driver.FindElement(By.XPath("//input[@type='submit' and @value='Thêm mới sản phẩm']"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", submitButton);

            WaitForUrl("Admin/Home", 10);

            var products = driver.FindElements(By.XPath($"//table[contains(@class,'table-bordered')]//td[contains(text(), '{productName}')]"));

            Assert.IsTrue(products.Count > 0, $"❌ Test Failed: Sản phẩm '{productName}' đã được thêm vào bảng mà không có số lượng!");
        }



        [Test, Order(6)]
        public void AddProductNegativeQuantity()
        {
            Login("Admin@gmail.com", "12345678");

            var addButton = wait.Until(d => d.FindElement(By.XPath("//p/button[@class='btn-default']/a")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", addButton);

            driver.FindElement(By.Id("Tensp")).SendKeys("Điện thoại Xiaomi Redmi K20 Pro");
            driver.FindElement(By.Id("Giatien")).SendKeys("5000000");
            driver.FindElement(By.Id("Soluong")).SendKeys("-5"); // Nhập số lượng âm
            driver.FindElement(By.Id("Mota")).SendKeys("Sản phẩm hot");
            driver.FindElement(By.Id("Thesim")).SendKeys("2");
            driver.FindElement(By.Id("Bonhotrong")).SendKeys("128");
            driver.FindElement(By.Id("Ram")).SendKeys("8");

            new SelectElement(driver.FindElement(By.Id("Sanphammoi"))).SelectByText("False");
            new SelectElement(driver.FindElement(By.Id("Mahang"))).SelectByText("Sam Sung");
            new SelectElement(driver.FindElement(By.Id("Mahdh"))).SelectByText("Android");

            driver.FindElement(By.Id("Anhbia")).SendKeys("/Images/files/ss3.jpg");

            var submitButton = driver.FindElement(By.XPath("//input[@type='submit' and @value='Thêm mới sản phẩm']"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", submitButton);

            // Kiểm tra nếu trang không chuyển hướng (tức là validation đúng)
            Assert.AreEqual("Admin/Create", new Uri(driver.Url).AbsolutePath, "❌ Test Failed: Hệ thống cho phép nhập số lượng âm và chuyển hướng sang trang Home!");

            // Nếu vẫn ở trang "Thêm sản phẩm", test pass ngay lập tức
            Assert.Pass("✅ Test Passed: Hệ thống chặn nhập số lượng âm!");
        }


        [Test, Order(7)]
        public void AddProductFractionalQuantity()
        {
            Login("Admin@gmail.com", "12345678");

            var addButton = wait.Until(d => d.FindElement(By.XPath("//p/button[@class='btn-default']/a")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", addButton);

            driver.FindElement(By.Id("Tensp")).SendKeys("Điện thoại Xiaomi Redmi K20 Pro");
            driver.FindElement(By.Id("Giatien")).SendKeys("5000000");
            driver.FindElement(By.Id("Soluong")).SendKeys("3.5"); // Nhập số lượng phân số
            driver.FindElement(By.Id("Mota")).SendKeys("Sản phẩm hot");
            driver.FindElement(By.Id("Thesim")).SendKeys("2");
            driver.FindElement(By.Id("Bonhotrong")).SendKeys("128");
            driver.FindElement(By.Id("Ram")).SendKeys("8");

            new SelectElement(driver.FindElement(By.Id("Sanphammoi"))).SelectByText("False");
            new SelectElement(driver.FindElement(By.Id("Mahang"))).SelectByText("Sam Sung");
            new SelectElement(driver.FindElement(By.Id("Mahdh"))).SelectByText("Android");

            driver.FindElement(By.Id("Anhbia")).SendKeys("/Images/files/ss3.jpg");

            string initialUrl = driver.Url;
            Console.WriteLine("🔍 URL trước khi submit: " + initialUrl);

            var submitButton = driver.FindElement(By.XPath("//input[@type='submit' and @value='Thêm mới sản phẩm']"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", submitButton);

            Thread.Sleep(3000);

            string finalUrl = driver.Url;
            Console.WriteLine("🔍 URL sau khi submit: " + finalUrl);

            // Kiểm tra nếu trang không chuyển hướng
            Assert.AreEqual(initialUrl, finalUrl, "❌ Test Failed: Hệ thống cho phép nhập số lượng phân số và chuyển hướng sang Home!");

            // Nếu vẫn ở trang "Thêm sản phẩm", test pass ngay lập tức
            Assert.Pass("✅ Test Passed: Hệ thống chặn nhập số lượng phân số!");
        }

        [Test, Order(8)]
        public void AddProductQuantityWithOperation()
        {
            Login("Admin@gmail.com", "12345678");

            IWebElement addButton = wait.Until(d => d.FindElement(By.XPath("//p/button[@class='btn-default']/a")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", addButton);

            driver.FindElement(By.Id("Tensp")).SendKeys("Điện thoại Xiaomi Redmi K20 Pro");
            driver.FindElement(By.Id("Giatien")).SendKeys("5000000");
            driver.FindElement(By.Id("Soluong")).SendKeys("5+7");
            driver.FindElement(By.Id("Mota")).SendKeys("Sản phẩm hot");
            driver.FindElement(By.Id("Thesim")).SendKeys("2");
            driver.FindElement(By.Id("Bonhotrong")).SendKeys("128");
            driver.FindElement(By.Id("Ram")).SendKeys("8");
            new SelectElement(driver.FindElement(By.Id("Sanphammoi"))).SelectByText("False");
            new SelectElement(driver.FindElement(By.Id("Mahang"))).SelectByText("Sam Sung");
            new SelectElement(driver.FindElement(By.Id("Mahdh"))).SelectByText("Android");

            driver.FindElement(By.Id("Anhbia")).SendKeys("/Images/files/ss3.jpg");

            string initialUrl = driver.Url;
            Console.WriteLine("🔍 URL trước khi submit: " + initialUrl);

            IWebElement submitButton = driver.FindElement(By.XPath("//input[@type='submit' and @value='Thêm mới sản phẩm']"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", submitButton);

            Thread.Sleep(3000); 

            string finalUrl = driver.Url;
            Console.WriteLine("🔍 URL sau khi submit: " + finalUrl);

            Assert.AreEqual(initialUrl, finalUrl, "❌ Test Failed: Hệ thống không kiểm tra số lượng chứa phép tính.");
            Console.WriteLine("✅ Test Passed: Hệ thống đã không thêm được sản phẩm với số lượng chứa phép tính.");
        }

        [Test, Order(9)]
        public void AddProductBigQuantity()
        {
            Login("Admin@gmail.com", "12345678");
            IWebElement addButton = wait.Until(d => d.FindElement(By.XPath("//p/button[@class='btn-default']/a")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", addButton);

            driver.FindElement(By.Id("Tensp")).SendKeys("Điện thoại Xiaomi Redmi K20 Pro");
            driver.FindElement(By.Id("Giatien")).SendKeys("5000000");
            driver.FindElement(By.Id("Soluong")).SendKeys("500000000000000"); // Số lượng rất lớn
            driver.FindElement(By.Id("Mota")).SendKeys("Sản phẩm hot");
            driver.FindElement(By.Id("Thesim")).SendKeys("2");
            driver.FindElement(By.Id("Bonhotrong")).SendKeys("128");
            driver.FindElement(By.Id("Ram")).SendKeys("8");

            new SelectElement(driver.FindElement(By.Id("Sanphammoi"))).SelectByText("False");
            new SelectElement(driver.FindElement(By.Id("Mahang"))).SelectByText("Sam Sung");
            new SelectElement(driver.FindElement(By.Id("Mahdh"))).SelectByText("Android");

            driver.FindElement(By.Id("Anhbia")).SendKeys("/Images/files/ss3.jpg");

            string initialUrl = driver.Url;
            Console.WriteLine("🔍 URL trước khi submit: " + initialUrl);

            IWebElement submitButton = driver.FindElement(By.XPath("//input[@type='submit' and @value='Thêm mới sản phẩm']"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", submitButton);

            Thread.Sleep(3000);

            string finalUrl = driver.Url;
            Console.WriteLine("🔍 URL sau khi submit: " + finalUrl);

            Assert.AreEqual(initialUrl, finalUrl, "❌ Test Failed: Hệ thống không kiểm tra số lượng lớn và đã thêm sản phẩm.");
            Console.WriteLine("✅ Test Passed: Hệ thống đã không thêm được sản phẩm với số lượng quá lớn.");
        }


        [Test, Order(10)]
        public void AddProductNoPrice()
        {
            Login("Admin@gmail.com", "12345678");

            IWebElement addButton = wait.Until(d => d.FindElement(By.XPath("//p/button[@class='btn-default']/a")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", addButton);

            string productName = "Điện thoại Xiaomi Redmi K20 Pro";
            driver.FindElement(By.Id("Tensp")).SendKeys(productName);
            driver.FindElement(By.Id("Soluong")).SendKeys("100");
            driver.FindElement(By.Id("Mota")).SendKeys("Sản phẩm hot");
            driver.FindElement(By.Id("Thesim")).SendKeys("2");
            driver.FindElement(By.Id("Bonhotrong")).SendKeys("128");
            driver.FindElement(By.Id("Ram")).SendKeys("8");

            new SelectElement(driver.FindElement(By.Id("Sanphammoi"))).SelectByText("False");
            new SelectElement(driver.FindElement(By.Id("Mahang"))).SelectByText("Sam Sung");
            new SelectElement(driver.FindElement(By.Id("Mahdh"))).SelectByText("Android");

            driver.FindElement(By.Id("Anhbia")).SendKeys("/Images/files/ss3.jpg");

            IWebElement submitButton = driver.FindElement(By.XPath("//input[@type='submit' and @value='Thêm mới sản phẩm']"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", submitButton);

            WaitForUrl("Admin/Home", 10);

            var products = driver.FindElements(By.XPath($"//table[contains(@class,'table-bordered')]//td[contains(text(), '{productName}')]"));

            Assert.IsTrue(products.Count > 0, $"❌ Test Failed: Sản phẩm '{productName}' đã được thêm vào bảng mà không có giá!");
        }
        

        [Test, Order(11)]
        public void AddProductNegativePrice()
        {
            Login("Admin@gmail.com", "12345678");

            IWebElement addButton = wait.Until(d => d.FindElement(By.XPath("//p/button[@class='btn-default']/a")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", addButton);

            string productName = "Điện thoại Xiaomi Redmi K20 Pro";
            driver.FindElement(By.Id("Tensp")).SendKeys(productName);
            driver.FindElement(By.Id("Giatien")).SendKeys("-1000000");
            driver.FindElement(By.Id("Soluong")).SendKeys("100");
            driver.FindElement(By.Id("Mota")).SendKeys("Sản phẩm hot");
            driver.FindElement(By.Id("Thesim")).SendKeys("2");
            driver.FindElement(By.Id("Bonhotrong")).SendKeys("128");
            driver.FindElement(By.Id("Ram")).SendKeys("8");
            new SelectElement(driver.FindElement(By.Id("Sanphammoi"))).SelectByText("False");
            new SelectElement(driver.FindElement(By.Id("Mahang"))).SelectByText("Sam Sung");
            new SelectElement(driver.FindElement(By.Id("Mahdh"))).SelectByText("Android");

            driver.FindElement(By.Id("Anhbia")).SendKeys("/Images/files/ss3.jpg");

            IWebElement submitButton = driver.FindElement(By.XPath("//input[@type='submit' and @value='Thêm mới sản phẩm']"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", submitButton);

            WaitForUrl("Admin/Home", 10);

            var products = driver.FindElements(By.XPath($"//table[contains(@class,'table-bordered')]//td[contains(text(), '{productName}')]"));

            Assert.IsTrue(products.Count > 0, $"❌ Test Failed: Sản phẩm '{productName}' đã được thêm vào bảng mà giá âm!");
        }

        [Test, Order(12)]
        public void AddProductFractionalPrice()
        {
            Login("Admin@gmail.com", "12345678");

            IWebElement addButton = wait.Until(d => d.FindElement(By.XPath("//p/button[@class='btn-default']/a")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", addButton);

            driver.FindElement(By.Id("Tensp")).SendKeys("Điện thoại Xiaomi Redmi K20 Pro");
            driver.FindElement(By.Id("Giatien")).SendKeys("-666,666"); 
            driver.FindElement(By.Id("Soluong")).SendKeys("1000");
            driver.FindElement(By.Id("Mota")).SendKeys("Sản phẩm hot");
            driver.FindElement(By.Id("Thesim")).SendKeys("2");
            driver.FindElement(By.Id("Bonhotrong")).SendKeys("128");
            driver.FindElement(By.Id("Ram")).SendKeys("8");

            new SelectElement(driver.FindElement(By.Id("Sanphammoi"))).SelectByText("False");
            new SelectElement(driver.FindElement(By.Id("Mahang"))).SelectByText("Sam Sung");
            new SelectElement(driver.FindElement(By.Id("Mahdh"))).SelectByText("Android");

            driver.FindElement(By.Id("Anhbia")).SendKeys("/Images/files/ss3.jpg");
            string initialUrl = driver.Url;
            Console.WriteLine($"🔍 URL trước khi submit: {initialUrl}");

            IWebElement submitButton = driver.FindElement(By.XPath("//input[@type='submit' and @value='Thêm mới sản phẩm']"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", submitButton);

            Assert.AreEqual(initialUrl, driver.Url, "❌ Test Failed: Hệ thống đã chuyển hướng, tức là sản phẩm vẫn được thêm!");

            Console.WriteLine("✅ Test Passed: Hệ thống không cho phép thêm sản phẩm với giá phân số.");
        }


        [Test, Order(13)]
        public void AddProductPriceWithOperation()
        {
            Login("Admin@gmail.com", "12345678");

            IWebElement addButton = wait.Until(d => d.FindElement(By.XPath("//p/button[@class='btn-default']/a")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", addButton);

            driver.FindElement(By.Id("Tensp")).SendKeys("Điện thoại Xiaomi Redmi K20 Pro");
            driver.FindElement(By.Id("Giatien")).SendKeys("5000000+1000000"); 
            driver.FindElement(By.Id("Mota")).SendKeys("Sản phẩm hot");
            driver.FindElement(By.Id("Thesim")).SendKeys("2");
            driver.FindElement(By.Id("Bonhotrong")).SendKeys("128");
            driver.FindElement(By.Id("Ram")).SendKeys("8");

            new SelectElement(driver.FindElement(By.Id("Sanphammoi"))).SelectByText("False");
            new SelectElement(driver.FindElement(By.Id("Mahang"))).SelectByText("Sam Sung");
            new SelectElement(driver.FindElement(By.Id("Mahdh"))).SelectByText("Android");
            driver.FindElement(By.Id("Anhbia")).SendKeys("/Images/files/ss3.jpg");

            string initialUrl = driver.Url;
            Console.WriteLine($"🔍 URL trước khi submit: {initialUrl}");

            IWebElement submitButton = driver.FindElement(By.XPath("//input[@type='submit' and @value='Thêm mới sản phẩm']"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", submitButton);

            Assert.AreEqual(initialUrl, driver.Url, "❌ Test Failed: Hệ thống đã chuyển hướng, tức là sản phẩm vẫn được thêm!");

           
        }

        [Test, Order(14)]
        public void AddProductSmallPrice()
        {
            Login("Admin@gmail.com", "12345678");

            IWebElement addButton = wait.Until(d => d.FindElement(By.XPath("//p/button[@class='btn-default']/a")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", addButton);

            driver.FindElement(By.Id("Tensp")).SendKeys("Điện thoại Xiaomi Redmi K20 Pro");
            driver.FindElement(By.Id("Giatien")).SendKeys("1"); 
            driver.FindElement(By.Id("Soluong")).SendKeys("100");
            driver.FindElement(By.Id("Mota")).SendKeys("Sản phẩm hot");
            driver.FindElement(By.Id("Thesim")).SendKeys("2");
            driver.FindElement(By.Id("Bonhotrong")).SendKeys("128");
            driver.FindElement(By.Id("Ram")).SendKeys("8");

            new SelectElement(driver.FindElement(By.Id("Sanphammoi"))).SelectByText("False");
            new SelectElement(driver.FindElement(By.Id("Mahang"))).SelectByText("Sam Sung");
            new SelectElement(driver.FindElement(By.Id("Mahdh"))).SelectByText("Android");

            driver.FindElement(By.Id("Anhbia")).SendKeys("/Images/files/ss3.jpg");

            string initialUrl = driver.Url;
            Console.WriteLine($"🔍 URL trước khi submit: {initialUrl}");

            IWebElement submitButton = driver.FindElement(By.XPath("//input[@type='submit' and @value='Thêm mới sản phẩm']"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", submitButton);


            Assert.AreEqual(initialUrl, driver.Url, "❌ Test Failed: Hệ thống đã chuyển hướng, tức là sản phẩm vẫn được thêm!");

        }


        [Test, Order(15)]
        public void AddProductBigPrice()
        {
            Login("Admin@gmail.com", "12345678");

            IWebElement addButton = wait.Until(d => d.FindElement(By.XPath("//p/button[@class='btn-default']/a")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", addButton);

            driver.FindElement(By.Id("Tensp")).SendKeys("Điện thoại Xiaomi Redmi K20 Pro");
            driver.FindElement(By.Id("Giatien")).SendKeys("600000000000000000000"); 
            driver.FindElement(By.Id("Soluong")).SendKeys("500");
            driver.FindElement(By.Id("Mota")).SendKeys("Sản phẩm hot");
            driver.FindElement(By.Id("Thesim")).SendKeys("2");
            driver.FindElement(By.Id("Bonhotrong")).SendKeys("128");
            driver.FindElement(By.Id("Ram")).SendKeys("8");

            new SelectElement(driver.FindElement(By.Id("Sanphammoi"))).SelectByText("False");
            new SelectElement(driver.FindElement(By.Id("Mahang"))).SelectByText("Sam Sung");
            new SelectElement(driver.FindElement(By.Id("Mahdh"))).SelectByText("Android");

            driver.FindElement(By.Id("Anhbia")).SendKeys("/Images/files/ss3.jpg");

            string initialUrl = driver.Url;
            Console.WriteLine($"🔍 URL trước khi submit: {initialUrl}");

            IWebElement submitButton = driver.FindElement(By.XPath("//input[@type='submit' and @value='Thêm mới sản phẩm']"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", submitButton);

            Assert.AreEqual(initialUrl, driver.Url, "❌ Test Failed: Hệ thống đã chuyển hướng, tức là sản phẩm vẫn được thêm!");

        }


        [Test, Order(16)]
        public void AddProduct5Sim()
        {
            Login("Admin@gmail.com", "12345678");

            IWebElement addButton = wait.Until(d => d.FindElement(By.XPath("//p/button[@class='btn-default']/a")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", addButton);

            driver.FindElement(By.Id("Tensp")).SendKeys("Điện thoại Xiaomi Redmi K20 Pro");
            driver.FindElement(By.Id("Giatien")).SendKeys("600000000000");
            driver.FindElement(By.Id("Soluong")).SendKeys("5000");
            driver.FindElement(By.Id("Mota")).SendKeys("Sản phẩm hot");
            driver.FindElement(By.Id("Thesim")).SendKeys("5"); 
            driver.FindElement(By.Id("Bonhotrong")).SendKeys("128");
            driver.FindElement(By.Id("Ram")).SendKeys("8");

            new SelectElement(driver.FindElement(By.Id("Sanphammoi"))).SelectByText("False");
            new SelectElement(driver.FindElement(By.Id("Mahang"))).SelectByText("Sam Sung");
            new SelectElement(driver.FindElement(By.Id("Mahdh"))).SelectByText("Android");

            driver.FindElement(By.Id("Anhbia")).SendKeys("/Images/files/ss3.jpg");

            string initialUrl = driver.Url;
            Console.WriteLine($"🔍 URL trước khi submit: {initialUrl}");

            IWebElement submitButton = driver.FindElement(By.XPath("//input[@type='submit' and @value='Thêm mới sản phẩm']"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", submitButton);


            Assert.AreEqual(initialUrl, driver.Url, "❌ Test Failed: Hệ thống đã chuyển hướng, tức là sản phẩm vẫn được thêm!");

        }


        [Test, Order(17)]
        public void AddProductNoSim()
        {
            Login("Admin@gmail.com", "12345678");

            IWebElement addButton = wait.Until(d => d.FindElement(By.XPath("//p/button[@class='btn-default']/a")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", addButton);

            driver.FindElement(By.Id("Tensp")).SendKeys("Điện thoại Xiaomi Redmi K20 Pro");
            driver.FindElement(By.Id("Giatien")).SendKeys("600000000000");
            driver.FindElement(By.Id("Soluong")).SendKeys("5000");
            driver.FindElement(By.Id("Mota")).SendKeys("Sản phẩm hot");
            driver.FindElement(By.Id("Bonhotrong")).SendKeys("128");
            driver.FindElement(By.Id("Ram")).SendKeys("8");

            new SelectElement(driver.FindElement(By.Id("Sanphammoi"))).SelectByText("False");
            new SelectElement(driver.FindElement(By.Id("Mahang"))).SelectByText("Sam Sung");
            new SelectElement(driver.FindElement(By.Id("Mahdh"))).SelectByText("Android");
            driver.FindElement(By.Id("Anhbia")).SendKeys("/Images/files/ss3.jpg");

            string initialUrl = driver.Url;
            Console.WriteLine($"🔍 URL trước khi submit: {initialUrl}");

            IWebElement submitButton = driver.FindElement(By.XPath("//input[@type='submit' and @value='Thêm mới sản phẩm']"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", submitButton);


            Assert.AreEqual(initialUrl, driver.Url, "❌ Test Failed: Hệ thống đã chuyển hướng, tức là sản phẩm vẫn được thêm!");

        }


        [Test, Order(18)]
        public void AddProductNegativeSim()
        {
            Login("Admin@gmail.com", "12345678");

            IWebElement addButton = wait.Until(d => d.FindElement(By.XPath("//p/button[@class='btn-default']/a")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", addButton);

            driver.FindElement(By.Id("Tensp")).SendKeys("Điện thoại Xiaomi Redmi K20 Pro");
            driver.FindElement(By.Id("Giatien")).SendKeys("600000000000");
            driver.FindElement(By.Id("Soluong")).SendKeys("5000");
            driver.FindElement(By.Id("Thesim")).SendKeys("-1"); 
            driver.FindElement(By.Id("Mota")).SendKeys("Sản phẩm hot");
            driver.FindElement(By.Id("Bonhotrong")).SendKeys("128");
            driver.FindElement(By.Id("Ram")).SendKeys("8");

            new SelectElement(driver.FindElement(By.Id("Sanphammoi"))).SelectByText("False");
            new SelectElement(driver.FindElement(By.Id("Mahang"))).SelectByText("Sam Sung");
            new SelectElement(driver.FindElement(By.Id("Mahdh"))).SelectByText("Android");

            driver.FindElement(By.Id("Anhbia")).SendKeys("/Images/files/ss3.jpg");

            string initialUrl = driver.Url;
            Console.WriteLine($"🔍 URL trước khi submit: {initialUrl}");

            IWebElement submitButton = driver.FindElement(By.XPath("//input[@type='submit' and @value='Thêm mới sản phẩm']"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", submitButton);


            Assert.AreEqual(initialUrl, driver.Url, "❌ Test Failed: Hệ thống đã chuyển hướng, tức là sản phẩm vẫn được thêm!");

        }

        [Test, Order(19)]
        public void AddProductSimWithOperation()
        {
            Login("Admin@gmail.com", "12345678");

            IWebElement addButton = wait.Until(d => d.FindElement(By.XPath("//p/button[@class='btn-default']/a")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", addButton);

            driver.FindElement(By.Id("Tensp")).SendKeys("Điện thoại Xiaomi Redmi K20 Pro");
            driver.FindElement(By.Id("Giatien")).SendKeys("600000000000");
            driver.FindElement(By.Id("Soluong")).SendKeys("500");
            driver.FindElement(By.Id("Mota")).SendKeys("Sản phẩm hot");
            driver.FindElement(By.Id("Thesim")).SendKeys("0+5"); 
            driver.FindElement(By.Id("Bonhotrong")).SendKeys("128");
            driver.FindElement(By.Id("Ram")).SendKeys("8");

            new SelectElement(driver.FindElement(By.Id("Sanphammoi"))).SelectByText("False");
            new SelectElement(driver.FindElement(By.Id("Mahang"))).SelectByText("Sam Sung");
            new SelectElement(driver.FindElement(By.Id("Mahdh"))).SelectByText("Android");

            driver.FindElement(By.Id("Anhbia")).SendKeys("/Images/files/ss3.jpg");

            string initialUrl = driver.Url;
            Console.WriteLine($"🔍 URL trước khi submit: {initialUrl}");

            IWebElement submitButton = driver.FindElement(By.XPath("//input[@type='submit' and @value='Thêm mới sản phẩm']"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", submitButton);


            Assert.AreEqual(initialUrl, driver.Url, "❌ Test Failed: Hệ thống đã chuyển hướng, tức là sản phẩm vẫn được thêm!");

        }


        [Test, Order(20)]
        public void AddProductNegativeInternalMemory()
        {
            Login("Admin@gmail.com", "12345678");

            IWebElement addButton = wait.Until(d => d.FindElement(By.XPath("//p/button[@class='btn-default']/a")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", addButton);

            driver.FindElement(By.Id("Tensp")).SendKeys("Điện thoại Xiaomi Redmi K20 Pro");
            driver.FindElement(By.Id("Giatien")).SendKeys("600000000000");
            driver.FindElement(By.Id("Soluong")).SendKeys("5000");
            driver.FindElement(By.Id("Thesim")).SendKeys("2");
            driver.FindElement(By.Id("Mota")).SendKeys("Sản phẩm hot");
            driver.FindElement(By.Id("Bonhotrong")).SendKeys("0"); 
            driver.FindElement(By.Id("Ram")).SendKeys("8");

            new SelectElement(driver.FindElement(By.Id("Sanphammoi"))).SelectByText("False");
            new SelectElement(driver.FindElement(By.Id("Mahang"))).SelectByText("Sam Sung");
            new SelectElement(driver.FindElement(By.Id("Mahdh"))).SelectByText("Android");

            driver.FindElement(By.Id("Anhbia")).SendKeys("/Images/files/ss3.jpg");

            string initialUrl = driver.Url;
            Console.WriteLine($"🔍 URL trước khi submit: {initialUrl}");

            IWebElement submitButton = driver.FindElement(By.XPath("//input[@type='submit' and @value='Thêm mới sản phẩm']"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", submitButton);


            Assert.AreEqual(initialUrl, driver.Url, "❌ Test Failed: Hệ thống đã chuyển hướng, tức là sản phẩm vẫn được thêm!");

        }


        [Test, Order(21)]
        public void AddProductNoInternalMemory()
        {
            Login("Admin@gmail.com", "12345678");

            IWebElement addButton = wait.Until(d => d.FindElement(By.XPath("//p/button[@class='btn-default']/a")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", addButton);

            driver.FindElement(By.Id("Tensp")).SendKeys("Điện thoại Xiaomi Redmi K20 Pro");
            driver.FindElement(By.Id("Giatien")).SendKeys("600000000000");
            driver.FindElement(By.Id("Soluong")).SendKeys("5000");
            driver.FindElement(By.Id("Mota")).SendKeys("Sản phẩm hot");
            driver.FindElement(By.Id("Ram")).SendKeys("8");

            new SelectElement(driver.FindElement(By.Id("Sanphammoi"))).SelectByText("False");
            new SelectElement(driver.FindElement(By.Id("Mahang"))).SelectByText("Sam Sung");
            new SelectElement(driver.FindElement(By.Id("Mahdh"))).SelectByText("Android");

            driver.FindElement(By.Id("Anhbia")).SendKeys("/Images/files/ss3.jpg");

            string initialUrl = driver.Url;
            Console.WriteLine($"🔍 URL trước khi submit: {initialUrl}");

            IWebElement submitButton = driver.FindElement(By.XPath("//input[@type='submit' and @value='Thêm mới sản phẩm']"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", submitButton);


            Assert.AreEqual(initialUrl, driver.Url, "❌ Test Failed: Hệ thống đã chuyển hướng, tức là sản phẩm vẫn được thêm!");

        }


        [Test, Order(22)]
        public void AddProductInternalMemoryWithOperation()
        {
            Login("Admin@gmail.com", "12345678");

            IWebElement addButton = wait.Until(d => d.FindElement(By.XPath("//p/button[@class='btn-default']/a")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", addButton);

            driver.FindElement(By.Id("Tensp")).SendKeys("Điện thoại Xiaomi Redmi K20 Pro");
            driver.FindElement(By.Id("Giatien")).SendKeys("600000000000");
            driver.FindElement(By.Id("Soluong")).SendKeys("5000");
            driver.FindElement(By.Id("Mota")).SendKeys("Sản phẩm hot");
            driver.FindElement(By.Id("Thesim")).SendKeys("2");
            driver.FindElement(By.Id("Bonhotrong")).SendKeys("5+6");  
            driver.FindElement(By.Id("Ram")).SendKeys("8");

            new SelectElement(driver.FindElement(By.Id("Sanphammoi"))).SelectByText("False");
            new SelectElement(driver.FindElement(By.Id("Mahang"))).SelectByText("Sam Sung");
            new SelectElement(driver.FindElement(By.Id("Mahdh"))).SelectByText("Android");

            driver.FindElement(By.Id("Anhbia")).SendKeys("/Images/files/ss3.jpg");

            string initialUrl = driver.Url;
            Console.WriteLine($"🔍 URL trước khi submit: {initialUrl}");

            IWebElement submitButton = driver.FindElement(By.XPath("//input[@type='submit' and @value='Thêm mới sản phẩm']"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", submitButton);


            Assert.AreEqual(initialUrl, driver.Url, "❌ Test Failed: Hệ thống đã chuyển hướng, tức là sản phẩm vẫn được thêm!");

        }


        [Test, Order(23)]
        public void AddProductFractionalInternalMemory()
        {
            Login("Admin@gmail.com", "12345678");
            IWebElement addButton = wait.Until(d => d.FindElement(By.XPath("//p/button[@class='btn-default']/a")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", addButton);

            driver.FindElement(By.Id("Tensp")).SendKeys("Điện thoại Xiaomi Redmi K20 Pro");
            driver.FindElement(By.Id("Giatien")).SendKeys("600000000000");
            driver.FindElement(By.Id("Soluong")).SendKeys("5000");
            driver.FindElement(By.Id("Mota")).SendKeys("Sản phẩm hot");
            driver.FindElement(By.Id("Thesim")).SendKeys("2");
            driver.FindElement(By.Id("Bonhotrong")).SendKeys("1,83");  
            driver.FindElement(By.Id("Ram")).SendKeys("8");

            new SelectElement(driver.FindElement(By.Id("Sanphammoi"))).SelectByText("False");
            new SelectElement(driver.FindElement(By.Id("Mahang"))).SelectByText("Sam Sung");
            new SelectElement(driver.FindElement(By.Id("Mahdh"))).SelectByText("Android");

            driver.FindElement(By.Id("Anhbia")).SendKeys("/Images/files/ss3.jpg");

            string initialUrl = driver.Url;
            Console.WriteLine($"🔍 URL trước khi submit: {initialUrl}");

            IWebElement submitButton = driver.FindElement(By.XPath("//input[@type='submit' and @value='Thêm mới sản phẩm']"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", submitButton);


            Assert.AreEqual(initialUrl, driver.Url, "❌ Test Failed: Hệ thống đã chuyển hướng, tức là sản phẩm vẫn được thêm!");

        }

        [Test, Order(24)]
        public void AddProductNoRam()
        {
            Login("Admin@gmail.com", "12345678");

            IWebElement addButton = wait.Until(d => d.FindElement(By.XPath("//p/button[@class='btn-default']/a")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", addButton);

            driver.FindElement(By.Id("Tensp")).SendKeys("Điện thoại Xiaomi Redmi K20 Pro");
            driver.FindElement(By.Id("Giatien")).SendKeys("600000000000");
            driver.FindElement(By.Id("Soluong")).SendKeys("5000");
            driver.FindElement(By.Id("Bonhotrong")).SendKeys("128");
            driver.FindElement(By.Id("Mota")).SendKeys("Sản phẩm hot");

            new SelectElement(driver.FindElement(By.Id("Sanphammoi"))).SelectByText("False");
            new SelectElement(driver.FindElement(By.Id("Mahang"))).SelectByText("Sam Sung");
            new SelectElement(driver.FindElement(By.Id("Mahdh"))).SelectByText("Android");

            driver.FindElement(By.Id("Anhbia")).SendKeys("/Images/files/ss3.jpg");

            string initialUrl = driver.Url;
            Console.WriteLine($"🔍 URL trước khi submit: {initialUrl}");

            IWebElement submitButton = driver.FindElement(By.XPath("//input[@type='submit' and @value='Thêm mới sản phẩm']"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", submitButton);


            Assert.AreEqual(initialUrl, driver.Url, "❌ Test Failed: Hệ thống đã chuyển hướng, tức là sản phẩm vẫn được thêm!");

        }

        [Test, Order(25)]
        public void AddProductNegativeRam()
        {
            Login("Admin@gmail.com", "12345678");

            IWebElement addButton = wait.Until(d => d.FindElement(By.XPath("//p/button[@class='btn-default']/a")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", addButton);

            driver.FindElement(By.Id("Tensp")).SendKeys("Điện thoại Xiaomi Redmi K20 Pro");
            driver.FindElement(By.Id("Giatien")).SendKeys("600000000000");
            driver.FindElement(By.Id("Soluong")).SendKeys("5000");
            driver.FindElement(By.Id("Thesim")).SendKeys("2");
            driver.FindElement(By.Id("Mota")).SendKeys("Sản phẩm hot");
            driver.FindElement(By.Id("Bonhotrong")).SendKeys("128");
            driver.FindElement(By.Id("Ram")).SendKeys("0"); 

            new SelectElement(driver.FindElement(By.Id("Sanphammoi"))).SelectByText("False");
            new SelectElement(driver.FindElement(By.Id("Mahang"))).SelectByText("Sam Sung");
            new SelectElement(driver.FindElement(By.Id("Mahdh"))).SelectByText("Android");

            driver.FindElement(By.Id("Anhbia")).SendKeys("/Images/files/ss3.jpg");

            string initialUrl = driver.Url;
            Console.WriteLine($"🔍 URL trước khi submit: {initialUrl}");

            IWebElement submitButton = driver.FindElement(By.XPath("//input[@type='submit' and @value='Thêm mới sản phẩm']"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", submitButton);


            Assert.AreEqual(initialUrl, driver.Url, "❌ Test Failed: Hệ thống đã chuyển hướng, tức là sản phẩm vẫn được thêm!");

        }

        [Test, Order(26)]
        public void AddProductFractionalRam()
        {
            Login("Admin@gmail.com", "12345678");

            IWebElement addButton = wait.Until(d => d.FindElement(By.XPath("//p/button[@class='btn-default']/a")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", addButton);

            driver.FindElement(By.Id("Tensp")).SendKeys("Điện thoại Xiaomi Redmi K20 Pro");
            driver.FindElement(By.Id("Giatien")).SendKeys("600000000000");
            driver.FindElement(By.Id("Soluong")).SendKeys("5000");
            driver.FindElement(By.Id("Mota")).SendKeys("Sản phẩm hot");
            driver.FindElement(By.Id("Thesim")).SendKeys("2");
            driver.FindElement(By.Id("Bonhotrong")).SendKeys("128");
            driver.FindElement(By.Id("Ram")).SendKeys("0.83"); 

            new SelectElement(driver.FindElement(By.Id("Sanphammoi"))).SelectByText("False");
            new SelectElement(driver.FindElement(By.Id("Mahang"))).SelectByText("Sam Sung");
            new SelectElement(driver.FindElement(By.Id("Mahdh"))).SelectByText("Android");

            driver.FindElement(By.Id("Anhbia")).SendKeys("/Images/files/ss3.jpg");

            string initialUrl = driver.Url;
            Console.WriteLine($"🔍 URL trước khi submit: {initialUrl}");

            IWebElement submitButton = driver.FindElement(By.XPath("//input[@type='submit' and @value='Thêm mới sản phẩm']"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", submitButton);


            Assert.AreEqual(initialUrl, driver.Url, "❌ Test Failed: Hệ thống đã chuyển hướng, tức là sản phẩm vẫn được thêm!");

        }


        [Test, Order(27)]
        public void AddProductRamWithOperation()
        {
            Login("Admin@gmail.com", "12345678");

            IWebElement addButton = wait.Until(d => d.FindElement(By.XPath("//p/button[@class='btn-default']/a")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", addButton);

            driver.FindElement(By.Id("Tensp")).SendKeys("Điện thoại Xiaomi Redmi K20 Pro");
            driver.FindElement(By.Id("Giatien")).SendKeys("600000000000");
            driver.FindElement(By.Id("Soluong")).SendKeys("5000");
            driver.FindElement(By.Id("Mota")).SendKeys("Sản phẩm hot");
            driver.FindElement(By.Id("Thesim")).SendKeys("2");
            driver.FindElement(By.Id("Bonhotrong")).SendKeys("128");
            driver.FindElement(By.Id("Ram")).SendKeys("5+6"); 

            new SelectElement(driver.FindElement(By.Id("Sanphammoi"))).SelectByText("False");
            new SelectElement(driver.FindElement(By.Id("Mahang"))).SelectByText("Sam Sung");
            new SelectElement(driver.FindElement(By.Id("Mahdh"))).SelectByText("Android");

            driver.FindElement(By.Id("Anhbia")).SendKeys("/Images/files/ss3.jpg");
            string initialUrl = driver.Url;
            Console.WriteLine($"🔍 URL trước khi submit: {initialUrl}");

            IWebElement submitButton = driver.FindElement(By.XPath("//input[@type='submit' and @value='Thêm mới sản phẩm']"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", submitButton);


            Assert.AreEqual(initialUrl, driver.Url, "❌ Test Failed: Hệ thống đã chuyển hướng, tức là sản phẩm vẫn được thêm!");

        }

        [TearDown]
        public void Cleanup()
        {
            if (driver != null)
            {
                driver.Quit();
                driver.Dispose();
                driver = null;
            }
            string status = TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Passed ? "Passed" : "Failed";
            UpdateTestResult(filePath, currentRow, status);
            currentRow++; 
        }
        private void UpdateTestResult(string filePath, int row, string status)
        {
            int statusColumn = 8; 

           
            while (IsFileLocked(filePath))
            {
                Console.WriteLine("⏳ File đang bị khóa, chờ 2 giây...");
                Thread.Sleep(2000);
            }

            try
            {
                using (var workbook = new XLWorkbook(filePath))
                {
                    var worksheet = workbook.Worksheet(1);

                    worksheet.Cell(row, statusColumn).Value = status;
                    workbook.Save();
                } // ✅ Workbook sẽ tự động đóng khi ra khỏi `using`

                Console.WriteLine($"✅ Ghi kết quả vào Excel: Dòng {row}, Trạng thái: {status}");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"❌ Lỗi: Không thể ghi vào file Excel! Chi tiết: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi không xác định: {ex.Message}");
            }
        }

        private bool IsFileLocked(string filePath)
        {
            try
            {
                using (FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    return false; // File không bị khóa
                }
            }
            catch (IOException)
            {
                return true; // File đang bị khóa
            }
        }


    }
}
