using System.Net;
using System.Net.Http.Json;

Console.WriteLine("--- CHAT CLIENT (RESTful API) ---");
using var http = new HttpClient { BaseAddress = new Uri("http://localhost:5111") }; // Nhớ đổi port khớp với máy bạn

// 1. Tạo một phiên chat hợp lệ
Console.WriteLine("\n[1] Gửi yêu cầu tạo phiên chat mới...");
var createReq = new CreateConversationRequest("Tống Thị Quỳnh Nga");
var createRes = await http.PostAsJsonAsync("/api/conversations", createReq);

if (createRes.StatusCode == HttpStatusCode.Created) {
    var newConv = await createRes.Content.ReadFromJsonAsync<ConversationDto>();
    Console.WriteLine($"=> THÀNH CÔNG: Đã tạo phiên [{newConv?.Id}], Khách hàng: {newConv?.CustomerName}, Trạng thái: {newConv?.Status}");
    
    // 2. Đọc lại danh sách phiên
    Console.WriteLine("\n[2] Đọc danh sách các phiên đang mở...");
    var list = await http.GetFromJsonAsync<List<ConversationDto>>("/api/conversations");
    foreach (var c in list ?? []) {
        Console.WriteLine($"  - {c.Id} | {c.CustomerName} | {c.Status}");
    }
}

// 3. Cố tình gửi sai dữ liệu để quan sát lỗi 400 Bad Request
Console.WriteLine("\n[3] Thử gửi yêu cầu lỗi (Tên rỗng)...");
var badReq = new CreateConversationRequest("");
var badRes = await http.PostAsJsonAsync("/api/conversations", badReq);

if (!badRes.IsSuccessStatusCode) {
    var error = await badRes.Content.ReadFromJsonAsync<ApiError>();
    Console.WriteLine($"=> BỊ TỪ CHỐI (Mã {(int)badRes.StatusCode}): {error?.Code} - {error?.Message}");
}

// --- DTO dùng chung cho Client ---
record ConversationDto(Guid Id, string CustomerName, string Status, DateTimeOffset CreatedAt);
record CreateConversationRequest(string CustomerName);
record ApiError(string Code, string Message);