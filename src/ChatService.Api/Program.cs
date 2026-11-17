var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Dữ liệu mô phỏng trong bộ nhớ (sẽ thay bằng Database ở các chương sau)
var conversations = new List<ConversationDto>();
var messages = new List<MessageDto>();

app.MapGet("/", () => Results.Ok(new { service = "ChatService.Api", status = "running" }));

// 1. Lấy danh sách phiên hỗ trợ
app.MapGet("/api/conversations", () => Results.Ok(conversations));

// 2. Tra cứu một phiên cụ thể
app.MapGet("/api/conversations/{id:guid}", (Guid id) => {
    var conv = conversations.FirstOrDefault(c => c.Id == id);
    return conv is null 
        ? Results.NotFound(new ApiError("NOT_FOUND", "Không tìm thấy phiên chat.")) 
        : Results.Ok(conv);
});

// 3. Tạo phiên hỗ trợ mới
app.MapPost("/api/conversations", (CreateConversationRequest request) => {
    // Validation tại biên hệ thống
    if (string.IsNullOrWhiteSpace(request.CustomerName)) {
        return Results.BadRequest(new ApiError("INVALID_DATA", "Tên khách hàng không được để trống."));
    }

    var newConv = new ConversationDto(
        Id: Guid.NewGuid(),
        CustomerName: request.CustomerName.Trim(),
        Status: "Waiting",
        CreatedAt: DateTimeOffset.UtcNow
    );
    conversations.Add(newConv);

    // Trả về 201 Created kèm URI để đọc lại tài nguyên
    return Results.Created($"/api/conversations/{newConv.Id}", newConv);
});

// 4. Lấy lịch sử tin nhắn của phiên
app.MapGet("/api/conversations/{id:guid}/messages", (Guid id) => {
    var conv = conversations.FirstOrDefault(c => c.Id == id);
    if (conv is null) return Results.NotFound(new ApiError("NOT_FOUND", "Không tìm thấy phiên chat."));
    
    var convMessages = messages.Where(m => m.ConversationId == id).ToList();
    return Results.Ok(convMessages);
});

// 5. Đóng phiên hỗ trợ
app.MapPost("/api/conversations/{id:guid}/close", (Guid id) => {
    var conv = conversations.FirstOrDefault(c => c.Id == id);
    if (conv is null) return Results.NotFound(new ApiError("NOT_FOUND", "Không tìm thấy phiên chat."));
    
    if (conv.Status == "Closed") return Results.BadRequest(new ApiError("ALREADY_CLOSED", "Phiên này đã được đóng trước đó."));

    // Cập nhật trạng thái
    var updatedConv = conv with { Status = "Closed" };
    conversations.Remove(conv);
    conversations.Add(updatedConv);

    return Results.Ok(updatedConv);
});

app.Run();

// --- Định nghĩa các DTO ---
record ConversationDto(Guid Id, string CustomerName, string Status, DateTimeOffset CreatedAt);
record MessageDto(Guid Id, Guid ConversationId, string Sender, string Content, DateTimeOffset SentAt);
record CreateConversationRequest(string CustomerName);
record ApiError(string Code, string Message);