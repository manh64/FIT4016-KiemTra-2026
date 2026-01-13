// File: Program.cs

using Microsoft.EntityFrameworkCore;
using SchoolManagement.Data;
// Thêm namespace này
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ADD SERVICES
builder.Services.AddControllers(); // Đổi từ AddControllersWithViews thành AddControllers nếu chỉ làm API
builder.Services.AddEndpointsApiExplorer(); // Bắt buộc cho Swagger

// Cấu hình Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "School Management API", Version = "v1" });
});

// DB Context
builder.Services.AddDbContext<SchoolDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

var app = builder.Build();

// SEED DATA
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<SchoolDbContext>();
    // Tự động tạo DB nếu chưa có (Tránh lỗi nếu máy giáo viên chưa chạy migration)
    context.Database.EnsureCreated();
    DataSeeder.Seed(context);
}

// HTTP PIPELINE
if (app.Environment.IsDevelopment())
{
    // Bật Swagger UI
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers(); // Map Controller API

app.Run();