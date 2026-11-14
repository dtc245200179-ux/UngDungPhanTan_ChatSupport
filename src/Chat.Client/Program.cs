using System.Net.Http.Json;

Console.WriteLine("--- KHOI DONG CHAT CLIENT ---");
Console.WriteLine("Dang goi API lay danh sach phien chat...");

// Tạo HttpClient và chỉ định địa chỉ của API (nhớ dùng đúng port 5111 của bạn)
using var client = new HttpClient();
client.BaseAddress = new Uri("http://localhost:5111");

try 
{
    // Gọi GET /api/conversations
    var response = await client.GetAsync("/api/conversations");
    
    // Kiểm tra xem phản hồi có thành công (status code 200-299) không
    response.EnsureSuccessStatusCode(); 

    // Đọc kết quả dạng chuỗi JSON và in ra màn hình
    var conversations = await response.Content.ReadAsStringAsync();
    Console.WriteLine("\n[Thanh cong] Ket qua tra ve tu Chat Service API:");
    Console.WriteLine(conversations);
}
catch (Exception ex)
{
    Console.WriteLine($"\n[Loi] Khong the ket noi den API: {ex.Message}");
}