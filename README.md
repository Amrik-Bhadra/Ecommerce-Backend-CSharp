# Multi-Vendor E-Commerce Backend Engine

A robust, enterprise-grade multi-vendor e-commerce backend built with **C# 10** and **.NET 10** following clean code practices, asynchronous programming execution pipelines, and rigid database-level security patterns.

---

## 🏛️ Architectural Overview

The project is structured around the **MVC (Model-View-Controller)** pattern and separated into highly decoupled monolithic service layers to promote high system maintainability and scalability:

* **Controllers (API Layer):** Exposes secure RESTful HTTP endpoints, manages incoming model routing states, and implements strict role-based access controllers.
* **Services (Business Logic Layer):** Orchestrates the underlying operational domain logic, calculates transaction values, enforces system constraints, and tracks state lifecycles.
* **Repositories (Data Access Layer):** Uses **Entity Framework Core (EF Core)** to run highly responsive, non-blocking asynchronous database operations against a relational SQL Server instance.
* **Data Transfer Objects (DTOs):** Shields core Entity Framework entities from over-posting vulnerabilities and applies metadata data annotations attributes to fail-fast on malformed payloads.

---

## ✨ Key System Features

### 🔄 1. Non-Blocking Asynchronous Core Pipeline
- Extensively utilizes `async/await` patterns and `Task<IActionResult>` wrappers seamlessly across all architectural layers.
- Eliminates thread starvation under highly concurrent operational loads, maximizing web-server throughput.

### 🧱 2. Atomic Onboarding via SQL Transactions
- Wraps multi-stage user creation models within an explicit transactional block (`BeginTransactionAsync`).
- Automatically provisions isolated relational profiles (`CustomerProfile` or `SellerProfile`) safely during user signup context initialization.

### 🛡️ 3. Request Claim Processing & Security Overrides
- Discards untrusted frontend tracking parameters from critical data payloads.
- Extracts identities context natively via server-side claims decoding using `User.FindFirstValue(ClaimTypes.NameIdentifier)`.
- Validates structural owner relationships before editing sensitive child profiles, neutralizing ID spoofing or cross-user resource tampering vulnerabilities.

### 📉 4. Automated Marketplace Rules & Soft-Deletes
- Implements layered LINQ evaluation filters across product marketplace lookup endpoints.
- Instantly isolates or conceals product catalogs from open-market routing indices if a vendor's shop gets deactivated or flagged as unverified.

### 🎯 5. Unified System Response Format
- Formulates a generic wrapper envelope utility (`ApiResponse<T>`) to output symmetrical JSON structures.
- Pairs with dedicated custom Business Exception models and global handlers to encapsulate runtime bugs cleanly away from client interfaces.

---

## 🗃️ Entity Relationship Diagram (ERD)

The database schema isolates user identities while connecting relational entities through precise cascading control constraints:

<img width="1094" height="840" alt="Database Entity Relationship Diagram" src="https://github.com/user-attachments/assets/ce0bdc0c-4c27-42bd-94c0-4642deb66103" />

---

## 🔌 API Route Reference Table

All protected routes require a secure **HttpOnly, Secure, and SameSite** cookie payload generated dynamically upon a successful authentication handshake.

### 🔐 Authentication Module
| HTTP Method | Route Endpoint | Target Payload / DTO Requirements | Access Level |
| :--- | :--- | :--- | :--- |
| **POST** | `/api/auth/register` | `Email, Username, Password, Role (Customer/Seller), ShopName` | Anonymous |
| **POST** | `/api/auth/login` | `Email, Password` | Anonymous |
| **POST** | `/api/auth/forgot-password` | `Email` | Anonymous |
| **POST** | `/api/auth/verify-otp` | `Email, Otp` | Anonymous |
| **POST** | `/api/auth/reset-password` | `Email, NewPassword` | Anonymous |
| **POST** | `/api/auth/logout` | None | Authenticated |

### 👤 Profile & Address Management Module
| HTTP Method | Route Endpoint | Target Payload / DTO Requirements | Access Level |
| :--- | :--- | :--- | :--- |
| **GET** | `/api/userprofile/my-profile` | None | Customer |
| **PUT** | `/api/userprofile/update-profile`| `FullName, PhoneNumber, Gender, DateOfBirth` | Customer |
| **GET** | `/api/userprofile/my-addresses` | None | Customer |
| **POST** | `/api/userprofile/add-address` | `AddressLine1, AddressLine2, City, State, Pincode, AddressType` | Customer |
| **PUT** | `/api/userprofile/update-address/{id}`| `AddressLine1, AddressLine2, City, State, Pincode, AddressType` | Customer (Owner) |
| **DELETE**| `/api/userprofile/delete-address/{id}`| None | Customer (Owner) |

### 🏪 Product Inventory Module
| HTTP Method | Route Endpoint | Target Payload / DTO Requirements | Access Level |
| :--- | :--- | :--- | :--- |
| **GET** | `/api/products` | None *(Filters out inactive seller catalogs)* | Public |
| **GET** | `/api/products/{id}` | Route Identity Key Validation | Public |
| **GET** | `/api/products/my-shop` | None *(Fetches complete seller warehouse items)* | Seller |
| **POST** | `/api/products` | `ProductName, Description, Price, StockQuantity` | Seller |

### 🛒 Cart & Checkout Module
| HTTP Method | Route Endpoint | Target Payload / DTO Requirements | Access Level |
| :--- | :--- | :--- | :--- |
| **GET** | `/api/cart/summary` | None | Customer |
| **POST** | `/api/cart/add-to-cart` | `ProductId, Quantity` | Customer |
| **PUT** | `/api/cart/update-quantity` | `ProductId, Action (Increment/Decrement)` | Customer |
| **DELETE**| `/api/cart/remove-from-cart/{id}`| Route Parameter Identification Key | Customer |
| **DELETE**| `/api/cart/clear-cart` | None | Customer |
| **POST** | `/api/orders/checkout` | `AddressId` *(Evaluated securely via owner validation check)*| Customer |
| **GET** | `/api/orders/history` | None | Customer |

---

## ⚙️ Tech Stack Used

- **Language Core:** C# 10
- **Framework Suite:** .NET 10 (ASP.NET Core Web API)
- **Data Mapper:** Entity Framework Core
- **Database Engine:** SQL Server
- **Authentication Engine:** JWT (JSON Web Tokens) with Secure Cookie Context Encryption
- **Tooling Engine:** Git, Postman Execution Suites, SSMS
