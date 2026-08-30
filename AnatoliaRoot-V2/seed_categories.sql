SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

USE [anat4927_root6060]
GO

-- Önce ana kategorileri ekleyelim (ParentCategoryId = NULL)
INSERT INTO [Categories] ([Name], [ParentCategoryId]) VALUES ('Peynir Çeşitleri', NULL);
INSERT INTO [Categories] ([Name], [ParentCategoryId]) VALUES ('Süt ve Süt İçecekleri', NULL);
INSERT INTO [Categories] ([Name], [ParentCategoryId]) VALUES ('Tereyağı ve Kaymak', NULL);
INSERT INTO [Categories] ([Name], [ParentCategoryId]) VALUES ('Yoğurt ve Ayran', NULL);
INSERT INTO [Categories] ([Name], [ParentCategoryId]) VALUES ('Çökelek ve Lor', NULL);
INSERT INTO [Categories] ([Name], [ParentCategoryId]) VALUES ('Yöresel Tulum Peynirleri', NULL);
INSERT INTO [Categories] ([Name], [ParentCategoryId]) VALUES ('Fermente Süt Ürünleri', NULL);
INSERT INTO [Categories] ([Name], [ParentCategoryId]) VALUES ('Keçi ve Koyun Ürünleri', NULL);
INSERT INTO [Categories] ([Name], [ParentCategoryId]) VALUES ('Organik Kahvaltılıklar', NULL);
INSERT INTO [Categories] ([Name], [ParentCategoryId]) VALUES ('Hediyelik Yöresel Paketler', NULL);
GO

-- Şimdi eklenen ana kategorilerin ID'lerine göre alt kategorileri ekleyelim

-- 1. Peynir Çeşitleri (ID: 1)
INSERT INTO [Categories] ([Name], [ParentCategoryId]) VALUES ('Taze Kaşar Peyniri', 1);
INSERT INTO [Categories] ([Name], [ParentCategoryId]) VALUES ('Eski Kaşar Peyniri', 1);
INSERT INTO [Categories] ([Name], [ParentCategoryId]) VALUES ('Beyaz Peynir (İnek)', 1);

-- 2. Süt ve Süt İçecekleri (ID: 2)
INSERT INTO [Categories] ([Name], [ParentCategoryId]) VALUES ('Günlük Çiğ İnek Sütü', 2);
INSERT INTO [Categories] ([Name], [ParentCategoryId]) VALUES ('Pastörize Süt', 2);
INSERT INTO [Categories] ([Name], [ParentCategoryId]) VALUES ('Kutulu Çiftlik Sütü', 2);

-- 3. Tereyağı ve Kaymak (ID: 3)
INSERT INTO [Categories] ([Name], [ParentCategoryId]) VALUES ('Geleneksel Yayık Tereyağı', 3);
INSERT INTO [Categories] ([Name], [ParentCategoryId]) VALUES ('Köy Kaymağı', 3);
INSERT INTO [Categories] ([Name], [ParentCategoryId]) VALUES ('Tavalık Süt Yağı', 3);

-- 4. Yoğurt ve Ayran (ID: 4)
INSERT INTO [Categories] ([Name], [ParentCategoryId]) VALUES ('Geleneksel Taş Yoğurt', 4);
INSERT INTO [Categories] ([Name], [ParentCategoryId]) VALUES ('Köy Ayranı', 4);
INSERT INTO [Categories] ([Name], [ParentCategoryId]) VALUES ('Süzme Yoğurt', 4);

-- 5. Çökelek ve Lor (ID: 5)
INSERT INTO [Categories] ([Name], [ParentCategoryId]) VALUES ('Kurutulmuş Çökelek', 5);
INSERT INTO [Categories] ([Name], [ParentCategoryId]) VALUES ('Tuzlu Lor Peyniri', 5);
INSERT INTO [Categories] ([Name], [ParentCategoryId]) VALUES ('Öksürük Otu Çökeleği', 5);

-- 6. Yöresel Tulum Peynirleri (ID: 6)
INSERT INTO [Categories] ([Name], [ParentCategoryId]) VALUES ('Erzincan Tulum Peyniri', 6);
INSERT INTO [Categories] ([Name], [ParentCategoryId]) VALUES ('Çökelekli Deri Tulum', 6);
INSERT INTO [Categories] ([Name], [ParentCategoryId]) VALUES ('İspir Tulumu', 6);

-- 7. Fermente Süt Ürünleri (ID: 7)
INSERT INTO [Categories] ([Name], [ParentCategoryId]) VALUES ('Ev Yapımı Kefir', 7);
INSERT INTO [Categories] ([Name], [ParentCategoryId]) VALUES ('Kurut (Keşk)', 7);

-- 8. Keçi ve Koyun Ürünleri (ID: 8)
INSERT INTO [Categories] ([Name], [ParentCategoryId]) VALUES ('%100 Keçi Peyniri', 8);
INSERT INTO [Categories] ([Name], [ParentCategoryId]) VALUES ('Koyun Beyaz Peyniri', 8);
INSERT INTO [Categories] ([Name], [ParentCategoryId]) VALUES ('Keçi Sütü', 8);

-- 9. Organik Kahvaltılıklar (ID: 9)
INSERT INTO [Categories] ([Name], [ParentCategoryId]) VALUES ('Çiftlik Yumurtası', 9);
INSERT INTO [Categories] ([Name], [ParentCategoryId]) VALUES ('Doğal Köy Balı', 9);

-- 10. Hediyelik Yöresel Paketler (ID: 10)
INSERT INTO [Categories] ([Name], [ParentCategoryId]) VALUES ('Anadolu Kahvaltı Sepeti', 10);
INSERT INTO [Categories] ([Name], [ParentCategoryId]) VALUES ('Peynir Tadım Seti', 10);
GO
