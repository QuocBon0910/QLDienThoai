using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;

namespace TestIctShop
{
    [TestFixture]
    public class AdminProductTests
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            driver.Manage().Window.Maximize();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        [Test]
        public void AdminProductManagementTest()
        {
            try
            {
                // 1️⃣ **Mở trang đăng nhập**
                driver.Navigate().GoToUrl("https://localhost:44322/User/DangNhap");

                // 2️⃣ **Nhập email và mật khẩu**
                driver.FindElement(By.Id("userMail")).SendKeys("Admin@gmail.com");
                driver.FindElement(By.Id("password")).SendKeys("12345678");
                driver.FindElement(By.XPath("//input[@value='Đăng nhập']")).Click();

                wait.Until(d => d.Url.Contains("Admin/Home"));

                // 3️⃣ **Nhấn vào nút 'Thêm mới' để mở form nhập sản phẩm**
                IWebElement addButton = wait.Until(d => d.FindElement(By.XPath("//p/button[@class='btn-default']/a")));
                addButton.Click();

                // 4️⃣ **Chờ form tải xong và nhập thông tin sản phẩm**
                wait.Until(d => d.FindElement(By.Id("Tensp"))).SendKeys("Samsung Galaxy S25");
                driver.FindElement(By.Id("Giatien")).SendKeys("25000000");
                driver.FindElement(By.Id("Soluong")).SendKeys("10");
                driver.FindElement(By.Id("Mota")).SendKeys("Điện thoại Samsung cao cấp");
                driver.FindElement(By.Id("Thesim")).SendKeys("2");  // Nhập số sim
                driver.FindElement(By.Id("Bonhotrong")).SendKeys("512"); // Nhập bộ nhớ trong
                driver.FindElement(By.Id("Ram")).SendKeys("12"); // Nhập dung lượng RAM

                // 5️⃣ **Chọn trạng thái sản phẩm mới**
                IWebElement dropdownNewProduct = driver.FindElement(By.Id("Sanphammoi"));
                SelectElement selectNewProduct = new SelectElement(dropdownNewProduct);
                selectNewProduct.SelectByText("True"); // Chọn "Có" hoặc "Yes" tùy vào ngôn ngữ trang web

                // 6️⃣ **Upload ảnh sản phẩm**
                IWebElement uploadFile = driver.FindElement(By.Id("Anhbia"));
                uploadFile.SendKeys("/Images/files/mi31.jpg"); // Đổi đường dẫn ảnh phù hợp với hệ thống của bạn

                // 7️⃣ **Chọn hãng điện thoại và hệ điều hành**
                SelectElement selectBrand = new SelectElement(driver.FindElement(By.Id("Mahang")));
                selectBrand.SelectByText("Apple");

                SelectElement selectOS = new SelectElement(driver.FindElement(By.Id("Mahdh")));
                selectOS.SelectByText("IOS");

                // 8️⃣ Nhấn nút 'Thêm mới sản phẩm'
                driver.FindElement(By.XPath("//input[@type='submit' and @value='Thêm mới sản phẩm']")).Click();

                //// Chờ cho trang chuyển hướng về Admin/Home
                //wait.Until(d => d.Url.Contains("Admin/Home"));

                // Chờ sản phẩm xuất hiện trong bảng
                wait.Until(d => d.FindElements(By.XPath("//table[contains(@class,'table-bordered')]//tr/td[1]"))
                                 .Any(e => e.Text.Contains("Samsung Galaxy S25")));

                // In ra thông báo kiểm tra thành công
                Console.WriteLine("Sản phẩm Samsung Galaxy S25 đã xuất hiện trong bảng!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Lỗi trong quá trình kiểm thử: " + ex.Message);
                throw;
            }
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
        }
    }
}
