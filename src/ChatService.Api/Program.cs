var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Điểm cuối 1: Kiểm tra trạng thái API đang chạy
app.MapGet("/", () => "Chat Service API dang hoat dong tot!");

// Điểm cuối 2: Trả về danh sách phiên chat (dữ liệu mô phỏng trong bộ nhớ)
app.MapGet("/api/conversations", () => new[] {
    new { Id = 1, Status = "Active", Customer = "Nguyen Van A" },
    new { Id = 2, Status = "Waiting", Customer = "Tran Thi B" }
});

app.Run();