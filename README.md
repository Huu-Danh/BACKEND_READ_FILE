# ✈️ BACKEND_READ_FILE (.NET 8 Web API)

> Dự án đọc file Word (.docx) để trích xuất thông tin vé máy bay, bao gồm mã đặt chỗ, hành khách, chuyến bay, thông tin liên hệ...  
> Viết bằng **C# (.NET 8)** — dùng trong bài test phỏng vấn.

---

## 🚀 Mục lục

- [Giới thiệu](#-giới-thiệu)
- [Công nghệ sử dụng](#-công-nghệ-sử-dụng)
- [Cấu trúc thư mục](#-cấu-trúc-thư-mục)
- [Cách chạy dự án](#️-cách-chạy-dự-án)
- [API Endpoint](#-api-endpoint)
- [Ví dụ phản hồi (Response)](#-ví-dụ-phản-hồi-response)
- [Lưu ý kỹ thuật](#️-lưu-ý-kỹ-thuật)
- [Tác giả](#-tác-giả)

---

## 📖 Giới thiệu

Dự án **BACKEND_READ_FILE** là một Web API cho phép:

- Nhận file Word (`.docx`) từ frontend hoặc đường dẫn.
- Đọc nội dung file (bao gồm cả text trong bảng).
- Phân tích & trích xuất dữ liệu: mã đặt chỗ (Booking Code), hành khách, chuyến bay, liên hệ, ngày đặt, v.v.
- Trả về kết quả dưới dạng JSON.

Mục tiêu: Giúp tự động hóa việc đọc dữ liệu từ các vé điện tử, phục vụ ứng dụng quản lý booking hoặc check-in tự động.

---

## 🧠 Công nghệ sử dụng

| Thành phần                       | Phiên bản / Mục đích                   |
| -------------------------------- | -------------------------------------- |
| **.NET 8 Web API**               | Nền tảng backend                       |
| **Swashbuckle.AspNetCore 6.6.2** | Sinh tài liệu Swagger/OpenAPI          |
| **DocumentFormat.OpenXml**       | Đọc file `.docx` không cần cài Word    |
| **CORS**                         | Cho phép frontend (React) truy cập API |
| **Regex / LINQ**                 | Phân tích và lọc dữ liệu từ file       |

---

## 📁 Cấu trúc thư mục

```
BACKEND_READ_FILE/
│
├── Controllers/
│   └── ParseController.cs        # API Upload & Parse file
│
├── Models/
│   ├── TicketModels.cs           # Model kết quả
│
│
├── Services/
│   ├── IWordParserService.cs     # Interface service đọc file
│   └── WordParserService.cs      # Xử lý đọc & phân tích file Word
│
├── Program.cs                    # Cấu hình .NET 8, Swagger, CORS
```

---

## ⚙️ Cách chạy dự án

### 1️⃣ Clone hoặc copy source

```bash
git clone https://github.com/yourusername/BACKEND_READ_FILE.git
cd BACKEND_READ_FILE
```

### 2️⃣ Cài đặt package cần thiết

```bash
dotnet restore
```

### 3️⃣ Build project

```bash
dotnet build
```

### 4️⃣ Chạy API

```bash
dotnet run
```

### 5️⃣ Mở Swagger UI

👉 [https://localhost:7053](https://localhost:7053)

---

## 🌐 API Endpoint

### **1. POST /api/parse/upload**

> Upload file Word (`.docx`) từ client.

**Request:**

- `Content-Type: multipart/form-data`
- Body:
  - `file`: File `.docx` cần đọc

**Response:**

```json
{
  "fileName": "VeVJ1.docx",
  "bookingCode": "SVE22W",
  "contactPhone": "0909641857",
  "contactEmail": "quipq.fit@vietravel.com",
  "bookingDate": "17/05/2022",
  "bookerName": "BUI, HUYEN THANH",
  "passengers": [
    { "firstName": "HUYEN THANH", "lastName": "BUI" },
    { "firstName": "THE TOAN", "lastName": "TRAN" }
  ],
  "flights": [
    {
      "flightNumber": "VJ864",
      "date": "21/09/2022",
      "fareClass": "Eco",
      "departureTimeAndPlace": "22:40 - Ho Chi Minh (SGN)",
      "arrivalTimeAndPlace": "05:45 - Seoul (ICN)"
    },
    {
      "flightNumber": "VJ863",
      "date": "25/09/2022",
      "fareClass": "Eco",
      "departureTimeAndPlace": "11:40 - Seoul (ICN)",
      "arrivalTimeAndPlace": "14:55 - Ho Chi Minh (SGN)"
    }
  ]
}
```

---

### **2. POST /api/parse/by-path**

> Đọc file `.docx` từ đường dẫn trên server.

**Request body (JSON):**

```json
{
  "path": "C:DataVeVJ1.docx"
}
```

**Response:** (giống `/upload`)

---

## 🧪 Ví dụ gọi API từ React (frontend)

> 💡 Cần bật CORS trong backend:
>
> ```csharp
> builder.Services.AddCors(options =>
> {
>     options.AddPolicy("AllowFrontend", policy =>
>     {
>         policy.WithOrigins("http://localhost:3000")
>               .AllowAnyHeader()
>               .AllowAnyMethod();
>     });
> });
> app.UseCors("AllowFrontend");
> ```

---

## ⚠️ Lưu ý kỹ thuật

| Mục                                 | Mô tả                                             |
| ----------------------------------- | ------------------------------------------------- |
| **File Word phải là `.docx`**       | Không hỗ trợ `.doc` (Office 97-2003)              |
| **API đọc được cả text trong bảng** | Duyệt tất cả `Paragraph` kể cả trong `TableCell`  |
| **Regex linh hoạt**                 | Có thể tùy chỉnh theo format vé máy bay khác nhau |
| **Loại bỏ hành khách trùng lặp**    | Dựa trên `Họ, Tên`                                |
| **Không cần Microsoft Word**        | Dùng `DocumentFormat.OpenXml` thuần túy           |

---

## 👤 Tác giả

**Nguyễn Tấn Hữu Danh**  
💼 _Backend Developer_  
📧 *your.nguyentanhuudanh@gmail.com*  
🌐 _GitHub:_ [github.com/Huu-Danh](https://github.com/Huu-Danh)

>
