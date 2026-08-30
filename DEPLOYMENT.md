# AnatoliaRoot Deployment Runbook

Bu belge yeni veya boş bir SQL Server veritabanına güvenli kurulum sırasını tanımlar. Uygulama runtime sırasında veritabanı ya da tablo oluşturmaz ve migration uygulamaz.

## Sunucu Gereksinimi

Uygulama `.NET 8` hedefler. IIS/Plesk sunucusunda güncel .NET 8 Hosting Bundle kurulu olmalı ve uygulama pool yeniden başlatılmalıdır. Eski .NET Core 3.1 runtime uygulama için artık kullanılmaz.

## Güvenlik Ön Koşulları

1. Repository veya mesajlaşma kanallarında paylaşılmış tüm SQL ve harici servis parolalarını değiştirin.
2. Connection string ve API anahtarlarını `appsettings.json` içine yazmayın.
3. Migration hesabı ile uygulama runtime hesabını mümkünse ayırın.
4. Migration hesabına geçici DDL yetkisi, runtime hesabına yalnızca gerekli okuma/yazma yetkileri verin.
5. SQL Server erişimini firewall veya IP allowlist ile sınırlandırın.
6. Kalıcı hedef olarak `Encrypt=True;TrustServerCertificate=False` ve doğrulanan bir SQL Server sertifikası kullanın.

## Zorunlu Yapılandırma

Production ortamı aşağıdaki değerleri secret store veya hosting environment alanından sağlamalıdır:

```text
ConnectionStrings__DefaultConnection
Cloudinary__CloudName
Cloudinary__ApiKey
Cloudinary__ApiSecret
ExternalApis__ExchangeRate__ApiKey
ExternalApis__GoldPrice__ApiKey
```

Sistemde yalnızca admin hesabı bulunur; public kayıt veya farklı kullanıcı rolü yoktur. İlk ve tek admin oluşturulurken geçici olarak şunlar sağlanır:

```text
AdminBootstrap__Username
AdminBootstrap__Password
```

Bootstrap parolası en az 12 karakter olmalıdır. Bu iki değişken admin oluşturulduktan hemen sonra kaldırılmalıdır.

## Migration Hazırlığı

Komutlar repository kök dizininde çalıştırılır:

```powershell
dotnet tool restore
dotnet restore ".\AnatoliaRoot-V2.sln"
```

Repository, proje ile uyumlu `dotnet-ef 8.0.30` sürümünü `.config/dotnet-tools.json` üzerinden sabitler.

Connection string yalnızca mevcut terminal oturumu veya güvenli deployment ortamı için tanımlanmalıdır:

```powershell
$env:ConnectionStrings__DefaultConnection = "<secret-store-degeri>"
```

## Script İncelemesi

Migration doğrudan uygulanmadan önce idempotent SQL script üretilmelidir:

```powershell
dotnet ef migrations script --idempotent `
  --project ".\AnatoliaRoot-V2\AnatoliaRoot-V2.csproj" `
  --startup-project ".\AnatoliaRoot-V2\AnatoliaRoot-V2.csproj" `
  --output "$env:TEMP\anatoliaroot-migration.sql"
```

Script içinde aşağıdaki tabloların oluştuğu doğrulanmalıdır:

```text
Categories
Users
Products
ExchangeRates
GoldPrices
__EFMigrationsHistory
```

## Migration Uygulama

Yeni boş veritabanında migration hesabıyla çalıştırılır:

```powershell
dotnet ef database update `
  --project ".\AnatoliaRoot-V2\AnatoliaRoot-V2.csproj" `
  --startup-project ".\AnatoliaRoot-V2\AnatoliaRoot-V2.csproj"
```

Uygulama başlatılmadan önce bekleyen migration bulunmadığı doğrulanır:

```powershell
dotnet ef migrations list `
  --project ".\AnatoliaRoot-V2\AnatoliaRoot-V2.csproj" `
  --startup-project ".\AnatoliaRoot-V2\AnatoliaRoot-V2.csproj"
```

`EnsureCreated`, elle `__EFMigrationsHistory` kaydı veya production başlangıcında otomatik migration kullanılmamalıdır.

## Hangfire Şeması

EF migration'ları Hangfire tablolarını oluşturmaz. İlk application başlangıcında Hangfire SQL storage kendi şemasını hazırlar.

1. İlk başlangıç için SQL hesabına yalnızca gerekli süre boyunca schema oluşturma yetkisi verin.
2. Uygulamayı tek instance olarak başlatın.
3. Hangfire tablolarının oluştuğunu ve dashboard'un yalnızca admin kullanıcıya açıldığını doğrulayın.
4. Geçici DDL yetkilerini kaldırın.
5. Normal runtime izinleriyle uygulamayı yeniden başlatın.

## İlk Admin

Migration tamamlandıktan ve `Users` tablosunun boş olduğu doğrulandıktan sonra:

```powershell
$env:AdminBootstrap__Username = "<admin-kullanici-adi>"
$env:AdminBootstrap__Password = "<secret-store-degeri>"

dotnet run `
  --project ".\AnatoliaRoot-V2\AnatoliaRoot-V2.csproj" `
  --no-launch-profile `
  -- --bootstrap-admin
```

Komut şu durumlarda güvenli şekilde başarısız olur:

- Veritabanına bağlanılamıyorsa
- Uygulanmamış migration varsa
- `Users` tablosunda kayıt varsa
- Kullanıcı adı geçersizse
- Parola 12 karakterden kısaysa

Başarılı işlemden sonra geçici değerleri kaldırın:

```powershell
Remove-Item Env:AdminBootstrap__Username
Remove-Item Env:AdminBootstrap__Password
```

## Uygulama Başlangıç Kontrolü

Normal başlangıçta uygulama:

1. Connection string'in tanımlı olduğunu doğrular.
2. SQL Server bağlantısını doğrular.
3. Bekleyen EF migration bulunmadığını doğrular.
4. Herhangi bir kontrol başarısızsa trafik almadan kapanır.

Uygulama migration uygulamaz ve şema durumunu değiştirmez.

## Smoke Test

Deployment sonrasında aşağıdakiler doğrulanmalıdır:

```text
/urunler                         anonim erişilebilir
/Product/Create                 anonim kullanıcı login'e yönlendirilir
/Product/AdminIndex             anonim kullanıcı login'e yönlendirilir
/kategoriler                    anonim kullanıcı login'e yönlendirilir
/hangfire                       anonim kullanıcı erişemez
/Account/Register               404 döner
/Account/Login                  admin girişini kabul eder
```

Admin kullanıcıyla kategori ve ürün create/edit/delete işlemleri test edilmelidir. Kur ve altın job'ları kontrollü olarak bir kez çalıştırılıp kayıt ve hata logları incelenmelidir.

## Rollback

1. Deployment öncesinde SQL backup veya hosting snapshot alın.
2. Önceki uygulama artifact'ini ayrı ve immutable olarak saklayın.
3. Migration rollback için otomatik `database update <old-migration>` çalıştırmayın.
4. Şema problemi durumunda trafiği kapatın ve doğrulanmış backup'tan geri dönün.
5. Cutover sonrasında veri yazıldıysa geri dönüşten önce veri uzlaştırması yapın.
