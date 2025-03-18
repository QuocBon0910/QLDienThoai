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
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
        }

        private void Login(string email, string password)
        {
            driver.Navigate().GoToUrl("https://localhost:44322/User/DangNhap");
            driver.FindElement(By.Id("userMail")).SendKeys(email);
            driver.FindElement(By.Id("password")).SendKeys(password);
            driver.FindElement(By.XPath("//input[@value='Đăng nhập']")).Click();
            wait.Until(d => d.Url.Contains("Admin/Home"));
        }

        [Test]
        public void AddProductNoName()
        {
            try
            {
                // Đăng nhập với tài khoản Admin
                Login("Admin@gmail.com", "12345678");

                // Nhấn vào nút 'Thêm mới'
                IWebElement addButton = WaitForElement(By.XPath("//p/button[@class='btn-default']/a"), 10);
                addButton.Click();

                // Nhập thông tin sản phẩm nhưng để tên trống
                driver.FindElement(By.Id("Tensp")).SendKeys("");
                driver.FindElement(By.Id("Giatien")).SendKeys("5000000");
                driver.FindElement(By.Id("Soluong")).SendKeys("100");
                driver.FindElement(By.Id("Mota")).SendKeys("Sản phẩm hot");
                driver.FindElement(By.Id("Thesim")).SendKeys("2");
                driver.FindElement(By.Id("Bonhotrong")).SendKeys("128");
                driver.FindElement(By.Id("Ram")).SendKeys("8");

                // Chọn trạng thái sản phẩm mới
                new SelectElement(driver.FindElement(By.Id("Sanphammoi"))).SelectByText("False");

                // Upload ảnh sản phẩm
                IWebElement uploadFile = driver.FindElement(By.Id("Anhbia"));
                uploadFile.SendKeys("/Images/files/ss3.jpg");

                // Chọn hãng điện thoại và hệ điều hành
                new SelectElement(driver.FindElement(By.Id("Mahang"))).SelectByText("Sam Sung");
                new SelectElement(driver.FindElement(By.Id("Mahdh"))).SelectByText("Android");

                // Nhấn nút 'Thêm mới sản phẩm'
                IWebElement submitButton = driver.FindElement(By.XPath("//input[@type='submit' and @value='Thêm mới sản phẩm']"));
                submitButton.Click();

                // Kiểm tra nếu có thông báo lỗi xuất hiện
                bool isErrorDisplayed = driver.FindElements(By.ClassName("text-danger")).Count > 0;

                // Kiểm tra xem sản phẩm có xuất hiện trong bảng Admin/Home không
                bool isProductInTable = driver.FindElements(By.XPath("//table[contains(@class,'table-bordered')]//tr")).Count > 0;

                if (isErrorDisplayed || !isProductInTable)
                {
                    Console.WriteLine("✅ Test Passed: Sản phẩm không hợp lệ, không được thêm vào danh sách.");
                }
                else
                {
                    Console.WriteLine("❌ Test Failed: Sản phẩm không có tên nhưng vẫn xuất hiện trong bảng!");
                    Assert.Fail("Sản phẩm không hợp lệ nhưng vẫn được thêm!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Lỗi trong quá trình kiểm thử: " + ex.Message);
                throw;
            }
        }
        [Test]
        public void AddProduct()
        {
            try
            {
                // 🟢 Bước 1: Đăng nhập với tài khoản Admin
                Login("Admin@gmail.com", "12345678");

                // 🟢 Bước 2: Chờ và click vào nút 'Thêm mới'
                IWebElement addButton = wait.Until(d => d.FindElement(By.XPath("//p/button[@class='btn-default']/a")));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", addButton);

                // 🟢 Bước 3: Nhập thông tin sản phẩm hợp lệ
                string productName = "Xiaomi Redmi K20 Pro";
                driver.FindElement(By.Id("Tensp")).SendKeys(productName);
                driver.FindElement(By.Id("Giatien")).SendKeys("5000000");
                driver.FindElement(By.Id("Soluong")).SendKeys("100");
                driver.FindElement(By.Id("Mota")).SendKeys("Sản phẩm hot");
                driver.FindElement(By.Id("Thesim")).SendKeys("2");
                driver.FindElement(By.Id("Bonhotrong")).SendKeys("128");
                driver.FindElement(By.Id("Ram")).SendKeys("8");

                // 🟢 Bước 4: Chọn trạng thái, hãng, hệ điều hành
                new SelectElement(driver.FindElement(By.Id("Sanphammoi"))).SelectByText("False");
                new SelectElement(driver.FindElement(By.Id("Mahang"))).SelectByText("Sam Sung");
                new SelectElement(driver.FindElement(By.Id("Mahdh"))).SelectByText("Android");

                // 🟢 Bước 5: Upload ảnh sản phẩm
                driver.FindElement(By.Id("Anhbia")).SendKeys("/Images/files/ss3.jpg");

                // 🟢 Bước 6: Nhấn nút 'Thêm mới sản phẩm'
                IWebElement submitButton = driver.FindElement(By.XPath("//input[@type='submit' and @value='Thêm mới sản phẩm']"));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", submitButton);

                // 🟢 Bước 7: Chờ trang chuyển về Admin/Home
                WaitForUrl("Admin/Home", 10);

                // 🟢 Bước 8: Kiểm tra sản phẩm có xuất hiện trong bảng tại Admin/Home không
                IWebElement productCell = WaitForElement(By.XPath($"//table[contains(@class,'table-bordered')]//td[contains(text(), '{productName}')]"), 10);
                Assert.IsNotNull(productCell, "❌ Test Failed: Sản phẩm không xuất hiện trong bảng trên trang Admin/Home.");

                Console.WriteLine("✅ Test Passed: Sản phẩm đã được thêm thành công và hiển thị trên Admin/Home!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Lỗi trong quá trình kiểm thử: " + ex.Message);
                throw;
            }
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
            for (int i = 0; i < timeoutInSeconds * 2; i++)  // Kiểm tra mỗi 0.5 giây
            {
                try
                {
                    var elements = driver.FindElements(by);
                    if (elements.Count > 0 && elements[0].Displayed)
                    {
                        return elements[0]; // Trả về phần tử đầu tiên tìm thấy
                    }
                }
                catch (Exception) { }

                Thread.Sleep(500); // Chờ 0.5 giây trước khi kiểm tra lại
            }

            throw new TimeoutException($"❌ Lỗi: Không tìm thấy phần tử {by} sau {timeoutInSeconds} giây.");
        }

        [Test]
        public void AddProduct_CheckDuplicate()
        {
            try
            {
                // 🟢 Bước 1: Đăng nhập với tài khoản Admin
                Login("Admin@gmail.com", "12345678");

                // 🟢 Bước 2: Kiểm tra số lần xuất hiện của sản phẩm trước khi thêm
                string productName = "Xiaomi Redmi K20 Pro";
                int initialCount = driver.FindElements(By.XPath($"//table[contains(@class,'table-bordered')]//td[contains(text(), '{productName}')]")).Count;

                // 🟢 Bước 3: Click vào nút 'Thêm mới'
                IWebElement addButton = wait.Until(d => d.FindElement(By.XPath("//p/button[@class='btn-default']/a")));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", addButton);

                // 🟢 Bước 4: Nhập thông tin sản phẩm
                driver.FindElement(By.Id("Tensp")).SendKeys(productName);
                driver.FindElement(By.Id("Giatien")).SendKeys("5000000");
                driver.FindElement(By.Id("Soluong")).SendKeys("100");
                driver.FindElement(By.Id("Mota")).SendKeys("Sản phẩm hot");
                driver.FindElement(By.Id("Thesim")).SendKeys("2");
                driver.FindElement(By.Id("Bonhotrong")).SendKeys("128");
                driver.FindElement(By.Id("Ram")).SendKeys("8");

                // 🟢 Bước 5: Chọn các giá trị dropdown
                new SelectElement(driver.FindElement(By.Id("Sanphammoi"))).SelectByText("False");
                new SelectElement(driver.FindElement(By.Id("Mahang"))).SelectByText("Sam Sung");
                new SelectElement(driver.FindElement(By.Id("Mahdh"))).SelectByText("Android");

                // 🟢 Bước 6: Upload ảnh
                driver.FindElement(By.Id("Anhbia")).SendKeys("/Images/files/ss3.jpg");

                // 🟢 Bước 7: Nhấn nút 'Thêm mới sản phẩm'
                IWebElement submitButton = driver.FindElement(By.XPath("//input[@type='submit' and @value='Thêm mới sản phẩm']"));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", submitButton);

                // 🟢 Bước 8: Kiểm tra có thông báo lỗi "Tên sản phẩm đã tồn tại" hay không
                try
                {
                    IWebElement errorMessage = wait.Until(d => d.FindElement(By.XPath("//div[contains(@class,'alert-danger') and contains(text(),'Tên sản phẩm đã tồn tại')]")));
                    Console.WriteLine("✅ Test Passed: Hệ thống hiển thị thông báo lỗi khi thêm trùng tên.");
                    return; // Kết thúc test do hệ thống hoạt động đúng
                }
                catch (WebDriverTimeoutException)
                {
                    // Nếu không có thông báo lỗi, tiếp tục kiểm tra danh sách sản phẩm
                }

                // 🟢 Bước 9: Chờ trang Admin/Home tải lại
                WaitForUrl("Admin/Home", 10);

                // 🟢 Bước 10: Kiểm tra số lần xuất hiện của sản phẩm sau khi thêm
                int finalCount = driver.FindElements(By.XPath($"//table[contains(@class,'table-bordered')]//td[contains(text(), '{productName}')]")).Count;

                if (finalCount > initialCount)
                {
                    Console.WriteLine("❌ Test Failed: Sản phẩm đã bị thêm trùng nhưng hệ thống không báo lỗi!");
                    Assert.Fail("Hệ thống cho phép thêm sản phẩm trùng tên.");
                }
                else
                {
                    Console.WriteLine("✅ Test Passed: Hệ thống không cho phép thêm sản phẩm trùng.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Lỗi trong quá trình kiểm thử: " + ex.Message);
                throw;
            }
        }

        [Test]
        public void AddProductVietnameseName()
        {
            try
            {
                // 🟢 Bước 1: Đăng nhập với tài khoản Admin
                Login("Admin@gmail.com", "12345678");

                // 🟢 Bước 2: Chờ và click vào nút 'Thêm mới'
                IWebElement addButton = wait.Until(d => d.FindElement(By.XPath("//p/button[@class='btn-default']/a")));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", addButton);

                // 🟢 Bước 3: Nhập thông tin sản phẩm hợp lệ
                string productName = "Điện thoại Xiaomi Redmi K20 Pro";
                driver.FindElement(By.Id("Tensp")).SendKeys(productName);
                driver.FindElement(By.Id("Giatien")).SendKeys("5000000");
                driver.FindElement(By.Id("Soluong")).SendKeys("100");
                driver.FindElement(By.Id("Mota")).SendKeys("Sản phẩm hot");
                driver.FindElement(By.Id("Thesim")).SendKeys("2");
                driver.FindElement(By.Id("Bonhotrong")).SendKeys("128");
                driver.FindElement(By.Id("Ram")).SendKeys("8");

                // 🟢 Bước 4: Chọn trạng thái, hãng, hệ điều hành
                new SelectElement(driver.FindElement(By.Id("Sanphammoi"))).SelectByText("False");
                new SelectElement(driver.FindElement(By.Id("Mahang"))).SelectByText("Sam Sung");
                new SelectElement(driver.FindElement(By.Id("Mahdh"))).SelectByText("Android");

                // 🟢 Bước 5: Upload ảnh sản phẩm
                driver.FindElement(By.Id("Anhbia")).SendKeys("/Images/files/ss3.jpg");

                // 🟢 Bước 6: Nhấn nút 'Thêm mới sản phẩm'
                IWebElement submitButton = driver.FindElement(By.XPath("//input[@type='submit' and @value='Thêm mới sản phẩm']"));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", submitButton);

                // 🟢 Bước 7: Chờ trang chuyển về Admin/Home
                WaitForUrl("Admin/Home", 10);

                // 🟢 Bước 8: Kiểm tra sản phẩm có xuất hiện trong bảng tại Admin/Home không
                IWebElement productCell = WaitForElement(By.XPath($"//table[contains(@class,'table-bordered')]//td[contains(text(), '{productName}')]"), 10);
                Assert.IsNotNull(productCell, "❌ Test Failed: Sản phẩm không xuất hiện trong bảng trên trang Admin/Home.");

                Console.WriteLine("✅ Test Passed: Sản phẩm đã được thêm thành công và hiển thị trên Admin/Home!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Lỗi trong quá trình kiểm thử: " + ex.Message);
                throw;
            }
        }

        [Test]
        public void AddProductNoQuantity()
        {
            try
            {
                // 🟢 Bước 1: Đăng nhập với tài khoản Admin
                Login("Admin@gmail.com", "12345678");

                // 🟢 Bước 2: Chờ và click vào nút 'Thêm mới'
                IWebElement addButton = wait.Until(d => d.FindElement(By.XPath("//p/button[@class='btn-default']/a")));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", addButton);

                // 🟢 Bước 3: Nhập thông tin sản phẩm nhưng bỏ trống số lượng
                string productName = "Điện thoại Xiaomi Redmi K20 Pro";
                driver.FindElement(By.Id("Tensp")).SendKeys(productName);
                driver.FindElement(By.Id("Giatien")).SendKeys("5000000");
                driver.FindElement(By.Id("Mota")).SendKeys("Sản phẩm hot");
                driver.FindElement(By.Id("Thesim")).SendKeys("2");
                driver.FindElement(By.Id("Bonhotrong")).SendKeys("128");
                driver.FindElement(By.Id("Ram")).SendKeys("8");

                // 🟢 Bước 4: Chọn trạng thái, hãng, hệ điều hành
                new SelectElement(driver.FindElement(By.Id("Sanphammoi"))).SelectByText("False");
                new SelectElement(driver.FindElement(By.Id("Mahang"))).SelectByText("Sam Sung");
                new SelectElement(driver.FindElement(By.Id("Mahdh"))).SelectByText("Android");

                // 🟢 Bước 5: Upload ảnh sản phẩm
                driver.FindElement(By.Id("Anhbia")).SendKeys("/Images/files/ss3.jpg");

                // 🟢 Bước 6: Nhấn nút 'Thêm mới sản phẩm'
                IWebElement submitButton = driver.FindElement(By.XPath("//input[@type='submit' and @value='Thêm mới sản phẩm']"));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", submitButton);

                // 🟢 Bước 7: Kiểm tra nếu có thông báo lỗi do bỏ trống số lượng
                try
                {
                    IWebElement errorMessage = wait.Until(d => d.FindElement(By.XPath("//div[contains(@class,'alert-danger') and contains(text(),'Số lượng không được để trống')]")));
                    Console.WriteLine("✅ Test Passed: Hệ thống không cho phép thêm sản phẩm khi bỏ trống số lượng.");
                    return; // Dừng test vì hệ thống đã hoạt động đúng
                }
                catch (WebDriverTimeoutException)
                {
                    // Nếu không tìm thấy thông báo lỗi, tiếp tục kiểm tra danh sách sản phẩm
                }

                // 🟢 Bước 8: Kiểm tra xem sản phẩm có bị thêm vào bảng Admin/Home hay không
                WaitForUrl("Admin/Home", 10);
                IWebElement productCell = WaitForElement(By.XPath($"//table[contains(@class,'table-bordered')]//td[contains(text(), '{productName}')]"), 5);

                if (productCell != null)
                {
                    Console.WriteLine("❌ Test Failed: Hệ thống cho phép thêm sản phẩm dù số lượng bị bỏ trống!");
                    Assert.Fail("Hệ thống không kiểm tra điều kiện số lượng.");
                }
                else
                {
                    Console.WriteLine("✅ Test Passed: Hệ thống không thêm sản phẩm khi thiếu số lượng.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Lỗi trong quá trình kiểm thử: " + ex.Message);
                throw;
            }
        }

        [Test]
        public void AddProductNegativeQuantity()
        {
            try
            {
                // 🟢 Bước 1: Đăng nhập với tài khoản Admin
                Login("Admin@gmail.com", "12345678");

                // 🟢 Bước 2: Chờ và click vào nút 'Thêm mới'
                IWebElement addButton = wait.Until(d => d.FindElement(By.XPath("//p/button[@class='btn-default']/a")));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", addButton);

                // 🟢 Bước 3: Nhập thông tin sản phẩm với số lượng âm
                string productName = "Điện thoại Xiaomi Redmi K20 Pro";
                driver.FindElement(By.Id("Tensp")).SendKeys(productName);
                driver.FindElement(By.Id("Giatien")).SendKeys("5000000");
                driver.FindElement(By.Id("Soluong")).SendKeys("-5"); // Nhập số lượng âm
                driver.FindElement(By.Id("Mota")).SendKeys("Sản phẩm hot");
                driver.FindElement(By.Id("Thesim")).SendKeys("2");
                driver.FindElement(By.Id("Bonhotrong")).SendKeys("128");
                driver.FindElement(By.Id("Ram")).SendKeys("8");

                // 🟢 Bước 4: Chọn trạng thái, hãng, hệ điều hành
                new SelectElement(driver.FindElement(By.Id("Sanphammoi"))).SelectByText("False");
                new SelectElement(driver.FindElement(By.Id("Mahang"))).SelectByText("Sam Sung");
                new SelectElement(driver.FindElement(By.Id("Mahdh"))).SelectByText("Android");

                // 🟢 Bước 5: Upload ảnh sản phẩm
                driver.FindElement(By.Id("Anhbia")).SendKeys("/Images/files/ss3.jpg");

                // 🟢 Bước 6: Nhấn nút 'Thêm mới sản phẩm'
                IWebElement submitButton = driver.FindElement(By.XPath("//input[@type='submit' and @value='Thêm mới sản phẩm']"));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", submitButton);

                // 🟢 Bước 7: Kiểm tra nếu có thông báo lỗi do số lượng âm
                try
                {
                    IWebElement errorMessage = wait.Until(d => d.FindElement(By.XPath("//div[contains(@class,'alert-danger') and contains(text(),'Số lượng phải là số dương')]")));
                    Console.WriteLine("✅ Test Passed: Hệ thống không cho phép thêm sản phẩm với số lượng âm.");
                    return; // Dừng test vì hệ thống đã hoạt động đúng
                }
                catch (WebDriverTimeoutException)
                {
                    // Nếu không tìm thấy thông báo lỗi, tiếp tục kiểm tra danh sách sản phẩm
                }

                // 🟢 Bước 8: Kiểm tra xem sản phẩm có bị thêm vào bảng Admin/Home hay không
                WaitForUrl("Admin/Home", 10);
                IWebElement productCell = WaitForElement(By.XPath($"//table[contains(@class,'table-bordered')]//td[contains(text(), '{productName}')]"), 5);

                if (productCell != null)
                {
                    Console.WriteLine("❌ Test Failed: Hệ thống cho phép thêm sản phẩm với số lượng âm!");
                    Assert.Fail("Hệ thống không kiểm tra điều kiện số lượng dương.");
                }
                else
                {
                    Console.WriteLine("✅ Test Passed: Hệ thống không thêm sản phẩm với số lượng âm.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Lỗi trong quá trình kiểm thử: " + ex.Message);
                throw;
            }
        }
        [Test]
        public void AddProductFractionalQuantity()
        {
            try
            {
                // 🟢 Bước 1: Đăng nhập với tài khoản Admin
                Login("Admin@gmail.com", "12345678");

                // 🟢 Bước 2: Chờ và click vào nút 'Thêm mới'
                IWebElement addButton = wait.Until(d => d.FindElement(By.XPath("//p/button[@class='btn-default']/a")));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", addButton);

                // 🟢 Bước 3: Nhập thông tin sản phẩm với số lượng phân số
                string productName = "Điện thoại Xiaomi Redmi K20 Pro";
                driver.FindElement(By.Id("Tensp")).SendKeys(productName);
                driver.FindElement(By.Id("Giatien")).SendKeys("5000000");
                driver.FindElement(By.Id("Soluong")).SendKeys("3.5"); // Nhập số lượng phân số
                driver.FindElement(By.Id("Mota")).SendKeys("Sản phẩm hot");
                driver.FindElement(By.Id("Thesim")).SendKeys("2");
                driver.FindElement(By.Id("Bonhotrong")).SendKeys("128");
                driver.FindElement(By.Id("Ram")).SendKeys("8");

                // 🟢 Bước 4: Chọn trạng thái, hãng, hệ điều hành
                new SelectElement(driver.FindElement(By.Id("Sanphammoi"))).SelectByText("False");
                new SelectElement(driver.FindElement(By.Id("Mahang"))).SelectByText("Sam Sung");
                new SelectElement(driver.FindElement(By.Id("Mahdh"))).SelectByText("Android");

                // 🟢 Bước 5: Upload ảnh sản phẩm
                driver.FindElement(By.Id("Anhbia")).SendKeys("/Images/files/ss3.jpg");

                // 🟢 Bước 6: Lưu URL trước khi nhấn submit
                string initialUrl = driver.Url;
                Console.WriteLine("🔍 URL trước khi submit: " + initialUrl);

                // 🟢 Bước 7: Nhấn nút 'Thêm mới sản phẩm'
                IWebElement submitButton = driver.FindElement(By.XPath("//input[@type='submit' and @value='Thêm mới sản phẩm']"));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", submitButton);

                // 🟢 Bước 8: Chờ một chút để kiểm tra URL thay đổi
                Thread.Sleep(3000); // Chờ để xem trang có chuyển hướng không

                // 🟢 Bước 9: Lấy URL sau khi submit
                string finalUrl = driver.Url;
                Console.WriteLine("🔍 URL sau khi submit: " + finalUrl);

                // 🟢 Bước 10: Kiểm tra nếu URL có thay đổi
                if (initialUrl != finalUrl && finalUrl.Contains("Admin/Home"))
                {
                    Console.WriteLine("❌ Test Failed: Hệ thống đã chuyển hướng.");
                    Assert.Fail("Hệ thống không kiểm tra số lượng sản phẩm và vẫn thêm sản phẩm.");
                }
                else
                {
                    Console.WriteLine("✅ Test Passed: Hệ thống đã không thêm được sản phẩm với số lượng phân số.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Lỗi trong quá trình kiểm thử: " + ex.Message);
                throw;
            }
        }

        [Test]
        public void AddProductQuantityWithOperation()
        {
            try
            {
                // 🟢 Bước 1: Đăng nhập với tài khoản Admin
                Login("Admin@gmail.com", "12345678");

                // 🟢 Bước 2: Chờ và click vào nút 'Thêm mới'
                IWebElement addButton = wait.Until(d => d.FindElement(By.XPath("//p/button[@class='btn-default']/a")));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", addButton);

                // 🟢 Bước 3: Nhập thông tin sản phẩm với số lượng phân số
                string productName = "Điện thoại Xiaomi Redmi K20 Pro";
                driver.FindElement(By.Id("Tensp")).SendKeys(productName);
                driver.FindElement(By.Id("Giatien")).SendKeys("5000000");
                driver.FindElement(By.Id("Soluong")).SendKeys("5+7"); // Nhập số lượng phân số
                driver.FindElement(By.Id("Mota")).SendKeys("Sản phẩm hot");
                driver.FindElement(By.Id("Thesim")).SendKeys("2");
                driver.FindElement(By.Id("Bonhotrong")).SendKeys("128");
                driver.FindElement(By.Id("Ram")).SendKeys("8");

                // 🟢 Bước 4: Chọn trạng thái, hãng, hệ điều hành
                new SelectElement(driver.FindElement(By.Id("Sanphammoi"))).SelectByText("False");
                new SelectElement(driver.FindElement(By.Id("Mahang"))).SelectByText("Sam Sung");
                new SelectElement(driver.FindElement(By.Id("Mahdh"))).SelectByText("Android");

                // 🟢 Bước 5: Upload ảnh sản phẩm
                driver.FindElement(By.Id("Anhbia")).SendKeys("/Images/files/ss3.jpg");

                // 🟢 Bước 6: Lưu URL trước khi nhấn submit
                string initialUrl = driver.Url;
                Console.WriteLine("🔍 URL trước khi submit: " + initialUrl);

                // 🟢 Bước 7: Nhấn nút 'Thêm mới sản phẩm'
                IWebElement submitButton = driver.FindElement(By.XPath("//input[@type='submit' and @value='Thêm mới sản phẩm']"));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", submitButton);

                // 🟢 Bước 8: Chờ một chút để kiểm tra URL thay đổi
                Thread.Sleep(3000); // Chờ để xem trang có chuyển hướng không

                // 🟢 Bước 9: Lấy URL sau khi submit
                string finalUrl = driver.Url;
                Console.WriteLine("🔍 URL sau khi submit: " + finalUrl);

                // 🟢 Bước 10: Kiểm tra nếu URL có thay đổi
                if (initialUrl != finalUrl && finalUrl.Contains("Admin/Home"))
                {
                    Console.WriteLine("❌ Test Failed: Hệ thống đã chuyển hướng.");
                    Assert.Fail("Hệ thống không kiểm tra số lượng sản phẩm và vẫn thêm sản phẩm.");
                }
                else
                {
                    Console.WriteLine("✅ Test Passed: Hệ thống đã không thêm được sản phẩm với số lượng chứa phép tính.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Lỗi trong quá trình kiểm thử: " + ex.Message);
                throw;
            }
        }

        [Test]
        public void AddProductBigQuantity()
        {
            try
            {
                // 🟢 Bước 1: Đăng nhập với tài khoản Admin
                Login("Admin@gmail.com", "12345678");

                // 🟢 Bước 2: Chờ và click vào nút 'Thêm mới'
                IWebElement addButton = wait.Until(d => d.FindElement(By.XPath("//p/button[@class='btn-default']/a")));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", addButton);

                // 🟢 Bước 3: Nhập thông tin sản phẩm với số lượng phân số
                string productName = "Điện thoại Xiaomi Redmi K20 Pro";
                driver.FindElement(By.Id("Tensp")).SendKeys(productName);
                driver.FindElement(By.Id("Giatien")).SendKeys("5000000");
                driver.FindElement(By.Id("Soluong")).SendKeys("500000000000000"); // Nhập số lượng phân số
                driver.FindElement(By.Id("Mota")).SendKeys("Sản phẩm hot");
                driver.FindElement(By.Id("Thesim")).SendKeys("2");
                driver.FindElement(By.Id("Bonhotrong")).SendKeys("128");
                driver.FindElement(By.Id("Ram")).SendKeys("8");

                // 🟢 Bước 4: Chọn trạng thái, hãng, hệ điều hành
                new SelectElement(driver.FindElement(By.Id("Sanphammoi"))).SelectByText("False");
                new SelectElement(driver.FindElement(By.Id("Mahang"))).SelectByText("Sam Sung");
                new SelectElement(driver.FindElement(By.Id("Mahdh"))).SelectByText("Android");

                // 🟢 Bước 5: Upload ảnh sản phẩm
                driver.FindElement(By.Id("Anhbia")).SendKeys("/Images/files/ss3.jpg");

                // 🟢 Bước 6: Lưu URL trước khi nhấn submit
                string initialUrl = driver.Url;
                Console.WriteLine("🔍 URL trước khi submit: " + initialUrl);

                // 🟢 Bước 7: Nhấn nút 'Thêm mới sản phẩm'
                IWebElement submitButton = driver.FindElement(By.XPath("//input[@type='submit' and @value='Thêm mới sản phẩm']"));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", submitButton);

                // 🟢 Bước 8: Chờ một chút để kiểm tra URL thay đổi
                Thread.Sleep(3000); // Chờ để xem trang có chuyển hướng không

                // 🟢 Bước 9: Lấy URL sau khi submit
                string finalUrl = driver.Url;
                Console.WriteLine("🔍 URL sau khi submit: " + finalUrl);

                // 🟢 Bước 10: Kiểm tra nếu URL có thay đổi
                if (initialUrl != finalUrl && finalUrl.Contains("Admin/Home"))
                {
                    Console.WriteLine("❌ Test Failed: Hệ thống đã chuyển hướng.");
                    Assert.Fail("Hệ thống không kiểm tra số lượng sản phẩm và vẫn thêm sản phẩm.");
                }
                else
                {
                    Console.WriteLine("✅ Test Passed: Hệ thống đã không thêm được sản phẩm với số lượng quá lớn.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Lỗi trong quá trình kiểm thử: " + ex.Message);
                throw;
            }
        }

        [Test]
        public void AddProductNoPrice()
        {
            try
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

                try
                {
                    IWebElement errorMessage = wait.Until(d => d.FindElement(By.XPath("//div[contains(@class,'alert-danger') and contains(text(),'Gía không được để trống')]")));
                    Console.WriteLine("✅ Test Passed: Hệ thống không cho phép thêm sản phẩm khi bỏ trống giá.");
                    return; // Dừng test vì hệ thống đã hoạt động đúng
                }
                catch (WebDriverTimeoutException)
                {
                }

                WaitForUrl("Admin/Home", 10);
                IWebElement productCell = WaitForElement(By.XPath($"//table[contains(@class,'table-bordered')]//td[contains(text(), '{productName}')]"), 5);

                if (productCell != null)
                {
                    Console.WriteLine("❌ Test Failed: Hệ thống cho phép thêm sản phẩm dù giá bị bỏ trống!");
                    Assert.Fail("Hệ thống không kiểm tra điều kiện số lượng.");
                }
                else
                {
                    Console.WriteLine("✅ Test Passed: Hệ thống không thêm sản phẩm khi thiếu giá.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Lỗi trong quá trình kiểm thử: " + ex.Message);
                throw;
            }
        }

        [Test]
        public void AddProductNegativePrice()
        {
            try
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

                try
                {
                    IWebElement errorMessage = wait.Until(d => d.FindElement(By.XPath("//div[contains(@class,'alert-danger') and contains(text(),'Gía phải là số dương')]")));
                    Console.WriteLine("✅ Test Passed: Hệ thống không cho phép thêm sản phẩm với giá âm.");
                    return; // Dừng test vì hệ thống đã hoạt động đúng
                }
                catch (WebDriverTimeoutException)
                {
                }

                WaitForUrl("Admin/Home", 10);
                IWebElement productCell = WaitForElement(By.XPath($"//table[contains(@class,'table-bordered')]//td[contains(text(), '{productName}')]"), 5);

                if (productCell != null)
                {
                    Console.WriteLine("❌ Test Failed: Hệ thống cho phép thêm sản phẩm với giá âm!");
                    Assert.Fail("Hệ thống không kiểm tra điều kiện số lượng dương.");
                }
                else
                {
                    Console.WriteLine("✅ Test Passed: Hệ thống không thêm sản phẩm với giá âm.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Lỗi trong quá trình kiểm thử: " + ex.Message);
                throw;
            }
        }

        [Test]
        public void AddProductFractionalPrice()
        {
            try
            {
                Login("Admin@gmail.com", "12345678");

                IWebElement addButton = wait.Until(d => d.FindElement(By.XPath("//p/button[@class='btn-default']/a")));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", addButton);

                string productName = "Điện thoại Xiaomi Redmi K20 Pro";
                driver.FindElement(By.Id("Tensp")).SendKeys(productName);
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
                Console.WriteLine("🔍 URL trước khi submit: " + initialUrl);

                IWebElement submitButton = driver.FindElement(By.XPath("//input[@type='submit' and @value='Thêm mới sản phẩm']"));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", submitButton);

                Thread.Sleep(3000); // Chờ để xem trang có chuyển hướng không

                string finalUrl = driver.Url;
                Console.WriteLine("🔍 URL sau khi submit: " + finalUrl);

                if (initialUrl != finalUrl && finalUrl.Contains("Admin/Home"))
                {
                    Console.WriteLine("❌ Test Failed: Hệ thống đã chuyển hướng.");
                    Assert.Fail("Hệ thống không kiểm tra giá sản phẩm và vẫn thêm sản phẩm.");
                }
                else
                {
                    Console.WriteLine("✅ Test Passed: Hệ thống đã không thêm được sản phẩm với giá phân số.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Lỗi trong quá trình kiểm thử: " + ex.Message);
                throw;
            }
        }

        [Test]
        public void AddProductPriceWithOperation()
        {
            try
            {
                Login("Admin@gmail.com", "12345678");

                IWebElement addButton = wait.Until(d => d.FindElement(By.XPath("//p/button[@class='btn-default']/a")));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", addButton);

                string productName = "Điện thoại Xiaomi Redmi K20 Pro";
                driver.FindElement(By.Id("Tensp")).SendKeys(productName);
                driver.FindElement(By.Id("Giatien")).SendKeys("5000000+1000000");
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
                Console.WriteLine("🔍 URL trước khi submit: " + initialUrl);

                IWebElement submitButton = driver.FindElement(By.XPath("//input[@type='submit' and @value='Thêm mới sản phẩm']"));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", submitButton);

                Thread.Sleep(3000); // Chờ để xem trang có chuyển hướng không

                string finalUrl = driver.Url;
                Console.WriteLine("🔍 URL sau khi submit: " + finalUrl);

                if (initialUrl != finalUrl && finalUrl.Contains("Admin/Home"))
                {
                    Console.WriteLine("❌ Test Failed: Hệ thống đã chuyển hướng.");
                    Assert.Fail("Hệ thống không kiểm tra giá sản phẩm và vẫn thêm sản phẩm.");
                }
                else
                {
                    Console.WriteLine("✅ Test Passed: Hệ thống đã không thêm được sản phẩm với giá chứa phép tính.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Lỗi trong quá trình kiểm thử: " + ex.Message);
                throw;
            }
        }

        [Test]
        public void AddProductSmallPrice()
        {
            try
            {
                Login("Admin@gmail.com", "12345678");

                IWebElement addButton = wait.Until(d => d.FindElement(By.XPath("//p/button[@class='btn-default']/a")));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", addButton);

                string productName = "Điện thoại Xiaomi Redmi K20 Pro";
                driver.FindElement(By.Id("Tensp")).SendKeys(productName);
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
                Console.WriteLine("🔍 URL trước khi submit: " + initialUrl);

                IWebElement submitButton = driver.FindElement(By.XPath("//input[@type='submit' and @value='Thêm mới sản phẩm']"));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", submitButton);

                Thread.Sleep(3000); // Chờ để xem trang có chuyển hướng không

                string finalUrl = driver.Url;
                Console.WriteLine("🔍 URL sau khi submit: " + finalUrl);

                if (initialUrl != finalUrl && finalUrl.Contains("Admin/Home"))
                {
                    Console.WriteLine("❌ Test Failed: Hệ thống đã chuyển hướng.");
                    Assert.Fail("Hệ thống không kiểm tra giá sản phẩm và vẫn thêm sản phẩm.");
                }
                else
                {
                    Console.WriteLine("✅ Test Passed: Hệ thống đã không thêm được sản phẩm với giá quá nhỏ.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Lỗi trong quá trình kiểm thử: " + ex.Message);
                throw;
            }
        }

        [Test]
        public void AddProductBigPrice()
        {
            try
            {
                Login("Admin@gmail.com", "12345678");

                IWebElement addButton = wait.Until(d => d.FindElement(By.XPath("//p/button[@class='btn-default']/a")));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", addButton);

                string productName = "Điện thoại Xiaomi Redmi K20 Pro";
                driver.FindElement(By.Id("Tensp")).SendKeys(productName);
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
                Console.WriteLine("🔍 URL trước khi submit: " + initialUrl);

                IWebElement submitButton = driver.FindElement(By.XPath("//input[@type='submit' and @value='Thêm mới sản phẩm']"));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", submitButton);

                Thread.Sleep(3000); // Chờ để xem trang có chuyển hướng không

                string finalUrl = driver.Url;
                Console.WriteLine("🔍 URL sau khi submit: " + finalUrl);

                if (initialUrl != finalUrl && finalUrl.Contains("Admin/Home"))
                {
                    Console.WriteLine("❌ Test Failed: Hệ thống đã chuyển hướng.");
                    Assert.Fail("Hệ thống không kiểm tra giá sản phẩm và vẫn thêm sản phẩm.");
                }
                else
                {
                    Console.WriteLine("✅ Test Passed: Hệ thống đã không thêm được sản phẩm với giá quá lớn.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Lỗi trong quá trình kiểm thử: " + ex.Message);
                throw;
            }
        }

        [Test]
        public void AddProduct5Sim()
        {
            try
            {
                Login("Admin@gmail.com", "12345678");

                IWebElement addButton = wait.Until(d => d.FindElement(By.XPath("//p/button[@class='btn-default']/a")));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", addButton);

                string productName = "Điện thoại Xiaomi Redmi K20 Pro";
                driver.FindElement(By.Id("Tensp")).SendKeys(productName);
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
                Console.WriteLine("🔍 URL trước khi submit: " + initialUrl);

                IWebElement submitButton = driver.FindElement(By.XPath("//input[@type='submit' and @value='Thêm mới sản phẩm']"));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", submitButton);

                Thread.Sleep(3000); // Chờ để xem trang có chuyển hướng không

                string finalUrl = driver.Url;
                Console.WriteLine("🔍 URL sau khi submit: " + finalUrl);

                if (initialUrl != finalUrl && finalUrl.Contains("Admin/Home"))
                {
                    Console.WriteLine("❌ Test Failed: Hệ thống đã chuyển hướng.");
                    Assert.Fail("Hệ thống không kiểm tra sim sản phẩm và vẫn thêm sản phẩm.");
                }
                else
                {
                    Console.WriteLine("✅ Test Passed: Hệ thống đã không thêm được sản phẩm với sim lớn hơn 4.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Lỗi trong quá trình kiểm thử: " + ex.Message);
                throw;
            }
        }

        [Test]
        public void AddProductNoSim()
        {
            try
            {
                Login("Admin@gmail.com", "12345678");

                IWebElement addButton = wait.Until(d => d.FindElement(By.XPath("//p/button[@class='btn-default']/a")));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", addButton);

                string productName = "Điện thoại Xiaomi Redmi K20 Pro";
                driver.FindElement(By.Id("Tensp")).SendKeys(productName);
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
                Console.WriteLine("🔍 URL trước khi submit: " + initialUrl);

                IWebElement submitButton = driver.FindElement(By.XPath("//input[@type='submit' and @value='Thêm mới sản phẩm']"));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", submitButton);

                Thread.Sleep(3000); // Chờ để xem trang có chuyển hướng không

                string finalUrl = driver.Url;
                Console.WriteLine("🔍 URL sau khi submit: " + finalUrl);

                if (initialUrl != finalUrl && finalUrl.Contains("Admin/Home"))
                {
                    Console.WriteLine("❌ Test Failed: Hệ thống đã chuyển hướng.");
                    Assert.Fail("Hệ thống không kiểm tra sim sản phẩm và vẫn thêm sản phẩm.");
                }
                else
                {
                    Console.WriteLine("✅ Test Passed: Hệ thống đã không thêm được sản phẩm với sim bỏ trống.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Lỗi trong quá trình kiểm thử: " + ex.Message);
                throw;
            }
        }

        [Test]
        public void AddProductNegativeSim()
        {
            try
            {
                Login("Admin@gmail.com", "12345678");

                IWebElement addButton = wait.Until(d => d.FindElement(By.XPath("//p/button[@class='btn-default']/a")));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", addButton);

                string productName = "Điện thoại Xiaomi Redmi K20 Pro";
                driver.FindElement(By.Id("Tensp")).SendKeys(productName);
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
                Console.WriteLine("🔍 URL trước khi submit: " + initialUrl);

                IWebElement submitButton = driver.FindElement(By.XPath("//input[@type='submit' and @value='Thêm mới sản phẩm']"));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", submitButton);

                Thread.Sleep(3000); // Chờ để xem trang có chuyển hướng không

                string finalUrl = driver.Url;
                Console.WriteLine("🔍 URL sau khi submit: " + finalUrl);

                if (initialUrl != finalUrl && finalUrl.Contains("Admin/Home"))
                {
                    Console.WriteLine("❌ Test Failed: Hệ thống đã chuyển hướng.");
                    Assert.Fail("Hệ thống không kiểm tra sim sản phẩm và vẫn thêm sản phẩm.");
                }
                else
                {
                    Console.WriteLine("✅ Test Passed: Hệ thống đã không thêm được sản phẩm với sim nhỏ hơn hoặc bằng 0.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Lỗi trong quá trình kiểm thử: " + ex.Message);
                throw;
            }
        }

        [Test]
        public void AddProductSimWithOperation()
        {
            try
            {
                Login("Admin@gmail.com", "12345678");

                IWebElement addButton = wait.Until(d => d.FindElement(By.XPath("//p/button[@class='btn-default']/a")));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", addButton);

                string productName = "Điện thoại Xiaomi Redmi K20 Pro";
                driver.FindElement(By.Id("Tensp")).SendKeys(productName);
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
                Console.WriteLine("🔍 URL trước khi submit: " + initialUrl);

                IWebElement submitButton = driver.FindElement(By.XPath("//input[@type='submit' and @value='Thêm mới sản phẩm']"));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", submitButton);

                Thread.Sleep(3000); // Chờ để xem trang có chuyển hướng không

                string finalUrl = driver.Url;
                Console.WriteLine("🔍 URL sau khi submit: " + finalUrl);

                if (initialUrl != finalUrl && finalUrl.Contains("Admin/Home"))
                {
                    Console.WriteLine("❌ Test Failed: Hệ thống đã chuyển hướng.");
                    Assert.Fail("Hệ thống không kiểm tra sim sản phẩm và vẫn thêm sản phẩm.");
                }
                else
                {
                    Console.WriteLine("✅ Test Passed: Hệ thống đã không thêm được sản phẩm với sim chứa phép tính.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Lỗi trong quá trình kiểm thử: " + ex.Message);
                throw;
            }
        }

        [Test]
        public void AddProductNegativeInternalMemory()
        {
            try
            {
                Login("Admin@gmail.com", "12345678");

                IWebElement addButton = wait.Until(d => d.FindElement(By.XPath("//p/button[@class='btn-default']/a")));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", addButton);

                string productName = "Điện thoại Xiaomi Redmi K20 Pro";
                driver.FindElement(By.Id("Tensp")).SendKeys(productName);
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
                Console.WriteLine("🔍 URL trước khi submit: " + initialUrl);

                IWebElement submitButton = driver.FindElement(By.XPath("//input[@type='submit' and @value='Thêm mới sản phẩm']"));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", submitButton);

                Thread.Sleep(3000); // Chờ để xem trang có chuyển hướng không

                string finalUrl = driver.Url;
                Console.WriteLine("🔍 URL sau khi submit: " + finalUrl);

                if (initialUrl != finalUrl && finalUrl.Contains("Admin/Home"))
                {
                    Console.WriteLine("❌ Test Failed: Hệ thống đã chuyển hướng.");
                    Assert.Fail("Hệ thống không kiểm tra bộ nhớ trong sản phẩm và vẫn thêm sản phẩm.");
                }
                else
                {
                    Console.WriteLine("✅ Test Passed: Hệ thống đã không thêm được sản phẩm với bộ nhớ trong nhỏ hơn hoặc bằng 0.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Lỗi trong quá trình kiểm thử: " + ex.Message);
                throw;
            }
        }

        [Test]
        public void AddProductNoInternalMemory()
        {
            try
            {
                Login("Admin@gmail.com", "12345678");

                IWebElement addButton = wait.Until(d => d.FindElement(By.XPath("//p/button[@class='btn-default']/a")));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", addButton);

                string productName = "Điện thoại Xiaomi Redmi K20 Pro";
                driver.FindElement(By.Id("Tensp")).SendKeys(productName);
                driver.FindElement(By.Id("Giatien")).SendKeys("600000000000");
                driver.FindElement(By.Id("Soluong")).SendKeys("5000");
                driver.FindElement(By.Id("Mota")).SendKeys("Sản phẩm hot");
                driver.FindElement(By.Id("Ram")).SendKeys("8");

                new SelectElement(driver.FindElement(By.Id("Sanphammoi"))).SelectByText("False");
                new SelectElement(driver.FindElement(By.Id("Mahang"))).SelectByText("Sam Sung");
                new SelectElement(driver.FindElement(By.Id("Mahdh"))).SelectByText("Android");

                driver.FindElement(By.Id("Anhbia")).SendKeys("/Images/files/ss3.jpg");

                string initialUrl = driver.Url;
                Console.WriteLine("🔍 URL trước khi submit: " + initialUrl);

                IWebElement submitButton = driver.FindElement(By.XPath("//input[@type='submit' and @value='Thêm mới sản phẩm']"));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", submitButton);

                Thread.Sleep(3000); // Chờ để xem trang có chuyển hướng không

                string finalUrl = driver.Url;
                Console.WriteLine("🔍 URL sau khi submit: " + finalUrl);

                if (initialUrl != finalUrl && finalUrl.Contains("Admin/Home"))
                {
                    Console.WriteLine("❌ Test Failed: Hệ thống đã chuyển hướng.");
                    Assert.Fail("Hệ thống không kiểm tra bộ nhớ trong sản phẩm và vẫn thêm sản phẩm.");
                }
                else
                {
                    Console.WriteLine("✅ Test Passed: Hệ thống đã không thêm được sản phẩm với bộ nhớ trong bỏ trống.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Lỗi trong quá trình kiểm thử: " + ex.Message);
                throw;
            }
        }

        [Test]
        public void AddProductInternalMemoryWithOperation()
        {
            try
            {
                Login("Admin@gmail.com", "12345678");

                IWebElement addButton = wait.Until(d => d.FindElement(By.XPath("//p/button[@class='btn-default']/a")));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", addButton);

                string productName = "Điện thoại Xiaomi Redmi K20 Pro";
                driver.FindElement(By.Id("Tensp")).SendKeys(productName);
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
                Console.WriteLine("🔍 URL trước khi submit: " + initialUrl);

                IWebElement submitButton = driver.FindElement(By.XPath("//input[@type='submit' and @value='Thêm mới sản phẩm']"));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", submitButton);

                Thread.Sleep(3000); // Chờ để xem trang có chuyển hướng không

                string finalUrl = driver.Url;
                Console.WriteLine("🔍 URL sau khi submit: " + finalUrl);

                if (initialUrl != finalUrl && finalUrl.Contains("Admin/Home"))
                {
                    Console.WriteLine("❌ Test Failed: Hệ thống đã chuyển hướng.");
                    Assert.Fail("Hệ thống không kiểm tra bộ nhớ trong sản phẩm và vẫn thêm sản phẩm.");
                }
                else
                {
                    Console.WriteLine("✅ Test Passed: Hệ thống đã không thêm được sản phẩm với bộ nhớ trong chứa phép tính.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Lỗi trong quá trình kiểm thử: " + ex.Message);
                throw;
            }
        }

        [Test]
        public void AddProductFractionalInternalMemory()
        {
            try
            {
                Login("Admin@gmail.com", "12345678");

                IWebElement addButton = wait.Until(d => d.FindElement(By.XPath("//p/button[@class='btn-default']/a")));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", addButton);

                string productName = "Điện thoại Xiaomi Redmi K20 Pro";
                driver.FindElement(By.Id("Tensp")).SendKeys(productName);
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
                Console.WriteLine("🔍 URL trước khi submit: " + initialUrl);

                IWebElement submitButton = driver.FindElement(By.XPath("//input[@type='submit' and @value='Thêm mới sản phẩm']"));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", submitButton);

                Thread.Sleep(3000); // Chờ để xem trang có chuyển hướng không

                string finalUrl = driver.Url;
                Console.WriteLine("🔍 URL sau khi submit: " + finalUrl);

                if (initialUrl != finalUrl && finalUrl.Contains("Admin/Home"))
                {
                    Console.WriteLine("❌ Test Failed: Hệ thống đã chuyển hướng.");
                    Assert.Fail("Hệ thống không kiểm tra bộ nhớ trong sản phẩm và vẫn thêm sản phẩm.");
                }
                else
                {
                    Console.WriteLine("✅ Test Passed: Hệ thống đã không thêm được sản phẩm với bộ nhớ trong là phân số.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Lỗi trong quá trình kiểm thử: " + ex.Message);
                throw;
            }
        }

        [Test]
        public void AddProductNoRam()
        {
            try
            {
                Login("Admin@gmail.com", "12345678");

                IWebElement addButton = wait.Until(d => d.FindElement(By.XPath("//p/button[@class='btn-default']/a")));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", addButton);

                string productName = "Điện thoại Xiaomi Redmi K20 Pro";
                driver.FindElement(By.Id("Tensp")).SendKeys(productName);
                driver.FindElement(By.Id("Giatien")).SendKeys("600000000000");
                driver.FindElement(By.Id("Soluong")).SendKeys("5000");
                driver.FindElement(By.Id("Bonhotrong")).SendKeys("128");
                driver.FindElement(By.Id("Mota")).SendKeys("Sản phẩm hot");

                new SelectElement(driver.FindElement(By.Id("Sanphammoi"))).SelectByText("False");
                new SelectElement(driver.FindElement(By.Id("Mahang"))).SelectByText("Sam Sung");
                new SelectElement(driver.FindElement(By.Id("Mahdh"))).SelectByText("Android");

                driver.FindElement(By.Id("Anhbia")).SendKeys("/Images/files/ss3.jpg");

                string initialUrl = driver.Url;
                Console.WriteLine("🔍 URL trước khi submit: " + initialUrl);

                IWebElement submitButton = driver.FindElement(By.XPath("//input[@type='submit' and @value='Thêm mới sản phẩm']"));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", submitButton);

                Thread.Sleep(3000); // Chờ để xem trang có chuyển hướng không

                string finalUrl = driver.Url;
                Console.WriteLine("🔍 URL sau khi submit: " + finalUrl);

                if (initialUrl != finalUrl && finalUrl.Contains("Admin/Home"))
                {
                    Console.WriteLine("❌ Test Failed: Hệ thống đã chuyển hướng.");
                    Assert.Fail("Hệ thống không kiểm tra RAM sản phẩm và vẫn thêm sản phẩm.");
                }
                else
                {
                    Console.WriteLine("✅ Test Passed: Hệ thống đã không thêm được sản phẩm với RAM bỏ trống.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Lỗi trong quá trình kiểm thử: " + ex.Message);
                throw;
            }
        }

        [Test]
        public void AddProductNegativeRam()
        {
            try
            {
                Login("Admin@gmail.com", "12345678");

                IWebElement addButton = wait.Until(d => d.FindElement(By.XPath("//p/button[@class='btn-default']/a")));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", addButton);

                string productName = "Điện thoại Xiaomi Redmi K20 Pro";
                driver.FindElement(By.Id("Tensp")).SendKeys(productName);
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
                Console.WriteLine("🔍 URL trước khi submit: " + initialUrl);

                IWebElement submitButton = driver.FindElement(By.XPath("//input[@type='submit' and @value='Thêm mới sản phẩm']"));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", submitButton);

                Thread.Sleep(3000); // Chờ để xem trang có chuyển hướng không

                string finalUrl = driver.Url;
                Console.WriteLine("🔍 URL sau khi submit: " + finalUrl);

                if (initialUrl != finalUrl && finalUrl.Contains("Admin/Home"))
                {
                    Console.WriteLine("❌ Test Failed: Hệ thống đã chuyển hướng.");
                    Assert.Fail("Hệ thống không kiểm tra RAM sản phẩm và vẫn thêm sản phẩm.");
                }
                else
                {
                    Console.WriteLine("✅ Test Passed: Hệ thống đã không thêm được sản phẩm với RAM nhỏ hơn hoặc bằng 0.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Lỗi trong quá trình kiểm thử: " + ex.Message);
                throw;
            }
        }

        [Test]
        public void AddProductFractionalRam()
        {
            try
            {
                Login("Admin@gmail.com", "12345678");

                IWebElement addButton = wait.Until(d => d.FindElement(By.XPath("//p/button[@class='btn-default']/a")));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", addButton);

                string productName = "Điện thoại Xiaomi Redmi K20 Pro";
                driver.FindElement(By.Id("Tensp")).SendKeys(productName);
                driver.FindElement(By.Id("Giatien")).SendKeys("600000000000");
                driver.FindElement(By.Id("Soluong")).SendKeys("5000");
                driver.FindElement(By.Id("Mota")).SendKeys("Sản phẩm hot");
                driver.FindElement(By.Id("Thesim")).SendKeys("2");
                driver.FindElement(By.Id("Bonhotrong")).SendKeys("128");
                driver.FindElement(By.Id("Ram")).SendKeys("0,83");

                new SelectElement(driver.FindElement(By.Id("Sanphammoi"))).SelectByText("False");
                new SelectElement(driver.FindElement(By.Id("Mahang"))).SelectByText("Sam Sung");
                new SelectElement(driver.FindElement(By.Id("Mahdh"))).SelectByText("Android");
                driver.FindElement(By.Id("Anhbia")).SendKeys("/Images/files/ss3.jpg");

                string initialUrl = driver.Url;
                Console.WriteLine("🔍 URL trước khi submit: " + initialUrl);

                IWebElement submitButton = driver.FindElement(By.XPath("//input[@type='submit' and @value='Thêm mới sản phẩm']"));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", submitButton);

                Thread.Sleep(3000); // Chờ để xem trang có chuyển hướng không

                string finalUrl = driver.Url;
                Console.WriteLine("🔍 URL sau khi submit: " + finalUrl);

                if (initialUrl != finalUrl && finalUrl.Contains("Admin/Home"))
                {
                    Console.WriteLine("❌ Test Failed: Hệ thống đã chuyển hướng.");
                    Assert.Fail("Hệ thống không kiểm tra RAM sản phẩm và vẫn thêm sản phẩm.");
                }
                else
                {
                    Console.WriteLine("✅ Test Passed: Hệ thống đã không thêm được sản phẩm với RAM là phân số.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Lỗi trong quá trình kiểm thử: " + ex.Message);
                throw;
            }
        }

        [Test]
        public void AddProductRamWithOperation()
        {
            try
            {
                Login("Admin@gmail.com", "12345678");

                IWebElement addButton = wait.Until(d => d.FindElement(By.XPath("//p/button[@class='btn-default']/a")));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", addButton);

                string productName = "Điện thoại Xiaomi Redmi K20 Pro";
                driver.FindElement(By.Id("Tensp")).SendKeys(productName);
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
                Console.WriteLine("🔍 URL trước khi submit: " + initialUrl);

                IWebElement submitButton = driver.FindElement(By.XPath("//input[@type='submit' and @value='Thêm mới sản phẩm']"));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", submitButton);

                Thread.Sleep(3000); // Chờ để xem trang có chuyển hướng không

                string finalUrl = driver.Url;
                Console.WriteLine("🔍 URL sau khi submit: " + finalUrl);

                if (initialUrl != finalUrl && finalUrl.Contains("Admin/Home"))
                {
                    Console.WriteLine("❌ Test Failed: Hệ thống đã chuyển hướng.");
                    Assert.Fail("Hệ thống không kiểm tra RAM sản phẩm và vẫn thêm sản phẩm.");
                }
                else
                {
                    Console.WriteLine("✅ Test Passed: Hệ thống đã không thêm được sản phẩm với RAM chứa phép tính.");
                }
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
