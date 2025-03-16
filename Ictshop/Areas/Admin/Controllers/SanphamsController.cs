using System;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

class SeleniumTest
{
    static void Main()
    {
        // Khởi tạo WebDriver (Chrome)
        IWebDriver driver = new ChromeDriver();

        try
        {
            // Mở trang quản lý sản phẩm
            driver.Navigate().GoToUrl("https://localhost:44322/Admin/Home");
            driver.Manage().Window.Maximize();
            Thread.Sleep(2000); // Đợi trang load

            // Tìm kiếm sản phẩm bằng ô tìm kiếm
            IWebElement searchBox = driver.FindElement(By.CssSelector("input[type='search']"));
            searchBox.SendKeys("Apple Iphone 4");
            searchBox.SendKeys(Keys.Enter);
            Thread.Sleep(2000); // Chờ cập nhật kết quả

            // Nhấn vào nút "Sửa" sản phẩm đầu tiên
            var editButtons = driver.FindElements(By.XPath("//button[contains(text(),'Sửa')]"));
            if (editButtons.Count > 0)
            {
                editButtons[0].Click();
                Thread.Sleep(2000);
                Console.WriteLine("Mở form sửa sản phẩm thành công.");
            }

            // Nhấn vào nút "Chi tiết"
            var detailButtons = driver.FindElements(By.XPath("//button[contains(text(),'Chi tiết')]"));
            if (detailButtons.Count > 0)
            {
                detailButtons[0].Click();
                Thread.Sleep(2000);
                Console.WriteLine("Xem chi tiết sản phẩm thành công.");
            }

            // Nhấn vào nút "Xóa" và xác nhận cảnh báo
            var deleteButtons = driver.FindElements(By.XPath("//button[contains(text(),'Xóa')]"));
            if (deleteButtons.Count > 0)
            {
                deleteButtons[0].Click();
                Thread.Sleep(2000);

                // Xác nhận cảnh báo
                driver.SwitchTo().Alert().Accept();
                Console.WriteLine("Xóa sản phẩm thành công.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Lỗi xảy ra: " + ex.Message);
        }
        finally
        {
            // Đóng trình duyệt
            Thread.Sleep(3000);
            driver.Quit();
        }
    }
}
