# 購物車系統
這是以購物流程為主題的資料庫系統設計專題。專案使用 Visual Studio Community 開發 ASP.NET Core MVC 網站，透過 Entity Framework Core 連接 SQL Server 資料庫，實作商品管理與購物車操作。

## 專案功能
商品頁面可以查看商品列表，並新增、修改或刪除商品。購物車頁面可以將商品加入購物車、調整數量及移除商品；重複加入同一商品時，購物車中的數量會增加。
按下結帳後，程式會先檢查商品庫存。若庫存足夠，就扣除庫存、清空購物車並更新購物車狀態；若庫存不足，則顯示提示並取消這次結帳操作。

## 系統架構
專案採用 MVC 架構：Controller 處理商品與購物車操作，View 顯示網頁，Model 定義使用者、商品、購物車及購物車項目的資料。資料存取由 Entity Framework Core 的 `AppDbContext` 處理。

## 使用技術
C#、ASP.NET Core MVC、Entity Framework Core、SQL Server。

## 專案展示
[Demo 影片](https://youtu.be/kkuoht5sxno)

## 執行說明
專案以 .NET 10 為目標框架，資料庫連線設定在 `ShoppingCart/appsettings.json`。
